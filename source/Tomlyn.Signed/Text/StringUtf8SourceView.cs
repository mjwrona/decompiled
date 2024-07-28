// Decompiled with JetBrains decompiler
// Type: Tomlyn.Text.StringUtf8SourceView
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll

using System.Text;


#nullable enable
namespace Tomlyn.Text
{
  internal struct StringUtf8SourceView : 
    ISourceView<StringCharacterUtf8Iterator>,
    ISourceView,
    IStringView,
    IStringView<StringCharacterUtf8Iterator>
  {
    private readonly byte[] _text;

    public StringUtf8SourceView(byte[] text, string sourcePath)
    {
      this._text = text;
      this.SourcePath = sourcePath;
    }

    public string SourcePath { get; }

    public string? GetString(int offset, int length) => offset + length <= this._text.Length ? Encoding.UTF8.GetString(this._text, offset, length) : (string) null;

    public StringCharacterUtf8Iterator GetIterator() => new StringCharacterUtf8Iterator(this._text);
  }
}
