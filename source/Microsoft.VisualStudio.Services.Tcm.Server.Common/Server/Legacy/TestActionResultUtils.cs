// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Legacy.TestActionResultUtils
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server.Legacy
{
  public static class TestActionResultUtils
  {
    internal static IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestResultParameter> Convert(
      IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter> parameters)
    {
      return parameters == null ? (IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestResultParameter>) null : parameters.Select<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter, Microsoft.TeamFoundation.TestManagement.Server.TestResultParameter>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter, Microsoft.TeamFoundation.TestManagement.Server.TestResultParameter>) (parameter => TestActionResultUtils.Convert(parameter)));
    }

    internal static Microsoft.TeamFoundation.TestManagement.Server.TestResultParameter Convert(
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter parameter)
    {
      if (parameter == null)
        return (Microsoft.TeamFoundation.TestManagement.Server.TestResultParameter) null;
      return new Microsoft.TeamFoundation.TestManagement.Server.TestResultParameter()
      {
        TestRunId = parameter.TestRunId,
        TestResultId = parameter.TestResultId,
        IterationId = parameter.IterationId,
        ActionPath = parameter.ActionPath,
        ParameterName = parameter.ParameterName,
        Expected = parameter.Expected,
        Actual = parameter.Actual
      };
    }

    internal static IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter> Convert(
      IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestResultParameter> parameters)
    {
      return parameters == null ? (IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter>) null : parameters.Select<Microsoft.TeamFoundation.TestManagement.Server.TestResultParameter, Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter>((Func<Microsoft.TeamFoundation.TestManagement.Server.TestResultParameter, Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter>) (parameter => TestActionResultUtils.Convert(parameter)));
    }

    internal static Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter Convert(
      Microsoft.TeamFoundation.TestManagement.Server.TestResultParameter webTestResultParameter)
    {
      if (webTestResultParameter == null)
        return (Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter) null;
      return new Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter()
      {
        TestRunId = webTestResultParameter.TestRunId,
        TestResultId = webTestResultParameter.TestResultId,
        IterationId = webTestResultParameter.IterationId,
        ActionPath = webTestResultParameter.ActionPath,
        ParameterName = webTestResultParameter.ParameterName,
        Expected = webTestResultParameter.Expected,
        Actual = webTestResultParameter.Actual
      };
    }

    internal static IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestActionResult> Convert(
      IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult> testActionResults)
    {
      return testActionResults == null ? (IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestActionResult>) null : testActionResults.Select<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult, Microsoft.TeamFoundation.TestManagement.Server.TestActionResult>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult, Microsoft.TeamFoundation.TestManagement.Server.TestActionResult>) (testActionResult => TestActionResultUtils.Convert(testActionResult)));
    }

    internal static Microsoft.TeamFoundation.TestManagement.Server.TestActionResult Convert(
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult testActionResult)
    {
      if (testActionResult == null)
        return (Microsoft.TeamFoundation.TestManagement.Server.TestActionResult) null;
      Microsoft.TeamFoundation.TestManagement.Server.TestActionResult instance = TestActionResultUtils.GetInstance(testActionResult.ActionPath, testActionResult.SharedStepId);
      instance.CreationDate = testActionResult.CreationDate;
      instance.Outcome = testActionResult.Outcome;
      instance.LastUpdatedBy = testActionResult.LastUpdatedBy;
      instance.LastUpdated = testActionResult.LastUpdated;
      instance.DateStarted = testActionResult.DateStarted;
      instance.DateCompleted = testActionResult.DateCompleted;
      instance.Duration = testActionResult.Duration;
      instance.ActionPath = testActionResult.ActionPath;
      instance.IterationId = testActionResult.IterationId;
      instance.Id = TestCaseResultIdentifierConverter.Convert(testActionResult.Id);
      instance.ErrorMessage = testActionResult.ErrorMessage;
      instance.Comment = testActionResult.Comment;
      instance.SetId = testActionResult.SharedStepId;
      instance.SetRevision = testActionResult.SharedStepRevision;
      return instance;
    }

    internal static IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult> Convert(
      IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestActionResult> testActionResults)
    {
      return testActionResults == null ? (IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult>) null : testActionResults.Select<Microsoft.TeamFoundation.TestManagement.Server.TestActionResult, Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult>((Func<Microsoft.TeamFoundation.TestManagement.Server.TestActionResult, Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult>) (testActionResult => TestActionResultUtils.Convert(testActionResult)));
    }

    internal static Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult Convert(
      Microsoft.TeamFoundation.TestManagement.Server.TestActionResult testActionResult)
    {
      if (testActionResult == null)
        return (Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult) null;
      return new Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult()
      {
        CreationDate = testActionResult.CreationDate,
        Outcome = testActionResult.Outcome,
        LastUpdatedBy = testActionResult.LastUpdatedBy,
        LastUpdated = testActionResult.LastUpdated,
        DateStarted = testActionResult.DateStarted,
        DateCompleted = testActionResult.DateCompleted,
        Duration = testActionResult.Duration,
        ActionPath = testActionResult.ActionPath,
        IterationId = testActionResult.IterationId,
        Id = TestCaseResultIdentifierConverter.Convert(testActionResult.Id),
        ErrorMessage = testActionResult.ErrorMessage,
        Comment = testActionResult.Comment,
        SharedStepId = testActionResult.SetId,
        SharedStepRevision = testActionResult.SetRevision
      };
    }

    internal static Microsoft.TeamFoundation.TestManagement.Server.TestActionResult GetInstance(
      string actionPath,
      int sharedStepId)
    {
      if (string.IsNullOrEmpty(actionPath))
        return (Microsoft.TeamFoundation.TestManagement.Server.TestActionResult) new TestIterationResult();
      return sharedStepId != 0 ? (Microsoft.TeamFoundation.TestManagement.Server.TestActionResult) new SharedStepResult() : (Microsoft.TeamFoundation.TestManagement.Server.TestActionResult) new TestStepResult();
    }
  }
}
