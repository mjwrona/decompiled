// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.GetDirectoryRolesRequest
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
  public class GetDirectoryRolesRequest : AadServiceRequest
  {
    public GetDirectoryRolesRequest()
    {
    }

    internal GetDirectoryRolesRequest(AadServiceRequest request) => this.CopyPropertiesFrom(request);

    internal override GraphApiSupportLevel GraphApiSupportLevel => GraphApiSupportLevel.BothAadAndMicrosoftGraph;

    internal override AadServiceResponse Execute(AadServiceRequestContext context) => GetDirectoryRolesRequest.GetDirectoryRolesFromCacheWithMissHandling(context, (Func<IEnumerable<AadCacheKey>, IEnumerable<AadCacheLookup<AadCacheDirectoryRoles>>>) (keys => GetDirectoryRolesRequest.OnCacheMiss(context, keys, GetDirectoryRolesRequest.\u003C\u003EO.\u003C0\u003E__GetDirectoryRolesUsingAadGraphClient ?? (GetDirectoryRolesRequest.\u003C\u003EO.\u003C0\u003E__GetDirectoryRolesUsingAadGraphClient = new Func<AadServiceRequestContext, GetDirectoryRolesResponse>(GetDirectoryRolesRequest.GetDirectoryRolesUsingAadGraphClient)))));

    internal static GetDirectoryRolesResponse GetDirectoryRolesUsingAadGraphClient(
      AadServiceRequestContext context)
    {
      context.VssRequestContext.TraceEnter(8525500, "VisualStudio.Services.Aad", "Service", nameof (GetDirectoryRolesUsingAadGraphClient));
      IAadGraphClient graphClient = context.GetGraphClient();
      JwtSecurityToken accessToken = context.GetAccessToken();
      IVssRequestContext vssRequestContext = context.VssRequestContext;
      Microsoft.VisualStudio.Services.Aad.Graph.GetDirectoryRolesRequest request = new Microsoft.VisualStudio.Services.Aad.Graph.GetDirectoryRolesRequest();
      request.AccessToken = accessToken;
      Microsoft.VisualStudio.Services.Aad.Graph.GetDirectoryRolesResponse directoryRoles = graphClient.GetDirectoryRoles(vssRequestContext, request);
      return new GetDirectoryRolesResponse()
      {
        DirectoryRoles = directoryRoles.DirectoryRoles
      };
    }

    internal static IEnumerable<AadCacheLookup<AadCacheDirectoryRoles>> OnCacheMiss(
      AadServiceRequestContext context,
      IEnumerable<AadCacheKey> keys,
      Func<AadServiceRequestContext, GetDirectoryRolesResponse> getDirectoryRolesUsingGraphFunc)
    {
      DateTimeOffset utcNow = DateTimeOffset.UtcNow;
      AadCacheKey key = keys.First<AadCacheKey>();
      List<AadCacheLookup<AadCacheDirectoryRoles>> aadCacheLookupList = new List<AadCacheLookup<AadCacheDirectoryRoles>>();
      GetDirectoryRolesResponse directoryRolesResponse;
      try
      {
        directoryRolesResponse = getDirectoryRolesUsingGraphFunc(context);
      }
      catch (AadGraphException ex)
      {
        context.VssRequestContext.TraceException(8525502, TraceLevel.Error, "VisualStudio.Services.Aad", "Service", (Exception) ex);
        aadCacheLookupList.Add(new AadCacheLookup<AadCacheDirectoryRoles>(key, AadCacheLookupStatus.Failure, (Exception) ex));
        return (IEnumerable<AadCacheLookup<AadCacheDirectoryRoles>>) aadCacheLookupList;
      }
      if (directoryRolesResponse.Exception != null)
      {
        context.VssRequestContext.TraceException(8525503, TraceLevel.Error, "VisualStudio.Services.Aad", "Service", directoryRolesResponse.Exception);
        return (IEnumerable<AadCacheLookup<AadCacheDirectoryRoles>>) new List<AadCacheLookup<AadCacheDirectoryRoles>>()
        {
          new AadCacheLookup<AadCacheDirectoryRoles>(key, AadCacheLookupStatus.Failure, GetDirectoryRolesRequest.MapException(directoryRolesResponse.Exception))
        };
      }
      context.VssRequestContext.Trace(8525504, TraceLevel.Info, "VisualStudio.Services.Aad", "Service", string.Format("{0} directory roles found", (object) directoryRolesResponse.DirectoryRoles.Count<AadDirectoryRole>()));
      return (IEnumerable<AadCacheLookup<AadCacheDirectoryRoles>>) new List<AadCacheLookup<AadCacheDirectoryRoles>>()
      {
        new AadCacheLookup<AadCacheDirectoryRoles>(key, new AadCacheDirectoryRoles(key, (ISet<AadDirectoryRole>) directoryRolesResponse.DirectoryRoles.ToHashSet<AadDirectoryRole>(), utcNow))
      };
    }

    internal override AadServiceResponse ExecuteWithMicrosoftGraph(
      AadServiceRequestContext context,
      bool bypassCache = false)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return !bypassCache ? GetDirectoryRolesRequest.GetDirectoryRolesFromCacheWithMissHandling(context, (Func<IEnumerable<AadCacheKey>, IEnumerable<AadCacheLookup<AadCacheDirectoryRoles>>>) (keys => GetDirectoryRolesRequest.OnCacheMiss(context, keys, GetDirectoryRolesRequest.\u003C\u003EO.\u003C1\u003E__GetDirectoryRolesUsingMicrosoftGraphClient ?? (GetDirectoryRolesRequest.\u003C\u003EO.\u003C1\u003E__GetDirectoryRolesUsingMicrosoftGraphClient = new Func<AadServiceRequestContext, GetDirectoryRolesResponse>(GetDirectoryRolesRequest.GetDirectoryRolesUsingMicrosoftGraphClient))))) : (AadServiceResponse) GetDirectoryRolesRequest.GetDirectoryRolesUsingMicrosoftGraphClient(context);
    }

    internal static GetDirectoryRolesResponse GetDirectoryRolesUsingMicrosoftGraphClient(
      AadServiceRequestContext context)
    {
      context.VssRequestContext.TraceEnter(8525500, "VisualStudio.Services.Aad", "Service", nameof (GetDirectoryRolesUsingMicrosoftGraphClient));
      IMicrosoftGraphClient msGraphClient = context.GetMsGraphClient();
      JwtSecurityToken accessToken = context.GetAccessToken(true);
      IVssRequestContext vssRequestContext = context.VssRequestContext;
      MsGraphGetDirectoryRolesRequest request = new MsGraphGetDirectoryRolesRequest();
      request.AccessToken = accessToken;
      MsGraphGetDirectoryRolesResponse directoryRoles = msGraphClient.GetDirectoryRoles(vssRequestContext, request);
      return new GetDirectoryRolesResponse()
      {
        DirectoryRoles = directoryRoles.DirectoryRoles
      };
    }

    private static AadServiceResponse GetDirectoryRolesFromCacheWithMissHandling(
      AadServiceRequestContext context,
      Func<IEnumerable<AadCacheKey>, IEnumerable<AadCacheLookup<AadCacheDirectoryRoles>>> onCacheMiss)
    {
      try
      {
        context.VssRequestContext.TraceEnter(8525500, "VisualStudio.Services.Aad", "Service", nameof (GetDirectoryRolesFromCacheWithMissHandling));
        Guid result1;
        if (!Guid.TryParse(context.TenantId, out result1))
          result1 = context.VssRequestContext.GetService<AadService>().GetTenant(context.VssRequestContext, new GetTenantRequest()).Tenant.ObjectId;
        AadCacheKey aadCacheKey = new AadCacheKey(result1, Guid.Empty);
        ISet<AadDirectoryRole> directoryRoles = (ISet<AadDirectoryRole>) null;
        bool fromCache = true;
        IAadCacheOrchestrator service = context.VssRequestContext.GetService<IAadCacheOrchestrator>();
        IVssRequestContext vssRequestContext = context.VssRequestContext;
        List<AadCacheKey> keys1 = new List<AadCacheKey>();
        keys1.Add(aadCacheKey);
        Func<IEnumerable<AadCacheKey>, IEnumerable<AadCacheLookup<AadCacheDirectoryRoles>>> onCacheMiss1 = (Func<IEnumerable<AadCacheKey>, IEnumerable<AadCacheLookup<AadCacheDirectoryRoles>>>) (keys =>
        {
          fromCache = false;
          return onCacheMiss(keys);
        });
        AadCacheDirectoryRoles result2 = service.GetObjects<AadCacheDirectoryRoles>(vssRequestContext, (IEnumerable<AadCacheKey>) keys1, onCacheMiss1).First<AadCacheLookup<AadCacheDirectoryRoles>>().Result;
        if (result2 != null)
          directoryRoles = result2.Value;
        if (directoryRoles == null)
          throw new AadException("Unable to get directoryRoles data.");
        context.VssRequestContext.TraceConditionally(8525505, TraceLevel.Verbose, "VisualStudio.Services.Aad", "Service", (Func<string>) (() => directoryRoles != null ? directoryRoles.Serialize<ISet<AadDirectoryRole>>() : string.Empty));
        GetDirectoryRolesResponse withMissHandling = new GetDirectoryRolesResponse();
        withMissHandling.DirectoryRoles = (IEnumerable<AadDirectoryRole>) directoryRoles;
        withMissHandling.FromCache = new bool?(fromCache);
        return (AadServiceResponse) withMissHandling;
      }
      finally
      {
        context.VssRequestContext.TraceLeave(8525510, "VisualStudio.Services.Aad", "Service", nameof (GetDirectoryRolesFromCacheWithMissHandling));
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
