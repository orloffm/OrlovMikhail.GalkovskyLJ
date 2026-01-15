# PostParts Directory

Post part abstractions representing document elements.

## Structure

```
PostParts/
├── Parts/           # Concrete part implementations
├── Processors/      # Transformation pipeline
└── (interfaces)     # Base interfaces and classes
```

## Core Interfaces

### IPostPart
Base interface for all post parts:
- `IPostPart FullClone()` - Deep copy for processor immutability

### IRendersAsText
Interface for text-generating parts:
- `bool CanBeTrimmed` - Whether whitespace can be trimmed

### PostPartBase.cs
Abstract base implementing IPostPart with clone support.

## Part Hierarchy

```
IPostPart
├── PostPartBase (abstract)
│   ├── RawTextPostPart         # Plain text
│   ├── UserLinkPart            # @username mention
│   ├── MultimediaBasePart
│   │   ├── ImagePart           # Image reference
│   │   └── VideoPart           # Video URL
│   ├── NewBlockStartBasePart
│   │   ├── LineBreakPart       # Single line break
│   │   └── ParagraphStartPart  # Paragraph with quote level
│   └── FormattingBasePart
│       ├── FormattingStartBasePart
│       │   ├── BoldStartPart
│       │   ├── ItalicStartPart
│       │   └── StrikeStartPart
│       └── FormattingEndBasePart
│           ├── BoldEndPart
│           ├── ItalicEndPart
│           └── StrikeEndPart
└── EmptyPostPart               # Zero-width placeholder
```

## Singleton Parts

These parts are stateless singletons:
- `EmptyPostPart.Instance`
- `LineBreakPart.Instance`
- `BoldStartPart.Instance` / `BoldEndPart.Instance`
- `ItalicStartPart.Instance` / `ItalicEndPart.Instance`
- `StrikeStartPart.Instance` / `StrikeEndPart.Instance`

## ParagraphStartPart

Special part for paragraph boundaries:
- `int QuotationLevel` - Nesting depth (0 = no quote, 1+ = nested quotes)
- Used to generate `>` prefixes in AsciiDoc output
