// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.PermissionCheckerService
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class PermissionCheckerService : IPermissionCheckerService, IVssFrameworkService
  {
    private const string TraceArea = "Commerce";
    private const string TraceLayer = "PermissionCheckerService";

    public bool CheckPermission(
      IVssRequestContext requestContext,
      int permissions,
      Guid securityNamespaceId,
      string nameSpaceToken = "AllAccounts",
      bool throwAccessDenied = true)
    {
      requestContext.TraceEnter(5105951, "Commerce", nameof (PermissionCheckerService), nameof (CheckPermission));
      try
      {
        bool alwaysAllowAdministrators = true;
        if (securityNamespaceId.Equals(CommerceSecurity.CommerceSecurityNamespaceId) && requestContext.UserContext == (IdentityDescriptor) null)
          throw new UserContextNullReferenceException();
        if (securityNamespaceId.Equals(CollectionBasedPermission.NamespaceId))
          alwaysAllowAdministrators = false;
        return this.CheckSecurityPermissions(requestContext, permissions, securityNamespaceId, nameSpaceToken, throwAccessDenied, alwaysAllowAdministrators);
      }
      finally
      {
        requestContext.TraceLeave(5105953, "Commerce", nameof (PermissionCheckerService), nameof (CheckPermission));
      }
    }

    [ExcludeFromCodeCoverage]
    internal virtual bool CheckSecurityPermissions(
      IVssRequestContext requestContext,
      int permissions,
      Guid securityNamespaceId,
      string nameSpaceToken = "AllAccounts",
      bool throwAccessDenied = true,
      bool alwaysAllowAdministrators = true)
    {
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, securityNamespaceId);
      requestContext.Trace(5105952, TraceLevel.Info, "Commerce", nameof (PermissionCheckerService), string.Format("User CUID:{0},{1}Permission:{2}", (object) requestContext.GetUserCuid(), (object) Environment.NewLine, (object) permissions));
      if (!throwAccessDenied)
        return securityNamespace.HasPermission(requestContext, nameSpaceToken, permissions, alwaysAllowAdministrators);
      securityNamespace.CheckPermission(requestContext, nameSpaceToken, permissions, alwaysAllowAdministrators);
      return true;
    }

    public void ServiceStart(IVssRequestContext systemRequestContext) => systemRequestContext.Trace(5105954, TraceLevel.Info, "Commerce", nameof (PermissionCheckerService), "PermissionCheckerService starting");

    public void ServiceEnd(IVssRequestContext systemRequestContext) => systemRequestContext.Trace(5105955, TraceLevel.Info, "Commerce", nameof (PermissionCheckerService), "PermissionCheckerService ending");
  }
}
