// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Common.Rcs
// Assembly: Microsoft.TeamFoundation.VersionControl.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 156CCB01-0A1F-468C-A332-06DB9F9B179E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Common.dll

using Microsoft.TeamFoundation.Diff;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.VersionControl.Common
{
  internal class Rcs : DiffOutput
  {
    public override void Output(
      IList<DiffLine> original,
      IList<DiffLine> modified,
      IDiffChange[] diffList)
    {
      foreach (IDiffChange diff in diffList)
      {
        if (diff.ChangeType == DiffChangeType.Delete || diff.ChangeType == DiffChangeType.Change)
          this.Out.WriteLine("d{0:d} {1:d}", (object) (diff.OriginalStart + 1), (object) diff.OriginalLength);
        if (diff.ChangeType == DiffChangeType.Insert || diff.ChangeType == DiffChangeType.Change)
        {
          this.Out.WriteLine("a{0:d} {1:d}", (object) (diff.ChangeType == DiffChangeType.Insert ? diff.OriginalStart : diff.OriginalStart + 1), (object) diff.ModifiedLength);
          for (int modifiedStart = diff.ModifiedStart; modifiedStart < diff.ModifiedStart + diff.ModifiedLength; ++modifiedStart)
            this.Out.Write(modified[modifiedStart].ToString());
        }
      }
    }
  }
}
