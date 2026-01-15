# PostParts/Parts Directory

Concrete post part implementations.

## Text Parts

### RawTextPostPart.cs
Plain text content:
- `string Text` - Content string
- Implements `IRendersAsText` with `CanBeTrimmed = true`
- `SubstringClone(start, length)` - Create substring clone

### EmptyPostPart.cs
Zero-width placeholder:
- Singleton: `EmptyPostPart.Instance`
- Used as placeholder during processing

### UserLinkPart.cs
Username mention (@user):
- `string Username` - Account name
- `bool IsCommunity` - Whether community vs. user
- Implements `IRendersAsText`

## Structure Parts

### LineBreakPart.cs
Single line break:
- Singleton: `LineBreakPart.Instance`
- Extends `NewBlockStartBasePart`

### ParagraphStartPart.cs
Paragraph boundary with quotation support:
- `int QuotationLevel` - Quote nesting (0+)
- Extends `NewBlockStartBasePart`
- Level 0 = normal paragraph
- Level 1+ = nested quote (`>`, `>>`, etc.)

## Formatting Parts

### BoldStartPart.cs / BoldEndPart.cs
Bold formatting brackets (singletons).

### ItalicStartPart.cs / ItalicEndPart.cs
Italic formatting brackets (singletons).

### StrikeStartPart.cs / StrikeEndPart.cs
Strikethrough formatting brackets (singletons).

## Multimedia Parts

### ImagePart.cs
Image reference:
- `FileInfoBase File` - Local file reference
- Equality by file path

### VideoPart.cs
Video reference:
- `string Url` - Video URL (YouTube, etc.)
