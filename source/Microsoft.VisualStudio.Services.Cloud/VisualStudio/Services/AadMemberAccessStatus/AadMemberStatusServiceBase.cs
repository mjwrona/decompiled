// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.AadMemberAccessStatus.AadMemberStatusServiceBase
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.AadMemberAccessStatus
{
  internal abstract class AadMemberStatusServiceBase : IAadMemberStatusService, IVssFrameworkService
  {
    protected internal AadMemberStatusCache LocalCache;
    protected internal IRedisCacheService RedisCacheService;
    protected const string TraceArea = "AadUserStateService";
    protected const string TraceLayer = "AadMemberStatusServiceBase";
    internal const string AadMemberStatusServiceLocalCacheFeatureName = "VisualStudio.Services.AadMemberStatusService.LocalCache";
    internal const string AadMemberStatusServiceRemoteCacheFeatureName = "VisualStudio.Services.AadMemberStatusService.RemoteCache";
    internal const string AadMemberStatusServiceSkipGetDeploymentDescriptorFeatureName = "VisualStudio.Services.AadMemberStatusService.GetDeploymentDescriptor";
    internal static readonly Guid RemoteMemberStatusCacheNamespace = new Guid("{56AB1585-8CAA-4446-B2D7-2FDE8F0C0E82}");

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return;
      this.LocalCache = new AadMemberStatusCache(this.DisabledUserExpirationInterval, this.DeletedUserExpirationInterval, this.ValidUserExpirationInterval, this.InvalidUserExpirationInterval);
      this.RedisCacheService = systemRequestContext.GetService<IRedisCacheService>();
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      if (systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return;
      this.LocalCache.Clear();
    }

    public AadMemberStatus GetAadMemberStatus(
      IVssRequestContext requestContext,
      SubjectDescriptor subjectDescriptor,
      Guid oid,
      Guid tenantId)
    {
      requestContext.TraceEnter(9002400, "AadUserStateService", nameof (AadMemberStatusServiceBase), nameof (GetAadMemberStatus));
      try
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        if (requestContext.ServiceHost.Is(TeamFoundationHostType.Application))
          return this.GetAadMemberStatus(requestContext, oid, tenantId, (string) subjectDescriptor, new Func<(bool, AadMemberStatus)>(GetAadMemberStatusFromRemoteLocal));
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Application);
        return vssRequestContext.GetService<IAadMemberStatusService>().GetAadMemberStatus(vssRequestContext, subjectDescriptor, oid, tenantId);
      }
      finally
      {
        requestContext.TraceLeave(9002404, "AadUserStateService", nameof (AadMemberStatusServiceBase), nameof (GetAadMemberStatus));
      }

      (bool Found, AadMemberStatus MemberStatus) GetAadMemberStatusFromRemoteLocal()
      {
        AadMemberStatus memberStatus;
        return (this.GetAadMemberStatusFromRemote(requestContext, subjectDescriptor, oid, tenantId, out memberStatus), memberStatus);
      }
    }

    public AadMemberStatus GetAadMemberStatus(
      IVssRequestContext requestContext,
      IdentityDescriptor identityDescriptor,
      Guid oid,
      Guid tenantId)
    {
      requestContext.TraceEnter(9002400, "AadUserStateService", nameof (AadMemberStatusServiceBase), nameof (GetAadMemberStatus));
      try
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        ArgumentUtility.CheckForNull<IdentityDescriptor>(identityDescriptor, nameof (identityDescriptor));
        if (requestContext.ServiceHost.Is(TeamFoundationHostType.Application))
          return this.GetAadMemberStatus(requestContext, oid, tenantId, identityDescriptor.ToString(), new Func<(bool, AadMemberStatus)>(GetAadMemberStatusFromRemoteLocal));
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Application);
        return vssRequestContext.GetService<IAadMemberStatusService>().GetAadMemberStatus(vssRequestContext, identityDescriptor, oid, tenantId);
      }
      finally
      {
        requestContext.TraceLeave(9002404, "AadUserStateService", nameof (AadMemberStatusServiceBase), nameof (GetAadMemberStatus));
      }

      (bool Found, AadMemberStatus MemberStatus) GetAadMemberStatusFromRemoteLocal()
      {
        AadMemberStatus memberStatus;
        return (this.GetAadMemberStatusFromRemote(requestContext, identityDescriptor, oid, tenantId, out memberStatus), memberStatus);
      }
    }

    private AadMemberStatus GetAadMemberStatus(
      IVssRequestContext requestContext,
      Guid oid,
      Guid tenantId,
      string tracingIdentifier,
      Func<(bool Found, AadMemberStatus MemberStatus)> funcGetAadMemberStatusFromRemote)
    {
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        requestContext.Trace(9002401, TraceLevel.Error, "AadUserStateService", nameof (AadMemberStatusServiceBase), "GetAadMemberStatus is called from DeploymentContext. Stack trace: {0}", (object) EnvironmentWrapper.ToReadableStackTrace());
        return (AadMemberStatus) null;
      }
      AadMemberAccessState memberAccessState = AadMemberAccessState.Indeterminate;
      string cacheKey = AadMemberStatusServiceBase.CreateCacheKey(oid, tenantId);
      AadMemberStatus cachedStatus = this.GetCachedStatus(requestContext, cacheKey);
      if (cachedStatus != null && cachedStatus.StatusValidUntil > DateTimeOffset.UtcNow)
      {
        requestContext.Trace(9002401, TraceLevel.Info, "AadUserStateService", nameof (AadMemberStatusServiceBase), "Cache hit for cacheKey {0}", (object) cacheKey);
        return cachedStatus;
      }
      AadMemberStatus aadMemberStatus = cachedStatus;
      requestContext.Trace(9002423, TraceLevel.Info, "AadUserStateService", nameof (AadMemberStatusServiceBase), "Cache miss for identifier '{0}'", (object) tracingIdentifier);
      (bool, AadMemberStatus) valueTuple = funcGetAadMemberStatusFromRemote();
      if (valueTuple.Item1)
      {
        AadMemberStatus memberStatus1 = valueTuple.Item2;
        requestContext.Trace(9002401, TraceLevel.Info, "AadUserStateService", nameof (AadMemberStatusServiceBase), "Remote call returns value identifier '{0}', State: {1}", (object) tracingIdentifier, (object) memberStatus1.MemberState);
        if (memberStatus1.MemberState != AadMemberAccessState.Indeterminate)
        {
          this.UpdateCache(requestContext, cacheKey, memberStatus1);
          return memberStatus1;
        }
        if (memberAccessState != AadMemberAccessState.Indeterminate)
        {
          requestContext.Trace(9002401, TraceLevel.Info, "AadUserStateService", nameof (AadMemberStatusServiceBase), "Identity with identifier '{0}' is in invalid state. Setting cache to {1}", (object) tracingIdentifier, (object) memberAccessState);
          AadMemberStatus memberStatus2 = new AadMemberStatus()
          {
            MemberState = memberAccessState,
            StatusValidUntil = new DateTimeOffset()
          };
          this.UpdateCache(requestContext, cacheKey, memberStatus2);
          return memberStatus2;
        }
      }
      if (aadMemberStatus != null)
      {
        requestContext.Trace(9002401, TraceLevel.Info, "AadUserStateService", nameof (AadMemberStatusServiceBase), "We cannot determine the state of the identity: '{0}'. Returning fallback status", (object) tracingIdentifier);
        return aadMemberStatus;
      }
      requestContext.Trace(9002401, TraceLevel.Info, "AadUserStateService", nameof (AadMemberStatusServiceBase), "We cannot determine the state of the identity: '{0}'. Returning unknown", (object) tracingIdentifier);
      return new AadMemberStatus()
      {
        MemberState = AadMemberAccessState.Indeterminate,
        StatusValidUntil = new DateTimeOffset()
      };
    }

    public AadMemberStatus GetAadMemberStatusFromCache(
      IVssRequestContext requestContext,
      IdentityDescriptor identityDescriptor,
      Guid oid,
      Guid tenantId,
      bool localOnly)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(90024005, "AadUserStateService", nameof (AadMemberStatusServiceBase), nameof (GetAadMemberStatusFromCache));
      try
      {
        ArgumentUtility.CheckForNull<IdentityDescriptor>(identityDescriptor, nameof (identityDescriptor));
        if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        {
          requestContext.Trace(9002406, TraceLevel.Error, "AadUserStateService", nameof (AadMemberStatusServiceBase), "GetAadMemberStatusFromCache is called from DeploymentContext. Stack trace: {0}", (object) EnvironmentWrapper.ToReadableStackTrace());
          return (AadMemberStatus) null;
        }
        if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Application))
        {
          IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Application);
          return vssRequestContext.GetService<IAadMemberStatusService>().GetAadMemberStatusFromCache(vssRequestContext, identityDescriptor, oid, tenantId);
        }
        string cacheKey = AadMemberStatusServiceBase.CreateCacheKey(oid, tenantId);
        return this.GetCachedStatus(requestContext, cacheKey, localOnly);
      }
      finally
      {
        requestContext.TraceLeave(9002409, "AadUserStateService", nameof (AadMemberStatusServiceBase), nameof (GetAadMemberStatusFromCache));
      }
    }

    private AadMemberStatus GetCachedStatus(
      IVssRequestContext requestContext,
      string cacheKey,
      bool localOnly = false)
    {
      requestContext.TraceEnter(9002410, "AadUserStateService", nameof (AadMemberStatusServiceBase), nameof (GetCachedStatus));
      AadMemberStatus cachedStatus = (AadMemberStatus) null;
      try
      {
        AadMemberStatus memberStatus1 = (AadMemberStatus) null;
        IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
        if (requestContext1.IsFeatureEnabled("VisualStudio.Services.AadMemberStatusService.LocalCache"))
        {
          if (this.LocalCache.TryGetValue(cacheKey, out memberStatus1))
          {
            if (memberStatus1.StatusValidUntil > DateTimeOffset.UtcNow)
            {
              requestContext.Trace(9002414, TraceLevel.Info, "AadUserStateService", nameof (AadMemberStatusServiceBase), "Local cache hit for cacheKey {0}, State: {1}", (object) cacheKey, (object) memberStatus1.MemberState.ToString());
              return memberStatus1;
            }
            cachedStatus = memberStatus1;
            requestContext.Trace(9002415, TraceLevel.Info, "AadUserStateService", nameof (AadMemberStatusServiceBase), "Local cache hit for cacheKey {0}, State: {1} but cache is expired. ValidTo: {2}", (object) cacheKey, (object) memberStatus1.MemberState.ToString(), (object) memberStatus1.StatusValidUntil);
          }
          else
            requestContext.Trace(9002416, TraceLevel.Info, "AadUserStateService", nameof (AadMemberStatusServiceBase), "Local cache miss for cacheKey {0}", (object) cacheKey);
        }
        if (localOnly || !requestContext1.IsFeatureEnabled("VisualStudio.Services.AadMemberStatusService.RemoteCache") || !this.RedisCacheService.IsEnabled(requestContext))
          return cachedStatus;
        AadMemberStatus memberStatus2;
        if (this.GetRemoteCacheContainer(requestContext).TryGet<string, AadMemberStatus>(requestContext, cacheKey, out memberStatus2))
        {
          if (memberStatus2.StatusValidUntil > DateTimeOffset.UtcNow)
          {
            this.UpdateCache(requestContext, cacheKey, memberStatus2, true);
            requestContext.Trace(9002417, TraceLevel.Info, "AadUserStateService", nameof (AadMemberStatusServiceBase), "Remote cache hit for cache key {0}, State {1} and ValidTo {2}", (object) cacheKey, (object) memberStatus2.MemberState, (object) memberStatus2.StatusValidUntil);
            return memberStatus2;
          }
          requestContext.Trace(9002418, TraceLevel.Info, "AadUserStateService", nameof (AadMemberStatusServiceBase), "Remote cache hit for cacheKey {0}, State: {1} but cache is expired. ValidTo: {2}", (object) cacheKey, (object) memberStatus2.MemberState.ToString(), (object) memberStatus2.StatusValidUntil);
          if (cachedStatus == null || cachedStatus.StatusValidUntil < memberStatus2.StatusValidUntil)
            cachedStatus = memberStatus2;
        }
        else
          requestContext.Trace(9002413, TraceLevel.Info, "AadUserStateService", nameof (AadMemberStatusServiceBase), "Remote cache miss for cacheKey {0}", (object) cacheKey);
        return cachedStatus;
      }
      finally
      {
        requestContext.TraceLeave(9002419, "AadUserStateService", nameof (AadMemberStatusServiceBase), nameof (GetCachedStatus));
      }
    }

    private void UpdateCache(
      IVssRequestContext requestContext,
      string cacheKey,
      AadMemberStatus memberStatus,
      bool localOnly = false)
    {
      requestContext.TraceEnter(9002420, "AadUserStateService", nameof (AadMemberStatusServiceBase), nameof (UpdateCache));
      try
      {
        IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
        if (requestContext1.IsFeatureEnabled("VisualStudio.Services.AadMemberStatusService.LocalCache"))
          this.LocalCache.Set(cacheKey, memberStatus);
        if (localOnly || !requestContext1.IsFeatureEnabled("VisualStudio.Services.AadMemberStatusService.RemoteCache") || !this.RedisCacheService.IsEnabled(requestContext))
          return;
        this.GetRemoteCacheContainer(requestContext).Set(requestContext, (IDictionary<string, AadMemberStatus>) new Dictionary<string, AadMemberStatus>()
        {
          {
            cacheKey,
            memberStatus
          }
        });
      }
      catch (Exception ex)
      {
        requestContext.TraceException(9002422, "AadUserStateService", nameof (AadMemberStatusServiceBase), ex);
      }
      finally
      {
        requestContext.TraceLeave(9002421, "AadUserStateService", nameof (AadMemberStatusServiceBase), nameof (UpdateCache));
      }
    }

    private IMutableDictionaryCacheContainer<string, AadMemberStatus> GetRemoteCacheContainer(
      IVssRequestContext requestContext)
    {
      ContainerSettings settings = new ContainerSettings()
      {
        CiAreaName = "AadRemoteMemberStatusCache"
      };
      return this.RedisCacheService.GetVolatileDictionaryContainer<string, AadMemberStatus, AadMemberStatusServiceBase.RedisCacheSecurityToken>(requestContext, AadMemberStatusServiceBase.RemoteMemberStatusCacheNamespace, settings);
    }

    private static string CreateCacheKey(Guid oid, Guid tenantId) => string.Format("{0}-{1}", (object) oid.ToString(), (object) tenantId.ToString());

    protected internal abstract bool GetAadMemberStatusFromRemote(
      IVssRequestContext requestContext,
      IdentityDescriptor identityDescriptor,
      Guid oid,
      Guid tenantId,
      out AadMemberStatus memberStatus);

    protected internal abstract bool GetAadMemberStatusFromRemote(
      IVssRequestContext requestContext,
      SubjectDescriptor subjectDescriptor,
      Guid oid,
      Guid tenantId,
      out AadMemberStatus memberStatus);

    protected virtual TimeSpan DisabledUserExpirationInterval { get; } = TimeSpan.FromMinutes(5.0);

    protected virtual TimeSpan DeletedUserExpirationInterval { get; } = TimeSpan.FromMinutes(5.0);

    protected virtual TimeSpan ValidUserExpirationInterval { get; } = TimeSpan.FromMinutes(5.0);

    protected virtual TimeSpan InvalidUserExpirationInterval { get; } = TimeSpan.FromMinutes(5.0);

    internal sealed class RedisCacheSecurityToken
    {
    }
  }
}
