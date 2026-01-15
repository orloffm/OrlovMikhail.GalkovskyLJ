# GalkovskyLJ Source

## Projects Overview

### Libraries

**OrlovMikhail.LJ.Galkovsky.Shared**
Shared utilities: fragment discovery, split loading, Galkovsky-specific folder naming.

**OrlovMikhail.LJ.BookWriter**
Core library: HTML tokenization → post parts → processing pipeline.

**OrlovMikhail.LJ.BookWriter.AsciiDoc**
AsciiDoc format writer implementation.

### Console Applications

**OrlovMikhail.LJ.Galkovsky.Dumper**
Downloads blog entries using Grabber library. Entry point for content acquisition.

**OrlovMikhail.LJ.Galkovsky.BookMaker**
Converts dump.xml files to AsciiDoc fragments. Main transformation tool.

**OrlovMikhail.LJ.Galkovsky.Preparator**
Assembles fragment files into book master documents with includes.

**OrlovMikhail.LJ.Galkovsky.PDFTester**
Validates generated PDFs contain all expected bookmarks.

### Tests

**OrlovMikhail.LJ.BookWriter.Tests**
NUnit tests for BookWriter library.

## Dependency Graph

```
OrlovMikhail.LJ.Grabber (NuGet)
    ↓
OrlovMikhail.LJ.Galkovsky.Shared
    ↓
├── OrlovMikhail.LJ.Galkovsky.Dumper
├── OrlovMikhail.LJ.Galkovsky.Preparator
└── OrlovMikhail.LJ.BookWriter
        ↓
    ├── OrlovMikhail.LJ.BookWriter.AsciiDoc
    └── OrlovMikhail.LJ.Galkovsky.BookMaker
```

## Package Management

Uses Paket (not NuGet directly):
- `paket.dependencies` - Root dependency definitions
- `.paket/` - Paket bootstrapper
- `paket.references` - Per-project references
