// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.GetUserRolesAndGroupsRequest
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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Aad
{
  public class GetUserRolesAndGroupsRequest : AadServiceRequest
  {
    public GetUserRolesAndGroupsRequest()
    {
    }

    internal GetUserRolesAndGroupsRequest(AadServiceRequest request) => this.CopyPropertiesFrom(request);

    public Guid UserObjectId { get; set; }

    internal override GraphApiSupportLevel GraphApiSupportLevel => GraphApiSupportLevel.BothAadAndMicrosoftGraph;

    internal override AadServiceResponse Execute(AadServiceRequestContext context) => this.GetUserRolesAndGroupsFromCacheWithMissHandling(context, (Func<IEnumerable<AadCacheKey>, IEnumerable<AadCacheLookup<AadCacheUserRolesAndGroups>>>) (keys => GetUserRolesAndGroupsRequest.OnCacheMiss(context, keys, new Func<AadServiceRequestContext, GetUserRolesAndGroupsResponse>(this.GetUserRolesAndGroupsUsingAadGraphClient))));

    internal override AadServiceResponse ExecuteWithMicrosoftGraph(
      AadServiceRequestContext context,
      bool bypassCache = false)
    {
      return !bypassCache ? this.GetUserRolesAndGroupsFromCacheWithMissHandling(context, (Func<IEnumerable<AadCacheKey>, IEnumerable<AadCacheLookup<AadCacheUserRolesAndGroups>>>) (keys => GetUserRolesAndGroupsRequest.OnCacheMiss(context, keys, new Func<AadServiceRequestContext, GetUserRolesAndGroupsResponse>(this.GetUserRolesAndGroupsUsingMicrosoftGraphClient)))) : (AadServiceResponse) this.GetUserRolesAndGroupsUsingMicrosoftGraphClient(context);
    }

    internal GetUserRolesAndGroupsResponse GetUserRolesAndGroupsUsingAadGraphClient(
      AadServiceRequestContext context)
    {
      IAadGraphClient graphClient = context.GetGraphClient();
      JwtSecurityToken accessToken = context.GetAccessToken();
      IAadGraphClient aadGraphClient = graphClient;
      IVssRequestContext vssRequestContext = context.VssRequestContext;
      Microsoft.VisualStudio.Services.Aad.Graph.GetUserRolesAndGroupsRequest request = new Microsoft.VisualStudio.Services.Aad.Graph.GetUserRolesAndGroupsRequest();
      request.AccessToken = accessToken;
      request.UserObjectId = this.UserObjectId;
      Microsoft.VisualStudio.Services.Aad.Graph.GetUserRolesAndGroupsResponse graphResponse = aadGraphClient.GetUserRolesAndGroups(vssRequestContext, request);
      if (graphResponse.Exception != null)
        throw new AadGraphException(graphResponse.Exception.Message);
      context.VssRequestContext.TraceConditionally(8525635, TraceLevel.Verbose, "VisualStudio.Services.Aad", "Service", (Func<string>) (() => graphResponse != null ? graphResponse.Serialize<Microsoft.VisualStudio.Services.Aad.Graph.GetUserRolesAndGroupsResponse>() : string.Empty));
      return new GetUserRolesAndGroupsResponse()
      {
        Members = graphResponse.Members
      };
    }

    internal GetUserRolesAndGroupsResponse GetUserRolesAndGroupsUsingMicrosoftGraphClient(
      AadServiceRequestContext context)
    {
      IMicrosoftGraphClient msGraphClient = context.GetMsGraphClient();
      JwtSecurityToken accessToken = context.GetAccessToken(true);
      IMicrosoftGraphClient microsoftGraphClient = msGraphClient;
      IVssRequestContext vssRequestContext = context.VssRequestContext;
      MsGraphGetUserRolesAndGroupsRequest request = new MsGraphGetUserRolesAndGroupsRequest();
      request.AccessToken = accessToken;
      request.UserObjectId = this.UserObjectId;
      MsGraphGetUserRolesAndGroupsResponse getUserRolesAndGroupsResponse = microsoftGraphClient.GetUserRolesAndGroups(vssRequestContext, request);
      if (getUserRolesAndGroupsResponse.Exception != null)
        throw new AadGraphException(getUserRolesAndGroupsResponse.Exception.Message);
      context.VssRequestContext.TraceConditionally(8525635, TraceLevel.Verbose, "VisualStudio.Services.Aad", "Service", (Func<string>) (() => getUserRolesAndGroupsResponse != null ? getUserRolesAndGroupsResponse.Serialize<MsGraphGetUserRolesAndGroupsResponse>() : string.Empty));
      return new GetUserRolesAndGroupsResponse()
      {
        Members = getUserRolesAndGroupsResponse.Members
      };
    }

    private AadServiceResponse GetUserRolesAndGroupsFromCacheWithMissHandling(
      AadServiceRequestContext context,
      Func<IEnumerable<AadCacheKey>, IEnumerable<AadCacheLookup<AadCacheUserRolesAndGroups>>> onCacheMiss)
    {
      try
      {
        context.VssRequestContext.TraceEnter(8525630, "VisualStudio.Services.Aad", "Service", nameof (GetUserRolesAndGroupsFromCacheWithMissHandling));
        Guid result;
        if (!Guid.TryParse(context.TenantId, out result))
          result = context.VssRequestContext.GetService<AadService>().GetTenant(context.VssRequestContext, new GetTenantRequest((AadServiceRequest) this)).Tenant.ObjectId;
        AadCacheKey aadCacheKey = new AadCacheKey(result, this.UserObjectId);
        bool fromCache = true;
        IAadCacheOrchestrator service = context.VssRequestContext.GetService<IAadCacheOrchestrator>();
        IVssRequestContext vssRequestContext = context.VssRequestContext;
        List<AadCacheKey> keys1 = new List<AadCacheKey>();
        keys1.Add(aadCacheKey);
        Func<IEnumerable<AadCacheKey>, IEnumerable<AadCacheLookup<AadCacheUserRolesAndGroups>>> onCacheMiss1 = (Func<IEnumerable<AadCacheKey>, IEnumerable<AadCacheLookup<AadCacheUserRolesAndGroups>>>) (keys =>
        {
          fromCache = false;
          return onCacheMiss(keys);
        });
        ISet<Guid> guidSet = service.GetObjects<AadCacheUserRolesAndGroups>(vssRequestContext, (IEnumerable<AadCacheKey>) keys1, onCacheMiss1).First<AadCacheLookup<AadCacheUserRolesAndGroups>>().Result.Value;
        if (guidSet == null)
          throw new AadException("Unable to get user Roles and Groups data.");
        GetUserRolesAndGroupsResponse withMissHandling = new GetUserRolesAndGroupsResponse();
        withMissHandling.Members = guidSet;
        withMissHandling.FromCache = new bool?(fromCache);
        return (AadServiceResponse) withMissHandling;
      }
      finally
      {
        context.VssRequestContext.TraceLeave(8525636, "VisualStudio.Services.Aad", "Service", nameof (GetUserRolesAndGroupsFromCacheWithMissHandling));
      }
    }

    internal static IEnumerable<AadCacheLookup<AadCacheUserRolesAndGroups>> OnCacheMiss(
      AadServiceRequestContext context,
      IEnumerable<AadCacheKey> aadCacheKeys,
      Func<AadServiceRequestContext, GetUserRolesAndGroupsResponse> getUserRolesAndGroupsUsingGraphFunc)
    {
      DateTimeOffset utcNow = DateTimeOffset.UtcNow;
      AadCacheKey key = aadCacheKeys.First<AadCacheKey>();
      GetUserRolesAndGroupsResponse andGroupsResponse;
      try
      {
        andGroupsResponse = getUserRolesAndGroupsUsingGraphFunc(context);
      }
      catch (Exception ex) when (ex is AadGraphException || ex is MicrosoftGraphException)
      {
        context.VssRequestContext.TraceException(8525633, TraceLevel.Error, "VisualStudio.Services.Aad", "Service", ex);
        return (IEnumerable<AadCacheLookup<AadCacheUserRolesAndGroups>>) new List<AadCacheLookup<AadCacheUserRolesAndGroups>>()
        {
          new AadCacheLookup<AadCacheUserRolesAndGroups>(key, AadCacheLookupStatus.Failure, ex)
        };
      }
      if (andGroupsResponse.Exception != null)
      {
        context.VssRequestContext.TraceException(8525631, TraceLevel.Error, "VisualStudio.Services.Aad", "Service", andGroupsResponse.Exception);
        return (IEnumerable<AadCacheLookup<AadCacheUserRolesAndGroups>>) new List<AadCacheLookup<AadCacheUserRolesAndGroups>>()
        {
          new AadCacheLookup<AadCacheUserRolesAndGroups>(key, AadCacheLookupStatus.Failure, GetUserRolesAndGroupsRequest.MapException(andGroupsResponse.Exception))
        };
      }
      context.VssRequestContext.Trace(8525632, TraceLevel.Info, "VisualStudio.Services.Aad", "Service", string.Format("{0} Roles And Groups found for User {1}", (object) andGroupsResponse.Members.Count, (object) key.ObjectId));
      return (IEnumerable<AadCacheLookup<AadCacheUserRolesAndGroups>>) new List<AadCacheLookup<AadCacheUserRolesAndGroups>>()
      {
        new AadCacheLookup<AadCacheUserRolesAndGroups>(key, new AadCacheUserRolesAndGroups(key, andGroupsResponse.Members, utcNow))
      };
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
