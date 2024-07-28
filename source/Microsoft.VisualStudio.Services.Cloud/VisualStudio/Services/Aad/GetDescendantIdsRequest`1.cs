// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.GetDescendantIdsRequest`1
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Aad.Cache;
using Microsoft.VisualStudio.Services.Aad.Graph;
using Microsoft.VisualStudio.Services.Cloud;
using Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Aad
{
  public class GetDescendantIdsRequest<TIdentifier> : AadServiceRequest
  {
    public GetDescendantIdsRequest()
    {
    }

    internal GetDescendantIdsRequest(AadServiceRequest request) => this.CopyPropertiesFrom(request);

    public TIdentifier Identifier { get; set; }

    public int Expand { get; set; } = -1;

    internal override void Validate()
    {
      AadServiceUtils.ValidateId<TIdentifier>(this.Identifier, AadServiceUtils.IdentifierType.Group, "Identifier");
      if (this.Expand != -1)
        throw new ArgumentException("Unsupported group expansion: " + this.Expand.ToString());
    }

    internal override AadServiceResponse Execute(AadServiceRequestContext context) => this.GetDescendantIdsFromCacheWithMissHandling(context, (Func<IEnumerable<AadCacheKey>, IEnumerable<AadCacheLookup<AadCacheDescendantIds>>>) (keys => GetDescendantIdsRequest<TIdentifier>.OnCacheMiss(context, keys, GetDescendantIdsRequest<TIdentifier>.\u003C\u003EO.\u003C0\u003E__GetDescendantIdsUsingAadGraphClient ?? (GetDescendantIdsRequest<TIdentifier>.\u003C\u003EO.\u003C0\u003E__GetDescendantIdsUsingAadGraphClient = new Func<AadServiceRequestContext, Guid, GetDescendantIdsResponse>(GetDescendantIdsRequest<TIdentifier>.GetDescendantIdsUsingAadGraphClient)))));

    private AadServiceResponse GetDescendantIdsFromCacheWithMissHandling(
      AadServiceRequestContext context,
      Func<IEnumerable<AadCacheKey>, IEnumerable<AadCacheLookup<AadCacheDescendantIds>>> onCacheMiss)
    {
      try
      {
        context.VssRequestContext.TraceEnter(1035040, "VisualStudio.Services.Aad", "Service", nameof (GetDescendantIdsFromCacheWithMissHandling));
        Guid result;
        if (!Guid.TryParse(context.TenantId, out result))
          result = context.VssRequestContext.GetService<AadService>().GetTenant(context.VssRequestContext, new GetTenantRequest((AadServiceRequest) this)).Tenant.ObjectId;
        AadCacheKey aadCacheKey = new AadCacheKey(result, AadServiceUtils.MapIds<TIdentifier>(context.VssRequestContext, (IEnumerable<TIdentifier>) new TIdentifier[1]
        {
          this.Identifier
        }).Single<KeyValuePair<TIdentifier, Guid>>().Value);
        context.VssRequestContext.TraceConditionally(1035047, TraceLevel.Verbose, "VisualStudio.Services.Aad", "Service", (Func<string>) (() => string.Format("Getting Descendant Ids from AadCacheOrchestrator for objectId = {0}", (object) this.Identifier)));
        bool fromCache = true;
        AadDescendantIds descendants = context.VssRequestContext.GetService<IAadCacheOrchestrator>().GetObjects<AadCacheDescendantIds>(context.VssRequestContext, (IEnumerable<AadCacheKey>) new AadCacheKey[1]
        {
          aadCacheKey
        }, (Func<IEnumerable<AadCacheKey>, IEnumerable<AadCacheLookup<AadCacheDescendantIds>>>) (keys =>
        {
          fromCache = false;
          return onCacheMiss(keys);
        })).Single<AadCacheLookup<AadCacheDescendantIds>>().Result.Value;
        context.VssRequestContext.TraceConditionally(1035041, TraceLevel.Verbose, "VisualStudio.Services.Aad", "Service", (Func<string>) (() => descendants != null ? descendants.Serialize<AadDescendantIds>() : "null"));
        GetDescendantIdsResponse withMissHandling = new GetDescendantIdsResponse();
        withMissHandling.DescendantIds = descendants?.Identifiers;
        withMissHandling.IsComplete = descendants != null && descendants.IsComplete;
        withMissHandling.IncompletenessReason = descendants?.IncompletenessReason;
        withMissHandling.FromCache = new bool?(fromCache);
        return (AadServiceResponse) withMissHandling;
      }
      finally
      {
        context.VssRequestContext.TraceLeave(1035042, "VisualStudio.Services.Aad", "Service", nameof (GetDescendantIdsFromCacheWithMissHandling));
      }
    }

    internal override AadServiceResponse ExecuteWithMicrosoftGraph(
      AadServiceRequestContext context,
      bool bypassCache = false)
    {
      if (!bypassCache)
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        return this.GetDescendantIdsFromCacheWithMissHandling(context, (Func<IEnumerable<AadCacheKey>, IEnumerable<AadCacheLookup<AadCacheDescendantIds>>>) (keys => GetDescendantIdsRequest<TIdentifier>.OnCacheMiss(context, keys, GetDescendantIdsRequest<TIdentifier>.\u003C\u003EO.\u003C1\u003E__GetDescendantIdsUsingMicrosoftGraphClient ?? (GetDescendantIdsRequest<TIdentifier>.\u003C\u003EO.\u003C1\u003E__GetDescendantIdsUsingMicrosoftGraphClient = new Func<AadServiceRequestContext, Guid, GetDescendantIdsResponse>(GetDescendantIdsRequest<TIdentifier>.GetDescendantIdsUsingMicrosoftGraphClient)))));
      }
      return (AadServiceResponse) GetDescendantIdsRequest<TIdentifier>.GetDescendantIdsUsingMicrosoftGraphClient(context, AadServiceUtils.MapIds<TIdentifier>(context.VssRequestContext, (IEnumerable<TIdentifier>) new TIdentifier[1]
      {
        this.Identifier
      }).Single<KeyValuePair<TIdentifier, Guid>>().Value);
    }

    internal static IEnumerable<AadCacheLookup<AadCacheDescendantIds>> OnCacheMiss(
      AadServiceRequestContext context,
      IEnumerable<AadCacheKey> keys,
      Func<AadServiceRequestContext, Guid, GetDescendantIdsResponse> getDescendantIdsUsingGraphFunc)
    {
      List<AadCacheLookup<AadCacheDescendantIds>> aadCacheLookupList = new List<AadCacheLookup<AadCacheDescendantIds>>();
      foreach (AadCacheKey key1 in keys)
      {
        AadCacheKey key = key1;
        DateTimeOffset utcNow = DateTimeOffset.UtcNow;
        context.VssRequestContext.TraceConditionally(1035048, TraceLevel.Verbose, "VisualStudio.Services.Aad", "Service", (Func<string>) (() => string.Format("Cache missed occurred for objectId = {0} , getting descendant ids from Graph Client", (object) key.ObjectId)));
        GetDescendantIdsResponse descendantIdsResponse;
        try
        {
          descendantIdsResponse = getDescendantIdsUsingGraphFunc(context, key.ObjectId);
        }
        catch (AadGraphException ex)
        {
          context.VssRequestContext.TraceException(1035043, TraceLevel.Error, "VisualStudio.Services.Aad", "Service", (Exception) ex);
          aadCacheLookupList.Add(new AadCacheLookup<AadCacheDescendantIds>(key, AadCacheLookupStatus.Failure, (Exception) ex));
          return (IEnumerable<AadCacheLookup<AadCacheDescendantIds>>) aadCacheLookupList;
        }
        AadDescendantIds aadDescendantIds = new AadDescendantIds()
        {
          Identifiers = descendantIdsResponse.DescendantIds,
          IsComplete = descendantIdsResponse.IsComplete,
          IncompletenessReason = descendantIdsResponse.IncompletenessReason
        };
        if (aadDescendantIds.Identifiers == null)
          aadCacheLookupList.Add(new AadCacheLookup<AadCacheDescendantIds>(key, AadCacheLookupStatus.Failure, (Exception) null));
        else
          aadCacheLookupList.Add(new AadCacheLookup<AadCacheDescendantIds>(key, new AadCacheDescendantIds(key, aadDescendantIds, utcNow)));
      }
      return (IEnumerable<AadCacheLookup<AadCacheDescendantIds>>) aadCacheLookupList;
    }

    internal static GetDescendantIdsResponse GetDescendantIdsUsingAadGraphClient(
      AadServiceRequestContext context,
      Guid groupId)
    {
      string pagingToken = (string) null;
      int pagesTraversed = 0;
      bool flag = false;
      int setting1 = context.GetSetting<int>("/Service/Aad/GetDescendantIds/MaxResults", 9990);
      int setting2 = context.GetSetting<int>("/Service/Aad/GetDescendantIds/MaxPages", 10);
      int setting3 = context.GetSetting<int>("/Service/Aad/GetDescendantIds/MaxPageSize", 999);
      IAadGraphClient graphClient = context.GetGraphClient();
      JwtSecurityToken accessToken = context.GetAccessToken();
      HashSet<Tuple<Guid, AadObjectType>> descendantIds = new HashSet<Tuple<Guid, AadObjectType>>();
      context.VssRequestContext.TraceConditionally(1035044, TraceLevel.Verbose, "VisualStudio.Services.Aad", "Service", (Func<string>) (() => string.Format("GetDescendantIdsUsingGraphClient is requested with objectId = {0}", (object) groupId)));
      for (; descendantIds.Count < setting1 && pagesTraversed < setting2; pagesTraversed++)
      {
        context.VssRequestContext.TraceConditionally(1035045, TraceLevel.Verbose, "VisualStudio.Services.Aad", "Service", (Func<string>) (() => string.Format("Getting Aad descendant Ids at page = {0} with paging token = {1}", (object) pagesTraversed, (object) pagingToken)));
        IAadGraphClient aadGraphClient = graphClient;
        IVssRequestContext vssRequestContext = context.VssRequestContext;
        GetDescendantIdsRequest request = new GetDescendantIdsRequest();
        request.ObjectId = groupId;
        request.AccessToken = accessToken;
        request.PagingToken = pagingToken;
        request.MaxResults = new int?(setting3);
        Microsoft.VisualStudio.Services.Aad.Graph.GetDescendantIdsResponse descendantIds1 = aadGraphClient.GetDescendantIds(vssRequestContext, request);
        descendantIds.AddRange<Tuple<Guid, AadObjectType>, HashSet<Tuple<Guid, AadObjectType>>>((IEnumerable<Tuple<Guid, AadObjectType>>) descendantIds1.Identifiers);
        if (!descendantIds1.HasMoreResults)
        {
          flag = true;
          break;
        }
        pagingToken = descendantIds1.PagingToken;
      }
      if (descendantIds.Count > setting1)
        descendantIds = descendantIds.Take<Tuple<Guid, AadObjectType>>(setting1).ToHashSet<Tuple<Guid, AadObjectType>>();
      int num = setting1;
      if (setting2 <= pagesTraversed)
        num = setting2 * setting3;
      context.VssRequestContext.TraceConditionally(1035046, TraceLevel.Verbose, "VisualStudio.Services.Aad", "Service", (Func<string>) (() => string.Format("Aad descendant Ids response for object Id = {0} is {1}", (object) groupId, (object) descendantIds.Serialize<HashSet<Tuple<Guid, AadObjectType>>>())));
      return new GetDescendantIdsResponse()
      {
        IsComplete = flag,
        DescendantIds = (ISet<Tuple<Guid, AadObjectType>>) descendantIds,
        IncompletenessReason = !flag ? HostingResources.AadGroupTooBigToTraverseDescendants((object) num, (object) descendantIds.Count) : (string) null
      };
    }

    internal static GetDescendantIdsResponse GetDescendantIdsUsingMicrosoftGraphClient(
      AadServiceRequestContext context,
      Guid groupId)
    {
      string pagingToken = (string) null;
      int pagesTraversed = 0;
      bool flag = false;
      int setting1 = context.GetSetting<int>("/Service/Aad/GetDescendantIds/MaxResults", 9990);
      int setting2 = context.GetSetting<int>("/Service/Aad/GetDescendantIds/MaxPages", 10);
      int setting3 = context.GetSetting<int>("/Service/Aad/GetDescendantIds/MaxPageSize", 999);
      IMicrosoftGraphClient msGraphClient = context.GetMsGraphClient();
      JwtSecurityToken accessToken = context.GetAccessToken(true);
      HashSet<Tuple<Guid, AadObjectType>> descendantIds = new HashSet<Tuple<Guid, AadObjectType>>();
      context.VssRequestContext.TraceConditionally(1035044, TraceLevel.Verbose, "VisualStudio.Services.Aad", "Service", (Func<string>) (() => string.Format("GetDescendantIdsUsingGraphClient is requested with objectId = {0}", (object) groupId)));
      for (; descendantIds.Count < setting1 && pagesTraversed < setting2; pagesTraversed++)
      {
        context.VssRequestContext.TraceConditionally(1035045, TraceLevel.Verbose, "VisualStudio.Services.Aad", "Service", (Func<string>) (() => string.Format("Getting Aad descendant Ids at page = {0} with paging token = {1}", (object) pagesTraversed, (object) pagingToken)));
        IMicrosoftGraphClient microsoftGraphClient = msGraphClient;
        IVssRequestContext vssRequestContext = context.VssRequestContext;
        MsGraphGetDescendantIdsRequest request = new MsGraphGetDescendantIdsRequest();
        request.GroupId = groupId;
        request.AccessToken = accessToken;
        request.PagingToken = pagingToken;
        request.PageSize = new int?(setting3);
        MsGraphGetDescendantIdsResponse descendantIds1 = microsoftGraphClient.GetDescendantIds(vssRequestContext, request);
        descendantIds.AddRange<Tuple<Guid, AadObjectType>, HashSet<Tuple<Guid, AadObjectType>>>((IEnumerable<Tuple<Guid, AadObjectType>>) descendantIds1.DescendantIdAndTypes);
        if (!descendantIds1.HasMoreResults)
        {
          flag = true;
          break;
        }
        pagingToken = descendantIds1.PagingToken;
      }
      if (descendantIds.Count > setting1)
        descendantIds = descendantIds.Take<Tuple<Guid, AadObjectType>>(setting1).ToHashSet<Tuple<Guid, AadObjectType>>();
      int num = setting1;
      if (setting2 <= pagesTraversed)
        num = setting2 * setting3;
      context.VssRequestContext.TraceConditionally(1035046, TraceLevel.Verbose, "VisualStudio.Services.Aad", "Service", (Func<string>) (() => string.Format("Aad descendant Ids response for object Id = {0} is {1}", (object) groupId, (object) descendantIds.Serialize<HashSet<Tuple<Guid, AadObjectType>>>())));
      return new GetDescendantIdsResponse()
      {
        IsComplete = flag,
        DescendantIds = (ISet<Tuple<Guid, AadObjectType>>) descendantIds,
        IncompletenessReason = !flag ? HostingResources.AadGroupTooBigToTraverseDescendants((object) num, (object) descendantIds.Count) : (string) null
      };
    }

    internal override GraphApiSupportLevel GraphApiSupportLevel => GraphApiSupportLevel.BothAadAndMicrosoftGraph;
  }
}
