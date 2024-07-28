// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.CoverageChangeExtension
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class CoverageChangeExtension : CoverageChange
  {
    public int dataSpaceId { get; set; }

    public CoverageChangeExtension(CoverageChange c, int id)
    {
      this.dataSpaceId = id;
      this.BuildConfiguration = c.BuildConfiguration;
      this.ChangeType = c.ChangeType;
      this.ConfigurationId = c.ConfigurationId;
      this.CoverageChangeId = c.CoverageChangeId;
      this.SessionId = c.SessionId;
      this.TestResultId = c.TestResultId;
      this.TestRunId = c.TestRunId;
    }
  }
}
