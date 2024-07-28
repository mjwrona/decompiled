// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.Infrastructure.SqlHints
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

namespace Microsoft.VisualStudio.Services.Analytics.Model.Infrastructure
{
  internal struct SqlHints
  {
    public SqlHints()
    {
      // ISSUE: reference to a compiler-generated field
      this.\u003CEnableParallelPlan\u003Ek__BackingField = (string) null;
      this.RollupMaxdop = "";
      this.BurndownMaxdop = "";
      this.MashupMaxdop = "";
      this.RollupForceOrder = "";
    }

    public string RollupMaxdop { get; set; }

    public string BurndownMaxdop { get; set; }

    public string MashupMaxdop { get; set; }

    public string RollupForceOrder { get; set; }

    public string EnableParallelPlan { get; set; }
  }
}
