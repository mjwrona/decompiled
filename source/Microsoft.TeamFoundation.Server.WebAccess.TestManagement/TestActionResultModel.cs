// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TestActionResultModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.TestManagement.Server;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  [DataContract]
  public class TestActionResultModel : TestResultModelBase
  {
    public TestActionResultModel(TestActionResult actionResult)
      : base((TestResult) actionResult)
    {
      this.ActionPath = actionResult.ActionPath;
      this.SharedStepId = actionResult.SetId;
      this.IterationId = actionResult.IterationId;
      this.SharedStepRevision = actionResult.SetRevision;
    }

    public TestActionResultModel(
      int testRunId,
      int testResultId,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestActionResultModel actionResult)
    {
      this.ActionPath = actionResult.ActionPath;
      this.IterationId = actionResult.IterationId;
      this.Id = new TestCaseResultIdentifierModel(new Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier(testRunId, testResultId));
      if (actionResult.SharedStepModel != null)
      {
        this.SharedStepId = actionResult.SharedStepModel.Id;
        this.SharedStepRevision = actionResult.SharedStepModel.Revision;
      }
      Microsoft.TeamFoundation.TestManagement.Client.TestOutcome result;
      if (Enum.TryParse<Microsoft.TeamFoundation.TestManagement.Client.TestOutcome>(actionResult.Outcome, out result))
        this.Outcome = (byte) result;
      this.ErrorMessage = actionResult.ErrorMessage;
      this.Comment = actionResult.Comment;
      this.DateStarted = actionResult.StartedDate <= DateTime.MinValue ? DateTime.Now : actionResult.StartedDate;
      this.DateCompleted = actionResult.CompletedDate <= DateTime.MinValue ? DateTime.Now : actionResult.CompletedDate;
      this.Duration = (long) actionResult.DurationInMs;
    }

    public TestActionResultModel()
    {
    }

    [DataMember(Name = "actionPath")]
    public string ActionPath { get; set; }

    [DataMember(Name = "sharedStepId")]
    public int SharedStepId { get; set; }

    [DataMember(Name = "iterationId")]
    public int IterationId { get; set; }

    [DataMember(Name = "sharedStepRevision")]
    public int SharedStepRevision { get; set; }

    internal TestActionResult CreateFromModel()
    {
      TestActionResult result = new TestActionResult()
      {
        ActionPath = this.ActionPath,
        SetId = this.SharedStepId,
        IterationId = this.IterationId,
        SetRevision = this.SharedStepRevision
      };
      result.Id = new Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier(this.Id.TestRunId, this.Id.TestResultId);
      this.UpdateFromModel((TestResult) result);
      return result;
    }
  }
}
