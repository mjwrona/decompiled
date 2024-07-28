// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TestCaseResultModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.Server;
using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  [DataContract]
  public class TestCaseResultModel : TestResultModelBase
  {
    public TestCaseResultModel()
    {
    }

    public TestCaseResultModel(Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult result)
      : base((TestResult) result)
    {
      this.TestCaseId = result.TestCaseId;
      this.ConfigurationId = result.ConfigurationId;
      this.ConfigurationName = result.ConfigurationName;
      this.TestPointId = result.TestPointId;
      this.State = result.State;
      this.ComputerName = result.ComputerName;
      this.Owner = result.Owner;
      this.RunBy = result.RunBy;
      this.RunByName = result.RunByName;
      this.TestCaseTitle = result.TestCaseTitle;
      this.Revision = result.Revision;
      this.DataRowCount = result.DataRowCount;
      this.TestCaseRevision = result.TestCaseRevision;
      this.TestRunId = result.TestRunId;
      this.TestResultId = result.TestResultId;
      this.FailureType = result.FailureType;
      this.ResolutionStateId = result.ResolutionStateId;
      this.BuildNumber = result.BuildNumber;
      this.Priority = result.Priority;
      this.OwnerName = result.OwnerName;
      this.AutomatedTestType = result.AutomatedTestType;
      this.PlanId = result.TestPlanId;
      this.SuiteId = result.TestSuiteId;
      this.SuiteName = result.SuiteName;
      this.TestCaseReferenceId = result.TestCaseReferenceId;
    }

    public TestCaseResultModel(Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult result)
    {
      this.TestResultId = result.Id;
      this.TestCaseReferenceId = result.TestCaseReferenceId;
      int result1;
      if (!string.IsNullOrEmpty(result.TestRun?.Id) && int.TryParse(result.TestRun.Id, out result1))
        this.TestRunId = result1;
      this.Id = new TestCaseResultIdentifierModel(new Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier(this.TestRunId, this.TestResultId));
      int result2;
      if (!string.IsNullOrEmpty(result.TestCase?.Id) && int.TryParse(result.TestCase.Id, out result2))
        this.TestCaseId = result2;
      this.TestCaseTitle = !string.IsNullOrEmpty(result.TestCaseTitle) ? result.TestCaseTitle : this.TestCaseId.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      this.Revision = result.Revision;
      this.TestCaseRevision = result.TestCaseRevision;
      int result3;
      if (!string.IsNullOrEmpty(result.Configuration?.Id) && int.TryParse(result.Configuration.Id, out result3))
      {
        this.ConfigurationId = result3;
        this.ConfigurationName = !string.IsNullOrEmpty(result.Configuration.Name) ? result.Configuration.Name : result3.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      }
      int result4;
      if (!string.IsNullOrEmpty(result.TestPoint?.Id) && int.TryParse(result.TestPoint.Id, out result4))
        this.TestPointId = result4;
      int result5;
      if (!string.IsNullOrEmpty(result.TestPlan?.Id) && int.TryParse(result.TestPlan.Id, out result5))
      {
        this.PlanId = result5;
        this.PlanName = !string.IsNullOrEmpty(result.TestPlan.Name) ? result.TestPlan.Name : result5.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      }
      int result6;
      if (!string.IsNullOrEmpty(result.TestSuite?.Id) && int.TryParse(result.TestSuite.Id, out result6))
      {
        this.SuiteId = result6;
        this.SuiteName = result.TestSuite.Name;
      }
      Microsoft.TeamFoundation.TestManagement.Client.TestOutcome result7;
      if (Enum.TryParse<Microsoft.TeamFoundation.TestManagement.Client.TestOutcome>(result.Outcome, out result7))
        this.Outcome = (byte) result7;
      TestResultState result8;
      if (Enum.TryParse<TestResultState>(result.State, out result8))
        this.State = (byte) result8;
      this.ComputerName = result.ComputerName;
      Guid result9;
      if (result.Owner != null && Guid.TryParse(result.Owner.Id, out result9))
      {
        this.Owner = result9;
        this.OwnerName = result.Owner.DisplayName;
      }
      Guid result10;
      if (result.RunBy != null && Guid.TryParse(result.RunBy.Id, out result10))
      {
        this.RunBy = result10;
        this.RunByName = result.RunBy.DisplayName;
      }
      if (result.FailureType != null)
      {
        Microsoft.TeamFoundation.TestManagement.Client.FailureType result11;
        if (Enum.TryParse<Microsoft.TeamFoundation.TestManagement.Client.FailureType>(result.FailureType.Replace(" ", ""), out result11))
          this.FailureType = (byte) result11;
        this.FailureTypeString = result.FailureType;
      }
      this.ErrorMessage = result.ErrorMessage;
      this.Comment = result.Comment;
      this.DateStarted = result.StartedDate <= DateTime.MinValue ? DateTime.Now : result.StartedDate;
      this.DateCompleted = result.CompletedDate <= DateTime.MinValue ? DateTime.Now : result.CompletedDate;
      this.Duration = (long) result.DurationInMs;
      this.ResolutionStateId = result.ResolutionStateId;
      if (result.BuildReference != null)
        this.BuildNumber = result.BuildReference.Number;
      else if (result.Build != null)
        this.BuildNumber = result.Build.Name;
      this.Priority = (byte) result.Priority;
      this.AutomatedTestType = result.AutomatedTestType;
    }

    [DataMember(Name = "testCaseId")]
    public int TestCaseId { get; set; }

    [DataMember(Name = "configurationId")]
    public int ConfigurationId { get; set; }

    [DataMember(Name = "configurationName")]
    public string ConfigurationName { get; set; }

    [DataMember(Name = "buildNumber")]
    public string BuildNumber { get; set; }

    [DataMember(Name = "testPointId")]
    public int TestPointId { get; set; }

    [DataMember(Name = "state")]
    public byte State { get; set; }

    [DataMember(Name = "computerName")]
    public string ComputerName { get; set; }

    [DataMember(Name = "owner")]
    public Guid Owner { get; set; }

    [DataMember(Name = "runBy")]
    public Guid RunBy { get; set; }

    [DataMember(Name = "runByName")]
    public string RunByName { get; set; }

    [DataMember(Name = "testCaseTitle")]
    public string TestCaseTitle { get; set; }

    [DataMember(Name = "revision")]
    public int Revision { get; set; }

    [DataMember(Name = "dataRowCount")]
    public int DataRowCount { get; set; }

    [DataMember(Name = "testCaseRevision")]
    public int TestCaseRevision { get; set; }

    [DataMember(Name = "testRunId")]
    public int TestRunId { get; set; }

    [DataMember(Name = "testResultId")]
    public int TestResultId { get; set; }

    [DataMember(Name = "failureType")]
    public byte FailureType { get; set; }

    [DataMember(Name = "failureTypeString")]
    public string FailureTypeString { get; set; }

    [DataMember(Name = "resolutionStateId")]
    public int ResolutionStateId { get; set; }

    [DataMember(Name = "priority")]
    public byte Priority { get; set; }

    [DataMember(Name = "ownerName")]
    public string OwnerName { get; set; }

    [DataMember(Name = "automatedTestType")]
    public string AutomatedTestType { get; set; }

    [DataMember(Name = "planId")]
    public int PlanId { get; set; }

    [DataMember(Name = "planName")]
    public string PlanName { get; set; }

    [DataMember(Name = "suiteId")]
    public int SuiteId { get; set; }

    [DataMember(Name = "suiteName")]
    public string SuiteName { get; set; }

    [DataMember(Name = "useTeamSettings")]
    public bool UseTeamSettings { get; set; }

    [DataMember(Name = "testCaseReferenceId")]
    public int TestCaseReferenceId { get; set; }

    internal Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult CreateFromModel()
    {
      Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult result = new Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult();
      result.Id = new Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier(this.Id.TestRunId, this.Id.TestResultId);
      return this.UpdateFromModel(result);
    }

    internal Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult UpdateFromModel(
      Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult result)
    {
      this.UpdateFromModel((TestResult) result);
      result.TestCaseId = this.TestCaseId;
      result.ConfigurationId = this.ConfigurationId;
      result.ConfigurationName = this.ConfigurationName;
      result.TestPointId = this.TestPointId;
      result.State = this.State;
      result.ComputerName = this.ComputerName;
      result.Owner = this.Owner;
      result.RunBy = this.RunBy;
      result.RunByName = this.RunByName;
      result.TestCaseTitle = this.TestCaseTitle;
      result.Revision = this.Revision;
      result.DataRowCount = this.DataRowCount;
      result.TestCaseRevision = this.TestCaseRevision;
      result.FailureType = this.FailureType;
      result.ResolutionStateId = this.ResolutionStateId;
      return result;
    }
  }
}
