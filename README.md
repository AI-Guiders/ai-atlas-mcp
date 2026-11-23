# AI Atlas MCP (`ai-atlas-mcp`, formerly `agent-notes-mcp`)

**[Model Context Protocol (MCP)](https://modelcontextprotocol.io) Server for Anti-Confabulation Epistemic Guardrails (`L0`), Hot Context Routing (`route_context`), and [Open Knowledge Format (OKF)](https://github.com/open-knowledge-format/open-knowledge-format) Knowledge Bases**

Paired with the public knowledge base **[`AI-Guiders/ai-welcome-atlas`](https://github.com/AI-Guiders/ai-welcome-atlas)**, **AI Atlas MCP** transforms frontier LLMs from fluent text generators into rigorous scientific and engineering peers that verify claims via formal tools (such as the **Lean 4** theorem prover in [`AI-Guiders/scientific-papers`](https://github.com/AI-Guiders/scientific-papers) and [`AI-Guiders/discrete-quantum-mechanics`](https://github.com/AI-Guiders/discrete-quantum-mechanics)) before speaking.

---

## Why Scientists & Experimentalists Need `ai-atlas-mcp` + `ai-welcome-atlas`

Standard LLMs are trained with RLHF to answer rapidly and confidently from parametric weights, which frequently leads to **confabulation** (plausible-sounding unverified equations or hallucinated physical parameters). When connected to **`ai-atlas-mcp`** and **`ai-welcome-atlas`**, the AI agent receives:
1. **L0 Epistemic Contracts (`epistemic-default-distrust`, `search-before-answer`):** every recalled formula or parameter is treated as an unverified hypothesis until checked by a compiler (`lake build` in Lean 4), script, or primary source with an explicit trace.
2. **Demand-Driven Context Routing (`route_context`, `read_knowledge_file` with `outline` first):** prevents context-window pollution by loading only the relevant domain cards (`knowledge/worlds/`, `knowledge/domains/`) and operational playbooks.
3. **Bilateral Personal Contour Sovereignty (OKF v0.3):** clean separation between the read-only public atlas (`o`) and the researcher's personal/laboratory contour (`u`).

### Quick Start (English)

```bash
# 1. Clone the public knowledge atlas and the MCP server:
git clone https://github.com/AI-Guiders/ai-welcome-atlas.git
git clone https://github.com/AI-Guiders/ai-atlas-mcp.git

# 2. Build the MCP server (.NET 8+ / .NET 10+):
cd ai-atlas-mcp
dotnet build
dotnet publish AgentNotesMcp.csproj -c Release -o publish
```

Copy and edit `config/agent-notes-mcp.toml` to point `[knowledge.roots]` to your local clone of `ai-welcome-atlas`, then register the server in your AI client (`mcp.json` / `opencode.jsonc` / Cursor / Claude Code):

```json
{
  "mcpServers": {
    "agent-notes": {
      "command": "D:\\ai-atlas-mcp\\publish\\AgentNotesMcp.exe",
      "args": ["--config", "D:/ai-atlas-mcp/publish/agent-notes-mcp.toml"],
      "env": {}
    }
  }
}
```

### The `AI-Guiders` Scientific & Engineering Ecosystem

| Repository | Role |
| :--- | :--- |
| **[`AI-Guiders/ai-atlas-mcp`](https://github.com/AI-Guiders/ai-atlas-mcp)** | This repository: MCP server for L0 anti-confabulation routing and OKF knowledge management. |
| **[`AI-Guiders/ai-welcome-atlas`](https://github.com/AI-Guiders/ai-welcome-atlas)** | Public OKF knowledge base (`knowledge/`), epistemic contracts, and scientific world cards. |
| **[`AI-Guiders/scientific-papers`](https://github.com/AI-Guiders/scientific-papers)** | Series I (`paper-1`..`paper-4`) & Series II (`paper-5`..`paper-8`) papers and standalone Lean 4 kernel [`RotatorCalculusPaper.lean`](https://github.com/AI-Guiders/scientific-papers/blob/main/RotatorCalculusPaper.lean) (`thm1`–`thm17`, `0 sorryAx`, `0 Classical.choice`). |
| **[`AI-Guiders/discrete-quantum-mechanics`](https://github.com/AI-Guiders/discrete-quantum-mechanics)** | Full 8-chapter academic monograph, natural sciences course, `lean/MtCarrier/` formalization, and cleanroom photovoltaic troubleshooting protocols (`M1`–`M4`, `F1`–`F6`). |
| **[`AI-Guiders/guiders-physics`](https://github.com/AI-Guiders/guiders-physics)** | `.NET` digital twin and interactive carrier-impedance simulator. |

---

## Документация на русском языке (AI Atlas MCP / `agent-notes-mcp`)

[MCP](https://modelcontextprotocol.io)-сервер для **hot-контекста** (`agent-notes.md`) и **слоя `knowledge/`** (чтение/запись карточек, мета-контрактов L0 и плейбуков в стандарте Open Knowledge Format). Версия **2.2.0** (Core **AIGuiders.AgentNotes.Core** 2.2.0, `.NET 11`, git-native history по ADR-027, multi-root read-only) настраивается через **локальный TOML** (`--config`).

### Быстрый старт

```bash
git clone https://github.com/AI-Guiders/ai-atlas-mcp.git
cd ai-atlas-mcp
dotnet build
dotnet publish AgentNotesMcp.csproj -c Release -o publish
```

Скопируй и отредактируй пример конфига: **`config/agent-notes-mcp.toml`** (пути `[knowledge.roots]`, `[workspace]`). После `publish-and-deploy.ps1` тот же файл попадает рядом с exe.

В **`mcp.json`**:

```json
{
  "mcpServers": {
    "agent-notes": {
      "command": "D:\\agent-notes-mcp\\AgentNotesMcp.exe",
      "args": ["--config", "D:/agent-notes-mcp/agent-notes-mcp.toml"],
      "env": {}
    }
  }
}
```

Без **`--config`** процесс завершится с ошибкой (fail fast). Переменная **`AGENT_NOTES_CONFIG`** — альтернатива пути к TOML.

Публичный атлас базы знаний (только чтение) — **[`AI-Guiders/ai-welcome-atlas`](https://github.com/AI-Guiders/ai-welcome-atlas)**. Подробнее по тулам — **[docs/MCP-TOOLS.md](docs/MCP-TOOLS.md)**.

### Лицензия

Software: [MIT](LICENSE) ([текст OSI](https://opensource.org/license/MIT)) · Ethical use: [declaration](https://github.com/AI-Guiders/licensing/blob/main/docs/ethical-use.md)

### Справочник по документации

| Что | Где |
|-----|-----|
| Имена тулов, аргументы, примеры | **[docs/MCP-TOOLS.md](docs/MCP-TOOLS.md)** и `mcp-tools.manifest.json` |
| Локальный TOML (`--config`) | **[docs/adr/014-agent-notes-local-settings-toml-v1.md](docs/adr/014-agent-notes-local-settings-toml-v1.md)** |
| Правила для `.cursor/rules` (Integrity POST, канон KB) | **[docs/cursor-rules-examples.md](docs/cursor-rules-examples.md)** |
| ADR по MCP и KB | **[docs/adr/](docs/adr/)** (канон также в репо **ai-welcome-atlas**, `knowledge/adr/`) |
| Чистая установка (новый пользователь) | Playbook: `knowledge/domains/agent-operations/playbook-knowledge-stack-clean-setup-v1.md`; шаблоны: `knowledge/templates/newcomer/` (`ai-welcome-atlas`) |
| Сборка и релизы (PowerShell), зеркала Git | **[docs/publishing-and-ci.md](docs/publishing-and-ci.md)** |

### Возможности (сжато)

- **Заметки и секции:** атомарная запись, точечный `upsert`/`delete` секций `<!-- section:id -->`, история изменений напрямую через **Git (`git commit`, ADR-027)** с защитой от перезаписи незакоммиченных внешних правок и случайного усечения (`allow_shrink`).
- **Hot-context и анти-конфабуляционный роутер:** `read_hot_context`, `extract_from_archive`, `compact_hot_context`, `memory_health`, `route_context`.
- **Knowledge (`knowledge/`):** `read_knowledge_file` (с режимом `outline` для экономного обзора заголовков), `write_knowledge_file`, `append_knowledge_file`, `upsert_knowledge_section`, `knowledge_tags` — пути относительно `knowledge/`; корень — **`knowledge_path`** в туле или **primary root** из TOML (с поддержкой read-only корней `group` / публичного атласа).
- **Контракты:** `KB-V2-CONTRACT.md`, `coexistence-framework-v1.md` — в репозитории канона.

Полнотекст по Markdown-дереву канона **не** в этом процессе: для поиска — **[Hybrid Codebase Index](https://github.com/KarataevDmitry/hybrid-codebase-index)**.

### Где лежит `agent-notes.md`

При запущенном MCP с **`--config`**: **`{primary knowledge root}/agent-notes.md`** (см. `[knowledge]` в TOML).

Иначе (in-proc / тесты без runtime): **`AGENT_NOTES_FILE`** → иначе **`workspace_path/.cascade-ide/agent-notes.md`**. История изменений ведётся в Git (ADR-027).

### Слой `knowledge/`

- **`knowledge_path`** в вызове тула — явный корень репозитория с каталогом **`knowledge/`**.
- Без аргумента — **primary root** из **`--config`** (`[knowledge].primary` → `[knowledge.roots]`).
- **`file_path`:** только внутри `knowledge/`, без `..` и абсолютных путей.

Пример TOML и схема: `knowledge/work/local/agent-notes.workspace.example.toml` в репозитории канона.

### Workspace scope map

Секция **`workspace-scope-map-v1`** в hot-файле и файлы из **`[workspace]`** в TOML (`scope_map`, `scope_aliases`). Дефолты для нейтрального example — embedded в **AgentNotes.Core** (`agent-notes-mcp.defaults.toml`); см. **`docs/adr/008-workspace-scope-map-resolution.md`**.

**`workspace_path`** в аргументах тула — текущий проект (longest-prefix match по карте).

### Два разных «корня»

| | Назначение | Откуда путь |
|---|------------|-------------|
| **Hot-файл** | секции, `read_hot_context`, `route_context` | primary root из **`--config`** (или `AGENT_NOTES_FILE` / `.cascade-ide` без runtime) |
| **`knowledge/`** | read/write knowledge | **`knowledge_path`** или primary из TOML |

Один TOML с primary на клон **ai-welcome-atlas** / **agent-notes** согласует hot-файл и **`knowledge/`**.

### Участие

Issues и PR — на **GitHub**: [AI-Guiders/ai-atlas-mcp](https://github.com/AI-Guiders/ai-atlas-mcp).

Обновить описание тулов из кода:

```bash
dotnet run --project tools/ExportMcpManifest -- --write
```

(рабочий каталог — корень репозитория).
