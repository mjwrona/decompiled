// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestImpact.Server.Common.DefinitionRunDatabase
// Assembly: Microsoft.TeamFoundation.TestImpact.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 079E4AEE-0642-4BDD-8B76-CECF38EBB798
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestImpact.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestImpact.WebApi.Contracts;
using System;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.TestImpact.Server.Common
{
  [CLSCompliant(false)]
  public class DefinitionRunDatabase : TestImpactServiceDatabaseBase, IDefinitionRunDatabaseComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[3]
    {
      (IComponentCreator) new ComponentCreator<DefinitionRunDatabase>(1),
      (IComponentCreator) new ComponentCreator<DefinitionRunDatabase>(2),
      (IComponentCreator) new ComponentCreator<DefinitionRunDatabase>(3)
    }, "TestImpactService");

    public DefinitionRunDatabase()
    {
    }

    internal DefinitionRunDatabase(string connectionString, IVssRequestContext requestContext)
      : base(connectionString, requestContext)
    {
    }

    public virtual void QueueDeleteDefinitionRun(
      Guid projectId,
      int definitionType,
      int definitionId,
      int runId)
    {
      this.PrepareStoredProcedure(TestImpactServiceDatabaseBase.GetDatabaseObjectName("prc_QueueDeleteDefinitionRun"));
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "TestManagement"));
      this.BindByte("@definitionType", (byte) definitionType);
      this.BindInt("@definitionId", definitionId);
      this.BindInt("@runId", runId);
      this.ExecuteNonQuery();
    }

    public virtual BuildType QueryIfRebaseRun(
      Guid projectId,
      int definitionType,
      int definitionId,
      int runId)
    {
      this.PrepareStoredProcedure(TestImpactServiceDatabaseBase.GetDatabaseObjectName("prc_QueryIfRebaseRun"));
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "TestManagement"));
      this.BindByte("@definitionType", (byte) definitionType);
      this.BindInt("@definitionId", definitionId);
      this.BindInt("@runId", runId);
      using (SqlDataReader sqlDataReader = this.ExecuteReader())
      {
        sqlDataReader.Read();
        if (!sqlDataReader.HasRows)
          return BuildType.TestImpactOff;
        return (bool) sqlDataReader["IsRebase"] ? BuildType.BaseLine : BuildType.TestImpactOn;
      }
    }

    public virtual void DeleteDefinitionRun(int deletionBatchSize = 0)
    {
      this.PrepareStoredProcedure(TestImpactServiceDatabaseBase.GetDatabaseObjectName("prc_DeleteDefinitionRun"));
      if (deletionBatchSize > 0)
        this.BindInt("@deletionBatchSize", deletionBatchSize);
      this.ExecuteNonQuery();
    }
  }
}
