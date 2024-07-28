// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DevSecOps.Common.UnixNormal
// Assembly: Microsoft.VisualStudio.Services.DevSecOps.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 072F1303-F456-426E-A1CB-C0838641751B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DevSecOps.Common.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.DevSecOps.Common
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
