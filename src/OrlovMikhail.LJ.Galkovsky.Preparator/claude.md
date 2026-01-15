# OrlovMikhail.LJ.Galkovsky.Preparator

Console application for assembling fragment files into book master documents.

## Purpose

Takes individual fragment.asc files and creates master AsciiDoc documents with includes, organized by splits (volumes).

## Entry Point

`Program.Main(string[] args)`

## Configuration

- Root folder path (where fragments and split.txt are)

## Workflow

```
1. Load splits from split.txt via SplitLoader
2. Discover all fragments via FragmentHelper
3. For each split:
   a. Select fragments in ID range (FromId to Next.FromId)
   b. Generate AsciiDoc header:
      - Title and subtitle
      - Document metadata (:doctype:, :toc:, etc.)
   c. Generate split information table
   d. Generate include directives for each fragment
   e. Write GalkovskyLJ_[Name].asc with UTF-8 BOM
```

## Output Format

```asciidoc
= Дмитрий Галковский: ЖЖ
== Volume Name

:doctype: book
:docinfo:
:toc:
:toclevels: 2
:imagesdir: .

[Split information table with entry ranges]

include::2003/0001/fragment.asc[]
include::2003/0002/fragment.asc[]
...
```

## Output Files

For each split in split.txt:
- `GalkovskyLJ_[SplitName].asc`

These master files are input for AsciiDoctor to generate PDF/EPUB.

## Dependencies

- **OrlovMikhail.LJ.Galkovsky.Shared** - FragmentHelper, SplitLoader, GalkovskyFolderNamingStrategy
