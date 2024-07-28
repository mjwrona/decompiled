// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Platform.Abstraction.TeamFoundationSecurityServiceProxy
// Assembly: Microsoft.TeamFoundation.CodeSense.Platform.Abstraction, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7627AC5C-7AFD-416A-A79B-D03A392C9E3B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Platform.Abstraction.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.TeamFoundation.CodeSense.Platform.Abstraction
{
  public class TeamFoundationSecurityServiceProxy : 
    IVssFrameworkService,
    ITeamFoundationSecurityServiceProxy
  {
    private ExportLifetimeContext<ITeamFoundationSecurityServiceProxy> tfsSecurityServiceProxy;

    public void CheckPermissions(
      IVssRequestContext requestContext,
      Guid securityNamespaceGuid,
      string token,
      int permissions)
    {
      this.tfsSecurityServiceProxy.Value.CheckPermissions(requestContext, securityNamespaceGuid, token, permissions);
    }

    public IDictionary<int, bool> GetWorkItemPermissions(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds)
    {
      return this.tfsSecurityServiceProxy.Value.GetWorkItemPermissions(requestContext, workItemIds);
    }

    public bool HasPermissions(
      IVssRequestContext requestContext,
      Guid securityNamespaceGuid,
      string token,
      int permissions)
    {
      return this.tfsSecurityServiceProxy.Value.HasPermissions(requestContext, securityNamespaceGuid, token, permissions);
    }

    public Dictionary<string, bool> ObtainPermissions(
      IVssRequestContext requestContext,
      Guid securityNamespaceGuid,
      IEnumerable<string> tokens,
      int permissions)
    {
      return this.tfsSecurityServiceProxy.Value.ObtainPermissions(requestContext, securityNamespaceGuid, tokens, permissions);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      if (this.tfsSecurityServiceProxy == null)
        return;
      this.tfsSecurityServiceProxy.Dispose();
      this.tfsSecurityServiceProxy = (ExportLifetimeContext<ITeamFoundationSecurityServiceProxy>) null;
    }

    public void ServiceStart(IVssRequestContext systemRequestContext) => this.tfsSecurityServiceProxy = CodeLensExtension.CreateLifeTimeExport<ITeamFoundationSecurityServiceProxy>(systemRequestContext);
  }
}
