// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.Position
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

namespace Microsoft.Ajax.Utilities
{
  internal class Position
  {
    private int m_line;
    private int m_char;

    public int Line => this.m_line;

    public int Char => this.m_char;

    public Position() => this.m_line = 1;

    public Position(int line, int character)
    {
      this.m_line = line;
      this.m_char = character;
    }

    public void NextLine()
    {
      ++this.m_line;
      this.m_char = 0;
    }

    public void NextChar() => ++this.m_char;

    public void PreviousChar() => --this.m_char;

    public Position Clone() => new Position(this.m_line, this.m_char);
  }
}
