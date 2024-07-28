// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.Cache.AadCacheConstants
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.VisualStudio.Services.Aad.Cache
{
  internal static class AadCacheConstants
  {
    internal const string Area = "VisualStudio.Services.Aad";
    internal const string Layer = "Cache";
    internal const string ExpirationTime = "AadCacheConstants.ExpirationTime";

    internal static class FeatureFlags
    {
      internal const string LocalCache = "Microsoft.VisualStudio.Services.Aad.Cache.LocalCache";
      internal const string RemoteCache = "Microsoft.VisualStudio.Services.Aad.Cache.RemoteCache";
    }

    internal static class RegistryKeys
    {
      private const string Prefix = "/Service/Aad/Cache";
      private const string AncestorIdsSuffix = "/AncestorIds";
      private const string TenantSuffix = "/Tenant";
      private const string DescendantIdsSuffix = "/DescendantIds";
      private const string DirectoryRolesSuffix = "/DirectoryRoles";
      private const string DirectoryRoleMembersSuffix = "/DirectoryRoleMembers";
      private const string UserRolesAndGroupsSuffix = "/UserRolesAndGroups";

      internal static class Orchestrator
      {
        private const string OrchestratorPrefix = "/Service/Aad/Cache/Orchestrator";
        internal const string NotificationFilter = "/Service/Aad/Cache/Orchestrator/...";
        internal const string DefaultChunkSize = "/Service/Aad/Cache/Orchestrator/ChunkSize";
        internal static readonly IDictionary<Type, string> ChunkSize = (IDictionary<Type, string>) new ReadOnlyDictionary<Type, string>((IDictionary<Type, string>) new Dictionary<Type, string>()
        {
          {
            typeof (AadCacheAncestorIds),
            "/Service/Aad/Cache/Orchestrator/ChunkSize/AncestorIds"
          },
          {
            typeof (AadCacheDescendantIds),
            "/Service/Aad/Cache/Orchestrator/ChunkSize/DescendantIds"
          },
          {
            typeof (AadCacheTenant),
            "/Service/Aad/Cache/Orchestrator/ChunkSize/Tenant"
          },
          {
            typeof (AadCacheDirectoryRoles),
            "/Service/Aad/Cache/Orchestrator/ChunkSize/DirectoryRoles"
          },
          {
            typeof (AadCacheDirectoryRoleMembers),
            "/Service/Aad/Cache/Orchestrator/ChunkSize/DirectoryRoleMembers"
          },
          {
            typeof (AadCacheUserRolesAndGroups),
            "/Service/Aad/Cache/Orchestrator/ChunkSize/UserRolesAndGroups"
          }
        });
        internal const string DefaultLocalStalenessThreshold = "/Service/Aad/Cache/Orchestrator/LocalStalenessThreshold";
        internal static readonly IDictionary<Type, string> LocalStalenessThreshold = (IDictionary<Type, string>) new ReadOnlyDictionary<Type, string>((IDictionary<Type, string>) new Dictionary<Type, string>()
        {
          {
            typeof (AadCacheAncestorIds),
            "/Service/Aad/Cache/Orchestrator/LocalStalenessThreshold/AncestorIds"
          },
          {
            typeof (AadCacheDescendantIds),
            "/Service/Aad/Cache/Orchestrator/LocalStalenessThreshold/DescendantIds"
          },
          {
            typeof (AadCacheTenant),
            "/Service/Aad/Cache/Orchestrator/LocalStalenessThreshold/Tenant"
          },
          {
            typeof (AadCacheDirectoryRoles),
            "/Service/Aad/Cache/Orchestrator/LocalStalenessThreshold/DirectoryRoles"
          },
          {
            typeof (AadCacheDirectoryRoleMembers),
            "/Service/Aad/Cache/Orchestrator/LocalStalenessThreshold/DirectoryRoleMembers"
          },
          {
            typeof (AadCacheUserRolesAndGroups),
            "/Service/Aad/Cache/Orchestrator/LocalStalenessThreshold/UserRolesAndGroups"
          }
        });
        internal const string DefaultRemoteStalenessThreshold = "/Service/Aad/Cache/Orchestrator/RemoteStalenessThreshold";
        internal static readonly IDictionary<Type, string> RemoteStalenessThreshold = (IDictionary<Type, string>) new ReadOnlyDictionary<Type, string>((IDictionary<Type, string>) new Dictionary<Type, string>()
        {
          {
            typeof (AadCacheAncestorIds),
            "/Service/Aad/Cache/Orchestrator/RemoteStalenessThreshold/AncestorIds"
          },
          {
            typeof (AadCacheDescendantIds),
            "/Service/Aad/Cache/Orchestrator/RemoteStalenessThreshold/DescendantIds"
          },
          {
            typeof (AadCacheTenant),
            "/Service/Aad/Cache/Orchestrator/RemoteStalenessThreshold/Tenant"
          },
          {
            typeof (AadCacheDirectoryRoles),
            "/Service/Aad/Cache/Orchestrator/RemoteStalenessThreshold/DirectoryRoles"
          },
          {
            typeof (AadCacheDirectoryRoleMembers),
            "/Service/Aad/Cache/Orchestrator/RemoteStalenessThreshold/DirectoryRoleMembers"
          },
          {
            typeof (AadCacheUserRolesAndGroups),
            "/Service/Aad/Cache/Orchestrator/RemoteStalenessThreshold/UserRolesAndGroups"
          }
        });
        internal const string DefaultMaximumStalenessThreshold = "/Service/Aad/Cache/Orchestrator/MaximumStalenessThreshold";
        internal static readonly IDictionary<Type, string> MaximumStalenessThreshold = (IDictionary<Type, string>) new ReadOnlyDictionary<Type, string>((IDictionary<Type, string>) new Dictionary<Type, string>()
        {
          {
            typeof (AadCacheAncestorIds),
            "/Service/Aad/Cache/Orchestrator/MaximumStalenessThreshold/AncestorIds"
          },
          {
            typeof (AadCacheDescendantIds),
            "/Service/Aad/Cache/Orchestrator/MaximumStalenessThreshold/DescendantIds"
          },
          {
            typeof (AadCacheTenant),
            "/Service/Aad/Cache/Orchestrator/MaximumStalenessThreshold/Tenant"
          },
          {
            typeof (AadCacheDirectoryRoles),
            "/Service/Aad/Cache/Orchestrator/MaximumStalenessThreshold/DirectoryRoles"
          },
          {
            typeof (AadCacheDirectoryRoleMembers),
            "/Service/Aad/Cache/Orchestrator/MaximumStalenessThreshold/DirectoryRoleMembers"
          },
          {
            typeof (AadCacheUserRolesAndGroups),
            "/Service/Aad/Cache/Orchestrator/MaximumStalenessThreshold/UserRolesAndGroups"
          }
        });
        internal const string DefaultIsRemoteCacheEnabled = "/Service/Aad/Cache/Orchestrator/IsRemoteCacheEnabled";
        internal static readonly IDictionary<Type, string> IsRemoteCacheEnabled = (IDictionary<Type, string>) new ReadOnlyDictionary<Type, string>((IDictionary<Type, string>) new Dictionary<Type, string>()
        {
          {
            typeof (AadCacheAncestorIds),
            "/Service/Aad/Cache/Orchestrator/IsRemoteCacheEnabled/AncestorIds"
          },
          {
            typeof (AadCacheDescendantIds),
            "/Service/Aad/Cache/Orchestrator/IsRemoteCacheEnabled/DescendantIds"
          },
          {
            typeof (AadCacheTenant),
            "/Service/Aad/Cache/Orchestrator/IsRemoteCacheEnabled/Tenant"
          },
          {
            typeof (AadCacheDirectoryRoles),
            "/Service/Aad/Cache/Orchestrator/IsRemoteCacheEnabled/DirectoryRoles"
          },
          {
            typeof (AadCacheDirectoryRoleMembers),
            "/Service/Aad/Cache/Orchestrator/IsRemoteCacheEnabled/DirectoryRoleMembers"
          },
          {
            typeof (AadCacheUserRolesAndGroups),
            "/Service/Aad/Cache/Orchestrator/IsRemoteCacheEnabled/UserRolesAndGroups"
          }
        });
      }

      internal static class LocalCache
      {
        private const string LocalCachePrefix = "/Service/Aad/Cache/LocalCache";
        internal const string NotificationFilter = "/Service/Aad/Cache/LocalCache/...";
        internal const string DefaultContainerSize = "/Service/Aad/Cache/LocalCache/ContainerSize";
        internal static readonly IDictionary<Type, string> ContainerSize = (IDictionary<Type, string>) new ReadOnlyDictionary<Type, string>((IDictionary<Type, string>) new Dictionary<Type, string>()
        {
          {
            typeof (AadCacheAncestorIds),
            "/Service/Aad/Cache/LocalCache/ContainerSize/AncestorIds"
          },
          {
            typeof (AadCacheDescendantIds),
            "/Service/Aad/Cache/LocalCache/ContainerSize/DescendantIds"
          },
          {
            typeof (AadCacheTenant),
            "/Service/Aad/Cache/LocalCache/ContainerSize/Tenant"
          },
          {
            typeof (AadCacheDirectoryRoles),
            "/Service/Aad/Cache/LocalCache/ContainerSize/DirectoryRoles"
          },
          {
            typeof (AadCacheDirectoryRoleMembers),
            "/Service/Aad/Cache/LocalCache/ContainerSize/DirectoryRoleMembers"
          },
          {
            typeof (AadCacheUserRolesAndGroups),
            "/Service/Aad/Cache/LocalCache/ContainerSize/UserRolesAndGroups"
          }
        });
        internal const string DefaultEvictionSampleSize = "/Service/Aad/Cache/LocalCache/EvictionSampleSize";
        internal static readonly IDictionary<Type, string> EvictionSampleSize = (IDictionary<Type, string>) new ReadOnlyDictionary<Type, string>((IDictionary<Type, string>) new Dictionary<Type, string>()
        {
          {
            typeof (AadCacheAncestorIds),
            "/Service/Aad/Cache/LocalCache/EvictionSampleSize/AncestorIds"
          },
          {
            typeof (AadCacheDescendantIds),
            "/Service/Aad/Cache/LocalCache/EvictionSampleSize/DescendantIds"
          },
          {
            typeof (AadCacheTenant),
            "/Service/Aad/Cache/LocalCache/EvictionSampleSize/Tenant"
          },
          {
            typeof (AadCacheDirectoryRoles),
            "/Service/Aad/Cache/LocalCache/EvictionSampleSize/DirectoryRoles"
          },
          {
            typeof (AadCacheDirectoryRoleMembers),
            "/Service/Aad/Cache/LocalCache/EvictionSampleSize/DirectoryRoleMembers"
          },
          {
            typeof (AadCacheUserRolesAndGroups),
            "/Service/Aad/Cache/LocalCache/EvictionSampleSize/UserRolesAndGroups"
          }
        });
      }

      internal static class RemoteCache
      {
        private const string RemoteCachePrefix = "/Service/Aad/Cache/RemoteCache";
        internal const string NotificationFilter = "/Service/Aad/Cache/RemoteCache/...";
        private const string RedisNamespacePrefix = "/Service/Aad/Cache/RemoteCache/RedisNamespace";
        internal static readonly IDictionary<Type, string> RedisNamespace = (IDictionary<Type, string>) new ReadOnlyDictionary<Type, string>((IDictionary<Type, string>) new Dictionary<Type, string>()
        {
          {
            typeof (AadCacheAncestorIds),
            "/Service/Aad/Cache/RemoteCache/RedisNamespace/AncestorIds"
          },
          {
            typeof (AadCacheTenant),
            "/Service/Aad/Cache/RemoteCache/RedisNamespace/Tenant"
          }
        });
        private const string RedisKeyExpiryPrefix = "/Service/Aad/Cache/RemoteCache/RedisKeyExpiry";
        internal static readonly IDictionary<Type, string> RedisKeyExpiry = (IDictionary<Type, string>) new ReadOnlyDictionary<Type, string>((IDictionary<Type, string>) new Dictionary<Type, string>()
        {
          {
            typeof (AadCacheAncestorIds),
            "/Service/Aad/Cache/RemoteCache/RedisKeyExpiry/AncestorIds"
          },
          {
            typeof (AadCacheTenant),
            "/Service/Aad/Cache/RemoteCache/RedisKeyExpiry/Tenant"
          }
        });
        private const string RedisKeysChunkSizePrefix = "/Service/Aad/Cache/RemoteCache/RedisKeysChunkSize";
        internal static readonly IDictionary<Type, string> RedisKeysChunkSize = (IDictionary<Type, string>) new ReadOnlyDictionary<Type, string>((IDictionary<Type, string>) new Dictionary<Type, string>()
        {
          {
            typeof (AadCacheAncestorIds),
            "/Service/Aad/Cache/RemoteCache/RedisKeysChunkSize/AncestorIds"
          },
          {
            typeof (AadCacheTenant),
            "/Service/Aad/Cache/RemoteCache/RedisKeysChunkSize/Tenant"
          }
        });
      }
    }

    internal static class DefaultSettings
    {
      internal static class Orchestrator
      {
        internal static readonly int DefaultChunkSize = 10;
        internal static readonly IDictionary<Type, int> ChunkSize = (IDictionary<Type, int>) new ReadOnlyDictionary<Type, int>((IDictionary<Type, int>) new Dictionary<Type, int>()
        {
          {
            typeof (AadCacheAncestorIds),
            10
          },
          {
            typeof (AadCacheDescendantIds),
            AadCacheConstants.DefaultSettings.Orchestrator.DefaultChunkSize
          },
          {
            typeof (AadCacheTenant),
            1
          },
          {
            typeof (AadCacheDirectoryRoles),
            5
          },
          {
            typeof (AadCacheDirectoryRoleMembers),
            10
          },
          {
            typeof (AadCacheUserRolesAndGroups),
            5
          }
        });
        internal static readonly TimeSpan DefaultLocalStalenessThreshold = TimeSpan.FromMinutes(10.0);
        internal static readonly TimeSpan DefaultLocalStalenessForDescendantIds = TimeSpan.FromHours(4.0);
        internal static readonly TimeSpan DefaultLocalStalenessForDirectoryRoles = TimeSpan.FromHours(4.0);
        internal static readonly TimeSpan DefaultLocalStalenessForDirectoryRoleMembers = TimeSpan.FromHours(1.0);
        internal static readonly TimeSpan DefaultLocalStalenessForUserRolesAndGroups = TimeSpan.FromHours(1.0);
        internal static readonly IDictionary<Type, TimeSpan> LocalStalenessThreshold = (IDictionary<Type, TimeSpan>) new ReadOnlyDictionary<Type, TimeSpan>((IDictionary<Type, TimeSpan>) new Dictionary<Type, TimeSpan>()
        {
          {
            typeof (AadCacheAncestorIds),
            AadCacheConstants.DefaultSettings.Orchestrator.DefaultLocalStalenessThreshold
          },
          {
            typeof (AadCacheDescendantIds),
            AadCacheConstants.DefaultSettings.Orchestrator.DefaultLocalStalenessForDescendantIds
          },
          {
            typeof (AadCacheTenant),
            AadCacheConstants.DefaultSettings.Orchestrator.DefaultLocalStalenessThreshold
          },
          {
            typeof (AadCacheDirectoryRoles),
            AadCacheConstants.DefaultSettings.Orchestrator.DefaultLocalStalenessForDirectoryRoles
          },
          {
            typeof (AadCacheDirectoryRoleMembers),
            AadCacheConstants.DefaultSettings.Orchestrator.DefaultLocalStalenessForDirectoryRoleMembers
          },
          {
            typeof (AadCacheUserRolesAndGroups),
            AadCacheConstants.DefaultSettings.Orchestrator.DefaultLocalStalenessForUserRolesAndGroups
          }
        });
        internal static readonly TimeSpan DefaultRemoteStalenessThreshold = TimeSpan.FromHours(1.0);
        internal static readonly IDictionary<Type, TimeSpan> RemoteStalenessThreshold = (IDictionary<Type, TimeSpan>) new ReadOnlyDictionary<Type, TimeSpan>((IDictionary<Type, TimeSpan>) new Dictionary<Type, TimeSpan>()
        {
          {
            typeof (AadCacheAncestorIds),
            AadCacheConstants.DefaultSettings.Orchestrator.DefaultRemoteStalenessThreshold
          },
          {
            typeof (AadCacheDescendantIds),
            AadCacheConstants.DefaultSettings.Orchestrator.DefaultRemoteStalenessThreshold
          },
          {
            typeof (AadCacheTenant),
            AadCacheConstants.DefaultSettings.Orchestrator.DefaultRemoteStalenessThreshold
          },
          {
            typeof (AadCacheDirectoryRoles),
            AadCacheConstants.DefaultSettings.Orchestrator.DefaultRemoteStalenessThreshold
          },
          {
            typeof (AadCacheDirectoryRoleMembers),
            AadCacheConstants.DefaultSettings.Orchestrator.DefaultRemoteStalenessThreshold
          },
          {
            typeof (AadCacheUserRolesAndGroups),
            AadCacheConstants.DefaultSettings.Orchestrator.DefaultRemoteStalenessThreshold
          }
        });
        internal static readonly TimeSpan DefaultMaximumStalenessThreshold = TimeSpan.FromDays(14.0);
        internal static readonly IDictionary<Type, TimeSpan> MaximumStalenessThreshold = (IDictionary<Type, TimeSpan>) new ReadOnlyDictionary<Type, TimeSpan>((IDictionary<Type, TimeSpan>) new Dictionary<Type, TimeSpan>()
        {
          {
            typeof (AadCacheAncestorIds),
            AadCacheConstants.DefaultSettings.Orchestrator.DefaultMaximumStalenessThreshold
          },
          {
            typeof (AadCacheDescendantIds),
            AadCacheConstants.DefaultSettings.Orchestrator.DefaultMaximumStalenessThreshold
          },
          {
            typeof (AadCacheTenant),
            AadCacheConstants.DefaultSettings.Orchestrator.DefaultMaximumStalenessThreshold
          },
          {
            typeof (AadCacheDirectoryRoles),
            AadCacheConstants.DefaultSettings.Orchestrator.DefaultMaximumStalenessThreshold
          },
          {
            typeof (AadCacheDirectoryRoleMembers),
            AadCacheConstants.DefaultSettings.Orchestrator.DefaultMaximumStalenessThreshold
          },
          {
            typeof (AadCacheUserRolesAndGroups),
            AadCacheConstants.DefaultSettings.Orchestrator.DefaultMaximumStalenessThreshold
          }
        });
        internal static readonly bool DefaultIsRemoteCacheEnabled = true;
        internal static readonly bool DefaultIsRemoteCacheEnabledForDescendantIds = false;
        internal static readonly bool DefaultIsRemoteCacheEnabledForDirectoryRoles = false;
        internal static readonly bool DefaultIsRemoteCacheEnabledForDirectoryRoleMembers = false;
        internal static readonly bool DefaultIsRemoteCacheEnabledForUserRolesAndGroups = false;
        internal static readonly IDictionary<Type, bool> IsRemoteCacheEnabled = (IDictionary<Type, bool>) new ReadOnlyDictionary<Type, bool>((IDictionary<Type, bool>) new Dictionary<Type, bool>()
        {
          {
            typeof (AadCacheAncestorIds),
            AadCacheConstants.DefaultSettings.Orchestrator.DefaultIsRemoteCacheEnabled
          },
          {
            typeof (AadCacheDescendantIds),
            AadCacheConstants.DefaultSettings.Orchestrator.DefaultIsRemoteCacheEnabledForDescendantIds
          },
          {
            typeof (AadCacheTenant),
            AadCacheConstants.DefaultSettings.Orchestrator.DefaultIsRemoteCacheEnabled
          },
          {
            typeof (AadCacheDirectoryRoles),
            AadCacheConstants.DefaultSettings.Orchestrator.DefaultIsRemoteCacheEnabledForDirectoryRoles
          },
          {
            typeof (AadCacheDirectoryRoleMembers),
            AadCacheConstants.DefaultSettings.Orchestrator.DefaultIsRemoteCacheEnabledForDirectoryRoleMembers
          },
          {
            typeof (AadCacheUserRolesAndGroups),
            AadCacheConstants.DefaultSettings.Orchestrator.DefaultIsRemoteCacheEnabledForUserRolesAndGroups
          }
        });
      }

      internal static class LocalCache
      {
        internal const int DefaultContainerSize = 1024;
        internal static readonly IDictionary<Type, int> ContainerSize = (IDictionary<Type, int>) new ReadOnlyDictionary<Type, int>((IDictionary<Type, int>) new Dictionary<Type, int>()
        {
          {
            typeof (AadCacheAncestorIds),
            1000000
          },
          {
            typeof (AadCacheDescendantIds),
            1000000
          },
          {
            typeof (AadCacheTenant),
            100000
          },
          {
            typeof (AadCacheDirectoryRoles),
            100000
          },
          {
            typeof (AadCacheDirectoryRoleMembers),
            100000
          },
          {
            typeof (AadCacheUserRolesAndGroups),
            100000
          }
        });
        internal const int DefaultEvictionSampleSize = 5;
        internal static readonly IDictionary<Type, int> EvictionSampleSize = (IDictionary<Type, int>) new ReadOnlyDictionary<Type, int>((IDictionary<Type, int>) new Dictionary<Type, int>()
        {
          {
            typeof (AadCacheAncestorIds),
            5
          },
          {
            typeof (AadCacheDescendantIds),
            5
          },
          {
            typeof (AadCacheTenant),
            5
          },
          {
            typeof (AadCacheDirectoryRoles),
            5
          },
          {
            typeof (AadCacheDirectoryRoleMembers),
            5
          },
          {
            typeof (AadCacheUserRolesAndGroups),
            5
          }
        });
      }

      internal static class RemoteCache
      {
        internal static readonly IDictionary<Type, Guid> RedisNamespace = (IDictionary<Type, Guid>) new ReadOnlyDictionary<Type, Guid>((IDictionary<Type, Guid>) new Dictionary<Type, Guid>()
        {
          {
            typeof (AadCacheAncestorIds),
            new Guid("F86EE118-08F3-4B2B-8732-DD58EABAA02B")
          },
          {
            typeof (AadCacheTenant),
            new Guid("0112A6D7-6D8F-48CF-BB1C-510660E087BE")
          }
        });
        internal static readonly IDictionary<Type, int> RedisKeysChunkSize = (IDictionary<Type, int>) new ReadOnlyDictionary<Type, int>((IDictionary<Type, int>) new Dictionary<Type, int>()
        {
          {
            typeof (AadCacheAncestorIds),
            -1
          },
          {
            typeof (AadCacheTenant),
            -1
          }
        });
      }
    }
  }
}
