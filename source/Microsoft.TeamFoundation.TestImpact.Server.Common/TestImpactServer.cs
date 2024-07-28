// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestImpact.Server.Common.TestImpactServer
// Assembly: Microsoft.TeamFoundation.TestImpact.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 079E4AEE-0642-4BDD-8B76-CECF38EBB798
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestImpact.Server.Common.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.TestImpact.Server.Common.Properties;
using Microsoft.TeamFoundation.TestImpact.WebApi.Contracts;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Clients;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.TestImpact.Server.Common
{
  [CLSCompliant(false)]
  public static class TestImpactServer
  {
    public static string GetProjectName(TestImpactRequestContext context, Guid projectId) => context.RequestContext.GetService<IProjectService>().GetProjectName(context.RequestContext, projectId);

    public static void GetMaxMinDateInTestBuild(
      TestImpactRequestContext context,
      out DateTime? maxBuildStartTime,
      out DateTime? minBuildStartTime)
    {
      using (TestImpactDatabase component = context.RequestContext.CreateComponent<TestImpactDatabase>())
        component.GetMaxMinBuildStartTimeInTestBuild(out maxBuildStartTime, out minBuildStartTime);
    }

    public static List<TestBuild> QueryTestBuilds(
      TestImpactRequestContext context,
      int batchSize,
      bool isDeleted)
    {
      using (TestImpactDatabase component = context.RequestContext.CreateComponent<TestImpactDatabase>())
        return component.QueryTestBuilds(isDeleted, batchSize);
    }

    public static List<TestBuild> GetTestBuildsAfterGivenTime(
      TestImpactRequestContext context,
      DateTime buildStartTime,
      int batchSize)
    {
      using (TestImpactDatabase component = context.RequestContext.CreateComponent<TestImpactDatabase>())
        return component.GetTestBuildsAfterGivenStartTime(buildStartTime, batchSize);
    }

    internal static void PublishImpactData(
      TestImpactRequestContext context,
      Guid projectId,
      int testRunId,
      int definitionType,
      int definitionId,
      IEnumerable<TestResultCodeSignatures> results,
      IList<string> signatures,
      SignatureType signatureType)
    {
      ArgumentUtility.CheckForNull<TestImpactRequestContext>(context, nameof (context));
      ArgumentUtility.CheckGreaterThanZero((float) definitionId, nameof (definitionId), context.RequestContext.ServiceName);
      ArgumentUtility.CheckForNull<IEnumerable<TestResultCodeSignatures>>(results, nameof (results), context.RequestContext.ServiceName);
      ArgumentUtility.CheckForNull<IList<string>>(signatures, nameof (signatures), context.RequestContext.ServiceName);
      TeamProjectReference projectReference = new TeamProjectReference()
      {
        Name = (string) null,
        Id = projectId
      };
      IList<TestCaseResult> source = new TestManagementResultHelper().QueryTestResultsByRun(context, projectId, testRunId);
      using (ImpactDatabase component = context.RequestContext.CreateComponent<ImpactDatabase>())
      {
        HashSet<Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.Signature> signatureSet = new HashSet<Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.Signature>();
        foreach (TestResultCodeSignatures result1 in results)
        {
          TestResultCodeSignatures result = result1;
          string automatedTestName = source.First<TestCaseResult>((System.Func<TestCaseResult, bool>) (x => x.Id == result.TestResultId)).AutomatedTestName;
          ArgumentUtility.CheckStringForNullOrEmpty(automatedTestName, "automatedTestName", context.RequestContext.ServiceName);
          if (result != null)
          {
            signatureSet.Clear();
            foreach (int index in result.Indexes)
            {
              if (index >= 0 && index < signatures.Count)
                signatureSet.Add(new Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.Signature()
                {
                  SignatureType = signatureType,
                  CodeSignature = signatures[index]
                });
            }
            using (context.RequestContext.AcquireExemptionLock())
              component.PublishTestSignatures(projectId, testRunId, result.TestResultId, result.ConfigurationId, definitionType, definitionId, (IEnumerable<Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.Signature>) signatureSet, automatedTestName);
          }
        }
      }
    }

    internal static void PublishCodeSignatures(
      TestImpactRequestContext context,
      int testRunId,
      IEnumerable<TestResultCodeSignatures> results,
      IList<string> signatures)
    {
      ArgumentUtility.CheckForNull<TestImpactRequestContext>(context, nameof (context));
      ArgumentUtility.CheckForNull<IEnumerable<TestResultCodeSignatures>>(results, nameof (results), context.RequestContext.ServiceName);
      ArgumentUtility.CheckForNull<IList<string>>(signatures, nameof (signatures), context.RequestContext.ServiceName);
      using (TestImpactDatabase component = context.RequestContext.CreateComponent<TestImpactDatabase>())
      {
        using (context.RequestContext.AcquireExemptionLock())
        {
          HashSet<string> signatures1 = new HashSet<string>();
          foreach (TestResultCodeSignatures result in results)
          {
            signatures1.Clear();
            foreach (int index in result.Indexes)
            {
              if (index >= 0 && index < signatures.Count)
                signatures1.Add(signatures[index]);
            }
            component.CreateTestCodeSignatures(testRunId, result.TestResultId, result.ConfigurationId, (IEnumerable<string>) signatures1);
          }
        }
      }
    }

    internal static void PublishImpactData(
      TestImpactRequestContext context,
      Guid projectId,
      int definitionRunId,
      TestResultSignaturesInfo results)
    {
      ArgumentUtility.CheckForNull<TestImpactRequestContext>(context, nameof (context));
      ArgumentUtility.CheckGreaterThanZero((float) definitionRunId, "definitionId", context.RequestContext.ServiceName);
      ArgumentUtility.CheckForNull<TestResultSignaturesInfo>(results, nameof (results), context.RequestContext.ServiceName);
      IList<TestCaseResult> source;
      using (PerformanceTimer.StartMeasure(context.RequestContext, "TestImpactServer.PublishImpactData"))
        source = new TestManagementResultHelper().QueryTestResultsByRun(context, projectId, results.TestRunId);
      using (ImpactDatabase component = context.RequestContext.CreateComponent<ImpactDatabase>())
      {
        HashSet<Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.Signature> signatureSet = new HashSet<Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.Signature>();
        foreach (TestResultSignatures testResultSignature in results.TestResultSignatures)
        {
          TestResultSignatures result = testResultSignature;
          string automatedTestName = source.First<TestCaseResult>((System.Func<TestCaseResult, bool>) (x => x.Id == result.TestResultId)).AutomatedTestName;
          ArgumentUtility.CheckStringForNullOrEmpty(automatedTestName, "automatedTestName", context.RequestContext.ServiceName);
          if (result != null)
          {
            signatureSet.Clear();
            if (result.Signatures.Count > 0)
            {
              foreach (string signature in result.Signatures)
                signatureSet.Add(new Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.Signature()
                {
                  SignatureType = SignatureType.File,
                  CodeSignature = signature
                });
            }
            component.PublishTestSignatures(projectId, results.TestRunId, result.TestResultId, result.ConfigurationId, (int) results.DefinitionType, definitionRunId, (IEnumerable<Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.Signature>) signatureSet, automatedTestName);
          }
        }
      }
    }

    public static void SoftDeleteTestBuild(TestImpactRequestContext context, string buildUri)
    {
      ArgumentUtility.CheckForNull<TestImpactRequestContext>(context, nameof (context));
      Utility.CheckUri(buildUri, nameof (buildUri), context.RequestContext.ServiceName);
      using (TestImpactDatabase component = context.RequestContext.CreateComponent<TestImpactDatabase>())
        component.QueueDeleteTestBuild(buildUri);
    }

    public static void DeleteBuildImpact(TestImpactRequestContext context, string buildUri)
    {
      ArgumentUtility.CheckForNull<TestImpactRequestContext>(context, nameof (context));
      Utility.CheckUri(buildUri, nameof (buildUri), context.RequestContext.ServiceName);
      using (TestImpactDatabase component = context.RequestContext.CreateComponent<TestImpactDatabase>())
      {
        int resumeStage = 1;
        while (resumeStage != 0)
          resumeStage = component.DeleteBuildImpact(buildUri, resumeStage);
      }
    }

    internal static List<CodeChange> QueryBuildCodeChanges(
      TestImpactRequestContext context,
      string buildUri)
    {
      ArgumentUtility.CheckForNull<TestImpactRequestContext>(context, nameof (context));
      Utility.CheckUri(buildUri, nameof (buildUri), context.RequestContext.ServiceName);
      List<CodeChange> codeChangeList = new List<CodeChange>();
      using (TestImpactDatabase component = context.RequestContext.CreateComponent<TestImpactDatabase>())
      {
        using (SqlDataReader reader = component.QueryCodeChangesForBuild(buildUri))
        {
          while (reader.Read())
            codeChangeList.Add(TestImpactServer.ReadCodeChange(reader));
        }
      }
      return codeChangeList;
    }

    internal static Microsoft.TeamFoundation.Build.WebApi.Build GetBuildDetail(
      TestImpactRequestContext requestContext,
      Guid project,
      int buildId)
    {
      return requestContext.RequestContext.GetClient<BuildHttpClient>().GetBuildAsync(project, buildId).Result ?? throw new InvalidDefinitionRunIdException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.InvalidBuildId, (object) buildId));
    }

    internal static Release GetReleaseDetail(
      TestImpactRequestContext requestContext,
      int releaseId,
      Guid project)
    {
      ReleaseHttpClient client = requestContext.RequestContext.GetClient<ReleaseHttpClient>();
      Guid project1 = project;
      int releaseId1 = releaseId;
      SingleReleaseExpands? nullable = new SingleReleaseExpands?(SingleReleaseExpands.None);
      ApprovalFilters? approvalFilters = new ApprovalFilters?();
      SingleReleaseExpands? expand = nullable;
      int? topGateRecords = new int?();
      CancellationToken cancellationToken = new CancellationToken();
      return client.GetReleaseAsync(project1, releaseId1, approvalFilters, expand: expand, topGateRecords: topGateRecords, cancellationToken: cancellationToken).Result ?? throw new InvalidDefinitionRunIdException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.InvalidBuildId, (object) releaseId));
    }

    internal static ImpactedTests GetAllTestsForAGivenRun(
      TestImpactRequestContext requestContext,
      int runId,
      Guid project)
    {
      IList<TestCaseResult> testCaseResultList = new TestManagementResultHelper().QueryTestResultsByRun(requestContext, project, runId);
      ImpactedTests testsForAgivenRun = new ImpactedTests();
      foreach (TestCaseResult testCaseResult in (IEnumerable<TestCaseResult>) testCaseResultList)
      {
        Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.Test test = new Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.Test();
        Guid result;
        if (Guid.TryParse(testCaseResult.AutomatedTestId, out result))
          test.AutomatedTestId = result;
        test.TestName = testCaseResult.AutomatedTestName;
        test.TestCaseId = testCaseResult.Id;
        testsForAgivenRun.Tests.Add(test);
      }
      return testsForAgivenRun;
    }

    internal static int GetBaseEnvIdForRun(Release release, int envDefId)
    {
      foreach (ReleaseEnvironment environment in (IEnumerable<ReleaseEnvironment>) release.Environments)
      {
        if (environment.DefinitionEnvironmentId == envDefId)
          return environment.Id;
      }
      return 0;
    }

    internal static List<Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.Test> GetFailedTests(
      TestImpactRequestContext requestContext,
      Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.DefinitionType definitionType,
      int currentRunId,
      int buildId,
      Guid project)
    {
      List<Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.Test> failedTests = new List<Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.Test>();
      IList<ShallowTestCaseResult> shallowTestCaseResultList;
      if (definitionType == Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.DefinitionType.Build)
      {
        shallowTestCaseResultList = new TestManagementResultHelper().GetTestResultDetailsForBuild(requestContext, project, buildId);
      }
      else
      {
        TestRun testRun = new TestManagementRunHelper().GetTestRun(requestContext, currentRunId, project);
        int baseEnvIdForRun = TestImpactServer.GetBaseEnvIdForRun(TestImpactServer.GetReleaseDetail(requestContext, buildId, project), testRun.Release.EnvironmentDefinitionId);
        shallowTestCaseResultList = new TestManagementResultHelper().GetTestResultDetailsForRelease(requestContext, project, buildId, baseEnvIdForRun);
      }
      foreach (ShallowTestCaseResult shallowTestCaseResult in (IEnumerable<ShallowTestCaseResult>) shallowTestCaseResultList)
      {
        Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.Test test = new Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.Test()
        {
          TestName = shallowTestCaseResult.AutomatedTestName
        };
        failedTests.Add(test);
      }
      return failedTests;
    }

    internal static CodeChange ReadCodeChange(SqlDataReader reader)
    {
      CodeChange codeChange = new CodeChange();
      TestImpactDatabase.QueryCodeChangesColumns codeChangesColumns = new TestImpactDatabase.QueryCodeChangesColumns();
      codeChange.Name = codeChangesColumns.CodeChangeName.GetString((IDataReader) reader, false);
      codeChange.Signature = codeChangesColumns.CodeChangeSignature.GetString((IDataReader) reader, false);
      codeChange.AssemblyName = codeChangesColumns.AssemblyName.GetString((IDataReader) reader, false);
      codeChange.AssemblyIdentifier = codeChangesColumns.AssemblyIdentifier.GetGuid((IDataReader) reader, false);
      codeChange.Reason = (CodeChangeReason) codeChangesColumns.Reason.GetByte((IDataReader) reader);
      codeChange.FileName = codeChangesColumns.FileName.GetString((IDataReader) reader, true);
      codeChange.MethodKind = (MethodKind) codeChangesColumns.MethodKind.GetByte((IDataReader) reader);
      codeChange.MethodAccess = (MethodAccess) codeChangesColumns.MethodAccess.GetByte((IDataReader) reader);
      string str1 = codeChangesColumns.Changesets.GetString((IDataReader) reader, true);
      if (str1 != null)
      {
        string str2 = str1;
        char[] separator = new char[1]{ ';' };
        foreach (string str3 in str2.Split(separator, StringSplitOptions.RemoveEmptyEntries))
        {
          int result;
          if (str3.StartsWith("[", StringComparison.Ordinal) && str3.EndsWith("]", StringComparison.Ordinal) && int.TryParse(str3.Substring(1, str3.Length - 2), out result))
            codeChange.Changesets.Add(result);
        }
      }
      return codeChange;
    }

    internal struct TestResultData
    {
      public TestResultData(int testRunId, int testResultId, int configurationId, Test test)
        : this()
      {
        this.TestRunId = testRunId;
        this.TestResultId = testResultId;
        this.ConfigurationId = configurationId;
        this.Test = test;
      }

      public int TestRunId { get; private set; }

      public int TestResultId { get; private set; }

      public int ConfigurationId { get; private set; }

      public Test Test { get; private set; }
    }
  }
}
