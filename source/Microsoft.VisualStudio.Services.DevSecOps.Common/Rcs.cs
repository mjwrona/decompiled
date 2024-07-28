// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DevSecOps.Common.Rcs
// Assembly: Microsoft.VisualStudio.Services.DevSecOps.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 072F1303-F456-426E-A1CB-C0838641751B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DevSecOps.Common.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.DevSecOps.Common
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
