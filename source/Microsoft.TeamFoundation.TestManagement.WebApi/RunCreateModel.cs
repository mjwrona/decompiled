// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.RunCreateModel
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public class RunCreateModel
  {
    public RunCreateModel(
      string name = "",
      string iteration = "",
      int[] pointIds = null,
      ShallowReference plan = null,
      ShallowReference testSettings = null,
      int buildId = 0,
      string state = "",
      bool? isAutomated = null,
      string errorMessage = "",
      string dueDate = "",
      string type = "",
      string controller = "",
      string buildDropLocation = "",
      string comment = "",
      string testEnvironmentId = "",
      string startedDate = "",
      string completedDate = "",
      int[] configIds = null,
      RunFilter filter = null,
      ShallowReference dtlTestEnvironment = null,
      DtlEnvironmentDetails environmentDetails = null,
      IdentityRef owner = null,
      string buildPlatform = "",
      string buildFlavor = "",
      string releaseUri = "",
      string releaseEnvironmentUri = "",
      TimeSpan? runTimeout = null,
      string testconfigurationsMapping = "",
      ShallowReference dtlAutEnvironment = null)
    {
      this.Name = name;
      this.Iteration = iteration;
      this.PointIds = pointIds;
      if (buildId != 0)
        this.Build = new ShallowReference()
        {
          Id = buildId.ToString()
        };
      this.State = state;
      this.Automated = isAutomated;
      this.Controller = controller;
      this.Plan = plan;
      this.ErrorMessage = errorMessage;
      this.DueDate = dueDate;
      this.Comment = comment;
      this.TestSettings = testSettings;
      this.TestEnvironmentId = testEnvironmentId;
      this.StartDate = startedDate;
      this.CompleteDate = completedDate;
      this.Owner = owner;
      this.Filter = filter;
      this.DtlTestEnvironment = dtlTestEnvironment;
      this.DtlAutEnvironment = dtlAutEnvironment;
      this.EnvironmentDetails = environmentDetails;
      this.Type = type;
      this.BuildDropLocation = buildDropLocation;
      this.BuildPlatform = buildPlatform;
      this.BuildFlavor = buildFlavor;
      this.ConfigurationIds = configIds;
      this.ReleaseUri = releaseUri;
      this.ReleaseEnvironmentUri = releaseEnvironmentUri;
      this.RunTimeout = runTimeout.HasValue ? runTimeout.Value : TimeSpan.Zero;
      this.TestConfigurationsMapping = testconfigurationsMapping;
      this.CustomTestFields = new List<CustomTestField>();
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Iteration { get; private set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ShallowReference Build { get; private set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string State { get; private set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool? Automated { get; private set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public ShallowReference Plan { get; private set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ErrorMessage { get; private set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string DueDate { get; private set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Type { get; private set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Controller { get; private set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string BuildDropLocation { get; private set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string BuildPlatform { get; private set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string BuildFlavor { get; private set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Comment { get; private set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ShallowReference TestSettings { get; private set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string TestEnvironmentId { get; private set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string StartDate { get; private set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string CompleteDate { get; private set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int[] PointIds { get; private set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public int[] ConfigurationIds { get; private set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IdentityRef Owner { get; private set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public RunFilter Filter { get; private set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ShallowReference DtlTestEnvironment { get; private set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ShallowReference DtlAutEnvironment { get; private set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DtlEnvironmentDetails EnvironmentDetails { get; private set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ReleaseUri { get; private set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ReleaseEnvironmentUri { get; private set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public TimeSpan RunTimeout { get; private set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string TestConfigurationsMapping { get; private set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public List<CustomTestField> CustomTestFields { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string SourceWorkflow { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public BuildConfiguration BuildReference { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ReleaseReference ReleaseReference { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public PipelineReference PipelineReference { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IList<RunSummaryModel> RunSummary { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IList<TestTag> Tags { get; set; }
  }
}
