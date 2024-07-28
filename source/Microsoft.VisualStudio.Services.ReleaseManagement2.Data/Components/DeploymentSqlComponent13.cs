// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.DeploymentSqlComponent13
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Versioning mechanism")]
  public class DeploymentSqlComponent13 : DeploymentSqlComponent12
  {
    public override IEnumerable<Deployment> GetLastDeploymentForReleaseDefinitions(
      Guid projectId,
      IEnumerable<int> releaseDefinitionIds)
    {
      this.PrepareStoredProcedure("Release.prc_GetLastDeploymentForReleaseDefinitions", projectId);
      this.BindInt32Table("definitionIds", releaseDefinitionIds);
      return this.GetDeployments();
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "required")]
    public override DeploymentsAndDefinitions GetDeploymentsAndDefinitionsAcrossProjects(
      Guid identityId,
      HashSet<KeyValuePair<int, int>> dataspaceDefinitionIdKeyValuePairs,
      int maxItems = 30)
    {
      this.PrepareStoredProcedure("Release.prc_QueryDeploymentsAndDefinitionsAcrossProjects");
      this.BindGuid("requestedBy", identityId);
      this.BindInt(nameof (maxItems), maxItems);
      this.BindKeyValuePairInt32Int32Table("favoriteDefinitions", (IEnumerable<KeyValuePair<int, int>>) dataspaceDefinitionIdKeyValuePairs);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        IEnumerable<Deployment> deployments = this.GetDeployments(resultCollection, true, true);
        resultCollection.AddBinder<ReleaseDefinitionShallowReference>((ObjectBinder<ReleaseDefinitionShallowReference>) this.GetReleaseDefinitionShallowReferenceBinder());
        resultCollection.NextResult();
        List<ReleaseDefinitionShallowReference> items = resultCollection.GetCurrent<ReleaseDefinitionShallowReference>().Items;
        DeploymentsAndDefinitions definitionsAcrossProjects = new DeploymentsAndDefinitions();
        definitionsAcrossProjects.Deployments.AddRange<Deployment, IList<Deployment>>(deployments);
        definitionsAcrossProjects.Definitions.AddRange<ReleaseDefinitionShallowReference, IList<ReleaseDefinitionShallowReference>>((IEnumerable<ReleaseDefinitionShallowReference>) items);
        return definitionsAcrossProjects;
      }
    }

    public override IEnumerable<Deployment> GetDeploymentsCreatedByAnIdentity(
      Guid projectId,
      Guid createdById,
      DeploymentStatus deploymentStatus,
      DeploymentOperationStatus operationStatus,
      int maxDeployments,
      DateTime minQueuedTime,
      DateTime maxQueuedTime)
    {
      this.PrepareStoredProcedure("Release.prc_GetDeploymentsCreatedByAnIdentity", projectId);
      this.BindGuid(nameof (createdById), createdById);
      this.BindInt(nameof (deploymentStatus), (int) deploymentStatus);
      this.BindInt(nameof (operationStatus), (int) operationStatus);
      this.BindInt(nameof (maxDeployments), maxDeployments);
      this.BindDateTime(nameof (minQueuedTime), minQueuedTime);
      this.BindDateTime(nameof (maxQueuedTime), maxQueuedTime);
      return this.GetDeployments(false, false);
    }

    private ReleaseDefinitionShallowReferenceBinder GetReleaseDefinitionShallowReferenceBinder() => new ReleaseDefinitionShallowReferenceBinder((ReleaseManagementSqlResourceComponentBase) this);
  }
}
