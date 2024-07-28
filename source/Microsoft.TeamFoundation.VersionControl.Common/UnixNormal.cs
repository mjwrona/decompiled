// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Common.UnixNormal
// Assembly: Microsoft.TeamFoundation.VersionControl.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 156CCB01-0A1F-468C-A332-06DB9F9B179E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Common.dll

using Microsoft.TeamFoundation.Diff;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.VersionControl.Common
{
  internal class UnixNormal : DiffOutput
  {
    public override void Output(
      IList<DiffLine> original,
      IList<DiffLine> modified,
      IDiffChange[] diffList)
    {
      foreach (IDiffChange diff in diffList)
      {
        this.WriteUnixChangeHeader(diff);
        this.WriteElementRange(original, "< ", diff.OriginalStart, diff.OriginalStart + diff.OriginalLength - 1);
        if (diff.ChangeType == DiffChangeType.Change)
          this.Out.WriteLine("---");
        this.WriteElementRange(modified, "> ", diff.ModifiedStart, diff.ModifiedStart + diff.ModifiedLength - 1);
      }
    }
  }
}
