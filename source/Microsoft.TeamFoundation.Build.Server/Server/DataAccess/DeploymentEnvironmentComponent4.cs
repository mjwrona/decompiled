// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.DeploymentEnvironmentComponent4
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal class DeploymentEnvironmentComponent4 : DeploymentEnvironmentComponent3
  {
    public DeploymentEnvironmentComponent4()
    {
      this.ServiceVersion = ServiceVersion.V5;
      this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;
    }

    internal override void AddDeploymentEnvironment(
      DeploymentEnvironmentCreationData deploymentEnvironmentCreationData,
      Guid projectId)
    {
      this.PrepareStoredProcedure("prc_AddDeploymentEnvironment");
      this.BindString("@name", deploymentEnvironmentCreationData.EnvironmentMetadata.Name, 256, false, SqlDbType.NVarChar);
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindString("@connectedServiceName", deploymentEnvironmentCreationData.EnvironmentMetadata.ConnectedServiceName, 256, false, SqlDbType.NVarChar);
      this.BindByte("@kind", (byte) deploymentEnvironmentCreationData.EnvironmentMetadata.Kind);
      this.BindString("@friendlyName", deploymentEnvironmentCreationData.EnvironmentMetadata.FriendlyName, 512, true, SqlDbType.NVarChar);
      this.BindString("@description", deploymentEnvironmentCreationData.EnvironmentMetadata.Description, int.MaxValue, true, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    internal override void DeleteDeploymentEnvironment(string name, Guid projectId)
    {
      this.PrepareStoredProcedure("prc_DeleteDeploymentEnvironment");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindString("@name", name, 256, false, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    internal override DeploymentEnvironmentMetadata GetDeploymentEnvironment(
      string name,
      Guid projectId,
      string teamProject)
    {
      this.PrepareStoredProcedure("prc_GetDeploymentEnvironment");
      this.BindString("@name", name, 256, false, SqlDbType.NVarChar);
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<DeploymentEnvironmentMetadata>((ObjectBinder<DeploymentEnvironmentMetadata>) new DeploymentEnvironmentComponent.EnvironmentMetadataBinder(teamProject));
        List<DeploymentEnvironmentMetadata> items = resultCollection.GetCurrent<DeploymentEnvironmentMetadata>().Items;
        return items != null && items.Count > 0 ? items[0] : (DeploymentEnvironmentMetadata) null;
      }
    }

    internal override List<DeploymentEnvironmentMetadata> GetDeploymentEnvironments(
      Guid projectId,
      string teamProject,
      string serviceName = "")
    {
      this.PrepareStoredProcedure("prc_GetDeploymentEnvironments");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindString("@connectedServiceName", serviceName, 256, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<DeploymentEnvironmentMetadata>((ObjectBinder<DeploymentEnvironmentMetadata>) new DeploymentEnvironmentComponent.EnvironmentMetadataBinder(teamProject));
        List<DeploymentEnvironmentMetadata> items = resultCollection.GetCurrent<DeploymentEnvironmentMetadata>().Items;
        return items != null && items.Count > 0 ? items : new List<DeploymentEnvironmentMetadata>();
      }
    }
  }
}
