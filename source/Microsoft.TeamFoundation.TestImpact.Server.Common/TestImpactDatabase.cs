// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestImpact.Server.Common.TestImpactDatabase
// Assembly: Microsoft.TeamFoundation.TestImpact.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 079E4AEE-0642-4BDD-8B76-CECF38EBB798
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestImpact.Server.Common.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.TestImpact.Server.Common
{
  [CLSCompliant(false)]
  public class TestImpactDatabase : TeamFoundationSqlResourceComponent
  {
    private static readonly SqlMetaData[] typ_SignatureTable = new SqlMetaData[1]
    {
      new SqlMetaData("Signature", SqlDbType.Char, 40L)
    };
    private static readonly SqlMetaData[] typ_BuildDefinitionUriTable = new SqlMetaData[1]
    {
      new SqlMetaData("BuildDefinitionUri", SqlDbType.NVarChar, 256L)
    };
    private static readonly SqlMetaData[] typ_CodeChangeTable = new SqlMetaData[9]
    {
      new SqlMetaData("AssemblyName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("AssemblyIdentifier", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Name", SqlDbType.NVarChar, 1024L),
      new SqlMetaData("Signature", SqlDbType.Char, 40L),
      new SqlMetaData("MethodKind", SqlDbType.TinyInt),
      new SqlMetaData("MethodAccess", SqlDbType.TinyInt),
      new SqlMetaData("FileName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("Reason", SqlDbType.TinyInt),
      new SqlMetaData("Changesets", SqlDbType.NVarChar, 1024L)
    };
    private static readonly DateTime MinSqlDateTime = (DateTime) SqlDateTime.MinValue;
    private static readonly DateTime MaxSqlDateTime = (DateTime) SqlDateTime.MaxValue;

    public TestImpactDatabase() => this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;

    internal void CreateBuildImpact(
      string buildUri,
      string buildDefinitionUri,
      string projectUri,
      DateTime startTime,
      IEnumerable<CodeChange> changes)
    {
      this.PrepareStoredProcedure(this.GetTestResultQuerySprocName("prc_CreateBuildImpact"));
      this.BindString("@buildUri", buildUri, 256, false, SqlDbType.NVarChar);
      this.BindString("@buildDefinitionUri", buildDefinitionUri, 256, false, SqlDbType.NVarChar);
      this.BindString("@projectUri", projectUri, 256, false, SqlDbType.NVarChar);
      this.BindDateTime("@startTime", this.SqlDateTimeBoundsCheck(startTime));
      this.BindCodeChangeTable("@changes", changes);
      this.ExecuteNonQuery();
    }

    internal void GetMaxMinBuildStartTimeInTestBuild(
      out DateTime? maxBuildStartTime,
      out DateTime? minBuildStartTime)
    {
      maxBuildStartTime = new DateTime?();
      minBuildStartTime = new DateTime?();
      this.PrepareStoredProcedure("prc_QueryMaxMinBuildStartTime");
      using (SqlDataReader sqlDataReader = this.ExecuteReader())
      {
        if (!sqlDataReader.HasRows || !sqlDataReader.Read() || sqlDataReader.IsDBNull(0) || sqlDataReader.IsDBNull(1))
          return;
        maxBuildStartTime = (DateTime?) sqlDataReader["MaxBuildStartTime"];
        minBuildStartTime = (DateTime?) sqlDataReader["MinBuildStartTime"];
      }
    }

    internal int CreateTestBuild(
      string buildUri,
      string buildDefinitionUri,
      string projectUri,
      DateTime startTime)
    {
      this.PrepareStoredProcedure("prc_CreateTestBuild");
      this.BindString("@buildUri", buildUri, 256, false, SqlDbType.NVarChar);
      this.BindString("@buildDefinitionUri", buildDefinitionUri, 256, false, SqlDbType.NVarChar);
      this.BindString("@projectUri", projectUri, 256, false, SqlDbType.NVarChar);
      this.BindDateTime("@startTime", this.SqlDateTimeBoundsCheck(startTime));
      SqlParameter sqlParameter = this.BindInt("@buildId", 0);
      sqlParameter.Direction = ParameterDirection.Output;
      this.ExecuteNonQuery();
      return (int) sqlParameter.Value;
    }

    internal void CreateTestCodeSignatures(
      int testRunId,
      int testResultId,
      int configurationId,
      IEnumerable<string> signatures)
    {
      this.PrepareStoredProcedure(this.GetTestResultQuerySprocName("prc_CreateTestCodeSignatures"));
      this.BindInt("@testRunId", testRunId);
      this.BindInt("@testResultId", testResultId);
      this.BindInt("@configurationId", configurationId);
      this.BindSignatureTable("@signatures", signatures);
      this.ExecuteNonQuery();
    }

    public int DeleteBuildImpact(string buildUri, int resumeStage, int batchSize = 5000)
    {
      this.PrepareStoredProcedure("prc_DeleteBuildImpact");
      this.BindString("@buildUri", buildUri, 256, true, SqlDbType.NVarChar);
      this.BindInt("@resumeStage", resumeStage);
      this.BindInt("@batchSize", batchSize);
      return (int) this.ExecuteScalar();
    }

    internal void DeleteProjectImpact(string projectUri)
    {
      this.PrepareStoredProcedure("prc_DeleteProjectImpact");
      this.BindString("@projectUri", projectUri, 256, false, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    internal List<TestBuild> GetTestBuildsAfterGivenStartTime(
      DateTime buildStartTime,
      int batchSize)
    {
      this.PrepareStoredProcedure("prc_QueryTestBuildByBuildStartTime");
      this.BindDateTime("@buildstartTime", buildStartTime);
      this.BindInt("@batchSize", batchSize);
      List<TestBuild> afterGivenStartTime = new List<TestBuild>();
      using (SqlDataReader sqlDataReader = this.ExecuteReader())
      {
        if (sqlDataReader.HasRows)
        {
          int ordinal1 = sqlDataReader.GetOrdinal("BuildStartTime");
          int ordinal2 = sqlDataReader.GetOrdinal("BuildUri");
          while (sqlDataReader.Read())
            afterGivenStartTime.Add(new TestBuild()
            {
              BuildStartTime = (DateTime?) sqlDataReader[ordinal1],
              BuildUri = (string) sqlDataReader[ordinal2]
            });
        }
      }
      return afterGivenStartTime;
    }

    public List<TestBuild> QueryTestBuilds(bool isDeleted, int batchSize)
    {
      this.PrepareStoredProcedure("prc_QueryTestBuilds");
      int parameterValue = 0;
      if (isDeleted)
        parameterValue = 1;
      this.BindInt("@isDeleted", parameterValue);
      this.BindInt("@batchSize", batchSize);
      List<TestBuild> testBuildList = new List<TestBuild>();
      using (SqlDataReader sqlDataReader = this.ExecuteReader())
      {
        if (sqlDataReader.HasRows)
        {
          int ordinal = sqlDataReader.GetOrdinal("IsDeleted");
          while (sqlDataReader.Read())
            testBuildList.Add(new TestBuild()
            {
              IsDeleted = !sqlDataReader.IsDBNull(ordinal) && sqlDataReader.GetBoolean(ordinal),
              BuildUri = (string) sqlDataReader["BuildUri"]
            });
        }
      }
      return testBuildList;
    }

    public SqlDataReader QueryCodeChangesForBuild(string buildUri)
    {
      this.PrepareStoredProcedure("prc_QueryCodeChangesForBuild");
      this.BindString("@buildUri", buildUri, 256, false, SqlDbType.NVarChar);
      return this.ExecuteReader();
    }

    public SqlDataReader QueryImpactedTestsForBuild(string buildUri)
    {
      this.PrepareStoredProcedure("prc_QueryImpactedTestsForBuild");
      this.BindString("@buildUri", buildUri, 256, false, SqlDbType.NVarChar);
      return this.ExecuteReader();
    }

    public SqlDataReader QueryImpactMappingsForBuild(string buildUri)
    {
      this.PrepareStoredProcedure("prc_QueryImpactMappingsForBuild");
      this.BindString("@buildUri", buildUri, 256, false, SqlDbType.NVarChar);
      return this.ExecuteReader();
    }

    internal void QueueDeleteTestBuild(string buildUri)
    {
      this.PrepareStoredProcedure("prc_QueueDeleteTestBuild");
      this.BindString("@buildUri", buildUri, 256, true, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    public SqlDataReader QueryTestsWithCodeSignatures(IEnumerable<string> buildDefinitionUris)
    {
      this.PrepareStoredProcedure(this.GetTestResultQuerySprocName("prc_QueryTestsWithCodeSignatures"));
      this.BindBuildDefinitionUriTable("@definitions", buildDefinitionUris);
      return this.ExecuteReader();
    }

    internal SqlDataReader QueryCodeSignaturesForResult(
      int testRunId,
      int testResultId,
      int configurationId)
    {
      this.PrepareStoredProcedure("prc_QueryCodeSignaturesForResult");
      this.BindInt("@testRunId", testRunId);
      this.BindInt("@testResultId", testResultId);
      this.BindInt("@configurationId", configurationId);
      return this.ExecuteReader();
    }

    private DateTime SqlDateTimeBoundsCheck(DateTime time)
    {
      if (time < TestImpactDatabase.MinSqlDateTime)
        return TestImpactDatabase.MinSqlDateTime;
      return time > TestImpactDatabase.MaxSqlDateTime ? TestImpactDatabase.MaxSqlDateTime : time;
    }

    internal virtual string GetTestResultQuerySprocName(string sprocName) => "TestResult." + sprocName;

    protected SqlParameter BindSignatureTable(string parameterName, IEnumerable<string> signatures)
    {
      signatures = signatures ?? Enumerable.Empty<string>();
      return this.BindTable(parameterName, "typ_SignatureTable", this.BindSignatureTableRows(signatures));
    }

    private IEnumerable<SqlDataRecord> BindSignatureTableRows(IEnumerable<string> signatures)
    {
      foreach (string signature in signatures)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestImpactDatabase.typ_SignatureTable);
        sqlDataRecord.SetString(0, signature);
        yield return sqlDataRecord;
      }
    }

    protected SqlParameter BindBuildDefinitionUriTable(
      string parameterName,
      IEnumerable<string> buildUris)
    {
      buildUris = buildUris ?? Enumerable.Empty<string>();
      return this.BindTable(parameterName, "typ_BuildDefinitionUriTable", this.BindBuildDefinitionUriTableRows(buildUris));
    }

    private IEnumerable<SqlDataRecord> BindBuildDefinitionUriTableRows(IEnumerable<string> buildUris)
    {
      foreach (string buildUri in buildUris)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestImpactDatabase.typ_BuildDefinitionUriTable);
        sqlDataRecord.SetString(0, buildUri);
        yield return sqlDataRecord;
      }
    }

    protected SqlParameter BindCodeChangeTable(
      string parameterName,
      IEnumerable<CodeChange> changes)
    {
      changes = changes ?? Enumerable.Empty<CodeChange>();
      return this.BindTable(parameterName, "typ_CodeChangeTable", this.BindCodeChangeTableRows(changes));
    }

    private IEnumerable<SqlDataRecord> BindCodeChangeTableRows(IEnumerable<CodeChange> changes)
    {
      foreach (CodeChange change in changes)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestImpactDatabase.typ_CodeChangeTable);
        sqlDataRecord.SetString(0, change.AssemblyName);
        sqlDataRecord.SetGuid(1, change.AssemblyIdentifier);
        sqlDataRecord.SetString(2, change.Name);
        sqlDataRecord.SetString(3, change.Signature);
        sqlDataRecord.SetByte(4, (byte) change.MethodKind);
        sqlDataRecord.SetByte(5, (byte) change.MethodAccess);
        sqlDataRecord.SetString(6, string.IsNullOrEmpty(change.FileName) ? string.Empty : change.FileName);
        sqlDataRecord.SetByte(7, (byte) change.Reason);
        string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[{0}]", (object) string.Join<int>(";", (IEnumerable<int>) change.Changesets));
        sqlDataRecord.SetString(8, str);
        yield return sqlDataRecord;
      }
    }

    internal class QueryCodeChangesColumns
    {
      internal SqlColumnBinder BuildCodeChangeId = new SqlColumnBinder(nameof (BuildCodeChangeId));
      internal SqlColumnBinder AssemblyName = new SqlColumnBinder(nameof (AssemblyName));
      internal SqlColumnBinder AssemblyIdentifier = new SqlColumnBinder(nameof (AssemblyIdentifier));
      internal SqlColumnBinder CodeChangeName = new SqlColumnBinder(nameof (CodeChangeName));
      internal SqlColumnBinder CodeChangeSignature = new SqlColumnBinder(nameof (CodeChangeSignature));
      internal SqlColumnBinder Reason = new SqlColumnBinder(nameof (Reason));
      internal SqlColumnBinder Changesets = new SqlColumnBinder(nameof (Changesets));
      internal SqlColumnBinder FileName = new SqlColumnBinder(nameof (FileName));
      internal SqlColumnBinder MethodKind = new SqlColumnBinder(nameof (MethodKind));
      internal SqlColumnBinder MethodAccess = new SqlColumnBinder(nameof (MethodAccess));
    }

    internal class QueryImpactedTestsColumns
    {
      internal SqlColumnBinder ImpactedTestId = new SqlColumnBinder(nameof (ImpactedTestId));
      internal SqlColumnBinder TestCaseId = new SqlColumnBinder(nameof (TestCaseId));
      internal SqlColumnBinder AutomatedTestId = new SqlColumnBinder(nameof (AutomatedTestId));
      internal SqlColumnBinder AutomatedTestName = new SqlColumnBinder(nameof (AutomatedTestName));
      internal SqlColumnBinder AutomatedTestType = new SqlColumnBinder(nameof (AutomatedTestType));
      internal SqlColumnBinder DateCompleted = new SqlColumnBinder(nameof (DateCompleted));
    }

    internal class QueryImpactMappingsColumns
    {
      internal SqlColumnBinder ImpactedTestId = new SqlColumnBinder(nameof (ImpactedTestId));
      internal SqlColumnBinder BuildCodeChangeId = new SqlColumnBinder(nameof (BuildCodeChangeId));
    }

    internal class QueryTestsWithCodeSignaturesColumns
    {
      internal SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      internal SqlColumnBinder TestResultId = new SqlColumnBinder(nameof (TestResultId));
      internal SqlColumnBinder ConfigurationId = new SqlColumnBinder(nameof (ConfigurationId));
      internal SqlColumnBinder TestCaseId = new SqlColumnBinder(nameof (TestCaseId));
      internal SqlColumnBinder AutomatedTestId = new SqlColumnBinder(nameof (AutomatedTestId));
      internal SqlColumnBinder AutomatedTestName = new SqlColumnBinder(nameof (AutomatedTestName));
      internal SqlColumnBinder AutomatedTestType = new SqlColumnBinder(nameof (AutomatedTestType));
      internal SqlColumnBinder DateCompleted = new SqlColumnBinder(nameof (DateCompleted));
    }

    internal class QueryCodeSignaturesForResultColumns
    {
      internal SqlColumnBinder CodeSignature = new SqlColumnBinder(nameof (CodeSignature));
    }
  }
}
