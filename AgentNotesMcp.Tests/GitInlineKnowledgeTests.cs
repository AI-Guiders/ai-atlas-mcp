using System.Diagnostics;
using AgentNotes.Core;

namespace AgentNotesMcp.Tests;

/// <summary>
/// ADR-027: inline git versioning of knowledge writes.
/// These tests exercise the real git flow in a temp repository:
/// write → path-scoped commit; external dirty edit → write refused.
/// They no-op (return) when git is not available on the machine.
/// </summary>
public sealed class GitInlineKnowledgeTests
{
    [Fact]
    public void WriteKnowledgeFile_CommitsToGit()
    {
        using var root = CreateGitKbRoot();
        if (root is null) return;
        using var runtime = AgentNotesTestToml.InstallForRoot(root.Path);
        var storage = new NotesStorage();

        Assert.Equal("OK", storage.WriteKnowledgeFile(root.Path, "git-flow.md", "# v1\n"));
        Assert.Equal("MCP write: knowledge/git-flow.md", LastCommitMessage(root.Path));

        Assert.Equal("OK", storage.UpsertKnowledgeSection(root.Path, "git-flow.md", "sec-v1", "body\n"));
        Assert.Equal("MCP upsert section sec-v1: knowledge/git-flow.md", LastCommitMessage(root.Path));

        // No .revisions snapshots are created anymore.
        Assert.False(Directory.Exists(Path.Combine(root.Path, "knowledge", ".revisions")));
    }

    [Fact]
    public void WriteKnowledgeFile_RefusesWhenFileDirtyFromExternalEdits()
    {
        using var root = CreateGitKbRoot();
        if (root is null) return;
        using var runtime = AgentNotesTestToml.InstallForRoot(root.Path);
        var storage = new NotesStorage();

        Assert.Equal("OK", storage.WriteKnowledgeFile(root.Path, "dirty.md", "# committed\n"));

        // External edit that nobody commits.
        File.WriteAllText(Path.Combine(root.Path, "knowledge", "dirty.md"), "# external edit\n");

        var ex = Assert.Throws<InvalidOperationException>(() =>
            storage.WriteKnowledgeFile(root.Path, "dirty.md", "# mcp overwrite\n"));
        Assert.Contains("uncommitted external edits", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void DeleteKnowledgeFile_CommitsTheDeletion()
    {
        using var root = CreateGitKbRoot();
        if (root is null) return;
        using var runtime = AgentNotesTestToml.InstallForRoot(root.Path);
        var storage = new NotesStorage();

        Assert.Equal("OK", storage.WriteKnowledgeFile(root.Path, "gone.md", "# bye\n"));
        Assert.Equal("OK", storage.DeleteKnowledgeFile(root.Path, "gone.md"));
        Assert.Equal("MCP delete file: knowledge/gone.md", LastCommitMessage(root.Path));
        Assert.False(File.Exists(Path.Combine(root.Path, "knowledge", "gone.md")));
    }

    /// <summary>Temp KB root with a real git repo (identity configured locally). Null when git is unavailable.</summary>
    private static TempGitRoot? CreateGitKbRoot()
    {
        var path = Path.Combine(Path.GetTempPath(), "AgentNotesMcpTests", "gitkb", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Path.Combine(path, "knowledge", "work", "local"));

        if (Git(path, "init") != 0) { TryCleanup(path); return null; }
        if (Git(path, "config", "user.name", "mcp-test") != 0) { TryCleanup(path); return null; }
        if (Git(path, "config", "user.email", "mcp-test@example.invalid") != 0) { TryCleanup(path); return null; }

        return new TempGitRoot(path);
    }

    private static string LastCommitMessage(string repoPath)
    {
        var output = GitCapture(repoPath, "log", "-1", "--pretty=%s");
        return output.Trim();
    }

    private static int Git(string cwd, params string[] args)
    {
        var psi = new ProcessStartInfo("git")
        {
            WorkingDirectory = cwd,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        foreach (var arg in args)
            psi.ArgumentList.Add(arg);
        using var process = Process.Start(psi)!;
        process.WaitForExit(15_000);
        return process.ExitCode;
    }

    private static string GitCapture(string cwd, params string[] args)
    {
        var psi = new ProcessStartInfo("git")
        {
            WorkingDirectory = cwd,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        foreach (var arg in args)
            psi.ArgumentList.Add(arg);
        using var process = Process.Start(psi)!;
        process.WaitForExit(15_000);
        return process.StandardOutput.ReadToEnd();
    }

    private static void TryCleanup(string path)
    {
        try
        {
            if (Directory.Exists(path))
                Directory.Delete(path, recursive: true);
        }
        catch
        {
            // best effort — temp dir
        }
    }

    internal sealed class TempGitRoot : IDisposable
    {
        internal string Path { get; }

        internal TempGitRoot(string path) => Path = path;

        public void Dispose()
        {
            // .git objects may still be memory-mapped on Windows: retry briefly.
            for (var attempt = 0; attempt < 3; attempt++)
            {
                try
                {
                    if (Directory.Exists(Path))
                        Directory.Delete(Path, recursive: true);
                    return;
                }
                catch
                {
                    Thread.Sleep(200);
                }
            }
        }
    }
}
