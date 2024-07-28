// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityConversionHelper
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Account;
using Microsoft.VisualStudio.Services.OrganizationTenant;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.Identity
{
  public static class IdentityConversionHelper
  {
    private const string c_area = "Identity";
    private const string c_layer = "IdentityConversionHelper";

    public static void ConvertIdentityToBindPending(
      IVssRequestContext requestContext,
      List<Guid> identityIds)
    {
      requestContext.TraceEnter(9281900, "Identity", nameof (IdentityConversionHelper), nameof (ConvertIdentityToBindPending));
      requestContext.CheckOrganizationRequestContext();
      IVssRequestContext deploymentContext = requestContext.To(TeamFoundationHostType.Deployment);
      Guid organizationId = requestContext.ServiceHost.InstanceId;
      Guid guid = Guid.Empty;
      Guid eventId = Guid.NewGuid();
      Guid tenantId;
      if (IdentityConversionHelper.IsTenantBacked(requestContext, out tenantId))
        guid = tenantId;
      bool isFullTracing = requestContext.IsTracing(9281901, TraceLevel.Verbose, "Identity", nameof (IdentityConversionHelper));
      requestContext.Trace(9281902, TraceLevel.Info, "Identity", nameof (IdentityConversionHelper), string.Format("[{0}]-[{1}]: Converting all identity to bind pending in organization: {2}", (object) organizationId, (object) eventId, (object) organizationId));
      IList<Guid> scopedIdentityIds = IdentityConversionHelper.GetScopedIdentityIds(requestContext);
      List<Microsoft.VisualStudio.Services.Identity.Identity> list1 = IdentityConversionHelper.GetIdentities(requestContext, scopedIdentityIds).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (identity => identity != null && !identity.IsImported && IdentityHelper.IsUserIdentity(deploymentContext, (IReadOnlyVssIdentity) identity))).ToList<Microsoft.VisualStudio.Services.Identity.Identity>().Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (identity => identity.MemberOf.Count <= 0)).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
      List<Microsoft.VisualStudio.Services.Identity.Identity> list2 = IdentityConversionHelper.GetIdentities(requestContext, (IList<Guid>) identityIds).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (identity => identity != null && !identity.IsImported && IdentityHelper.IsUserIdentity(deploymentContext, (IReadOnlyVssIdentity) identity))).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
      requestContext.Trace(9281903, TraceLevel.Info, "Identity", nameof (IdentityConversionHelper), string.Format("[{0}]-[{1}]: Found {2} identities", (object) organizationId, (object) eventId, (object) list2.Count));
      IList<Microsoft.VisualStudio.Services.Identity.Identity> duplicateIdentities = IdentityConversionHelper.GetDuplicateIdentities((IList<Microsoft.VisualStudio.Services.Identity.Identity>) list2);
      if (duplicateIdentities.Any<Microsoft.VisualStudio.Services.Identity.Identity>())
      {
        requestContext.TraceConditionally(9281904, TraceLevel.Error, "Identity", nameof (IdentityConversionHelper), (Func<string>) (() => string.Format("[{0}]-[{1}]: Duplicate user identities from active members: {2}.", (object) organizationId, (object) eventId, (object) IdentityConversionHelper.GetTraceMessageForIdentities(duplicateIdentities, isFullTracing))));
        throw new AccountStateNotValidException("duplicate identity found in active identities");
      }
      IdentityConversionHelper.GetBindPendingConversionHelper(new IdentityConversionPayload()
      {
        ActiveIdentities = list2,
        InactiveIdentities = list1,
        TenantId = guid,
        EventId = eventId,
        IsFullTracing = isFullTracing
      }).ConvertIdentitiesToBindPending(requestContext);
      requestContext.TraceLeave(9281900, "Identity", nameof (IdentityConversionHelper), nameof (ConvertIdentityToBindPending));
    }

    public static void ConvertAllActiveIdentitiesToBindPending(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(9281900, "Identity", nameof (IdentityConversionHelper), nameof (ConvertAllActiveIdentitiesToBindPending));
      requestContext.CheckOrganizationRequestContext();
      Guid organizationId = requestContext.ServiceHost.InstanceId;
      Guid guid = Guid.Empty;
      Guid eventId = Guid.NewGuid();
      Guid tenantId;
      if (IdentityConversionHelper.IsTenantBacked(requestContext, out tenantId))
        guid = tenantId;
      bool isFullTracing = requestContext.IsTracing(9281901, TraceLevel.Verbose, "Identity", nameof (IdentityConversionHelper));
      requestContext.Trace(9281902, TraceLevel.Info, "Identity", nameof (IdentityConversionHelper), string.Format("[{0}]-[{1}]: Converting all identity to bind pending in organization: {2}", (object) organizationId, (object) eventId, (object) organizationId));
      IVssRequestContext deploymentContext = requestContext.To(TeamFoundationHostType.Deployment);
      IList<Guid> scopedIdentityIds = IdentityConversionHelper.GetScopedIdentityIds(requestContext);
      List<Microsoft.VisualStudio.Services.Identity.Identity> list1 = IdentityConversionHelper.GetIdentities(requestContext, scopedIdentityIds).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (identity => identity != null && !identity.IsImported && IdentityHelper.IsUserIdentity(deploymentContext, (IReadOnlyVssIdentity) identity))).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
      List<Microsoft.VisualStudio.Services.Identity.Identity> list2 = list1.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (identity => identity.MemberOf.Count > 0)).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
      List<Microsoft.VisualStudio.Services.Identity.Identity> list3 = list1.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (identity => identity.MemberOf.Count <= 0)).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
      requestContext.Trace(9281903, TraceLevel.Info, "Identity", nameof (IdentityConversionHelper), string.Format("[{0}]-[{1}]: Found {2} active identities and {3} inactive identities", (object) organizationId, (object) eventId, (object) list2.Count, (object) list3.Count));
      requestContext.Trace(9281903, TraceLevel.Info, "Identity", nameof (IdentityConversionHelper), string.Format("[{0}]-[{1}]: Active Identities: {2}.", (object) organizationId, (object) eventId, (object) IdentityConversionHelper.GetTraceMessageForIdentities((IList<Microsoft.VisualStudio.Services.Identity.Identity>) list2, isFullTracing)));
      requestContext.Trace(9281903, TraceLevel.Info, "Identity", nameof (IdentityConversionHelper), string.Format("[{0}]-[{1}]: Inactive Identities: {2}.", (object) organizationId, (object) eventId, (object) IdentityConversionHelper.GetTraceMessageForIdentities((IList<Microsoft.VisualStudio.Services.Identity.Identity>) list3, isFullTracing)));
      IList<Microsoft.VisualStudio.Services.Identity.Identity> duplicateIdentities = IdentityConversionHelper.GetDuplicateIdentities((IList<Microsoft.VisualStudio.Services.Identity.Identity>) list2);
      if (duplicateIdentities.Any<Microsoft.VisualStudio.Services.Identity.Identity>())
      {
        requestContext.TraceConditionally(9281904, TraceLevel.Error, "Identity", nameof (IdentityConversionHelper), (Func<string>) (() => string.Format("[{0}]-[{1}]: Duplicate user identities from active members: {2}.", (object) organizationId, (object) eventId, (object) IdentityConversionHelper.GetTraceMessageForIdentities(duplicateIdentities, isFullTracing))));
        throw new AccountStateNotValidException("duplicate identity found in active identities");
      }
      IdentityConversionHelper.GetBindPendingConversionHelper(new IdentityConversionPayload()
      {
        ActiveIdentities = list2,
        InactiveIdentities = list3,
        TenantId = guid,
        EventId = eventId,
        IsFullTracing = isFullTracing
      }).ConvertIdentitiesToBindPending(requestContext);
      requestContext.TraceLeave(9281900, "Identity", nameof (IdentityConversionHelper), nameof (ConvertAllActiveIdentitiesToBindPending));
    }

    private static IList<Guid> GetScopedIdentityIds(IVssRequestContext requestContext)
    {
      requestContext.CheckOrganizationRequestContext();
      Guid instanceId = requestContext.ServiceHost.InstanceId;
      return requestContext.GetService<IPlatformIdentityServiceInternal>().ReadIdentityIdsInScope(requestContext, instanceId);
    }

    private static IList<Microsoft.VisualStudio.Services.Identity.Identity> GetIdentities(
      IVssRequestContext requestContext,
      IList<Guid> identityIds)
    {
      requestContext.CheckOrganizationRequestContext();
      return requestContext.GetService<IdentityService>().ReadIdentities(requestContext, identityIds, QueryMembership.Expanded, (IEnumerable<string>) null);
    }

    private static string GetTraceMessageForIdentities(IList<Microsoft.VisualStudio.Services.Identity.Identity> identities, bool fullTracing = false)
    {
      if (identities == null || identities.Count == 0)
        return string.Empty;
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("{");
      stringBuilder.AppendFormat("Total count: {0}. ", (object) identities.Count);
      int num = 0;
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identities)
      {
        if (num > 0)
          stringBuilder.Append(",");
        stringBuilder.Append(IdentityConversionHelper.GetTraceMessageForIdentity(identity));
        if (num++ > 10)
        {
          if (!fullTracing)
            break;
        }
      }
      stringBuilder.Append("}");
      return stringBuilder.ToString();
    }

    private static string GetTraceMessageForIdentity(Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      if (identity == null)
        return string.Empty;
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "(id: {0}; mid: {1}; cuid: {2}; sid: {3}; domain: {4}; accountName: {5})", (object) identity.Id, (object) identity.MasterId, (object) identity.GetProperty<Guid>("CUID", Guid.Empty), (object) identity.Descriptor.Identifier, (object) identity.GetProperty<string>("Domain", string.Empty), (object) identity.GetProperty<string>("Account", string.Empty));
    }

    private static IList<Microsoft.VisualStudio.Services.Identity.Identity> GetDuplicateIdentities(
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities)
    {
      IEnumerable<IGrouping<string, Microsoft.VisualStudio.Services.Identity.Identity>> groupings = identities.GroupBy<Microsoft.VisualStudio.Services.Identity.Identity, string>((Func<Microsoft.VisualStudio.Services.Identity.Identity, string>) (i => i.Descriptor.Identifier)).Where<IGrouping<string, Microsoft.VisualStudio.Services.Identity.Identity>>((Func<IGrouping<string, Microsoft.VisualStudio.Services.Identity.Identity>, bool>) (g => g.Count<Microsoft.VisualStudio.Services.Identity.Identity>() > 1));
      List<Microsoft.VisualStudio.Services.Identity.Identity> duplicateIdentities = new List<Microsoft.VisualStudio.Services.Identity.Identity>();
      foreach (IGrouping<string, Microsoft.VisualStudio.Services.Identity.Identity> source in groupings)
        duplicateIdentities.AddRange((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) source.ToList<Microsoft.VisualStudio.Services.Identity.Identity>());
      return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) duplicateIdentities;
    }

    public static bool IsTenantBacked(IVssRequestContext enterpriseContext, out Guid tenantId)
    {
      Guid instanceId = enterpriseContext.ServiceHost.InstanceId;
      IVssRequestContext context = enterpriseContext.To(TeamFoundationHostType.Deployment);
      IOrganizationTenantService service = context.GetService<IOrganizationTenantService>();
      tenantId = service.GetOrganizationTenantId(context.Elevate(), instanceId);
      return tenantId != Guid.Empty;
    }

    private static BindPendingConversionHelper GetBindPendingConversionHelper(
      IdentityConversionPayload identityConversionPayload)
    {
      return identityConversionPayload.TenantId.Equals(Guid.Empty) ? (BindPendingConversionHelper) new MsaBindPendingConversionHelper(identityConversionPayload) : (BindPendingConversionHelper) new AadBindPendingConversionHelper(identityConversionPayload);
    }
  }
}
