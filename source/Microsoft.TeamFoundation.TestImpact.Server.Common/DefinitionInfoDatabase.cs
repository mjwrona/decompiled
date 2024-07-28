// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestImpact.Server.Common.DefinitionInfoDatabase
// Assembly: Microsoft.TeamFoundation.TestImpact.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 079E4AEE-0642-4BDD-8B76-CECF38EBB798
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestImpact.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.TestImpact.Server.Common
{
  [CLSCompliant(false)]
  public class DefinitionInfoDatabase : 
    TestImpactServiceDatabaseBase,
    IDefinitionInfoDatabaseComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[3]
    {
      (IComponentCreator) new ComponentCreator<DefinitionInfoDatabase>(1),
      (IComponentCreator) new ComponentCreator<DefinitionInfoDatabase>(2),
      (IComponentCreator) new ComponentCreator<DefinitionInfoDatabase>(3)
    }, "TestImpactService");

    public DefinitionInfoDatabase()
    {
    }

    internal DefinitionInfoDatabase(string connectionString, IVssRequestContext requestContext)
      : base(connectionString, requestContext)
    {
    }

    public SqlDataReader QueryDefinitionInfo(bool isDeleted)
    {
      this.PrepareStoredProcedure(TestImpactServiceDatabaseBase.GetDatabaseObjectName("prc_QueryDefinitionInfo"));
      this.BindBoolean("@isDeleted", isDeleted);
      return this.ExecuteReader();
    }

    public List<DefinitionInfo> QueryDefinitionInfoList(bool isDeleted)
    {
      List<DefinitionInfo> definitionInfoList = new List<DefinitionInfo>();
      using (SqlDataReader sqlDataReader = this.QueryDefinitionInfo(isDeleted))
      {
        while (sqlDataReader.Read())
          definitionInfoList.Add(new DefinitionInfo()
          {
            DefinitionInfoId = (int) sqlDataReader["DefinitionInfoId"],
            DataspaceId = (int) sqlDataReader["DataspaceId"],
            DefinitionType = (int) (byte) sqlDataReader["DefinitionType"],
            DefinitionId = (int) sqlDataReader["DefinitionId"]
          });
      }
      return definitionInfoList;
    }

    public int DeleteDefinitionInfo(
      int dataspaceId,
      int definitionInfoId,
      int resumeStage,
      int deletionBatchSize)
    {
      this.PrepareStoredProcedure(TestImpactServiceDatabaseBase.GetDatabaseObjectName("prc_DeleteDefinitionInfo"));
      this.BindInt("@dataspaceId", dataspaceId);
      this.BindInt("@definitionInfoId", definitionInfoId);
      this.BindInt("@resumeStage", resumeStage);
      this.BindInt("@deletionBatchSize", deletionBatchSize);
      int num = 0;
      using (SqlDataReader sqlDataReader = this.ExecuteReader())
      {
        while (sqlDataReader.Read())
          num = (int) sqlDataReader["ResumeStage"];
      }
      return num;
    }

    public void QueueDeleteDefinitionInfo(Guid projectId, int definitionType, int definitionId)
    {
      this.PrepareStoredProcedure(TestImpactServiceDatabaseBase.GetDatabaseObjectName("prc_QueueDeleteDefinitionInfo"));
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "TestManagement"));
      this.BindByte("@definitionType", (byte) definitionType);
      this.BindInt("@definitionId", definitionId);
      this.ExecuteNonQuery();
    }
  }
}
