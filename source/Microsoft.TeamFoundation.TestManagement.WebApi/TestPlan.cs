// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.TestPlan
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public sealed class TestPlan
  {
    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public int Id { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Url { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ShallowReference Project { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ShallowReference Area { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ShallowReference Build { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Description { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime StartDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime EndDate { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public string Iteration { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime UpdatedDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IdentityRef UpdatedBy { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public IdentityRef Owner { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ShallowReference PreviousBuild { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int Revision { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string State { get; set; }

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

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public ShallowReference RootSuite { get; set; }

    [ClientInternalUseOnly(false)]
    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public string ClientUrl { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ShallowReference BuildDefinition { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ReleaseEnvironmentDefinitionReference ReleaseEnvironmentDefinition { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public TestOutcomeSettings TestOutcomeSettings { get; set; }
  }
}
