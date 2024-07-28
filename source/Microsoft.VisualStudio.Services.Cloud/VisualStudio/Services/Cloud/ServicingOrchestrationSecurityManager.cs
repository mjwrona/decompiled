// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ServicingOrchestrationSecurityManager
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Configuration;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class ServicingOrchestrationSecurityManager
  {
    private static readonly Guid s_securityNamespaceId = new Guid("84cc1aa4-15bc-423d-90d9-f97c450fc729");

    public virtual string Area => "ServicingOrchestration";

    public virtual Guid SecurityNamespaceId => ServicingOrchestrationSecurityManager.s_securityNamespaceId;

    public virtual string SecurityNamespaceTokenAll => "AllRequests";

    public void CheckPermission(
      IVssRequestContext requestContext,
      ServicingOrchestrationPermission permission)
    {
      this.GetSecurityNamespace(requestContext).CheckPermission(requestContext, this.SecurityNamespaceTokenAll, (int) permission);
    }

    public void SetPermissions(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor,
      ServicingOrchestrationPermission allow,
      ServicingOrchestrationPermission deny)
    {
      this.GetSecurityNamespace(requestContext).SetPermissions(requestContext, this.SecurityNamespaceTokenAll, descriptor, (int) allow, (int) deny, true);
    }

    public void RemovePermissions(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor,
      ServicingOrchestrationPermission remove)
    {
      this.GetSecurityNamespace(requestContext).RemovePermissions(requestContext, this.SecurityNamespaceTokenAll, descriptor, (int) remove);
    }

    private IVssSecurityNamespace GetSecurityNamespace(IVssRequestContext requestContext) => requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, this.SecurityNamespaceId) ?? throw new ConfigurationErrorsException("Could not find security " + this.Area + " namespace");
  }
}
