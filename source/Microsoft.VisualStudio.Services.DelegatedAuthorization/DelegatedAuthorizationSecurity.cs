// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.DelegatedAuthorizationSecurity
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Security;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  public static class DelegatedAuthorizationSecurity
  {
    public static readonly Guid PublicDataAuthorizationNamespaceId = new Guid("d6e671ee-0bd9-4ede-8614-71109766580e");
    public const string PublicDataAuthorizationNamespaceToken = "AllUsers";
    public static readonly Guid ScopesSecurityNamespaceId = new Guid("9839dd6e-1592-4412-9487-5e80e037cf5d");
    private const string Area = "DelegatedAuthorization";
    private const string Layer = "DelegatedAuthorizationSecurity";

    internal static void CheckPermission(IVssRequestContext requestContext, int requestedPermission)
    {
      if (!ScopeSecurity.HasPermission(requestContext, DelegatedAuthorizationSecurity.ScopesSecurityNamespaceId, "/Scopes/Token", requestedPermission))
      {
        string accessData = DelegatedAuthorizationResources.NoPermissionToAccessData((object) (requestContext.GetUserIdentity() ?? requestContext.GetAuthenticatedIdentity()).Id);
        requestContext.Trace(1059002, TraceLevel.Warning, "DelegatedAuthorization", nameof (DelegatedAuthorizationSecurity), accessData);
        throw new AccessCheckException(accessData);
      }
    }

    internal static void CheckForAdministrativePermission(IVssRequestContext requestContext) => requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.FrameworkNamespaceId).CheckPermission(requestContext, FrameworkSecurity.FrameworkNamespaceToken, 2);

    public static class ScopePermissions
    {
      public const int Read = 1;
      public const int Write = 2;
      public const int Manage = 4;
      public const int Rotate = 8;
      public const int ReadWrite = 3;
    }

    public static class PublicDataAuthorizationPermissions
    {
      public const int Read = 1;
      public const int Write = 2;
      public const int ReadWrite = 3;
    }
  }
}
