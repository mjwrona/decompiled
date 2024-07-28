// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Users.Server.UserIdentifierConversionCacheService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Users.Server
{
  internal class UserIdentifierConversionCacheService : 
    IUserIdentifierConversionCacheService,
    IVssFrameworkService
  {
    private IMutableDictionaryCacheContainer<Guid, SubjectDescriptor> m_storageKeyToSubjectDescriptorRemoteCache;
    private IMutableDictionaryCacheContainer<SubjectDescriptor, Guid> m_subjectDescriptorToStorageKeyRemoteCache;
    private TimeSpan m_cacheEntryTimeout;
    protected static readonly Guid s_namespace = new Guid("6d195b30-b54d-4864-9bb3-ed5173ef992b");
    private const string s_area = "User";
    private const string s_layer = "UserIdentifierConversionCacheService";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().RegisterNotification(systemRequestContext, "Default", DeploymentUserIdentityCacheService.DeploymentUserIdentityChanged, new SqlNotificationHandler(this.OnIdentityChanged), false);
      this.m_cacheEntryTimeout = systemRequestContext.GetService<IVssRegistryService>().GetValue<TimeSpan>(systemRequestContext, (RegistryQuery) "Configuration/Caching/UserIdentifierConversion/CacheEntryTimeout", UserIdentifierConversionCacheService.Constants.SettingDefaults.CacheEntryTimeout);
      IRedisCacheService service = systemRequestContext.GetService<IRedisCacheService>();
      this.m_storageKeyToSubjectDescriptorRemoteCache = service.GetVolatileDictionaryContainer<Guid, SubjectDescriptor, UserIdentifierConversionCacheService.SecurityToken>(systemRequestContext, UserIdentifierConversionCacheService.s_namespace, new ContainerSettings()
      {
        KeyExpiry = new TimeSpan?(this.m_cacheEntryTimeout),
        CiAreaName = nameof (UserIdentifierConversionCacheService),
        NoThrowMode = new bool?(true)
      });
      this.m_subjectDescriptorToStorageKeyRemoteCache = service.GetVolatileDictionaryContainer<SubjectDescriptor, Guid, UserIdentifierConversionCacheService.SecurityToken>(systemRequestContext, UserIdentifierConversionCacheService.s_namespace, new ContainerSettings()
      {
        KeyExpiry = new TimeSpan?(this.m_cacheEntryTimeout),
        CiAreaName = nameof (UserIdentifierConversionCacheService),
        NoThrowMode = new bool?(true)
      });
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public Guid GetStorageKey(
      IVssRequestContext deploymentContext,
      SubjectDescriptor subjectDescriptor)
    {
      if (subjectDescriptor == new SubjectDescriptor())
      {
        deploymentContext.TraceDataConditionally(808411201, TraceLevel.Verbose, "User", nameof (UserIdentifierConversionCacheService), "Input subject descriptor is empty", methodName: nameof (GetStorageKey));
        return new Guid();
      }
      UserIdentifierConversionMemoryCacheService service = deploymentContext.GetService<UserIdentifierConversionMemoryCacheService>();
      Guid storageKey = service.GetStorageKey(deploymentContext, subjectDescriptor);
      if (storageKey != new Guid())
        deploymentContext.TraceDataConditionally(808411202, TraceLevel.Verbose, "User", nameof (UserIdentifierConversionCacheService), "Found storage key in local cache", (Func<object>) (() => (object) new
        {
          subjectDescriptor = subjectDescriptor,
          storageKey = storageKey
        }), nameof (GetStorageKey));
      else if (this.m_subjectDescriptorToStorageKeyRemoteCache.TryGet<SubjectDescriptor, Guid>(deploymentContext, subjectDescriptor, out storageKey))
      {
        deploymentContext.TraceDataConditionally(808411203, TraceLevel.Verbose, "User", nameof (UserIdentifierConversionCacheService), "Found storage key in remote cache", (Func<object>) (() => (object) new
        {
          subjectDescriptor = subjectDescriptor,
          storageKey = storageKey
        }), nameof (GetStorageKey));
        service.Set(deploymentContext, subjectDescriptor, storageKey);
      }
      else
        deploymentContext.TraceDataConditionally(808411204, TraceLevel.Verbose, "User", nameof (UserIdentifierConversionCacheService), "Could not find storage key", (Func<object>) (() => (object) new
        {
          subjectDescriptor = subjectDescriptor
        }), nameof (GetStorageKey));
      return storageKey;
    }

    public SubjectDescriptor GetSubjectDescriptor(
      IVssRequestContext deploymentContext,
      Guid storageKey)
    {
      if (storageKey == new Guid())
      {
        deploymentContext.TraceDataConditionally(808411401, TraceLevel.Verbose, "User", nameof (UserIdentifierConversionCacheService), "Input storage key is empty", methodName: nameof (GetSubjectDescriptor));
        return new SubjectDescriptor();
      }
      UserIdentifierConversionMemoryCacheService service = deploymentContext.GetService<UserIdentifierConversionMemoryCacheService>();
      SubjectDescriptor subjectDescriptor = service.GetSubjectDescriptor(deploymentContext, storageKey);
      if (subjectDescriptor != new SubjectDescriptor())
        deploymentContext.TraceDataConditionally(808411402, TraceLevel.Verbose, "User", nameof (UserIdentifierConversionCacheService), "Found subject descriptor in local cache", (Func<object>) (() => (object) new
        {
          storageKey = storageKey,
          subjectDescriptor = subjectDescriptor
        }), nameof (GetSubjectDescriptor));
      else if (this.m_storageKeyToSubjectDescriptorRemoteCache.TryGet<Guid, SubjectDescriptor>(deploymentContext, storageKey, out subjectDescriptor))
      {
        deploymentContext.TraceDataConditionally(808411403, TraceLevel.Verbose, "User", nameof (UserIdentifierConversionCacheService), "Found subject descriptor in remote cache", (Func<object>) (() => (object) new
        {
          storageKey = storageKey,
          subjectDescriptor = subjectDescriptor
        }), nameof (GetSubjectDescriptor));
        service.Set(deploymentContext, subjectDescriptor, storageKey);
      }
      else
        deploymentContext.TraceDataConditionally(808411404, TraceLevel.Verbose, "User", nameof (UserIdentifierConversionCacheService), "Could not find subject descriptor", (Func<object>) (() => (object) new
        {
          storageKey = storageKey
        }), nameof (GetSubjectDescriptor));
      return subjectDescriptor;
    }

    public void Set(
      IVssRequestContext deploymentContext,
      SubjectDescriptor subjectDescriptor,
      Guid storageKey)
    {
      GraphValidation.CheckDescriptor(subjectDescriptor, nameof (subjectDescriptor));
      ArgumentUtility.CheckForEmptyGuid(storageKey, nameof (storageKey));
      deploymentContext.TraceDataConditionally(808411601, TraceLevel.Verbose, "User", nameof (UserIdentifierConversionCacheService), "Setting subject descriptor to storage key mapping", (Func<object>) (() => (object) new
      {
        subjectDescriptor = subjectDescriptor,
        storageKey = storageKey
      }), nameof (Set));
      deploymentContext.GetService<UserIdentifierConversionMemoryCacheService>().Set(deploymentContext, subjectDescriptor, storageKey);
      this.m_subjectDescriptorToStorageKeyRemoteCache.Set(deploymentContext, (IDictionary<SubjectDescriptor, Guid>) new Dictionary<SubjectDescriptor, Guid>()
      {
        {
          subjectDescriptor,
          storageKey
        }
      });
      this.m_storageKeyToSubjectDescriptorRemoteCache.Set(deploymentContext, (IDictionary<Guid, SubjectDescriptor>) new Dictionary<Guid, SubjectDescriptor>()
      {
        {
          storageKey,
          subjectDescriptor
        }
      });
    }

    public void Remove(
      IVssRequestContext deploymentContext,
      SubjectDescriptor subjectDescriptor = default (SubjectDescriptor),
      Guid storageKey = default (Guid))
    {
      deploymentContext.TraceDataConditionally(808411801, TraceLevel.Verbose, "User", nameof (UserIdentifierConversionCacheService), "Removing subject descriptor to storage key mapping", (Func<object>) (() => (object) new
      {
        subjectDescriptor = subjectDescriptor,
        storageKey = storageKey
      }), nameof (Remove));
      UserIdentifierConversionMemoryCacheService service = deploymentContext.GetService<UserIdentifierConversionMemoryCacheService>();
      Guid guid1 = subjectDescriptor != new SubjectDescriptor() ? service.GetStorageKey(deploymentContext, subjectDescriptor) : new Guid();
      SubjectDescriptor subjectDescriptor1 = storageKey != new Guid() ? service.GetSubjectDescriptor(deploymentContext, storageKey) : new SubjectDescriptor();
      IEnumerable<SubjectDescriptor> subjectDescriptorsToRemove = ((IEnumerable<SubjectDescriptor>) new SubjectDescriptor[2]
      {
        subjectDescriptor,
        subjectDescriptor1
      }).Where<SubjectDescriptor>((Func<SubjectDescriptor, bool>) (v => v != new SubjectDescriptor())).Distinct<SubjectDescriptor>();
      deploymentContext.TraceDataConditionally(808411802, TraceLevel.Verbose, "User", nameof (UserIdentifierConversionCacheService), "Removing subject descriptors from local cache", (Func<object>) (() => (object) subjectDescriptorsToRemove), nameof (Remove));
      foreach (SubjectDescriptor subjectDescriptor2 in subjectDescriptorsToRemove)
        service.Remove(deploymentContext, subjectDescriptor);
      IEnumerable<Guid> storageKeysToRemove = ((IEnumerable<Guid>) new Guid[2]
      {
        storageKey,
        guid1
      }).Where<Guid>((Func<Guid, bool>) (v => v != new Guid())).Distinct<Guid>();
      deploymentContext.TraceDataConditionally(808411803, TraceLevel.Verbose, "User", nameof (UserIdentifierConversionCacheService), "Removing storage keys from local cache", (Func<object>) (() => (object) storageKeysToRemove), nameof (Remove));
      foreach (Guid storageKey1 in storageKeysToRemove)
        service.Remove(deploymentContext, storageKey1);
      Guid guid2 = new Guid();
      SubjectDescriptor subjectDescriptor3 = new SubjectDescriptor();
      int num1 = subjectDescriptor != new SubjectDescriptor() ? (this.m_subjectDescriptorToStorageKeyRemoteCache.TryGet<SubjectDescriptor, Guid>(deploymentContext, subjectDescriptor, out guid2) ? 1 : 0) : 0;
      int num2 = storageKey != new Guid() ? (this.m_storageKeyToSubjectDescriptorRemoteCache.TryGet<Guid, SubjectDescriptor>(deploymentContext, storageKey, out subjectDescriptor3) ? 1 : 0) : 0;
      HashSet<SubjectDescriptor> subjectDescriptorsToInvalidate = new HashSet<SubjectDescriptor>();
      if (subjectDescriptor != new SubjectDescriptor())
        subjectDescriptorsToInvalidate.Add(subjectDescriptor);
      if (num2 != 0 && subjectDescriptor3 != new SubjectDescriptor())
        subjectDescriptorsToInvalidate.Add(subjectDescriptor3);
      deploymentContext.TraceDataConditionally(808411804, TraceLevel.Verbose, "User", nameof (UserIdentifierConversionCacheService), "Removing subject descriptors from remote cache", (Func<object>) (() => (object) subjectDescriptorsToInvalidate), nameof (Remove));
      this.m_subjectDescriptorToStorageKeyRemoteCache.Invalidate(deploymentContext, (IEnumerable<SubjectDescriptor>) subjectDescriptorsToInvalidate);
      HashSet<Guid> storageKeysToInvalidate = new HashSet<Guid>();
      if (storageKey != new Guid())
        storageKeysToInvalidate.Add(storageKey);
      if (num1 != 0 && guid2 != new Guid())
        storageKeysToInvalidate.Add(guid2);
      deploymentContext.TraceDataConditionally(808411805, TraceLevel.Verbose, "User", nameof (UserIdentifierConversionCacheService), "Removing storage keys from remote cache", (Func<object>) (() => (object) storageKeysToInvalidate), nameof (Remove));
      this.m_storageKeyToSubjectDescriptorRemoteCache.Invalidate(deploymentContext, (IEnumerable<Guid>) storageKeysToInvalidate);
    }

    private void OnIdentityChanged(IVssRequestContext requestContext, NotificationEventArgs args)
    {
      DeploymentUserIdentityChange identityChange = args.Deserialize<DeploymentUserIdentityChange>();
      if (identityChange == null)
        return;
      requestContext.TraceDataConditionally(808411901, TraceLevel.Verbose, "User", nameof (UserIdentifierConversionCacheService), "Processing identity change", (Func<object>) (() => (object) identityChange), nameof (OnIdentityChanged));
      this.Remove(requestContext, identityChange.SubjectDescriptor, identityChange.StorageKey);
    }

    private class SecurityToken
    {
    }

    public static class Constants
    {
      public static class SettingKeys
      {
        private const string Base = "Configuration/Caching/UserIdentifierConversion/";
        public const string CacheEntryTimeout = "Configuration/Caching/UserIdentifierConversion/CacheEntryTimeout";
      }

      public static class SettingDefaults
      {
        public static readonly TimeSpan CacheEntryTimeout = TimeSpan.FromHours(24.0);
      }
    }
  }
}
