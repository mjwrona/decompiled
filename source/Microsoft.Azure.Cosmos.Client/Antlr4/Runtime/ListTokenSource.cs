// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.ListTokenSource
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections.Generic;

namespace Antlr4.Runtime
{
  internal class ListTokenSource : ITokenSource
  {
    protected internal readonly IList<IToken> tokens;
    private readonly string sourceName;
    protected internal int i;
    protected internal IToken eofToken;
    private ITokenFactory _factory = CommonTokenFactory.Default;

    public ListTokenSource(IList<IToken> tokens)
      : this(tokens, (string) null)
    {
    }

    public ListTokenSource(IList<IToken> tokens, string sourceName)
    {
      this.tokens = tokens != null ? tokens : throw new ArgumentNullException("tokens cannot be null");
      this.sourceName = sourceName;
    }

    public virtual int Column
    {
      get
      {
        if (this.i < this.tokens.Count)
          return this.tokens[this.i].Column;
        if (this.eofToken != null)
          return this.eofToken.Column;
        if (this.tokens.Count <= 0)
          return 0;
        IToken token = this.tokens[this.tokens.Count - 1];
        string text = token.Text;
        if (text != null)
        {
          int num = text.LastIndexOf('\n');
          if (num >= 0)
            return text.Length - num - 1;
        }
        return token.Column + token.StopIndex - token.StartIndex + 1;
      }
    }

    public virtual IToken NextToken()
    {
      if (this.i >= this.tokens.Count)
      {
        if (this.eofToken == null)
        {
          int start = -1;
          if (this.tokens.Count > 0)
          {
            int stopIndex = this.tokens[this.tokens.Count - 1].StopIndex;
            if (stopIndex != -1)
              start = stopIndex + 1;
          }
          int stop = Math.Max(-1, start - 1);
          this.eofToken = this._factory.Create(Tuple.Create<ITokenSource, ICharStream>((ITokenSource) this, this.InputStream), -1, "EOF", 0, start, stop, this.Line, this.Column);
        }
        return this.eofToken;
      }
      IToken token = this.tokens[this.i];
      if (this.i == this.tokens.Count - 1 && token.Type == -1)
        this.eofToken = token;
      ++this.i;
      return token;
    }

    public virtual int Line
    {
      get
      {
        if (this.i < this.tokens.Count)
          return this.tokens[this.i].Line;
        if (this.eofToken != null)
          return this.eofToken.Line;
        if (this.tokens.Count <= 0)
          return 1;
        IToken token = this.tokens[this.tokens.Count - 1];
        int line = token.Line;
        string text = token.Text;
        if (text != null)
        {
          for (int index = 0; index < text.Length; ++index)
          {
            if (text[index] == '\n')
              ++line;
          }
        }
        return line;
      }
    }

    public virtual ICharStream InputStream
    {
      get
      {
        if (this.i < this.tokens.Count)
          return this.tokens[this.i].InputStream;
        if (this.eofToken != null)
          return this.eofToken.InputStream;
        return this.tokens.Count > 0 ? this.tokens[this.tokens.Count - 1].InputStream : (ICharStream) null;
      }
    }

    public virtual string SourceName
    {
      get
      {
        if (this.sourceName != null)
          return this.sourceName;
        ICharStream inputStream = this.InputStream;
        return inputStream != null ? inputStream.SourceName : "List";
      }
    }

    public virtual ITokenFactory TokenFactory
    {
      get => this._factory;
      set => this._factory = value;
    }
  }
}
