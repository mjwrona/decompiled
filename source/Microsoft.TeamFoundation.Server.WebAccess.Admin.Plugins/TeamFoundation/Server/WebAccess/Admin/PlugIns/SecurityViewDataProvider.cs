// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns.SecurityViewDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Organization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web.Script.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns
{
  public class SecurityViewDataProvider : IExtensionDataProvider
  {
    public string Name => "Admin.SecurityView";

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      SecurityViewData data;
      try
      {
        TfsWebContext webContext = (TfsWebContext) WebContextFactory.GetWebContext(requestContext);
        Guid permissionSetId = new Guid();
        string permissionSetToken = (string) null;
        SecurityViewDataProvider.GetSecurityViewDataParams(providerContext, out permissionSetId, out permissionSetToken);
        SecurityNamespacePermissionsManager permissionsManager = SecurityViewDataProvider.CreatePermissionsManager(webContext, new Guid?(permissionSetId), permissionSetToken);
        data = new SecurityViewData();
        data.UserHasReadAccess = permissionsManager.UserHasReadAccess;
        if (data.UserHasReadAccess)
        {
          ManageViewModel manageViewModel = new ManageViewModel();
          if (webContext.NavigationContext.TopMostLevel == NavigationContextLevels.Team)
          {
            data.Title = webContext.IsHosted ? AdminResources.Users : AdminServerResources.UsersAndGroups;
            manageViewModel.DefaultFilter = "users";
          }
          else
          {
            data.Title = AdminResources.Members;
            manageViewModel.DefaultFilter = "groups";
          }
          data.IsAadGroupsAdminUi = requestContext.IsFeatureEnabled("VisualStudio.Services.AadGroupsAdminUi");
          SecurityModel securityModel = this.CreateSecurityModel(webContext, permissionsManager, new Guid?(permissionSetId), permissionSetToken, (string) null, new bool?(), false);
          data.PermissionsContextJson = !requestContext.IsFeatureEnabled("VisualStudio.Services.EnableSafeDeserializer") ? new JavaScriptSerializer().Serialize((object) securityModel.ToJson()) : JsonConvert.SerializeObject((object) securityModel.ToJson());
          if (webContext.IsHosted && requestContext.IsOrganizationAadBacked())
            manageViewModel.IsAadAccount = true;
          data.HasSingleCollectionAdmin = false;
          switch (webContext.NavigationContext.TopMostLevel)
          {
            case NavigationContextLevels.Collection:
              manageViewModel.DisplayScope = requestContext.ServiceHost.CollectionServiceHost.Name;
              if (webContext.IsHosted && !requestContext.IsOrganizationAadBacked() && SecurityViewDataProvider.GetLicensedUsersCount(webContext.TfsRequestContext) > 1 && SecurityViewDataProvider.HasSingleProjectCollectionAdmin(webContext.TfsRequestContext))
              {
                data.HasSingleCollectionAdmin = true;
                break;
              }
              break;
            case NavigationContextLevels.Project:
              manageViewModel.DisplayScope = webContext.Project.Name;
              break;
            case NavigationContextLevels.Team:
              if (webContext.Team == null)
                throw new IdentityNotFoundException(webContext.NavigationContext.Team);
              manageViewModel.DisplayScope = webContext.Team.Name;
              break;
          }
          data.SecurityViewOptionsJson = !requestContext.IsFeatureEnabled("VisualStudio.Services.EnableSafeDeserializer") ? new JavaScriptSerializer().Serialize((object) new
          {
            displayScope = manageViewModel.DisplayScope,
            defaultFilter = manageViewModel.DefaultFilter,
            isAadAccount = manageViewModel.IsAadAccount
          }) : JsonConvert.SerializeObject((object) new
          {
            displayScope = manageViewModel.DisplayScope,
            defaultFilter = manageViewModel.DefaultFilter,
            isAadAccount = manageViewModel.IsAadAccount
          });
          data.UserIsCollectionAdmin = this.CheckIsCollectionAdmin(webContext.TfsRequestContext);
        }
      }
      catch (Exception ex)
      {
        requestContext.Trace(10050090, TraceLevel.Error, "SecurityView", "DataProvider", ex.Message);
        return (object) null;
      }
      return (object) data;
    }

    internal static SecurityNamespacePermissionsManager CreatePermissionsManager(
      TfsWebContext tfsWebContext,
      Guid? permissionSet,
      string token)
    {
      Guid permissionSetId = SecurityViewDataProvider.GetPermissionSetId(tfsWebContext.NavigationContext, permissionSet);
      if (string.IsNullOrWhiteSpace(token))
        token = SecurityViewDataProvider.GetDefaultPermissionSetToken(tfsWebContext.TfsRequestContext, tfsWebContext, permissionSetId);
      return SecurityNamespacePermissionsManagerFactory.CreateManager(tfsWebContext.TfsRequestContext, permissionSetId, token, tfsWebContext.ProjectContext?.Name) ?? throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, WACommonResources.InvalidParameter, (object) nameof (permissionSet)));
    }

    private bool CheckIsCollectionAdmin(IVssRequestContext requestContext) => requestContext.GetService<TeamFoundationIdentityService>().IsMember(requestContext, GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup, requestContext.UserContext);

    private SecurityModel CreateSecurityModel(
      TfsWebContext tfsWebContext,
      SecurityNamespacePermissionsManager permissionsManager,
      Guid? permissionSet,
      string token,
      string tokenDisplayVal,
      bool? showAllGroupsIfCollection,
      bool controlManagesFocus = true)
    {
      Guid permissionSetId = SecurityViewDataProvider.GetPermissionSetId(tfsWebContext.NavigationContext, permissionSet);
      if (string.IsNullOrWhiteSpace(token))
        token = SecurityViewDataProvider.GetDefaultPermissionSetToken(tfsWebContext.TfsRequestContext, tfsWebContext, permissionSetId);
      return new SecurityModel()
      {
        ViewTitle = tokenDisplayVal,
        TitlePrefix = SecurityViewDataProvider.GetPermissionTitlePrefix(permissionSetId),
        Title = SecurityViewDataProvider.GetPermissionTitle(tfsWebContext, permissionSetId, token, tokenDisplayVal),
        Header = SecurityViewDataProvider.GetPermissionHeader(permissionSetId),
        PermissionSetId = permissionSetId,
        Token = token,
        CanManageIdentities = permissionsManager.CanManageIdentities,
        CanManagePermissions = permissionsManager.CanManagePermissions,
        InheritPermissions = permissionsManager.InheritPermissions,
        CanTokenInheritPermissions = permissionsManager.CanTokenInheritPermissions,
        CustomDoNotHavePermissionsText = permissionsManager.CustomDoNotHavePermissionsText,
        HideExplicitClearButton = permissionsManager.HideExplicitClearButton,
        ShowAllGroupsIfCollection = showAllGroupsIfCollection.HasValue && showAllGroupsIfCollection.Value,
        ControlManagesFocus = controlManagesFocus
      };
    }

    protected static string GetPermissionTitlePrefix(Guid permissionSetId)
    {
      if (permissionSetId == NamespacePermissionSetConstants.VersionControl)
        return AdminResources.Path;
      if (permissionSetId == NamespacePermissionSetConstants.Build)
        return AdminResources.BuildDefinition;
      if (permissionSetId == NamespacePermissionSetConstants.Area)
        return AdminResources.Area;
      if (permissionSetId == NamespacePermissionSetConstants.Iteration)
        return AdminResources.Iteration;
      return permissionSetId == NamespacePermissionSetConstants.WitQueryFolders ? AdminResources.Path : string.Empty;
    }

    protected static string GetPermissionTitle(
      TfsWebContext webContext,
      Guid permissionSetId,
      string token,
      string tokenDisplayVal)
    {
      if (permissionSetId == NamespacePermissionSetConstants.ProjectLevel)
        return webContext.Project.Name;
      if (permissionSetId == NamespacePermissionSetConstants.CollectionLevel)
        return webContext.TfsRequestContext.ServiceHost.CollectionServiceHost.Name;
      return permissionSetId == NamespacePermissionSetConstants.VersionControl ? token : tokenDisplayVal;
    }

    protected static Guid GetPermissionSetId(NavigationContext navigationContext, Guid? id) => !id.HasValue || !(id.Value != new Guid()) ? (!navigationContext.Levels.HasFlag((Enum) NavigationContextLevels.Project) ? NamespacePermissionSetConstants.CollectionLevel : NamespacePermissionSetConstants.ProjectLevel) : id.Value;

    protected static string GetDefaultPermissionSetToken(
      IVssRequestContext requestContext,
      TfsWebContext webContext,
      Guid permissionSetId)
    {
      return permissionSetId == NamespacePermissionSetConstants.ProjectLevel ? webContext.Project.Uri : string.Empty;
    }

    private static string GetPermissionHeader(Guid permissionSetId)
    {
      if (permissionSetId == NamespacePermissionSetConstants.OrganizationLevel)
        return AdminServerResources.OrganizationPermissions;
      if (permissionSetId == NamespacePermissionSetConstants.ProjectLevel)
        return AdminServerResources.ProjectPermissions;
      if (permissionSetId == NamespacePermissionSetConstants.CollectionLevel)
        return AdminServerResources.CollectionPermissions;
      if (permissionSetId == NamespacePermissionSetConstants.VersionControl)
        return AdminServerResources.VersionControlPermissions;
      if (permissionSetId == NamespacePermissionSetConstants.Build)
        return AdminServerResources.BuildPermissions;
      if (permissionSetId == NamespacePermissionSetConstants.Area)
        return AdminServerResources.AreaPermissions;
      if (permissionSetId == NamespacePermissionSetConstants.Iteration)
        return AdminServerResources.IterationPermissions;
      if (permissionSetId == NamespacePermissionSetConstants.WitQueryFolders)
        return AdminServerResources.WorkItemQueryPermissions;
      return permissionSetId == NamespacePermissionSetConstants.Plan ? AdminServerResources.PlanPermissions : string.Empty;
    }

    private static bool HasSingleProjectCollectionAdmin(IVssRequestContext requestContext)
    {
      requestContext.CheckProjectCollectionRequestContext();
      int num1 = 0;
      int num2 = 0;
      int num3 = 10;
      List<IdentityDescriptor> descriptors = new List<IdentityDescriptor>()
      {
        GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup
      };
      IdentityService service = requestContext.GetService<IdentityService>();
      HashSet<IdentityDescriptor> collection = new HashSet<IdentityDescriptor>();
      for (; num2 < num3 && descriptors.Count > 0; ++num2)
      {
        IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = service.ReadIdentities(requestContext, (IList<IdentityDescriptor>) descriptors, QueryMembership.Direct, (IEnumerable<string>) null);
        if (identityList == null)
          return true;
        foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identityList)
        {
          if (identity != null && !identity.Members.IsNullOrEmpty<IdentityDescriptor>())
          {
            num1 += identity.Members.Count<IdentityDescriptor>((Func<IdentityDescriptor, bool>) (x => x != (IdentityDescriptor) null && x.IsClaimsIdentityType() && !ServicePrincipals.IsServicePrincipal(requestContext, x)));
            if (num1 > 1)
              return false;
            collection.AddRange<IdentityDescriptor, HashSet<IdentityDescriptor>>(identity.Members.Where<IdentityDescriptor>((Func<IdentityDescriptor, bool>) (x => x != (IdentityDescriptor) null && x.IsTeamFoundationType())));
          }
        }
        descriptors.Clear();
        descriptors.AddRange((IEnumerable<IdentityDescriptor>) collection);
        collection.Clear();
      }
      return true;
    }

    private static int GetLicensedUsersCount(IVssRequestContext requestContext)
    {
      requestContext.CheckProjectCollectionRequestContext();
      Microsoft.VisualStudio.Services.Identity.Identity readIdentity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext.Elevate(), (IList<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        GroupWellKnownIdentityDescriptors.LicensedUsersGroup
      }, QueryMembership.Direct, (IEnumerable<string>) null, true)[0];
      return readIdentity == null || readIdentity.Members.IsNullOrEmpty<IdentityDescriptor>() ? 0 : readIdentity.Members.Count<IdentityDescriptor>((Func<IdentityDescriptor, bool>) (x => x != (IdentityDescriptor) null && x.IsClaimsIdentityType() && !ServicePrincipals.IsServicePrincipal(requestContext, x)));
    }

    private static void GetSecurityViewDataParams(
      DataProviderContext providerContext,
      out Guid permissionSetId,
      out string permissionSetToken)
    {
      permissionSetId = new Guid();
      permissionSetToken = (string) null;
      if (providerContext.Properties.ContainsKey(nameof (permissionSetId)) && providerContext.Properties[nameof (permissionSetId)] != null)
        Guid.TryParse(providerContext.Properties[nameof (permissionSetId)].ToString(), out permissionSetId);
      if (!providerContext.Properties.ContainsKey(nameof (permissionSetToken)) || providerContext.Properties[nameof (permissionSetToken)] == null)
        return;
      permissionSetToken = providerContext.Properties[nameof (permissionSetToken)].ToString();
    }
  }
}
