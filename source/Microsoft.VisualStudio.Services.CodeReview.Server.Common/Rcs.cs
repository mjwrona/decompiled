// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.Common.Rcs
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F16CDF2D-8103-4EAE-A2A8-4FA5B1C1BE58
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.Server.Common.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.CodeReview.Server.Common
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
