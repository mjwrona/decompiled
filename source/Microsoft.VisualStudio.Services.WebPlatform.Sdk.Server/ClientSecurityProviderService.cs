// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.ClientSecurityProviderService
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server
{
  internal class ClientSecurityProviderService : IClientSecurityProviderService, IVssFrameworkService
  {
    private const string c_permissionsSharedDataKey = "_permissions";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void AddPermissions(
      IVssRequestContext requestContext,
      DataProviderSharedData sharedData,
      Guid namespaceId,
      string token)
    {
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, namespaceId);
      if (securityNamespace == null)
        throw new InvalidSecurityNamespaceException(namespaceId);
      IEnumerable<EvaluationPrincipal> evaluationPrincipals = requestContext.GetUserEvaluationPrincipals();
      int permissions = securityNamespace.QueryEffectivePermissions(requestContext, token, evaluationPrincipals);
      object obj;
      SharedPermissionCollection permissionCollection;
      if (sharedData.TryGetValue("_permissions", out obj) && obj is SharedPermissionCollection)
      {
        permissionCollection = obj as SharedPermissionCollection;
      }
      else
      {
        permissionCollection = new SharedPermissionCollection();
        sharedData.Add("_permissions", (object) permissionCollection);
      }
      permissionCollection.AddPermissions(namespaceId, token, permissions);
    }
  }
}
