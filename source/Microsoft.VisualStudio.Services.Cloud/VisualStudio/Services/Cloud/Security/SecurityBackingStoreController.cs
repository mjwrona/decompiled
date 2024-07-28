// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.Security.SecurityBackingStoreController
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Security;
using System;
using System.Collections.Generic;
using System.Net;

namespace Microsoft.VisualStudio.Services.Cloud.Security
{
  public abstract class SecurityBackingStoreController : TfsApiController
  {
    private static readonly Dictionary<Type, HttpStatusCode> s_httpExceptions = new Dictionary<Type, HttpStatusCode>()
    {
      {
        typeof (InvalidSecurityNamespaceException),
        HttpStatusCode.NotFound
      },
      {
        typeof (InvalidAclStoreException),
        HttpStatusCode.NotFound
      },
      {
        typeof (InvalidAccessException),
        HttpStatusCode.NotFound
      },
      {
        typeof (InvalidSecurityTokenException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (InvalidOperationException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (ArgumentException),
        HttpStatusCode.BadRequest
      }
    };

    protected LocalSecurityNamespace GetSecurityNamespace(
      Guid securityNamespaceId,
      bool forWriteOperation)
    {
      SecurityBackingStoreController.SbsSecurity.CheckPermission(this.TfsRequestContext, securityNamespaceId, forWriteOperation);
      LocalSecurityNamespace securityNamespace = this.TfsRequestContext.GetService<LocalSecurityService>().GetSecurityNamespace(this.TfsRequestContext, securityNamespaceId);
      if (securityNamespace == null || !securityNamespace.Description.IsRemotable)
        throw new InvalidSecurityNamespaceException(securityNamespaceId);
      return securityNamespace;
    }

    protected CachingAclStore GetAclStore(LocalSecurityNamespace @namespace, Guid aclStoreId)
    {
      CachingAclStore aclStore;
      if (!@namespace.GetRemotableAclStores(this.TfsRequestContext).TryGetValue(aclStoreId, out aclStore))
        throw new InvalidAclStoreException(@namespace.Description.NamespaceId, aclStoreId);
      return aclStore;
    }

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) SecurityBackingStoreController.s_httpExceptions;

    public override string TraceArea => "SecurityBackingStore";

    public override string ActivityLogArea => "Framework";

    protected static class SbsSecurity
    {
      public static readonly Guid NamespaceId = new Guid("32B259FC-926F-411D-82FA-E13864305465");
      public const int ReadPermission = 1;
      public const int WritePermission = 2;

      public static string GetTokenForNamespace(IVssRequestContext requestContext, Guid namespaceId)
      {
        int hostType = (int) requestContext.ServiceHost.HostType;
        ArgumentUtility.CheckForMultipleBits(hostType, "hostType");
        return string.Format("/{0}/{1}", (object) namespaceId, (object) hostType.ToString());
      }

      public static void CheckPermission(
        IVssRequestContext requestContext,
        Guid namespaceId,
        bool forWriteOperation)
      {
        string tokenForNamespace = SecurityBackingStoreController.SbsSecurity.GetTokenForNamespace(requestContext, namespaceId);
        requestContext = requestContext.To(TeamFoundationHostType.Deployment);
        IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, SecurityBackingStoreController.SbsSecurity.NamespaceId);
        if (forWriteOperation)
          securityNamespace.CheckPermission(requestContext, tokenForNamespace, 2, false);
        else
          securityNamespace.CheckPermission(requestContext, tokenForNamespace, 1, false);
      }
    }
  }
}
