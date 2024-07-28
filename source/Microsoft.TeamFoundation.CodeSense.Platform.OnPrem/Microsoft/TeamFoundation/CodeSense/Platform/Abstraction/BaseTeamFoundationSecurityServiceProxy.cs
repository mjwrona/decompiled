// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Platform.Abstraction.BaseTeamFoundationSecurityServiceProxy
// Assembly: Microsoft.TeamFoundation.CodeSense.Platform.OnPrem, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B5DEEFA-3C5E-4BFB-92E2-3ADDA47952C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Platform.OnPrem.dll

using Microsoft.TeamFoundation.CodeSense.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.CodeSense.Platform.Abstraction
{
  public abstract class BaseTeamFoundationSecurityServiceProxy : 
    ITeamFoundationSecurityServiceProxy,
    IVssFrameworkService
  {
    public void CheckPermissions(
      IVssRequestContext requestContext,
      Guid securityNamespaceGuid,
      string token,
      int permissions)
    {
      using (new CodeSenseTraceWatch(requestContext, 1024120, true, TraceLayer.ExternalFramework, "Checking permissions for '{0}' in namespace {1}", new object[2]
      {
        (object) token,
        (object) securityNamespaceGuid
      }))
      {
        bool isLegacySecurityNamespace = false;
        IVssSecurityNamespace securityNamespace = BaseTeamFoundationSecurityServiceProxy.GetSecurityNamespace(requestContext, securityNamespaceGuid, out isLegacySecurityNamespace);
        if (securityNamespace == null)
          throw new InvalidSecurityNamespaceException(securityNamespaceGuid);
        if (isLegacySecurityNamespace)
          token = token.ReplaceProjectGuidWithName(requestContext, (ProjectMapCache) null);
        securityNamespace.CheckPermission(requestContext, token, permissions, false);
      }
    }

    public abstract IDictionary<int, bool> GetWorkItemPermissions(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds);

    public abstract bool HasPermissions(
      IVssRequestContext requestContext,
      Guid securityNamespaceGuid,
      string token,
      int permissions);

    public Dictionary<string, bool> ObtainPermissions(
      IVssRequestContext requestContext,
      Guid securityNamespaceGuid,
      IEnumerable<string> tokens,
      int permissions)
    {
      Dictionary<string, bool> permissions1 = new Dictionary<string, bool>();
      bool isLegacySecurityNamespace = false;
      IVssSecurityNamespace securityNamespace = BaseTeamFoundationSecurityServiceProxy.GetSecurityNamespace(requestContext, securityNamespaceGuid, out isLegacySecurityNamespace);
      if (securityNamespace == null)
        throw new InvalidSecurityNamespaceException(securityNamespaceGuid);
      if (isLegacySecurityNamespace)
        tokens = tokens.Select<string, string>((Func<string, string>) (s => s.ReplaceProjectGuidWithName(requestContext, (ProjectMapCache) null)));
      foreach (string token in tokens)
      {
        using (new CodeSenseTraceWatch(requestContext, 1024135, TraceLayer.ExternalFramework, "Obtaining permissions for '{0}'", new object[1]
        {
          (object) token
        }))
        {
          bool flag = securityNamespace.HasPermission(requestContext, token, permissions, false);
          if (isLegacySecurityNamespace)
            permissions1[token.ReplaceProjectNameWithGuid(requestContext, (ProjectMapCache) null)] = flag;
          else
            permissions1[token] = flag;
        }
      }
      return permissions1;
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    protected static IVssSecurityNamespace GetSecurityNamespace(
      IVssRequestContext requestContext,
      Guid securityNamespaceGuid,
      out bool isLegacySecurityNamespace)
    {
      ITeamFoundationSecurityService service = requestContext.GetService<ITeamFoundationSecurityService>();
      IVssSecurityNamespace securityNamespace = service.GetSecurityNamespace(requestContext, securityNamespaceGuid);
      if (securityNamespace == null && securityNamespaceGuid.Equals(SecurityConstants.RepositorySecurity2NamespaceGuid))
      {
        securityNamespace = service.GetSecurityNamespace(requestContext, SecurityConstants.RepositorySecurityNamespaceGuid);
        isLegacySecurityNamespace = true;
      }
      else
        isLegacySecurityNamespace = false;
      return securityNamespace;
    }
  }
}
