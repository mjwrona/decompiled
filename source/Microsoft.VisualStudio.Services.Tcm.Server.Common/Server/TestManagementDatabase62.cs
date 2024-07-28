// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase62
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
  public class TestManagementDatabase62 : TestManagementDatabase61
  {
    internal TestManagementDatabase62(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase62()
    {
    }

    internal override List<TestCaseReferenceRecord> GetTestResultsMetaData(
      Guid projectId,
      IList<int> testcaseReferenceIds,
      bool shouldIncludeFlakyDetails = false)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.GetTestResultsMetaData");
        Dictionary<int, TestCaseReferenceRecord> dictionary1 = new Dictionary<int, TestCaseReferenceRecord>();
        this.PrepareStoredProcedure("TestResult.prc_QueryTestCaseReferenceByIds");
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindIdTypeTable("@testcaseReferenceIdTable", testcaseReferenceIds != null ? (IEnumerable<int>) testcaseReferenceIds.Distinct<int>().ToList<int>() : (IEnumerable<int>) null);
        this.BindBoolean("@includeFlakyIdentifiers", shouldIncludeFlakyDetails);
        SqlDataReader reader = this.ExecuteReader();
        TestManagementDatabase31.FetchTestCaseRefRecords testCaseRefRecords = new TestManagementDatabase31.FetchTestCaseRefRecords(TestArtifactSource.Tfs);
        while (reader.Read())
        {
          TestCaseReferenceRecord caseReferenceRecord = testCaseRefRecords.Bind(reader, projectId, out DateTime _);
          dictionary1.Add(caseReferenceRecord.TestCaseReferenceId, caseReferenceRecord);
        }
        if (reader.NextResult())
        {
          Dictionary<int, List<string>> dictionary2 = new Dictionary<int, List<string>>();
          TestManagementDatabase62.TestCaseFlakyIdentifierColumns identifierColumns = new TestManagementDatabase62.TestCaseFlakyIdentifierColumns();
          while (reader.Read())
          {
            TestCaseFlakyIdentifier caseFlakyIdentifier = identifierColumns.Bind(reader);
            if (dictionary2.ContainsKey(caseFlakyIdentifier.TestCaseReferenceId))
            {
              dictionary2[caseFlakyIdentifier.TestCaseReferenceId].Add(caseFlakyIdentifier.BranchName);
            }
            else
            {
              dictionary2[caseFlakyIdentifier.TestCaseReferenceId] = new List<string>();
              dictionary2[caseFlakyIdentifier.TestCaseReferenceId].Add(caseFlakyIdentifier.BranchName);
            }
          }
          foreach (int key in dictionary2.Keys)
          {
            if (dictionary1.ContainsKey(key))
              dictionary1[key].TestFlakyBranchName = dictionary2[key];
          }
        }
        return dictionary1.Values.ToList<TestCaseReferenceRecord>();
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.GetTestResultsMetaData");
      }
    }

    protected class TestCaseFlakyIdentifierColumns
    {
      private SqlColumnBinder TestCaseRefId = new SqlColumnBinder(nameof (TestCaseRefId));
      private SqlColumnBinder BranchName = new SqlColumnBinder(nameof (BranchName));

      internal TestCaseFlakyIdentifier Bind(SqlDataReader reader) => new TestCaseFlakyIdentifier()
      {
        TestCaseReferenceId = this.TestCaseRefId.GetInt32((IDataReader) reader),
        BranchName = this.BranchName.GetString((IDataReader) reader, true)
      };
    }
  }
}
