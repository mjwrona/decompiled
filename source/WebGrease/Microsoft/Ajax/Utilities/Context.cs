// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.Context
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.ComponentModel;

namespace Microsoft.Ajax.Utilities
{
  public class Context
  {
    public DocumentContext Document { get; private set; }

    public int StartLineNumber { get; internal set; }

    public int StartLinePosition { get; internal set; }

    public int StartPosition { get; internal set; }

    public int EndLineNumber { get; internal set; }

    public int EndLinePosition { get; internal set; }

    public int EndPosition { get; internal set; }

    public int SourceOffsetStart { get; internal set; }

    public int SourceOffsetEnd { get; internal set; }

    public int OutputLine { get; set; }

    public int OutputColumn { get; set; }

    public JSToken Token { get; internal set; }

    public int StartColumn => this.StartPosition - this.StartLinePosition;

    public int EndColumn => this.EndPosition - this.EndLinePosition;

    public bool HasCode => !this.Document.IsGenerated && this.EndPosition > this.StartPosition && this.EndPosition <= this.Document.Source.Length && this.EndPosition != this.StartPosition;

    public string Code => this.Document.IsGenerated || this.EndPosition <= this.StartPosition || this.EndPosition > this.Document.Source.Length ? (string) null : this.Document.Source.Substring(this.StartPosition, this.EndPosition - this.StartPosition);

    private string ErrorSegment
    {
      get
      {
        string source = this.Document.Source;
        if (this.StartPosition >= source.Length)
          return string.Empty;
        int length = this.EndPosition - this.StartPosition;
        return this.StartPosition + length <= source.Length ? source.Substring(this.StartPosition, length).Trim() : source.Substring(this.StartPosition).Trim();
      }
    }

    public Context(DocumentContext document)
    {
      this.Document = document != null ? document : throw new ArgumentNullException(nameof (document));
      this.StartLineNumber = 1;
      this.EndLineNumber = 1;
      this.EndPosition = this.Document.Source.IfNotNull<string, int>((Func<string, int>) (s => s.Length));
      this.Token = JSToken.None;
    }

    public Context(
      DocumentContext document,
      int startLineNumber,
      int startLinePosition,
      int startPosition,
      int endLineNumber,
      int endLinePosition,
      int endPosition,
      JSToken token)
      : this(document)
    {
      this.StartLineNumber = startLineNumber;
      this.StartLinePosition = startLinePosition;
      this.StartPosition = startPosition;
      this.EndLineNumber = endLineNumber;
      this.EndLinePosition = endLinePosition;
      this.EndPosition = endPosition;
      this.Token = token;
    }

    public Context Clone() => new Context(this.Document)
    {
      StartLineNumber = this.StartLineNumber,
      StartLinePosition = this.StartLinePosition,
      StartPosition = this.StartPosition,
      EndLineNumber = this.EndLineNumber,
      EndLinePosition = this.EndLinePosition,
      EndPosition = this.EndPosition,
      SourceOffsetStart = this.SourceOffsetStart,
      SourceOffsetEnd = this.SourceOffsetEnd,
      Token = this.Token
    };

    public Context FlattenToStart()
    {
      Context start = this.Clone();
      start.EndLineNumber = start.StartLineNumber;
      start.EndLinePosition = start.StartLinePosition;
      start.EndPosition = start.StartPosition;
      start.Token = JSToken.None;
      return start;
    }

    public Context FlattenToEnd()
    {
      Context end = this.Clone();
      end.StartLineNumber = end.EndLineNumber;
      end.StartLinePosition = end.EndLinePosition;
      end.StartPosition = end.EndPosition;
      end.Token = JSToken.None;
      return end;
    }

    public Context CombineWith(Context other) => this.Clone().UpdateWith(other);

    public Context SplitStart(int length)
    {
      Context context = this.Clone();
      context.EndPosition = (this.StartPosition += length);
      context.EndLineNumber = context.StartLineNumber;
      context.EndLinePosition = context.StartLinePosition;
      return context;
    }

    public Context UpdateWith(Context other)
    {
      if (other != null)
      {
        if (other.StartPosition < this.StartPosition)
        {
          this.StartPosition = other.StartPosition;
          this.StartLineNumber = other.StartLineNumber;
          this.StartLinePosition = other.StartLinePosition;
          this.SourceOffsetStart = other.SourceOffsetStart;
        }
        if (other.EndPosition > this.EndPosition)
        {
          this.EndPosition = other.EndPosition;
          this.EndLineNumber = other.EndLineNumber;
          this.EndLinePosition = other.EndLinePosition;
          this.SourceOffsetEnd = other.SourceOffsetEnd;
        }
        if (this.Token != other.Token)
          this.Token = JSToken.None;
      }
      return this;
    }

