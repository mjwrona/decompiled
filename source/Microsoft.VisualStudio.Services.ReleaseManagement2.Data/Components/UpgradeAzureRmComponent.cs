// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.UpgradeAzureRmComponent
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  public class UpgradeAzureRmComponent : ReleaseManagementSqlResourceComponentBase
  {
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "This signature is required by VSSF. Don't change.")]
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<UpgradeAzureRmComponent>(1)
    }, "UpgradeAzureRmTask", "ReleaseManagement");

    public IList<int> GetReleaseIdsWithAzureRmTask(int dataspaceId, Guid taskId)
    {
      string sqlStatement = "\r\n            SET NOCOUNT ON\r\n            DECLARE @sql       NVARCHAR(MAX)\r\n            DECLARE @param     NVARCHAR(260)\r\n            DECLARE @taskId    NVARCHAR(128)\r\n            \r\n            SET @taskId = @azureRmtaskId\r\n            SET @param = '@partitionId INT, @dataspaceId INT'\r\n            SET @sql = \r\n                'SELECT DISTINCT TOP 1000 re.[ReleaseId] FROM Release.tbl_ReleaseEnvironment re WITH (NOLOCK)\r\n                    INNER JOIN Release.tbl_Release r \r\n                    ON re.PartitionId = @partitionId\r\n                    AND re.DataspaceId = @dataspaceId\r\n                    AND re.ReleaseId = r.Id\r\n                    WHERE re.DeployPhaseSnapshots LIKE ''%' + @taskId + '%''' +\r\n                    'AND re.DeployPhaseSnapshots NOT LIKE ''%UseWebDeploy%''\r\n                    AND r.PartitionId = @partitionId\r\n                    AND r.DataspaceId = @dataspaceId\r\n                    AND r.IsDeleted = 0\r\n                    AND DATEDIFF(day, r.CreatedOn, GETDATE()) <= 60 \r\n                    OPTION (OPTIMIZE FOR(@partitionId UNKNOWN, @dataspaceId UNKNOWN))'\r\n            EXEC sp_executesql @sql, @param, @partitionId = @partitionId, @dataspaceId = @dataspaceId";
      this.PrepareSqlBatch(sqlStatement.Length, true);
      this.AddStatement(sqlStatement);
      this.BindInt("@dataspaceId", dataspaceId);
      this.BindString("@azureRmtaskId", taskId.ToString(), 128, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      SqlDataReader sqlDataReader = this.ExecuteReader();
      List<int> idsWithAzureRmTask = new List<int>();
      while (sqlDataReader.Read())
        idsWithAzureRmTask.Add(sqlDataReader.GetInt32(0));
      return (IList<int>) idsWithAzureRmTask;
    }

    public IList<int> GetReleaseDefinitionIdsWithAzureRmTask(int dataspaceId, Guid taskId)
    {
      string sqlStatement = "\r\n            SET NOCOUNT ON\r\n            DECLARE @sql       NVARCHAR(MAX)\r\n            DECLARE @param     NVARCHAR(260)\r\n            DECLARE @taskId    NVARCHAR(128)\r\n\r\n            SET @taskId = @azureRmtaskId\r\n            SET @param = '@partitionId INT, @dataspaceId INT'\r\n            SET @sql = \r\n                'SELECT DISTINCT TOP 1000 [DefinitionId] FROM\r\n                    Release.tbl_DefinitionEnvironmentDeployPhase WITH (NOLOCK)\r\n                    WHERE Workflow LIKE ''%' + @taskId + '%''' +\r\n                    'AND Workflow NOT LIKE ''%UseWebDeploy%''\r\n                    AND PartitionId = @partitionId\r\n                    AND DataspaceId = @dataspaceId\r\n                    OPTION (OPTIMIZE FOR(@partitionId UNKNOWN, @dataspaceId UNKNOWN))'\r\n            EXEC sp_executesql @sql, @param, @partitionId = @partitionId, @dataspaceId = @dataspaceId";
      this.PrepareSqlBatch(sqlStatement.Length, true);
      this.AddStatement(sqlStatement);
      this.BindInt("@dataspaceId", dataspaceId);
      this.BindString("@azureRmtaskId", taskId.ToString(), 128, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      SqlDataReader sqlDataReader = this.ExecuteReader();
      List<int> idsWithAzureRmTask = new List<int>();
      while (sqlDataReader.Read())
        idsWithAzureRmTask.Add(sqlDataReader.GetInt32(0));
      return (IList<int>) idsWithAzureRmTask;
    }

    [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", Justification = "We are checking serialization perf")]
    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "It is a validated else where")]
    public void UpdateReleaseEnvironment(ReleaseEnvironment releaseEnvironment, int dataspaceId)
    {
      string parameterValue = JsonConvert.SerializeObject((object) releaseEnvironment.DeploymentSnapshot);
      this.PrepareStoredProcedure("Release.prc_UpgradeAzureRmTask");
      this.BindInt("@dataspaceId", dataspaceId);
      this.BindInt("@releaseId", releaseEnvironment.ReleaseId);
      this.BindInt("@id", releaseEnvironment.Id);
      this.BindString("@deployPhaseSnapshots", parameterValue, -1, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }
  }
}
