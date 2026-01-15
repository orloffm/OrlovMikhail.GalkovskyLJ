# OrlovMikhail.LJ.Galkovsky.Shared

Shared utilities for fragment management, split configuration, and Galkovsky-specific folder naming.

## Key Classes

### FragmentHelper / IFragmentHelper
Fragment file discovery and selection:
- `GetFragments(DirectoryInfoBase root)` - Returns `Dictionary<long, FragmentInformation>` mapping entry IDs to fragment files
- `SelectByAppropriateSplit(fragments, split)` - Filters fragments within split's ID range
- Parses dump.xml files to extract entry metadata
- Uses LayerParser from Grabber library

### FragmentInformation
Data holder:
- `string RelativeFragmentPath` - Path to fragment.asc file
- `string GalkovskyEntryKey` - Author's custom entry identifier (e.g., "123", "PS-001")

### GalkovskyFolderNamingStrategy / IGalkovskyFolderNamingStrategy
Extends IFolderNamingStrategy with Galkovsky-specific naming:
- `TryGetSubfolderByEntry(Entry)` - Returns folder name based on entry number
- `GetGalkovskyEntryKey(Entry)` - Extracts entry key from post subject

**Entry key extraction** from subject format `"NUMBER. Subject text"`:
- Regular entries: 4-digit zero-padded number (e.g., "0123")
- Postscripts: "PS-" prefix with 3-digit number (e.g., "PS-001")
- Special categories (hardcoded IDs):
  - "ДЕЛИКАТНАЯ ТЕМА" → entry 3
  - "УТОЧНЕНИЕ ИНТЕРЕСОВ" → entry 2
  - "ПРОБА ПЕРА" → entry 1
- Fixes Cyrillic lookalikes: А→A, В→B (for cross-platform compatibility)

### Split
Book split definition:
- `string Name` - Book/volume name
- `long FromId` - Starting entry ID
- `Split Next` - Reference to next split (linked list)
- `string Description` - Human-readable description

### SplitLoader / ISplitLoader
Reads `split.txt` configuration:
- Tab-separated format: `Name\tFromId\tDescription`
- Links splits together in chain
- `Load(FileInfoBase)` - Returns first split in chain

## split.txt Format

```
Name    FromId    Description
Том1    1         Первый том (2003-2005)
Том2    500       Второй том (2006-2008)
...
```
