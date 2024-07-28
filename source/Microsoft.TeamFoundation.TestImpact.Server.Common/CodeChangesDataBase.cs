// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestImpact.Server.Common.CodeChangesDataBase
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
  public class CodeChangesDataBase : TestImpactServiceDatabaseBase, ICodeChangesDataBaseComponent
  {
    private static readonly SqlMetaData[] typ_TiaCodeChangeTable = new SqlMetaData[5]
    {
      new SqlMetaData("AssemblyName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("FileName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("MethodName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("SignatureType", SqlDbType.TinyInt),
      new SqlMetaData("Signature", SqlDbType.Char, 40L)
    };
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[3]
    {
      (IComponentCreator) new ComponentCreator<CodeChangesDataBase>(1),
      (IComponentCreator) new ComponentCreator<CodeChangesDataBase>(2),
      (IComponentCreator) new ComponentCreator<CodeChangesDataBase>(3)
    }, "TestImpactService");

    public CodeChangesDataBase()
    {
    }

    internal CodeChangesDataBase(string connectionString, IVssRequestContext requestContext)
      : base(connectionString, requestContext)
    {
    }

    public TestImpactBuildData QueryCodeChanges(
      Guid projectId,
      int definitionType,
      int definitionId,
      int runId)
    {
      this.PrepareStoredProcedure(TestImpactServiceDatabaseBase.GetDatabaseObjectName("prc_QueryCodeChanges"));
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "TestManagement"));
      this.BindByte("@definitionType", (byte) definitionType);
      this.BindInt("@definitionId", definitionId);
      this.BindInt("@runId", runId);
      TestImpactBuildData testImpactBuildData = new TestImpactBuildData();
      List<Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.CodeChange> codeChangeList = new List<Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.CodeChange>();
      CodeChangesDataBase.QueryCodeChangesColumns codeChangesColumns = new CodeChangesDataBase.QueryCodeChangesColumns();
      using (SqlDataReader reader = this.ExecuteReader())
      {
        if (reader.HasRows)
        {
          while (reader.Read())
          {
            string str1 = codeChangesColumns.AssemblyName.GetString((IDataReader) reader, true);
            string str2 = codeChangesColumns.CodeSignature.GetString((IDataReader) reader, false);
            string str3 = codeChangesColumns.FileName.GetString((IDataReader) reader, true);
            string str4 = codeChangesColumns.MethodName.GetString((IDataReader) reader, true);
            byte num = codeChangesColumns.SignatureType.GetByte((IDataReader) reader);
            codeChangeList.Add(new Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.CodeChange()
            {
              AssemblyName = str1,
              CodeSignature = str2,
              FileName = str3,
              Name = str4,
              SignatureType = (SignatureType) num
            });
          }
        }
      }
      testImpactBuildData.CodeChanges = codeChangeList.ToArray();
      return testImpactBuildData;
    }

    public void PublishCodeChanges(
      Guid projectId,
      int definitionType,
      int definitionId,
      int runId,
      int signatureType,
      IEnumerable<Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.CodeChange> changes,
      int rebaseLimit = 50)
    {
      this.PrepareStoredProcedure(TestImpactServiceDatabaseBase.GetDatabaseObjectName("prc_PublishBuildCodeChanges"));
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "TestManagement"));
      this.BindByte("@definitionType", (byte) definitionType);
      this.BindInt("@definitionId", definitionId);
      this.BindInt("@runId", runId);
      this.BindByte("@signatureType", (byte) signatureType);
      this.BindCodeChangeTable("@changes", changes);
      this.BindInt("@rebaseLimit", rebaseLimit);
      this.ExecuteNonQuery();
    }

    private SqlParameter BindCodeChangeTable(string parameterName, IEnumerable<Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.CodeChange> changes)
    {
      changes = changes ?? Enumerable.Empty<Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.CodeChange>();
      return this.BindTable(parameterName, TestImpactServiceDatabaseBase.GetDatabaseObjectName("typ_CodeChangeTable"), this.BindTiaCodeChangeTableRows(changes));
    }

    private IEnumerable<SqlDataRecord> BindTiaCodeChangeTableRows(IEnumerable<Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.CodeChange> changes)
    {
      foreach (Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.CodeChange change in changes)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(CodeChangesDataBase.typ_TiaCodeChangeTable);
        sqlDataRecord.SetString(0, change.AssemblyName);
        sqlDataRecord.SetString(1, string.IsNullOrEmpty(change.FileName) ? string.Empty : change.FileName);
        sqlDataRecord.SetString(2, string.IsNullOrEmpty(change.Name) ? string.Empty : change.Name);
        sqlDataRecord.SetByte(3, (byte) change.SignatureType);
        sqlDataRecord.SetString(4, change.CodeSignature);
        yield return sqlDataRecord;
      }
    }

    internal class QueryCodeChangesColumns
    {
      internal SqlColumnBinder SignatureType = new SqlColumnBinder(nameof (SignatureType));
      internal SqlColumnBinder CodeSignature = new SqlColumnBinder(nameof (CodeSignature));
      internal SqlColumnBinder MethodName = new SqlColumnBinder(nameof (MethodName));
      internal SqlColumnBinder FileName = new SqlColumnBinder(nameof (FileName));
      internal SqlColumnBinder AssemblyName = new SqlColumnBinder(nameof (AssemblyName));
    }
  }
}
