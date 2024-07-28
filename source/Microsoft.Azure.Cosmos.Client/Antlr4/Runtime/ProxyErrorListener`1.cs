// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.ProxyErrorListener`1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections.Generic;
using System.IO;

namespace Antlr4.Runtime
{
  internal class ProxyErrorListener<Symbol> : IAntlrErrorListener<Symbol>
  {
    private readonly IEnumerable<IAntlrErrorListener<Symbol>> delegates;

    public ProxyErrorListener(IEnumerable<IAntlrErrorListener<Symbol>> delegates) => this.delegates = delegates != null ? delegates : throw new ArgumentNullException(nameof (delegates));

    protected internal virtual IEnumerable<IAntlrErrorListener<Symbol>> Delegates => this.delegates;

    public virtual void SyntaxError(
      TextWriter output,
      IRecognizer recognizer,
      Symbol offendingSymbol,
      int line,
      int charPositionInLine,
      string msg,
      RecognitionException e)
    {
      foreach (IAntlrErrorListener<Symbol> antlrErrorListener in this.delegates)
        antlrErrorListener.SyntaxError(output, recognizer, offendingSymbol, line, charPositionInLine, msg, e);
    }
  }
}
