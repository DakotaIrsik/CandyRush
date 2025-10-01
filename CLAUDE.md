# Claude Configuration Guide

This document provides instructions for working with Claude on the Candy Rush project.

## Loading External Memory

When starting a new Claude session, always begin by requesting Claude to "load external memory". This command triggers Claude to:

1. **Load Local Configuration**
   - Reads `.claude/settings.local.json` for project context
   - Loads Marvel team system personas
   - Initializes OpenAI integration settings
   - Loads project-specific permissions

2. **Check Google Drive Documentation**
   - Connects to the main documentation folder: `1l_EQ8dg-jEw-qt8caFYc7xrOiOc_6WLF`
   - Checks the `.claude` subfolder: `1tXeK9eUWeJk9wQ8dYQJLq9_XVe3A7HEs`
   - Loads any updated documentation or context

3. **Sync Configuration Files**
   - Ensures all `.claude/*.json` files are up to date
   - Verifies Google Drive access permissions
   - Confirms GitHub CLI authentication status

## Configuration Files

The `.claude/` directory contains:

- **settings.local.json**: Main configuration with project context, permissions, and system settings
- **openai.config.json**: OpenAI GPT-4o integration settings
- **steamworks.config.json**: Steam integration configuration
- **gdrive.config.json**: Google Drive API configuration

## Syncing Latest Versions

To ensure Claude always has the latest configuration:

1. **Manual Sync Command**:
   ```bash
   sync-claude-settings.bat
   ```

2. **Google Drive Sync**:
   - Upload updated configs: `gdrive files upload --parent 1tXeK9eUWeJk9wQ8dYQJLq9_XVe3A7HEs [file]`
   - Download latest configs: `gdrive files download [fileId]`

3. **Git Integration**:
   - Configuration changes should be committed to the repository
   - Claude can use git commands to pull latest changes

## Marvel Team System

The project uses character personas for different development roles:

- **@tony_stark**: Technical Lead - Architecture and implementation
- **@sue_storm**: Code Reviewer - Quality and best practices
- **@bruce_banner**: QA Engineer - Testing and validation
- **@jean_grey**: Business Analyst - Requirements translation
- **@wolverine**: DevOps Engineer - Deployment and CI/CD
- **@charles_xavier**: Product Owner - Feature prioritization
- **@stephen_strange**: Game Mechanics Expert - Physics and gameplay
- **@nick_fury**: Game Design Auditor - Overall assessment

Address Claude with @persona-name to activate specific expertise.

## External Documentation Structure

Google Drive folder organization:
```
Candy Rush Documentation (1l_EQ8dg-jEw-qt8caFYc7xrOiOc_6WLF)
└── .claude (1tXeK9eUWeJk9wQ8dYQJLq9_XVe3A7HEs)
    ├── System Docs
    ├── Backlog Items
    ├── Technical Debt
    └── Meeting Notes
```

## Best Practices

1. **Start Every Session**: Always begin with "load external memory"
2. **Regular Syncs**: Periodically sync configuration during long sessions
3. **Document Changes**: Update this file when configuration structure changes
4. **Version Control**: Commit configuration changes to git
5. **Backup**: Keep Google Drive backups of critical configurations
6. **CRITICAL**: NEVER create files named "nul" - this destroys git stashes and breaks Windows file system operations

## Troubleshooting

If external memory fails to load:

1. Check `.claude/` directory exists and contains config files
2. Verify Google Drive authentication: `gdrive about`
3. Confirm GitHub CLI authentication: `gh auth status`
4. Ensure permissions in `settings.local.json` are correct
5. Run manual sync script if automatic sync fails

## OpenAI Integration

When specialized analysis is needed:
- Ask Claude to "ask GPT about [topic]"
- Used for: hole physics, spawning patterns, Unity 2D optimization
- Budget: $5 testing allocation
- Model: GPT-4o

## Quick Reference Commands

```bash
# Load external memory (say this to Claude)
"load external memory"

# Check Google Drive files
gdrive files list --parent 1l_EQ8dg-jEw-qt8caFYc7xrOiOc_6WLF

# Check .claude subfolder
gdrive files list --parent 1tXeK9eUWeJk9wQ8dYQJLq9_XVe3A7HEs

# Git operations (pre-approved)
git add .
git commit -m "message"
git push
```

---

*Remember: External memory loading ensures Claude has full project context and latest configurations.*