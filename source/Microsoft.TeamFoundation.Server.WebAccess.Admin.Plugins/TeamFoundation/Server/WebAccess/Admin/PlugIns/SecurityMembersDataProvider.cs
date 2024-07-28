// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns.SecurityMembersDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.Helpers;
using Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns
{
  public class SecurityMembersDataProvider : IExtensionDataProvider
  {
    private readonly IAadServicePrincipalConfigurationHelper aadServicePrincipalConfigurationHelper;

    public SecurityMembersDataProvider()
      : this(AadServicePrincipalConfigurationHelper.Instance)
    {
    }

    public SecurityMembersDataProvider(
      IAadServicePrincipalConfigurationHelper aadServicePrincipalConfigurationHelper)
    {
      this.aadServicePrincipalConfigurationHelper = aadServicePrincipalConfigurationHelper;
    }

    public string Name => "Admin.SecurityMembers";

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      try
      {
        Guid permissionSetId = new Guid();
        string permissionSetToken = (string) null;
        SecurityMembersDataProvider.GetSecurityMembersParams(providerContext, out permissionSetId, out permissionSetToken);
        ArgumentUtility.CheckForEmptyGuid(permissionSetId, "permissionSetId");
        ArgumentUtility.CheckStringForNullOrEmpty(permissionSetToken, "token");
        TfsWebContext webContext = (TfsWebContext) WebContextFactory.GetWebContext(requestContext);
        SecurityNamespacePermissionsManager permissionsManager = SecurityNamespacePermissionsManagerFactory.CreateManager(requestContext, permissionSetId, permissionSetToken, webContext.ProjectContext?.Name);
        List<TeamFoundationIdentity> source = new List<TeamFoundationIdentity>((IEnumerable<TeamFoundationIdentity>) permissionsManager.GetIdentities(requestContext));
        source.Sort((IComparer<TeamFoundationIdentity>) new TeamFoundationIdentityDisplayNameComparer());
        IdentityViewData identityViewData = TfsAdminIdentityHelper.JsonFromFilteredIdentitiesList(requestContext, new TeamFoundationFilteredIdentitiesList()
        {
          Items = source.Where<TeamFoundationIdentity>((Func<TeamFoundationIdentity, bool>) (identity => permissionsManager.ShouldIncludeIdentity(identity))).ToArray<TeamFoundationIdentity>()
        });
        bool flag = this.aadServicePrincipalConfigurationHelper.IsObjectLevelSecurityMembersSupportServicePrincipalsEnabled(requestContext);
        return (object) new SecurityMembersData()
        {
          Identities = identityViewData.Identities,
          TotalIdentityCount = identityViewData.Identities.Count<GraphViewModel>(),
          SupportServicePrincipals = flag
        };
      }
      catch (UnauthorizedAccessException ex)
      {
        requestContext.Trace(10050076, TraceLevel.Error, "SecurityMembers", "DataProvider", ex.Message);
        return (object) new EmptyResult();
      }
      catch (Exception ex)
      {
        requestContext.Trace(10050077, TraceLevel.Error, "SecurityMembers", "DataProvider", ex.Message);
        return (object) new EmptyResult();
      }
    }

    private static void GetSecurityMembersParams(
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
