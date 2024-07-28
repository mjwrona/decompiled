// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.CommonTokenFactory
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Misc;
using System;

namespace Antlr4.Runtime
{
  internal class CommonTokenFactory : ITokenFactory
  {
    public static readonly ITokenFactory Default = (ITokenFactory) new CommonTokenFactory();
    protected internal readonly bool copyText;

    public CommonTokenFactory(bool copyText) => this.copyText = copyText;

    public CommonTokenFactory()
      : this(false)
    {
    }

    public virtual CommonToken Create(
      Tuple<ITokenSource, ICharStream> source,
      int type,
      string text,
      int channel,
      int start,
      int stop,
      int line,
      int charPositionInLine)
    {
      CommonToken commonToken = new CommonToken(source, type, channel, start, stop);
      commonToken.Line = line;
      commonToken.Column = charPositionInLine;
      if (text != null)
        commonToken.Text = text;
      else if (this.copyText && source.Item2 != null)
        commonToken.Text = source.Item2.GetText(Interval.Of(start, stop));
      return commonToken;
    }

    IToken ITokenFactory.Create(
      Tuple<ITokenSource, ICharStream> source,
      int type,
      string text,
      int channel,
      int start,
      int stop,
      int line,
      int charPositionInLine)
    {
      return (IToken) this.Create(source, type, text, channel, start, stop, line, charPositionInLine);
    }

    public virtual CommonToken Create(int type, string text) => new CommonToken(type, text);

    IToken ITokenFactory.Create(int type, string text) => (IToken) this.Create(type, text);
  }
}
