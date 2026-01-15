# OrlovMikhail.LJ.BookWriter.AsciiDoc

AsciiDoc format implementation of IBookWriter interface.

## Files

### AsciiDocBookWriter.cs
Extends BookWriterBase, outputs AsciiDoc formatted text:
- Maintains `currentQuotationLevel` state for chevron prefixes
- Uses StreamWriter for file output

**Key Methods**:

`EntryHeaderInternal(Entry, Userpic, string relativeUserpicPath)`
- Outputs: `== Title` with date, URL, optional userpic image

`CommentHeaderInternal(Comment, string userpicPath)`
- Outputs: Username, date, comment indicators
- Shows depth, parent URL, policy markers

`WriteParagraphStartInternal(ParagraphStartPart)`
- Generates chevron prefixes: `> `, `>> `, etc.
- Based on `QuotationLevel` property

`WritePreparedTextInternal(string text)`
- Splits text to ~60 character lines
- Respects word boundaries (space-aware splitting)

`WriteImageInternal(ImagePart, bookRoot)`
- Format: `image::path[scaledwidth=X%]`
- Calculates scale based on aspect ratio (max 60% height)

`WriteVideoInternal(VideoPart)`
- Outputs: `(Видео: URL)` placeholder

`WriteLineBreakInternal()`
- Outputs AsciiDoc line continuation: ` +`

**Format markers**:
- Bold: `**text**`
- Italic: `__text__`
- Strike: `[line-through]#text#` or `/-text-/`

### AsciiDocBookWriterFactory.cs
Factory creating AsciiDocBookWriter instances.

### AsciiDocTextPreparator.cs
AsciiDoc-specific text handling (extends TextPreparator):
- Escapes formatter chars (`_`, `*`) when appearing in pairs
- Converts Russian quotes to `«»`
- Prepends `{empty}==` to prevent accidental headers
- Handles № symbol (replaces with N)

## Output Format

```asciidoc
== Entry Title
_2010-01-15 12:30_ +
http://username.livejournal.com/12345.html

Entry text content here...

=== Комментарии

[quote]
____
> Quoted reply text
____
```
