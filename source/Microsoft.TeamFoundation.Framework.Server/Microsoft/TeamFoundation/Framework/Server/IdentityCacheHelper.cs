// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IdentityCacheHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Organization;
using System;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class IdentityCacheHelper
  {
    private const string c_skipCertainAadGroupsMembershipChange = "VisualStudio.Services.IdentityCache.SkipCertainAadGroupsMembershipChange";
    private const string c_area = "IdentityService";
    private const string c_layer = "IdentityCacheHelper";

    public static bool IsSkipMembershipChangeEnabled(IVssRequestContext context)
    {
      if (context == null || context.ExecutionEnvironment.IsOnPremisesDeployment || context.ServiceHost.Is(TeamFoundationHostType.Deployment) || !context.IsFeatureEnabled("VisualStudio.Services.IdentityCache.SkipCertainAadGroupsMembershipChange"))
        return false;
      Guid instanceId = context.To(TeamFoundationHostType.Application).ServiceHost.InstanceId;
      IVssRequestContext context1 = context.To(TeamFoundationHostType.Deployment).Elevate();
      ImmutableHashSet<Guid> membershipChange = context1.GetService<IIdentityCacheSettingsService>().GetOrgsToSkipAadGroupsMembershipChange(context1);
      return membershipChange != null && membershipChange.Contains(instanceId);
    }

    public static bool ShouldSkipMembershipChange(
      IVssRequestContext context,
      MembershipChangeInfo changeInfo)
    {
      if (context == null || changeInfo == null || !changeInfo.IsMemberGroup || changeInfo.MemberDescriptor == (IdentityDescriptor) null)
        return false;
      IVssRequestContext context1 = context.To(TeamFoundationHostType.Deployment).Elevate();
      IIdentityCacheSettingsService service = context1.GetService<IIdentityCacheSettingsService>();
      ImmutableHashSet<string> membershipChange1 = service.GetAadGroupSidsToSkipMembershipChange(context1);
      if (membershipChange1 != null && membershipChange1.Contains(changeInfo.MemberDescriptor.Identifier))
        return true;
      ImmutableHashSet<string> membershipChange2 = service.GetWellKnownGroupSidsToSkipMembershipChange(context1);
      return membershipChange2 != null && membershipChange2.Contains(changeInfo.MemberDescriptor.Identifier);
    }

    internal static bool IsValidCacheData(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      int tracePoint)
    {
      if (identity != null)
      {
        if (requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        {
          try
          {
            if (requestContext.IsOrganizationAadBacked())
            {
              if (identity.IsMsaIdentity())
              {
                if (!identity.IsActive)
                  return false;
              }
            }
            else if (identity.IsExternalUser)
            {
              if (!identity.IsActive)
                return false;
            }
          }
          catch (OrganizationNotFoundException ex)
          {
          }
          catch (Exception ex)
          {
            requestContext.TraceException(1754848, TraceLevel.Warning, "IdentityService", nameof (IdentityCacheHelper), ex);
          }
          return true;
        }
      }
      return true;
    }
  }
}
