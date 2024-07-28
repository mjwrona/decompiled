// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TestRunModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.TestManagement.Server;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  [DataContract]
  public class TestRunModel : TestRunModelBase
  {
    public TestRunModel()
    {
    }

    public TestRunModel(Microsoft.TeamFoundation.TestManagement.WebApi.TestRun testRun)
    {
      this.TestRunId = testRun.Id;
      this.Title = testRun.Name;
      Guid result1;
      if (testRun.Owner != null && Guid.TryParse(testRun.Owner.Id, out result1))
      {
        this.Owner = result1;
        this.OwnerName = testRun.Owner.DisplayName;
      }
      this.StartDate = testRun.StartedDate;
      this.CompleteDate = testRun.CompletedDate <= DateTime.MinValue ? DateTime.Now : testRun.CompletedDate;
      int result2;
      if (testRun.Plan != null && int.TryParse(testRun.Plan.Id, out result2))
        this.TestPlanId = result2;
      int result3;
      if (testRun.TestSettings != null && int.TryParse(testRun.TestSettings.Id, out result3))
      {
        this.TestSettingsId = result3;
        this.PublicTestSettingsId = result3;
        this.TestSettingsName = testRun.TestSettings.Name;
      }
      if (testRun.TestEnvironment != null)
      {
        this.TestEnvironmentId = testRun.TestEnvironment.EnvironmentId;
        this.TestEnvironment = testRun.TestEnvironment.EnvironmentName;
      }
      Microsoft.TeamFoundation.TestManagement.Client.TestRunState result4;
      if (Enum.TryParse<Microsoft.TeamFoundation.TestManagement.Client.TestRunState>(testRun.State, out result4))
        this.State = (byte) result4;
      this.IsAutomated = testRun.IsAutomated;
      this.Iteration = testRun.Iteration;
      this.ErrorMessage = testRun.ErrorMessage;
      this.Comments = testRun.Comment;
      this.BuildUri = testRun.BuildConfiguration?.Uri;
      this.BuildNumber = testRun.BuildConfiguration?.Number;
      this.BuildPlatform = testRun.BuildConfiguration?.Platform;
      this.BuildFlavor = testRun.BuildConfiguration?.Flavor;
    }

    public TestRunModel(Microsoft.TeamFoundation.TestManagement.Server.TestRun testRun)
      : base((TestRunBase) testRun)
    {
      this.TestRunId = testRun.TestRunId;
      this.Iteration = testRun.Iteration;
      this.State = testRun.State;
      this.IsAutomated = testRun.IsAutomated;
      this.ErrorMessage = testRun.ErrorMessage;
      this.Comments = testRun.Comment;
      this.BuildPlatform = testRun.BuildPlatform;
      this.BuildFlavor = testRun.BuildFlavor;
    }

    [DataMember(Name = "testRunId")]
    public int TestRunId { get; set; }

    [DataMember(Name = "iteration")]
    public string Iteration { get; set; }

    [DataMember(Name = "isAutomated")]
    public bool IsAutomated { get; set; }

    [DataMember(Name = "state")]
    public byte State { get; set; }

    [DataMember(Name = "TestEnvironment")]
    public string TestEnvironment { get; set; }

    [DataMember(Name = "BuildPlatform")]
    public string BuildPlatform { get; set; }

    [DataMember(Name = "BuildFlavor")]
    public string BuildFlavor { get; set; }

    [DataMember(Name = "TestSettingsName")]
    public string TestSettingsName { get; set; }

    [DataMember(Name = "ErrorMessage")]
    public string ErrorMessage { get; set; }

    [DataMember(Name = "Comment")]
    public string Comments { get; set; }

    internal Microsoft.TeamFoundation.TestManagement.Server.TestRun CreateFromModel() => this.UpdateFromModel(new Microsoft.TeamFoundation.TestManagement.Server.TestRun());

    internal Microsoft.TeamFoundation.TestManagement.Server.TestRun UpdateFromModel(Microsoft.TeamFoundation.TestManagement.Server.TestRun run)
    {
      this.UpdateFromModel((TestRunBase) run);
      run.TestRunId = this.TestRunId;
      run.State = this.State;
      run.Iteration = this.Iteration;
      run.IsAutomated = this.IsAutomated;
      return run;
    }
  }
}
