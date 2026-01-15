# OrlovMikhail.LJ.BookWriter

Core library for converting HTML posts to structured document parts. Format-agnostic with pluggable output writers.

## Directory Structure

```
├── HTMLParsing/     # HTML tokenization
├── PostParts/       # Post part abstractions
│   ├── Parts/       # Concrete part types
│   └── Processors/  # Transformation pipeline
├── Text/            # Text preparation/typography
└── (root files)     # Interfaces and base classes
```

## Architecture

### Two-Stage Conversion

**Stage 1: HTML → Tokens** (HTMLParsing/)
```
HTML string → IHTMLParser → HTMLTokenBase[]
```

**Stage 2: Tokens → Post Parts** (PostPartsMaker)
```
HTMLTokenBase[] → IPostPartsMaker → IPostPart[]
                                      ↓
                               11-stage Processor Pipeline
                                      ↓
                               IPostPart[] (normalized)
```

### Output Writing

```
IPostPart[] → IBookWriter (abstract interface)
                   ↓
           AsciiDocBookWriter (concrete implementation)
```

## Key Interfaces

### IBookWriter
Format-agnostic output interface:
- Document structure: `EntryPageBegin/End`, `CommentsBegin/End`, `ThreadBegin/End`
- Content: `EntryHeader`, `CommentHeader`, `WritePart`
- Entry/Comment end markers

### IBookWriterFactory
Creates IBookWriter instances for a target file.

### IPostPartsMaker
Converts HTML tokens to post parts:
- `Make(HTMLTokenBase[], IFileStorage)` - Returns `IPostPart[]`

### ITextPreparator
Text normalization interface:
- `Prepare(string text)` - Process content
- `PrepareUsername(string)` - Process usernames

## Processor Pipeline

11 stages transforming raw post parts to normalized output:

1. **TextMergingProcessor** - Combine consecutive text parts
2. **OpenCloseRemovalProcessor** - Remove empty formatting pairs
3. **TextTrimmingProcessor** - Trim whitespace at boundaries
4. **LineBreaksMergingProcessor** - Merge linebreaks to paragraphs
5. **LineBreakAdjacentFormattingSwapProcessor** - Move formatting outside breaks
6. **ImagesExtralineProcessor** - Ensure multimedia on separate lines
7. **ArtificialLinesRemoverProcessor** - Convert `---`/`===` separators
8. **ChevronsProcessor** - Handle quotation nesting (> prefixes)
9. **FormattingSpanningProcessor** - Ensure formatting spans paragraphs
10. **QuoteNormalizingProcessor** - Normalize quote depth levels
11. **ListsDisablerProcessor** - Prevent accidental list interpretation
12. **DoubleSpacesRemovalProcessor** - Remove double spaces
13. **BlocksAtTheEdgesRemover** - Clean up document edges

(Some processors run multiple times in the pipeline)
