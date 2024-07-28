// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts.Services.WorkItemExternalArtifactLinkService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts.Components;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts.Services
{
  public class WorkItemExternalArtifactLinkService : 
    BaseTeamFoundationWorkItemTrackingService,
    IWorkItemExternalArtifactLinkService,
    IVssFrameworkService
  {
    public IEnumerable<ExternalDeployment> GetDeploymentArtifactLinks(
      IVssRequestContext requestContext,
      int workitemId)
    {
      using (ExternalArtifactSqlComponent component = requestContext.CreateComponent<ExternalArtifactSqlComponent>())
        return (IEnumerable<ExternalDeployment>) component.GetDeploymentArtifactLinks(workitemId).Select<ExternalDeploymentDataset, ExternalDeployment>((Func<ExternalDeploymentDataset, ExternalDeployment>) (dataset => ExternalDeploymentUtilities.ConvertToServerExternalDeployment(dataset))).ToList<ExternalDeployment>();
    }

    public Guid CreateDeploymentArtifactLinks(
      IVssRequestContext requestContext,
      IEnumerable<int> workitemIds,
      ExternalDeployment deployment)
    {
      using (ExternalArtifactSqlComponent component = requestContext.CreateComponent<ExternalArtifactSqlComponent>())
      {
        ExternalDeploymentDataset deploymentDataset = ExternalDeploymentUtilities.ConvertToExternalDeploymentDataset(deployment);
        return component.CreateDeploymentArtifactLinks(workitemIds, deploymentDataset);
      }
    }
  }
}
