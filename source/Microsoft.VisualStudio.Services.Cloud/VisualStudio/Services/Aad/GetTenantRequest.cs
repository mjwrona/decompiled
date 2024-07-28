// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.GetTenantRequest
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Aad.Cache;
using Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Aad
{
  public class GetTenantRequest : AadServiceRequest
  {
    private static readonly string requestContextKeyFormat = typeof (GetTenantRequest).FullName + ".GetTenant({0})";

    public GetTenantRequest()
    {
    }

    internal GetTenantRequest(AadServiceRequest request) => this.CopyPropertiesFrom(request);

    internal override AadServiceResponse Execute(AadServiceRequestContext context) => GetTenantRequest.GetTenantIdFromCacheWithMissHandling(context, (Func<AadTenant>) (() =>
    {
      Microsoft.VisualStudio.Services.Aad.Graph.GetTenantRequest request = new Microsoft.VisualStudio.Services.Aad.Graph.GetTenantRequest()
      {
        AccessToken = context.GetAccessToken()
      };
      return context.GetGraphClient().GetTenant(context.VssRequestContext, request).Tenant;
    }));

    internal override AadServiceResponse ExecuteWithMicrosoftGraph(
      AadServiceRequestContext context,
      bool bypassCache = false)
    {
      if (!bypassCache)
        return GetTenantRequest.GetTenantIdFromCacheWithMissHandling(context, (Func<AadTenant>) (() =>
        {
          MsGraphGetTenantRequest request = new MsGraphGetTenantRequest()
          {
            AccessToken = context.GetAccessToken(true)
          };
          return context.GetMsGraphClient().GetTenant(context.VssRequestContext, request).Tenant;
        }));
      MsGraphGetTenantRequest getTenantRequest = new MsGraphGetTenantRequest();
      getTenantRequest.AccessToken = context.GetAccessToken(true);
      MsGraphGetTenantRequest request1 = getTenantRequest;
      GetTenantResponse getTenantResponse = new GetTenantResponse();
      getTenantResponse.Tenant = context.GetMsGraphClient().GetTenant(context.VssRequestContext, request1).Tenant;
      getTenantResponse.FromCache = new bool?(false);
      return (AadServiceResponse) getTenantResponse;
    }

    internal override GraphApiSupportLevel GraphApiSupportLevel => GraphApiSupportLevel.BothAadAndMicrosoftGraph;

    private static AadServiceResponse GetTenantIdFromCacheWithMissHandling(
      AadServiceRequestContext context,
      Func<AadTenant> funcGetTenantFromGraph)
    {
      string key1 = string.Format(GetTenantRequest.requestContextKeyFormat, (object) context.TenantId);
      AadTenant aadTenant1 = (AadTenant) null;
      if (context.VssRequestContext.Items.TryGetValue<AadTenant>(key1, out aadTenant1) && aadTenant1 != null)
      {
        GetTenantResponse withMissHandling = new GetTenantResponse();
        withMissHandling.Tenant = aadTenant1;
        withMissHandling.FromCache = new bool?(true);
        return (AadServiceResponse) withMissHandling;
      }
      bool fromCache = true;
      Guid result1;
      if (Guid.TryParse(context.TenantId, out result1))
      {
        AadCacheTenant result2 = context.VssRequestContext.GetService<IAadCacheOrchestrator>().GetObjects<AadCacheTenant>(context.VssRequestContext, (IEnumerable<AadCacheKey>) new AadCacheKey[1]
        {
          new AadCacheKey(result1, result1)
        }, (Func<IEnumerable<AadCacheKey>, IEnumerable<AadCacheLookup<AadCacheTenant>>>) (keys =>
        {
          fromCache = false;
          AadCacheKey key2 = keys.First<AadCacheKey>();
          AadTenant aadTenant2 = funcGetTenantFromGraph();
          return (IEnumerable<AadCacheLookup<AadCacheTenant>>) new AadCacheLookup<AadCacheTenant>[1]
          {
            new AadCacheLookup<AadCacheTenant>(key2, new AadCacheTenant(key2, aadTenant2))
          };
        })).First<AadCacheLookup<AadCacheTenant>>().Result;
        if (result2 != null)
          aadTenant1 = result2.Value;
      }
      else
      {
        if (!context.VssRequestContext.IsFeatureEnabled("VisualStudio.Services.Aad.DisableUseGuidMicrosoftServiceTenant"))
          context.VssRequestContext.Trace(1038080, TraceLevel.Warning, "VisualStudio.Services.Aad", "Service", "GetTenant received tenant not in Guid and is not Microsoft Service tenant: " + context.TenantId);
        fromCache = false;
        aadTenant1 = funcGetTenantFromGraph();
      }
      context.VssRequestContext.Items[key1] = aadTenant1 != null ? (object) aadTenant1 : throw new AadException("Unable to get tenant data.");
      GetTenantResponse withMissHandling1 = new GetTenantResponse();
      withMissHandling1.Tenant = aadTenant1;
      withMissHandling1.FromCache = new bool?(fromCache);
      return (AadServiceResponse) withMissHandling1;
    }
  }
}
