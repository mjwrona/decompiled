// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Parser.SqlQueryParser
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Cosmos.SqlObjects;
using System;
using System.Runtime.ExceptionServices;

namespace Microsoft.Azure.Cosmos.Query.Core.Parser
{
  internal static class SqlQueryParser
  {
    public static bool TryParse(string text, out SqlQuery sqlQuery)
    {
      TryCatch<SqlQuery> tryCatch = SqlQueryParser.Monadic.Parse(text);
      if (tryCatch.Failed)
      {
        sqlQuery = (SqlQuery) null;
        return false;
      }
      sqlQuery = tryCatch.Result;
      return true;
    }

    public static SqlQuery Parse(string text)
    {
      TryCatch<SqlQuery> tryCatch = SqlQueryParser.Monadic.Parse(text);
      tryCatch.ThrowIfFailed();
      return tryCatch.Result;
    }

    public static class Monadic
    {
      public static TryCatch<SqlQuery> Parse(string text)
      {
        sqlLexer sqlLexer = text != null ? new sqlLexer((ICharStream) new AntlrInputStream(text)) : throw new ArgumentNullException(nameof (text));
        CommonTokenStream commonTokenStream = new CommonTokenStream((ITokenSource) sqlLexer);
        sqlParser sqlParser1 = new sqlParser((ITokenStream) commonTokenStream);
        sqlParser1.ErrorHandler = (IAntlrErrorStrategy) SqlQueryParser.Monadic.ThrowExceptionOnErrors.Singleton;
        sqlParser sqlParser2 = sqlParser1;
        ErrorListener<IToken> listener = new ErrorListener<IToken>((Antlr4.Runtime.Parser) sqlParser2, (Lexer) sqlLexer, commonTokenStream);
        sqlParser2.AddErrorListener((IAntlrErrorListener<IToken>) listener);
        sqlParser.ProgramContext tree;
        try
        {
          tree = sqlParser2.program();
        }
        catch (Exception ex)
        {
          return TryCatch<SqlQuery>.FromException(ex);
        }
        return listener.parseException != null ? TryCatch<SqlQuery>.FromException((Exception) listener.parseException) : TryCatch<SqlQuery>.FromResult((SqlQuery) CstToAstVisitor.Singleton.Visit((IParseTree) tree));
      }

      private sealed class ThrowExceptionOnErrors : IAntlrErrorStrategy
      {
        public static readonly SqlQueryParser.Monadic.ThrowExceptionOnErrors Singleton = new SqlQueryParser.Monadic.ThrowExceptionOnErrors();

        public bool InErrorRecoveryMode(Antlr4.Runtime.Parser recognizer) => false;

        public void Recover(Antlr4.Runtime.Parser recognizer, RecognitionException e) => ExceptionDispatchInfo.Capture((Exception) e).Throw();

        [return: NotNull]
        public IToken RecoverInline(Antlr4.Runtime.Parser recognizer) => throw new NotSupportedException("can not recover.");

        public void ReportError(Antlr4.Runtime.Parser recognizer, RecognitionException e) => ExceptionDispatchInfo.Capture((Exception) e).Throw();

        public void ReportMatch(Antlr4.Runtime.Parser recognizer)
        {
        }

        public void Reset(Antlr4.Runtime.Parser recognizer)
        {
        }

        public void Sync(Antlr4.Runtime.Parser recognizer)
        {
        }
      }
    }
  }
}
