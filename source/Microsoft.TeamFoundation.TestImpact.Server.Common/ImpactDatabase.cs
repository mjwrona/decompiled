// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestImpact.Server.Common.ImpactDatabase
// Assembly: Microsoft.TeamFoundation.TestImpact.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 079E4AEE-0642-4BDD-8B76-CECF38EBB798
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestImpact.Server.Common.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestImpact.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.TestImpact.Server.Common
{
  [CLSCompliant(false)]
  public class ImpactDatabase : TestImpactServiceDatabaseBase, IImpactDataBaseComponent
  {
    private static readonly SqlMetaData[] typ_SignatureTable = new SqlMetaData[2]
    {
      new SqlMetaData("SignatureType", SqlDbType.TinyInt),
      new SqlMetaData("CodeSignature", SqlDbType.Char, 40L)
    };
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[3]
    {
      (IComponentCreator) new ComponentCreator<ImpactDatabase>(1),
      (IComponentCreator) new ComponentCreator<ImpactDatabase2>(2),
      (IComponentCreator) new ComponentCreator<ImpactDatabase3>(3)
    }, "TestImpactService");

    public ImpactDatabase()
    {
    }

    internal ImpactDatabase(string connectionString, IVssRequestContext requestContext)
      : base(connectionString, requestContext)
    {
    }

    public virtual void PublishTestSignatures(
      Guid projectId,
      int testRunId,
      int testResultId,
      int configurationId,
      int definitionType,
      int definitionId,
      IEnumerable<Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.Signature> signatures,
      string automatedTestName)
    {
      this.PrepareStoredProcedure(TestImpactServiceDatabaseBase.GetDatabaseObjectName("prc_PublishCodeSignatures"));
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "TestManagement"));
      this.BindInt("@testRunId", testRunId);
      this.BindInt("@testResultId", testResultId);
      this.BindInt("@configurationId", configurationId);
      this.BindByte("@definitionType", (byte) definitionType);
      this.BindInt("@definitionId", definitionId);
      this.BindSignatureTable("@signatures", signatures);
      this.ExecuteNonQuery();
    }

    public ImpactedTests QueryImpactedTests(
      Guid projectId,
      int definitionType,
      int definitionId,
      int runId)
    {
      this.PrepareStoredProcedure(TestImpactServiceDatabaseBase.GetDatabaseObjectName("prc_QueryImpactedTests"));
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "TestManagement"));
      this.BindByte("@definitionType", (byte) definitionType);
      this.BindInt("@definitionId", definitionId);
      this.BindInt("@runId", runId);
      ImpactedTests impactedTests = new ImpactedTests();
      ImpactDatabase.QueryImpactedTestsColumns impactedTestsColumns = new ImpactDatabase.QueryImpactedTestsColumns();
      using (SqlDataReader reader = this.ExecuteReader())
      {
        while (reader.Read())
        {
          int int32 = impactedTestsColumns.TestCaseId.GetInt32((IDataReader) reader);
          string str = impactedTestsColumns.TestMethodName.GetString((IDataReader) reader, false);
          impactedTests.Tests.Add(new Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.Test()
          {
            AutomatedTestId = Guid.Empty,
            TestCaseId = int32,
            TestName = str
          });
        }
      }
      return impactedTests;
    }

    public virtual ImpactedTests QueryAllTests(
      Guid projectId,
      int definitionType,
      int definitionId,
      int runId)
    {
      return new ImpactedTests();
    }

    public void DeleteTestMethod(int deletionBatchSize, int waitDaysForCleanup)
    {
      this.PrepareStoredProcedure(TestImpactServiceDatabaseBase.GetDatabaseObjectName("prc_DeleteTestMethod"));
      this.BindInt("@deletionBatchSize", deletionBatchSize);
      this.BindInt("@waitDaysForCleanup", waitDaysForCleanup);
      this.ExecuteNonQuery();
    }

    protected SqlParameter BindSignatureTable(
      string parameterName,
      IEnumerable<Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.Signature> signatures)
    {
      signatures = signatures ?? Enumerable.Empty<Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.Signature>();
      return this.BindTable(parameterName, TestImpactServiceDatabaseBase.GetDatabaseObjectName("typ_SignatureTable"), this.BindSignatureTableRows(signatures));
    }

    private IEnumerable<SqlDataRecord> BindSignatureTableRows(IEnumerable<Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.Signature> signatures)
    {
      foreach (Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.Signature signature in signatures)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ImpactDatabase.typ_SignatureTable);
        sqlDataRecord.SetByte(0, (byte) signature.SignatureType);
        sqlDataRecord.SetString(1, signature.CodeSignature);
        yield return sqlDataRecord;
      }
    }

    internal class QueryImpactedTestsColumns
    {
      internal SqlColumnBinder TestCaseId = new SqlColumnBinder(nameof (TestCaseId));
      internal SqlColumnBinder TestMethodName = new SqlColumnBinder(nameof (TestMethodName));
    }

    internal class QueryAllTestsColumn
    {
      internal SqlColumnBinder TestMethodName = new SqlColumnBinder(nameof (TestMethodName));
    }
  }
}