    public bool Is(JSToken token) => this.Token == token;

    public bool IsOne(params JSToken[] tokens)
    {
      if (tokens != null)
      {
        JSToken token = this.Token;
        for (int index = tokens.Length - 1; index >= 0; --index)
        {
          if (tokens[index] == token)
            return true;
        }
      }
      return false;
    }

    public bool IsNot(JSToken token) => this.Token != token;

    public bool IsNotAny(params JSToken[] tokens)
    {
      if (tokens != null)
      {
        JSToken token = this.Token;
        for (int index = tokens.Length - 1; index >= 0; --index)
        {
          if (tokens[index] == token)
            return false;
        }
      }
      return true;
    }

    [Localizable(false)]
    public bool Is(string text) => text != null && this.EndPosition - this.StartPosition == text.Length && this.EndPosition <= this.Document.Source.Length && this.StartPosition >= 0 && this.StartPosition <= this.EndPosition && string.CompareOrdinal(this.Document.Source, this.StartPosition, text, 0, text.Length) == 0;

    internal void ReportUndefined(Lookup lookup) => this.Document.ReportUndefined(new UndefinedReference(lookup, this));

    internal void ChangeFileContext(string fileContext)
    {
      if (string.Compare(this.Document.FileContext, fileContext, StringComparison.OrdinalIgnoreCase) == 0)
        return;
      this.Document = this.Document.Clone();
      this.Document.FileContext = fileContext;
    }

    public static string GetErrorString(JSError errorCode) => JScript.ResourceManager.GetString(errorCode.ToString(), JScript.Culture);

    internal void HandleError(JSError errorId, bool forceToError = false)
    {
      if ((errorId == JSError.UndeclaredVariable || errorId == JSError.UndeclaredFunction) && this.Document.HasAlreadySeenErrorFor(this.Code))
        return;
      int severity = Context.GetSeverity(errorId);
      string str = Context.GetErrorString(errorId);
      string errorSegment = this.ErrorSegment;
      if (!errorSegment.IsNullOrWhiteSpace())
        str = str + CommonStrings.ContextSeparator + errorSegment;
      this.Document.HandleError(new ContextError()
      {
        IsError = forceToError || severity < 2,
        File = this.Document.FileContext,
        Severity = severity,
        Subcategory = ContextError.GetSubcategory(severity),
        ErrorNumber = (int) errorId,
        ErrorCode = "JS{0}".FormatInvariant((object) (int) errorId),
        StartLine = this.StartLineNumber,
        StartColumn = this.StartColumn + 1,
        EndLine = this.EndLineNumber,
        EndColumn = this.EndColumn + 1,
        Message = str
      });
    }

    public bool IsBefore(Context other)
    {
      if (other == null || this.StartLineNumber < other.StartLineNumber)
        return true;
      return this.StartLineNumber == other.StartLineNumber && this.StartColumn < other.StartColumn;
    }

    public override string ToString() => this.Code;

    private static int GetSeverity(JSError errorCode)
    {
      switch (errorCode)
      {
        case JSError.UnusedLabel:
        case JSError.SuspectAssignment:
        case JSError.SuspectSemicolon:
        case JSError.StatementBlockExpected:
        case JSError.WithNotRecommended:
        case JSError.ObjectConstructorTakesNoArguments:
        case JSError.NumericMaximum:
        case JSError.NumericMinimum:
        case JSError.OctalLiteralsDeprecated:
        case JSError.FunctionNameMustBeIdentifier:
        case JSError.SuspectEquality:
        case JSError.SemicolonInsertion:
        case JSError.NoModuleExport:
        case JSError.NewLineNotAllowed:
          return 4;
        case JSError.DuplicateName:
        case JSError.UndeclaredVariable:
        case JSError.UndeclaredFunction:
        case JSError.VariableDefinedNotReferenced:
        case JSError.ArgumentNotReferenced:
        case JSError.FunctionNotReferenced:
          return 3;
        case JSError.KeywordUsedAsIdentifier:
        case JSError.MisplacedFunctionDeclaration:
        case JSError.DuplicateConstantDeclaration:
        case JSError.ObjectLiteralKeyword:
        case JSError.DuplicateLexicalDeclaration:
        case JSError.DuplicateCatch:
        case JSError.ArrayLiteralTrailingComma:
        case JSError.HighSurrogate:
        case JSError.LowSurrogate:
          return 2;
        case JSError.AmbiguousCatchVar:
        case JSError.NumericOverflow:
        case JSError.AmbiguousNamedFunctionExpression:
        case JSError.StrictComparisonIsAlwaysTrueOrFalse:
        case JSError.ExportNotAtModuleLevel:
          return 1;
        default:
          return 0;
      }
    }
  }
}
