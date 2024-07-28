// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.PlatformPropertyCacheService
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity.Cache;
using Microsoft.VisualStudio.Services.Security;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class PlatformPropertyCacheService : IPropertyCacheService, IVssFrameworkService
  {
    private const string s_Area = "PropertyCache";
    private const string s_Layer = "PlatformPropertyCacheService";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ExecutionEnvironment.IsHostedDeployment)
        throw new InvalidOperationException(FrameworkResources.ServiceAvailableInHostedTfsOnly());
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public string Cache<T>(IVssRequestContext requestContext, T value) where T : class
    {
      try
      {
        requestContext.TraceEnter(1099000, "PropertyCache", nameof (PlatformPropertyCacheService), nameof (Cache));
        if (requestContext.IsFeatureEnabled("VisualStudio.Services.PropertyCache.Disabled"))
          return string.Empty;
        ArgumentUtility.CheckForNull<T>(value, nameof (value));
        if (!this.IsFrameworkImpersonater(requestContext))
        {
          requestContext.Trace(1099001, TraceLevel.Warning, "PropertyCache", nameof (PlatformPropertyCacheService), "Request identity does not have framework impersonate permissions.");
          throw new AccessCheckException("Request identity does not have framework impersonate permissions.");
        }
        string key;
        if (!requestContext.GetService<IL2CacheService>().TrySet<T>(requestContext, value, out key))
        {
          requestContext.Trace(1099002, TraceLevel.Warning, "PropertyCache", nameof (PlatformPropertyCacheService), "Property cache fail to cache the key value pair.");
          throw new PropertyCacheServiceNotAvailableException("Property cache fail to cache the key value pair.");
        }
        return key;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1099008, "PropertyCache", nameof (PlatformPropertyCacheService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1099009, "PropertyCache", nameof (PlatformPropertyCacheService), nameof (Cache));
      }
    }

    public T Get<T>(IVssRequestContext requestContext, string key) where T : class
    {
      try
      {
        requestContext.TraceEnter(1098000, "PropertyCache", nameof (PlatformPropertyCacheService), nameof (Get));
        if (requestContext.IsFeatureEnabled("VisualStudio.Services.PropertyCache.Disabled"))
          return default (T);
        ArgumentUtility.CheckStringForNullOrWhiteSpace(key, nameof (key));
        if (!this.IsFrameworkImpersonater(requestContext))
        {
          requestContext.Trace(1098007, TraceLevel.Warning, "PropertyCache", nameof (PlatformPropertyCacheService), "Request identity does not have framework impersonate permissions.");
          throw new AccessCheckException("Request identity does not have framework impersonate permissions.");
        }
        T obj;
        if (!requestContext.GetService<IL2CacheService>().TryGet<T>(requestContext, key, out obj))
        {
          requestContext.Trace(1098002, TraceLevel.Warning, "PropertyCache", nameof (PlatformPropertyCacheService), "Property cache fail to get the cache object from a key.");
          throw new PropertyCacheServiceNotAvailableException("Property cache fail to get the cache object from a key.");
        }
        return obj;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1098008, "PropertyCache", nameof (PlatformPropertyCacheService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1098009, "PropertyCache", nameof (PlatformPropertyCacheService), nameof (Get));
      }
    }

    public void Delete(IVssRequestContext requestContext, string cacheKey)
    {
      try
      {
        requestContext.TraceEnter(1098010, "PropertyCache", nameof (PlatformPropertyCacheService), nameof (Delete));
        if (requestContext.IsFeatureEnabled("VisualStudio.Services.PropertyCache.Disabled"))
          return;
        ArgumentUtility.CheckStringForNullOrWhiteSpace(cacheKey, nameof (cacheKey));
        if (!this.IsFrameworkImpersonater(requestContext))
        {
          requestContext.Trace(1098007, TraceLevel.Warning, "PropertyCache", nameof (PlatformPropertyCacheService), "Request identity does not have framework impersonate permissions.");
          throw new AccessCheckException("Request identity does not have framework impersonate permissions.");
        }
        requestContext.GetService<IL2CacheService>().Invalidate<object>(requestContext, cacheKey);
      }
      finally
      {
        requestContext.TraceLeave(1098019, "PropertyCache", nameof (PlatformPropertyCacheService), nameof (Delete));
      }
    }

    private bool IsFrameworkImpersonater(IVssRequestContext requestContext) => requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.FrameworkNamespaceId).HasPermission(requestContext, FrameworkSecurity.FrameworkNamespaceToken, 4, false);
  }
}
