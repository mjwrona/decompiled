// Decompiled with JetBrains decompiler
// Type: Tomlyn.Parsing.LexerInternalState
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll

using System.Diagnostics;
using Tomlyn.Syntax;
using Tomlyn.Text;

namespace Tomlyn.Parsing
{
  [DebuggerDisplay("{Position} {Character}")]
  internal struct LexerInternalState
  {
    public TextPosition NextPosition;
    public TextPosition Position;
    public char32 PreviousChar;
    public char32 CurrentChar;

    public LexerInternalState(
      TextPosition nextPosition,
      TextPosition position,
      char32 previousChar,
      char32 c)
    {
      this.NextPosition = nextPosition;
      this.Position = position;
      this.PreviousChar = previousChar;
      this.CurrentChar = c;
    }
  }
}
