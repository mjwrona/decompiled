// Decompiled with JetBrains decompiler
// Type: Tomlyn.Text.StringCharacterUtf8Iterator
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll

using System.Runtime.CompilerServices;
using Tomlyn.Collections;


#nullable enable
namespace Tomlyn.Text
{
  internal struct StringCharacterUtf8Iterator : CharacterIterator, Iterator<char32, int>
  {
    private readonly byte[] _text;
    private readonly int _start;

    public StringCharacterUtf8Iterator(byte[] text)
    {
      this._text = text;
      this._start = text.Length < 3 || text[0] != (byte) 239 || text[1] != (byte) 187 || text[0] != (byte) 191 ? 0 : 3;
    }

    public int Start => this._start;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public char32? TryGetNext(ref int position) => CharHelper.ToUtf8(this._text, ref position);
  }
}
