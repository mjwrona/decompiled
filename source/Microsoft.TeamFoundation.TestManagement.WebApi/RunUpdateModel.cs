// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.RunUpdateModel
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public class RunUpdateModel
  {
    public RunUpdateModel(
      string name = "",
      string completedDate = "",
      string iteration = "",
      string state = "",
      string controller = "",
      ShallowReference build = null,
      string startedDate = "",
      string dueDate = "",
      string comment = "",
      ShallowReference testSettings = null,
      string testEnvironmentId = "",
      bool? deleteUnexecutedResults = null,
      TestRunSubstate substate = TestRunSubstate.None,
      string errorMessage = "",
      ShallowReference dtlEnvironment = null,
      DtlEnvironmentDetails dtlEnvDetails = null,
      List<TestMessageLogDetails> logEntries = null,
      ShallowReference dtlAutEnvironment = null)
    {
      this.State = state;
      this.Name = name;
      this.Iteration = iteration;
      this.Controller = controller;
      this.Build = build;
      this.Comment = comment;
      this.TestSettings = testSettings;
      this.TestEnvironmentId = testEnvironmentId;
      this.DueDate = dueDate;
      this.StartedDate = startedDate;
      this.CompletedDate = completedDate;
      this.DeleteInProgressResults = deleteUnexecutedResults;
      this.Substate = substate;
      this.ErrorMessage = errorMessage;
      this.DtlEnvironment = dtlEnvironment;
      this.DtlAutEnvironment = dtlAutEnvironment;
      this.DtlEnvironmentDetails = dtlEnvDetails;
      this.LogEntries = logEntries;
      this.CustomTestFields = new List<CustomTestField>();
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string State { get; private set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Name { get; private set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string CompletedDate { get; private set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Iteration { get; private set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Controller { get; private set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ShallowReference Build { get; private set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string StartedDate { get; private set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string DueDate { get; private set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Comment { get; private set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ShallowReference TestSettings { get; private set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string TestEnvironmentId { get; private set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool? DeleteInProgressResults { get; private set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public TestRunSubstate Substate { get; private set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ErrorMessage { get; private set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ShallowReference DtlEnvironment { get; private set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ShallowReference DtlAutEnvironment { get; private set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DtlEnvironmentDetails DtlEnvironmentDetails { get; private set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public List<TestMessageLogDetails> LogEntries { get; private set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string BuildDropLocation { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ReleaseUri { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ReleaseEnvironmentUri { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string BuildPlatform { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string BuildFlavor { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string SourceWorkflow { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IList<RunSummaryModel> RunSummary { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IList<TestTag> Tags { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public List<CustomTestField> CustomTestFields { get; set; }
  }
}
