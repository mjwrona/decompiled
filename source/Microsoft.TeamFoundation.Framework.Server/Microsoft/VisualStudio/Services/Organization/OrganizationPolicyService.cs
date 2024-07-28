// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.OrganizationPolicyService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Organization
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class OrganizationPolicyService : IOrganizationPolicyService, IVssFrameworkService
  {
    private Guid m_serviceHostId;
    private const string c_area = "Organization";
    private const string c_layer = "OrganizationPolicyService";
    private const string BypassCollectionCache = "VisualStudio.Services.Organization.BypassCollectionCache";
    private const string s_checkCleanDisallowAadGuestUserAccessPolicy = "Microsoft.VisualStudio.Services.Identity.CleanDisallowAadGuestUserAccessPolicy";
    private const string c_enforceToken = "Enforce";
    private static readonly ActionTracer s_tracer = new ActionTracer("Organization", nameof (OrganizationPolicyService));
    private static readonly IReadOnlyDictionary<string, PolicyInfo> s_knownPolicyInfos = (IReadOnlyDictionary<string, PolicyInfo>) new Dictionary<string, PolicyInfo>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        "Policy.DisallowBasicAuthentication",
        new PolicyInfo("Policy.DisallowBasicAuthentication", FrameworkResources.PolicyDisallowBasicAuthenticationDescription(), new Uri(FrameworkResources.PolicyDisallowBasicAuthenticationMoreInfoLink()))
      },
      {
        "Policy.DisallowOAuthAuthentication",
        new PolicyInfo("Policy.DisallowOAuthAuthentication", FrameworkResources.PolicyDisallowOAuthAuthenticationDescription(), new Uri(FrameworkResources.PolicyDisallowOAuthAuthenticationMoreInfoLink()))
      },
      {
        "Policy.DisallowAadGuestUserAccess",
        new PolicyInfo("Policy.DisallowAadGuestUserAccess", FrameworkResources.PolicyDisallowAadGuestUserAccessDescription(), new Uri(FrameworkResources.PolicyDisallowAadGuestUserAccessMoreInfoLink()))
      },
      {
        "Policy.DisallowSecureShell",
        new PolicyInfo("Policy.DisallowSecureShell", FrameworkResources.PolicyDisallowSecureShellDescription(), new Uri(FrameworkResources.PolicyDisallowSecureShellMoreInfoLink()))
      },
      {
        "Policy.AllowAnonymousAccess",
        new PolicyInfo("Policy.AllowAnonymousAccess", FrameworkResources.PolicyAllowAnonymousAccessDescription(), new Uri(FrameworkResources.PolicyAllowAnonymousAccessMoreInfoLink()))
      },
      {
        "Policy.AllowOrgAccess",
        new PolicyInfo("Policy.AllowOrgAccess", FrameworkResources.PolicyAllowOrgAccessDescription(), new Uri(FrameworkResources.PolicyEmptyInfoLink()))
      },
      {
        "Policy.EnforceAADConditionalAccess",
        new PolicyInfo("Policy.EnforceAADConditionalAccess", FrameworkResources.PolicyConditionalAccessPolicyDescription(), new Uri(FrameworkResources.PolicyConditionalAccessPolicyLink()))
      },
      {
        "Policy.AllowGitHubInvitationsAccessToken",
        new PolicyInfo("Policy.AllowGitHubInvitationsAccessToken", FrameworkResources.PolicyGitHubInviteDescription(), new Uri("https://aka.ms/InviteGHUsers"))
      },
      {
        "Policy.AllowRequestAccessToken",
        new PolicyInfo("Policy.AllowRequestAccessToken", FrameworkResources.PolicyAllowRequestAccessDescription(), new Uri(FrameworkResources.PolicyAllowRequestAccessLink()))
      },
      {
        "Policy.AllowTeamAdminsInvitationsAccessToken",
        new PolicyInfo("Policy.AllowTeamAdminsInvitationsAccessToken", FrameworkResources.PolicyTeamAdminsInvitationsDescription(), new Uri(FrameworkResources.PolicyTeamAdminsInvitationsLink()))
      },
      {
        "Policy.ArtifactsExternalPackageProtectionToken",
        new PolicyInfo("Policy.ArtifactsExternalPackageProtectionToken", FrameworkResources.PolicyExternalPackageProtectionDescription(), new Uri(FrameworkResources.PolicyExternalPackageProtectionLink()))
      },
      {
        "Policy.LogAuditEvents",
        new PolicyInfo("Policy.LogAuditEvents", FrameworkResources.PolicyLogAuditEventsDescription(), new Uri(FrameworkResources.PolicyLogAuditEventsLink()))
      }
    };

    public void ServiceStart(IVssRequestContext context)
    {
      context.CheckHostedDeployment();
      if (context.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(context.ServiceHost.HostType);
      this.m_serviceHostId = context.ServiceHost.InstanceId;
    }

    public void ServiceEnd(IVssRequestContext context)
    {
    }

    public PolicyInfo GetPolicyInfo(IVssRequestContext context, string policyName)
    {
      this.ValidateRequestContext(context);
      PolicyArgumentValidator.ValidatePolicyName(policyName);
      PolicyInfo policyInfo;
      return OrganizationPolicyService.s_knownPolicyInfos.TryGetValue(policyName, out policyInfo) ? policyInfo : new PolicyInfo(policyName, (string) null, (Uri) null);
    }

    public Policy<T> GetPolicy<T>(IVssRequestContext context, string policyName, T defaultValue)
    {
      this.ValidateRequestContext(context);
      PolicyArgumentValidator.ValidatePolicyName(policyName);
      PolicyArgumentValidator.ValidatePolicyValueType<T>();
      PolicyArgumentValidator.ValidatePolicyNameSize(policyName);
      return OrganizationPolicyService.s_tracer.TraceAction<Policy<T>>(context, (ActionTracePoints) OrganizationPolicyService.TracePoints.GetPolicy, (Func<Policy<T>>) (() =>
      {
        string enforceKey = OrganizationPolicyService.GetEnforceKey(policyName);
        IVssRequestContext context1 = context.To(TeamFoundationHostType.Application);
        IOrganizationService service = context1.GetService<IOrganizationService>();
        IVssRequestContext context2 = context1;
        string[] propertyNames = new string[2]
        {
          policyName,
          enforceKey
        };
        Policy<T> policy = OrganizationPolicyService.ConvertToPolicy<T>(context, service.GetOrganization(context2, (IEnumerable<string>) propertyNames)?.Properties, policyName, enforceKey, defaultValue, (Policy<T>) null);
        if (context.ServiceHost.Is(TeamFoundationHostType.Application))
          return policy;
        if (context.IsFeatureEnabled("VisualStudio.Services.Organization.BypassCollectionCache"))
          context.Items.TryAdd<string, object>("Organization.Cache.Bypass", (object) true);
        Collection collection = context.GetService<ICollectionService>().GetCollection(context, (IEnumerable<string>) new string[2]
        {
          policyName,
          enforceKey
        });
        context.Items.Remove("Organization.Cache.Bypass");
        return OrganizationPolicyService.ConvertToPolicy<T>(context, collection?.Properties, policyName, enforceKey, defaultValue, policy);
      }), nameof (GetPolicy));
    }

    public void SetPolicyValue<T>(IVssRequestContext context, string policyName, T value)
    {
      this.ValidateRequestContext(context);
      PolicyArgumentValidator.ValidatePolicyName(policyName);
      PolicyArgumentValidator.ValidatePolicyValue<T>(value);
      PolicyArgumentValidator.ValidatePolicyNameSize(policyName);
      OrganizationPolicyService.s_tracer.TraceAction(context, (ActionTracePoints) OrganizationPolicyService.TracePoints.SetPolicyValue, (Action) (() =>
      {
        PropertyBag properties = new PropertyBag()
        {
          [policyName] = (object) (T) value
        };
        if (context.IsFeatureEnabled("Microsoft.VisualStudio.Services.Identity.CleanDisallowAadGuestUserAccessPolicy") && policyName == "Policy.DisallowAadGuestUserAccess")
          OrganizationPolicyService.CheckDisallowAadGuestUserAccessPolicy<T>(context, policyName, value);
        if (context.ServiceHost.Is(TeamFoundationHostType.Application))
          context.GetService<IOrganizationService>().UpdateProperties(context, properties);
        else
          context.GetService<ICollectionService>().UpdateProperties(context, properties);
        context.LogAuditEvent("OrganizationPolicy.PolicyValueUpdated", new Dictionary<string, object>()
        {
          {
            "PolicyName",
            (object) policyName
          },
          {
            "PolicyValue",
            string.Equals(bool.TrueString, value.ToString(), StringComparison.OrdinalIgnoreCase) ? (object) "ON" : (object) "OFF"
          }
        });
      }), nameof (SetPolicyValue));
    }

    public void SetPolicyEnforcementValue(
      IVssRequestContext context,
      string policyName,
      bool enforce)
    {
      this.ValidateRequestContext(context);
      if (!context.ServiceHost.IsOnly(TeamFoundationHostType.Application))
        throw new OrganizationBadRequestException("This operation is only supported on organization level");
      PolicyArgumentValidator.ValidatePolicyName(policyName);
      PolicyArgumentValidator.ValidatePolicyNameSize(policyName);
      OrganizationPolicyService.s_tracer.TraceAction(context, (ActionTracePoints) OrganizationPolicyService.TracePoints.SetPolicyOverrideValue, (Action) (() =>
      {
        string enforceKey = OrganizationPolicyService.GetEnforceKey(policyName);
        IOrganizationService service = context.GetService<IOrganizationService>();
        bool flag;
        service.GetOrganization(context, (IEnumerable<string>) new string[1]
        {
          enforceKey
        }).Properties.TryGetValue<bool>(enforceKey, out flag);
        if (flag == enforce)
          return;
        Dictionary<string, object> data = new Dictionary<string, object>()
        {
          {
            "EnforcePolicyName",
            (object) enforceKey
          }
        };
        if (!enforce)
        {
          service.DeleteProperties(context, (IEnumerable<string>) new string[1]
          {
            enforceKey
          });
          context.LogAuditEvent("OrganizationPolicy.EnforcePolicyRemoved", data);
        }
        else
        {
          PropertyBag properties = new PropertyBag()
          {
            [enforceKey] = (object) true
          };
          service.UpdateProperties(context, properties);
          context.LogAuditEvent("OrganizationPolicy.EnforcePolicyAdded", data);
        }
      }), nameof (SetPolicyEnforcementValue));
    }

    private void ValidateRequestContext(IVssRequestContext context)
    {
      context.CheckHostedDeployment();
      context.CheckServiceHostId(this.m_serviceHostId, (IVssFrameworkService) this);
    }

    internal static string GetEnforceKey(string policyName) => policyName + ".Enforce";

    internal static Policy<T> ConvertToPolicy<T>(
      IVssRequestContext context,
      PropertyBag properties,
      string policyName,
      string enforceKey,
      T defaultValue,
      Policy<T> parentPolicy)
    {
      bool isPolicyValueUndefined;
      T policyValue;
      OrganizationPolicyService.TryGetPolicyValue<T>(context, properties, policyName, defaultValue, out isPolicyValueUndefined, out policyValue);
      bool enforce = false;
      properties?.TryGetValue<bool>(enforceKey, out enforce);
      return new Policy<T>(policyName, policyValue, enforce, isPolicyValueUndefined, parentPolicy);
    }

    private static void TryGetPolicyValue<T>(
      IVssRequestContext context,
      PropertyBag properties,
      string policyName,
      T defaultValue,
      out bool isPolicyValueUndefined,
      out T policyValue)
    {
      if (properties == null)
      {
        isPolicyValueUndefined = true;
        policyValue = defaultValue;
      }
      else if (properties.TryGetValue<T>(policyName, out policyValue))
      {
        isPolicyValueUndefined = false;
      }
      else
      {
        object obj;
        if (properties.TryGetValue(policyName, out obj))
        {
          if (obj != null)
          {
            try
            {
              isPolicyValueUndefined = false;
              policyValue = RegistryUtility.FromString<T>(obj.ToString());
              return;
            }
            catch (Exception ex)
            {
              OrganizationPolicyService.s_tracer.TraceException(context, 8520015, ex, nameof (TryGetPolicyValue));
              isPolicyValueUndefined = true;
              policyValue = defaultValue;
              return;
            }
          }
        }
        isPolicyValueUndefined = true;
        policyValue = defaultValue;
      }
    }

    private static void CheckDisallowAadGuestUserAccessPolicyHelper<T>(
      IVssRequestContext enterpriseContext,
      IVssRequestContext collectionContext,
      string policyName,
      T value)
    {
      string enforceKey = OrganizationPolicyService.GetEnforceKey(policyName);
      IOrganizationService service1 = enterpriseContext.GetService<IOrganizationService>();
      Microsoft.VisualStudio.Services.Organization.Organization organization = service1.GetOrganization(enterpriseContext, (IEnumerable<string>) new string[2]
      {
        policyName,
        enforceKey
      });
      Policy<T> policy1 = OrganizationPolicyService.ConvertToPolicy<T>(enterpriseContext, organization?.Properties, policyName, enforceKey, value, (Policy<T>) null);
      ICollectionService service2 = collectionContext.GetService<ICollectionService>();
      Collection collection = service2.GetCollection(collectionContext, (IEnumerable<string>) new string[2]
      {
        policyName,
        enforceKey
      });
      Policy<T> policy2 = OrganizationPolicyService.ConvertToPolicy<T>(collectionContext, collection?.Properties, policyName, enforceKey, value, (Policy<T>) null);
      if (policy1.IsValueUndefined || policy2.IsValueUndefined)
        return;
      service2.DeleteProperties(collectionContext, (IEnumerable<string>) new List<string>()
      {
        "Policy.DisallowAadGuestUserAccess"
      });
      service1.DeleteProperties(enterpriseContext, (IEnumerable<string>) new List<string>()
      {
        "Policy.DisallowAadGuestUserAccess"
      });
      enterpriseContext.TraceAlways(8520025, TraceLevel.Info, "Organization", nameof (OrganizationPolicyService), "DisallowAadGuestUserAccess had value at collection and enterprise level");
    }

    private static void CheckDisallowAadGuestUserAccessPolicy<T>(
      IVssRequestContext context,
      string policyName,
      T value)
    {
      try
      {
        if (context.ServiceHost.Is(TeamFoundationHostType.Application))
        {
          Microsoft.VisualStudio.Services.Organization.Organization organization = context.GetService<IOrganizationService>().GetOrganization(context, (IEnumerable<string>) null);
          if (organization.Collections.Count > 1)
          {
            context.TraceAlways(8520025, TraceLevel.Error, "Organization", nameof (OrganizationPolicyService), "Cleaning the policy is not supported on enterprises with more than 1 collection");
          }
          else
          {
            IVssRequestContext vssRequestContext = context.To(TeamFoundationHostType.Deployment);
            using (IVssRequestContext collectionContext = vssRequestContext.GetService<ITeamFoundationHostManagementService>().BeginRequest(vssRequestContext, organization.Collections.First<CollectionRef>().Id, RequestContextType.ServicingContext, throwIfShutdown: false))
              OrganizationPolicyService.CheckDisallowAadGuestUserAccessPolicyHelper<T>(context, collectionContext, policyName, value);
          }
        }
        else
          OrganizationPolicyService.CheckDisallowAadGuestUserAccessPolicyHelper<T>(context.To(TeamFoundationHostType.Application), context, policyName, value);
      }
      catch (Exception ex)
      {
        OrganizationPolicyService.s_tracer.TraceException(context, 8520020, ex, nameof (CheckDisallowAadGuestUserAccessPolicy));
      }
    }

    private static class TracePoints
    {
      internal static readonly TimedActionTracePoints GetPolicy = new TimedActionTracePoints(8520010, 8520017, 8520018, 8520019);
      internal static readonly TimedActionTracePoints SetPolicyValue = new TimedActionTracePoints(8520020, 8520027, 8520028, 8520029);
      internal static readonly TimedActionTracePoints SetPolicyOverrideValue = new TimedActionTracePoints(8520030, 8520037, 8520038, 8520039);
    }
  }
}
