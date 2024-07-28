// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Authorization.Server.IAuthorizationEventPublisherService
// Assembly: Microsoft.Azure.Pipelines.Authorization.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 22B31FF9-0E6B-45B0-A4F8-77598802CAB3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Authorization.Server.dll

using Microsoft.Azure.Pipelines.Authorization.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.Azure.Pipelines.Authorization.Server
{
  [DefaultServiceImplementation(typeof (AuthorizationEventPublisherService))]
  public interface IAuthorizationEventPublisherService : IVssFrameworkService
  {
    void OnAuthorizationCompleted(
      IVssRequestContext requestContext,
      Guid projectId,
      ResourcePipelinePermissions resourcePipelinePermission);
  }
}
