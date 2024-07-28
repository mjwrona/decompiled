// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.DeploymentUserIdentityCacheService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Redis;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class DeploymentUserIdentityCacheService : 
    IDeploymentUserIdentityCacheService,
    IVssFrameworkService
  {
    private IMutableDictionaryCacheContainer<Guid, Microsoft.VisualStudio.Services.Identity.Identity> m_vsidToIdentityRemoteCache;
    private IMutableDictionaryCacheContainer<string, Guid> m_descriptorToVsidRemoteCache;
    private TimeSpan m_cacheEntryTimeout;
    protected static readonly Guid s_namespace = new Guid("38c93993-a08e-4755-a153-e8bd292544b6");
    internal static readonly Guid DeploymentUserIdentityChanged = new Guid("ae0985f3-6c0e-45d2-8322-8e354c2d74d0");

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().RegisterNotification(systemRequestContext, "Default", DeploymentUserIdentityCacheService.DeploymentUserIdentityChanged, new SqlNotificationHandler(this.OnIdentityChanged), false);
      this.m_cacheEntryTimeout = systemRequestContext.GetService<IVssRegistryService>().GetValue<TimeSpan>(systemRequestContext, (RegistryQuery) FrameworkServerConstants.RoutingServiceCacheEntryTimeout, TimeSpan.FromHours(12.0));
      IRedisCacheService service = systemRequestContext.GetService<IRedisCacheService>();
      this.m_vsidToIdentityRemoteCache = service.GetVolatileDictionaryContainer<Guid, Microsoft.VisualStudio.Services.Identity.Identity, DeploymentUserIdentityCacheService.SecurityToken>(systemRequestContext, DeploymentUserIdentityCacheService.s_namespace, new ContainerSettings()
      {
        KeyExpiry = new TimeSpan?(this.m_cacheEntryTimeout),
        CiAreaName = nameof (DeploymentUserIdentityCacheService),
        NoThrowMode = new bool?(true)
      });
      this.m_descriptorToVsidRemoteCache = service.GetVolatileDictionaryContainer<string, Guid, DeploymentUserIdentityCacheService.SecurityToken>(systemRequestContext, DeploymentUserIdentityCacheService.s_namespace, new ContainerSettings()
      {
        KeyExpiry = new TimeSpan?(this.m_cacheEntryTimeout),
        CiAreaName = nameof (DeploymentUserIdentityCacheService),
        NoThrowMode = new bool?(true)
      });
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public Microsoft.VisualStudio.Services.Identity.Identity Get(
      IVssRequestContext deploymentContext,
      SubjectDescriptor subjectDescriptor)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity;
      return !this.TryGetValue(deploymentContext, subjectDescriptor, out identity) ? (Microsoft.VisualStudio.Services.Identity.Identity) null : identity.Clone();
    }

    private bool TryGetValue(
      IVssRequestContext deploymentContext,
      SubjectDescriptor subjectDescriptor,
      out Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      DeploymentUserIdentityMemoryCacheService service = deploymentContext.GetService<DeploymentUserIdentityMemoryCacheService>();
      bool flag = service.TryGetValue(deploymentContext, subjectDescriptor, out identity);
      if (!flag)
      {
        Guid key;
        if (this.m_descriptorToVsidRemoteCache.TryGet<string, Guid>(deploymentContext, (string) subjectDescriptor, out key))
          flag = this.m_vsidToIdentityRemoteCache.TryGet<Guid, Microsoft.VisualStudio.Services.Identity.Identity>(deploymentContext, key, out identity);
        if (flag)
          service.Set(deploymentContext, identity.SubjectDescriptor, identity.Clone());
      }
      return flag;
    }

    public Microsoft.VisualStudio.Services.Identity.Identity Get(
      IVssRequestContext deploymentContext,
      IdentityDescriptor identityDescriptor)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity;
      return !this.TryGetValue(deploymentContext, identityDescriptor, out identity) ? (Microsoft.VisualStudio.Services.Identity.Identity) null : identity.Clone();
    }

    private bool TryGetValue(
      IVssRequestContext deploymentContext,
      IdentityDescriptor identityDescriptor,
      out Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      DeploymentUserIdentityMemoryCacheService service = deploymentContext.GetService<DeploymentUserIdentityMemoryCacheService>();
      bool flag = service.TryGetValue(deploymentContext, identityDescriptor, out identity);
      if (!flag)
      {
        Guid key;
        if (this.m_descriptorToVsidRemoteCache.TryGet<string, Guid>(deploymentContext, identityDescriptor.ToString(), out key))
          flag = this.m_vsidToIdentityRemoteCache.TryGet<Guid, Microsoft.VisualStudio.Services.Identity.Identity>(deploymentContext, key, out identity);
        if (flag)
          service.Set(deploymentContext, identity.SubjectDescriptor, identity.Clone());
      }
      return flag;
    }

    public Microsoft.VisualStudio.Services.Identity.Identity Get(
      IVssRequestContext deploymentContext,
      Guid identityId)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity;
      return !this.TryGetValue(deploymentContext, identityId, out identity) ? (Microsoft.VisualStudio.Services.Identity.Identity) null : identity.Clone();
    }

    private bool TryGetValue(
      IVssRequestContext deploymentContext,
      Guid identityId,
      out Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      DeploymentUserIdentityMemoryCacheService service = deploymentContext.GetService<DeploymentUserIdentityMemoryCacheService>();
      bool flag = service.TryGetValue(deploymentContext, identityId, out identity);
      if (!flag)
      {
        flag = this.m_vsidToIdentityRemoteCache.TryGet<Guid, Microsoft.VisualStudio.Services.Identity.Identity>(deploymentContext, identityId, out identity);
        if (flag)
          service.Set(deploymentContext, identity.SubjectDescriptor, identity.Clone());
      }
      return flag;
    }

    public void Set(IVssRequestContext deploymentContext, Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity1 = identity.Clone();
      deploymentContext.GetService<DeploymentUserIdentityMemoryCacheService>().Set(deploymentContext, identity.SubjectDescriptor, identity1);
      this.m_descriptorToVsidRemoteCache.Set(deploymentContext, (IDictionary<string, Guid>) new Dictionary<string, Guid>()
      {
        {
          identity.SubjectDescriptor.ToString(),
          identity.Id
        },
        {
          identity.Descriptor.ToString(),
          identity.Id
        }
      });
      this.m_vsidToIdentityRemoteCache.Set(deploymentContext, (IDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>) new Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>()
      {
        {
          identity.Id,
          identity1
        }
      });
    }

    public void Remove(
      IVssRequestContext deploymentContext,
      Guid identityId,
      bool invalidateRemoteCache = true)
    {
      deploymentContext.GetService<DeploymentUserIdentityMemoryCacheService>().Remove(deploymentContext, identityId);
      if (!invalidateRemoteCache)
        return;
      this.m_vsidToIdentityRemoteCache.Invalidate(deploymentContext, (IEnumerable<Guid>) new Guid[1]
      {
        identityId
      });
    }

    public void SendIdentityChangedNotification(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      DeploymentUserIdentityChange changeData = new DeploymentUserIdentityChange()
      {
        SubjectDescriptor = identity.SubjectDescriptor,
        StorageKey = identity.Id
      };
      this.SendIdentityChangedNotification(requestContext, changeData);
    }

    public void SendIdentityChangedNotification(
      IVssRequestContext requestContext,
      DeploymentUserIdentityChange changeData)
    {
      requestContext.GetService<ITeamFoundationSqlNotificationService>().SendNotification(requestContext, DeploymentUserIdentityCacheService.DeploymentUserIdentityChanged, TeamFoundationSerializationUtility.SerializeToString<DeploymentUserIdentityChange>(changeData));
    }

    private void OnIdentityChanged(IVssRequestContext requestContext, NotificationEventArgs args)
    {
      DeploymentUserIdentityChange userIdentityChange = args.Deserialize<DeploymentUserIdentityChange>();
      if (userIdentityChange == null)
        return;
      this.Remove(requestContext, userIdentityChange.StorageKey, false);
    }

    private class SecurityToken
    {
    }
  }
}
