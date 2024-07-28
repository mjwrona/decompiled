// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.Common.UnixNormal
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F16CDF2D-8103-4EAE-A2A8-4FA5B1C1BE58
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.Server.Common.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.CodeReview.Server.Common
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
