// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.PlanUpdateModel
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public class PlanUpdateModel
  {
    [DataMember(Name = "startDate")]
    public string StartDate;
    [DataMember(Name = "endDate")]
    public string EndDate;

    public PlanUpdateModel(
      string name = "",
      string description = "",
      ShallowReference area = null,
      string iteration = "",
      IdentityRef owner = null,
      string startDate = "",
      string endDate = "",
      TestEnvironment manualTestEnvironment = null,
      TestEnvironment automatedTestEnvironment = null,
      TestSettings manualTestSettings = null,
      TestSettings automatedTestSettings = null,
      ShallowReference build = null,
      string state = "",
      string status = "")
    {
      this.Name = name;
      this.Description = description;
      if (area != null)
        this.Area = new ShallowReference(area);
      this.Iteration = iteration;
      if (owner != null)
        this.Owner = new IdentityRef()
        {
          Id = owner.Id,
          DisplayName = owner.DisplayName
        };
      this.StartDate = startDate;
      this.EndDate = endDate;
      if (manualTestEnvironment != null)
        this.ManualTestEnvironment = new TestEnvironment()
        {
          EnvironmentId = manualTestEnvironment.EnvironmentId
        };
      if (automatedTestEnvironment != null)
        this.AutomatedTestEnvironment = new TestEnvironment()
        {
          EnvironmentId = automatedTestEnvironment.EnvironmentId
        };
      if (manualTestSettings != null)
        this.ManualTestSettings = new TestSettings()
        {
          TestSettingsId = manualTestSettings.TestSettingsId
        };
      if (automatedTestSettings != null)
        this.AutomatedTestSettings = new TestSettings()
        {
          TestSettingsId = automatedTestSettings.TestSettingsId
        };
      if (build != null)
        this.Build = new ShallowReference(build);
      this.State = state;
      this.Status = status;
    }

    [DataMember(Name = "name")]
    public string Name { get; private set; }

    [DataMember(Name = "description")]
    public string Description { get; private set; }

    [DataMember(Name = "area")]
    public ShallowReference Area { get; private set; }

    [DataMember(Name = "iteration")]
    public string Iteration { get; private set; }

    [DataMember(Name = "owner")]
    public IdentityRef Owner { get; private set; }

    [ClientInternalUseOnly(false)]
    [DataMember(Name = "manualTestEnvironment")]
    public TestEnvironment ManualTestEnvironment { get; private set; }

    [ClientInternalUseOnly(false)]
    [DataMember(Name = "automatedTestEnvironment")]
    public TestEnvironment AutomatedTestEnvironment { get; private set; }

    [ClientInternalUseOnly(false)]
    [DataMember(Name = "manualTestSettings")]
    public TestSettings ManualTestSettings { get; private set; }

    [ClientInternalUseOnly(false)]
    [DataMember(Name = "automatedTestSettings")]
    public TestSettings AutomatedTestSettings { get; private set; }

    [DataMember(Name = "build")]
    public ShallowReference Build { get; private set; }

    [DataMember(Name = "configurationIds")]
    public int[] ConfigurationIds { get; private set; }

    [DataMember(Name = "state")]
    public string State { get; private set; }

    [ClientInternalUseOnly(false)]
    [DataMember(Name = "status")]
    public string Status { get; private set; }

    [DataMember(Name = "buildDefinition")]
    public ShallowReference BuildDefinition { get; set; }

    [DataMember(Name = "releaseEnvironmentDefinition")]
    public ReleaseEnvironmentDefinitionReference ReleaseEnvironmentDefinition { get; set; }

    [DataMember(Name = "testOutcomeSettings")]
    public TestOutcomeSettings TestOutcomeSettings { get; set; }
  }
}
