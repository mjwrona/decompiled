// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.InvalidateStaleAncestorIdsCacheRequest`1
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Aad.Cache;
using Microsoft.VisualStudio.Services.Aad.Graph;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Aad
{
  internal class InvalidateStaleAncestorIdsCacheRequest<TIdentifier> : AadServiceRequest
  {
    public InvalidateStaleAncestorIdsCacheRequest()
    {
    }

    internal InvalidateStaleAncestorIdsCacheRequest(AadServiceRequest request) => this.CopyPropertiesFrom(request);

    public AadObjectType ObjectType { get; set; }

    public TIdentifier Identifier { get; set; }

    public bool IsOnDemandUpdate { get; set; }

    internal override void Validate()
    {
      if (this.ObjectType != AadObjectType.User)
        throw new ArgumentException("Unsupported object type: " + this.ObjectType.ToString());
      AadServiceUtils.ValidateId<TIdentifier>(this.Identifier, name: "Identifier");
    }

    internal override AadServiceResponse Execute(AadServiceRequestContext context)
    {
      context.VssRequestContext.TraceEnter(1035070, "VisualStudio.Services.Aad", "Service", nameof (Execute));
      context.VssRequestContext.CheckDeploymentRequestContext();
      Guid tenantId;
      if (!Guid.TryParse(context.TenantId, out tenantId))
        context.VssRequestContext.Trace(1035071, TraceLevel.Error, "VisualStudio.Services.Aad", "Service", "TenantId is null or type is unsupported " + context.TenantId);
      AadCacheKey aadCacheKey = AadServiceUtils.MapIds<TIdentifier>(context.VssRequestContext, (IEnumerable<TIdentifier>) new TIdentifier[1]
      {
        this.Identifier
      }).Select<KeyValuePair<TIdentifier, Guid>, AadCacheKey>((Func<KeyValuePair<TIdentifier, Guid>, AadCacheKey>) (kvp => new AadCacheKey(tenantId, kvp.Value))).FirstOrDefault<AadCacheKey>();
      if (aadCacheKey == null)
        context.VssRequestContext.Trace(1035072, TraceLevel.Error, "VisualStudio.Services.Aad", "Service", string.Format("CacheKey not found for identifier {0}", (object) this.Identifier));
      InvalidateStaleAncestorIdsCacheResponse idsCacheResponse = new InvalidateStaleAncestorIdsCacheResponse()
      {
        CacheInvalidated = false
      };
      if (this.IsOnDemandUpdate || this.ShouldInvalidateAncestorsIdsCache(context, aadCacheKey))
      {
        InvalidateStaleAncestorIdsCacheRequest<TIdentifier>.InvalidateAadCaches(context, aadCacheKey);
        InvalidateStaleAncestorIdsCacheRequest<TIdentifier>.SendInvalidateStaleAncestorIdsCacheNotification(context.VssRequestContext, aadCacheKey);
        idsCacheResponse.CacheInvalidated = true;
      }
      return (AadServiceResponse) idsCacheResponse;
    }

    private bool ShouldInvalidateAncestorsIdsCache(
      AadServiceRequestContext context,
      AadCacheKey cacheKey)
    {
      ISet<Guid> cachedAncestorIds = InvalidateStaleAncestorIdsCacheRequest<TIdentifier>.GetCachedAncestorIds(context, cacheKey);
      ISet<Guid> graphAncestorIds;
      try
      {
        graphAncestorIds = this.GetGraphAncestorIds(context, cacheKey);
      }
      catch (Exception ex)
      {
        context.VssRequestContext.TraceException(1035076, TraceLevel.Error, "VisualStudio.Services.Aad", "Service", ex);
        return false;
      }
      return !InvalidateStaleAncestorIdsCacheRequest<TIdentifier>.SetEquals((ICollection<Guid>) cachedAncestorIds, (ICollection<Guid>) graphAncestorIds);
    }

    private static void InvalidateAadCaches(
      AadServiceRequestContext context,
      AadCacheKey invalidateAadCacheKeys)
    {
      InvalidateStaleAncestorIdsCacheRequest<TIdentifier>.RemoveLocalAadCacheObjects(context, invalidateAadCacheKeys);
      InvalidateStaleAncestorIdsCacheRequest<TIdentifier>.RemoveRemoteAadCacheObjects(context, invalidateAadCacheKeys);
    }

    private static ISet<Guid> GetCachedAncestorIds(
      AadServiceRequestContext context,
      AadCacheKey cacheKey)
    {
      ISet<Guid> cachedAncestorIds = context.VssRequestContext.GetService<IAadCacheOrchestrator>().GetObjects<AadCacheAncestorIds>(context.VssRequestContext, (IEnumerable<AadCacheKey>) new AadCacheKey[1]
      {
        cacheKey
      }).FirstOrDefault<AadCacheLookup<AadCacheAncestorIds>>()?.Result?.Value;
      if (context.VssRequestContext.IsTracing(1035073, TraceLevel.Verbose, "VisualStudio.Services.Aad", "Service"))
      {
        string message = cacheKey.ToString() + " : " + (cachedAncestorIds != null ? cachedAncestorIds.Serialize<ISet<Guid>>() : (string) null);
        InvalidateStaleAncestorIdsCacheRequest<TIdentifier>.TraceMessageWithChunking(context.VssRequestContext, 1035073, message);
      }
      return cachedAncestorIds;
    }

    private ISet<Guid> GetGraphAncestorIds(AadServiceRequestContext context, AadCacheKey cacheKey)
    {
      ISet<Guid> graphAncestorIds;
      if (context.VssRequestContext.IsFeatureEnabled("VisualStudio.Services.Aad.InvalidateStaleAncestorIdsCacheToUseAadService"))
      {
        AadService service = context.VssRequestContext.GetService<AadService>();
        IVssRequestContext vssRequestContext = context.VssRequestContext;
        GetAncestorIdsRequest<Guid> ancestorIdsRequest = new GetAncestorIdsRequest<Guid>();
        ancestorIdsRequest.ToTenant = cacheKey.TenantId.ToString();
        ancestorIdsRequest.ObjectType = this.ObjectType;
        ancestorIdsRequest.Identifiers = (IEnumerable<Guid>) new Guid[1]
        {
          cacheKey.ObjectId
        };
        ancestorIdsRequest.Expand = -1;
        ancestorIdsRequest.BypassCache = true;
        GetAncestorIdsRequest<Guid> request = ancestorIdsRequest;
        graphAncestorIds = service.GetAncestorIds<Guid>(vssRequestContext, request).Ancestors[cacheKey.ObjectId] ?? throw new Exception("MsGraph graphAncestorIdsAadService is null or throws exception. Check tracepoints 44750141 and 44750143 for details.");
      }
      else
        graphAncestorIds = InvalidateStaleAncestorIdsCacheRequest<TIdentifier>.GetAncestorIdsFromAadGraphClient(context.VssRequestContext, context.GetGraphClient(), context.GetAccessToken(), this.ObjectType, cacheKey)?.Value;
      if (context.VssRequestContext.IsTracing(1035074, TraceLevel.Verbose, "VisualStudio.Services.Aad", "Service"))
      {
        string message = cacheKey.ToString() + " : " + (graphAncestorIds != null ? graphAncestorIds.Serialize<ISet<Guid>>() : (string) null);
        InvalidateStaleAncestorIdsCacheRequest<TIdentifier>.TraceMessageWithChunking(context.VssRequestContext, 1035074, message);
      }
      return graphAncestorIds;
    }

    private static AadCacheAncestorIds GetAncestorIdsFromAadGraphClient(
      IVssRequestContext context,
      IAadGraphClient graphClient,
      JwtSecurityToken accessToken,
      AadObjectType objectType,
      AadCacheKey key)
    {
      GetAncestorIdsResponse ancestorIds;
      try
      {
        IAadGraphClient aadGraphClient = graphClient;
        IVssRequestContext context1 = context;
        GetAncestorIdsRequest ancestorIdsRequest = new GetAncestorIdsRequest();
        ancestorIdsRequest.AccessToken = accessToken;
        ancestorIdsRequest.ObjectType = objectType;
        ancestorIdsRequest.ObjectIds = (IEnumerable<Guid>) new Guid[1]
        {
          key.ObjectId
        };
        ancestorIdsRequest.Expand = -1;
        GetAncestorIdsRequest request = ancestorIdsRequest;
        ancestorIds = aadGraphClient.GetAncestorIds(context1, request);
      }
      catch (AadGraphException ex)
      {
        context.TraceException(1035075, TraceLevel.Error, "VisualStudio.Services.Aad", "Service", (Exception) ex);
        throw;
      }
      GetAncestorIdResponse ancestor = ancestorIds.Ancestors[key.ObjectId];
      if (ancestor != null && ancestor.Exception == null)
        return new AadCacheAncestorIds(key, ancestor.Ancestors, DateTimeOffset.UtcNow);
      throw new Exception("Graph ancestorIdResponse is null or throws exception.", ancestor?.Exception);
    }

    private static bool SetEquals(ICollection<Guid> a, ICollection<Guid> b)
    {
      if (a == null && b != null || a != null && b == null)
        return false;
      if (a == null)
        return true;
      return a.All<Guid>(new Func<Guid, bool>(b.Contains)) && b.All<Guid>(new Func<Guid, bool>(a.Contains));
    }

    private static void RemoveRemoteAadCacheObjects(
      AadServiceRequestContext context,
      AadCacheKey aadIdentifierKeys)
    {
      context.VssRequestContext.To(TeamFoundationHostType.Deployment).GetService<IAadRemoteCache>().RemoveObjects<AadCacheAncestorIds>(context.VssRequestContext, (IEnumerable<AadCacheKey>) new AadCacheKey[1]
      {
        aadIdentifierKeys
      });
    }

    private static void RemoveLocalAadCacheObjects(
      AadServiceRequestContext context,
      AadCacheKey aadIdentifierKeys)
    {
      context.VssRequestContext.To(TeamFoundationHostType.Deployment).GetService<IAadLocalCache>().RemoveObjects<AadCacheAncestorIds>(context.VssRequestContext, (IEnumerable<AadCacheKey>) new AadCacheKey[1]
      {
        aadIdentifierKeys
      });
    }

    private static void SendInvalidateStaleAncestorIdsCacheNotification(
      IVssRequestContext context,
      AadCacheKey message)
    {
      string eventData = message.Serialize<AadCacheKey>();
      context.GetService<ITeamFoundationSqlNotificationService>().SendNotification(context, SqlNotificationEventClasses.AadUserMembershipChanged, eventData);
    }

    private static void TraceMessageWithChunking(
      IVssRequestContext context,
      int tracePoint,
      string message)
    {
      if (message.Length < 20000)
      {
        context.Trace(tracePoint, TraceLevel.Verbose, "VisualStudio.Services.Aad", "Service", message);
      }
      else
      {
        for (int startIndex = 0; startIndex < message.Length; startIndex += 20000)
        {
          string str = message.Substring(startIndex, Math.Min(20000, message.Length - startIndex));
          context.Trace(tracePoint, TraceLevel.Verbose, "VisualStudio.Services.Aad", "Service", "(Start Index {0}) {1}", (object) startIndex, (object) str);
        }
      }
    }
  }
}
