// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TenantPolicy.TenantPolicyPropertyHelper
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Aad;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.TenantPolicy
{
  public static class TenantPolicyPropertyHelper
  {
    private const string c_area = "TenantPolicy";
    private const string c_layer = "TenantPolicyPropertyHelper";
    public const string UseFixedPersonalAccessTokenExpirationDateFeatureName = "VisualStudio.Services.TenantPolicy.UseFixedPersonalAccessTokenExpirationDate";

    public static bool IsUserMemberofPolicyAllowedList(
      IVssRequestContext requestContext,
      Policy policy,
      bool bypassAadCache = false)
    {
      return TenantPolicyPropertyHelper.IsUserMemberofPolicyAllowedList(requestContext, policy, requestContext.GetUserIdentity(), bypassAadCache);
    }

    public static bool IsUserMemberofPolicyAllowedList(
      IVssRequestContext requestContext,
      Policy policy,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      bool bypassAadCache = false)
    {
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Identity.Identity>(identity, nameof (identity));
      Guid aadObjectId = identity.GetAadObjectId();
      Guid tenantId = AadIdentityHelper.ExtractTenantId((IReadOnlyVssIdentity) identity);
      return TenantPolicyPropertyHelper.IsUserMemberofPolicyAllowedList(requestContext, policy, aadObjectId, tenantId, bypassAadCache);
    }

    public static bool IsUserMemberofPolicyAllowedList(
      IVssRequestContext requestContext,
      Policy policy,
      Guid userObjectId,
      Guid targetTenantId,
      bool bypassAadCache = false)
    {
      ArgumentUtility.CheckForNull<Policy>(policy, nameof (policy));
      if (!PolicyNames.KnownTenantPolicyNames.Contains<string>(policy.Name))
        return false;
      ISet<Guid> allowedUsersAndGroups = TenantPolicyPropertyHelper.GetAllowedUsersAndGroups(requestContext, policy.Properties);
      if (allowedUsersAndGroups.IsNullOrEmpty<Guid>())
      {
        requestContext.TraceAlways(8524606, TraceLevel.Info, "TenantPolicy", nameof (TenantPolicyPropertyHelper), "AllowedUsersAndGroups is not set. However the policy is enabled for tenant ");
        return false;
      }
      return TenantPolicyPropertyHelper.IsUserMemberofAllowedList(requestContext, userObjectId, allowedUsersAndGroups, targetTenantId, bypassAadCache);
    }

    public static bool IsUserMemberofAllowedList(
      IVssRequestContext requestContext,
      Guid userObjectId,
      ISet<Guid> allowedUsersAndGroupObjectIds,
      Guid targetTenantId,
      bool bypassAadCache)
    {
      ISet<Guid> userAadAncestorIds = TenantPolicyPropertyHelper.GetUserAadAncestorIds(requestContext, userObjectId, targetTenantId, bypassAadCache);
      if (allowedUsersAndGroupObjectIds.Contains(userObjectId) || !userAadAncestorIds.IsNullOrEmpty<Guid>() && allowedUsersAndGroupObjectIds.Any<Guid>((Func<Guid, bool>) (x => userAadAncestorIds.Contains(x))))
        return true;
      requestContext.Trace(8524607, TraceLevel.Info, "TenantPolicy", nameof (TenantPolicyPropertyHelper), string.Format("The user {0} is not member of AllowedUsersAndGroupObjectIds", (object) userObjectId));
      return false;
    }

    public static bool IsValidToWithinMaxPatLifespan(
      IVssRequestContext requestContext,
      int maxPatLifespanInDays,
      DateTime validFrom,
      DateTime? validTo)
    {
      try
      {
        DateTime dateTime = DateTime.UtcNow;
        if (requestContext.IsFeatureEnabled("VisualStudio.Services.TenantPolicy.UseFixedPersonalAccessTokenExpirationDate"))
          dateTime = validFrom.ToUniversalTime();
        DateTime valueOrDefault = validTo.GetValueOrDefault(new DateTime());
        return valueOrDefault.Equals(new DateTime()) || (valueOrDefault.ToUniversalTime().Date - dateTime.Date).TotalDays <= (double) maxPatLifespanInDays;
      }
      catch (Exception ex)
      {
        requestContext.TraceAlways(8524608, TraceLevel.Info, "TenantPolicy", nameof (TenantPolicyPropertyHelper), string.Format("MaxPatLifespanInDays is not set. However the policy is enabled for tenant. Exception: ${0}", (object) ex));
        return false;
      }
    }

    public static ISet<Guid> GetAllowedUsersAndGroups(
      IVssRequestContext requestContext,
      IDictionary<string, string> properties)
    {
      string str;
      if (properties.TryGetValue("AllowedUsersAndGroupObjectIds", out str))
      {
        if (!str.IsNullOrEmpty<char>())
        {
          try
          {
            return (ISet<Guid>) JsonUtilities.Deserialize<HashSet<Guid>>(str);
          }
          catch (Exception ex)
          {
            requestContext.TraceException(8524610, TraceLevel.Error, "TenantPolicy", nameof (TenantPolicyPropertyHelper), ex);
            return (ISet<Guid>) new HashSet<Guid>();
          }
        }
      }
      return (ISet<Guid>) new HashSet<Guid>();
    }

    private static ISet<Guid> GetUserAadAncestorIds(
      IVssRequestContext requestContext,
      Guid userObjectId,
      Guid targetTenantId,
      bool bypassAadCache)
    {
      IVssRequestContext context1 = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
      AadService service = context1.GetService<AadService>();
      IVssRequestContext context2 = context1;
      GetAncestorIdsRequest<Guid> ancestorIdsRequest = new GetAncestorIdsRequest<Guid>();
      ancestorIdsRequest.ToTenant = targetTenantId.ToString();
      ancestorIdsRequest.ObjectType = AadObjectType.User;
      ancestorIdsRequest.Identifiers = (IEnumerable<Guid>) new Guid[1]
      {
        userObjectId
      };
      ancestorIdsRequest.Expand = -1;
      ancestorIdsRequest.BypassCache = bypassAadCache;
      GetAncestorIdsRequest<Guid> request = ancestorIdsRequest;
      GetAncestorIdsResponse<Guid> ancestorIds = service.GetAncestorIds<Guid>(context2, request);
      ISet<Guid> guidSet1;
      ISet<Guid> guidSet2;
      return ancestorIds.Ancestors == null || !ancestorIds.Ancestors.TryGetValue(userObjectId, out guidSet1) ? (guidSet2 = (ISet<Guid>) new HashSet<Guid>()) : guidSet1;
    }
  }
}
