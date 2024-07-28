// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.TokenStreamRewriter
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Antlr4.Runtime
{
  internal class TokenStreamRewriter
  {
    public const string DefaultProgramName = "default";
    public const int ProgramInitSize = 100;
    public const int MinTokenIndex = 0;
    protected internal readonly ITokenStream tokens;
    protected internal readonly IDictionary<string, IList<TokenStreamRewriter.RewriteOperation>> programs;
    protected internal readonly IDictionary<string, int> lastRewriteTokenIndexes;

    public TokenStreamRewriter(ITokenStream tokens)
    {
      this.tokens = tokens;
      this.programs = (IDictionary<string, IList<TokenStreamRewriter.RewriteOperation>>) new Dictionary<string, IList<TokenStreamRewriter.RewriteOperation>>();
      this.programs["default"] = (IList<TokenStreamRewriter.RewriteOperation>) new List<TokenStreamRewriter.RewriteOperation>(100);
      this.lastRewriteTokenIndexes = (IDictionary<string, int>) new Dictionary<string, int>();
    }

    public ITokenStream TokenStream => this.tokens;

    public virtual void Rollback(int instructionIndex) => this.Rollback("default", instructionIndex);

    public virtual void Rollback(string programName, int instructionIndex)
    {
      IList<TokenStreamRewriter.RewriteOperation> source;
      if (!this.programs.TryGetValue(programName, out source))
        return;
      this.programs[programName] = (IList<TokenStreamRewriter.RewriteOperation>) new List<TokenStreamRewriter.RewriteOperation>(source.Skip<TokenStreamRewriter.RewriteOperation>(0).Take<TokenStreamRewriter.RewriteOperation>(instructionIndex));
    }

    public virtual void DeleteProgram() => this.DeleteProgram("default");

    public virtual void DeleteProgram(string programName) => this.Rollback(programName, 0);

    public virtual void InsertAfter(IToken t, object text) => this.InsertAfter("default", t, text);

    public virtual void InsertAfter(int index, object text) => this.InsertAfter("default", index, text);

    public virtual void InsertAfter(string programName, IToken t, object text) => this.InsertAfter(programName, t.TokenIndex, text);

    public virtual void InsertAfter(string programName, int index, object text) => this.InsertBefore(programName, index + 1, text);

    public virtual void InsertBefore(IToken t, object text) => this.InsertBefore("default", t, text);

    public virtual void InsertBefore(int index, object text) => this.InsertBefore("default", index, text);

    public virtual void InsertBefore(string programName, IToken t, object text) => this.InsertBefore(programName, t.TokenIndex, text);

    public virtual void InsertBefore(string programName, int index, object text)
    {
      TokenStreamRewriter.RewriteOperation rewriteOperation = (TokenStreamRewriter.RewriteOperation) new TokenStreamRewriter.InsertBeforeOp(this.tokens, index, text);
      IList<TokenStreamRewriter.RewriteOperation> program = this.GetProgram(programName);
      rewriteOperation.instructionIndex = program.Count;
      program.Add(rewriteOperation);
    }

    public virtual void Replace(int index, object text) => this.Replace("default", index, index, text);

    public virtual void Replace(int from, int to, object text) => this.Replace("default", from, to, text);

    public virtual void Replace(IToken indexT, object text) => this.Replace("default", indexT, indexT, text);

    public virtual void Replace(IToken from, IToken to, object text) => this.Replace("default", from, to, text);

    public virtual void Replace(string programName, int from, int to, object text)
    {
      if (from > to || from < 0 || to < 0 || to >= this.tokens.Size)
        throw new ArgumentException("replace: range invalid: " + from.ToString() + ".." + to.ToString() + "(size=" + this.tokens.Size.ToString() + ")");
      TokenStreamRewriter.RewriteOperation rewriteOperation = (TokenStreamRewriter.RewriteOperation) new TokenStreamRewriter.ReplaceOp(this.tokens, from, to, text);
      IList<TokenStreamRewriter.RewriteOperation> program = this.GetProgram(programName);
      rewriteOperation.instructionIndex = program.Count;
      program.Add(rewriteOperation);
    }

    public virtual void Replace(string programName, IToken from, IToken to, object text) => this.Replace(programName, from.TokenIndex, to.TokenIndex, text);

    public virtual void Delete(int index) => this.Delete("default", index, index);

    public virtual void Delete(int from, int to) => this.Delete("default", from, to);

    public virtual void Delete(IToken indexT) => this.Delete("default", indexT, indexT);

    public virtual void Delete(IToken from, IToken to) => this.Delete("default", from, to);

    public virtual void Delete(string programName, int from, int to) => this.Replace(programName, from, to, (object) null);

    public virtual void Delete(string programName, IToken from, IToken to) => this.Replace(programName, from, to, (object) null);

    public virtual int LastRewriteTokenIndex => this.GetLastRewriteTokenIndex("default");

    protected internal virtual int GetLastRewriteTokenIndex(string programName)
    {
      int num;
      return !this.lastRewriteTokenIndexes.TryGetValue(programName, out num) ? -1 : num;
    }

    protected internal virtual void SetLastRewriteTokenIndex(string programName, int i) => this.lastRewriteTokenIndexes[programName] = i;

    protected internal virtual IList<TokenStreamRewriter.RewriteOperation> GetProgram(string name)
    {
      IList<TokenStreamRewriter.RewriteOperation> program;
      if (!this.programs.TryGetValue(name, out program))
        program = this.InitializeProgram(name);
      return program;
    }

    private IList<TokenStreamRewriter.RewriteOperation> InitializeProgram(string name)
    {
      IList<TokenStreamRewriter.RewriteOperation> rewriteOperationList = (IList<TokenStreamRewriter.RewriteOperation>) new List<TokenStreamRewriter.RewriteOperation>(100);
      this.programs[name] = rewriteOperationList;
      return rewriteOperationList;
    }

    public virtual string GetText() => this.GetText("default", Interval.Of(0, this.tokens.Size - 1));

    public virtual string GetText(Interval interval) => this.GetText("default", interval);

    public virtual string GetText(string programName, Interval interval)
    {
      IList<TokenStreamRewriter.RewriteOperation> rewrites;
      if (!this.programs.TryGetValue(programName, out rewrites))
        rewrites = (IList<TokenStreamRewriter.RewriteOperation>) null;
      int num1 = interval.a;
      int num2 = interval.b;
      if (num2 > this.tokens.Size - 1)
        num2 = this.tokens.Size - 1;
      if (num1 < 0)
        num1 = 0;
      if (rewrites == null || rewrites.Count == 0)
        return this.tokens.GetText(interval);
      StringBuilder buf = new StringBuilder();
      IDictionary<int, TokenStreamRewriter.RewriteOperation> operationPerIndex = this.ReduceToSingleOperationPerIndex(rewrites);
      int num3 = num1;
      while (num3 <= num2 && num3 < this.tokens.Size)
      {
        TokenStreamRewriter.RewriteOperation rewriteOperation;
        if (operationPerIndex.TryGetValue(num3, out rewriteOperation))
          operationPerIndex.Remove(num3);
        IToken token = this.tokens.Get(num3);
        if (rewriteOperation == null)
        {
          if (token.Type != -1)
            buf.Append(token.Text);
          ++num3;
        }
        else
          num3 = rewriteOperation.Execute(buf);
      }
      if (num2 == this.tokens.Size - 1)
      {
        foreach (TokenStreamRewriter.RewriteOperation rewriteOperation in (IEnumerable<TokenStreamRewriter.RewriteOperation>) operationPerIndex.Values)
        {
          if (rewriteOperation.index >= this.tokens.Size - 1)
            buf.Append(rewriteOperation.text);
        }
      }
      return buf.ToString();
    }

    protected internal virtual IDictionary<int, TokenStreamRewriter.RewriteOperation> ReduceToSingleOperationPerIndex(
      IList<TokenStreamRewriter.RewriteOperation> rewrites)
    {
      for (int index = 0; index < rewrites.Count; ++index)
      {
        TokenStreamRewriter.RewriteOperation rewrite1 = rewrites[index];
        if (rewrite1 != null && rewrite1 is TokenStreamRewriter.ReplaceOp)
        {
          TokenStreamRewriter.ReplaceOp rewrite2 = (TokenStreamRewriter.ReplaceOp) rewrites[index];
          foreach (TokenStreamRewriter.InsertBeforeOp kindOfOp in (IEnumerable<TokenStreamRewriter.InsertBeforeOp>) this.GetKindOfOps<TokenStreamRewriter.InsertBeforeOp>(rewrites, index))
          {
            if (kindOfOp.index == rewrite2.index)
            {
              rewrites[kindOfOp.instructionIndex] = (TokenStreamRewriter.RewriteOperation) null;
              rewrite2.text = (object) (kindOfOp.text.ToString() + (rewrite2.text != null ? rewrite2.text.ToString() : string.Empty));
            }
            else if (kindOfOp.index > rewrite2.index && kindOfOp.index <= rewrite2.lastIndex)
              rewrites[kindOfOp.instructionIndex] = (TokenStreamRewriter.RewriteOperation) null;
          }
          foreach (TokenStreamRewriter.ReplaceOp kindOfOp in (IEnumerable<TokenStreamRewriter.ReplaceOp>) this.GetKindOfOps<TokenStreamRewriter.ReplaceOp>(rewrites, index))
          {
            if (kindOfOp.index >= rewrite2.index && kindOfOp.lastIndex <= rewrite2.lastIndex)
            {
              rewrites[kindOfOp.instructionIndex] = (TokenStreamRewriter.RewriteOperation) null;
            }
            else
            {
              bool flag = kindOfOp.lastIndex < rewrite2.index || kindOfOp.index > rewrite2.lastIndex;
              if (kindOfOp.text == null && rewrite2.text == null && !flag)
              {
                rewrites[kindOfOp.instructionIndex] = (TokenStreamRewriter.RewriteOperation) null;
                rewrite2.index = Math.Min(kindOfOp.index, rewrite2.index);
                rewrite2.lastIndex = Math.Max(kindOfOp.lastIndex, rewrite2.lastIndex);
                Console.Out.WriteLine("new rop " + rewrite2?.ToString());
              }
              else if (!flag)
                throw new ArgumentException("replace op boundaries of " + rewrite2?.ToString() + " overlap with previous " + kindOfOp?.ToString());
            }
          }
        }
      }
      for (int index = 0; index < rewrites.Count; ++index)
      {
        TokenStreamRewriter.RewriteOperation rewrite3 = rewrites[index];
        if (rewrite3 != null && rewrite3 is TokenStreamRewriter.InsertBeforeOp)
        {
          TokenStreamRewriter.InsertBeforeOp rewrite4 = (TokenStreamRewriter.InsertBeforeOp) rewrites[index];
          foreach (TokenStreamRewriter.InsertBeforeOp kindOfOp in (IEnumerable<TokenStreamRewriter.InsertBeforeOp>) this.GetKindOfOps<TokenStreamRewriter.InsertBeforeOp>(rewrites, index))
          {
            if (kindOfOp.index == rewrite4.index)
            {
              rewrite4.text = (object) this.CatOpText(rewrite4.text, kindOfOp.text);
              rewrites[kindOfOp.instructionIndex] = (TokenStreamRewriter.RewriteOperation) null;
            }
          }
          foreach (TokenStreamRewriter.ReplaceOp kindOfOp in (IEnumerable<TokenStreamRewriter.ReplaceOp>) this.GetKindOfOps<TokenStreamRewriter.ReplaceOp>(rewrites, index))
          {
            if (rewrite4.index == kindOfOp.index)
            {
              kindOfOp.text = (object) this.CatOpText(rewrite4.text, kindOfOp.text);
              rewrites[index] = (TokenStreamRewriter.RewriteOperation) null;
            }
            else if (rewrite4.index >= kindOfOp.index && rewrite4.index <= kindOfOp.lastIndex)
              throw new ArgumentException("insert op " + rewrite4?.ToString() + " within boundaries of previous " + kindOfOp?.ToString());
          }
        }
      }
      IDictionary<int, TokenStreamRewriter.RewriteOperation> operationPerIndex = (IDictionary<int, TokenStreamRewriter.RewriteOperation>) new Dictionary<int, TokenStreamRewriter.RewriteOperation>();
      for (int index = 0; index < rewrites.Count; ++index)
      {
        TokenStreamRewriter.RewriteOperation rewrite = rewrites[index];
        if (rewrite != null)
        {
          if (operationPerIndex.ContainsKey(rewrite.index))
            throw new InvalidOperationException("should only be one op per index");
          operationPerIndex[rewrite.index] = rewrite;
        }
      }
      return operationPerIndex;
    }

    protected internal virtual string CatOpText(object a, object b)
    {
      string empty1 = string.Empty;
      string empty2 = string.Empty;
      if (a != null)
        empty1 = a.ToString();
      if (b != null)
        empty2 = b.ToString();
      return empty1 + empty2;
    }

    protected internal virtual IList<T> GetKindOfOps<T>(
      IList<TokenStreamRewriter.RewriteOperation> rewrites,
      int before)
    {
      return (IList<T>) rewrites.Take<TokenStreamRewriter.RewriteOperation>(before).OfType<T>().ToList<T>();
    }

    internal class RewriteOperation
    {
      protected internal readonly ITokenStream tokens;
      protected internal int instructionIndex;
      protected internal int index;
      protected internal object text;

      protected internal RewriteOperation(ITokenStream tokens, int index)
      {
        this.tokens = tokens;
        this.index = index;
      }

      protected internal RewriteOperation(ITokenStream tokens, int index, object text)
      {
        this.tokens = tokens;
        this.index = index;
        this.text = text;
      }

      public virtual int Execute(StringBuilder buf) => this.index;

      public override string ToString()
      {
        string fullName = this.GetType().FullName;
        int num = fullName.IndexOf('$');
        return "<" + Antlr4.Runtime.Sharpen.Runtime.Substring(fullName, num + 1, fullName.Length) + "@" + this.tokens.Get(this.index)?.ToString() + ":\"" + this.text?.ToString() + "\">";
      }
    }

    internal class InsertBeforeOp : TokenStreamRewriter.RewriteOperation
    {
      public InsertBeforeOp(ITokenStream tokens, int index, object text)
        : base(tokens, index, text)
      {
      }

      public override int Execute(StringBuilder buf)
      {
        buf.Append(this.text);
        if (this.tokens.Get(this.index).Type != -1)
          buf.Append(this.tokens.Get(this.index).Text);
        return this.index + 1;
      }
    }

    internal class ReplaceOp : TokenStreamRewriter.RewriteOperation
    {
      protected internal int lastIndex;

      public ReplaceOp(ITokenStream tokens, int from, int to, object text)
        : base(tokens, from, text)
      {
        this.lastIndex = to;
      }

      public override int Execute(StringBuilder buf)
      {
        if (this.text != null)
          buf.Append(this.text);
        return this.lastIndex + 1;
      }

      public override string ToString() => this.text == null ? "<DeleteOp@" + this.tokens.Get(this.index)?.ToString() + ".." + this.tokens.Get(this.lastIndex)?.ToString() + ">" : "<ReplaceOp@" + this.tokens.Get(this.index)?.ToString() + ".." + this.tokens.Get(this.lastIndex)?.ToString() + ":\"" + this.text?.ToString() + "\">";
    }
  }
}
