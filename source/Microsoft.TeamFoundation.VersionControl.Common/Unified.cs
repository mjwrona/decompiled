// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Common.Unified
// Assembly: Microsoft.TeamFoundation.VersionControl.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 156CCB01-0A1F-468C-A332-06DB9F9B179E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Common.dll

using Microsoft.TeamFoundation.Diff;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.TeamFoundation.VersionControl.Common
{
  internal class Unified : DiffOutput
  {
    public override void Output(
      IList<DiffLine> original,
      IList<DiffLine> modified,
      IDiffChange[] diffList)
    {
      int contextLines = Math.Max(this.Options.ContextLines, 0);
      if (diffList.Length != 0)
      {
        this.Out.WriteLine("--- {0}", (object) this.Options.SourceLabel);
        this.Out.WriteLine("+++ {0}", (object) this.Options.TargetLabel);
      }
      int index = 0;
      while (index < diffList.Length)
      {
        int hunkStart = index;
        int hunkEnd = this.ComputeHunkEnd(diffList, hunkStart, contextLines);
        int startIndex = Math.Max(diffList[hunkStart].OriginalStart - contextLines, 0);
        int num1 = Math.Max(diffList[hunkStart].ModifiedStart - contextLines, 0);
        int endIndex1 = Math.Min(diffList[hunkEnd].OriginalStart + diffList[hunkEnd].OriginalLength + contextLines - 1, original.Count - 1);
        int num2 = Math.Min(diffList[hunkEnd].ModifiedStart + diffList[hunkEnd].ModifiedLength + contextLines - 1, modified.Count - 1);
        this.Out.WriteLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "@@ -{0:d},{1:d} +{2:d},{3:d} @@", (object) (startIndex + 1), (object) (endIndex1 - startIndex + 1), (object) (num1 + 1), (object) (num2 - num1 + 1)));
        bool flag = false;
        this.WriteElementRange(original, " ", startIndex, diffList[hunkStart].OriginalStart - 1);
        for (; index <= hunkEnd; ++index)
        {
          IDiffChange diff = diffList[index];
          int endIndex2 = diff.OriginalStart + diff.OriginalLength - 1;
          int endIndex3 = diff.ModifiedStart + diff.ModifiedLength - 1;
          this.WriteElementRange(original, "-", diff.OriginalStart, endIndex2);
          if (endIndex2 >= original.Count - 1 && original.Count > 0)
          {
            DiffLine diffLine = original[original.Count - 1];
            if (diffLine != null && diffLine.EndOfLineTerminator == EndOfLineTerminator.None)
            {
              this.Out.WriteLine("\\ No newline at end of file");
              flag = true;
            }
          }
          this.WriteElementRange(modified, "+", diff.ModifiedStart, endIndex3);
          if (endIndex3 >= modified.Count - 1 && modified.Count > 0)
          {
            DiffLine diffLine = modified[modified.Count - 1];
            if (diffLine != null && diffLine.EndOfLineTerminator == EndOfLineTerminator.None)
              this.Out.WriteLine("\\ No newline at end of file");
          }
          if (index < hunkEnd)
            this.WriteElementRange(original, " ", endIndex2 + 1, diffList[index + 1].OriginalStart - 1);
        }
        this.WriteElementRange(original, " ", diffList[hunkEnd].OriginalStart + diffList[hunkEnd].OriginalLength, endIndex1);
        if (!flag && endIndex1 >= original.Count - 1 && original.Count > 0)
        {
          DiffLine diffLine = original[original.Count - 1];
          if (diffLine != null && diffLine.EndOfLineTerminator == EndOfLineTerminator.None)
            this.Out.WriteLine("\\ No newline at end of file");
        }
      }
    }
  }
}
