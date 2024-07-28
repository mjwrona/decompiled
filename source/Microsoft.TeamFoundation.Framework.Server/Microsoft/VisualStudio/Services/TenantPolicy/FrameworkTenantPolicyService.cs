// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TenantPolicy.FrameworkTenantPolicyService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.VisualStudio.Services.TenantPolicy.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.TenantPolicy
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class FrameworkTenantPolicyService : ITenantPolicyService, IVssFrameworkService
  {
    private Guid m_serviceHostId;
    private static readonly PerformanceTracer s_tracer = new PerformanceTracer(OrganizationPerfCounters.StandardSet, "TenantPolicy", nameof (FrameworkTenantPolicyService));
    private const string c_area = "TenantPolicy";
    private const string c_layer = "FrameworkTenantPolicyService";

    public void ServiceStart(IVssRequestContext context)
    {
      context.CheckHostedDeployment();
      this.m_serviceHostId = context.ServiceHost.InstanceId;
    }

    public void ServiceEnd(IVssRequestContext context)
    {
    }

    public Policy GetPolicy(IVssRequestContext context, string policyName)
    {
      if (!context.IsFeatureEnabled("VisualStudio.Services.TenantPolicy.EnableTenantPolicyService"))
        return (Policy) null;
      this.ValidateRequestContext(context);
      Guid targetTenantId = context.GetOrganizationAadTenantId();
      PolicyDataValidator.ValidatePolicyTenantId(targetTenantId);
      PolicyDataValidator.ValidatePolicyName(policyName);
      if (TenantPolicyRolloutHelper.IsPolicyFeatureAvailable(context, policyName, targetTenantId))
        return FrameworkTenantPolicyService.s_tracer.TraceAction<Policy>(context, (ActionTracePoints) FrameworkTenantPolicyService.TracePoints.GetPolicy, (Func<Policy>) (() =>
        {
          Microsoft.VisualStudio.Services.TenantPolicy.Client.Policy policy = this.GetTenantPolicyHttpClient(context).GetPolicyAsync(policyName).SyncResult<Microsoft.VisualStudio.Services.TenantPolicy.Client.Policy>();
          context.TraceConditionally(8524101, TraceLevel.Info, "TenantPolicy", nameof (FrameworkTenantPolicyService), (Func<string>) (() =>
          {
            string str3 = string.Format("The {0} retrieved for tenant {1} is:", (object) policyName, (object) targetTenantId);
            Microsoft.VisualStudio.Services.TenantPolicy.Client.Policy policy1 = policy;
            string str4 = policy1 != null ? policy1.Serialize<Microsoft.VisualStudio.Services.TenantPolicy.Client.Policy>() : (string) null;
            return str3 + str4;
          }));
          return policy.ToServer();
        }), nameof (GetPolicy));
      context.Trace(8524102, TraceLevel.Info, "TenantPolicy", nameof (FrameworkTenantPolicyService), string.Format("TenantPolicy is not in preview for tenant {0}, request policy is {1}", (object) targetTenantId, (object) policyName));
      return (Policy) null;
    }

    public Policy GetPolicy(IVssRequestContext context, string policyName, Guid tenantId)
    {
      if (!context.IsFeatureEnabled("VisualStudio.Services.TenantPolicy.EnableTenantPolicyService"))
        return (Policy) null;
      this.ValidateRequestContext(context);
      PolicyDataValidator.ValidatePolicyTenantId(tenantId);
      PolicyDataValidator.ValidatePolicyName(policyName);
      if (TenantPolicyRolloutHelper.IsPolicyFeatureAvailable(context, policyName, tenantId))
        return FrameworkTenantPolicyService.s_tracer.TraceAction<Policy>(context, (ActionTracePoints) FrameworkTenantPolicyService.TracePoints.GetPolicyForTenant, (Func<Policy>) (() =>
        {
          Microsoft.VisualStudio.Services.TenantPolicy.Client.Policy policy = this.GetTenantPolicyHttpClient(context.To(TeamFoundationHostType.Deployment)).GetPolicyForTenantAsync(policyName, tenantId).SyncResult<Microsoft.VisualStudio.Services.TenantPolicy.Client.Policy>();
          context.TraceConditionally(8524101, TraceLevel.Info, "TenantPolicy", nameof (FrameworkTenantPolicyService), (Func<string>) (() =>
          {
            string str3 = string.Format("The {0} retrieved for tenant {1} is:", (object) policyName, (object) tenantId);
            Microsoft.VisualStudio.Services.TenantPolicy.Client.Policy policy1 = policy;
            string str4 = policy1 != null ? policy1.Serialize<Microsoft.VisualStudio.Services.TenantPolicy.Client.Policy>() : (string) null;
            return str3 + str4;
          }));
          return policy.ToServer();
        }), nameof (GetPolicy));
      context.Trace(8524102, TraceLevel.Info, "TenantPolicy", nameof (FrameworkTenantPolicyService), string.Format("TenantPolicy is not in preview for tenant {0}, request policy is {1}", (object) tenantId, (object) policyName));
      return (Policy) null;
    }

    public void SetPolicy(
      IVssRequestContext context,
      string policyName,
      Policy policy,
      Guid? tenantId = null)
    {
      if (!context.IsFeatureEnabled("VisualStudio.Services.TenantPolicy.EnableTenantPolicyService"))
        return;
      this.ValidateRequestContext(context);
      Guid targetTenantId = !tenantId.HasValue ? context.GetOrganizationAadTenantId() : tenantId.Value;
      PolicyDataValidator.ValidatePolicyTenantId(targetTenantId);
      PolicyDataValidator.ValidatePolicyNameAndData(policyName, policy);
      if (!TenantPolicyRolloutHelper.IsPolicyFeatureAvailable(context, policyName, targetTenantId))
        context.Trace(8524112, TraceLevel.Info, "TenantPolicy", nameof (FrameworkTenantPolicyService), string.Format("TenantPolicy is not in preview for tenant {0}, request policy is {1}", (object) targetTenantId, (object) policyName));
      else
        FrameworkTenantPolicyService.s_tracer.TraceAction(context, (ActionTracePoints) FrameworkTenantPolicyService.TracePoints.SetPolicy, (Action) (() =>
        {
          context.TraceConditionally(8524111, TraceLevel.Info, "TenantPolicy", nameof (FrameworkTenantPolicyService), (Func<string>) (() =>
          {
            string str3 = string.Format("The {0} for tenant {1} to update is:", (object) policyName, (object) targetTenantId);
            Policy policy2 = policy;
            string str4 = policy2 != null ? policy2.Serialize<Policy>() : (string) null;
            return str3 + str4;
          }));
          this.GetTenantPolicyHttpClient(context).SetPolicyAsync(policy.ToClient(), policyName).SyncResult();
        }), nameof (SetPolicy));
    }

    public PolicyInfo GetPolicyInfo(IVssRequestContext context, string policyName)
    {
      this.ValidateRequestContext(context);
      PolicyDataValidator.ValidatePolicyName(policyName);
      PolicyInfo policyInfo = (PolicyInfo) null;
      switch (policyName)
      {
        case "TenantPolicy.OrganizationCreationRestriction":
          policyInfo = new PolicyInfo("TenantPolicy.OrganizationCreationRestriction", FrameworkResources.TenantPolicyOrganizationCreationRestrictionDescription());
          break;
        case "TenantPolicy.RestrictFullScopePersonalAccessToken":
          policyInfo = new PolicyInfo("TenantPolicy.RestrictFullScopePersonalAccessToken", FrameworkResources.TenantPolicyRestrictFullScopePersonalAccessTokenDescription());
          break;
        case "TenantPolicy.RestrictGlobalPersonalAccessToken":
          policyInfo = new PolicyInfo("TenantPolicy.RestrictGlobalPersonalAccessToken", FrameworkResources.TenantPolicyRestrictGlobalPersonalAccessTokenDescription());
          break;
        case "TenantPolicy.RestrictPersonalAccessTokenLifespan":
          policyInfo = new PolicyInfo("TenantPolicy.RestrictPersonalAccessTokenLifespan", FrameworkResources.TenantPolicyRestrictPersonalAccessTokenLifespanDescription());
          break;
        case "TenantPolicy.EnableLeakedPersonalAccessTokenAutoRevocation":
          policyInfo = new PolicyInfo("TenantPolicy.EnableLeakedPersonalAccessTokenAutoRevocation", FrameworkResources.TenantPolicyEnableLeakedPersonalAccessTokenAutoRevocationDescription());
          break;
      }
      return policyInfo;
    }

    private void ValidateRequestContext(IVssRequestContext context) => context.CheckServiceHostId(this.m_serviceHostId, (IVssFrameworkService) this);

    private TenantPolicyHttpClient GetTenantPolicyHttpClient(IVssRequestContext requestContext) => requestContext.GetClient<TenantPolicyHttpClient>();

    private static class TracePoints
    {
      internal static readonly TimedActionTracePoints GetPolicy = new TimedActionTracePoints(8524100, 8524103, 8524105, 8524107);
      internal static readonly TimedActionTracePoints SetPolicy = new TimedActionTracePoints(8524110, 8524113, 8524115, 8524117);
      internal static readonly TimedActionTracePoints GetPolicyForTenant = new TimedActionTracePoints(8524120, 8524123, 8524125, 8524127);
    }
  }
}
