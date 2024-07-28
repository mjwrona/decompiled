// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.CommonToken
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Misc;
using System;

namespace Antlr4.Runtime
{
  [Serializable]
  internal class CommonToken : IWritableToken, IToken
  {
    private const long serialVersionUID = -6708843461296520577;
    protected internal static readonly Tuple<ITokenSource, ICharStream> EmptySource = Tuple.Create<ITokenSource, ICharStream>((ITokenSource) null, (ICharStream) null);
    private int _type;
    private int _line;
    protected internal int charPositionInLine = -1;
    private int _channel;
    [NotNull]
    protected internal Tuple<ITokenSource, ICharStream> source;
    private string _text;
    protected internal int index = -1;
    protected internal int start;
    protected internal int stop;

    public CommonToken(int type)
    {
      this._type = type;
      this.source = CommonToken.EmptySource;
    }

    public CommonToken(
      Tuple<ITokenSource, ICharStream> source,
      int type,
      int channel,
      int start,
      int stop)
    {
      this.source = source;
      this._type = type;
      this._channel = channel;
      this.start = start;
      this.stop = stop;
      if (source.Item1 == null)
        return;
      this._line = source.Item1.Line;
      this.charPositionInLine = source.Item1.Column;
    }

    public CommonToken(int type, string text)
    {
      this._type = type;
      this._channel = 0;
      this._text = text;
      this.source = CommonToken.EmptySource;
    }

    public CommonToken(IToken oldToken)
    {
      this._type = oldToken.Type;
      this._line = oldToken.Line;
      this.index = oldToken.TokenIndex;
      this.charPositionInLine = oldToken.Column;
      this._channel = oldToken.Channel;
      this.start = oldToken.StartIndex;
      this.stop = oldToken.StopIndex;
      if (oldToken is CommonToken)
      {
        this._text = ((CommonToken) oldToken)._text;
        this.source = ((CommonToken) oldToken).source;
      }
      else
      {
        this._text = oldToken.Text;
        this.source = Tuple.Create<ITokenSource, ICharStream>(oldToken.TokenSource, oldToken.InputStream);
      }
    }

    public virtual int Type
    {
      get => this._type;
      set => this._type = value;
    }

    public virtual int Line
    {
      get => this._line;
      set => this._line = value;
    }

    public virtual string Text
    {
      get
      {
        if (this._text != null)
          return this._text;
        ICharStream inputStream = this.InputStream;
        if (inputStream == null)
          return (string) null;
        int size = inputStream.Size;
        return this.start < size && this.stop < size ? inputStream.GetText(Interval.Of(this.start, this.stop)) : "<EOF>";
      }
      set => this._text = value;
    }

    public virtual int Column
    {
      get => this.charPositionInLine;
      set => this.charPositionInLine = value;
    }

    public virtual int Channel
    {
      get => this._channel;
      set => this._channel = value;
    }

    public virtual int StartIndex
    {
      get => this.start;
      set => this.start = value;
    }

    public virtual int StopIndex
    {
      get => this.stop;
      set => this.stop = value;
    }

    public virtual int TokenIndex
    {
      get => this.index;
      set => this.index = value;
    }

    public virtual ITokenSource TokenSource => this.source.Item1;

    public virtual ICharStream InputStream => this.source.Item2;

    public override string ToString()
    {
      string str1 = string.Empty;
      if (this._channel > 0)
        str1 = ",channel=" + this._channel.ToString();
      string text = this.Text;
      string str2 = text == null ? "<no text>" : text.Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t");
      string[] strArray = new string[17];
      strArray[0] = "[@";
      int num = this.TokenIndex;
      strArray[1] = num.ToString();
      strArray[2] = ",";
      strArray[3] = this.start.ToString();
      strArray[4] = ":";
      strArray[5] = this.stop.ToString();
      strArray[6] = "='";
      strArray[7] = str2;
      strArray[8] = "',<";
      strArray[9] = this._type.ToString();
      strArray[10] = ">";
      strArray[11] = str1;
      strArray[12] = ",";
      strArray[13] = this._line.ToString();
      strArray[14] = ":";
      num = this.Column;
      strArray[15] = num.ToString();
      strArray[16] = "]";
      return string.Concat(strArray);
    }
  }
}
