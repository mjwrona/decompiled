// Decompiled with JetBrains decompiler
// Type: Tomlyn.Text.StringSourceView
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll


#nullable enable
namespace Tomlyn.Text
{
  internal struct StringSourceView : 
    ISourceView<StringCharacterIterator>,
    ISourceView,
    IStringView,
    IStringView<StringCharacterIterator>
  {
    private readonly string _text;

    public StringSourceView(string text, string sourcePath)
    {
      this._text = text;
      this.SourcePath = sourcePath;
    }

    public string SourcePath { get; }

    public string? GetString(int offset, int length) => offset + length <= this._text.Length ? this._text.Substring(offset, length) : (string) null;

    public StringCharacterIterator GetIterator() => new StringCharacterIterator(this._text);
  }
}
