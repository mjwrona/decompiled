// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Core.Cursor
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;

namespace YamlDotNet.Core
{
  [Serializable]
  public class Cursor
  {
    public int Index { get; set; }

    public int Line { get; set; }

    public int LineOffset { get; set; }

    public Cursor() => this.Line = 1;

    public Cursor(Cursor cursor)
    {
      this.Index = cursor.Index;
      this.Line = cursor.Line;
      this.LineOffset = cursor.LineOffset;
    }

    public YamlDotNet.Core.Mark Mark() => new YamlDotNet.Core.Mark(this.Index, this.Line, this.LineOffset + 1);

    public void Skip()
    {
      ++this.Index;
      ++this.LineOffset;
    }

    public void SkipLineByOffset(int offset)
    {
      this.Index += offset;
      ++this.Line;
      this.LineOffset = 0;
    }

    public void ForceSkipLineAfterNonBreak()
    {
      if (this.LineOffset == 0)
        return;
      ++this.Line;
      this.LineOffset = 0;
    }
  }
}
