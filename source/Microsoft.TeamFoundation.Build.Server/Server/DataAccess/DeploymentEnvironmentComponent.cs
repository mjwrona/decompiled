// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.DeploymentEnvironmentComponent
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal class DeploymentEnvironmentComponent : BuildSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[4]
    {
      (IComponentCreator) new ComponentCreator<DeploymentEnvironmentComponent>(2),
      (IComponentCreator) new ComponentCreator<DeploymentEnvironmentComponent2>(3),
      (IComponentCreator) new ComponentCreator<DeploymentEnvironmentComponent3>(4),
      (IComponentCreator) new ComponentCreator<DeploymentEnvironmentComponent4>(5)
    }, "BuildDeploymentEnvironment", "Build");

    public DeploymentEnvironmentComponent() => this.ServiceVersion = ServiceVersion.V2;

    internal virtual void AddDeploymentEnvironment(
      DeploymentEnvironmentCreationData deploymentEnvironmentCreationData,
      Guid projectId)
    {
      this.PrepareStoredProcedure("prc_AddDeploymentEnvironment");
      this.BindString("@name", deploymentEnvironmentCreationData.EnvironmentMetadata.Name, 256, false, SqlDbType.NVarChar);
      this.BindGuid("@projectId", projectId);
      this.BindString("@connectedServiceName", deploymentEnvironmentCreationData.EnvironmentMetadata.ConnectedServiceName, 256, false, SqlDbType.NVarChar);
      this.BindByte("@kind", (byte) deploymentEnvironmentCreationData.EnvironmentMetadata.Kind);
      this.BindString("@friendlyName", deploymentEnvironmentCreationData.EnvironmentMetadata.FriendlyName, 512, true, SqlDbType.NVarChar);
      this.BindString("@description", deploymentEnvironmentCreationData.EnvironmentMetadata.Description, int.MaxValue, true, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    internal virtual List<DeploymentEnvironmentMetadata> GetDeploymentEnvironments(
      Guid projectId,
      string teamProject,
      string serviceName = "")
    {
      this.PrepareStoredProcedure("prc_GetDeploymentEnvironments");
      this.BindGuid("@projectId", projectId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<DeploymentEnvironmentMetadata>((ObjectBinder<DeploymentEnvironmentMetadata>) new DeploymentEnvironmentComponent.EnvironmentMetadataBinder(teamProject));
        List<DeploymentEnvironmentMetadata> items = resultCollection.GetCurrent<DeploymentEnvironmentMetadata>().Items;
        return items != null && items.Count > 0 ? items : new List<DeploymentEnvironmentMetadata>();
      }
    }

    internal virtual DeploymentEnvironmentMetadata GetDeploymentEnvironment(
      string name,
      Guid projectId,
      string teamProject)
    {
      this.PrepareStoredProcedure("prc_GetDeploymentEnvironment");
      this.BindString("@name", name, 256, false, SqlDbType.NVarChar);
      this.BindGuid("@projectId", projectId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<DeploymentEnvironmentMetadata>((ObjectBinder<DeploymentEnvironmentMetadata>) new DeploymentEnvironmentComponent.EnvironmentMetadataBinder(teamProject));
        List<DeploymentEnvironmentMetadata> items = resultCollection.GetCurrent<DeploymentEnvironmentMetadata>().Items;
        return items != null && items.Count > 0 ? items[0] : (DeploymentEnvironmentMetadata) null;
      }
    }

    internal virtual void DeleteDeploymentEnvironment(string name, Guid projectId)
    {
      this.PrepareStoredProcedure("prc_DeleteDeploymentEnvironment");
      this.BindString("@name", name, 256, false, SqlDbType.NVarChar);
      this.BindGuid("@projectId", projectId);
      this.ExecuteNonQuery();
    }

    internal class EnvironmentMetadataBinder : ObjectBinder<DeploymentEnvironmentMetadata>
    {
      private SqlColumnBinder NameColumn = new SqlColumnBinder("Name");
      private SqlColumnBinder ConnectedServiceNameColumn = new SqlColumnBinder("ConnectedServiceName");
      private SqlColumnBinder KindColumn = new SqlColumnBinder("Kind");
      private SqlColumnBinder FriendlyNameColumn = new SqlColumnBinder("FriendlyName");
      private SqlColumnBinder DescriptionColumn = new SqlColumnBinder("Description");
      private string m_teamProject;

      internal EnvironmentMetadataBinder(string teamProject) => this.m_teamProject = teamProject;

      protected override DeploymentEnvironmentMetadata Bind() => new DeploymentEnvironmentMetadata(this.NameColumn.GetString((IDataReader) this.Reader, false), this.m_teamProject, this.ConnectedServiceNameColumn.GetString((IDataReader) this.Reader, false), (DeploymentEnvironmentKind) this.KindColumn.GetByte((IDataReader) this.Reader), this.FriendlyNameColumn.GetString((IDataReader) this.Reader, true), this.DescriptionColumn.GetString((IDataReader) this.Reader, true));
    }
  }
}
