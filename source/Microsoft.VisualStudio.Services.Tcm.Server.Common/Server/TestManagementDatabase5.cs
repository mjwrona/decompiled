// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase5
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase5 : TestManagementDatabase4
  {
    internal TestManagementDatabase5(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase5()
    {
    }

    public override List<TestCaseResult> GetTestCaseResultsByIds(
      Guid projectId,
      List<TestCaseResultIdentifier> resultIds,
      List<string> fields)
    {
      List<string> fieldsToFetch = new List<string>();
      List<string> additionalFields = new List<string>();
      List<string> buildFields = new List<string>();
      List<string> releaseFields = new List<string>();
      List<string> coreFields = this.SeparateCoreFieldsFromAdditionalFields(fields, out additionalFields, out buildFields, out releaseFields);
      string str1 = additionalFields.Any<string>() ? TestResultsConstants.TrueCondition : TestResultsConstants.FalseCondition;
      string str2 = buildFields.Any<string>() ? TestResultsConstants.TrueCondition : TestResultsConstants.FalseCondition;
      string str3 = releaseFields.Any<string>() ? TestResultsConstants.TrueCondition : TestResultsConstants.FalseCondition;
      string andFieldsToFetch = this.GetDynmSprocAndFieldsToFetch(coreFields, out fieldsToFetch);
      fieldsToFetch = fieldsToFetch.Select<string, string>((System.Func<string, string>) (f => "result." + f)).ToList<string>();
      string sqlStatement = string.Format((IFormatProvider) CultureInfo.InvariantCulture, andFieldsToFetch, (object) string.Join(",", (IEnumerable<string>) fieldsToFetch), (object) str1, (object) str2, (object) str3);
      List<TestCaseResultIdAndRev> ids1 = new List<TestCaseResultIdAndRev>(resultIds.Count);
      HashSet<int> ids2 = new HashSet<int>();
      foreach (TestCaseResultIdentifier resultId in resultIds)
      {
        ids1.Add(new TestCaseResultIdAndRev(resultId, 0));
        ids2.Add(resultId.TestRunId);
      }
      this.PrepareSqlBatch(sqlStatement.Length, true);
      this.AddStatement(sqlStatement, 0, true, true);
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt32TypeTable("@runIds", (IEnumerable<int>) ids2);
      this.BindTestCaseResultIdAndRevTypeTable("@idsTable", (IEnumerable<TestCaseResultIdAndRev>) ids1);
      this.BindNameTypeTable("@additionalFields", (IEnumerable<string>) additionalFields);
      Dictionary<TestCaseResultIdentifier, TestCaseResult> resultsMap = new Dictionary<TestCaseResultIdentifier, TestCaseResult>(resultIds.Count);
      Dictionary<int, BuildConfiguration> dictionary1 = new Dictionary<int, BuildConfiguration>();
      Dictionary<int, ReleaseReference> dictionary2 = new Dictionary<int, ReleaseReference>();
      TestManagementDatabase.FetchTestResultsColumns testResultsColumns = new TestManagementDatabase.FetchTestResultsColumns();
      using (SqlDataReader reader = this.ExecuteReader())
      {
        while (reader.Read())
        {
          TestCaseResult testCaseResult = testResultsColumns.bind(reader);
          testCaseResult.CustomFields = new List<TestExtensionField>();
          resultsMap[new TestCaseResultIdentifier(testCaseResult.TestRunId, testCaseResult.TestResultId)] = testCaseResult;
        }
        if (str1.Equals(TestResultsConstants.TrueCondition) && reader.NextResult())
          this.AddAdditionalFieldDataToResultsMap(resultsMap, reader);
        if (str2.Equals(TestResultsConstants.TrueCondition) && reader.NextResult())
          this.AddBuildFieldDataToBuildMap(dictionary1, reader);
        if (str3.Equals(TestResultsConstants.TrueCondition) && reader.NextResult())
          this.AddReleaseFieldDataToReleaseMap(dictionary2, reader);
        if (dictionary1.Any<KeyValuePair<int, BuildConfiguration>>())
          resultsMap = this.MapBuildRefToResults(resultsMap, dictionary1);
        if (dictionary2.Any<KeyValuePair<int, ReleaseReference>>())
          resultsMap = this.MapReleaseRefToResults(resultsMap, dictionary2);
      }
      return this.RemoveFieldsToNotReturn(resultsMap.Values.ToList<TestCaseResult>(), new HashSet<string>((IEnumerable<string>) fields));
    }

    protected virtual string GetDynmSprocAndFieldsToFetch(
      List<string> coreFields,
      out List<string> fieldsToFetch)
    {
      HashSet<string> source = new HashSet<string>((IEnumerable<string>) coreFields);
      if (source.Contains<string>(TestResultsConstants.TestResultPropertyDuration, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
      {
        if (!source.Contains<string>(TestResultsConstants.TestResultPropertyDateStarted, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
          source.Add(TestResultsConstants.TestResultPropertyDateStarted);
        if (!source.Contains<string>(TestResultsConstants.TestResultPropertyDateCompleted, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
          source.Add(TestResultsConstants.TestResultPropertyDateCompleted);
      }
      fieldsToFetch = source.ToList<string>();
      return TestManagementDynamicSqlBatchStatements.dynprc_QueryTestResultsByIds_UnifyingViews;
    }

    private List<TestCaseResult> RemoveFieldsToNotReturn(
      List<TestCaseResult> results,
      HashSet<string> fields)
    {
      bool returnDateStarted = fields.Contains<string>(TestResultsConstants.TestResultPropertyDateStarted, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      bool returnDateCompleted = fields.Contains<string>(TestResultsConstants.TestResultPropertyDateCompleted, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      results.ForEach((Action<TestCaseResult>) (r =>
      {
        r.DateStarted = returnDateStarted ? r.DateStarted : new DateTime();
        r.DateCompleted = returnDateCompleted ? r.DateCompleted : new DateTime();
      }));
      return results;
    }

    private Dictionary<TestCaseResultIdentifier, TestCaseResult> MapBuildRefToResults(
      Dictionary<TestCaseResultIdentifier, TestCaseResult> resultsMap,
      Dictionary<int, BuildConfiguration> buildMap)
    {
      foreach (KeyValuePair<TestCaseResultIdentifier, TestCaseResult> results in resultsMap)
      {
        int testRunId = results.Key.TestRunId;
        if (buildMap.ContainsKey(testRunId))
          results.Value.BuildReference = buildMap[testRunId];
      }
      return resultsMap;
    }

    private Dictionary<TestCaseResultIdentifier, TestCaseResult> MapReleaseRefToResults(
      Dictionary<TestCaseResultIdentifier, TestCaseResult> resultsMap,
      Dictionary<int, ReleaseReference> releaseMap)
    {
      foreach (KeyValuePair<TestCaseResultIdentifier, TestCaseResult> results in resultsMap)
      {
        int testRunId = results.Key.TestRunId;
        if (releaseMap.ContainsKey(testRunId))
          results.Value.ReleaseReference = releaseMap[testRunId];
      }
      return resultsMap;
    }

    private void AddBuildFieldDataToBuildMap(
      Dictionary<int, BuildConfiguration> buildMap,
      SqlDataReader reader)
    {
      TestManagementDatabase5.FetchBuildReferenceColumns referenceColumns = new TestManagementDatabase5.FetchBuildReferenceColumns();
      while (reader.Read())
        referenceColumns.bind(reader, buildMap);
    }

    private void AddReleaseFieldDataToReleaseMap(
      Dictionary<int, ReleaseReference> releaseMap,
      SqlDataReader reader)
    {
      TestManagementDatabase5.FetchReleaseReferenceColumns referenceColumns = new TestManagementDatabase5.FetchReleaseReferenceColumns();
      while (reader.Read())
        referenceColumns.bind(reader, releaseMap);
    }

    private void AddAdditionalFieldDataToResultsMap(
      Dictionary<TestCaseResultIdentifier, TestCaseResult> resultsMap,
      SqlDataReader reader)
    {
      TestManagementDatabase.FetchTestResultsExColumns resultsExColumns = new TestManagementDatabase.FetchTestResultsExColumns();
      while (reader.Read())
      {
        Tuple<int, int, TestExtensionField> tuple = resultsExColumns.bind(reader);
        TestCaseResultIdentifier key = new TestCaseResultIdentifier(tuple.Item1, tuple.Item2);
        if (resultsMap.ContainsKey(key))
        {
          if (string.Equals("StackTrace", tuple.Item3.Field.Name, StringComparison.OrdinalIgnoreCase))
            resultsMap[key].StackTrace = tuple.Item3;
          else if (string.Equals("FailingSince", tuple.Item3.Field.Name, StringComparison.OrdinalIgnoreCase))
          {
            if (tuple.Item3.Value is string jsonString)
            {
              FailingSince convertedObject = (FailingSince) null;
              if (TestResultHelper.TryJsonConvertWithRetry<FailingSince>(jsonString, out convertedObject, true))
                resultsMap[key].FailingSince = convertedObject;
            }
          }
          else if (string.Equals("Comment", tuple.Item3.Field.Name, StringComparison.OrdinalIgnoreCase))
            resultsMap[key].Comment = tuple.Item3.Value as string;
          else if (string.Equals("ErrorMessage", tuple.Item3.Field.Name, StringComparison.OrdinalIgnoreCase))
            resultsMap[key].ErrorMessage = tuple.Item3.Value as string;
          else if (string.Equals("UnsanitizedTestCaseTitle", tuple.Item3.Field.Name, StringComparison.OrdinalIgnoreCase))
          {
            if (tuple.Item3.Value is string str1)
              resultsMap[key].TestCaseTitle = str1;
          }
          else if (string.Equals("UnsanitizedAutomatedTestName", tuple.Item3.Field.Name, StringComparison.OrdinalIgnoreCase))
          {
            if (tuple.Item3.Value is string str2)
              resultsMap[key].AutomatedTestName = str2;
          }
          else if (string.Equals("MaxReservedSubResultId", tuple.Item3.Field.Name, StringComparison.OrdinalIgnoreCase))
            resultsMap[key].SubResultCount = (int) tuple.Item3.Value;
          else if (string.Equals("TestResultGroupType", tuple.Item3.Field.Name, StringComparison.OrdinalIgnoreCase))
            resultsMap[key].ResultGroupType = (ResultGroupType) TestManagementServiceUtility.ValidateAndGetEnumValue<ResultGroupType>((string) tuple.Item3.Value, ResultGroupType.None);
          else
            resultsMap[key].CustomFields.Add(tuple.Item3);
        }
      }
    }

    protected List<string> SeparateCoreFieldsFromAdditionalFields(
      List<string> fields,
      out List<string> additionalFields,
      out List<string> buildFields,
      out List<string> releaseFields)
    {
      HashSet<string> source = new HashSet<string>();
      additionalFields = new List<string>();
      releaseFields = new List<string>();
      buildFields = new List<string>();
      foreach (string field in fields)
      {
        if (!string.IsNullOrWhiteSpace(field))
        {
          if (TestResultsConstants.TestResultProperties.Contains<string>(field, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
          {
            source.Add(field);
            if (field.Equals(TestResultsConstants.AutomatedTestNameColumn, StringComparison.OrdinalIgnoreCase))
              additionalFields.Add("UnsanitizedAutomatedTestName");
            else if (field.Equals(TestResultsConstants.TestCaseTitleColumnName, StringComparison.OrdinalIgnoreCase))
              additionalFields.Add("UnsanitizedTestCaseTitle");
          }
          else if (field.Equals(TestResultsConstants.ReleaseEnvironmentIdColumnName, StringComparison.OrdinalIgnoreCase) || field.Equals(TestResultsConstants.ReleaseReferenceColumnName, StringComparison.OrdinalIgnoreCase))
            releaseFields.Add(field);
          else if (field.Equals(TestResultsConstants.BuildReferenceColumnName, StringComparison.OrdinalIgnoreCase))
            buildFields.Add(field);
          else
            additionalFields.Add(field);
          if (field != null && (field.Equals(TestResultsConstants.CommentColumnName, StringComparison.OrdinalIgnoreCase) || field.Equals(TestResultsConstants.ErrorMessageColumnName, StringComparison.OrdinalIgnoreCase)))
            additionalFields.Add(field);
        }
      }
      if (!source.Any<string>() && !additionalFields.Any<string>() && !releaseFields.Any<string>())
      {
        foreach (string testResultProperty in TestResultsConstants.TestResultProperties)
          source.Add(testResultProperty);
        if (TestResultsConstants.TestResultProperties.Contains<string>(TestResultsConstants.CommentColumnName, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
          additionalFields.Add(TestResultsConstants.CommentColumnName);
        if (TestResultsConstants.TestResultProperties.Contains<string>(TestResultsConstants.ErrorMessageColumnName, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
          additionalFields.Add(TestResultsConstants.ErrorMessageColumnName);
      }
      if (!source.Contains<string>(TestResultsConstants.TestResultPropertyTestRunId, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
        source.Add(TestResultsConstants.TestResultPropertyTestRunId);
      if (!source.Contains<string>(TestResultsConstants.TestResultPropertyTestResultId, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
        source.Add(TestResultsConstants.TestResultPropertyTestResultId);
      if (!source.Contains<string>(TestResultsConstants.TestResultPropertyTestCaseRefId, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
        source.Add(TestResultsConstants.TestResultPropertyTestCaseRefId);
      return source.ToList<string>();
    }

    protected class FetchBuildReferenceColumns
    {
      private SqlColumnBinder RunId = new SqlColumnBinder(nameof (RunId));
      private SqlColumnBinder BuildId = new SqlColumnBinder(nameof (BuildId));
      private SqlColumnBinder BuildDefinitionId = new SqlColumnBinder(nameof (BuildDefinitionId));

      internal void bind(SqlDataReader reader, Dictionary<int, BuildConfiguration> buildMap)
      {
        BuildConfiguration buildConfiguration = new BuildConfiguration();
        int int32 = this.RunId.GetInt32((IDataReader) reader);
        buildConfiguration.BuildId = this.BuildId.ColumnExists((IDataReader) reader) ? this.BuildId.GetInt32((IDataReader) reader) : 0;
        buildConfiguration.BuildDefinitionId = this.BuildDefinitionId.ColumnExists((IDataReader) reader) ? this.BuildDefinitionId.GetInt32((IDataReader) reader) : 0;
        if (buildMap.ContainsKey(int32))
          return;
        buildMap[int32] = buildConfiguration;
      }
    }

    protected class FetchReleaseReferenceColumns
    {
      private SqlColumnBinder RunId = new SqlColumnBinder(nameof (RunId));
      private SqlColumnBinder ReleaseId = new SqlColumnBinder(nameof (ReleaseId));
      private SqlColumnBinder ReleaseEnvId = new SqlColumnBinder(nameof (ReleaseEnvId));
      private SqlColumnBinder ReleaseDefId = new SqlColumnBinder(nameof (ReleaseDefId));
      private SqlColumnBinder ReleaseEnvDefId = new SqlColumnBinder(nameof (ReleaseEnvDefId));

      internal void bind(SqlDataReader reader, Dictionary<int, ReleaseReference> releaseMap)
      {
        ReleaseReference releaseReference = new ReleaseReference();
        int int32 = this.RunId.GetInt32((IDataReader) reader);
        releaseReference.ReleaseId = this.ReleaseId.ColumnExists((IDataReader) reader) ? this.ReleaseId.GetInt32((IDataReader) reader) : 0;
        releaseReference.ReleaseEnvId = this.ReleaseEnvId.ColumnExists((IDataReader) reader) ? this.ReleaseEnvId.GetInt32((IDataReader) reader) : 0;
        releaseReference.ReleaseDefId = this.ReleaseDefId.ColumnExists((IDataReader) reader) ? this.ReleaseDefId.GetInt32((IDataReader) reader) : 0;
        releaseReference.ReleaseEnvDefId = this.ReleaseEnvDefId.ColumnExists((IDataReader) reader) ? this.ReleaseEnvDefId.GetInt32((IDataReader) reader) : 0;
        if (releaseMap.ContainsKey(int32))
          return;
        releaseMap[int32] = releaseReference;
      }
    }
  }
}
