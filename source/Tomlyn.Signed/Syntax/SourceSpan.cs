// Decompiled with JetBrains decompiler
// Type: Tomlyn.Syntax.SourceSpan
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll


#nullable enable
namespace Tomlyn.Syntax
{
  public struct SourceSpan
  {
    public string FileName;
    public TextPosition Start;
    public TextPosition End;

    public SourceSpan(string fileName, TextPosition start, TextPosition end)
    {
      this.FileName = fileName;
      this.Start = start;
      this.End = end;
    }

    public int Offset => this.Start.Offset;

    public int Length => this.End.Offset - this.Start.Offset + 1;

    public override string ToString() => string.Format("{0}{1}-{2}", (object) this.FileName, (object) this.Start, (object) this.End);

    public string ToStringSimple() => string.Format("{0}{1}", (object) this.FileName, (object) this.Start);
  }
}
