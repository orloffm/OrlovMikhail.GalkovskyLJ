# OrlovMikhail.LJ.Galkovsky.BookMaker

Console application for converting dump.xml files to AsciiDoc fragments.

## Purpose

Main transformation tool. Reads dump.xml files and produces fragment.asc files with properly formatted AsciiDoc content.

## Entry Point

`Program.Main(string[] args)`

## Configuration

- Root folder path
- Source path (where dump.xml files are)
- Optional overwrite flag

## Workflow

```
1. Find all dump.xml files recursively in source path
2. Configure Autofac DI container with all required services
3. Parallel process each dump (10 concurrent in Release, 1 in Debug):
   a. Check if target exists / overwrite conditions
   b. Parse dump.xml with LayerParser → EntryPage
   c. Pick suitable comments with ISuitableCommentsPicker
   d. Create file storage (maps URLs to local files)
   e. Load userpics storage
   f. Convert entry HTML to post parts via HTMLToParts()
   g. Convert each comment thread to post parts
   h. Write all via IBookWriter (AsciiDoc format)
```

## Key Method: HTMLToParts()

```
HTML string
    ↓
IHTMLParser.Parse() → HTMLTokenBase[]
    ↓
IPostPartsMaker.Make() → IPostPart[]
    ↓
(11-stage processor pipeline runs internally)
    ↓
IPostPart[] (normalized)
```

## Output

For each `NNNN/dump.xml`:
- Creates `NNNN/fragment.asc` with AsciiDoc content

```asciidoc
== Entry Title
_2010-01-15 12:30_ +
http://galkovsky.livejournal.com/12345.html

Entry content...

=== Комментарии

Comment threads with proper formatting...
```

## Dependencies

- **OrlovMikhail.LJ.Grabber** - LayerParser, ISuitableCommentsPicker
- **OrlovMikhail.LJ.Galkovsky.Shared** - Fragment handling
- **OrlovMikhail.LJ.BookWriter** - Core conversion
- **OrlovMikhail.LJ.BookWriter.AsciiDoc** - Output format
