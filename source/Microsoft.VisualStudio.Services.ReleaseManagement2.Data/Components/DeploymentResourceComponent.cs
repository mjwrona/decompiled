// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.DeploymentResourceComponent
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  public class DeploymentResourceComponent : ReleaseManagementSqlResourceComponentBase
  {
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "This signature is required by VSSF. Don't change.")]
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[2]
    {
      (IComponentCreator) new ComponentCreator<DeploymentResourceComponent>(1),
      (IComponentCreator) new ComponentCreator<DeploymentResourceComponent2>(2)
    }, "DeploymentTrackingResource", "ReleaseManagement");

    public virtual DeploymentResource AddDeploymentResource(
      Guid projectId,
      string resourceIdentifier,
      int releaseDefinitionId,
      int definitionEnvironmentId)
    {
      this.PrepareStoredProcedure("Release.prc_AddDeploymentResource", projectId);
      this.BindString(nameof (resourceIdentifier), resourceIdentifier, 450, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindInt(nameof (releaseDefinitionId), releaseDefinitionId);
      this.BindInt(nameof (definitionEnvironmentId), definitionEnvironmentId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<DeploymentResource>((ObjectBinder<DeploymentResource>) this.GetDeploymentResourceBinder());
        return resultCollection.GetCurrent<DeploymentResource>().Items.FirstOrDefault<DeploymentResource>();
      }
    }

    public virtual DeploymentResource UpdateDeploymentResource(
      Guid projectId,
      int id,
      int releaseDefinitionId,
      int definitionEnvironmentId)
    {
      this.PrepareStoredProcedure("Release.prc_UpdateDeploymentResource", projectId);
      this.BindInt(nameof (id), id);
      this.BindInt(nameof (releaseDefinitionId), releaseDefinitionId);
      this.BindInt(nameof (definitionEnvironmentId), definitionEnvironmentId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<DeploymentResource>((ObjectBinder<DeploymentResource>) this.GetDeploymentResourceBinder());
        return resultCollection.GetCurrent<DeploymentResource>().Items.FirstOrDefault<DeploymentResource>();
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These are optional parameters")]
    public virtual IList<DeploymentResource> GetDeploymentResources(
      Guid projectId,
      int id = 0,
      string resourceIdentifier = null,
      int releaseDefinitionId = 0,
      int maxDeploymentResourcesCount = 0,
      int continuationToken = 0)
    {
      this.PrepareStoredProcedure("Release.prc_GetDeploymentResources", projectId);
      this.BindInt(nameof (id), id);
      this.BindString(nameof (resourceIdentifier), resourceIdentifier, 450, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindInt(nameof (releaseDefinitionId), releaseDefinitionId);
      this.BindInt("@maxDeploymentResourcesCount", maxDeploymentResourcesCount);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<DeploymentResource>((ObjectBinder<DeploymentResource>) this.GetDeploymentResourceBinder());
        return (IList<DeploymentResource>) resultCollection.GetCurrent<DeploymentResource>().Items;
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These are optional parameters")]
    public virtual void DeleteDeploymentResource(
      Guid projectId,
      int id = 0,
      string resourceIdentifier = null,
      int releaseDefinitionId = 0)
    {
      this.PrepareStoredProcedure("Release.prc_DeleteDeploymentResources", projectId);
      this.BindInt(nameof (id), id);
      this.BindString(nameof (resourceIdentifier), resourceIdentifier, 450, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindInt(nameof (releaseDefinitionId), releaseDefinitionId);
      this.ExecuteNonQuery();
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "It is a virtual function")]
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Override of base class method.")]
    protected virtual DeploymentResourceBinder GetDeploymentResourceBinder() => new DeploymentResourceBinder();
  }
}
