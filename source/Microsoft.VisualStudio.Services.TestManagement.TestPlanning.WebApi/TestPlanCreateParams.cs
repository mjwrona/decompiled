// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPlanCreateParams
// Assembly: Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6FBA62B7-DF7C-48A4-98F0-AF0ACAEA014F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.dll

using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi
{
  [DataContract]
  public class TestPlanCreateParams
  {
    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string AreaPath { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Description { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime? StartDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime? EndDate { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public string Iteration { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public IdentityRef Owner { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string State { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int BuildId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public BuildDefinitionReference BuildDefinition { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ReleaseEnvironmentDefinitionReference ReleaseEnvironmentDefinition { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public TestOutcomeSettings TestOutcomeSettings { get; set; }

    [ClientInternalUseOnly(false)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public TestEnvironment AutomatedTestEnvironment { get; set; }

    [ClientInternalUseOnly(false)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public TestEnvironment ManualTestEnvironment { get; set; }

    [ClientInternalUseOnly(false)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public TestSettings ManualTestSettings { get; set; }

    [ClientInternalUseOnly(false)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public TestSettings AutomatedTestSettings { get; set; }
  }
}
