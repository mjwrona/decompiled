// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Recognizer`2
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;

namespace Antlr4.Runtime
{
  internal abstract class Recognizer<Symbol, ATNInterpreter> : IRecognizer where ATNInterpreter : ATNSimulator
  {
    public const int Eof = -1;
    [NotNull]
    private IAntlrErrorListener<Symbol>[] _listeners = new IAntlrErrorListener<Symbol>[1]
    {
      (IAntlrErrorListener<Symbol>) ConsoleErrorListener<Symbol>.Instance
    };
    private ATNInterpreter _interp;
    private int _stateNumber = -1;

    public abstract string[] RuleNames { get; }

    public abstract IVocabulary Vocabulary { get; }

    [NotNull]
    public virtual IDictionary<string, int> TokenTypeMap => this.CreateTokenTypeMap(this.Vocabulary);

    protected virtual IDictionary<string, int> CreateTokenTypeMap(IVocabulary vocabulary)
    {
      Dictionary<string, int> tokenTypeMap = new Dictionary<string, int>();
      for (int tokenType = 0; tokenType <= this.Atn.maxTokenType; ++tokenType)
      {
        string literalName = vocabulary.GetLiteralName(tokenType);
        if (literalName != null)
          tokenTypeMap[literalName] = tokenType;
        string symbolicName = vocabulary.GetSymbolicName(tokenType);
        if (symbolicName != null)
          tokenTypeMap[symbolicName] = tokenType;
      }
      tokenTypeMap["EOF"] = -1;
      return (IDictionary<string, int>) tokenTypeMap;
    }

    [NotNull]
    public virtual IDictionary<string, int> RuleIndexMap => Utils.ToMap(this.RuleNames ?? throw new NotSupportedException("The current recognizer does not provide a list of rule names."));

    public virtual int GetTokenType(string tokenName)
    {
      int num;
      return this.TokenTypeMap.TryGetValue(tokenName, out num) ? num : 0;
    }

    public virtual string SerializedAtn
    {
      [return: NotNull] get => throw new NotSupportedException("there is no serialized ATN");
    }

    public abstract string GrammarFileName { get; }

    public virtual ATN Atn => this._interp.atn;

    public virtual ATNInterpreter Interpreter
    {
      get => this._interp;
      protected set => this._interp = value;
    }

    public virtual ParseInfo ParseInfo => (ParseInfo) null;

    [return: NotNull]
    public virtual string GetErrorHeader(RecognitionException e)
    {
      int line = e.OffendingToken.Line;
      int column = e.OffendingToken.Column;
      return "line " + line.ToString() + ":" + column.ToString();
    }

    [Obsolete("This method is not called by the ANTLR 4 Runtime. Specific implementations of IAntlrErrorStrategy may provide a similar feature when necessary. For example, see DefaultErrorStrategy.GetTokenErrorDisplay(IToken).")]
    public virtual string GetTokenErrorDisplay(IToken t) => t == null ? "<no token>" : "'" + (t.Text ?? (t.Type != -1 ? "<" + t.Type.ToString() + ">" : "<EOF>")).Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t") + "'";

    public virtual void AddErrorListener(IAntlrErrorListener<Symbol> listener)
    {
      Args.NotNull(nameof (listener), (object) listener);
      IAntlrErrorListener<Symbol>[] listeners = this._listeners;
      Array.Resize<IAntlrErrorListener<Symbol>>(ref listeners, listeners.Length + 1);
      listeners[listeners.Length - 1] = listener;
      this._listeners = listeners;
    }

    public virtual void RemoveErrorListener(IAntlrErrorListener<Symbol> listener)
    {
      IAntlrErrorListener<Symbol>[] listeners = this._listeners;
      int destinationIndex = Array.IndexOf<IAntlrErrorListener<Symbol>>(listeners, listener);
      if (destinationIndex < 0)
        return;
      Array.Copy((Array) listeners, destinationIndex + 1, (Array) listeners, destinationIndex, listeners.Length - destinationIndex - 1);
      Array.Resize<IAntlrErrorListener<Symbol>>(ref listeners, listeners.Length - 1);
      this._listeners = listeners;
    }

    public virtual void RemoveErrorListeners() => this._listeners = new IAntlrErrorListener<Symbol>[0];

    [NotNull]
    public virtual IList<IAntlrErrorListener<Symbol>> ErrorListeners => (IList<IAntlrErrorListener<Symbol>>) new List<IAntlrErrorListener<Symbol>>((IEnumerable<IAntlrErrorListener<Symbol>>) this._listeners);

    public virtual IAntlrErrorListener<Symbol> ErrorListenerDispatch => (IAntlrErrorListener<Symbol>) new ProxyErrorListener<Symbol>((IEnumerable<IAntlrErrorListener<Symbol>>) this.ErrorListeners);

    public virtual bool Sempred(RuleContext _localctx, int ruleIndex, int actionIndex) => true;

    public virtual bool Precpred(RuleContext localctx, int precedence) => true;

    public virtual void Action(RuleContext _localctx, int ruleIndex, int actionIndex)
    {
    }

    public int State
    {
      get => this._stateNumber;
      set => this._stateNumber = value;
    }

    public abstract IIntStream InputStream { get; }
  }
}
