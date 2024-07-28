// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services.DeploymentResourceService
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services
{
  public class DeploymentResourceService : IVssFrameworkService
  {
    public virtual DeploymentResource AddDeploymentResource(
      IVssRequestContext requestContext,
      Guid projectId,
      string resourceIdentifier,
      int releaseDefinitionId,
      int definitionEnvironmentId)
    {
      using (ReleaseManagementTimer.Create(requestContext, "DataAccessLayer", "DeploymentTrackingController.AddDeloymentResource", 1976416))
      {
        Func<DeploymentResourceComponent, DeploymentResource> action = (Func<DeploymentResourceComponent, DeploymentResource>) (component => component.AddDeploymentResource(projectId, resourceIdentifier, releaseDefinitionId, definitionEnvironmentId));
        return requestContext.ExecuteWithinUsingWithComponent<DeploymentResourceComponent, DeploymentResource>(action);
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These are optional parameters")]
    public virtual IList<DeploymentResource> GetDeploymentResources(
      IVssRequestContext requestContext,
      Guid projectId,
      int deploymentResourceId,
      string resourceIdentifier,
      int releaseDefinitionId,
      int maxDeploymentResourcesCount,
      int continuationToken = 0)
    {
      using (ReleaseManagementTimer.Create(requestContext, "DataAccessLayer", "DeploymentTrackingController.GetDeploymentResource", 1976417))
      {
        Func<DeploymentResourceComponent, IList<DeploymentResource>> action = (Func<DeploymentResourceComponent, IList<DeploymentResource>>) (component => component.GetDeploymentResources(projectId, deploymentResourceId, resourceIdentifier, releaseDefinitionId, maxDeploymentResourcesCount, continuationToken));
        return requestContext.ExecuteWithinUsingWithComponent<DeploymentResourceComponent, IList<DeploymentResource>>(action);
      }
    }

    public virtual DeploymentResource UpdateDeploymentResource(
      IVssRequestContext requestContext,
      Guid projectId,
      int deploymentResourceId,
      int releaseDefinitionId,
      int definitionEnvironmentId)
    {
      using (ReleaseManagementTimer.Create(requestContext, "DataAccessLayer", "DeploymentTrackingController.UpdateDeploymentResource", 1976418))
      {
        Func<DeploymentResourceComponent, DeploymentResource> action = (Func<DeploymentResourceComponent, DeploymentResource>) (component => component.UpdateDeploymentResource(projectId, deploymentResourceId, releaseDefinitionId, definitionEnvironmentId));
        return requestContext.ExecuteWithinUsingWithComponent<DeploymentResourceComponent, DeploymentResource>(action);
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These are optional parameters")]
    public virtual void DeleteDeploymentResource(
      IVssRequestContext requestContext,
      Guid projectId,
      int deploymentResourceId = 0,
      string resourceIdentifier = null,
      int releaseDefinitionId = 0)
    {
      using (ReleaseManagementTimer.Create(requestContext, "DataAccessLayer", "DeploymentTrackingController.DeleteDeploymentResource", 1976419))
      {
        Action<DeploymentResourceComponent> action = (Action<DeploymentResourceComponent>) (component => component.DeleteDeploymentResource(projectId, deploymentResourceId, resourceIdentifier, releaseDefinitionId));
        requestContext.ExecuteWithinUsingWithComponent<DeploymentResourceComponent>(action);
      }
    }

    public virtual void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public virtual void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }
  }
}
