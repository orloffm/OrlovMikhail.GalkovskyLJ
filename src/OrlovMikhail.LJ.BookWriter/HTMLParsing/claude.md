# HTMLParsing Directory

HTML tokenization - converts HTML strings to token streams.

## Files

### IHTMLParser.cs / HTMLParser.cs
HTML tokenizer:
- `Parse(string html)` - Returns `HTMLTokenBase[]`
- Character-by-character tokenization finding `<` and `>` pairs
- Decodes HTML entities in text tokens
- Static `StripOfTags(string)` - Remove all tags, keep text

### HTMLTokenBase.cs
Abstract base for tokens.

### TextHTMLToken.cs
Text content token:
- `string Text` - Decoded text content (HTML entities resolved)

### TagHTMLToken.cs
HTML element token:
- `HTMLElementKind Kind` - Element type enum
- `bool IsOpening` - Opening tag (`<tag>`)
- `bool IsClosing` - Closing tag (`</tag>` or `<tag/>`)
- `Dictionary<string, string> Attributes` - Tag attributes (case-insensitive)

**Static Methods**:
- `FromTag(string tagContent)` - Parse tag string to TagHTMLToken
- Uses regex for attribute extraction: `(\w+)\s*=\s*[""']?([^""'>]+)`

### HTMLElementKind.cs
Enum of recognized elements:
- **Formatting**: Bold, Italic, Strike, Underline
- **Layout**: Center, LineBreak
- **Media**: Image, IFrame
- **Links**: Anchor, Span

Unrecognized tags are skipped during post part creation.
