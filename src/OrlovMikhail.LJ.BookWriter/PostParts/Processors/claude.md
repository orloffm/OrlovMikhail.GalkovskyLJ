# PostParts/Processors Directory

Transformation pipeline for normalizing post parts.

## Base Infrastructure

### IProcessor.cs
Interface: `List<IPostPart> Process(List<IPostPart>)`

### ProcessorBase.cs
Abstract base with utilities:
- `Clone(List<IPostPart>)` - Deep copy list
- `FindNextPartIndex<T>(list, startIndex)` - Find next part of type T
- `EnumerateTextPartsBetween(list, from, to)` - Get text parts in range
- `EnumerateCharsBetween(list, from, to)` - Extract characters across parts

## Processors (in pipeline order)

### TextMergingProcessor.cs
Combines consecutive `RawTextPostPart` into single parts.

### OpenCloseRemovalProcessor.cs
Removes adjacent open/close formatting pairs (empty `**` or `__`).

### TextTrimmingProcessor.cs
Trims leading/trailing whitespace at paragraph boundaries.

### LineBreaksMergingProcessor.cs
Converts multiple `LineBreakPart` sequences to single `ParagraphStartPart`.

### LineBreakAdjacentFormattingSwapProcessor.cs
Moves formatting markers outside line breaks:
- `**\n` → `\n**`
- Ensures formatting doesn't span line boundaries inappropriately

### ImagesExtralineProcessor.cs
Ensures multimedia (images/videos) appear on their own paragraphs:
- Inserts paragraph breaks before/after if needed

### ArtificialLinesRemoverProcessor.cs
Handles decorative separators:
- `---` or `===` lines → converted to italic or removed
- Cleans up artificial formatting from HTML

### ChevronsProcessor.cs
**Most complex processor** - handles quotation nesting:
- Counts leading `>` characters for quote depth
- Converts special delimiters (`--`, `===`, `//`) to chevrons
- Manages quote level transitions
- Removes formatting from quoted sections
- Sets `QuotationLevel` on `ParagraphStartPart`

### FormattingSpanningProcessor.cs
Ensures formatting brackets span complete paragraphs:
- Inserts closing bracket at paragraph end
- Resumes formatting in next paragraph
- Handles image blocks specially (don't wrap)

### QuoteNormalizingProcessor.cs
Normalizes quotation levels to remove gaps:
- Quote levels 1, 3, 3 → 1, 2, 2
- Processes between zero-level quote blocks

### ListsDisablerProcessor.cs
Prepends `{empty}` to lines that look like lists:
- Regex: `^(?:[*\da-zА-ЯA-Zа-я]*\.|\[)`
- Prevents AsciiDoc from interpreting content as lists

### DoubleSpacesRemovalProcessor.cs
Removes consecutive spaces in text parts.

### BlocksAtTheEdgesRemover.cs
Removes `ParagraphStartPart` at document start/end (unless quoted content).

## Pipeline Execution

Processors run in sequence, each receiving output of previous:
```
Raw Parts → TextMerging → OpenCloseRemoval → TextTrimming → LineBreaksMerging
    → FormattingSwap → ImagesExtraline → ArtificialLines → Chevrons
    → FormattingSpanning → TextMerging → TextTrimming → LineBreaksMerging
    → QuoteNormalizing → BlocksAtEdges → ListsDisabler → DoubleSpaces
    → Final Parts
```

Note: Some processors run multiple times for iterative cleanup.
