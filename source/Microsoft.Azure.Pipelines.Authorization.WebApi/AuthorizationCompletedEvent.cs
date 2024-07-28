// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Authorization.WebApi.AuthorizationCompletedEvent
// Assembly: Microsoft.Azure.Pipelines.Authorization.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4807FD31-F2A4-4329-AA76-35B262BDA671
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Authorization.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.Authorization.WebApi
{
  [ServiceEventObject]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class AuthorizationCompletedEvent
  {
    public AuthorizationCompletedEvent(
      string eventType,
      Guid projectId,
      ResourcePipelinePermissions pipelinePermissionsForResource)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(eventType, nameof (eventType));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForNull<ResourcePipelinePermissions>(pipelinePermissionsForResource, nameof (pipelinePermissionsForResource));
      this.EventType = eventType;
      this.ProjectId = projectId;
      this.PipelinePermissionsForResource = pipelinePermissionsForResource;
    }

    [DataMember]
    public string EventType { get; set; }

    [DataMember]
    public Guid ProjectId { get; set; }

    [DataMember]
    public ResourcePipelinePermissions PipelinePermissionsForResource { get; set; }
  }
}
