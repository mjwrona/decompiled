// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Common.Context
// Assembly: Microsoft.TeamFoundation.VersionControl.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 156CCB01-0A1F-468C-A332-06DB9F9B179E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Common.dll

using Microsoft.TeamFoundation.Diff;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.TeamFoundation.VersionControl.Common
{
  internal class Context : DiffOutput
  {
    public override void Output(
      IList<DiffLine> original,
      IList<DiffLine> modified,
      IDiffChange[] diffList)
    {
      int contextLines = Math.Max(this.Options.ContextLines, 0);
      if (diffList.Length != 0)
      {
        this.Out.WriteLine("*** {0}", (object) this.Options.SourceLabel);
        this.Out.WriteLine("--- {0}", (object) this.Options.TargetLabel);
      }
      int index1 = 0;
      while (index1 < diffList.Length)
      {
        int hunkStart = index1;
        int hunkEnd = this.ComputeHunkEnd(diffList, hunkStart, contextLines);
        int startIndex = Math.Max(diffList[hunkStart].OriginalStart - contextLines, 0);
        int num1 = Math.Max(diffList[hunkStart].ModifiedStart - contextLines, 0);
        int endIndex1 = Math.Min(diffList[hunkEnd].OriginalStart + diffList[hunkEnd].OriginalLength + contextLines - 1, original.Count - 1);
        int num2 = Math.Min(diffList[hunkEnd].ModifiedStart + diffList[hunkEnd].ModifiedLength + contextLines - 1, modified.Count - 1);
        this.Out.WriteLine("***************");
        bool flag1 = false;
        bool flag2 = false;
        for (int index2 = hunkStart; index2 <= hunkEnd; ++index2)
        {
          switch (diffList[index2].ChangeType)
          {
            case DiffChangeType.Insert:
              flag2 = true;
              break;
            case DiffChangeType.Delete:
              flag1 = true;
              break;
            case DiffChangeType.Change:
              flag1 = true;
              flag2 = true;
              break;
          }
        }
        this.Out.WriteLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "*** {0:d},{1:d} ****", (object) (startIndex + 1), (object) (endIndex1 + 1)));
        if (flag1)
        {
          index1 = hunkStart;
          this.WriteElementRange(original, "  ", startIndex, diffList[hunkStart].OriginalStart - 1);
          for (; index1 <= hunkEnd; ++index1)
          {
            IDiffChange diff = diffList[index1];
            int endIndex2 = diff.OriginalStart + diff.OriginalLength - 1;
            int modifiedStart = diff.ModifiedStart;
            int modifiedLength = diff.ModifiedLength;
            this.WriteElementRange(original, diff.ChangeType == DiffChangeType.Delete ? "- " : "! ", diff.OriginalStart, endIndex2);
            if (index1 < hunkEnd)
              this.WriteElementRange(original, "  ", endIndex2 + 1, diffList[index1 + 1].OriginalStart - 1);
          }
          this.WriteElementRange(original, "  ", diffList[hunkEnd].OriginalStart + diffList[hunkEnd].OriginalLength, endIndex1);
        }
        this.Out.WriteLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "--- {0:d},{1:d} ----", (object) (num1 + 1), (object) (num2 + 1)));
        if (flag2)
        {
          index1 = hunkStart;
          this.WriteElementRange(original, "  ", startIndex, diffList[hunkStart].OriginalStart - 1);
          for (; index1 <= hunkEnd; ++index1)
          {
            IDiffChange diff = diffList[index1];
            int num3 = diff.OriginalStart + diff.OriginalLength - 1;
            int endIndex3 = diff.ModifiedStart + diff.ModifiedLength - 1;
            this.WriteElementRange(modified, diff.ChangeType == DiffChangeType.Insert ? "+ " : "! ", diff.ModifiedStart, endIndex3);
            if (index1 < hunkEnd)
              this.WriteElementRange(original, "  ", num3 + 1, diffList[index1 + 1].OriginalStart - 1);
          }
          this.WriteElementRange(original, "  ", diffList[hunkEnd].OriginalStart + diffList[hunkEnd].OriginalLength, endIndex1);
        }
      }
    }
  }
}
