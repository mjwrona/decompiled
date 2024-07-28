// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Charting.TestAuthoringDetails
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;

namespace Microsoft.TeamFoundation.TestManagement.Server.Charting
{
  public class TestAuthoringDetails
  {
    public int PointId { get; set; }

    public Guid TesterId { get; set; }

    public int ConfigurationId { get; set; }

    public string ConfigurationName { get; set; }

    public int SuiteId { get; set; }

    public string SuiteName { get; set; }

    public byte State { get; set; }

    public DateTime LastUpdated { get; set; }

    public Guid? RunBy { get; set; }

    public byte? Priority { get; set; }

    public bool? IsAutomated { get; set; }

    public Microsoft.TeamFoundation.TestManagement.WebApi.TestAuthoringDetails ToWebApiModel() => new Microsoft.TeamFoundation.TestManagement.WebApi.TestAuthoringDetails()
    {
      PointId = this.PointId,
      SuiteId = this.SuiteId,
      ConfigurationId = this.ConfigurationId,
      TesterId = this.TesterId,
      State = (TestPointState) this.State,
      LastUpdated = this.LastUpdated,
      RunBy = this.RunBy,
      Priority = this.Priority,
      IsAutomated = this.IsAutomated
    };

    public static TestAuthoringDetails FromWebApiModel(Microsoft.TeamFoundation.TestManagement.WebApi.TestAuthoringDetails webApiModel) => new TestAuthoringDetails()
    {
      PointId = webApiModel.PointId,
      SuiteId = webApiModel.SuiteId,
      ConfigurationId = webApiModel.ConfigurationId,
      TesterId = webApiModel.TesterId,
      State = (byte) webApiModel.State,
      LastUpdated = webApiModel.LastUpdated,
      RunBy = webApiModel.RunBy,
      Priority = webApiModel.Priority,
      IsAutomated = webApiModel.IsAutomated
    };
  }
}
