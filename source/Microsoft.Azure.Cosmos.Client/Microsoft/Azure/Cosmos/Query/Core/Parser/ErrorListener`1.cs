// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Parser.ErrorListener`1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.Azure.Cosmos.Query.Core.Parser
{
  internal sealed class ErrorListener<S> : IAntlrErrorListener<S>
  {
    public ParseException parseException;
    private readonly Antlr4.Runtime.Parser parser;
    private readonly Lexer lexer;
    private readonly CommonTokenStream tokenStream;
    private bool firstTime;

    public ErrorListener(Antlr4.Runtime.Parser parser, Lexer lexer, CommonTokenStream token_stream)
    {
      this.parser = parser;
      this.lexer = lexer;
      this.tokenStream = token_stream;
      this.firstTime = true;
    }

    public void SyntaxError(
      TextWriter output,
      IRecognizer recognizer,
      S offendingSymbol,
      int line,
      int col,
      string msg,
      RecognitionException recognitionException)
    {
      if (!this.firstTime)
        return;
      try
      {
        this.firstTime = false;
        IntervalSet intervalSet = new LASets().Compute(this.parser, this.tokenStream, line, col);
        List<string> stringList = new List<string>();
        foreach (int tokenType in (IEnumerable<int>) intervalSet.ToList())
        {
          string symbolicName = this.parser.Vocabulary.GetSymbolicName(tokenType);
          stringList.Add(symbolicName);
        }
        string message;
        if (stringList.Any<string>())
          message = string.Format("Parse error line:{0}, col:{1}, expecting: {2}", (object) line, (object) col, (object) string.Join(", ", (IEnumerable<string>) stringList));
        else
          message = string.Format("Parse error: message:{0} offendingSymbol: {1}, line:{2}, col:{3}", (object) msg, (object) offendingSymbol, (object) line, (object) col);
        this.parseException = new ParseException(message, (Exception) recognitionException);
      }
      catch (Exception ex)
      {
        this.parseException = new ParseException("Unknown parse exception", ex);
      }
    }
  }
}
