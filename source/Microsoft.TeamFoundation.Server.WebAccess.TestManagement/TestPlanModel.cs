// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TestPlanModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.TestManagement.Server;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  [DataContract]
  internal class TestPlanModel
  {
    public TestPlanModel()
    {
    }

    public TestPlanModel(TestPlan plan, string clientUrl)
    {
      this.Id = plan.PlanId;
      this.Name = plan.Name;
      this.RootSuiteId = plan.RootSuiteId;
      this.Iteration = plan.Iteration;
      this.BuildUri = plan.BuildUri;
      this.AreaPath = plan.AreaPath;
      this.BuildDefinitionId = plan.BuildDefinitionId;
      if (plan.ReleaseEnvDef != null)
        this.ReleaseEnvironmentDefinition = new ReleaseEnvironmentDefinitionModel(plan.ReleaseEnvDef.ReleaseDefinitionId, plan.ReleaseEnvDef.ReleaseEnvDefinitionId);
      this.ClientUrl = clientUrl;
    }

    [DataMember(Name = "id")]
    public int Id { get; set; }

    [DataMember(Name = "name")]
    public string Name { get; set; }

    [DataMember(Name = "rootSuiteId")]
    public int RootSuiteId { get; set; }

    [DataMember(Name = "iteration")]
    public string Iteration { get; set; }

    [DataMember(Name = "buildUri")]
    public string BuildUri { get; set; }

    [DataMember(Name = "areaPath")]
    public string AreaPath { get; set; }

    [DataMember(Name = "clientUrl")]
    public string ClientUrl { get; set; }

    [DataMember(Name = "buildDefinitionId")]
    public int BuildDefinitionId { get; set; }

    [DataMember(Name = "releaseEnvironmentDefinition")]
    public ReleaseEnvironmentDefinitionModel ReleaseEnvironmentDefinition { get; set; }
  }
}
