// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase71
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase71 : TestManagementDatabase70
  {
    public override void CreateBuildConfiguration(Guid projectId, BuildConfiguration buildRef)
    {
      this.PrepareStoredProcedure("prc_CreateBuildConfiguration");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindBuildRefTypeTable4("@buildRef", (IEnumerable<BuildConfiguration>) new BuildConfiguration[1]
      {
        buildRef
      });
      this.ExecuteNonQuery();
    }

    internal TestManagementDatabase71(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase71()
    {
    }

    internal override List<TestResultExArchivalRecord> QueryTestResultExtensionsByTestRunChangedDate(
      int dataspaceId,
      int runBatchSize,
      int resultExBatchSize,
      TestResultExArchivalWatermark fromWatermark,
      DateTime maxTestRunUpdatedDate,
      out TestResultExArchivalWatermark toWatermark,
      TestArtifactSource dataSource,
      List<string> fieldNames = null,
      List<int> runStates = null,
      List<int> excludedRunTypes = null)
    {
      this.PrepareStoredProcedure("TestResult.prc_QueryTestResultExtensionsByTestRunChangedDate");
      this.BindInt("@dataspaceId", dataspaceId);
      this.BindInt("@runBatchSize", runBatchSize);
      this.BindInt("@resultExBatchSize", resultExBatchSize);
      this.BindDateTime("@fromRunChangedDate", fromWatermark != null ? fromWatermark.TestRunUpdatedDate : new DateTime(), true);
      this.BindDateTime("@toRunChangedDate", maxTestRunUpdatedDate, true);
      this.BindInt("@fromRunId", fromWatermark != null ? fromWatermark.TestRunId : 0);
      this.BindInt("@fromResultId", fromWatermark != null ? fromWatermark.TestResultId : 0);
      if (fieldNames == null)
        fieldNames = new List<string>()
        {
          "ErrorMessage",
          "Comment",
          "StackTrace"
        };
      if (runStates == null)
        runStates = new List<int>() { 3, 6 };
      if (excludedRunTypes == null)
        excludedRunTypes = new List<int>() { 32 };
      this.BindNameTypeTable("@fieldNames", (IEnumerable<string>) fieldNames);
      this.BindIdTypeTable("@runStates", (IEnumerable<int>) runStates);
      this.BindIdTypeTable("@excludedTypes", (IEnumerable<int>) excludedRunTypes);
      SqlDataReader reader = this.ExecuteReader();
      TestManagementDatabase58.FetchTestResultExArchivalRecord exArchivalRecord1 = new TestManagementDatabase58.FetchTestResultExArchivalRecord();
      TestManagementDatabase58.FetchFieldExMappingColumn fieldExMappingColumn = new TestManagementDatabase58.FetchFieldExMappingColumn();
      Dictionary<int, string> mapping = new Dictionary<int, string>();
      mapping[0] = (string) null;
      toWatermark = new TestResultExArchivalWatermark()
      {
        TestRunUpdatedDate = fromWatermark != null ? fromWatermark.TestRunUpdatedDate : new DateTime(),
        TestRunId = fromWatermark != null ? fromWatermark.TestRunId : 0,
        TestResultId = fromWatermark != null ? fromWatermark.TestResultId : 0
      };
      List<TestResultExArchivalRecord> source = new List<TestResultExArchivalRecord>();
      List<TestResultExArchivalRecord> collection = new List<TestResultExArchivalRecord>();
      TestResultExArchivalRecord record = (TestResultExArchivalRecord) null;
      bool flag = false;
      Guid dataspaceIdentifier = this.GetDataspaceIdentifier(dataspaceId);
      while (reader.Read())
      {
        KeyValue<int, string> keyValue = fieldExMappingColumn.Bind(reader);
        mapping[keyValue.Key] = keyValue.Value;
      }
      if (reader.NextResult())
      {
        while (reader.Read())
          collection.Add(exArchivalRecord1.Bind(reader, (IDictionary<int, string>) mapping, dataspaceIdentifier));
      }
      if (reader.NextResult())
      {
        while (reader.Read())
        {
          TestResultExArchivalRecord exArchivalRecord2 = exArchivalRecord1.Bind(reader, (IDictionary<int, string>) mapping, dataspaceIdentifier);
          flag = flag || exArchivalRecord2.TestRunId == (fromWatermark != null ? fromWatermark.TestRunId : 0);
          source.Add(exArchivalRecord2);
        }
      }
      if (!flag)
        source.InsertRange(0, (IEnumerable<TestResultExArchivalRecord>) collection);
      if (reader.NextResult() && reader.Read())
        record = exArchivalRecord1.Bind(reader, (IDictionary<int, string>) mapping, dataspaceIdentifier);
      toWatermark = source.Any<TestResultExArchivalRecord>() || record != null ? (record != null ? new TestResultExArchivalWatermark(record) : new TestResultExArchivalWatermark(source.Last<TestResultExArchivalRecord>())) : new TestResultExArchivalWatermark(maxTestRunUpdatedDate, 0, 0);
      return source;
    }

    internal override IEnumerable<KeyValuePair<string, int>> GetTCMServiceMigrationThreshold(
      bool isTCMService = false)
    {
      string storedProcedure = "prc_GetTCMServiceMigrationThreshold";
      this.PrepareStoredProcedure(storedProcedure);
      this.BindBoolean("@isTcmService", isTCMService);
      SqlDataReader reader = this.ExecuteReader();
      if (!reader.Read())
        throw new UnexpectedDatabaseResultException(storedProcedure);
      TestManagementDatabase71.MigrationThreshold migrationThreshold1 = new TestManagementDatabase71.MigrationThreshold();
      List<KeyValuePair<string, int>> migrationThreshold2 = new List<KeyValuePair<string, int>>();
      migrationThreshold2.Add(new KeyValuePair<string, int>("TestRunThreshold", migrationThreshold1.TestRunThreshold.GetInt32((IDataReader) reader, 0) + 1000000));
      if (!reader.NextResult() || !reader.Read())
        throw new UnexpectedDatabaseResultException(storedProcedure);
      migrationThreshold2.Add(new KeyValuePair<string, int>("TestAttachmentThreshold", migrationThreshold1.TestAttachmentThreshold.GetInt32((IDataReader) reader, 0) + 1000000));
      return (IEnumerable<KeyValuePair<string, int>>) migrationThreshold2;
    }

    private new class MigrationThreshold
    {
      public SqlColumnBinder TestRunThreshold = new SqlColumnBinder(nameof (TestRunThreshold));
      public SqlColumnBinder TestAttachmentThreshold = new SqlColumnBinder(nameof (TestAttachmentThreshold));
    }
  }
}
