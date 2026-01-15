# OrlovMikhail.GalkovskyLJ

Project-specific tooling for archiving Dmitry Galkovsky's LiveJournal blog and generating AsciiDoc books.

## Solution Structure

```
src/
├── OrlovMikhail.LJ.Galkovsky.Shared/      # Shared utilities (fragment handling, splits)
├── OrlovMikhail.LJ.BookWriter/            # Core HTML→PostParts conversion
├── OrlovMikhail.LJ.BookWriter.AsciiDoc/   # AsciiDoc output format
├── OrlovMikhail.LJ.Galkovsky.Dumper/      # Console: Download entries
├── OrlovMikhail.LJ.Galkovsky.BookMaker/   # Console: Convert dumps to AsciiDoc
├── OrlovMikhail.LJ.Galkovsky.Preparator/  # Console: Assemble book master files
├── OrlovMikhail.LJ.Galkovsky.PDFTester/   # Console: Validate PDFs
├── OrlovMikhail.LJ.BookWriter.Tests/      # Unit tests
└── OrlovMikhail.GalkovskyLJ.sln

book/                                       # Output directory
├── 2003/ through 2017/                    # Year-organized content
├── userpics/                              # User avatars
├── split.txt                              # Book split configuration
├── Gemfile                                # Ruby dependencies
├── build.bat, build.ps1                   # Build scripts
└── *.asc                                  # Generated AsciiDoc files
```

## Framework & Dependencies

- **Framework**: .NET 4.5
- **Package Manager**: Paket (paket.dependencies)
- **Key Dependencies**:
  - `OrlovMikhail.LJ.Grabber` (NuGet) - Core downloading library
  - `OrlovMikhail.Tools` (NuGet) - Shared utilities
  - `Autofac` - Dependency injection
  - `log4net` - Logging
  - `System.IO.Abstractions` - File I/O abstraction
  - `iTextSharp` - PDF validation (PDFTester only)

## Workflow

```
1. Dumper          → Downloads blog entries → dump.xml files
2. BookMaker       → Converts dump.xml → fragment.asc files
3. Preparator      → Assembles fragments → GalkovskyLJ_[Name].asc
4. AsciiDoctor     → External: Generates PDF/EPUB (Ruby)
5. PDFTester       → Validates PDF contains all expected entries
```

## Key Configuration

**split.txt** defines book splits (volumes):
```
Name    FromId    Description
Book1   1         First volume
Book2   500       Second volume
...
```

Each split becomes a separate book file covering entries from FromId to the next split's FromId.
