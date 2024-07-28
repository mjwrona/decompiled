// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DevSecOps.Common.WordPosition
// Assembly: Microsoft.VisualStudio.Services.DevSecOps.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 072F1303-F456-426E-A1CB-C0838641751B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DevSecOps.Common.dll

namespace Microsoft.VisualStudio.Services.DevSecOps.Common
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
