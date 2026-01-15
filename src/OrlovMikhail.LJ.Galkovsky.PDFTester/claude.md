# OrlovMikhail.LJ.Galkovsky.PDFTester

Console application for validating generated PDF files.

## Purpose

Verifies that generated PDFs contain all expected entries by checking PDF bookmarks against expected entry list.

## Entry Point

`Program.Main(string[] args)`

## Configuration

- Root folder path (where PDFs and split.txt are)

## Workflow

```
1. Load splits from split.txt
2. Discover expected fragments via FragmentHelper
3. For each split:
   a. Attempt to open PDF file: GalkovskyLJ_[Name].pdf
   b. Extract PDF bookmarks using iTextSharp
   c. Parse entry keys from bookmark titles
   d. Compare expected vs. actual entry keys
   e. Report missing entries
   f. Log file size and page count
```

## Validation Logic

**Bookmark extraction**:
- Uses iTextSharp's SimpleBookmark.GetBookmark()
- Extracts "Title" from each bookmark
- Parses Galkovsky entry key from title using GalkovskyFolderNamingStrategy

**Special case handling**:
- If entry "4" found, prepends "1", "2", "3" (category entries are consolidated)

**Output**:
- Lists missing entries
- Reports file size (MB) and page count
- Logs errors for inaccessible PDFs

## Dependencies

- **OrlovMikhail.LJ.Galkovsky.Shared** - FragmentHelper, SplitLoader
- **iTextSharp** (~5.5.7) - PDF reading and bookmark extraction
