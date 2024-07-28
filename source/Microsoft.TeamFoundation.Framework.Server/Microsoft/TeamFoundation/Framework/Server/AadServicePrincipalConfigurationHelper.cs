// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AadServicePrincipalConfigurationHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.ConfigFramework;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Organization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class AadServicePrincipalConfigurationHelper : IAadServicePrincipalConfigurationHelper
  {
    private const string TraceArea = "ConfigFramework";
    private const string TraceLayer = "AadServicePrincipalConfigurationHelper";
    private static readonly IConfigPrototype<bool> AadSPTokenValidationEnabledPrototype;
    private static readonly IConfigPrototype<bool> AadSPCrudOperationsEnabledPrototype;
    private static readonly IConfigPrototype<bool> AadSPGraphOperationsEnabledPrototype;
    private static readonly IConfigPrototype<bool> SearchingForServicePrincipalsEnabledPrototype;
    private static readonly IConfigPrototype<bool> SyncServicePrincipalConfigPrototype;
    private static readonly IConfigPrototype<bool> GroupRulesServicePrincipalConfigPrototype;
    private static readonly IConfigPrototype<bool> UserHubSupportServicePrincipalsEnabledPrototype;
    private static readonly IConfigPrototype<bool> FixServicePrincipalsWithUnknownMetaTypePrototype;
    private static readonly IConfigPrototype<bool> OrgGroupMembershipSupportServicePrincipalsEnabledPrototype;
    private static readonly IConfigPrototype<bool> RestrictionOfCertainApisForServicePrincipalsEnabledPrototype;
    private static readonly IConfigPrototype<bool> AadBackedOrgAttributeEnalbedPrototype;
    private static readonly IConfigPrototype<bool> SPPermissionsOperationsInUIEnabledPrototype;
    private static readonly IConfigPrototype<bool> MergeUserAndSPPermissionsInUIEnabledPrototype;
    private static readonly IConfigPrototype<bool> ObjectLevelSecurityMembersSupportServicePrincipalsEnabledPrototype;
    private static readonly IConfigPrototype<bool> BranchPolicyStatusChecksSupportServicePrincipalsEnabledPrototype;
    private static readonly IConfigPrototype<bool> ArtifactsPermissionsSupportServicePrincipalsEnabledPrototype;
    private static readonly IConfigPrototype<bool> EmsPermissionsSupportServicePrincipalsEnabledPrototype;
    private static readonly IConfigPrototype<bool> AllowedSubjectTypesEnabledPrototype;
    private static readonly IConfigPrototype<Dictionary<string, HashSet<Guid>>> AadServicePrincipalRestrictionWhiteListPrototype;
    private static ReadOnlyDictionary<string, HashSet<Guid>> s_aadServicePrincipalRestrictionPreWhiteList = new ReadOnlyDictionary<string, HashSet<Guid>>((IDictionary<string, HashSet<Guid>>) new Dictionary<string, HashSet<Guid>>()
    {
      {
        "WhiteList.SessionToken",
        new HashSet<Guid>()
      }
    });
    public static IAadServicePrincipalConfigurationHelper Instance;
    private readonly IConfigQueryable<bool> AadSPTokenValidationEnabledConfig;
    private readonly IConfigQueryable<bool> AadSPCrudOperationsEnabledConfig;
    private readonly IConfigQueryable<bool> AadSPGraphOperationsEnabledConfig;
    private readonly IConfigQueryable<bool> SearchingForServicePrincipalsEnabledConfig;
    private readonly IConfigQueryable<bool> SyncServicePrincipalMembershipsEnabledConfig;
    private readonly IConfigQueryable<bool> GroupRulesServicePrincipalMembershipsEnabledConfig;
    private readonly IConfigQueryable<bool> UserHubSupportServicePrincipalsEnabledConfig;
    private readonly IConfigQueryable<bool> FixServicePrincipalsWithUnknownMetaTypeConfig;
    private readonly IConfigQueryable<bool> OrgGroupMembershipSupportServicePrincipalsEnabledConfig;
    private readonly IConfigQueryable<bool> RestrictionOfCertainApisForServicePrincipalsEnabledConfig;
    private readonly IConfigQueryable<bool> AadBackedOrgAttributeEnalbedConfig;
    private readonly IConfigQueryable<bool> SPPermissionsOperationsInUIEnabledConfig;
    private readonly IConfigQueryable<bool> MergeUserAndSPPermissionsInUIEnabledConfig;
    private readonly IConfigQueryable<bool> ObjectLevelSecurityMembersSupportServicePrincipalsEnabledConfig;
    private readonly IConfigQueryable<bool> BranchPolicyStatusChecksSupportServicePrincipalsEnabledConfig;
    private readonly IConfigQueryable<bool> ArtifactsPermissionsSupportServicePrincipalsEnabledConfig;
    private readonly IConfigQueryable<bool> EmsPermissionsSupportServicePrincipalsEnabledConfig;
    private readonly IConfigQueryable<bool> AllowedSubjectTypesEnabledConfig;
    private readonly IConfigQueryable<Dictionary<string, HashSet<Guid>>> AadServicePrincipalRestrictionWhiteListConfig;

    static AadServicePrincipalConfigurationHelper()
    {
      AadServicePrincipalConfigurationHelper.AadSPTokenValidationEnabledPrototype = ConfigPrototype.Create<bool>("Identity.AADSPTokenValidationEnabled.M207", false);
      AadServicePrincipalConfigurationHelper.AadSPCrudOperationsEnabledPrototype = ConfigPrototype.Create<bool>("Identity.AADSPCrudOperationsEnabled.M207", false);
      AadServicePrincipalConfigurationHelper.AadSPGraphOperationsEnabledPrototype = ConfigPrototype.Create<bool>("Identity.AadSPGraphOperationsEnabled.M209", false);
      AadServicePrincipalConfigurationHelper.SearchingForServicePrincipalsEnabledPrototype = ConfigPrototype.Create<bool>("Identity.SearchingForServicePrincipalsEnabled", false);
      AadServicePrincipalConfigurationHelper.SyncServicePrincipalConfigPrototype = ConfigPrototype.Create<bool>("Identity.AADSPSyncGroupMembershipsEnabled.M212", false);
      AadServicePrincipalConfigurationHelper.GroupRulesServicePrincipalConfigPrototype = ConfigPrototype.Create<bool>("Identity.ServicePrincipalGroupRules.M214", false);
      AadServicePrincipalConfigurationHelper.UserHubSupportServicePrincipalsEnabledPrototype = ConfigPrototype.Create<bool>("Identity.UserHubSupportServicePrincipalsEnabled", false);
      AadServicePrincipalConfigurationHelper.FixServicePrincipalsWithUnknownMetaTypePrototype = ConfigPrototype.Create<bool>("Identity.FixServicePrincipalsWithUnknownMetaType.M214", false);
      AadServicePrincipalConfigurationHelper.OrgGroupMembershipSupportServicePrincipalsEnabledPrototype = ConfigPrototype.Create<bool>("Identity.OrgGroupMembershipSupportServicePrincipalsEnabled", false);
      AadServicePrincipalConfigurationHelper.RestrictionOfCertainApisForServicePrincipalsEnabledPrototype = ConfigPrototype.Create<bool>("Identity.RestrictionOfCertainApisForSPsEnabled.215", false);
      AadServicePrincipalConfigurationHelper.AadBackedOrgAttributeEnalbedPrototype = ConfigPrototype.Create<bool>("Identity.RequireAadBackedOrgAttributeEnalbed.M216", false);
      AadServicePrincipalConfigurationHelper.SPPermissionsOperationsInUIEnabledPrototype = ConfigPrototype.Create<bool>("Identity.SPPermissionsOperationsInUIEnabled", false);
      AadServicePrincipalConfigurationHelper.MergeUserAndSPPermissionsInUIEnabledPrototype = ConfigPrototype.Create<bool>("Identity.MergeUserAndSPPermissionsInUIEnabled", false);
      AadServicePrincipalConfigurationHelper.ObjectLevelSecurityMembersSupportServicePrincipalsEnabledPrototype = ConfigPrototype.Create<bool>("Identity.ObjectLevelSecurityMembersSupportServicePrincipalsEnabled.M218", false);
      AadServicePrincipalConfigurationHelper.BranchPolicyStatusChecksSupportServicePrincipalsEnabledPrototype = ConfigPrototype.Create<bool>("Identity.BranchPolicyStatusChecksSupportServicePrincipalsEnabled.M218", false);
      AadServicePrincipalConfigurationHelper.ArtifactsPermissionsSupportServicePrincipalsEnabledPrototype = ConfigPrototype.Create<bool>("Identity.ArtifactsPermissionsSupportServicePrincipalsEnabled.M220", false);
      AadServicePrincipalConfigurationHelper.EmsPermissionsSupportServicePrincipalsEnabledPrototype = ConfigPrototype.Create<bool>("Identity.EmsPermissionsSupportServicePrincipalsEnabled.M220", false);
      AadServicePrincipalConfigurationHelper.AllowedSubjectTypesEnabledPrototype = ConfigPrototype.Create<bool>("Identity.AllowedSubjectTypesEnabled.M226", false);
      AadServicePrincipalConfigurationHelper.AadServicePrincipalRestrictionWhiteListPrototype = ConfigPrototype.Create<Dictionary<string, HashSet<Guid>>>("Identity.AadServicePrincipalRestrictionWhiteList.LongTerm", new Dictionary<string, HashSet<Guid>>());
      AadServicePrincipalConfigurationHelper.Instance = (IAadServicePrincipalConfigurationHelper) new AadServicePrincipalConfigurationHelper();
    }

    private AadServicePrincipalConfigurationHelper()
      : this(ConfigProxy.Create<bool>(AadServicePrincipalConfigurationHelper.AadSPTokenValidationEnabledPrototype), ConfigProxy.Create<bool>(AadServicePrincipalConfigurationHelper.AadSPCrudOperationsEnabledPrototype), ConfigProxy.Create<bool>(AadServicePrincipalConfigurationHelper.AadSPGraphOperationsEnabledPrototype), ConfigProxy.Create<bool>(AadServicePrincipalConfigurationHelper.SearchingForServicePrincipalsEnabledPrototype), ConfigProxy.Create<bool>(AadServicePrincipalConfigurationHelper.SyncServicePrincipalConfigPrototype), ConfigProxy.Create<bool>(AadServicePrincipalConfigurationHelper.GroupRulesServicePrincipalConfigPrototype), ConfigProxy.Create<bool>(AadServicePrincipalConfigurationHelper.UserHubSupportServicePrincipalsEnabledPrototype), ConfigProxy.Create<bool>(AadServicePrincipalConfigurationHelper.FixServicePrincipalsWithUnknownMetaTypePrototype), ConfigProxy.Create<bool>(AadServicePrincipalConfigurationHelper.OrgGroupMembershipSupportServicePrincipalsEnabledPrototype), ConfigProxy.Create<bool>(AadServicePrincipalConfigurationHelper.RestrictionOfCertainApisForServicePrincipalsEnabledPrototype), ConfigProxy.Create<bool>(AadServicePrincipalConfigurationHelper.AadBackedOrgAttributeEnalbedPrototype), ConfigProxy.Create<bool>(AadServicePrincipalConfigurationHelper.SPPermissionsOperationsInUIEnabledPrototype), ConfigProxy.Create<bool>(AadServicePrincipalConfigurationHelper.MergeUserAndSPPermissionsInUIEnabledPrototype), ConfigProxy.Create<bool>(AadServicePrincipalConfigurationHelper.ObjectLevelSecurityMembersSupportServicePrincipalsEnabledPrototype), ConfigProxy.Create<bool>(AadServicePrincipalConfigurationHelper.BranchPolicyStatusChecksSupportServicePrincipalsEnabledPrototype), ConfigProxy.Create<bool>(AadServicePrincipalConfigurationHelper.ArtifactsPermissionsSupportServicePrincipalsEnabledPrototype), ConfigProxy.Create<bool>(AadServicePrincipalConfigurationHelper.EmsPermissionsSupportServicePrincipalsEnabledPrototype), ConfigProxy.Create<bool>(AadServicePrincipalConfigurationHelper.AllowedSubjectTypesEnabledPrototype), ConfigProxy.Create<Dictionary<string, HashSet<Guid>>>(AadServicePrincipalConfigurationHelper.AadServicePrincipalRestrictionWhiteListPrototype))
    {
    }

    public AadServicePrincipalConfigurationHelper(
      IConfigQueryable<bool> aadSPTokenValidationEnabledConfig,
      IConfigQueryable<bool> aadSPCrudOperationsEnabledConfig,
      IConfigQueryable<bool> aadSPGraphOperationsEnabledConfig,
      IConfigQueryable<bool> searchingForServicePrincipalsEnabledConfig,
      IConfigQueryable<bool> syncServicePrincipalMembershipsEnabledConfig,
      IConfigQueryable<bool> groupRulesServicePrincipalMembershipsEnabledConfig,
      IConfigQueryable<bool> userHubSupporServicePrincipalsEnabledConfig,
      IConfigQueryable<bool> fixServicePrincipalsWithUnknownMetaTypeConfig,
      IConfigQueryable<bool> orgGroupMembershipSupportServicePrincipalsEnabledConfig,
      IConfigQueryable<bool> restrictionOfCertainApisForServicePrincipalsEnabledConfig,
      IConfigQueryable<bool> aadBackedOrgAttributeEnalbedConfig,
      IConfigQueryable<bool> spPermissionsOperationsInUIEnabledConfig,
      IConfigQueryable<bool> mergeUserAndSPPermissionsInUIEnabledConfig,
      IConfigQueryable<bool> objectLevelSecurityMembersSupportServicePrincipalsEnabledConfig,
      IConfigQueryable<bool> branchPolicyStatusChecksSupportServicePrincipalsEnabledConfig,
      IConfigQueryable<bool> artifactsPermissionsSupportServicePrincipalsEnabledConfig,
      IConfigQueryable<bool> emsPermissionsSupportServicePrincipalsEnabledConfig,
      IConfigQueryable<bool> allowedSubjectTypesEnabledConfig,
      IConfigQueryable<Dictionary<string, HashSet<Guid>>> aadServicePrincipalRestrictionWhiteListConfig)
    {
      this.AadSPTokenValidationEnabledConfig = aadSPTokenValidationEnabledConfig;
      this.AadSPCrudOperationsEnabledConfig = aadSPCrudOperationsEnabledConfig;
      this.AadSPGraphOperationsEnabledConfig = aadSPGraphOperationsEnabledConfig;
      this.SearchingForServicePrincipalsEnabledConfig = searchingForServicePrincipalsEnabledConfig;
      this.SyncServicePrincipalMembershipsEnabledConfig = syncServicePrincipalMembershipsEnabledConfig;
      this.GroupRulesServicePrincipalMembershipsEnabledConfig = groupRulesServicePrincipalMembershipsEnabledConfig;
      this.UserHubSupportServicePrincipalsEnabledConfig = userHubSupporServicePrincipalsEnabledConfig;
      this.FixServicePrincipalsWithUnknownMetaTypeConfig = fixServicePrincipalsWithUnknownMetaTypeConfig;
      this.OrgGroupMembershipSupportServicePrincipalsEnabledConfig = orgGroupMembershipSupportServicePrincipalsEnabledConfig;
      this.RestrictionOfCertainApisForServicePrincipalsEnabledConfig = restrictionOfCertainApisForServicePrincipalsEnabledConfig;
      this.AadBackedOrgAttributeEnalbedConfig = aadBackedOrgAttributeEnalbedConfig;
      this.SPPermissionsOperationsInUIEnabledConfig = spPermissionsOperationsInUIEnabledConfig;
      this.MergeUserAndSPPermissionsInUIEnabledConfig = mergeUserAndSPPermissionsInUIEnabledConfig;
      this.ObjectLevelSecurityMembersSupportServicePrincipalsEnabledConfig = objectLevelSecurityMembersSupportServicePrincipalsEnabledConfig;
      this.BranchPolicyStatusChecksSupportServicePrincipalsEnabledConfig = branchPolicyStatusChecksSupportServicePrincipalsEnabledConfig;
      this.ArtifactsPermissionsSupportServicePrincipalsEnabledConfig = artifactsPermissionsSupportServicePrincipalsEnabledConfig;
      this.EmsPermissionsSupportServicePrincipalsEnabledConfig = emsPermissionsSupportServicePrincipalsEnabledConfig;
      this.AllowedSubjectTypesEnabledConfig = allowedSubjectTypesEnabledConfig;
      this.AadServicePrincipalRestrictionWhiteListConfig = aadServicePrincipalRestrictionWhiteListConfig;
    }

    internal AadServicePrincipalConfigurationHelper(
      IConfigQueryable<bool> config,
      IConfigQueryable<Dictionary<string, HashSet<Guid>>> aadSpWhitelistConfig = null,
      ReadOnlyDictionary<string, HashSet<Guid>> staticAadSpWhitelistConfig = null)
      : this(config, config, config, config, config, config, config, config, config, config, config, config, config, config, config, config, config, config, aadSpWhitelistConfig)
    {
      AadServicePrincipalConfigurationHelper.s_aadServicePrincipalRestrictionPreWhiteList = staticAadSpWhitelistConfig;
    }

    public bool IsTokenValidationEnabled(IVssRequestContext requestContext) => this.AadSPTokenValidationEnabledConfig.QueryByCtx<bool>(requestContext);

    public bool IsEntitlementsApiEnabled(IVssRequestContext requestContext) => this.QueryConfig<bool>(requestContext, this.AadSPCrudOperationsEnabledConfig, nameof (IsEntitlementsApiEnabled));

    public bool IsAdoGraphApiEnabled(IVssRequestContext requestContext) => this.QueryConfig<bool>(requestContext, this.AadSPGraphOperationsEnabledConfig, nameof (IsAdoGraphApiEnabled));

    public bool IsSearchingForServicePrincipalsEnabled(IVssRequestContext requestContext) => this.QueryConfig<bool>(requestContext, this.SearchingForServicePrincipalsEnabledConfig, nameof (IsSearchingForServicePrincipalsEnabled));

    public bool IsUserHubSupportServicePrincipalsEnabled(IVssRequestContext requestContext) => this.QueryConfig<bool>(requestContext, this.UserHubSupportServicePrincipalsEnabledConfig, nameof (IsUserHubSupportServicePrincipalsEnabled));

    public bool IsSyncingForServicePrincipalsEnabled(IVssRequestContext requestContext) => this.QueryConfig<bool>(requestContext, this.SyncServicePrincipalMembershipsEnabledConfig, nameof (IsSyncingForServicePrincipalsEnabled));

    public bool IsGroupRulesForServicePrincipalsEnabled(IVssRequestContext requestContext) => this.QueryConfig<bool>(requestContext, this.GroupRulesServicePrincipalMembershipsEnabledConfig, nameof (IsGroupRulesForServicePrincipalsEnabled));

    public bool IsFixServicePrincipalsWithUnknownMetaTypeEnabled(
      IVssRequestContext requestContext,
      Guid oid)
    {
      return this.FixServicePrincipalsWithUnknownMetaTypeConfig.QueryById<bool>(requestContext, oid);
    }

    public bool IsOrgGroupMembershipSupportServicePrincipalsEnabled(
      IVssRequestContext requestContext)
    {
      return this.QueryConfig<bool>(requestContext, this.OrgGroupMembershipSupportServicePrincipalsEnabledConfig, nameof (IsOrgGroupMembershipSupportServicePrincipalsEnabled));
    }

    public bool IsRestrictionOfCertainApisForServicePrincipalsEnabled(
      IVssRequestContext requestContext)
    {
      return this.QueryConfig<bool>(requestContext, this.RestrictionOfCertainApisForServicePrincipalsEnabledConfig, nameof (IsRestrictionOfCertainApisForServicePrincipalsEnabled));
    }

    public bool IsRequireAadBackedOrgAttributeEnabled(IVssRequestContext requestContext) => this.QueryConfig<bool>(requestContext, this.AadBackedOrgAttributeEnalbedConfig, nameof (IsRequireAadBackedOrgAttributeEnabled));

    public bool AreSPPermissionsOperationsInUIEnabled(IVssRequestContext requestContext) => this.QueryConfig<bool>(requestContext, this.SPPermissionsOperationsInUIEnabledConfig, nameof (AreSPPermissionsOperationsInUIEnabled));

    public bool IsMergeUserAndSPPermissionsInUIEnabled(IVssRequestContext requestContext) => this.QueryConfig<bool>(requestContext, this.MergeUserAndSPPermissionsInUIEnabledConfig, nameof (IsMergeUserAndSPPermissionsInUIEnabled));

    public bool IsObjectLevelSecurityMembersSupportServicePrincipalsEnabled(
      IVssRequestContext requestContext)
    {
      return this.QueryConfig<bool>(requestContext, this.ObjectLevelSecurityMembersSupportServicePrincipalsEnabledConfig, nameof (IsObjectLevelSecurityMembersSupportServicePrincipalsEnabled));
    }

    public bool IsBranchPolicyStatusChecksSupportServicePrincipalsEnabled(
      IVssRequestContext requestContext)
    {
      return this.QueryConfig<bool>(requestContext, this.BranchPolicyStatusChecksSupportServicePrincipalsEnabledConfig, nameof (IsBranchPolicyStatusChecksSupportServicePrincipalsEnabled));
    }

    public bool IsArtifactsPermissionsSupportServicePrincipalsEnabled(
      IVssRequestContext requestContext)
    {
      return this.QueryConfig<bool>(requestContext, this.ArtifactsPermissionsSupportServicePrincipalsEnabledConfig, nameof (IsArtifactsPermissionsSupportServicePrincipalsEnabled));
    }

    public bool IsEmsPermissionsSupportServicePrincipalsEnabled(IVssRequestContext requestContext) => this.QueryConfig<bool>(requestContext, this.EmsPermissionsSupportServicePrincipalsEnabledConfig, nameof (IsEmsPermissionsSupportServicePrincipalsEnabled));

    public bool IsAllowedSubjectTypesEnabled(IVssRequestContext requestContext) => this.QueryConfig<bool>(requestContext, this.AllowedSubjectTypesEnabledConfig, nameof (IsAllowedSubjectTypesEnabled));

    public bool IsAadServicePrincipalWhitelistedForCategory(
      IVssRequestContext requestContext,
      string categoryName,
      Guid aadServicePrincipalCuid)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(categoryName, nameof (categoryName));
      ArgumentUtility.CheckForEmptyGuid(aadServicePrincipalCuid, nameof (aadServicePrincipalCuid));
      HashSet<Guid> guidSet1;
      if (AadServicePrincipalConfigurationHelper.s_aadServicePrincipalRestrictionPreWhiteList.TryGetValue(categoryName, out guidSet1) && guidSet1.Contains(aadServicePrincipalCuid))
        return true;
      HashSet<Guid> guidSet2 = (HashSet<Guid>) null;
      this.QueryConfig<Dictionary<string, HashSet<Guid>>>(requestContext, this.AadServicePrincipalRestrictionWhiteListConfig, nameof (IsAadServicePrincipalWhitelistedForCategory))?.TryGetValue(categoryName, out guidSet2);
      // ISSUE: explicit non-virtual call
      return guidSet2 != null && __nonvirtual (guidSet2.Contains(aadServicePrincipalCuid));
    }

    private T QueryConfig<T>(
      IVssRequestContext requestContext,
      IConfigQueryable<T> config,
      [CallerMemberName] string callerMemberName = null)
    {
      return requestContext.GetTenantId() == Guid.Empty ? this.QueryCustomQuery<T>(requestContext, config, callerMemberName) : config.QueryByCtx<T>(requestContext);
    }

    private T QueryCustomQuery<T>(
      IVssRequestContext requestContext,
      IConfigQueryable<T> config,
      string configName)
    {
      Guid organizationAadTenantId = requestContext.GetOrganizationAadTenantId();
      Guid hostId = Guid.Empty;
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        hostId = requestContext.ServiceHost.InstanceId;
      requestContext.Trace(37000009, TraceLevel.Warning, "ConfigFramework", nameof (AadServicePrincipalConfigurationHelper), string.Format("When checking {0}, RequestContext.GetTenantId() returned an empty guid, falling back to GetOrganizationAadTenantId(). GetOrganizationAadTenantId() returned: {1}.", (object) configName, (object) organizationAadTenantId));
      return config.Query(requestContext, new Query(requestContext.GetUserId(), hostId, organizationAadTenantId));
    }
  }
}
