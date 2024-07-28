// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers.TaskOrchestrationHelper
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C3F75541-7C8A-4AF6-A47E-709CEEE7550D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Common.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers
{
  public static class TaskOrchestrationHelper
  {
    public static TaskOrchestrationOwner GetOwnerReference(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId,
      string releaseName,
      int environmentId,
      string environmentName)
    {
      TaskOrchestrationOwner ownerReference = new TaskOrchestrationOwner();
      ownerReference.Id = environmentId;
      ownerReference.Name = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} / {1}", (object) releaseName, (object) environmentName);
      ownerReference.Links.AddLink("web", WebAccessUrlBuilder.GetReleaseWebAccessUri(requestContext, projectId.ToString(), releaseId));
      ownerReference.Links.AddLink("self", WebAccessUrlBuilder.GetReleaseRestUrl(requestContext, projectId, releaseId));
      return ownerReference;
    }

    public static TaskOrchestrationOwner GetDefinitionReference(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      string releaseDefinitionName)
    {
      TaskOrchestrationOwner definitionReference = new TaskOrchestrationOwner();
      definitionReference.Id = definitionId;
      definitionReference.Name = releaseDefinitionName;
      definitionReference.Links.AddLink("web", WebAccessUrlBuilder.GetReleaseDefinitionWebAccessUri(requestContext, projectId.ToString(), definitionId));
      definitionReference.Links.AddLink("self", WebAccessUrlBuilder.GetReleaseDefinitionRestUrl(requestContext, projectId, definitionId));
      return definitionReference;
    }

    public static ServiceEndpointExecutionOwner GetServiceEndpointExecutionOwnerReference(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId,
      string releaseName,
      int environmentId)
    {
      ServiceEndpointExecutionOwner executionOwnerReference = new ServiceEndpointExecutionOwner();
      executionOwnerReference.Id = environmentId;
      executionOwnerReference.Name = releaseName;
      executionOwnerReference.Links.AddLink("web", WebAccessUrlBuilder.GetReleaseWebAccessUri(requestContext, projectId.ToString(), releaseId));
      executionOwnerReference.Links.AddLink("self", WebAccessUrlBuilder.GetReleaseRestUrl(requestContext, projectId, releaseId));
      return executionOwnerReference;
    }

    public static ServiceEndpointExecutionOwner GetServiceEndpointDefinitionReference(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      string releaseDefinitionName)
    {
      ServiceEndpointExecutionOwner definitionReference = new ServiceEndpointExecutionOwner();
      definitionReference.Id = definitionId;
      definitionReference.Name = releaseDefinitionName;
      definitionReference.Links.AddLink("web", WebAccessUrlBuilder.GetReleaseDefinitionWebAccessUri(requestContext, projectId.ToString(), definitionId));
      definitionReference.Links.AddLink("self", WebAccessUrlBuilder.GetReleaseDefinitionRestUrl(requestContext, projectId, definitionId));
      return definitionReference;
    }
  }
}
