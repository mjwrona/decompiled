// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.GallerySecurity
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.Server.Components;
using Microsoft.VisualStudio.Services.Gallery.Types.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Security;
using Microsoft.VisualStudio.Services.SecurityRoles;
using Microsoft.VisualStudio.Services.SecurityRoles.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  internal static class GallerySecurity
  {
    public static readonly Guid PublisherSecurityNamespace = GalleryConstants.SecurityNamespace;

    public static void CheckRootPermission(
      IVssRequestContext requestContext,
      PublisherPermissions requestedPermissions)
    {
      if (requestContext.ExecutionEnvironment.IsDevFabricDeployment && requestContext.IsServicingContext)
        return;
      IVssRequestContext vssRequestContext = requestContext;
      if (requestedPermissions.HasFlag((Enum) PublisherPermissions.Admin) && GallerySecurity.IsPartOfGalleryAdminSecurityGroup(requestContext))
        vssRequestContext = requestContext.Elevate();
      vssRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, GallerySecurity.PublisherSecurityNamespace).CheckPermission(vssRequestContext, string.Empty, (int) requestedPermissions);
    }

    public static bool HasRootPermission(
      IVssRequestContext requestContext,
      PublisherPermissions requestedPermissions)
    {
      if (requestContext.ExecutionEnvironment.IsDevFabricDeployment && requestContext.IsServicingContext)
        return true;
      IVssRequestContext vssRequestContext = requestContext;
      if (requestedPermissions.HasFlag((Enum) PublisherPermissions.Admin) && GallerySecurity.IsPartOfGalleryAdminSecurityGroup(requestContext))
        vssRequestContext = requestContext.Elevate();
      return vssRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, GallerySecurity.PublisherSecurityNamespace).HasPermission(vssRequestContext, string.Empty, (int) requestedPermissions);
    }

    public static void SetRootPermission(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor,
      int allow,
      int deny,
      bool merge)
    {
      requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, GallerySecurity.PublisherSecurityNamespace).SetPermissions(requestContext, string.Empty, descriptor, allow, deny, merge);
    }

    public static void CheckExtensionPermission(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      string accountToken,
      PublisherPermissions requestedPermissions,
      bool allowAnonymous)
    {
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, GallerySecurity.PublisherSecurityNamespace);
      string securityTokenByName = GallerySecurity.GetExtensionSecurityTokenByName(extension.Publisher.PublisherName, extension.ExtensionName);
      if (GallerySecurity.HasExtensionPermission(requestContext, extension, accountToken, requestedPermissions, allowAnonymous))
        return;
      if (requestContext.UserContext == (IdentityDescriptor) null)
        throw new AccessCheckException(UserWellKnownIdentityDescriptors.AnonymousPrincipal, securityTokenByName, (int) requestedPermissions, GallerySecurity.PublisherSecurityNamespace, GalleryResources.AnonymousAccessDeniedFormat((object) securityTokenByName, (object) string.Join(", ", securityNamespace.Description.GetLocalizedActions((int) requestedPermissions))));
      securityNamespace.CheckPermission(requestContext, securityTokenByName, (int) requestedPermissions);
    }

    public static void CheckExtensionChangePermissions(
      IVssRequestContext requestContext,
      string publisherName,
      PublishedExtensionFlags sourceFlags,
      PublishedExtensionFlags targetFlags,
      IEnumerable<InstallationTarget> installationTargets)
    {
      if (targetFlags.HasFlag((Enum) PublishedExtensionFlags.Locked) != sourceFlags.HasFlag((Enum) PublishedExtensionFlags.Locked) && !GallerySecurity.HasRootPermission(requestContext, PublisherPermissions.Admin))
        throw new AccessCheckException(GalleryResources.LockedExtensionEditErrorMessage());
      if (targetFlags.HasFlag((Enum) PublishedExtensionFlags.Validated) || targetFlags.HasFlag((Enum) PublishedExtensionFlags.Disabled) != sourceFlags.HasFlag((Enum) PublishedExtensionFlags.Disabled))
        GallerySecurity.CheckRootPermission(requestContext, PublisherPermissions.Admin);
      if (targetFlags.HasFlag((Enum) PublishedExtensionFlags.System) || targetFlags.HasFlag((Enum) PublishedExtensionFlags.BuiltIn) || targetFlags.HasFlag((Enum) PublishedExtensionFlags.Trusted))
        GallerySecurity.CheckRootPermission(requestContext, PublisherPermissions.TrustedPartner);
      if (!targetFlags.HasFlag((Enum) PublishedExtensionFlags.Public) || !requestContext.ExecutionEnvironment.IsHostedDeployment || !GallerySecurity.IsPublisherVerificationRequired(installationTargets))
        return;
      Publisher publisher;
      using (PublisherComponent component = requestContext.CreateComponent<PublisherComponent>())
        publisher = component.QueryPublisher(publisherName, PublisherQueryFlags.None);
      if (!publisher.Flags.HasFlag((Enum) PublisherFlags.Verified))
        throw new VerifiedPublisherRequiredException(GalleryWebApiResources.VerifiedPublisherRequired((object) publisherName, (object) "https://aka.ms/vsmarketplace-verify"));
    }

    public static string ParseAccountToken(IVssRequestContext requestContext, string accountToken)
    {
      string accountToken1 = (string) null;
      if (accountToken != null)
      {
        string key = "market.token:" + accountToken;
        object obj;
        if (requestContext.Items.TryGetValue(key, out obj))
        {
          accountToken1 = (string) obj;
        }
        else
        {
          IVssRequestContext vssRequestContext = requestContext.Elevate();
          JwtClaims jwtToken = vssRequestContext.GetService<IGalleryJwtTokenService>().ParseJwtToken(vssRequestContext, "AccountSigningKey", accountToken);
          if (jwtToken != null && jwtToken.ExtraClaims != null && jwtToken.ExtraClaims.TryGetValue("aid", out accountToken1))
            requestContext.Items[key] = (object) accountToken1;
        }
      }
      return accountToken1;
    }

    public static void CheckAssetPermissions(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      string accountToken,
      string assetToken)
    {
      bool flag = extension.Flags.HasFlag((Enum) PublishedExtensionFlags.Public);
      if (!flag && !string.IsNullOrEmpty(assetToken))
      {
        string publisherName;
        string extensionName;
        requestContext.GetService<IPublisherAssetService>().GetPublisherFromToken(requestContext, assetToken, out publisherName, out extensionName);
        flag = string.Equals(publisherName, extension.Publisher.PublisherName, StringComparison.OrdinalIgnoreCase) && string.Equals(extensionName, extension.ExtensionName, StringComparison.OrdinalIgnoreCase);
      }
      if (flag || GallerySecurity.HasExtensionPermission(requestContext, extension, accountToken, PublisherPermissions.PrivateRead, false))
        return;
      GallerySecurity.CheckExtensionPermission(requestContext, extension, accountToken, PublisherPermissions.Read, false);
    }

    public static bool HasExtensionPermission(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      string accountToken,
      PublisherPermissions requestedPermissions,
      bool allowAnonymous)
    {
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, GallerySecurity.PublisherSecurityNamespace);
      string securityTokenByName = GallerySecurity.GetExtensionSecurityTokenByName(extension.Publisher.PublisherName, extension.ExtensionName);
      bool hasPermission = false;
      if (!extension.Flags.HasFlag((Enum) PublishedExtensionFlags.Public))
      {
        if (extension.SharedWith != null)
        {
          if (!string.IsNullOrEmpty(accountToken))
          {
            string accountId = GallerySecurity.ParseAccountToken(requestContext, accountToken);
            if (!string.IsNullOrWhiteSpace(accountId))
            {
              hasPermission = extension.SharedWith.Find((Predicate<ExtensionShare>) (share => share.Type == "account" && share.Id.Equals(accountId, StringComparison.OrdinalIgnoreCase))) != null;
              if (!hasPermission)
              {
                List<ExtensionShare> all = extension.SharedWith.FindAll((Predicate<ExtensionShare>) (share => share.Type == "organization"));
                if (!(all != null ? all.Select<ExtensionShare, string>((Func<ExtensionShare, string>) (s => s.Id)) : (IEnumerable<string>) null).IsNullOrEmpty<string>())
                {
                  if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableTenantAsEnterpriseBoundary"))
                  {
                    hasPermission = GalleryOrganizationHelper.IsMicrosoftTenantBackedAccount(requestContext, accountId, extension?.SharedWith);
                  }
                  else
                  {
                    Guid? accountOrg = GalleryOrganizationHelper.GetOrganizationIdForCollection(requestContext, Guid.Parse(accountId));
                    if (accountOrg.HasValue)
                      hasPermission = extension.SharedWith.Find((Predicate<ExtensionShare>) (share => share.Type == "organization" && share.Id.Equals(accountOrg.Value.ToString(), StringComparison.OrdinalIgnoreCase))) != null;
                  }
                }
              }
            }
          }
          else if (requestContext.UserContext != (IdentityDescriptor) null && !ServicePrincipals.IsServicePrincipal(requestContext, requestContext.UserContext))
          {
            IFirstPartyPublisherAccessService service = requestContext.GetService<IFirstPartyPublisherAccessService>();
            List<ExtensionShare> all = extension.SharedWith.FindAll((Predicate<ExtensionShare>) (share => share.Type == "organization"));
            if (!(all != null ? all.Select<ExtensionShare, string>((Func<ExtensionShare, string>) (s => s.Id)) : (IEnumerable<string>) null).IsNullOrEmpty<string>())
              hasPermission = service.IsInternalEmployee(requestContext) && !service.IsInternalPartner(requestContext);
          }
        }
      }
      else if (allowAnonymous)
        hasPermission = true;
      if (!hasPermission && requestContext.UserContext != (IdentityDescriptor) null)
      {
        hasPermission = securityNamespace.HasPermission(requestContext, securityTokenByName, (int) requestedPermissions);
        GallerySecurity.TraceIfHasPermissionForUpdateExtension(requestContext, extension, accountToken, requestedPermissions, allowAnonymous, securityNamespace, securityTokenByName, hasPermission, "PublisherNameSpaceCheck");
      }
      if (!hasPermission && GallerySecurity.IsExtensionUpdateRequest(requestedPermissions) && requestContext.UserContext != (IdentityDescriptor) null)
      {
        hasPermission = GallerySecurity.IsPartOfGalleryAdminSecurityGroup(requestContext);
        GallerySecurity.TraceIfHasPermissionForUpdateExtension(requestContext, extension, accountToken, requestedPermissions, allowAnonymous, securityNamespace, securityTokenByName, hasPermission, "PartOfGalleryAdminSecurityGroupCheck");
      }
      requestContext.Trace(12061035, TraceLevel.Info, "gallery", nameof (GallerySecurity), string.Format("HasExtensionPermission | AccountToken:{0}, allowAnonymous:{1}, PublisherNamespace:{2}, SecurityTokenByName:{3}, RequestedPermissions:{4}, ExtensionFlags:{5}, hasPermission:{6}", (object) accountToken, (object) allowAnonymous, (object) securityNamespace, (object) securityTokenByName, (object) (int) requestedPermissions, (object) (int) extension.Flags, (object) hasPermission));
      return hasPermission;
    }

    private static void TraceIfHasPermissionForUpdateExtension(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      string accountToken,
      PublisherPermissions requestedPermissions,
      bool allowAnonymous,
      IVssSecurityNamespace publisherNamespace,
      string securityTokenByName,
      bool hasPermission,
      string permissionCheckStep)
    {
      if (((!requestedPermissions.HasFlag((Enum) PublisherPermissions.UpdateExtension) ? 0 : (!allowAnonymous ? 1 : 0)) & (hasPermission ? 1 : 0)) == 0 || accountToken != null)
        return;
      requestContext.Trace(12061051, TraceLevel.Info, "gallery", nameof (GallerySecurity), string.Format("HasExtensionPermission | AccountToken:{0}, allowAnonymous:{1}, PublisherNamespace:{2}, SecurityTokenByName:{3}, RequestedPermissions:{4}, ExtensionFlags:{5}, hasPermission:{6}, Stepname:{7}", (object) accountToken, (object) allowAnonymous, (object) publisherNamespace, (object) securityTokenByName, (object) (int) requestedPermissions, (object) (int) extension.Flags, (object) hasPermission, (object) permissionCheckStep));
    }

    public static void SetExtensionPermission(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      IdentityDescriptor descriptor,
      int allow,
      int deny,
      bool merge)
    {
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, GallerySecurity.PublisherSecurityNamespace);
      string securityTokenByName = GallerySecurity.GetExtensionSecurityTokenByName(extension.Publisher.PublisherName, extension.ExtensionName);
      IVssRequestContext requestContext1 = requestContext;
      string token = securityTokenByName;
      IdentityDescriptor descriptor1 = descriptor;
      int allow1 = allow;
      int deny1 = deny;
      int num = merge ? 1 : 0;
      securityNamespace.SetPermissions(requestContext1, token, descriptor1, allow1, deny1, num != 0);
    }

    public static void CheckPublisherPermission(
      IVssRequestContext requestContext,
      Publisher publisher,
      PublisherPermissions requestedPermissions)
    {
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, GallerySecurity.PublisherSecurityNamespace);
      if (GallerySecurity.HasPublisherPermission(requestContext, publisher, requestedPermissions))
        return;
      securityNamespace.OnDataChanged(requestContext);
      if (GallerySecurity.HasPublisherPermission(requestContext, publisher, requestedPermissions))
        return;
      securityNamespace.CheckPermission(requestContext, GallerySecurity.GetPublisherSecurityTokenByName(publisher.PublisherName), (int) requestedPermissions);
    }

    public static bool HasPublisherPermission(
      IVssRequestContext requestContext,
      Publisher publisher,
      PublisherPermissions requestedPermissions)
    {
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, GallerySecurity.PublisherSecurityNamespace);
      string securityTokenById = GallerySecurity.GetPublisherSecurityTokenById(publisher.PublisherId);
      string token = string.Empty;
      requestContext.Trace(12061029, TraceLevel.Info, "gallery", nameof (HasPublisherPermission), string.Format("Checking Publisher Permission for PublisherSecurityTokenById = {0}", (object) securityTokenById));
      bool flag;
      if (!(flag = securityNamespace.HasPermission(requestContext, securityTokenById, (int) requestedPermissions)))
      {
        token = GallerySecurity.GetPublisherSecurityTokenByName(publisher.PublisherName);
        requestContext.Trace(12061029, TraceLevel.Info, "gallery", nameof (HasPublisherPermission), string.Format("Checking Publisher Permission for PublisherSecurityTokenByName = {0}", (object) token));
        flag = securityNamespace.HasPermission(requestContext, token, (int) requestedPermissions);
      }
      if (!flag)
        requestContext.Trace(12061029, TraceLevel.Info, "gallery", nameof (HasPublisherPermission), string.Format("Permission disabled for PublisherSecurityTokenById = {0}, PublisherSecurityTokenByName = {1}", (object) securityTokenById, (object) token));
      if (!flag && GallerySecurity.IsPublisherUpdateRequest(requestContext, requestedPermissions) && requestContext.UserContext != (IdentityDescriptor) null)
        flag = GallerySecurity.IsPartOfGalleryAdminSecurityGroup(requestContext);
      return flag;
    }

    public static void OnDataChanged(IVssRequestContext requestContext) => requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, GallerySecurity.PublisherSecurityNamespace).OnDataChanged(requestContext.Elevate());

    public static void SetPublisherPermission(
      IVssRequestContext requestContext,
      Publisher publisher,
      IdentityDescriptor descriptor,
      int allow,
      int deny,
      bool merge)
    {
      requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, GallerySecurity.PublisherSecurityNamespace).SetPermissions(requestContext, GallerySecurity.GetPublisherSecurityTokenByName(publisher.PublisherName), descriptor, allow, deny, merge);
    }

    public static bool IsPartOfGalleryAdminSecurityGroup(IVssRequestContext requestContext)
    {
      bool flag = false;
      if (requestContext.UserContext != (IdentityDescriptor) null && requestContext.ExecutionEnvironment.IsHostedDeployment && !ServicePrincipals.IsServicePrincipal(requestContext, requestContext.UserContext))
      {
        IGalleryAdminAuthorizer extension = requestContext.GetExtension<IGalleryAdminAuthorizer>();
        if (extension != null)
        {
          try
          {
            flag = extension.IsPartOfGalleryAdminSecurityGroup(requestContext);
            requestContext.Trace(12061030, TraceLevel.Info, "gallery", "HasPublisherPermission", string.Format("IsPartOfGalleryAdminSecurityGroup = {0}", (object) flag));
          }
          catch (Exception ex)
          {
            requestContext.Trace(12061018, TraceLevel.Warning, "gallery", "GalleryAdminAuthorizer", ex.ToString());
            flag = false;
          }
        }
      }
      return flag;
    }

    public static SecurityRole GetPublisherRoleForGivenVSID(
      IVssRequestContext requestContext,
      Publisher publisher,
      Guid vsid)
    {
      foreach (RoleAssignment roleAssignment in requestContext.GetService<ISecurityRoleMappingService>().GetRoleAssignments(requestContext.Elevate(), publisher.PublisherName, "gallery.publisher"))
      {
        if (roleAssignment.Identity.Id == vsid.ToString())
          return roleAssignment.Role;
      }
      return (SecurityRole) null;
    }

    private static bool IsPublisherUpdateRequest(
      IVssRequestContext requestContext,
      PublisherPermissions requestedPermissions)
    {
      PublisherPermissions flag = PublisherPermissions.EditSettings;
      return requestedPermissions.HasFlag((Enum) flag);
    }

    private static bool IsExtensionUpdateRequest(PublisherPermissions requestedPermissions) => requestedPermissions.HasFlag((Enum) PublisherPermissions.UpdateExtension);

    private static string GetExtensionSecurityTokenById(Guid publisherId, Guid extensionId) => string.Format("{0}/{1}", (object) GallerySecurity.GetPublisherSecurityTokenById(publisherId), (object) extensionId.ToString());

    internal static string GetExtensionSecurityTokenByName(
      string publisherName,
      string extensionName)
    {
      return string.Format("{0}/{1}", (object) GallerySecurity.GetPublisherSecurityTokenByName(publisherName), (object) extensionName);
    }

    private static string GetPublisherSecurityTokenById(Guid publisherId)
    {
      string securityTokenById = string.Empty;
      if (publisherId != Guid.Empty)
        securityTokenById = securityTokenById + "/" + publisherId.ToString();
      return securityTokenById;
    }

    private static bool IsPublisherVerificationRequired(
      IEnumerable<InstallationTarget> installationTargets)
    {
      if (installationTargets == null)
        return true;
      string installationTargets1 = GalleryUtil.GetProductTypeForInstallationTargets(installationTargets);
      return !string.Equals("vscode", installationTargets1, StringComparison.OrdinalIgnoreCase) && !string.Equals("vs", installationTargets1, StringComparison.OrdinalIgnoreCase) && !string.Equals("vsformac", installationTargets1, StringComparison.OrdinalIgnoreCase);
    }

    internal static string GetPublisherSecurityTokenByName(string publisherName)
    {
      string securityTokenByName = string.Empty;
      if (!string.IsNullOrEmpty(publisherName))
        securityTokenByName = securityTokenByName + "/" + publisherName;
      return securityTokenByName;
    }
  }
}
