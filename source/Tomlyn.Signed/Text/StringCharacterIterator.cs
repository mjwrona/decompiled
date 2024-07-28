// Decompiled with JetBrains decompiler
// Type: Tomlyn.Text.StringCharacterIterator
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll

using System.Runtime.CompilerServices;
using Tomlyn.Collections;


#nullable enable
namespace Tomlyn.Text
{
  internal struct StringCharacterIterator : CharacterIterator, Iterator<char32, int>
  {
    private readonly string _text;

    public StringCharacterIterator(string text) => this._text = text;

    public int Start => 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public char32? TryGetNext(ref int position)
    {
      if (position < this._text.Length)
      {
        char ch = this._text[position];
        ++position;
        return new char32?((char32) (char.IsHighSurrogate(ch) ? this.NextCharWithSurrogate(ref position, ch) : (int) ch));
      }
      position = this._text.Length;
      return new char32?();
    }

    private int NextCharWithSurrogate(ref int position, char c1)
    {
      if (position >= this._text.Length)
        throw new CharReaderException("Unexpected EOF after high-surrogate char");
      char ch = this._text[position];
      ++position;
      return char.IsLowSurrogate(ch) ? char.ConvertToUtf32(c1, ch) : throw new CharReaderException("Unexpected character after high-surrogate char");
    }
  }
}
