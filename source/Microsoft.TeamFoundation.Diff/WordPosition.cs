// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Diff.WordPosition
// Assembly: Microsoft.TeamFoundation.Diff, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F647AACF-6EF1-4C0C-AB27-20317A054A39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Diff.dll

namespace Microsoft.TeamFoundation.Diff
{
  public class WordPosition
  {
    public readonly int Line;
    public readonly int Start;
    public readonly int Length;
    public readonly int FullStart;
    public readonly string Word;

    public WordPosition(string word, int line, int start, int length, int fullstart)
    {
      this.Start = start;
      this.FullStart = fullstart;
      this.Length = length;
      this.Word = word;
      this.Line = line;
    }
  }
}
