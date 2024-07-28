// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IdentityHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server.Aad;
using Microsoft.VisualStudio.Services.Aad;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Directories;
using Microsoft.VisualStudio.Services.Directories.DirectoryService;
using Microsoft.VisualStudio.Services.Directories.Telemetry;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.NameResolution;
using Microsoft.VisualStudio.Services.NameResolution.Server;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.VisualStudio.Services.Profile;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Jwt;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class IdentityHelper
  {
    private static readonly IDictionary<FrameworkIdentityType, string> s_IdentityTypeNameMap = (IDictionary<FrameworkIdentityType, string>) new Dictionary<FrameworkIdentityType, string>()
    {
      {
        FrameworkIdentityType.ServiceIdentity,
        "Microsoft.TeamFoundation.ServiceIdentity"
      },
      {
        FrameworkIdentityType.AggregateIdentity,
        "Microsoft.TeamFoundation.AggregateIdentity"
      },
      {
        FrameworkIdentityType.ImportedIdentity,
        "Microsoft.TeamFoundation.ImportedIdentity"
      }
    };
    private static readonly char[] s_trimCharacters = new char[2]
    {
      '-',
      ' '
    };
    private const string c_globalScope = "[SERVER]";
    private const string c_bindPendingEmailAddressPrefix = "email:";
    private const string c_ServiceUserDomain = "TEAM FOUNDATION";
    private const string c_windowsSystemSid = "S-1-5-18";
    private const string c_windowsLocalServiceSid = "S-1-5-19";
    private const string c_windowsNetworkServiceSid = "S-1-5-20";
    private const string c_windowsRemoteComputerSuffix = "$";
    private const string c_authenticatedUser = ".";
    internal const string c_FeatureSkipAadDiscovery = "VisualStudio.Services.Directory.RemoveDeploymentAadBp.SkipAadDiscovery";
    internal const string c_FeatureDisableSubjectDescriptorPermissionCheck = "VisualStudio.Services.DisableSubjectDescriptorPermissionCheck";
    private const string c_FeatureEnableFrameworkIdentityInCollectionLevelOnly = "VisualStudio.Services.Identity.EnableFrameworkIdentityInCollectionLevelOnly";
    private const string c_FeatureEnableFrameworkAccountNameEmailAddressFallback = "VisualStudio.Services.Identity.EnableFrameworkAccountNameEmailAddressFallback";
    private const string c_FeatureUseQualifiedAccountNameForMSA = "Microsoft.VisualStudio.Services.Identity.UseQualifiedAccountNameForMSA";
    private const string c_FeatureDisableMaterializeUserBySubjectDescriptor = "VisualStudio.Services.Identity.DisableMaterializeUserBySubjectDescriptor";
    internal const string c_domainQualifiedAccountNameFormat = "{0}\\{1}";
    private const string c_enterpriseUsersAadOIDGroupMPSKey = "AADGroupOid-MicrosoftTenant";
    private const string c_enterpriseUsersAadGroup = "EnterpriseUsersAadGroup";
    private const string c_checkAllowTeamAdminMaterialization = "Microsoft.VisualStudio.Services.Identity.teamAdminMaterialization";
    private const string c_checkLogMsftAllAccountsDeactivation = "Microsoft.VisualStudio.Services.Identity.LogMsftAllAccountsDeactivation";
    private static readonly string c_AddMemberMethodName = "AddMember";
    private static readonly Guid c_genevaActionServiceInstanceType = new Guid("0000005B-0000-8888-8000-000000000000");
    private const string s_area = "IdentityService";
    private const string s_layer = "IdentityHelper";

    public static bool IdentityCanRecieveMail(IReadOnlyVssIdentity identity) => identity.CanReceiveMail();

    public static string GetPreferredEmailAddress(
      IVssRequestContext requestContext,
      Guid identityId,
      bool confirmed = true)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(identityId, nameof (identityId));
      string defaultMailAddress;
      string customEmailAddress;
      string confirmedEmailAddress;
      string accountName;
      if (!IdentityHelper.TryGetIdentityMailAddresses(requestContext, identityId, out defaultMailAddress, out customEmailAddress, out confirmedEmailAddress, out accountName))
        throw new IdentityNotFoundException(identityId);
      requestContext.Trace(80234, TraceLevel.Info, "IdentityService", nameof (IdentityHelper), "Read email addresses for identity '{0}' (defaultMailAddress='{1}', confirmedEmailAddress='{2}',customEmailAddress='{3}')", (object) identityId, (object) string.Equals(accountName, defaultMailAddress, StringComparison.CurrentCultureIgnoreCase), (object) string.Equals(accountName, confirmedEmailAddress, StringComparison.CurrentCultureIgnoreCase), (object) string.Equals(accountName, customEmailAddress, StringComparison.CurrentCultureIgnoreCase));
      if (string.IsNullOrEmpty(customEmailAddress) || !string.Equals(customEmailAddress, confirmedEmailAddress, StringComparison.CurrentCultureIgnoreCase) & confirmed)
      {
        requestContext.Trace(80234, TraceLevel.Info, "IdentityService", nameof (IdentityHelper), "Read email for identity '{0}'. Use defaultEmailAddress", (object) identityId);
        customEmailAddress = defaultMailAddress;
      }
      if (string.IsNullOrWhiteSpace(customEmailAddress) && requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.EnableFrameworkAccountNameEmailAddressFallback") && !string.IsNullOrEmpty(accountName))
      {
        bool flag = ArgumentUtility.IsValidEmailAddress(accountName);
        requestContext.Trace(80234, TraceLevel.Info, "IdentityService", nameof (IdentityHelper), "Read email for identity '{0}'. Use accountName ({1})", (object) identityId, (object) flag);
        customEmailAddress = flag ? accountName : customEmailAddress;
      }
      return customEmailAddress;
    }

    public static bool IsEmailConfirmationPending(
      IVssRequestContext requestContext,
      Guid identityId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(identityId, nameof (identityId));
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return false;
      string defaultMailAddress;
      string customEmailAddress;
      string confirmedEmailAddress;
      if (!IdentityHelper.TryGetIdentityMailAddresses(requestContext, identityId, out defaultMailAddress, out customEmailAddress, out confirmedEmailAddress, out string _))
        throw new IdentityNotFoundException(identityId);
      return !string.IsNullOrEmpty(customEmailAddress) && !string.Equals(customEmailAddress, defaultMailAddress, StringComparison.CurrentCultureIgnoreCase) && !string.Equals(customEmailAddress, confirmedEmailAddress, StringComparison.CurrentCultureIgnoreCase);
    }

    private static bool TryGetIdentityMailAddresses(
      IVssRequestContext requestContext,
      Guid id,
      out string defaultMailAddress,
      out string customEmailAddress,
      out string confirmedEmailAddress,
      out string accountName)
    {
      customEmailAddress = string.Empty;
      confirmedEmailAddress = string.Empty;
      defaultMailAddress = string.Empty;
      accountName = string.Empty;
      Microsoft.VisualStudio.Services.Identity.Identity readIdentity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<Guid>) new Guid[1]
      {
        id
      }, QueryMembership.None, (IEnumerable<string>) new string[2]
      {
        "CustomNotificationAddresses",
        "ConfirmedNotificationAddress"
      })[0];
      if (readIdentity == null)
        return false;
      customEmailAddress = readIdentity.GetProperty<string>("CustomNotificationAddresses", (string) null);
      accountName = readIdentity.GetProperty<string>("Account", (string) null);
      confirmedEmailAddress = readIdentity.GetProperty<string>("ConfirmedNotificationAddress", (string) null);
      defaultMailAddress = readIdentity.GetProperty<string>("Mail", string.Empty);
      return true;
    }

    public static bool TryGetDirectoryEntityDescriptor(
      string accountName,
      string originDirectory,
      Guid? tenantId,
      IDictionary<string, object> properties,
      string invitationFlowName,
      bool skipAadDiscovery,
      string entityOrigin,
      out IDirectoryEntityDescriptor entityDescriptor,
      string entityType = null)
    {
      string str1 = (string) null;
      if (originDirectory != "ghb" && "ServicePrincipal" != entityType)
      {
        if (string.IsNullOrWhiteSpace(accountName))
        {
          entityDescriptor = (IDirectoryEntityDescriptor) null;
          return false;
        }
        str1 = accountName;
      }
      string s = (string) null;
      if (properties != null && properties.ContainsKey("http://schemas.microsoft.com/identity/claims/objectidentifier"))
      {
        if (originDirectory == "ghb")
        {
          properties.TryGetValue<string>("http://schemas.microsoft.com/identity/claims/objectidentifier", out s);
          if (string.IsNullOrEmpty(s) || !int.TryParse(s, out int _))
          {
            entityDescriptor = (IDirectoryEntityDescriptor) null;
            return false;
          }
        }
        else
        {
          Guid guid;
          if (properties.TryGetGuid("http://schemas.microsoft.com/identity/claims/objectidentifier", out guid))
          {
            s = guid.ToString();
          }
          else
          {
            entityDescriptor = (IDirectoryEntityDescriptor) null;
            return false;
          }
        }
      }
      ref IDirectoryEntityDescriptor local = ref entityDescriptor;
      string entityType1 = entityType;
      string str2 = s;
      string str3 = str1;
      string originDirectory1 = originDirectory;
      string originId = str2;
      string principalName = str3;
      DirectoryEntityDescriptor entityDescriptor1 = new DirectoryEntityDescriptor(entityType1, originDirectory1, originId, principalName: principalName, properties: (IReadOnlyDictionary<string, object>) new Dictionary<string, object>()
      {
        {
          "InvitationMethod",
          (object) invitationFlowName
        },
        {
          "SkipAadDiscovery",
          (object) skipAadDiscovery
        },
        {
          "TenantId",
          (object) tenantId
        },
        {
          "EntityOrigin",
          (object) entityOrigin
        }
      });
      local = (IDirectoryEntityDescriptor) entityDescriptor1;
      return true;
    }

    private static bool TryParseOriginDirectoryAndTenant(
      string domain,
      out string originDirectory,
      out Guid? tenantId)
    {
      if (!IdentityHelper.IsValidUserDomain(domain))
      {
        originDirectory = (string) null;
        tenantId = new Guid?();
        return false;
      }
      if (string.Equals(domain, "Windows Live ID", StringComparison.OrdinalIgnoreCase))
      {
        originDirectory = "vsd";
        tenantId = new Guid?(Guid.Empty);
      }
      else if (string.Equals(domain, "github.com", StringComparison.OrdinalIgnoreCase))
      {
        originDirectory = "ghb";
        tenantId = new Guid?(Guid.Empty);
      }
      else
      {
        originDirectory = "aad";
        tenantId = new Guid?(Guid.Parse(domain));
      }
      return true;
    }

    internal static bool TryParseEntityOrigin(string domain, out string entityOrigin)
    {
      if (!IdentityHelper.IsValidUserDomain(domain))
      {
        entityOrigin = (string) null;
        return false;
      }
      entityOrigin = !string.Equals(domain, "Windows Live ID", StringComparison.OrdinalIgnoreCase) ? (!string.Equals(domain, "github.com", StringComparison.OrdinalIgnoreCase) ? "aad" : "ghb") : "msa";
      return true;
    }

    public static Microsoft.VisualStudio.Services.Identity.Identity GetOrCreateBindPendingIdentity(
      IVssRequestContext requestContext,
      string domain,
      string accountName,
      string puid = null,
      IDictionary<string, object> properties = null,
      bool forceEntityToMatchDomain = false,
      [CallerMemberName] string callerName = null)
    {
      IVssRequestContext scopedRequestContext = IdentityHelper.GetScopedRequestContext(requestContext, requestContext.Elevate());
      var callerParameters = new
      {
        domain = domain,
        accountName = accountName,
        puid = puid,
        properties = properties,
        callerName = callerName
      };
      scopedRequestContext.CheckHostedDeployment();
      bool flag1 = scopedRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment);
      Guid organizationAadTenantId = scopedRequestContext.GetOrganizationAadTenantId();
      string str = organizationAadTenantId != Guid.Empty ? organizationAadTenantId.ToString() : "Windows Live ID";
      string originDirectory = (string) null;
      Guid? tenantId = new Guid?();
      bool flag2 = !string.Equals(str.Trim(), domain.Trim(), StringComparison.OrdinalIgnoreCase);
      if (flag2 | flag1 && !IdentityHelper.TryParseOriginDirectoryAndTenant(domain, out originDirectory, out tenantId))
        throw new ArgumentException(FrameworkResources.InvalidIdentityDomain(), nameof (domain));
      bool skipAadDiscovery = !flag1 & flag2 && scopedRequestContext.IsImsFeatureEnabled("VisualStudio.Services.Directory.RemoveDeploymentAadBp.SkipAadDiscovery");
      string invitationFlowName = DirectoryEntityInvitationMethod.QualifyInvitationMethod(nameof (GetOrCreateBindPendingIdentity), callerName);
      string entityOrigin = (string) null;
      if (forceEntityToMatchDomain)
        IdentityHelper.TryParseEntityOrigin(domain, out entityOrigin);
      IDirectoryEntityDescriptor entityDescriptor;
      if (!IdentityHelper.TryGetDirectoryEntityDescriptor(accountName, originDirectory, tenantId, properties, invitationFlowName, skipAadDiscovery, entityOrigin, out entityDescriptor))
        IdentityHelper.ThrowWithTrace(requestContext, accountName, 80851, string.Format("Cannot call {0} because an input parameter is invalid for caller parameters: {1}", (object) IdentityHelper.c_AddMemberMethodName, (object) callerParameters));
      if (scopedRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Identity.teamAdminMaterialization") && !scopedRequestContext.GetService<IOrganizationPolicyService>().GetPolicy<bool>(scopedRequestContext, "Policy.AllowTeamAdminsInvitationsAccessToken", true).EffectiveValue)
        IdentityHelper.ValidatePermissionsToMaterializeMember(requestContext, entityDescriptor);
      IdentityDirectoryEntityResult<Microsoft.VisualStudio.Services.Identity.Identity> directoryEntityResult = scopedRequestContext.GetService<IDirectoryService>().IncludeIdentities(scopedRequestContext).AddMember(scopedRequestContext, entityDescriptor, license: "None");
      IdentityHelper.ValidateAddMemberOperationResults(requestContext, accountName, directoryEntityResult, (object) callerParameters);
      if (flag2 && !string.Equals(directoryEntityResult.Identity.GetProperty<string>("Domain", string.Empty), domain, StringComparison.OrdinalIgnoreCase))
      {
        scopedRequestContext.TraceSerializedConditionally(80856, TraceLevel.Info, "IdentityService", nameof (IdentityHelper), "AddMember re-read at org level to skip translation for caller parameters: {0}", (object) callerParameters);
        IVssRequestContext vssRequestContext = scopedRequestContext.To(TeamFoundationHostType.Application);
        return vssRequestContext.GetService<IdentityService>().ReadIdentities(vssRequestContext, (IList<Guid>) new Guid[1]
        {
          directoryEntityResult.Identity.Id
        }, QueryMembership.None, (IEnumerable<string>) null).First<Microsoft.VisualStudio.Services.Identity.Identity>();
      }
      scopedRequestContext.TraceSerializedConditionally(80858, TraceLevel.Verbose, "IdentityService", nameof (IdentityHelper), "AddMember completed successfully for caller parameters: {0}", (object) callerParameters);
      return directoryEntityResult.Identity;
    }

    public static void ConvertToBindPending(Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      string property = identity.GetProperty<string>("Account", (string) null);
      identity.Descriptor = IdentityHelper.CreateDescriptorFromAccountName("Windows Live ID", property, true);
      identity.SetProperty("Domain", (object) "Windows Live ID");
      identity.SetProperty("http://schemas.microsoft.com/identity/claims/objectidentifier", (object) null);
      DateTime utcNow;
      if (!identity.Properties.TryGetValue<DateTime>("MetadataUpdateDate", out utcNow))
        utcNow = DateTime.UtcNow;
      identity.SetProperty("MetadataUpdateDate", (object) utcNow.AddSeconds(1.0));
      identity.SetProperty("PUID", (object) string.Empty);
    }

    public static void ValidateAddMemberOperationResults(
      IVssRequestContext requestContext,
      string accountName,
      IdentityDirectoryEntityResult<Microsoft.VisualStudio.Services.Identity.Identity> directoryEntityResult,
      object callerParameters)
    {
      if (directoryEntityResult == null)
        IdentityHelper.ThrowWithTrace(requestContext, accountName, 80852, IdentityHelper.c_AddMemberMethodName + " returned a null result for caller parameters: {0}", callerParameters);
      if (directoryEntityResult.Exception != null)
        IdentityHelper.ThrowWithTrace(requestContext, accountName, 80853, string.Format("{0} returned exception {1} for caller parameters: {{0}}", (object) IdentityHelper.c_AddMemberMethodName, (object) directoryEntityResult.Exception), callerParameters);
      if (directoryEntityResult.Status != "Success")
        IdentityHelper.ThrowWithTrace(requestContext, accountName, 80854, IdentityHelper.c_AddMemberMethodName + " returned non-success status " + directoryEntityResult.Status + " for caller parameters: {0}", callerParameters);
      if (directoryEntityResult.Identity != null)
        return;
      IdentityHelper.ThrowWithTrace(requestContext, accountName, 80855, IdentityHelper.c_AddMemberMethodName + " returned a null identity for caller parameters: {0}", callerParameters);
    }

    private static void ThrowWithTrace(
      IVssRequestContext requestContext,
      string accountName,
      int tracepoint,
      string message,
      params object[] callerParameters)
    {
      requestContext.TraceSerializedConditionally(tracepoint, TraceLevel.Error, "IdentityService", nameof (IdentityHelper), message, callerParameters);
      throw new IdentityMaterializationFailedException(accountName);
    }

    private static IVssRequestContext GetScopedRequestContext(
      IVssRequestContext requestContext,
      IVssRequestContext elevatedContext)
    {
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      return userIdentity != null && userIdentity.SocialDescriptor != new SocialDescriptor() && userIdentity.SocialDescriptor.IsGitHubSocialType() ? requestContext : elevatedContext;
    }

    public static Microsoft.VisualStudio.Services.Identity.Identity GetOrCreateBindPendingIdentity(
      IVssRequestContext requestContext,
      string domain,
      string accountName,
      out bool createdIdentity,
      string puid = null,
      IDictionary<string, object> properties = null)
    {
      string factorValue = accountName.Contains("\\") ? accountName : string.Format("{0}\\{1}", (object) domain, (object) accountName);
      Microsoft.VisualStudio.Services.Identity.Identity bindPendingIdentity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, IdentitySearchFilter.AccountName, factorValue, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (bindPendingIdentity != null)
      {
        createdIdentity = false;
        return bindPendingIdentity;
      }
      createdIdentity = true;
      return IdentityHelper.GetOrCreateBindPendingIdentity(requestContext, domain, accountName, puid, properties, callerName: nameof (GetOrCreateBindPendingIdentity));
    }

    internal static Microsoft.VisualStudio.Services.Identity.Identity CreateBindPendingIdentity(
      string domain,
      string accountName,
      IDictionary<string, object> properties = null)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity1 = new Microsoft.VisualStudio.Services.Identity.Identity();
      identity1.Descriptor = IdentityHelper.CreateDescriptorFromAccountName(domain, accountName, true);
      identity1.ProviderDisplayName = accountName;
      Microsoft.VisualStudio.Services.Identity.Identity identity2 = identity1;
      if (string.Equals(domain, "Windows Live ID", StringComparison.OrdinalIgnoreCase))
        identity2.SetProperty("Mail", (object) accountName);
      identity2.SetProperty("Domain", (object) domain);
      identity2.SetProperty("Account", (object) accountName);
      IdentityHelper.SetProperties((IVssIdentity) identity2, properties);
      return identity2;
    }

    public static string GetIdentityType(IIdentity identity)
    {
      if (!(identity is ClaimsIdentity claimsIdentity))
        return identity.GetType().FullName;
      if (claimsIdentity is WindowsIdentity)
        return "System.Security.Principal.WindowsIdentity";
      if (claimsIdentity.Claims.Any<Claim>((Func<Claim, bool>) (x => StringComparer.OrdinalIgnoreCase.Equals(x.Type, "CspPartner"))))
        return "Microsoft.TeamFoundation.Claims.CspPartnerIdentity";
      Claim first = claimsIdentity.FindFirst((Predicate<Claim>) (claim => claim.Type == "IdentityTypeClaim"));
      return first == null || string.Equals(first.Value, "System.Security.Principal.WindowsIdentity", StringComparison.InvariantCultureIgnoreCase) ? identity.GetType().FullName : first.Value;
    }

    public static bool IsFpmsa(this IReadOnlyVssIdentity identity)
    {
      if (identity == null || "Windows Live ID".Equals(identity.GetProperty<string>("Domain", string.Empty)))
        return false;
      string property = identity.GetProperty<string>("PUID", string.Empty);
      return !(property == string.Empty) && !property.StartsWith("aad:");
    }

    public static bool IsMsaOrFpmsa(this IReadOnlyVssIdentity identity)
    {
      if (identity == null)
        return false;
      if ("Windows Live ID".Equals(identity.GetProperty<string>("Domain", string.Empty)))
        return true;
      string property = identity.GetProperty<string>("PUID", string.Empty);
      return !(property == string.Empty) && !property.StartsWith("aad:");
    }

    public static bool IsMsa(this IReadOnlyVssIdentity identity) => identity != null && "Windows Live ID".Equals(identity.GetProperty<string>("Domain", string.Empty));

    public static bool IsBuildIdentity(this IReadOnlyVssIdentity identity)
    {
      string role;
      return identity.Descriptor.IsServiceIdentityType() && IdentityHelper.TryParseFrameworkServiceIdentityDescriptor(identity.Descriptor, out Guid _, out role, out string _) && string.Equals("Build", role, StringComparison.OrdinalIgnoreCase);
    }

    public static IdentityDescriptor CreateFrameworkIdentityDescriptor(
      FrameworkIdentityType identityType,
      Guid scopeId,
      string role,
      string identifier)
    {
      ArgumentUtility.CheckForEmptyGuid(scopeId, nameof (scopeId));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(role, nameof (role));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(identifier, nameof (identifier));
      return new IdentityDescriptor(IdentityHelper.s_IdentityTypeNameMap[identityType], string.Format("{0}{1}{2}{1}{3}", (object) scopeId.ToString(), (object) ":", (object) role, (object) identifier));
    }

    public static IdentityDescriptor CreateApplicationPrincipalIdentityDescriptor(
      IVssRequestContext requestContext,
      Guid registrationId)
    {
      ArgumentUtility.CheckForEmptyGuid(registrationId, nameof (registrationId));
      return IdentityHelper.CreateFrameworkIdentityDescriptor(FrameworkIdentityType.ServiceIdentity, requestContext.ServiceHost.InstanceId, "Application", registrationId.ToString("D"));
    }

    public static Microsoft.VisualStudio.Services.Identity.Identity GetApplicationPrincipalIdentity(
      IVssRequestContext requestContext,
      Guid registrationId)
    {
      return IdentityHelper.GetFrameworkIdentities(requestContext, FrameworkIdentityType.ServiceIdentity, "Application", (IList<string>) new string[1]
      {
        registrationId.ToString("D")
      }, (requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) ? 1 : 0) != 0).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
    }

    public static Microsoft.VisualStudio.Services.Identity.Identity GetFrameworkIdentity(
      IVssRequestContext requestContext,
      FrameworkIdentityType identityType,
      string role,
      string identifier,
      bool noSidFallback = false)
    {
      return IdentityHelper.GetFrameworkIdentities(requestContext, identityType, role, (IList<string>) new string[1]
      {
        identifier
      }, (noSidFallback ? 1 : 0) != 0).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
    }

    public static IList<Microsoft.VisualStudio.Services.Identity.Identity> GetFrameworkIdentities(
      IVssRequestContext requestContext,
      FrameworkIdentityType identityType,
      string role,
      IList<string> identifiers,
      bool noSidFallback = false)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) identifiers, nameof (identifiers));
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) && requestContext.ExecutionEnvironment.IsHostedDeployment)
        requestContext.TraceSerializedConditionally(80230, TraceLevel.Warning, "IdentityService", nameof (IdentityHelper), "Still trying to get the framework identity other than collection level. identityType:{0}, role:{1}, identifier:{2}", (object) identityType, (object) role, (object) identifiers[0]);
      Guid scopeId = noSidFallback ? requestContext.ServiceHost.InstanceId : IdentityHelper.GetRequestContextForFrameworkIdentity(requestContext).ServiceHost.InstanceId;
      List<IdentityDescriptor> list = identifiers.Select<string, IdentityDescriptor>((Func<string, IdentityDescriptor>) (x => IdentityHelper.CreateFrameworkIdentityDescriptor(identityType, scopeId, role, x))).ToList<IdentityDescriptor>();
      return requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<IdentityDescriptor>) list, QueryMembership.None, (IEnumerable<string>) null);
    }

    public static IVssRequestContext GetRequestContextForFrameworkIdentity(
      IVssRequestContext collectionContext)
    {
      if (!collectionContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        return collectionContext;
      IVssRequestContext frameworkIdentity;
      if (!collectionContext.IsFeatureEnabled("VisualStudio.Services.Identity.EnableFrameworkIdentityInCollectionLevelOnly"))
      {
        frameworkIdentity = collectionContext.To(TeamFoundationHostType.Application);
        collectionContext.TraceConditionally(80231, TraceLevel.Verbose, "IdentityService", nameof (IdentityHelper), (Func<string>) (() => "request context switch to organization level"));
      }
      else
      {
        frameworkIdentity = collectionContext;
        collectionContext.TraceConditionally(80232, TraceLevel.Verbose, "IdentityService", nameof (IdentityHelper), (Func<string>) (() => "request context remain unchanged"));
      }
      return frameworkIdentity;
    }

    public static bool TryParseFrameworkServiceIdentityDescriptor(
      IdentityDescriptor descriptor,
      out Guid scopeId,
      out string role,
      out string identifier)
    {
      scopeId = Guid.Empty;
      role = (string) null;
      identifier = (string) null;
      if ((object) descriptor == null || descriptor.IdentityType == null)
        return false;
      string[] strArray = descriptor.Identifier.Split(new string[1]
      {
        ":"
      }, StringSplitOptions.None);
      if (strArray.Length != 3 || !Guid.TryParse(strArray[0], out scopeId))
        return false;
      role = strArray[1];
      identifier = strArray[2];
      return true;
    }

    public static void LogInvalidServiceIdentityWhenNecessary(
      IVssRequestContext targetRequestContext,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> Identities,
      bool alwaysTracing = false)
    {
      if (targetRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) || !alwaysTracing && !targetRequestContext.IsTracing(80237, TraceLevel.Info, "IdentityService", nameof (IdentityHelper)))
        return;
      ArgumentUtility.CheckForNull<IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>>(Identities, nameof (Identities), "IdentityService");
      TraceLevel level = alwaysTracing ? TraceLevel.Error : TraceLevel.Warning;
      IVssRequestContext vssRequestContext = targetRequestContext.To(TeamFoundationHostType.Application);
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in Identities)
      {
        IdentityDescriptor descriptor = identity?.Descriptor;
        if (!(descriptor == (IdentityDescriptor) null) && descriptor.IsServiceIdentityType())
        {
          Guid scopeId;
          string identifier;
          if (!IdentityHelper.TryParseFrameworkServiceIdentityDescriptor(descriptor, out scopeId, out string _, out identifier))
          {
            targetRequestContext.TraceSerializedConditionally(80237, level, "IdentityService", nameof (IdentityHelper), string.Format("Expected a build service identity descriptor to be of the format 'scopeId:Build:localIdentifier', but instead found it to be: {0} with Id {1}", (object) descriptor, (object) identity.Id));
          }
          else
          {
            if (scopeId != vssRequestContext.ServiceHost.InstanceId)
            {
              if (targetRequestContext.ServiceHost.HostType == TeamFoundationHostType.ProjectCollection)
              {
                if (scopeId != targetRequestContext.ServiceHost.InstanceId)
                  targetRequestContext.TraceSerializedConditionally(80237, level, "IdentityService", nameof (IdentityHelper), string.Format("Expected the scope id {0} to be either organization host id {1} or collection host id {2}. But instead found descriptor to be: {3} with Id {4}", (object) scopeId, (object) vssRequestContext.ServiceHost.InstanceId, (object) targetRequestContext.ServiceHost.InstanceId, (object) descriptor, (object) identity.Id));
              }
              else
                targetRequestContext.TraceSerializedConditionally(80237, level, "IdentityService", nameof (IdentityHelper), string.Format("Expected the scope id {0} to be organization host id {1}. But instead found descriptor to be: {2} with Id {3}", (object) scopeId, (object) vssRequestContext.ServiceHost.InstanceId, (object) descriptor, (object) identity.Id));
            }
            Guid result;
            if (!Guid.TryParse(identifier, out result) || result == Guid.Empty)
              targetRequestContext.TraceSerializedConditionally(80237, level, "IdentityService", nameof (IdentityHelper), string.Format("Expected the identifier {0} to be a non-GuidEmpty value. But instead found descriptor to be: {1} with Id {2}", (object) identifier, (object) descriptor, (object) identity.Id));
          }
        }
      }
    }

    public static IdentityDescriptor CreateDescriptorFromAccountName(
      string domain,
      string accountName,
      bool bindPending = false)
    {
      string identityType = bindPending ? "Microsoft.TeamFoundation.BindPendingIdentity" : "Microsoft.IdentityModel.Claims.ClaimsIdentity";
      string identifierPrefix = bindPending ? "upn:" : string.Empty;
      return IdentityHelper.CreateDescriptorFromAccountName(domain, accountName, identityType, identifierPrefix);
    }

    public static IdentityDescriptor CreateDescriptorFromAccountName(
      string domain,
      string accountName,
      string identityType,
      string identifierPrefix)
    {
      if (string.IsNullOrWhiteSpace(domain))
        throw new ArgumentException(TFCommonResources.EmptyStringNotAllowed(), nameof (domain));
      if (string.IsNullOrWhiteSpace(accountName))
        throw new ArgumentException(TFCommonResources.EmptyStringNotAllowed(), nameof (accountName));
      if ("Windows Live ID".Equals(domain) && identifierPrefix != "upn:")
        throw new ArgumentException("PUID is required for Identity descriptor type Windows Live ID.");
      if (!IdentityHelper.IsValidUserDomain(domain))
        throw new ArgumentException(FrameworkResources.InvalidIdentityDomain(), nameof (domain));
      string identifier = identifierPrefix + domain + (object) '\\' + accountName;
      return new IdentityDescriptor(identityType, identifier);
    }

    public static IdentityDescriptor CreateAadServicePrincipalDescriptor(
      string domain,
      string objectId)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(domain, nameof (domain));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(objectId, nameof (objectId));
      return new IdentityDescriptor("Microsoft.VisualStudio.Services.Claims.AadServicePrincipal", string.Format("{0}\\{1}", (object) domain, (object) objectId));
    }

    public static void RetrieveDomainAndAccountNameFromBindPendingIdentifier(
      string identifier,
      out string domain,
      out string accountName)
    {
      domain = (string) null;
      accountName = (string) null;
      string message = FrameworkResources.InvalidBindPendingIdentityDescriptorErrorV2((object) identifier);
      string str = identifier.StartsWith("upn:") ? identifier.Substring("upn:".Length) : throw new InvalidBindPendingIdentityDescriptorException(message);
      string[] strArray = !string.IsNullOrWhiteSpace(str) ? str.Split('\\') : throw new InvalidBindPendingIdentityDescriptorException(message);
      if (strArray.Length != 2)
        throw new InvalidBindPendingIdentityDescriptorException(message);
      domain = IdentityHelper.IsValidUserDomain(strArray[0]) && !string.IsNullOrWhiteSpace(strArray[1]) ? strArray[0] : throw new InvalidBindPendingIdentityDescriptorException(message);
      accountName = strArray[1];
    }

    public static void RetrieveDomainAndAccountNameFromBindPendingDescriptor(
      IdentityDescriptor identityDescriptor,
      out string domain,
      out string accountName)
    {
      domain = (string) null;
      accountName = (string) null;
      if (!(identityDescriptor.IdentityType == "Microsoft.TeamFoundation.BindPendingIdentity"))
        return;
      IdentityHelper.RetrieveDomainAndAccountNameFromBindPendingIdentifier(identityDescriptor.Identifier, out domain, out accountName);
    }

    internal static void MungeIdentity(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      string prefix)
    {
      requestContext.Trace(80914, TraceLevel.Info, "IdentityService", nameof (IdentityHelper), string.Format("Munging identity: {0} with prefix: {1}", (object) identity.Descriptor, (object) prefix));
      string property = identity.GetProperty<string>("Account", (string) null);
      string str1 = string.Empty;
      IdentityDescriptor identityDescriptor;
      if (identity.IsMsa())
      {
        identityDescriptor = IdentityHelper.GetMungedMsaDescriptor(identity, prefix);
        if (!string.IsNullOrEmpty(property))
          str1 = string.Format("{0}_{1}_{2}", (object) prefix, (object) identity.Id, (object) property);
      }
      else
      {
        identityDescriptor = IdentityHelper.GetMungedAadDescriptor(identity, prefix);
        string str2 = identity.GetProperty<Guid>("http://schemas.microsoft.com/identity/claims/objectidentifier", new Guid()).ToString();
        Guid zero = GuidUtils.Change14thCharToZero(Guid.NewGuid());
        identity.SetProperty("http://schemas.microsoft.com/identity/claims/objectidentifier", (object) zero);
        if (!string.IsNullOrEmpty(property))
          str1 = prefix + "_" + str2 + "_" + property;
      }
      DateTime utcNow;
      if (!identity.Properties.TryGetValue<DateTime>("MetadataUpdateDate", out utcNow))
        utcNow = DateTime.UtcNow;
      identity.SetProperty("MetadataUpdateDate", (object) utcNow.AddSeconds(1.0));
      identity.SetProperty("PUID", (object) string.Empty);
      identity.Descriptor = identityDescriptor;
      identity.SetProperty("Account", (object) str1);
    }

    private static IdentityDescriptor GetMungedMsaDescriptor(Microsoft.VisualStudio.Services.Identity.Identity identity, string prefix)
    {
      string[] strArray = identity.Descriptor.Identifier.Split('\\');
      if (strArray.Length <= 1)
        return new IdentityDescriptor("Microsoft.TeamFoundation.ImportedIdentity", string.Format("{0}_{1}_{2}", (object) prefix, (object) identity.Id, (object) identity.Descriptor.Identifier));
      string str1 = strArray[0];
      string str2 = strArray[1];
      return new IdentityDescriptor("Microsoft.TeamFoundation.ImportedIdentity", string.Format("{0}\\{1}_{2}_{3}", (object) str1, (object) prefix, (object) identity.Id, (object) str2));
    }

    private static IdentityDescriptor GetMungedAadDescriptor(Microsoft.VisualStudio.Services.Identity.Identity identity, string prefix)
    {
      string[] strArray = identity.Descriptor.Identifier.Split('\\');
      if (strArray.Length <= 1)
        return new IdentityDescriptor("Microsoft.TeamFoundation.ImportedIdentity", string.Format("{0}_{1}_{2}", (object) prefix, (object) identity.Id, (object) identity.Descriptor.Identifier));
      string str1 = strArray[0];
      string str2 = strArray[1];
      string str3 = identity.GetProperty<Guid>("http://schemas.microsoft.com/identity/claims/objectidentifier", new Guid()).ToString();
      return new IdentityDescriptor("Microsoft.TeamFoundation.ImportedIdentity", str1 + "\\" + prefix + "_" + str3 + "_" + str2);
    }

    public static bool IsValidBindPendingDescriptor(IdentityDescriptor descriptor) => descriptor != (IdentityDescriptor) null && descriptor.IdentityType == "Microsoft.TeamFoundation.BindPendingIdentity" && descriptor.Identifier.StartsWith("upn:");

    public static bool IsValidLegacyBindPendingDescriptor(IdentityDescriptor descriptor) => descriptor != (IdentityDescriptor) null && descriptor.IdentityType == "Microsoft.TeamFoundation.BindPendingIdentity" && descriptor.Identifier.StartsWith("email:");

    public static bool IsValidUserDomain(string domain) => "Windows Live ID".Equals(domain) || "github.com".Equals(domain) || Guid.TryParse(domain, out Guid _);

    private static void SetProperties(IVssIdentity identity, IDictionary<string, object> properties)
    {
      if (identity == null || properties == null)
        return;
      object obj;
      properties.TryGetValue("ProviderDisplayName", out obj);
      string str = obj?.ToString();
      if (!string.IsNullOrWhiteSpace(str))
        identity.ProviderDisplayName = str.Trim();
      foreach (KeyValuePair<string, object> property in (IEnumerable<KeyValuePair<string, object>>) properties)
      {
        if (!property.Key.Equals("ProviderDisplayName"))
          identity.SetProperty(property.Key, property.Value);
      }
    }

    public static IdentityDescriptor CreateDescriptorFromSid(string sid)
    {
      if (string.IsNullOrEmpty(sid))
        throw new ArgumentException(TFCommonResources.EmptyStringNotAllowed(), nameof (sid));
      return sid.StartsWith(SidIdentityHelper.TeamFoundationSidPrefix, StringComparison.OrdinalIgnoreCase) ? IdentityHelper.CreateTeamFoundationDescriptor(sid) : IdentityHelper.CreateWindowsDescriptor(sid);
    }

    public static IdentityDescriptor CreateDescriptorFromSid(SecurityIdentifier securityId) => IdentityHelper.CreateDescriptorFromSid(new SecurityIdentifierInfo(securityId));

    internal static IdentityDescriptor CreateDescriptorFromSid(SecurityIdentifierInfo securityIdInfo) => SidIdentityHelper.IsTeamFoundationIdentifier(securityIdInfo) ? IdentityHelper.CreateTeamFoundationDescriptor(securityIdInfo, false) : IdentityHelper.CreateWindowsDescriptor(securityIdInfo);

    public static IdentityDescriptor CreateWindowsDescriptor(string sid) => new IdentityDescriptor("System.Security.Principal.WindowsIdentity", sid);

    public static IdentityDescriptor CreateWindowsDescriptor(SecurityIdentifier securityId) => IdentityHelper.CreateWindowsDescriptor(new SecurityIdentifierInfo(securityId));

    internal static IdentityDescriptor CreateWindowsDescriptor(SecurityIdentifierInfo securityIdInfo) => SidDescriptor.Create("System.Security.Principal.WindowsIdentity", securityIdInfo);

    public static IdentityDescriptor CreateTeamFoundationDescriptor(string sid) => new IdentityDescriptor("Microsoft.TeamFoundation.Identity", sid);

    public static IdentityDescriptor CreateTeamFoundationDescriptor(SecurityIdentifier securityId) => IdentityHelper.CreateTeamFoundationDescriptor(new SecurityIdentifierInfo(securityId), false);

    public static IdentityDescriptor CreateReadOnlyTeamFoundationDescriptor(
      SecurityIdentifier securityId)
    {
      return IdentityHelper.CreateTeamFoundationDescriptor(new SecurityIdentifierInfo(securityId), true);
    }

    internal static IdentityDescriptor CreateTeamFoundationDescriptor(
      SecurityIdentifierInfo securityIdInfo,
      bool readOnly)
    {
      return SidDescriptor.Create("Microsoft.TeamFoundation.Identity", securityIdInfo, readOnly);
    }

    public static IdentityDescriptor CreateUnauthenticatedDescriptor(string sid) => new IdentityDescriptor("Microsoft.TeamFoundation.UnauthenticatedIdentity", sid);

    public static IdentityDescriptor CreateUnauthenticatedDescriptor(SecurityIdentifier securityId) => SidDescriptor.Create("Microsoft.TeamFoundation.UnauthenticatedIdentity", new SecurityIdentifierInfo(securityId));

    public static IdentityDescriptor CreateReadOnlyUnauthenticatedDescriptor(
      SecurityIdentifier securityId)
    {
      return SidDescriptor.Create("Microsoft.TeamFoundation.UnauthenticatedIdentity", new SecurityIdentifierInfo(securityId), true);
    }

    public static bool IsWellKnownGroup(
      IdentityDescriptor descriptor,
      IdentityDescriptor targetGroupDescriptor)
    {
      return descriptor != (IdentityDescriptor) null && string.Equals(descriptor.IdentityType, "Microsoft.TeamFoundation.Identity", StringComparison.OrdinalIgnoreCase) && IdentityHelper.IsWellKnownGroup(descriptor.Identifier, targetGroupDescriptor.Identifier);
    }

    public static bool IsWellKnownGroup(string sid, string targetGroupSid) => SidIdentityHelper.IsWellKnownGroup(sid, targetGroupSid);

    public static bool IdentityHasName(Microsoft.VisualStudio.Services.Identity.Identity identity, string name) => VssStringComparer.UserName.Equals(IdentityHelper.GetUniqueName(identity), name) || VssStringComparer.UserName.Equals(identity.DisplayName, name) || VssStringComparer.UserName.Equals(identity.GetProperty<string>("Account", string.Empty), name) || VssStringComparer.UserName.Equals(IdentityHelper.GetDomainUserName(identity), name);

    internal static SpecialGroupType GetSpecialGroupType(IReadOnlyVssIdentity identity)
    {
      SpecialGroupType specialGroupType = SpecialGroupType.Generic;
      string property = identity.GetProperty<string>("SpecialType", (string) null);
      if (!string.IsNullOrEmpty(property))
        specialGroupType = (SpecialGroupType) Enum.Parse(typeof (SpecialGroupType), property);
      return specialGroupType;
    }

    public static string GetDomainUserName(Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      string displayableName;
      UserNameUtil.GetIdentityName(identity.Descriptor.IdentityType, identity.DisplayName, identity.GetProperty<string>("Domain", string.Empty), identity.GetProperty<string>("Account", string.Empty), identity.UniqueUserId, out string _, out displayableName);
      return displayableName;
    }

    internal static string CreateSecurityToken(IReadOnlyVssIdentity group) => group.GetProperty<Guid>("LocalScopeId", Guid.Empty).ToString() + (object) FrameworkSecurity.IdentitySecurityPathSeparator + group.Id.ToString();

    internal static bool IsEveryoneGroup(IdentityDescriptor descriptor) => descriptor != (IdentityDescriptor) null && string.Equals(descriptor.IdentityType, "Microsoft.TeamFoundation.Identity", StringComparison.OrdinalIgnoreCase) && IdentityHelper.IsEveryoneGroup(descriptor.Identifier);

    internal static bool IsEveryoneGroup(string sid) => sid.StartsWith(SidIdentityHelper.TeamFoundationSidPrefix, StringComparison.OrdinalIgnoreCase) && sid.EndsWith(GroupWellKnownSidConstants.EveryoneGroupSid.Substring(SidIdentityHelper.WellKnownDomainSid.Length), StringComparison.OrdinalIgnoreCase);

    internal static bool IsAnonymousPrincipal(IdentityDescriptor descriptor) => IdentityDescriptorComparer.Instance.Equals(descriptor, UserWellKnownIdentityDescriptors.AnonymousPrincipal);

    internal static bool IsAnonymousPrincipal(SubjectDescriptor subjectDescriptor) => subjectDescriptor.IsSystemPublicAccessType() && subjectDescriptor.Identifier.Equals(AnonymousAccessConstants.AnonymousSubjectId.ToString());

    internal static bool IsAnonymousUsersGroup(IdentityDescriptor descriptor) => string.Equals(descriptor.IdentityType, "Microsoft.TeamFoundation.Identity", StringComparison.OrdinalIgnoreCase) && descriptor.Identifier.StartsWith(SidIdentityHelper.TeamFoundationSidPrefix, StringComparison.OrdinalIgnoreCase) && descriptor.Identifier.EndsWith(GroupWellKnownSidConstants.AnonymousUsersGroupSid.Substring(SidIdentityHelper.WellKnownDomainSid.Length), StringComparison.OrdinalIgnoreCase);

    internal static bool IsLicensedUsersGroup(IdentityDescriptor descriptor) => (object) descriptor != null && descriptor.Identifier != null && string.Equals(descriptor.IdentityType, "Microsoft.TeamFoundation.Identity", StringComparison.OrdinalIgnoreCase) && descriptor.Identifier.StartsWith(SidIdentityHelper.TeamFoundationSidPrefix, StringComparison.OrdinalIgnoreCase) && descriptor.Identifier.EndsWith(GroupWellKnownSidConstants.LicensedUsersGroupSid.Substring(SidIdentityHelper.WellKnownDomainSid.Length), StringComparison.OrdinalIgnoreCase);

    internal static bool IsShardedFrameworkIdentity(
      IVssRequestContext requestContext,
      IdentityDescriptor identityDescriptor)
    {
      return requestContext.ExecutionEnvironment.IsHostedDeployment && identityDescriptor != (IdentityDescriptor) null && (identityDescriptor.IsServiceIdentityType() || identityDescriptor.IsUnauthenticatedIdentity() || identityDescriptor.IsImportedIdentityType()) && !IdentityHelper.IsAnonymousPrincipal(identityDescriptor);
    }

    internal static bool IsShardedFrameworkIdentity(
      IVssRequestContext requestContext,
      SubjectDescriptor subjectDescriptor)
    {
      return requestContext.ExecutionEnvironment.IsHostedDeployment && subjectDescriptor != new SubjectDescriptor() && (subjectDescriptor.IsServiceIdentityType() || subjectDescriptor.IsUnauthenticatedIdentityType() || subjectDescriptor.IsImportedIdentityType()) && !IdentityHelper.IsAnonymousPrincipal(subjectDescriptor);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string CleanProviderDisplayName(
      string originalDisplayName,
      IdentityDescriptor descriptor)
    {
      if (string.IsNullOrEmpty(originalDisplayName))
        return string.Empty;
      bool allowSquareBrackets = string.Equals(descriptor?.IdentityType, "Microsoft.TeamFoundation.Identity", StringComparison.OrdinalIgnoreCase) || string.Equals(descriptor?.IdentityType, "Microsoft.TeamFoundation.ImportedIdentity", StringComparison.OrdinalIgnoreCase);
      string str = IdentityHelper.CleanDisplayName(originalDisplayName, allowSquareBrackets, false);
      if (string.IsNullOrEmpty(str))
        TeamFoundationTrace.Warning("Provider displayname reduced to String.Empty after cleaning: {0}", (object) originalDisplayName);
      return str;
    }

    public static string CleanCustomDisplayName(
      string originalDisplayName,
      IdentityDescriptor descriptor,
      bool enforceClean = true)
    {
      if (string.IsNullOrEmpty(originalDisplayName))
        return string.Empty;
      string str = IdentityHelper.CleanWhitespace(originalDisplayName);
      bool allowSquareBrackets = string.Equals(descriptor?.IdentityType, "Microsoft.TeamFoundation.Identity", StringComparison.OrdinalIgnoreCase) || string.Equals(descriptor?.IdentityType, "Microsoft.TeamFoundation.ImportedIdentity", StringComparison.OrdinalIgnoreCase);
      string b = IdentityHelper.CleanDisplayName(str, allowSquareBrackets, true);
      if (enforceClean && !string.Equals(str, b, StringComparison.Ordinal))
        throw new InvalidDisplayNameException(FrameworkResources.CustomDisplayNameError());
      return b;
    }

    private static string CleanDisplayName(
      string originalDisplayName,
      bool allowSquareBrackets,
      bool customName)
    {
      string displayName = TFCommonUtil.ReplaceInvalidCharacters(originalDisplayName, " ", true).Replace(';', ' ').Replace('|', ' ').Replace('<', ' ').Replace('>', ' ').Replace('"', ' ').Replace('%', ' ').Replace('/', ' ');
      if (customName)
        displayName = displayName.Replace('@', ' ').Replace('\\', ' ');
      if (customName || !allowSquareBrackets)
        displayName = displayName.Replace('[', ' ').Replace(']', ' ');
      string text = IdentityHelper.CleanWhitespace(displayName).Trim(IdentityHelper.s_trimCharacters);
      if (text.Length > 256)
        text = StringUtil.Truncate(text, 256, false).TrimEnd(IdentityHelper.s_trimCharacters);
      if (!string.IsNullOrEmpty(text))
      {
        if (string.IsNullOrEmpty(text.Trim('.')))
          text = string.Empty;
      }
      return text;
    }

    public static string CleanWhitespace(string displayName)
    {
      displayName = displayName.Trim();
      StringBuilder stringBuilder = new StringBuilder(displayName);
      for (int index = stringBuilder.Length - 1; index >= 0; --index)
      {
        if (char.IsWhiteSpace(stringBuilder[index]))
        {
          stringBuilder[index] = ' ';
          if (index + 1 < stringBuilder.Length && char.IsWhiteSpace(stringBuilder[index + 1]))
            stringBuilder.Remove(index + 1, 1);
        }
      }
      return stringBuilder.ToString();
    }

    internal static string GetScopeUri(Guid scopeId, GroupScopeType scopeType)
    {
      switch (scopeType)
      {
        case GroupScopeType.Generic:
          return LinkingUtilities.EncodeUri(new ArtifactId("Framework", "Generic", scopeId.ToString()));
        case GroupScopeType.ServiceHost:
          return TFCommonUtil.GetIdentityDomainScope(scopeId);
        case GroupScopeType.TeamProject:
          return LinkingUtilities.EncodeUri(new ArtifactId("Classification", "TeamProject", scopeId.ToString("D")));
        default:
          throw new NotSupportedException("GroupScopeType");
      }
    }

    public static Guid ParseScopeId(
      string scope,
      Guid defaultScopeId,
      out GroupScopeType scopeType)
    {
      Guid scopeId;
      if (IdentityHelper.TryParseScopeId(scope, defaultScopeId, out scopeId, out scopeType))
        return scopeId;
      throw new GroupScopeDoesNotExistException(scope);
    }

    internal static bool TryParseScopeId(
      string scope,
      Guid defaultScopeId,
      out Guid scopeId,
      out GroupScopeType scopeType)
    {
      if (string.IsNullOrWhiteSpace(scope) || string.Equals(scope, "[SERVER]", StringComparison.OrdinalIgnoreCase))
      {
        scopeId = defaultScopeId;
        scopeType = GroupScopeType.ServiceHost;
        return true;
      }
      if (LinkingUtilities.IsUriWellFormed(scope))
      {
        ArtifactId artifactId = LinkingUtilities.DecodeUri(scope);
        if (Guid.TryParse(artifactId.ToolSpecificId, out scopeId))
        {
          switch (artifactId.ArtifactType)
          {
            case "TeamProject":
              scopeType = GroupScopeType.TeamProject;
              break;
            case "IdentityDomain":
              scopeType = GroupScopeType.ServiceHost;
              break;
            case "Generic":
              scopeType = GroupScopeType.Generic;
              break;
            default:
              throw new ArgumentException("scopeUri");
          }
          return true;
        }
        scopeId = Guid.Empty;
        scopeType = GroupScopeType.Generic;
        return false;
      }
      if (Guid.TryParse(scope, out scopeId))
      {
        scopeType = GroupScopeType.Generic;
        return true;
      }
      scopeId = Guid.Empty;
      scopeType = GroupScopeType.Generic;
      return false;
    }

    public static IdentityPuid GetPuid(IReadOnlyVssIdentity identity) => identity == null ? (IdentityPuid) null : new IdentityPuid(identity.GetProperty<string>("PUID", string.Empty));

    public static IdentityPuid GetPuid(
      IVssRequestContext requestContext,
      IdentityDescriptor identityDescriptor)
    {
      IdentityService service = requestContext.GetService<IdentityService>();
      IdentityDescriptor[] descriptors = new IdentityDescriptor[1]
      {
        identityDescriptor
      };
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = service.ReadIdentities(requestContext, (IList<IdentityDescriptor>) descriptors, QueryMembership.None, (IEnumerable<string>) new string[0]);
      return identityList.Count > 0 ? IdentityHelper.GetPuid((IReadOnlyVssIdentity) identityList[0]) : throw new IdentityNotFoundException(identityDescriptor);
    }

    public static JsonWebToken GetJwtToken(IVssRequestContext requestContext, string token)
    {
      try
      {
        if (!string.IsNullOrEmpty(token))
          return JsonWebToken.Create(token);
      }
      catch (JsonWebTokenDeserializationException ex)
      {
        requestContext.TraceAlways(1450027, TraceLevel.Verbose, "IdentityService", nameof (IdentityHelper), "Token was unable to be processed due to a deserialization exception: " + ex.Message);
      }
      catch (FormatException ex)
      {
        requestContext.TraceAlways(1450027, TraceLevel.Verbose, "IdentityService", nameof (IdentityHelper), "Token was unable to be processed due to a deserialization exception: " + ex.Message);
      }
      catch (JsonReaderException ex)
      {
        requestContext.TraceAlways(1450027, TraceLevel.Verbose, "IdentityService", nameof (IdentityHelper), "Token was unable to be processed due to a deserialization exception: " + ex.Message);
      }
      catch (ArgumentException ex)
      {
        requestContext.TraceAlways(1450027, TraceLevel.Verbose, "IdentityService", nameof (IdentityHelper), "Token was unable to be processed due to a deserialization exception: " + ex.Message);
      }
      return (JsonWebToken) null;
    }

    public static string GetIdentityResourceUriString(
      IVssRequestContext requestContext,
      Guid identityId,
      ILocationDataProvider locationDataProvider = null)
    {
      if (requestContext == null || identityId == Guid.Empty)
        return (string) null;
      if (locationDataProvider == null)
        locationDataProvider = IdentityHelper.GetLocationDataProviderForResourceUriLookup(requestContext);
      IVssRequestContext contextForUriLookup = IdentityHelper.GetRequestContextForUriLookup(requestContext);
      return locationDataProvider?.GetResourceUri(contextForUriLookup, "IMS", IdentityResourceIds.Identity, (object) new
      {
        identityId = identityId
      })?.ToString();
    }

    internal static ILocationDataProvider GetLocationDataProviderForResourceUriLookup(
      IVssRequestContext requestContext)
    {
      IVssRequestContext contextForUriLookup = IdentityHelper.GetRequestContextForUriLookup(requestContext);
      Guid serviceAreaIdentifier = !requestContext.ExecutionEnvironment.IsOnPremisesDeployment ? ServiceInstanceTypes.SPS : Guid.Empty;
      return contextForUriLookup.GetService<ILocationService>().GetLocationData(contextForUriLookup, serviceAreaIdentifier);
    }

    internal static IVssRequestContext GetRequestContextForUriLookup(
      IVssRequestContext requestContext)
    {
      return !requestContext.ExecutionEnvironment.IsOnPremisesDeployment ? requestContext.To(TeamFoundationHostType.Application) : requestContext;
    }

    internal static string GetHostLocation(IVssRequestContext requestContext)
    {
      if (requestContext == null)
        return (string) null;
      ILocationService service = requestContext.GetService<ILocationService>();
      Guid guid = requestContext.ExecutionEnvironment.IsOnPremisesDeployment ? Guid.Empty : ServiceInstanceTypes.TFS;
      IVssRequestContext requestContext1 = requestContext;
      Guid serviceAreaIdentifier = guid;
      string accessMappingMoniker = AccessMappingConstants.ClientAccessMappingMoniker;
      return service.GetLocationServiceUrl(requestContext1, serviceAreaIdentifier, accessMappingMoniker);
    }

    public static string GetImageResourceUrl(
      IVssRequestContext requestContext,
      Guid identityId,
      string hostLocation = null)
    {
      if (requestContext == null || identityId == Guid.Empty)
        return (string) null;
      if (hostLocation == null)
        hostLocation = IdentityHelper.GetHostLocation(requestContext);
      if (hostLocation == null)
        return (string) null;
      StringBuilder stringBuilder = new StringBuilder(hostLocation.TrimEnd('/'));
      stringBuilder.Append("/_api/_common/identityImage?id=");
      stringBuilder.Append((object) identityId);
      return stringBuilder.ToString();
    }

    public static string GetUniqueName(Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      if (identity == null)
        throw new ArgumentNullException(nameof (identity));
      return IdentityHelper.GetUniqueName(identity.GetProperty<string>("Domain", string.Empty), identity.GetProperty<string>("Account", string.Empty), identity.UniqueUserId);
    }

    public static string GetUniqueName(string domain, string account, int uniqueUserId)
    {
      if (string.IsNullOrWhiteSpace(account))
        return string.Empty;
      return uniqueUserId == 0 ? (string.IsNullOrWhiteSpace(domain) || account.Contains("@") ? account : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}\\{1}", (object) domain, (object) account)) : (string.IsNullOrWhiteSpace(domain) || account.Contains("@") ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) account, (object) uniqueUserId) : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}\\{1}:{2}", (object) domain, (object) account, (object) uniqueUserId));
    }

    public static bool IsDistinctDisplayName(string name)
    {
      if (string.IsNullOrWhiteSpace(name))
        return false;
      name = name.Trim();
      int num1 = name.IndexOf('<');
      int num2 = name.IndexOf('>');
      return !string.IsNullOrWhiteSpace(name) && num1 > 0 && num2 != -1 && num2 + 1 == name.Length;
    }

    public static string GetUniqueNameFromDistinctDisplayName(string distinctDisplayName)
    {
      if (string.IsNullOrWhiteSpace(distinctDisplayName))
        return (string) null;
      distinctDisplayName = distinctDisplayName.Trim();
      int num1 = distinctDisplayName.IndexOf('<');
      int num2 = distinctDisplayName.IndexOf('>');
      return num1 == -1 || num2 == -1 || num2 + 1 != distinctDisplayName.Length ? (string) null : distinctDisplayName.Substring(num1 + 1, num2 - num1 - 1);
    }

    public static string GetDisplayNameFromDistinctDisplayName(string distinctDisplayName)
    {
      if (distinctDisplayName == null)
        return (string) null;
      string distinctDisplayName1 = distinctDisplayName.Trim();
      int startIndex = 0;
      int length = distinctDisplayName.IndexOf(" <");
      if (length != -1)
        distinctDisplayName1 = distinctDisplayName1.Substring(startIndex, length);
      return distinctDisplayName1;
    }

    public static IdentityDescriptor GetGitHubBindPendingDescriptor(Microsoft.VisualStudio.Services.Identity.Identity identity) => new IdentityDescriptor("Microsoft.TeamFoundation.BindPendingIdentity", "upn:" + "github.com" + "\\" + identity.SocialDescriptor.Identifier);

    public static bool IsAcsServiceIdentity(IReadOnlyVssIdentity identity) => AcsServiceIdentityHelper.IsServiceIdentity(identity);

    public static bool IsServiceIdentityType(IdentityDescriptor identityDescriptor) => identityDescriptor?.IdentityType == "Microsoft.TeamFoundation.ServiceIdentity";

    public static bool IsServiceIdentity(
      IVssRequestContext requestContext,
      IReadOnlyVssIdentity identity)
    {
      if (identity == null)
        return false;
      if (ServicePrincipals.IsServicePrincipal(requestContext, identity.Descriptor) || IdentityHelper.IsAcsServiceIdentity(identity))
        return true;
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      return !requestContext.ExecutionEnvironment.IsHostedDeployment && vssRequestContext.GetService<IdentityService>().IsMember(vssRequestContext, GroupWellKnownIdentityDescriptors.ServiceUsersGroup, identity.Descriptor);
    }

    public static Microsoft.VisualStudio.Services.Identity.Identity GetPrimaryMsaIdentity(
      this IdentityService identityService,
      IVssRequestContext requestContext,
      IReadOnlyVssIdentity identity)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (identity == null)
        return (Microsoft.VisualStudio.Services.Identity.Identity) null;
      if (!identity.IsExternalUser || identity.IsContainer)
        return (Microsoft.VisualStudio.Services.Identity.Identity) null;
      string property1 = identity.GetProperty<string>("PUID", (string) null);
      if (string.IsNullOrWhiteSpace(property1))
        return (Microsoft.VisualStudio.Services.Identity.Identity) null;
      if (property1.StartsWith("aad:"))
        return (Microsoft.VisualStudio.Services.Identity.Identity) null;
      IVssRequestContext context = requestContext.To(TeamFoundationHostType.Deployment);
      IdentityService service = context.GetService<IdentityService>();
      Microsoft.VisualStudio.Services.Identity.Identity primaryMsaIdentity = service.ReadIdentities(context.Elevate(), (IList<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        new IdentityDescriptor("Microsoft.IdentityModel.Claims.ClaimsIdentity", property1 + "@Live.com")
      }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      string property2 = identity.GetProperty<string>("Account", (string) null);
      if (primaryMsaIdentity == null)
      {
        IdentityDescriptor descriptorFromAccountName = IdentityHelper.CreateDescriptorFromAccountName("Windows Live ID", property2, true);
        primaryMsaIdentity = service.ReadIdentities(context.Elevate(), (IList<IdentityDescriptor>) new IdentityDescriptor[1]
        {
          descriptorFromAccountName
        }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      }
      if (primaryMsaIdentity == null)
      {
        IdentityDescriptor identityDescriptor = IdentityAuthenticationHelper.BuildTemporaryDescriptorFromEmailAddress(property2);
        primaryMsaIdentity = service.ReadIdentities(context.Elevate(), (IList<IdentityDescriptor>) new IdentityDescriptor[1]
        {
          identityDescriptor
        }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      }
      return primaryMsaIdentity;
    }

    public static Microsoft.VisualStudio.Services.Identity.Identity FindIdentity(
      IVssRequestContext requestContext,
      string identityName,
      bool throwOnMissing,
      bool includeInactiveIdentities)
    {
      requestContext.Trace(700109, TraceLevel.Verbose, "IdentityService", nameof (IdentityHelper), "Entering {0}: {1}; {2}", (object) nameof (FindIdentity), (object) identityName, (object) throwOnMissing);
      IVssIdentitySearchService service = requestContext.GetService<IVssIdentitySearchService>();
      Microsoft.VisualStudio.Services.Identity.Identity identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      if (identityName == ".")
      {
        identity = requestContext.GetUserIdentity();
      }
      else
      {
        Guid result;
        if (Guid.TryParse(identityName, out result))
          identity = IdentityHelper.FindIdentity(requestContext, result, false);
        bool flag = false;
        if (identity == null)
        {
          flag = TFCommonUtil.IsLegalIdentity(identityName);
          if (flag)
          {
            if (requestContext.ExecutionEnvironment.IsHostedDeployment && !UserNameUtil.IsComplete(identityName) && ArgumentUtility.IsValidEmailAddress(identityName))
            {
              Guid organizationAadTenantId = requestContext.GetOrganizationAadTenantId();
              if (!organizationAadTenantId.Equals(Guid.Empty) || IdentityHelper.ShouldQualifyMsaByDomainName(requestContext))
              {
                string relative = organizationAadTenantId.Equals(Guid.Empty) ? "Windows Live ID" : organizationAadTenantId.ToString();
                string str = identityName;
                identityName = UserNameUtil.Complete(identityName, relative);
                requestContext.Trace(700301, TraceLevel.Verbose, "IdentityService", nameof (IdentityHelper), "disambiguated {0} to {1}", (object) str, (object) identityName);
              }
            }
            using (requestContext.AcquireExemptionLock())
            {
              requestContext.Trace(700311, TraceLevel.Verbose, "IdentityService", nameof (IdentityHelper), "calling FindActiveUser at account level, search AccountName ({0})", (object) identityName);
              try
              {
                identity = service.FindActiveUser(requestContext, IdentitySearchFilter.AccountName, identityName);
              }
              catch (MultipleIdentitiesFoundException ex)
              {
                requestContext.TraceException(919776, "IdentityService", nameof (IdentityHelper), (Exception) ex);
                throw ex;
              }
            }
          }
        }
        if (identity == null)
        {
          requestContext.Trace(700312, TraceLevel.Verbose, "IdentityService", nameof (IdentityHelper), "calling FindActiveUser at account level, searching DisplayName ({0})", (object) identityName);
          identity = service.FindActiveUser(requestContext, IdentitySearchFilter.DisplayName, identityName);
        }
        if (identity == null & includeInactiveIdentities & flag)
        {
          requestContext.Trace(700313, TraceLevel.Verbose, "IdentityService", nameof (IdentityHelper), "calling ReadIdentities at deployment level, searching AccountName ({0})", (object) identityName);
          identity = service.FindActiveOrHistoricalMember(requestContext, IdentitySearchFilter.AccountName, identityName);
        }
      }
      if (identity == null)
      {
        requestContext.Trace(700111, TraceLevel.Verbose, "IdentityService", nameof (IdentityHelper), "Identity not found:{0}  includeInactiveIdentities:{1}", (object) identityName, (object) includeInactiveIdentities);
        if (throwOnMissing)
          throw new IdentityNotFoundException(IdentityResources.IdentityNotFoundWithName((object) identityName));
      }
      requestContext.Trace(700113, TraceLevel.Verbose, "IdentityService", nameof (IdentityHelper), "Leaving {0}: {1}", (object) nameof (FindIdentity), (object) identity?.GetProperty<Guid>("CUID", identity.Id));
      return identity;
    }

    private static bool ShouldQualifyMsaByDomainName(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Identity.UseQualifiedAccountNameForMSA") || requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment);

    public static Microsoft.VisualStudio.Services.Identity.Identity GetIdentityFromList(
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList,
      string identityName)
    {
      List<Microsoft.VisualStudio.Services.Identity.Identity> matchingIdentities = new List<Microsoft.VisualStudio.Services.Identity.Identity>((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identityList);
      matchingIdentities.RemoveAll((Predicate<Microsoft.VisualStudio.Services.Identity.Identity>) (i => i == null));
      if (matchingIdentities.Count == 1)
        return matchingIdentities[0];
      if (matchingIdentities.Count > 1)
      {
        foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in matchingIdentities)
          identity.CustomDisplayName = IdentityHelper.GetUniqueName(identity);
        throw new MultipleIdentitiesFoundException(identityName, (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) matchingIdentities);
      }
      return (Microsoft.VisualStudio.Services.Identity.Identity) null;
    }

    public static Microsoft.VisualStudio.Services.Identity.Identity FindIdentity(
      IVssRequestContext requestContext,
      Guid vsid,
      bool throwOnMissing = true)
    {
      IIdentityServiceInternal identityServiceInternal = requestContext.GetService<IdentityService>().IdentityServiceInternal();
      requestContext.Trace(700114, TraceLevel.Verbose, "IdentityService", nameof (IdentityHelper), "Entering {0}: {1}", (object) nameof (FindIdentity), (object) vsid);
      IVssRequestContext requestContext1 = requestContext;
      Guid[] identityIds = new Guid[1]{ vsid };
      Microsoft.VisualStudio.Services.Identity.Identity identityFromList = IdentityHelper.GetIdentityFromList(identityServiceInternal.ReadIdentities(requestContext1, (IList<Guid>) identityIds, QueryMembership.None, (IEnumerable<string>) null), vsid.ToString());
      if (identityFromList == null & throwOnMissing)
        throw new IdentityNotFoundException(vsid);
      requestContext.Trace(700115, TraceLevel.Verbose, "IdentityService", nameof (IdentityHelper), "Leaving {0}: {1}", (object) nameof (FindIdentity), (object) identityFromList?.GetProperty<Guid>("CUID", identityFromList.Id));
      return identityFromList;
    }

    public static bool IsUserIdentity(
      IVssRequestContext requestContext,
      IReadOnlyVssIdentity identity)
    {
      string property = identity?.GetProperty<string>("Domain", (string) null);
      return property != null && !identity.IsContainer && !IdentityHelper.IsAcsServiceIdentity(identity) && !ServicePrincipals.IsServicePrincipal(requestContext, identity.Descriptor) && !VssStringComparer.DomainName.Equals(property, "TEAM FOUNDATION") && !IdentityHelper.IsWindowsLocalServiceIdentity(requestContext, identity) && !IdentityHelper.IsWindowsRemoteServiceIdentity(requestContext, identity);
    }

    public static void ScrubMasterId(Microsoft.VisualStudio.Services.Identity.Identity identity, bool returnEmptyGuid = false, bool forUpdate = false)
    {
      if (identity == null || identity.MasterId == Guid.Empty || identity.MasterId == IdentityConstants.LinkedId)
        return;
      if (forUpdate)
        identity.MasterId = Guid.Empty;
      else if (returnEmptyGuid)
        identity.MasterId = Guid.Empty;
      else
        identity.MasterId = identity.Id == identity.MasterId ? Guid.Empty : IdentityConstants.LinkedId;
    }

    public static Guid MaterializeUser(
      IVssRequestContext requestContext,
      IVssIdentity identity,
      [CallerMemberName] string callerMethodName = null)
    {
      requestContext.TraceDataConditionally(824539, TraceLevel.Verbose, "IdentityService", nameof (MaterializeUser), "Attempting member materialization", (Func<object>) (() => (object) new
      {
        identity = identity,
        callerMethodName = callerMethodName
      }), nameof (MaterializeUser));
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      if (!requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.DisableMaterializeUserBySubjectDescriptor"))
      {
        try
        {
          DirectoryEntityDescriptor subjectDescriptor = IdentityHelper.GetEntityDescriptorBySubjectDescriptor(requestContext, identity, callerMethodName);
          return IdentityHelper.AddMemberByEntityDescriptor(vssRequestContext, identity, subjectDescriptor, callerMethodName);
        }
        catch (IdentityMaterializationFailedException ex)
        {
          vssRequestContext.TraceDataConditionally(180875, TraceLevel.Error, "IdentityService", "AddMemberBySubjectDescriptor", ex.Message, (Func<object>) (() => (object) new
          {
            identity = identity,
            callerMethodName = callerMethodName
          }), nameof (MaterializeUser));
        }
      }
      try
      {
        DirectoryEntityDescriptor byObjectIdOrPuid = IdentityHelper.GetEntityDescriptorByObjectIdOrPuid(requestContext, identity, callerMethodName);
        return IdentityHelper.AddMemberByEntityDescriptor(vssRequestContext, identity, byObjectIdOrPuid, callerMethodName);
      }
      catch (IdentityMaterializationFailedException ex)
      {
        vssRequestContext.TraceDataConditionally(180875, TraceLevel.Error, "IdentityService", "AddMemberByObjectIdOrPuid", ex.Message, (Func<object>) (() => (object) new
        {
          identity = identity,
          callerMethodName = callerMethodName
        }), nameof (MaterializeUser));
      }
      DirectoryEntityDescriptor entityDescriptorByUpn = IdentityHelper.GetEntityDescriptorByUpn(requestContext, identity, callerMethodName);
      return IdentityHelper.AddMemberByEntityDescriptor(vssRequestContext, identity, entityDescriptorByUpn, callerMethodName);
    }

    public static bool DoesUserBelongToMicrosoftTenant(
      IVssRequestContext context,
      IVssIdentity identity)
    {
      Guid tenantId = AadIdentityHelper.ExtractTenantId((IReadOnlyVssIdentity) identity);
      if (tenantId == Guid.Empty)
        return false;
      Guid microsoftTenantId = context.GetMicrosoftTenantId();
      context.Trace(8524015, TraceLevel.Info, "IdentityService", nameof (IdentityHelper), string.Format("MSFT tenantId is {0}, user tenantId is {1}", (object) microsoftTenantId, (object) tenantId));
      if (!(microsoftTenantId == Guid.Empty))
        return microsoftTenantId == tenantId;
      context.TraceAlways(8524016, TraceLevel.Info, "IdentityService", nameof (IdentityHelper), "Failed to load the Microsoft AAD tenant Id.");
      return false;
    }

    private static Guid AddMemberByEntityDescriptor(
      IVssRequestContext elevatedRequestContext,
      IVssIdentity identity,
      DirectoryEntityDescriptor addMemberRequestDescriptor,
      string callerMethodName)
    {
      DirectoryEntityResult addMemberResult = elevatedRequestContext.GetService<IDirectoryService>().AddMember(elevatedRequestContext, (IDirectoryEntityDescriptor) addMemberRequestDescriptor, license: "None");
      if (addMemberResult == null)
      {
        string str = string.Format("Failed to materialize identity {0} ({1}) - null materialization response", (object) identity, (object) identity.Id);
        elevatedRequestContext.TraceDataConditionally(121254011, TraceLevel.Error, "IdentityService", nameof (AddMemberByEntityDescriptor), str, (Func<object>) (() => (object) new
        {
          addMemberRequestDescriptor = addMemberRequestDescriptor,
          identity = identity,
          callerMethodName = callerMethodName
        }), nameof (AddMemberByEntityDescriptor));
        throw new IdentityMaterializationFailedException(str);
      }
      if (addMemberResult.Status != "Success")
      {
        string str = string.Format("Failed to materialize identity {0} ({1}). Failed materialization response: {2}", (object) identity, (object) identity.Id, (object) addMemberResult.Status);
        elevatedRequestContext.TraceDataConditionally(121254012, TraceLevel.Error, "IdentityService", nameof (AddMemberByEntityDescriptor), str, (Func<object>) (() => (object) new
        {
          addMemberResult = addMemberResult,
          addMemberRequestDescriptor = addMemberRequestDescriptor,
          identity = identity,
          callerMethodName = callerMethodName
        }), nameof (AddMemberByEntityDescriptor));
        throw new IdentityMaterializationFailedException(str, addMemberResult.Exception);
      }
      if (addMemberResult.Entity == null)
      {
        string str = string.Format("Failed to materialize identity {0} ({1}) since the entity on the materialization response is null", (object) identity, (object) identity.Id);
        elevatedRequestContext.TraceDataConditionally(121254013, TraceLevel.Error, "IdentityService", nameof (AddMemberByEntityDescriptor), str, (Func<object>) (() => (object) new
        {
          addMemberResult = addMemberResult,
          addMemberRequestDescriptor = addMemberRequestDescriptor,
          identity = identity,
          callerMethodName = callerMethodName
        }), nameof (AddMemberByEntityDescriptor));
        throw new IdentityMaterializationFailedException(str, addMemberResult.Exception);
      }
      string localId = addMemberResult.Entity.LocalId;
      if (string.IsNullOrEmpty(localId))
      {
        string str = string.Format("Failed to materialize identity {0} ({1}) since the local ID on the materialization response is null or empty", (object) identity, (object) identity.Id);
        elevatedRequestContext.TraceDataConditionally(121254014, TraceLevel.Error, "IdentityService", nameof (AddMemberByEntityDescriptor), str, (Func<object>) (() => (object) new
        {
          addMemberResult = addMemberResult,
          addMemberRequestDescriptor = addMemberRequestDescriptor,
          identity = identity,
          callerMethodName = callerMethodName
        }), nameof (AddMemberByEntityDescriptor));
        throw new IdentityMaterializationFailedException(str, addMemberResult.Exception);
      }
      Guid result;
      if (!Guid.TryParse(localId, out result))
      {
        string str = string.Format("Failed to materialize identity {0} ({1}) since the local ID on the materialization response is not a GUID: {2}", (object) identity, (object) identity.Id, (object) localId);
        elevatedRequestContext.TraceDataConditionally(121254015, TraceLevel.Error, "IdentityService", nameof (AddMemberByEntityDescriptor), str, (Func<object>) (() => (object) new
        {
          addMemberResult = addMemberResult,
          addMemberRequestDescriptor = addMemberRequestDescriptor,
          identity = identity,
          callerMethodName = callerMethodName
        }), nameof (AddMemberByEntityDescriptor));
        throw new IdentityMaterializationFailedException(str, addMemberResult.Exception);
      }
      return result;
    }

    private static DirectoryEntityDescriptor GetEntityDescriptorBySubjectDescriptor(
      IVssRequestContext requestContext,
      IVssIdentity identity,
      string callerMethodName)
    {
      Dictionary<string, object> properties = new Dictionary<string, object>();
      properties["AllowIntroductionOfMembersNotPreviouslyInScope"] = (object) true;
      properties["InvitationMethod"] = (object) ("IdentityHelper.MaterializeUser.GetEntityDescriptorBySubjectDescriptor@" + callerMethodName);
      SubjectDescriptor subjectDescriptor = identity.GetSubjectDescriptor(requestContext);
      if (subjectDescriptor == new SubjectDescriptor())
      {
        string str = string.Format("Cannot materialize user {0} ({1}) since subject descriptor is missing", (object) identity, (object) identity.Id);
        requestContext.TraceDataConditionally(121254003, TraceLevel.Error, "IdentityService", nameof (GetEntityDescriptorBySubjectDescriptor), str, (Func<object>) (() => (object) new
        {
          identity = identity,
          callerMethodName = callerMethodName
        }), nameof (GetEntityDescriptorBySubjectDescriptor));
        throw new IdentityMaterializationFailedException(str);
      }
      properties["SubjectDescriptor"] = (object) subjectDescriptor;
      DirectoryEntityDescriptor addMemberRequestDescriptor = new DirectoryEntityDescriptor("User", properties: (IReadOnlyDictionary<string, object>) properties);
      requestContext.TraceDataConditionally(121254004, TraceLevel.Verbose, "IdentityService", nameof (GetEntityDescriptorBySubjectDescriptor), "Materializing with following", (Func<object>) (() => (object) new
      {
        addMemberRequestDescriptor = addMemberRequestDescriptor,
        identity = identity
      }), nameof (GetEntityDescriptorBySubjectDescriptor));
      return addMemberRequestDescriptor;
    }

    private static DirectoryEntityDescriptor GetEntityDescriptorByUpn(
      IVssRequestContext requestContext,
      IVssIdentity identity,
      string callerMethodName)
    {
      string property = identity.GetProperty<string>("Account", string.Empty);
      if (string.IsNullOrEmpty(property))
      {
        string str = string.Format("Cannot materialize MSA user {0} ({1}) since it has no principal name", (object) identity, (object) identity.Id);
        requestContext.TraceDataConditionally(121254003, TraceLevel.Error, "IdentityService", nameof (GetEntityDescriptorByUpn), str, (Func<object>) (() => (object) new
        {
          identity = identity,
          callerMethodName = callerMethodName
        }), nameof (GetEntityDescriptorByUpn));
        throw new IdentityMaterializationFailedException(str);
      }
      DirectoryEntityDescriptor addMemberRequestDescriptor = new DirectoryEntityDescriptor("User", principalName: property, properties: (IReadOnlyDictionary<string, object>) new Dictionary<string, object>()
      {
        ["AllowIntroductionOfMembersNotPreviouslyInScope"] = (object) true,
        ["InvitationMethod"] = (object) ("IdentityHelper.MaterializeUser.GetEntityDescriptorByUpn@" + callerMethodName)
      });
      requestContext.TraceDataConditionally(121254004, TraceLevel.Verbose, "IdentityService", nameof (GetEntityDescriptorByUpn), "Materializing with following", (Func<object>) (() => (object) new
      {
        addMemberRequestDescriptor = addMemberRequestDescriptor,
        identity = identity,
        callerMethodName = callerMethodName
      }), nameof (GetEntityDescriptorByUpn));
      return addMemberRequestDescriptor;
    }

    private static DirectoryEntityDescriptor GetEntityDescriptorByObjectIdOrPuid(
      IVssRequestContext requestContext,
      IVssIdentity identity,
      string callerMethodName)
    {
      Dictionary<string, object> properties = new Dictionary<string, object>();
      properties["AllowIntroductionOfMembersNotPreviouslyInScope"] = (object) true;
      properties["InvitationMethod"] = (object) ("IdentityHelper.MaterializeUser.GetEntityDescriptorByObjectIdOrPuid@" + callerMethodName);
      if (identity.IsExternalUser)
      {
        Guid property = identity.GetProperty<Guid>("http://schemas.microsoft.com/identity/claims/objectidentifier", Guid.Empty);
        if (property == Guid.Empty)
        {
          string str = string.Format("Cannot materialize AAD user {0} ({1}) since it has no object ID", (object) identity, (object) identity.Id);
          requestContext.TraceDataConditionally(121254001, TraceLevel.Error, "IdentityService", nameof (GetEntityDescriptorByObjectIdOrPuid), str, (Func<object>) (() => (object) new
          {
            identity = identity,
            callerMethodName = callerMethodName
          }), nameof (GetEntityDescriptorByObjectIdOrPuid));
          throw new IdentityMaterializationFailedException(str);
        }
        DirectoryEntityDescriptor addMemberRequestDescriptor = new DirectoryEntityDescriptor("User", "aad", property.ToString(), properties: (IReadOnlyDictionary<string, object>) properties);
        requestContext.TraceDataConditionally(121254002, TraceLevel.Verbose, "IdentityService", nameof (GetEntityDescriptorByObjectIdOrPuid), "Materializing with following", (Func<object>) (() => (object) new
        {
          addMemberRequestDescriptor = addMemberRequestDescriptor,
          identity = identity,
          callerMethodName = callerMethodName
        }), nameof (GetEntityDescriptorByObjectIdOrPuid));
        return addMemberRequestDescriptor;
      }
      if (identity.IsMsaIdentity())
      {
        string property = identity.GetProperty<string>("PUID", string.Empty);
        if (property == string.Empty)
        {
          string str = string.Format("Cannot materialize MSA user {0} ({1}) since it has no Puid", (object) identity, (object) identity.Id);
          requestContext.TraceDataConditionally(121254001, TraceLevel.Error, "IdentityService", nameof (GetEntityDescriptorByObjectIdOrPuid), str, (Func<object>) (() => (object) new
          {
            identity = identity,
            callerMethodName = callerMethodName
          }), nameof (GetEntityDescriptorByObjectIdOrPuid));
          throw new IdentityMaterializationFailedException(str);
        }
        properties["Puid"] = (object) property;
        DirectoryEntityDescriptor addMemberRequestDescriptor = new DirectoryEntityDescriptor("User", "vsd", properties: (IReadOnlyDictionary<string, object>) properties);
        requestContext.TraceDataConditionally(121254004, TraceLevel.Verbose, "IdentityService", nameof (GetEntityDescriptorByObjectIdOrPuid), "Materializing with following", (Func<object>) (() => (object) new
        {
          addMemberRequestDescriptor = addMemberRequestDescriptor,
          identity = identity,
          callerMethodName = callerMethodName
        }), nameof (GetEntityDescriptorByObjectIdOrPuid));
        return addMemberRequestDescriptor;
      }
      string str1 = string.Format("Cannot materialize identity {0} ({1}) since it is not a supported type (e.g. an AAD or MSA user)", (object) identity, (object) identity.Id);
      requestContext.TraceDataConditionally(121254005, TraceLevel.Error, "IdentityService", nameof (GetEntityDescriptorByObjectIdOrPuid), str1, (Func<object>) (() => (object) new
      {
        identity = identity,
        callerMethodName = callerMethodName
      }), nameof (GetEntityDescriptorByObjectIdOrPuid));
      throw new IdentityMaterializationFailedException(str1);
    }

    public static IdentityRef CreateUnboundIdentityRef(IVssRequestContext rc, Guid id)
    {
      rc.CheckPermissionToReadPublicIdentityInfo();
      return new IdentityRef() { Id = id.ToString() };
    }

    internal static bool IsOrgUser(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      IVssRequestContext vssRequestContext1 = requestContext.Elevate();
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) || !requestContext.IsOrganizationActivated() && !requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Identity.UseTenantAsBoundaryForOidProjectVisibility") || requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Identity.UseTenantAsBoundaryForOidProjectVisibility") && !requestContext.IsMicrosoftTenant() || !vssRequestContext1.GetService<IOrganizationPolicyService>().GetPolicy<bool>(vssRequestContext1, "Policy.AllowOrgAccess", true).EffectiveValue)
      {
        requestContext.Trace(584955, TraceLevel.Info, "IdentityService", nameof (IdentityHelper), "Return false because either we are in an inactivated org or allow org access policy is turned off.");
        return false;
      }
      Guid aadUserId;
      if (identity == null || !AadIdentityHelper.TryExtractObjectId((IReadOnlyVssIdentity) identity, out aadUserId))
      {
        requestContext.Trace(584954, TraceLevel.Info, "IdentityService", nameof (IdentityHelper), "Return false because identity is not an aad user.");
        return false;
      }
      IAadTenantDetailProvider extension = requestContext.GetExtension<IAadTenantDetailProvider>();
      IdentityService service1 = vssRequestContext1.GetService<IdentityService>();
      if (!requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Identity.UseTenantAsBoundaryForOidProjectVisibility"))
      {
        IVssRequestContext vssRequestContext2 = requestContext.To(TeamFoundationHostType.Application).Elevate();
        Microsoft.VisualStudio.Services.Identity.Identity identity1 = vssRequestContext2.GetService<IdentityService>().ReadIdentities(vssRequestContext2, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
        {
          GroupWellKnownIdentityDescriptors.UsersGroup
        }, QueryMembership.Direct, (IEnumerable<string>) null).SingleOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
        IEnumerable<IdentityDescriptor> identityDescriptors;
        if (identity1 == null)
        {
          identityDescriptors = (IEnumerable<IdentityDescriptor>) null;
        }
        else
        {
          ICollection<IdentityDescriptor> members = identity1.Members;
          identityDescriptors = members != null ? members.Where<IdentityDescriptor>((Func<IdentityDescriptor, bool>) (x => AadIdentityHelper.IsAadGroup(x))) : (IEnumerable<IdentityDescriptor>) null;
        }
        IEnumerable<IdentityDescriptor> source = identityDescriptors;
        if (source == null || !source.Any<IdentityDescriptor>())
        {
          requestContext.Trace(584953, TraceLevel.Info, "IdentityService", nameof (IdentityHelper), "Return false because no aad groups found in organization users group.");
          return false;
        }
        foreach (IdentityDescriptor groupDescriptor in source)
        {
          if (service1.IsMember(vssRequestContext1, groupDescriptor, identity.Descriptor))
            return true;
        }
        requestContext.Trace(584952, TraceLevel.Info, "IdentityService", nameof (IdentityHelper), "IsMember check failed. {0} is most likely not a member of the collection and we will need to consult AAD for membership info.", (object) identity.Descriptor);
        List<Guid> list = source.Select<IdentityDescriptor, Guid>((Func<IdentityDescriptor, Guid>) (x => AadIdentityHelper.ExtractAadGroupId(x))).ToList<Guid>();
        if (extension == null)
        {
          requestContext.Trace(584951, TraceLevel.Error, "IdentityService", nameof (IdentityHelper), "Could not get the IAadTenantDetailProvider extension.");
          return false;
        }
        return identity.IsAADServicePrincipal ? extension.IsServicePrincipalMemberOfAadGroups(requestContext, aadUserId, list) : extension.IsUserMemberOfAadGroups(requestContext, aadUserId, list);
      }
      bool isMemberResultFromAad = false;
      try
      {
        IVssRequestContext vssRequestContext3 = requestContext.To(TeamFoundationHostType.Deployment);
        NameResolutionEntry entry = vssRequestContext3.GetService<INameResolutionService>().QueryEntry(vssRequestContext3, "EnterpriseUsersAadGroup", "AADGroupOid-MicrosoftTenant");
        if (entry == null || entry.Value == Guid.Empty)
        {
          requestContext.TraceConditionally(584951, TraceLevel.Info, "IdentityService", nameof (IdentityHelper), (Func<string>) (() => string.Format("NameResolutionService return null for namespace {0} and key {1} in host {2}.", (object) "EnterpriseUsersAadGroup", (object) "AADGroupOid-MicrosoftTenant", (object) requestContext.ServiceHost.InstanceId)));
          return false;
        }
        IdentityDescriptor descriptor = IdentityHelper.CreateDescriptorFromSid(SidIdentityHelper.ConstructAadGroupSid(entry.Value));
        if (service1.IsMember(vssRequestContext1, descriptor, identity.Descriptor))
        {
          requestContext.TraceConditionally(584952, TraceLevel.Info, "IdentityService", nameof (IdentityHelper), (Func<string>) (() => string.Format("IsMember is true for group {0} for userIdentity {1} in host {2}.", (object) descriptor, (object) identity.Descriptor, (object) requestContext.ServiceHost.InstanceId)));
          return true;
        }
        IVssRequestContext organizationRequestContext = vssRequestContext1.To(TeamFoundationHostType.Application);
        IdentityService service2 = organizationRequestContext.GetService<IdentityService>();
        if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Identity.MaterializeEnterpriseUserAADGroupInADO") && service2.IsMember(organizationRequestContext, descriptor, identity.Descriptor))
        {
          organizationRequestContext.TraceConditionally(584953, TraceLevel.Info, "IdentityService", nameof (IdentityHelper), (Func<string>) (() => string.Format("IsMember is true for group {0} for userIdentity {1} in host {2}.", (object) descriptor, (object) identity.Descriptor, (object) organizationRequestContext.ServiceHost.InstanceId)));
          return true;
        }
        if (identity.IsAADServicePrincipal)
          isMemberResultFromAad = (extension.IsServicePrincipalMemberOfAadGroups(requestContext, aadUserId, new List<Guid>()
          {
            entry.Value
          }) ? 1 : 0) != 0;
        else
          isMemberResultFromAad = (extension.IsUserMemberOfAadGroups(requestContext, aadUserId, new List<Guid>()
          {
            entry.Value
          }) ? 1 : 0) != 0;
        if (isMemberResultFromAad && requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Identity.MaterializeEnterpriseUserAADGroupInADO"))
        {
          bool isGroupCreated;
          Microsoft.VisualStudio.Services.Identity.Identity enterpriseUserAadGroup = IdentityHelper.EnsureAadGroupExistsInIMS(organizationRequestContext, service2, entry.Value, out isGroupCreated);
          if (enterpriseUserAadGroup == null)
            requestContext.TraceConditionally(584954, TraceLevel.Warning, "IdentityService", nameof (IdentityHelper), (Func<string>) (() => string.Format("Cannot materialize AAD group {0} in host {1}.", (object) entry.Value, (object) requestContext.ServiceHost.InstanceId)));
          else if (isGroupCreated)
          {
            service2.AddMemberToGroup(organizationRequestContext, GroupWellKnownIdentityDescriptors.UsersGroup, enterpriseUserAadGroup);
            service2.RemoveMemberFromGroup(organizationRequestContext, GroupWellKnownIdentityDescriptors.UsersGroup, enterpriseUserAadGroup.Descriptor);
            requestContext.TraceConditionally(584957, TraceLevel.Info, "IdentityService", nameof (IdentityHelper), (Func<string>) (() => string.Format("Materialize {0} for userIdentity {1} in host {2}.", (object) enterpriseUserAadGroup.Descriptor, (object) aadUserId, (object) organizationRequestContext.ServiceHost.InstanceId)));
          }
        }
        else if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Identity.LogMsftAllAccountsDeactivation"))
        {
          try
          {
            IdentityHelper.AadGroupCacheService service3 = vssRequestContext3.GetService<IdentityHelper.AadGroupCacheService>();
            bool aadGroupExists;
            if (!service3.TryGetValue(vssRequestContext3, entry.Value, out aadGroupExists))
            {
              aadGroupExists = true;
              if (extension.GetGroupFromAad(requestContext, entry.Value) == null)
              {
                requestContext.TraceAlways(584958, TraceLevel.Info, "IdentityService", nameof (IdentityHelper), string.Format("Unable to resolve the aadGroup for org {0} with oid {1}", (object) requestContext.ServiceHost.InstanceId, (object) entry.Value));
                aadGroupExists = false;
              }
              service3.Set(vssRequestContext3, entry.Value, aadGroupExists);
            }
          }
          catch (Exception ex)
          {
            requestContext.TraceException(584959, TraceLevel.Warning, "IdentityService", nameof (IdentityHelper), ex);
          }
        }
        return isMemberResultFromAad;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(584955, TraceLevel.Warning, "IdentityService", nameof (IdentityHelper), ex);
      }
      requestContext.TraceConditionally(584956, TraceLevel.Info, "IdentityService", nameof (IdentityHelper), (Func<string>) (() => string.Format("IsOrgUser resulted in {0} for userIdentity {1} in host {2}.", (object) isMemberResultFromAad, (object) aadUserId, (object) requestContext.ServiceHost.InstanceId)));
      return isMemberResultFromAad;
    }

    private static Microsoft.VisualStudio.Services.Identity.Identity EnsureAadGroupExistsInIMS(
      IVssRequestContext organizationContext,
      IdentityService identityService,
      Guid aadGroupObjectId,
      out bool isGroupCreated)
    {
      SecurityIdentifier securityId = SidIdentityHelper.ConstructAadGroupSid(aadGroupObjectId);
      IdentityDescriptor descriptorFromSid = IdentityHelper.CreateDescriptorFromSid(securityId);
      Microsoft.VisualStudio.Services.Identity.Identity identity = identityService.ReadIdentities(organizationContext, (IList<IdentityDescriptor>) new List<IdentityDescriptor>()
      {
        descriptorFromSid
      }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      isGroupCreated = false;
      if (identity != null && !identity.GetProperty<bool>("IsGroupDeleted", false))
        return identity;
      IAadTenantDetailProvider extension = organizationContext.GetExtension<IAadTenantDetailProvider>();
      AadGroup groupFromAad;
      try
      {
        groupFromAad = extension.GetGroupFromAad(organizationContext, aadGroupObjectId);
      }
      catch (Exception ex)
      {
        organizationContext.Trace(32112532, TraceLevel.Error, "IdentityService", nameof (IdentityHelper), string.Format("Unable to resolve the aadGroup for org {0}. ", (object) organizationContext.ServiceHost.InstanceId) + "aadService.GetGroupsWithIds failed with exception " + ex.Message + ".");
        return (Microsoft.VisualStudio.Services.Identity.Identity) null;
      }
      Microsoft.VisualStudio.Services.Identity.Identity group = identityService.CreateGroup(organizationContext, organizationContext.ServiceHost.InstanceId, securityId.ToString(), string.Join("", groupFromAad.DisplayName?.Split(SidIdentityHelper.IllegalNameChars, StringSplitOptions.RemoveEmptyEntries)), TFCommonUtil.TruncateGroupDescription(groupFromAad.Description), SpecialGroupType.AzureActiveDirectoryApplicationGroup);
      isGroupCreated = true;
      return group;
    }

    internal static IdentityTracingItems GenerateIdentityTracingItems(
      IVssRequestContext requestContext,
      IReadOnlyVssIdentity vssIdentity)
    {
      Guid tenantId;
      Guid providerId;
      return new IdentityTracingItems(IdentityCuidHelper.ComputeCuid(requestContext, vssIdentity, NameBasedGuidVersion.Seven, out tenantId, out providerId), tenantId, providerId);
    }

    internal static string QualifyAccountName(IVssRequestContext requestContext, string factorValue)
    {
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        requestContext.Trace(80460, TraceLevel.Verbose, "IdentityService", nameof (IdentityHelper), "Nothing to do - cannot qualify factorValue at the deployment level");
        return factorValue;
      }
      string[] strArray = factorValue.Split('@');
      if (strArray.Length != 2)
      {
        requestContext.Trace(80460, TraceLevel.Verbose, "IdentityService", nameof (IdentityHelper), "Nothing to do - factorValue is not qualifiable");
        return factorValue;
      }
      if (Guid.TryParse(strArray[0], out Guid _) || Guid.TryParse(strArray[1], out Guid _))
      {
        requestContext.Trace(80460, TraceLevel.Verbose, "IdentityService", nameof (IdentityHelper), "Nothing to do - factorValue is a service identity");
        return factorValue;
      }
      if (factorValue.Contains("\\"))
      {
        requestContext.Trace(80460, TraceLevel.Verbose, "IdentityService", nameof (IdentityHelper), "Nothing to do - factorValue is already qualified");
        return factorValue;
      }
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return factorValue;
      requestContext = requestContext.To(TeamFoundationHostType.Application);
      Guid organizationAadTenantId = requestContext.GetOrganizationAadTenantId();
      if (organizationAadTenantId == Guid.Empty)
      {
        if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Identity.UseQualifiedAccountNameForMSA"))
        {
          requestContext.Trace(80460, TraceLevel.Verbose, "IdentityService", nameof (IdentityHelper), "Qualifying factorValue with WLID");
          return string.Format("{0}\\{1}", (object) "Windows Live ID", (object) factorValue);
        }
        requestContext.Trace(80461, TraceLevel.Verbose, "IdentityService", nameof (IdentityHelper), "Nothing to do - MSA orgs need to search both MSA and AAD identities");
        return factorValue;
      }
      requestContext.Trace(80460, TraceLevel.Verbose, "IdentityService", nameof (IdentityHelper), string.Format("Qualifying factorValue with {0}", (object) organizationAadTenantId));
      return string.Format("{0}\\{1}", (object) organizationAadTenantId, (object) factorValue);
    }

    internal static bool TryResolveMasterId(
      IVssRequestContext requestContext,
      Guid identityId,
      out Guid masterId)
    {
      masterId = new Guid();
      if (identityId != new Guid() && requestContext.RootContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
      {
        IVssRequestContext rootContext = requestContext.RootContext;
        Microsoft.VisualStudio.Services.Identity.Identity identity = rootContext.GetService<IdentityService>().ReadIdentities(rootContext, (IList<Guid>) new Guid[1]
        {
          identityId
        }, QueryMembership.None, (IEnumerable<string>) null).Single<Microsoft.VisualStudio.Services.Identity.Identity>();
        if (identity != null)
        {
          IVssRequestContext vssRequestContext = rootContext.To(TeamFoundationHostType.Application);
          Microsoft.VisualStudio.Services.Identity.Identity readIdentity = vssRequestContext.GetService<IdentityService>().ReadIdentities(vssRequestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
          {
            identity.Descriptor
          }, QueryMembership.None, (IEnumerable<string>) null)[0];
          if (readIdentity != null)
          {
            masterId = readIdentity.Id;
            return true;
          }
        }
      }
      return false;
    }

    internal static void ValidateIdentityTranslation(
      IVssRequestContext requestContext,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities,
      int tracePointForAadBacked,
      int tracePointForMsaBacked,
      string ffNameToCheck = null)
    {
      try
      {
        if (requestContext == null || identities == null || !identities.Any<Microsoft.VisualStudio.Services.Identity.Identity>() || !string.IsNullOrEmpty(ffNameToCheck) && !requestContext.IsFeatureEnabled(ffNameToCheck) || requestContext.IsPublicResourceLicense() || requestContext.ExecutionEnvironment.IsOnPremisesDeployment || !requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
          return;
        if (requestContext.IsOrganizationAadBacked())
        {
          List<\u003C\u003Ef__AnonymousType131<Guid, Guid, IdentityDescriptor, bool>> badTranslatedIdentities = identities.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x != null && x.IsMsaIdentity())).Select(x => new
          {
            Id = x.Id,
            MasterId = x.MasterId,
            Descriptor = x.Descriptor,
            IsActive = x.IsActive
          }).ToList();
          if (!badTranslatedIdentities.Any())
            return;
          string stackTrace = EnvironmentWrapper.ToReadableStackTrace();
          requestContext.TraceDataConditionally(tracePointForAadBacked, TraceLevel.Error, "IdentityService", nameof (IdentityHelper), "Got MSA identities in AAD backed account (translation cache issue)", (Func<object>) (() => (object) new
          {
            badTranslatedIdentities = badTranslatedIdentities,
            stackTrace = stackTrace
          }), nameof (ValidateIdentityTranslation));
        }
        else
        {
          List<\u003C\u003Ef__AnonymousType131<Guid, Guid, IdentityDescriptor, bool>> badTranslatedIdentities = identities.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x != null && x.IsExternalUser)).Select(x => new
          {
            Id = x.Id,
            MasterId = x.MasterId,
            Descriptor = x.Descriptor,
            IsActive = x.IsActive
          }).ToList();
          if (!badTranslatedIdentities.Any())
            return;
          string stackTrace = EnvironmentWrapper.ToReadableStackTrace();
          requestContext.TraceDataConditionally(tracePointForMsaBacked, TraceLevel.Error, "IdentityService", nameof (IdentityHelper), "Got AAD identities in MSA backed account (translation cache issue)", (Func<object>) (() => (object) new
          {
            badTranslatedIdentities = badTranslatedIdentities,
            stackTrace = stackTrace
          }), nameof (ValidateIdentityTranslation));
        }
      }
      catch (OrganizationNotFoundException ex)
      {
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1754848, TraceLevel.Warning, "IdentityService", nameof (IdentityHelper), ex);
      }
    }

    internal static void GetUserOriginAndOriginId(
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      out string origin,
      out string originId)
    {
      if (identity.Descriptor.IsWindowsType())
      {
        origin = "ad";
        originId = identity.Descriptor.Identifier;
      }
      else if (identity.IsExternalUser)
      {
        origin = "aad";
        originId = identity.GetProperty<string>("http://schemas.microsoft.com/identity/claims/objectidentifier", (string) null);
      }
      else if (identity.SocialDescriptor != new SocialDescriptor() && identity.SocialDescriptor.IsGitHubSocialType())
      {
        origin = "ghb";
        originId = identity.SocialDescriptor.Identifier;
      }
      else if (identity.GetProperty<string>("Domain", (string) null) == "Windows Live ID")
      {
        origin = "msa";
        originId = identity.GetProperty<string>("PUID", (string) null);
      }
      else
      {
        origin = "vsts";
        originId = identity.Id.ToString();
      }
    }

    private static bool IsWindowsLocalServiceIdentity(
      IVssRequestContext requestContext,
      IReadOnlyVssIdentity identity)
    {
      if (!requestContext.ExecutionEnvironment.IsOnPremisesDeployment || !string.Equals(identity.Descriptor.IdentityType, "System.Security.Principal.WindowsIdentity", StringComparison.Ordinal))
        return false;
      return VssStringComparer.SID.Equals(identity.Descriptor.Identifier, "S-1-5-18") || VssStringComparer.SID.Equals(identity.Descriptor.Identifier, "S-1-5-19") || VssStringComparer.SID.Equals(identity.Descriptor.Identifier, "S-1-5-20");
    }

    private static bool IsWindowsRemoteServiceIdentity(
      IVssRequestContext requestContext,
      IReadOnlyVssIdentity identity)
    {
      if (!requestContext.ExecutionEnvironment.IsOnPremisesDeployment || !string.Equals(identity.Descriptor.IdentityType, "System.Security.Principal.WindowsIdentity", StringComparison.Ordinal))
        return false;
      string property = identity.GetProperty<string>("Account", (string) null);
      return property != null && property.EndsWith("$");
    }

    public static bool IsAadServicePrincipalIdentity(IReadOnlyVssIdentity identity) => identity.Descriptor.IsAadServicePrincipalType();

    private static bool TryGetPuidBytes(string puidString, out byte[] puidBytes)
    {
      puidBytes = (byte[]) null;
      long result;
      if (string.IsNullOrWhiteSpace(puidString) || !long.TryParse(puidString, NumberStyles.AllowHexSpecifier, (IFormatProvider) null, out result))
        return false;
      puidBytes = BitConverter.GetBytes(result);
      return true;
    }

    private static Guid GetPuidBasedProviderId(string puidString) => Guid.Parse(puidString.PadLeft(32, '0'));

    private static void ValidateMemberArgs(IDirectoryEntityDescriptor member)
    {
      if (member.OriginId.IsNullOrEmpty<char>() && member.PrincipalName.IsNullOrEmpty<char>())
        throw new ArgumentException("To add a new identity, it must have at least one of the following values: OriginId, PrincipalName");
    }

    public static void ValidatePermissionsToMaterializeMember(
      IVssRequestContext requestContext,
      IDirectoryEntityDescriptor member)
    {
      if (!IdentityHelper.CheckIsProjectCollectionAdministratorOrOwner(requestContext) && !IdentityHelper.c_genevaActionServiceInstanceType.Equals(requestContext.GetAuthenticatedId()) && !IdentityHelper.CheckMemberAlreadyMaterialized(requestContext, member))
      {
        string str = "The identity with origin Id: " + member?.OriginId + " could not be added because you do not have permissions to add new users to the organization.If you'd like to add these users please contact a Project Collection Administrator. ";
        requestContext.TraceSerializedConditionally(80859, TraceLevel.Error, "IdentityService", nameof (IdentityHelper), str);
        throw new IdentityMaterializationFailedException(str, (Exception) null);
      }
    }

    private static bool CheckIsProjectCollectionAdministratorOrOwner(
      IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Application);
      return vssRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, FrameworkSecurity.IdentitiesNamespaceId).HasPermission(vssRequestContext, requestContext.ServiceHost.InstanceId.ToString(), 31);
    }

    private static bool CheckMemberAlreadyMaterialized(
      IVssRequestContext requestContext,
      IDirectoryEntityDescriptor member)
    {
      IdentityHelper.ValidateMemberArgs(member);
      IIdentityServiceInternal identityServiceInternal = requestContext.GetService<IdentityService>().IdentityServiceInternal();
      string factorValue = member.EntityType == "ServicePrincipal" ? member.OriginId : member.PrincipalName;
      if (factorValue == null)
        return true;
      IList<Microsoft.VisualStudio.Services.Identity.Identity> source = identityServiceInternal.ReadIdentities(requestContext, IdentitySearchFilter.AccountName, factorValue, QueryMembership.None, (IEnumerable<string>) null);
      Microsoft.VisualStudio.Services.Identity.Identity identity = source != null ? source.FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>() : (Microsoft.VisualStudio.Services.Identity.Identity) null;
      return identity != null && identityServiceInternal.IsMember(requestContext, GroupWellKnownIdentityDescriptors.EveryoneGroup, identity.Descriptor);
    }

    internal class AadGroupCacheService : VssCacheService
    {
      private VssMemoryCacheList<Guid, bool> m_cache;

      protected override void ServiceStart(IVssRequestContext systemRequestContext)
      {
        if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
          throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
        base.ServiceStart(systemRequestContext);
        this.m_cache = new VssMemoryCacheList<Guid, bool>((IVssCachePerformanceProvider) this, new VssCacheExpiryProvider<Guid, bool>(Capture.Create<TimeSpan>(TimeSpan.FromHours(4.0)), Capture.Create<TimeSpan>(TimeSpan.FromMinutes(15.0))));
      }

      protected override void ServiceEnd(IVssRequestContext systemRequestContext)
      {
        this.m_cache.Clear();
        base.ServiceEnd(systemRequestContext);
      }

      public bool TryGetValue(
        IVssRequestContext requestContext,
        Guid groupOid,
        out bool aadGroupExists)
      {
        return this.m_cache.TryGetValue(groupOid, out aadGroupExists);
      }

      public void Set(IVssRequestContext requestContext, Guid groupOid, bool aadGroupExists) => this.m_cache[groupOid] = aadGroupExists;
    }
  }
}
