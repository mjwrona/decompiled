// Decompiled with JetBrains decompiler
// Type: WebGrease.Css.PrinterFormatter
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System.Text;

namespace WebGrease.Css
{
  internal sealed class PrinterFormatter
  {
    private readonly StringBuilder _buffer = new StringBuilder(1024);
    private int _indentLevel;

    public bool PrettyPrint { get; set; }

    public char IndentCharacter { get; set; }

    public int IndentSize { get; set; }

    public override string ToString() => this._buffer.ToString();

    public void Append(string content) => this._buffer.Append(content);

    public void Append(char content) => this._buffer.Append(content);

    public void AppendLine(char content)
    {
      if (this.PrettyPrint)
        this._buffer.AppendLine(content.ToString());
      else
        this._buffer.Append(content);
    }

    public void AppendLine()
    {
      if (!this.PrettyPrint)
        return;
      this._buffer.AppendLine();
    }

    public void Remove(int startIndex, int length) => this._buffer.Remove(startIndex, length);

    public int Length() => this._buffer.Length;

    public void IncrementIndentLevel() => ++this._indentLevel;

    public void DecrementIndentLevel()
    {
      if (this._indentLevel <= 0)
        return;
      --this._indentLevel;
    }

    public void WriteIndent()
    {
      if (!this.PrettyPrint)
        return;
      this._buffer.Append(new string(this.IndentCharacter, this._indentLevel * this.IndentSize));
    }
  }
}
