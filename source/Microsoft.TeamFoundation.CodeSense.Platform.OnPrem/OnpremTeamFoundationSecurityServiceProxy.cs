// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Platform.OnPrem.OnpremTeamFoundationSecurityServiceProxy
// Assembly: Microsoft.TeamFoundation.CodeSense.Platform.OnPrem, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B5DEEFA-3C5E-4BFB-92E2-3ADDA47952C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Platform.OnPrem.dll

using Microsoft.TeamFoundation.CodeSense.Platform.Abstraction;
using Microsoft.TeamFoundation.CodeSense.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.TeamFoundation.CodeSense.Platform.OnPrem
{
  [Export(typeof (ITeamFoundationSecurityServiceProxy))]
  public class OnpremTeamFoundationSecurityServiceProxy : BaseTeamFoundationSecurityServiceProxy
  {
    public override IDictionary<int, bool> GetWorkItemPermissions(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds)
    {
      return TfWorkItemPermissionsFactory.GetPermissions(requestContext, workItemIds);
    }

    public override bool HasPermissions(
      IVssRequestContext requestContext,
      Guid securityNamespaceGuid,
      string token,
      int permissions)
    {
      using (new CodeSenseTraceWatch(requestContext, 1024125, TraceLayer.ExternalSecurity, "Checking permissions for '{0}' in namespace {1}", new object[2]
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
        return securityNamespace.HasPermission(requestContext, token, permissions, false);
      }
    }
  }
}
