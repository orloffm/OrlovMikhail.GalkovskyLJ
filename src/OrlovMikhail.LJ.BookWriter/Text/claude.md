# Text Directory

Text preparation and typography handling.

## Files

### ITextPreparator.cs
Interface:
- `Prepare(string text)` - Process content text
- `PrepareUsername(string)` - Process usernames

### TextPreparator.cs
Abstract base with sophisticated Russian typography:

**Quote handling**:
- Converts various Unicode quote characters to standard forms
- Handles `"`, `«»`, `„"`, curly quotes

**Dash handling**:
- Em-dash insertion with non-breaking spaces
- Pattern: `word — word` (with nbsp around dash)

**Russian typography rules**:
- Non-breaking space after short prepositions: в, на, по, из, к, с, о, у, а, и, но, за, до, от, при, для, без, под, над, об, про
- Prevents orphaned prepositions at line ends

**Processing phases**:
1. Pre-regex: Initial transformations
2. Main: Quote and dash handling
3. Post: Cleanup

### LatexTextPreparator.cs
LaTeX-specific escaping (extends TextPreparator):
- `~` for non-breaking space
- `\лк`, `\пк` for left/right quotes
- `\мт` for em-dash
- Escapes: `\`, `_`, `&`, `%`

Used when generating LaTeX output (alternative to AsciiDoc).
