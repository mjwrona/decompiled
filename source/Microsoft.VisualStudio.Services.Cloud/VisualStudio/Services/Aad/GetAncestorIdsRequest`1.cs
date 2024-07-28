// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.GetAncestorIdsRequest`1
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.Graph;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Aad.Cache;
using Microsoft.VisualStudio.Services.Aad.Graph;
using Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory;
using Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Aad
{
  public class GetAncestorIdsRequest<TIdentifier> : AadServiceRequest
  {
    public GetAncestorIdsRequest()
    {
    }

    internal GetAncestorIdsRequest(AadServiceRequest request) => this.CopyPropertiesFrom(request);

    public AadObjectType ObjectType { get; set; }

    public IEnumerable<TIdentifier> Identifiers { get; set; }

    public int Expand { get; set; }

    public bool BypassCache { get; set; }

    internal override void Validate()
    {
      if (this.ObjectType != AadObjectType.User && this.ObjectType != AadObjectType.Group && this.ObjectType != AadObjectType.ServicePrincipal)
        throw new ArgumentException("Unsupported object type: " + this.ObjectType.ToString());
      AadServiceUtils.ValidateIds<TIdentifier>(this.Identifiers, name: "Identifiers");
      if (this.Expand != -1)
        throw new ArgumentException("Unsupported group expansion: " + this.Expand.ToString());
    }

    internal override AadServiceResponse Execute(AadServiceRequestContext context)
    {
      try
      {
        context.VssRequestContext.TraceEnter(1035031, "VisualStudio.Services.Aad", "Service", nameof (Execute));
        Guid tenantGuid;
        if (!Guid.TryParse(context.TenantId, out tenantGuid))
          tenantGuid = context.VssRequestContext.GetService<AadService>().GetTenant(context.VssRequestContext, new GetTenantRequest((AadServiceRequest) this)).Tenant.ObjectId;
        IEnumerable<KeyValuePair<TIdentifier, AadCacheKey>> keyValuePairs = AadServiceUtils.MapIds<TIdentifier>(context.VssRequestContext, this.Identifiers).Select<KeyValuePair<TIdentifier, Guid>, KeyValuePair<TIdentifier, AadCacheKey>>((Func<KeyValuePair<TIdentifier, Guid>, KeyValuePair<TIdentifier, AadCacheKey>>) (kvp => new KeyValuePair<TIdentifier, AadCacheKey>(kvp.Key, new AadCacheKey(tenantGuid, kvp.Value))));
        int aadMaxRequestsPerBatch = GetAncestorIdsRequest<TIdentifier>.GetAadMaxRequestsPerBatch(context);
        GetAncestorIdsResponse<TIdentifier> ancestorIdsResponse = new GetAncestorIdsResponse<TIdentifier>();
        ancestorIdsResponse.FromCache = new bool?(true);
        GetAncestorIdsResponse<TIdentifier> response = ancestorIdsResponse;
        IEnumerable<AadCacheLookup<AadCacheAncestorIds>> aadCacheLookups = (IEnumerable<AadCacheLookup<AadCacheAncestorIds>>) new List<AadCacheLookup<AadCacheAncestorIds>>();
        IEnumerable<AadCacheLookup<AadCacheAncestorIds>> second;
        if (this.BypassCache)
        {
          response.FromCache = new bool?(false);
          second = GetAncestorIdsRequest<TIdentifier>.OnCacheMiss(context.VssRequestContext, context.GetGraphClient(), context.GetAccessToken(), this.ObjectType, keyValuePairs.Select<KeyValuePair<TIdentifier, AadCacheKey>, AadCacheKey>((Func<KeyValuePair<TIdentifier, AadCacheKey>, AadCacheKey>) (kvp => kvp.Value)), aadMaxRequestsPerBatch);
        }
        else
          second = context.VssRequestContext.GetService<IAadCacheOrchestrator>().GetObjects<AadCacheAncestorIds>(context.VssRequestContext, keyValuePairs.Select<KeyValuePair<TIdentifier, AadCacheKey>, AadCacheKey>((Func<KeyValuePair<TIdentifier, AadCacheKey>, AadCacheKey>) (kvp => kvp.Value)), (Func<IEnumerable<AadCacheKey>, IEnumerable<AadCacheLookup<AadCacheAncestorIds>>>) (keys =>
          {
            response.FromCache = new bool?(false);
            return GetAncestorIdsRequest<TIdentifier>.OnCacheMiss(context.VssRequestContext, context.GetGraphClient(), context.GetAccessToken(), this.ObjectType, keys, aadMaxRequestsPerBatch);
          }));
        context.VssRequestContext.IsFeatureEnabled("VisualStudio.Services.Aad.GroupClaims");
        Dictionary<TIdentifier, ISet<Guid>> dictionary = keyValuePairs.Zip(second, (cacheKey, cacheLookup) => new
        {
          Key = cacheKey.Key,
          Value = cacheLookup.Result != null ? cacheLookup.Result.Value : (ISet<Guid>) null
        }).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        if (context.VssRequestContext.IsTracing(1035035, TraceLevel.Verbose, "VisualStudio.Services.Aad", "Service"))
          context.VssRequestContext.Trace(1035035, TraceLevel.Verbose, "VisualStudio.Services.Aad", "Service", dictionary == null ? "null" : dictionary.Serialize<Dictionary<TIdentifier, ISet<Guid>>>());
        response.Ancestors = (IDictionary<TIdentifier, ISet<Guid>>) dictionary;
        return (AadServiceResponse) response;
      }
      finally
      {
        context.VssRequestContext.TraceLeave(1035039, "VisualStudio.Services.Aad", "Service", nameof (Execute));
      }
    }

    internal override AadServiceResponse ExecuteWithMicrosoftGraph(
      AadServiceRequestContext context,
      bool bypassCache = false)
    {
      try
      {
        context.VssRequestContext.TraceEnter(1035031, "VisualStudio.Services.Aad", "Service", nameof (ExecuteWithMicrosoftGraph));
        Guid tenantGuid;
        if (!Guid.TryParse(context.TenantId, out tenantGuid))
          tenantGuid = context.VssRequestContext.GetService<AadService>().GetTenant(context.VssRequestContext, new GetTenantRequest((AadServiceRequest) this)).Tenant.ObjectId;
        IEnumerable<KeyValuePair<TIdentifier, AadCacheKey>> cacheKeys = AadServiceUtils.MapIds<TIdentifier>(context.VssRequestContext, this.Identifiers).Select<KeyValuePair<TIdentifier, Guid>, KeyValuePair<TIdentifier, AadCacheKey>>((Func<KeyValuePair<TIdentifier, Guid>, KeyValuePair<TIdentifier, AadCacheKey>>) (kvp => new KeyValuePair<TIdentifier, AadCacheKey>(kvp.Key, new AadCacheKey(tenantGuid, kvp.Value))));
        GetAncestorIdsRequest<TIdentifier>.GetAadMaxRequestsPerBatch(context);
        IEnumerable<AadCacheLookup<AadCacheAncestorIds>> aadCacheLookups = (IEnumerable<AadCacheLookup<AadCacheAncestorIds>>) new List<AadCacheLookup<AadCacheAncestorIds>>();
        IEnumerable<AadCacheLookup<AadCacheAncestorIds>> second = bypassCache || this.BypassCache ? GetAncestorIdsRequest<TIdentifier>.OnCacheMissWithMicrosoftGraph(context, cacheKeys.Select<KeyValuePair<TIdentifier, AadCacheKey>, AadCacheKey>((Func<KeyValuePair<TIdentifier, AadCacheKey>, AadCacheKey>) (kvp => kvp.Value))) : context.VssRequestContext.GetService<IAadCacheOrchestrator>().GetObjects<AadCacheAncestorIds>(context.VssRequestContext, cacheKeys.Select<KeyValuePair<TIdentifier, AadCacheKey>, AadCacheKey>((Func<KeyValuePair<TIdentifier, AadCacheKey>, AadCacheKey>) (kvp => kvp.Value)), (Func<IEnumerable<AadCacheKey>, IEnumerable<AadCacheLookup<AadCacheAncestorIds>>>) (keys => GetAncestorIdsRequest<TIdentifier>.OnCacheMissWithMicrosoftGraph(context, cacheKeys.Select<KeyValuePair<TIdentifier, AadCacheKey>, AadCacheKey>((Func<KeyValuePair<TIdentifier, AadCacheKey>, AadCacheKey>) (kvp => kvp.Value)))));
        Dictionary<TIdentifier, ISet<Guid>> ancestors = cacheKeys.Zip(second, (cacheKey, cacheLookup) => new
        {
          Key = cacheKey.Key,
          Value = cacheLookup.Result?.Value
        }).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        context.VssRequestContext.TraceConditionally(1035035, TraceLevel.Verbose, "VisualStudio.Services.Aad", "Service", (Func<string>) (() => ancestors != null ? ancestors.Serialize<Dictionary<TIdentifier, ISet<Guid>>>() : "null"));
        return (AadServiceResponse) new GetAncestorIdsResponse<TIdentifier>()
        {
          Ancestors = (IDictionary<TIdentifier, ISet<Guid>>) ancestors
        };
      }
      finally
      {
        context.VssRequestContext.TraceLeave(1035039, "VisualStudio.Services.Aad", "Service", nameof (ExecuteWithMicrosoftGraph));
      }
    }

    internal override GraphApiSupportLevel GraphApiSupportLevel => GraphApiSupportLevel.BothAadAndMicrosoftGraph;

    private static int GetAadMaxRequestsPerBatch(AadServiceRequestContext context)
    {
      int setting = context.GetSetting<int>("/Service/Aad/MaxRequestsPerBatch", 5);
      if (setting < 1)
        return 1;
      if (setting <= 5)
        return setting;
      context.VssRequestContext.TraceAlways(1035033, TraceLevel.Warning, "VisualStudio.Services.Aad", "Service", string.Format("Configured AAD MaxRequestsPerBatch:{0} exceeded AadGraphClientConstants.MaxRequestsPerBatch:{1}. Applying the lower value", (object) setting, (object) 5));
      return 5;
    }

    internal static IEnumerable<AadCacheLookup<AadCacheAncestorIds>> OnCacheMiss(
      IVssRequestContext context,
      IAadGraphClient graphClient,
      JwtSecurityToken accessToken,
      AadObjectType objectType,
      IEnumerable<AadCacheKey> keys,
      int maxRequestsPerBatch)
    {
      foreach (IEnumerable<AadCacheKey> source in keys.Batch<AadCacheKey>(maxRequestsPerBatch))
      {
        Dictionary<Guid, AadCacheKey> dedupedDictionary = source.ToDedupedDictionary<AadCacheKey, Guid, AadCacheKey>((Func<AadCacheKey, Guid>) (aadCacheKey => aadCacheKey.ObjectId), (Func<AadCacheKey, AadCacheKey>) (aadCacheKey => aadCacheKey));
        DateTimeOffset now = DateTimeOffset.UtcNow;
        GetAncestorIdsResponse graphResponse = (GetAncestorIdsResponse) null;
        Exception exception = (Exception) null;
        try
        {
          IAadGraphClient aadGraphClient = graphClient;
          IVssRequestContext context1 = context;
          GetAncestorIdsRequest request = new GetAncestorIdsRequest();
          request.AccessToken = accessToken;
          request.ObjectType = objectType;
          request.ObjectIds = (IEnumerable<Guid>) dedupedDictionary.Keys;
          request.Expand = -1;
          graphResponse = aadGraphClient.GetAncestorIds(context1, request);
        }
        catch (AadGraphException ex)
        {
          context.TraceException(1035020, TraceLevel.Error, "VisualStudio.Services.Aad", "Service", (Exception) ex);
          exception = (Exception) ex;
        }
        if (exception != null)
        {
          foreach (KeyValuePair<Guid, AadCacheKey> keyValuePair in dedupedDictionary)
            yield return new AadCacheLookup<AadCacheAncestorIds>(keyValuePair.Value, AadCacheLookupStatus.Failure, GetAncestorIdsRequest<TIdentifier>.MapException(exception));
        }
        else
        {
          foreach (KeyValuePair<Guid, AadCacheKey> keyValuePair in dedupedDictionary)
          {
            Guid key1 = keyValuePair.Key;
            AadCacheKey key2 = keyValuePair.Value;
            GetAncestorIdResponse ancestor = graphResponse.Ancestors[key1];
            if (ancestor == null || ancestor.Exception != null)
              yield return new AadCacheLookup<AadCacheAncestorIds>(key2, AadCacheLookupStatus.Failure, GetAncestorIdsRequest<TIdentifier>.MapException(ancestor?.Exception));
            else
              yield return new AadCacheLookup<AadCacheAncestorIds>(key2, new AadCacheAncestorIds(key2, ancestor.Ancestors, now));
          }
          now = new DateTimeOffset();
          graphResponse = (GetAncestorIdsResponse) null;
          exception = (Exception) null;
        }
      }
    }

    internal static IEnumerable<AadCacheLookup<AadCacheAncestorIds>> OnCacheMissWithMicrosoftGraph(
      AadServiceRequestContext context,
      IEnumerable<AadCacheKey> keys)
    {
      foreach (IEnumerable<AadCacheKey> source in keys.Batch<AadCacheKey>(GetAncestorIdsRequest<TIdentifier>.GetAadMaxRequestsPerBatch(context)))
      {
        Dictionary<Guid, AadCacheKey> dedupedDictionary = source.ToDedupedDictionary<AadCacheKey, Guid, AadCacheKey>((Func<AadCacheKey, Guid>) (aadCacheKey => aadCacheKey.ObjectId), (Func<AadCacheKey, AadCacheKey>) (aadCacheKey => aadCacheKey));
        DateTimeOffset now = DateTimeOffset.UtcNow;
        IMicrosoftGraphClient msGraphClient = context.GetMsGraphClient();
        JwtSecurityToken accessToken = context.GetAccessToken(true);
        IMicrosoftGraphClient microsoftGraphClient = msGraphClient;
        IVssRequestContext vssRequestContext = context.VssRequestContext;
        MsGraphGetAncestorIdsRequest request = new MsGraphGetAncestorIdsRequest();
        request.AccessToken = accessToken;
        request.ObjectIds = (ICollection<Guid>) dedupedDictionary.Keys;
        MsGraphGetAncestorIdsResponse graphResponse = microsoftGraphClient.GetAncestorIds(vssRequestContext, request);
        foreach (KeyValuePair<Guid, AadCacheKey> keyValuePair in dedupedDictionary)
        {
          Guid key1 = keyValuePair.Key;
          AadCacheKey key2 = keyValuePair.Value;
          MsGraphGetAncestorIdResponse ancestor = graphResponse.Ancestors[keyValuePair.Key];
          if (ancestor == null || ancestor.Exception != null)
            yield return new AadCacheLookup<AadCacheAncestorIds>(key2, AadCacheLookupStatus.Failure, GetAncestorIdsRequest<TIdentifier>.MapException(ancestor?.Exception));
          else
            yield return new AadCacheLookup<AadCacheAncestorIds>(key2, new AadCacheAncestorIds(key2, ancestor.Ancestors, now));
        }
        now = new DateTimeOffset();
        graphResponse = (MsGraphGetAncestorIdsResponse) null;
      }
    }

    private static Exception MapException(Exception ex)
    {
      switch (ex)
      {
        case ObjectNotFoundException _:
          return (Exception) new AadGraphObjectNotFoundException(ex);
        case ServiceException _:
          if (((ServiceException) ex).IsResourceNotFoundError())
            return (Exception) new MicrosoftGraphObjectNotFoundException(ex);
          break;
      }
      return (Exception) null;
    }
  }
}
