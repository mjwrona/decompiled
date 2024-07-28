// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.CssContext
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

namespace Microsoft.Ajax.Utilities
{
  internal class CssContext
  {
    private Position m_start;
    private Position m_end;

    public Position Start => this.m_start;

    public Position End => this.m_end;

    internal CssContext()
    {
      this.m_start = new Position();
      this.m_end = new Position();
    }

    internal CssContext(Position start, Position end)
    {
      this.m_start = start.Clone();
      this.m_end = end.Clone();
    }

    public void Advance() => this.m_start = this.m_end.Clone();

    public CssContext Clone() => new CssContext(this.m_start.Clone(), this.m_end.Clone());

    public void Reset(int line, int column)
    {
      this.m_start = new Position(line, column);
      this.m_end = new Position(line, column);
    }
  }
}
