// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.Services.BaseMetadataDetailsService
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.Azure.Pipelines.Deployment.Services
{
  internal abstract class BaseMetadataDetailsService : 
    IBaseMetadataDetailsService,
    IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public abstract void AddMetadataDetails(
      IVssRequestContext requestContext,
      Guid projectId,
      BaseMetadataDetails metadataDetails);

    public void CheckProjectPermission(
      IVssRequestContext requestContext,
      Guid scopeId,
      int permission)
    {
      ArgumentUtility.CheckForEmptyGuid(scopeId, nameof (scopeId));
      string projectUri = ProjectInfo.GetProjectUri(scopeId);
      requestContext.GetService<IProjectService>().CheckProjectPermission(requestContext, projectUri, permission);
    }
  }
}
