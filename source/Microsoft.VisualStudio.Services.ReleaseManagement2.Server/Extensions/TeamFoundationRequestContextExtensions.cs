// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions.TeamFoundationRequestContextExtensions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions
{
  public static class TeamFoundationRequestContextExtensions
  {
    public static string GetRequestingUserDisplayName(this IVssRequestContext requestContext)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity = requestContext != null ? requestContext.GetUserIdentity() : throw new ArgumentNullException(nameof (requestContext));
      return identity != null ? identity.DisplayName : requestContext.DomainUserName;
    }

    public static string GetProjectName(this IVssRequestContext requestContext, Guid projectId)
    {
      IVssRequestContext requestContext1 = requestContext != null ? requestContext.Elevate() : throw new ArgumentNullException(nameof (requestContext));
      return requestContext.GetService<IProjectService>().GetProject(requestContext1, projectId).Name;
    }

    public static Dictionary<string, object> GetResourceContainers(
      this IVssRequestContext requestContext)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      Dictionary<string, object> resourceContainers = new Dictionary<string, object>();
      IVssServiceHost collectionServiceHost = requestContext.ServiceHost.CollectionServiceHost;
      Guid guid = collectionServiceHost != null ? collectionServiceHost.InstanceId : Guid.Empty;
      IVssServiceHost organizationServiceHost = requestContext.ServiceHost.OrganizationServiceHost;
      resourceContainers["Account"] = (object) (organizationServiceHost == null || !organizationServiceHost.IsOnly(TeamFoundationHostType.Application) ? Guid.Empty : organizationServiceHost.InstanceId);
      resourceContainers["Collection"] = (object) guid;
      return resourceContainers;
    }
  }
}
