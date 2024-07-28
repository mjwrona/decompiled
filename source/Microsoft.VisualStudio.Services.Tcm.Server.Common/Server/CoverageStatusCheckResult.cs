// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.CoverageStatusCheckResult
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class CoverageStatusCheckResult
  {
    public string Source { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public CoverageStatusCheckState State { get; set; }

    public List<CoverageStatusCheckResult> SubResults { get; set; }

    public Dictionary<string, object> Properties { get; set; }
  }
}
