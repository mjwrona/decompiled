// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Location.Server.LocationServiceRedisHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Location.Server
{
  internal class LocationServiceRedisHelper
  {
    private static readonly Guid s_locationCacheRecoveryJob = Guid.Parse("B6BB6680-1B64-480A-800B-5E77BD357839");
    internal const string RedisCachingFeatureName = "VisualStudio.FrameworkService.LocationService.RedisCache";
    private static readonly Guid s_directoryCacheNamespace = new Guid("8AF0ED84-03E7-4B31-A230-6EF5910A7598");
    private static readonly ContainerSettings s_directoryContainerSettings = new ContainerSettings()
    {
      CiAreaName = "LocationServiceDirectory",
      NoThrowMode = new bool?(false)
    };
    private const int c_SchedulingLocationCacheRecoveryJobDelayInSeconds = 60;
    protected const string s_area = "LocationService";
    protected const string s_layer = "LocationServiceRedisHelper";

    protected virtual Guid GetNamespaceId(IVssRequestContext requestContext)
    {
      Guid namespaceId;
      requestContext.GetService<IRedisCacheService>().GetVolatileDictionaryContainer<Guid, Guid, LocationServiceRedisHelper.RedisCacheSecurityToken>(requestContext, LocationServiceRedisHelper.s_directoryCacheNamespace, LocationServiceRedisHelper.s_directoryContainerSettings).TryGet<Guid, Guid>(requestContext, Microsoft.VisualStudio.Services.Location.LocationServiceConstants.SelfReferenceIdentifier, out namespaceId);
      if (namespaceId == Guid.Empty)
      {
        namespaceId = Guid.NewGuid();
        this.SetNamespaceId(requestContext, namespaceId);
      }
      return namespaceId;
    }

    internal bool SetNamespaceId(
      IVssRequestContext requestContext,
      Guid namespaceId,
      bool handleErrors = true)
    {
      ArgumentUtility.CheckForEmptyGuid(namespaceId, nameof (namespaceId));
      if (requestContext.IsFeatureEnabled("VisualStudio.FrameworkService.LocationService.RedisCache"))
      {
        IMutableDictionaryCacheContainer<Guid, Guid> dictionaryContainer = requestContext.GetService<IRedisCacheService>().GetVolatileDictionaryContainer<Guid, Guid, LocationServiceRedisHelper.RedisCacheSecurityToken>(requestContext, LocationServiceRedisHelper.s_directoryCacheNamespace, LocationServiceRedisHelper.s_directoryContainerSettings);
        try
        {
          dictionaryContainer.Set(requestContext, (IDictionary<Guid, Guid>) new Dictionary<Guid, Guid>()
          {
            {
              Microsoft.VisualStudio.Services.Location.LocationServiceConstants.SelfReferenceIdentifier,
              namespaceId
            }
          });
        }
        catch (Exception ex) when (handleErrors)
        {
          requestContext.Trace(1903573053, TraceLevel.Error, "LocationService", nameof (LocationServiceRedisHelper), "Failed to set cache namespace Id: {0}", (object) (ex.InnerException?.Message ?? ex.Message));
          requestContext.GetService<ITeamFoundationJobService>().QueueDelayedJobs(requestContext.Elevate(), (IEnumerable<Guid>) new Guid[1]
          {
            LocationServiceRedisHelper.s_locationCacheRecoveryJob
          }, 60, JobPriorityLevel.Idle);
          return false;
        }
      }
      return true;
    }

    internal class RedisCacheSecurityToken
    {
    }
  }
}
