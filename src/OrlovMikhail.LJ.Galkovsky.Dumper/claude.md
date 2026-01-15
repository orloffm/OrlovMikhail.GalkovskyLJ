# OrlovMikhail.LJ.Galkovsky.Dumper

Console application for downloading blog entries from LiveJournal.

## Purpose

Entry point for content acquisition. Downloads entries and saves as dump.xml files using the Grabber library with Galkovsky-specific folder naming.

## Entry Point

`Program.Main(string[] args)`

## Configuration

Reads from application settings:
- Starting URL
- Root folder path
- Authentication cookie string

## Arguments

- `--nocontinue` - Download only the starting entry, don't follow to next

## Workflow

```
1. Parse starting URL
2. Create authentication cookies via LJClient.CreateDataObject()
3. Loop:
   a. Call IWorker.Work(url, rootFolder, cookies)
   b. Uses GalkovskyFolderNamingStrategy for folder naming
   c. Saves dump.xml with all comments and media
   d. Extract NextUrl from result
   e. Update settings with latest URL
   f. Continue to next entry (unless --nocontinue)
4. Stop when no NextUrl or user interrupts
```

## Dependencies

- **OrlovMikhail.LJ.Grabber** (via NuGet) - Core downloading
- **OrlovMikhail.LJ.Galkovsky.Shared** - Folder naming strategy
- **Autofac** - Dependency injection

## Output

Creates folder structure:
```
rootFolder/
  YYYY/
    NNNN/           # Entry number from subject
      dump.xml      # Complete entry with comments
      files/        # Downloaded images
```

Where NNNN is the Galkovsky entry key extracted from post subject.
