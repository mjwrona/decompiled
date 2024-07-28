// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Legacy.TestCaseResultContractConverter
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.TestManagement.WebApi.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server.Legacy
{
  internal static class TestCaseResultContractConverter
  {
    internal static LegacyTestCaseResult Convert(TestCaseResult testCaseResult)
    {
      if (testCaseResult == null)
        return (LegacyTestCaseResult) null;
      LegacyTestCaseResult legacyTestCaseResult = new LegacyTestCaseResult();
      legacyTestCaseResult.TestCaseId = testCaseResult.TestCaseId;
      legacyTestCaseResult.ConfigurationId = testCaseResult.ConfigurationId;
      legacyTestCaseResult.ConfigurationName = testCaseResult.ConfigurationName;
      legacyTestCaseResult.TestPointId = testCaseResult.TestPointId;
      legacyTestCaseResult.State = testCaseResult.State;
      legacyTestCaseResult.FailureType = testCaseResult.FailureType;
      legacyTestCaseResult.ResolutionStateId = TestCaseResultContractConverter.GetResolutionStateId(testCaseResult);
      legacyTestCaseResult.ComputerName = testCaseResult.ComputerName;
      legacyTestCaseResult.Owner = testCaseResult.Owner;
      legacyTestCaseResult.OwnerName = testCaseResult.OwnerName;
      legacyTestCaseResult.RunBy = testCaseResult.RunBy;
      legacyTestCaseResult.RunByName = testCaseResult.RunByName;
      legacyTestCaseResult.Priority = testCaseResult.Priority;
      legacyTestCaseResult.TestCaseTitle = testCaseResult.TestCaseTitle;
      legacyTestCaseResult.TestCaseArea = testCaseResult.TestCaseArea;
      legacyTestCaseResult.TestCaseAreaUri = testCaseResult.TestCaseAreaUri;
      legacyTestCaseResult.AreaUri = testCaseResult.AreaUri;
      legacyTestCaseResult.TestCaseRevision = testCaseResult.TestCaseRevision;
      legacyTestCaseResult.AfnStripId = testCaseResult.AfnStripId;
      legacyTestCaseResult.ResetCount = testCaseResult.ResetCount;
      legacyTestCaseResult.AutomatedTestName = testCaseResult.AutomatedTestName;
      legacyTestCaseResult.AutomatedTestStorage = testCaseResult.AutomatedTestStorage;
      legacyTestCaseResult.AutomatedTestType = testCaseResult.AutomatedTestType;
      legacyTestCaseResult.AutomatedTestTypeId = testCaseResult.AutomatedTestTypeId;
      legacyTestCaseResult.AutomatedTestId = testCaseResult.AutomatedTestId;
      legacyTestCaseResult.Revision = testCaseResult.Revision;
      legacyTestCaseResult.BuildNumber = testCaseResult.BuildNumber;
      legacyTestCaseResult.TestPlanId = testCaseResult.TestPlanId;
      legacyTestCaseResult.TestSuiteId = testCaseResult.TestSuiteId;
      legacyTestCaseResult.SuiteName = testCaseResult.SuiteName;
      legacyTestCaseResult.SequenceId = testCaseResult.SequenceId;
      legacyTestCaseResult.TestCaseReferenceId = testCaseResult.TestCaseReferenceId;
      legacyTestCaseResult.StackTrace = TestExtensionFieldConverter.Convert(testCaseResult.StackTrace);
      IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestExtensionField> source = TestExtensionFieldConverter.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestExtensionField>) testCaseResult.CustomFields);
      legacyTestCaseResult.CustomFields = source != null ? source.ToList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestExtensionField>() : (List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestExtensionField>) null;
      legacyTestCaseResult.BuildReference = TestRunContractConverter.Convert(testCaseResult.BuildReference);
      legacyTestCaseResult.ReleaseReference = TestRunContractConverter.Convert(testCaseResult.ReleaseReference);
      legacyTestCaseResult.FailingSince = testCaseResult.FailingSince;
      legacyTestCaseResult.IsRerun = testCaseResult.IsRerun;
      legacyTestCaseResult.SubResultCount = testCaseResult.SubResultCount;
      legacyTestCaseResult.ResultGroupType = testCaseResult.ResultGroupType;
      legacyTestCaseResult.TestRunTitle = testCaseResult.TestRunTitle;
      legacyTestCaseResult.AreaId = testCaseResult.AreaId;
      legacyTestCaseResult.Id = TestCaseResultIdentifierConverter.Convert(testCaseResult.Id);
      legacyTestCaseResult.TestRunId = testCaseResult.TestRunId;
      legacyTestCaseResult.TestResultId = testCaseResult.TestResultId;
      legacyTestCaseResult.CreationDate = testCaseResult.CreationDate;
      legacyTestCaseResult.Outcome = testCaseResult.Outcome;
      legacyTestCaseResult.ErrorMessage = testCaseResult.ErrorMessage;
      legacyTestCaseResult.Comment = testCaseResult.Comment;
      legacyTestCaseResult.LastUpdatedBy = testCaseResult.LastUpdatedBy;
      legacyTestCaseResult.LastUpdatedByName = testCaseResult.LastUpdatedByName;
      legacyTestCaseResult.LastUpdated = testCaseResult.LastUpdated;
      legacyTestCaseResult.DateStarted = testCaseResult.DateStarted;
      legacyTestCaseResult.DateCompleted = testCaseResult.DateCompleted;
      legacyTestCaseResult.Duration = testCaseResult.Duration;
      return legacyTestCaseResult;
    }

    internal static IEnumerable<LegacyTestCaseResult> Convert(
      IEnumerable<TestCaseResult> testCaseResults)
    {
      return testCaseResults == null ? (IEnumerable<LegacyTestCaseResult>) null : testCaseResults.Select<TestCaseResult, LegacyTestCaseResult>((Func<TestCaseResult, LegacyTestCaseResult>) (result => TestCaseResultContractConverter.Convert(result)));
    }

    internal static TestCaseResult Convert(LegacyTestCaseResult testCaseResult)
    {
      if (testCaseResult == null)
        return (TestCaseResult) null;
      TestCaseResult testCaseResult1 = new TestCaseResult();
      testCaseResult1.TestCaseId = testCaseResult.TestCaseId;
      testCaseResult1.ConfigurationId = testCaseResult.ConfigurationId;
      testCaseResult1.ConfigurationName = testCaseResult.ConfigurationName;
      testCaseResult1.TestPointId = testCaseResult.TestPointId;
      testCaseResult1.State = testCaseResult.State;
      testCaseResult1.FailureType = testCaseResult.FailureType;
      testCaseResult1.ResolutionStateId = testCaseResult.ResolutionStateId;
      testCaseResult1.ComputerName = testCaseResult.ComputerName;
      testCaseResult1.Owner = testCaseResult.Owner;
      testCaseResult1.OwnerName = testCaseResult.OwnerName;
      testCaseResult1.RunBy = testCaseResult.RunBy;
      testCaseResult1.RunByName = testCaseResult.RunByName;
      testCaseResult1.Priority = testCaseResult.Priority;
      testCaseResult1.TestCaseTitle = testCaseResult.TestCaseTitle;
      testCaseResult1.TestCaseArea = testCaseResult.TestCaseArea;
      testCaseResult1.TestCaseAreaUri = testCaseResult.TestCaseAreaUri;
      testCaseResult1.AreaUri = testCaseResult.AreaUri;
      testCaseResult1.TestCaseRevision = testCaseResult.TestCaseRevision;
      testCaseResult1.AfnStripId = testCaseResult.AfnStripId;
      testCaseResult1.ResetCount = testCaseResult.ResetCount;
      testCaseResult1.AutomatedTestName = testCaseResult.AutomatedTestName;
      testCaseResult1.AutomatedTestStorage = testCaseResult.AutomatedTestStorage;
      testCaseResult1.AutomatedTestType = testCaseResult.AutomatedTestType;
      testCaseResult1.AutomatedTestTypeId = testCaseResult.AutomatedTestTypeId;
      testCaseResult1.AutomatedTestId = testCaseResult.AutomatedTestId;
      testCaseResult1.Revision = testCaseResult.Revision;
      testCaseResult1.BuildNumber = testCaseResult.BuildNumber;
      testCaseResult1.TestPlanId = testCaseResult.TestPlanId;
      testCaseResult1.TestSuiteId = testCaseResult.TestSuiteId;
      testCaseResult1.SuiteName = testCaseResult.SuiteName;
      testCaseResult1.SequenceId = testCaseResult.SequenceId;
      testCaseResult1.TestCaseReferenceId = testCaseResult.TestCaseReferenceId;
      testCaseResult1.StackTrace = TestExtensionFieldConverter.Convert(testCaseResult.StackTrace);
      IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestExtensionField> source = TestExtensionFieldConverter.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestExtensionField>) testCaseResult.CustomFields);
      testCaseResult1.CustomFields = source != null ? source.ToList<Microsoft.TeamFoundation.TestManagement.Server.TestExtensionField>() : (List<Microsoft.TeamFoundation.TestManagement.Server.TestExtensionField>) null;
      testCaseResult1.BuildReference = TestRunContractConverter.Convert(testCaseResult.BuildReference);
      testCaseResult1.ReleaseReference = TestRunContractConverter.Convert(testCaseResult.ReleaseReference);
      testCaseResult1.FailingSince = testCaseResult.FailingSince;
      testCaseResult1.IsRerun = testCaseResult.IsRerun;
      testCaseResult1.SubResultCount = testCaseResult.SubResultCount;
      testCaseResult1.ResultGroupType = testCaseResult.ResultGroupType;
      testCaseResult1.TestRunTitle = testCaseResult.TestRunTitle;
      testCaseResult1.AreaId = testCaseResult.AreaId;
      testCaseResult1.Id = TestCaseResultIdentifierConverter.Convert(testCaseResult.Id);
      testCaseResult1.TestRunId = testCaseResult.TestRunId;
      testCaseResult1.TestResultId = testCaseResult.TestResultId;
      testCaseResult1.CreationDate = testCaseResult.CreationDate;
      testCaseResult1.Outcome = testCaseResult.Outcome;
      testCaseResult1.ErrorMessage = testCaseResult.ErrorMessage;
      testCaseResult1.Comment = testCaseResult.Comment;
      testCaseResult1.LastUpdatedBy = testCaseResult.LastUpdatedBy;
      testCaseResult1.LastUpdatedByName = testCaseResult.LastUpdatedByName;
      testCaseResult1.LastUpdated = testCaseResult.LastUpdated;
      testCaseResult1.DateStarted = testCaseResult.DateStarted;
      testCaseResult1.DateCompleted = testCaseResult.DateCompleted;
      testCaseResult1.Duration = testCaseResult.Duration;
      return testCaseResult1;
    }

    internal static IEnumerable<TestCaseResult> Convert(
      IEnumerable<LegacyTestCaseResult> testCaseResults)
    {
      return testCaseResults == null ? (IEnumerable<TestCaseResult>) null : testCaseResults.Select<LegacyTestCaseResult, TestCaseResult>((Func<LegacyTestCaseResult, TestCaseResult>) (result => TestCaseResultContractConverter.Convert(result)));
    }

    private static int GetResolutionStateId(TestCaseResult testCaseResult) => testCaseResult.ResolutionStateId != -1 ? testCaseResult.ResolutionStateId : 0;
  }
}
