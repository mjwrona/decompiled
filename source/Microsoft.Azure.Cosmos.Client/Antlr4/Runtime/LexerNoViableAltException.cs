// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.LexerNoViableAltException
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using System;
using System.Globalization;

namespace Antlr4.Runtime
{
  [Serializable]
  internal class LexerNoViableAltException : RecognitionException
  {
    private const long serialVersionUID = -730999203913001726;
    private readonly int startIndex;
    [Nullable]
    private readonly ATNConfigSet deadEndConfigs;

    public LexerNoViableAltException(
      Lexer lexer,
      ICharStream input,
      int startIndex,
      ATNConfigSet deadEndConfigs)
      : base(lexer, input)
    {
      this.startIndex = startIndex;
      this.deadEndConfigs = deadEndConfigs;
    }

    public virtual int StartIndex => this.startIndex;

    [Nullable]
    public virtual ATNConfigSet DeadEndConfigs => this.deadEndConfigs;

    public override IIntStream InputStream => base.InputStream;

    public override string ToString()
    {
      string str = string.Empty;
      if (this.startIndex >= 0 && this.startIndex < this.InputStream.Size)
        str = Utils.EscapeWhitespace(((ICharStream) this.InputStream).GetText(Interval.Of(this.startIndex, this.startIndex)), false);
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}('{1}')", (object) typeof (LexerNoViableAltException).Name, (object) str);
    }
  }
}
