// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TestPlanCreationRequestModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.TestManagement.Server;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  [DataContract]
  public class TestPlanCreationRequestModel
  {
    public TestPlanCreationRequestModel()
    {
    }

    public TestPlanCreationRequestModel(TestPlan plan)
    {
      this.Name = plan.Name;
      this.Iteration = plan.Iteration;
      this.AreaPath = plan.AreaPath;
      this.StartDate = plan.StartDate;
      this.EndDate = plan.EndDate;
      this.Owner = plan.Owner;
    }

    [DataMember(Name = "name")]
    public string Name { get; set; }

    [DataMember(Name = "areaPath")]
    public string AreaPath { get; set; }

    [DataMember(Name = "iteration")]
    public string Iteration { get; set; }

    [DataMember(Name = "startDate")]
    public DateTime StartDate { get; set; }

    [DataMember(Name = "endDate")]
    public DateTime EndDate { get; set; }

    [DataMember(Name = "owner")]
    public Guid Owner { get; set; }
  }
}
