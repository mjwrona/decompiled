// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestImpact.Server.TestImpactServer
// Assembly: Microsoft.TeamFoundation.TestImpact.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1ECF5BB1-1B8D-4502-95D9-1C6B9B1F7C03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestImpact.Server.dll

using Microsoft.TeamFoundation.Build.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestImpact.Server.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.TestImpact.Server
{
  public static class TestImpactServer
  {
    internal static void PublishBuildChanges(
      IVssRequestContext context,
      Uri projectUri,
      string buildUri,
      IEnumerable<CodeChange> changes)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      ArgumentUtility.CheckForNull<Uri>(projectUri, nameof (projectUri), context.ServiceName);
      Uri buildUri1 = Utility.CheckUri(buildUri, nameof (buildUri), context.ServiceName);
      using (TestImpactDatabase component = context.CreateComponent<TestImpactDatabase>())
      {
        BuildDetail buildDetail = TestImpactServer.GetBuildDetail(context, buildUri1);
        using (context.AcquireExemptionLock())
          component.CreateBuildImpact(buildUri, buildDetail.BuildDefinitionUri, projectUri.ToString(), buildDetail.StartTime, changes);
      }
    }

    public static List<TestBuild> QueryTestBuilds(
      IVssRequestContext context,
      int batchSize,
      bool isDeleted)
    {
      using (TestImpactDatabase component = context.CreateComponent<TestImpactDatabase>())
        return component.QueryTestBuilds(isDeleted, batchSize);
    }

    public static void GetMaxMinDateInTestBuild(
      IVssRequestContext context,
      out DateTime? maxBuildStartTime,
      out DateTime? minBuildStartTime)
    {
      using (TestImpactDatabase component = context.CreateComponent<TestImpactDatabase>())
        component.GetMaxMinBuildStartTimeInTestBuild(out maxBuildStartTime, out minBuildStartTime);
    }

    public static List<TestBuild> GetTestBuildsAfterGivenTime(
      IVssRequestContext context,
      DateTime buildStartTime,
      int batchSize)
    {
      using (TestImpactDatabase component = context.CreateComponent<TestImpactDatabase>())
        return component.GetTestBuildsAfterGivenStartTime(buildStartTime, batchSize);
    }

    public static void SoftDeleteTestBuild(IVssRequestContext context, string buildUri)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      Utility.CheckUri(buildUri, nameof (buildUri), context.ServiceName);
      using (TestImpactDatabase component = context.CreateComponent<TestImpactDatabase>())
        component.QueueDeleteTestBuild(buildUri);
    }

    public static void DeleteBuildImpact(IVssRequestContext context, string buildUri)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      Utility.CheckUri(buildUri, nameof (buildUri), context.ServiceName);
      using (TestImpactDatabase component = context.CreateComponent<TestImpactDatabase>())
      {
        int resumeStage = 1;
        while (resumeStage != 0)
          resumeStage = component.DeleteBuildImpact(buildUri, resumeStage);
      }
    }

    internal static List<CodeChange> QueryBuildCodeChanges(
      IVssRequestContext context,
      string buildUri)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      Utility.CheckUri(buildUri, nameof (buildUri), context.ServiceName);
      List<CodeChange> codeChangeList = new List<CodeChange>();
      using (TestImpactDatabase component = context.CreateComponent<TestImpactDatabase>())
      {
        using (SqlDataReader reader = component.QueryCodeChangesForBuild(buildUri))
        {
          while (reader.Read())
            codeChangeList.Add(TestImpactServer.ReadCodeChange(reader));
        }
      }
      return codeChangeList;
    }

    public static BuildImpactedTests QueryImpactedTests(
      IVssRequestContext context,
      string projectName,
      string buildUri)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      ArgumentUtility.CheckStringForNullOrEmpty(projectName, nameof (projectName), context.ServiceName);
      Utility.CheckUri(buildUri, nameof (buildUri), context.ServiceName);
      TestCaseCache service = context.GetService<TestCaseCache>();
      using (TestImpactDatabase component = context.CreateComponent<TestImpactDatabase>())
      {
        BuildImpactedTests buildImpactedTests = new BuildImpactedTests();
        Dictionary<int, int> dictionary1 = new Dictionary<int, int>();
        Dictionary<int, int> dictionary2 = new Dictionary<int, int>();
        TestImpactDatabase.QueryImpactedTestsColumns impactedTestsColumns = new TestImpactDatabase.QueryImpactedTestsColumns();
        using (SqlDataReader reader = component.QueryImpactedTestsForBuild(buildUri))
        {
          while (reader.Read())
          {
            int int32 = impactedTestsColumns.ImpactedTestId.GetInt32((IDataReader) reader);
            Test test = TestImpactServer.ReadTest(reader, service, context, ref impactedTestsColumns.TestCaseId, ref impactedTestsColumns.AutomatedTestId, ref impactedTestsColumns.AutomatedTestName, ref impactedTestsColumns.AutomatedTestType, ref impactedTestsColumns.DateCompleted);
            if (test == null)
            {
              dictionary2[int32] = -1;
            }
            else
            {
              dictionary2[int32] = buildImpactedTests.Tests.Count;
              buildImpactedTests.Tests.Add(test);
            }
          }
        }
        using (SqlDataReader reader = component.QueryCodeChangesForBuild(buildUri))
        {
          TestImpactDatabase.QueryCodeChangesColumns codeChangesColumns = new TestImpactDatabase.QueryCodeChangesColumns();
          while (reader.Read())
          {
            int int32 = codeChangesColumns.BuildCodeChangeId.GetInt32((IDataReader) reader);
            dictionary1[int32] = buildImpactedTests.CodeChanges.Count;
            buildImpactedTests.CodeChanges.Add(TestImpactServer.ReadCodeChange(reader));
          }
        }
        using (SqlDataReader reader = component.QueryImpactMappingsForBuild(buildUri))
        {
          TestImpactDatabase.QueryImpactMappingsColumns impactMappingsColumns = new TestImpactDatabase.QueryImpactMappingsColumns();
          while (reader.Read())
          {
            int int32_1 = impactMappingsColumns.ImpactedTestId.GetInt32((IDataReader) reader);
            int int32_2 = impactMappingsColumns.BuildCodeChangeId.GetInt32((IDataReader) reader);
            int index;
            int num;
            if (dictionary2.TryGetValue(int32_1, out index) && index != -1 && dictionary1.TryGetValue(int32_2, out num))
              buildImpactedTests.Tests[index].AssociationIndexes.Add(num);
          }
        }
        return buildImpactedTests;
      }
    }

    internal static TestSignatureData QueryTestCaseSignatures(
      IVssRequestContext context,
      string projectName,
      IList<string> buildDefinitionUris,
      IEnumerable<ClientTestInfo> clientTests)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      ArgumentUtility.CheckForNull<string>(projectName, nameof (projectName), context.ServiceName);
      ArgumentUtility.CheckForNull<IList<string>>(buildDefinitionUris, nameof (buildDefinitionUris), context.ServiceName);
      TestCaseCache service = context.GetService<TestCaseCache>();
      TestSignatureData testSignatureData = new TestSignatureData();
      Dictionary<int, int> dictionary1 = new Dictionary<int, int>();
      List<TestImpactServer.TestResultData> testResultDataList = new List<TestImpactServer.TestResultData>();
      Dictionary<string, int> dictionary2 = new Dictionary<string, int>();
      if (clientTests != null)
      {
        foreach (ClientTestInfo clientTest in clientTests)
        {
          if (!dictionary1.TryGetValue(clientTest.TestCaseId, out int _))
          {
            dictionary1[clientTest.TestCaseId] = testSignatureData.Tests.Count;
            testSignatureData.Tests.Add(new Test()
            {
              TestCaseId = clientTest.TestCaseId,
              Exists = false,
              DateCompleted = clientTest.DateCompleted.ToUniversalTime()
            });
          }
        }
      }
      using (TestImpactDatabase component = context.CreateComponent<TestImpactDatabase>())
      {
        using (context.AcquireExemptionLock())
        {
          using (SqlDataReader reader1 = component.QueryTestsWithCodeSignatures((IEnumerable<string>) buildDefinitionUris))
          {
            TestImpactDatabase.QueryTestsWithCodeSignaturesColumns signaturesColumns = new TestImpactDatabase.QueryTestsWithCodeSignaturesColumns();
            while (reader1.Read())
            {
              int int32_1 = signaturesColumns.TestRunId.GetInt32((IDataReader) reader1);
              int int32_2 = signaturesColumns.TestResultId.GetInt32((IDataReader) reader1);
              int int32_3 = signaturesColumns.ConfigurationId.GetInt32((IDataReader) reader1);
              Test test = TestImpactServer.ReadTest(reader1, service, context, ref signaturesColumns.TestCaseId, ref signaturesColumns.AutomatedTestId, ref signaturesColumns.AutomatedTestName, ref signaturesColumns.AutomatedTestType, ref signaturesColumns.DateCompleted);
              if (test != null)
              {
                int index = -1;
                if (test.IsTestCase)
                {
                  if (!dictionary1.TryGetValue(test.TestCaseId, out index))
                    index = -1;
                  bool flag;
                  if (index == -1)
                  {
                    testSignatureData.Tests.Add(test);
                    flag = true;
                  }
                  else
                  {
                    flag = testSignatureData.Tests[index].DateCompleted < test.DateCompleted;
                    if (!flag)
                      test.AssociationOption = AssociationOption.KeepExisting;
                    testSignatureData.Tests[index] = test;
                  }
                  if (flag)
                    testResultDataList.Add(new TestImpactServer.TestResultData(int32_1, int32_2, int32_3, test));
                }
              }
            }
            foreach (TestImpactServer.TestResultData testResultData in testResultDataList)
            {
              Test test = testResultData.Test;
              using (SqlDataReader reader2 = component.QueryCodeSignaturesForResult(testResultData.TestRunId, testResultData.TestResultId, testResultData.ConfigurationId))
              {
                TestImpactDatabase.QueryCodeSignaturesForResultColumns forResultColumns = new TestImpactDatabase.QueryCodeSignaturesForResultColumns();
                while (reader2.Read())
                {
                  string key = forResultColumns.CodeSignature.GetString((IDataReader) reader2, false);
                  int count;
                  if (!dictionary2.TryGetValue(key, out count))
                  {
                    count = testSignatureData.Signatures.Count;
                    testSignatureData.Signatures.Add(key);
                    dictionary2[key] = count;
                  }
                  test.AssociationIndexes.Add(count);
                }
              }
            }
          }
          return testSignatureData;
        }
      }
    }

    internal static BuildDetail GetBuildDetail(IVssRequestContext requestContext, Uri buildUri)
    {
      using (TeamFoundationDataReader foundationDataReader = requestContext.GetService<ITeamFoundationBuildService>().QueryBuildsByUri(requestContext, (IList<string>) new string[1]
      {
        buildUri.AbsoluteUri
      }, (IList<string>) null, QueryOptions.None, QueryDeletedOption.ExcludeDeleted))
      {
        using (IEnumerator<BuildDetail> enumerator = foundationDataReader.Current<BuildQueryResult>().Builds.GetEnumerator())
        {
          if (enumerator.MoveNext())
          {
            BuildDetail current = enumerator.Current;
            if (current != null)
              return current;
          }
        }
        throw new InvalidBuildUriException(buildUri.AbsoluteUri);
      }
    }

    internal static BuildDefinition GetBuildDefinition(
      IVssRequestContext requestContext,
      Uri buildDefinitionUri)
    {
      using (List<BuildDefinition>.Enumerator enumerator = requestContext.GetService<TeamFoundationBuildService>().QueryBuildDefinitionsByUri(requestContext, (IList<string>) new string[1]
      {
        buildDefinitionUri.AbsoluteUri
      }, (IList<string>) null, QueryOptions.None, new Guid()).Definitions.GetEnumerator())
      {
        if (enumerator.MoveNext())
        {
          BuildDefinition current = enumerator.Current;
          if (current != null)
            return current;
        }
      }
      throw new BuildDefinitionDoesNotExistException(buildDefinitionUri.AbsoluteUri);
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

    internal static Test ReadTest(
      SqlDataReader reader,
      TestCaseCache testCaseCache,
      IVssRequestContext requestContext,
      ref SqlColumnBinder testCaseIdBinder,
      ref SqlColumnBinder automatedTestIdBinder,
      ref SqlColumnBinder automatedTestNameBinder,
      ref SqlColumnBinder automatedTestTypeBinder,
      ref SqlColumnBinder dateCompletedBinder)
    {
      DateTime dateTime = dateCompletedBinder.GetDateTime((IDataReader) reader);
      int int32 = testCaseIdBinder.GetInt32((IDataReader) reader);
      string title = "";
      string type = "";
      Guid automatedTestId = Guid.Empty;
      if (int32 > 0)
      {
        if (!testCaseCache.GetTestCaseInfo(requestContext, int32, out title, out type, out automatedTestId))
          return (Test) null;
      }
      else
      {
        string g = automatedTestIdBinder.GetString((IDataReader) reader, false);
        if (string.IsNullOrEmpty(g))
          return (Test) null;
        try
        {
          automatedTestId = new Guid(g);
        }
        catch (FormatException ex)
        {
          return (Test) null;
        }
        title = automatedTestNameBinder.GetString((IDataReader) reader, false);
        type = automatedTestTypeBinder.GetString((IDataReader) reader, false);
      }
      return new Test(int32, automatedTestId, title, type, dateTime);
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
