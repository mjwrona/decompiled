// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.PlatformAzureRmSubscriptionService
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Aad;
using Microsoft.VisualStudio.Services.CloudConfiguration;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server
{
  public class PlatformAzureRmSubscriptionService : 
    IAzureRmSubscriptionService2,
    IVssFrameworkService
  {
    private static readonly string SLayer = "EndpointService";
    private const int MaxSize = 2097152;
    private const int DefaultHttpTimeout = 30000;
    private const string DisplayNameString = "displayName";
    private const string TenantIdString = "tenantId";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public AzureSubscriptionQueryResult GetAzureSubscriptions(IVssRequestContext requestContext)
    {
      AzureSubscriptionQueryResult azureSubscriptions = new AzureSubscriptionQueryResult();
      try
      {
        List<AzureSubscriptionInfo> subscriptionsForUser = this.GetAzureSubscriptionsForUser(requestContext);
        azureSubscriptions.Value = new List<AzureSubscription>(subscriptionsForUser.Count);
        foreach (AzureSubscriptionInfo subscriptionInfo in subscriptionsForUser)
          azureSubscriptions.Value.Add(new AzureSubscription()
          {
            DisplayName = subscriptionInfo.DisplayName,
            SubscriptionId = subscriptionInfo.SubscriptionId,
            SubscriptionTenantId = subscriptionInfo.TenantId
          });
      }
      catch (Exception ex)
      {
        azureSubscriptions = new AzureSubscriptionQueryResult();
        string subscriptionsFailed = ServiceEndpointResources.GetAzureSubscriptionsFailed((object) ex.Message);
        requestContext.TraceError(34000825, PlatformAzureRmSubscriptionService.SLayer, subscriptionsFailed);
        azureSubscriptions.ErrorMessage = subscriptionsFailed;
      }
      requestContext.TraceLeave(0, PlatformAzureRmSubscriptionService.SLayer, nameof (GetAzureSubscriptions));
      return azureSubscriptions;
    }

    public AzureManagementGroupQueryResult GetAzureManagementGroups(
      IVssRequestContext requestContext)
    {
      AzureManagementGroupQueryResult managementGroups = new AzureManagementGroupQueryResult();
      try
      {
        List<AzureManagementGroupInfo> managementGroupsForUser = this.GetAzureManagementGroupsForUser(requestContext);
        managementGroups.Value = new List<AzureManagementGroup>(managementGroupsForUser.Count);
        foreach (AzureManagementGroupInfo managementGroupInfo in managementGroupsForUser)
        {
          AzureManagementGroup azureManagementGroup = new AzureManagementGroup()
          {
            Name = managementGroupInfo.Name,
            Id = managementGroupInfo.Id,
            DisplayName = managementGroupInfo.Properties["displayName"],
            TenantId = managementGroupInfo.Properties["tenantId"]
          };
          managementGroups.Value.Add(azureManagementGroup);
        }
      }
      catch (Exception ex)
      {
        managementGroups = new AzureManagementGroupQueryResult();
        string managementGroupsFailed = ServiceEndpointResources.GetAzureManagementGroupsFailed((object) ex.Message);
        requestContext.TraceError(34000826, PlatformAzureRmSubscriptionService.SLayer, managementGroupsFailed);
        managementGroups.ErrorMessage = managementGroupsFailed;
      }
      requestContext.TraceLeave(0, PlatformAzureRmSubscriptionService.SLayer, nameof (GetAzureManagementGroups));
      return managementGroups;
    }

    private List<AzureSubscriptionInfo> GetAzureSubscriptionsForUser(
      IVssRequestContext requestContext)
    {
      List<AzureSubscriptionInfo> subscriptionsForUser = new List<AzureSubscriptionInfo>();
      foreach (Guid tenantId in this.GetTenants(requestContext).Where<Guid>((Func<Guid, bool>) (tid => !Guid.Empty.Equals(tid))))
        subscriptionsForUser.AddRange(this.GetAzureSubscriptionInfo(requestContext, tenantId));
      return subscriptionsForUser;
    }

    private IEnumerable<AzureSubscriptionInfo> GetAzureSubscriptionInfo(
      IVssRequestContext requestContext,
      Guid tenantId)
    {
      HttpWebResponse response = (HttpWebResponse) null;
      try
      {
        HttpWebRequest httpWebRequest = (HttpWebRequest) WebRequest.Create(PlatformAzureRmSubscriptionService.BuildUriForListSubscriptions(requestContext));
        httpWebRequest.Method = "GET";
        httpWebRequest.Timeout = 30000;
        httpWebRequest.Headers.Add("Authorization", "Bearer " + MsalAzureAccessTokenHelper.GetResourceAccessToken(requestContext, tenantId.ToString(), (string) null, (string) null));
        response = (HttpWebResponse) httpWebRequest.GetResponse();
        return (IEnumerable<AzureSubscriptionInfo>) this.CreateResponseBodyObject(response)["value"].ToObject<List<AzureSubscriptionInfo>>();
      }
      catch (WebException ex)
      {
        string message = ex.Message;
        if (ex.Response != null)
        {
          using (Stream responseStream = ex.Response.GetResponseStream())
          {
            using (StreamReader streamReader = new StreamReader(responseStream))
            {
              string end = streamReader.ReadToEnd();
              message += end;
            }
          }
        }
        string subscriptionsFailed = ServiceEndpointResources.GetAzureSubscriptionsFailed((object) message);
        requestContext.TraceError(34000825, PlatformAzureRmSubscriptionService.SLayer, subscriptionsFailed);
        return (IEnumerable<AzureSubscriptionInfo>) Array.Empty<AzureSubscriptionInfo>();
      }
      catch (Exception ex)
      {
        string subscriptionsFailed = ServiceEndpointResources.GetAzureSubscriptionsFailed((object) ex.Message);
        requestContext.TraceError(34000825, PlatformAzureRmSubscriptionService.SLayer, subscriptionsFailed);
        return (IEnumerable<AzureSubscriptionInfo>) Array.Empty<AzureSubscriptionInfo>();
      }
      finally
      {
        response?.Dispose();
      }
    }

    private List<AzureManagementGroupInfo> GetAzureManagementGroupsForUser(
      IVssRequestContext requestContext)
    {
      List<AzureManagementGroupInfo> managementGroupsForUser = new List<AzureManagementGroupInfo>();
      foreach (Guid tenantId in this.GetTenants(requestContext).Where<Guid>((Func<Guid, bool>) (tid => !Guid.Empty.Equals(tid))))
        managementGroupsForUser.AddRange(this.GetAzureManagementGroupInfo(requestContext, tenantId));
      return managementGroupsForUser;
    }

    public virtual IEnumerable<AzureManagementGroupInfo> GetAzureManagementGroupInfo(
      IVssRequestContext requestContext,
      Guid tenantId)
    {
      HttpWebResponse response = (HttpWebResponse) null;
      try
      {
        HttpWebRequest httpWebRequest = (HttpWebRequest) WebRequest.Create(PlatformAzureRmSubscriptionService.BuildUriForGetManagementGroups(requestContext));
        httpWebRequest.Method = "GET";
        httpWebRequest.Timeout = 30000;
        httpWebRequest.Headers.Add("Authorization", "Bearer " + MsalAzureAccessTokenHelper.GetResourceAccessToken(requestContext, tenantId.ToString(), (string) null, (string) null));
        response = (HttpWebResponse) httpWebRequest.GetResponse();
        return (IEnumerable<AzureManagementGroupInfo>) this.CreateResponseBodyObject(response)["value"].ToObject<List<AzureManagementGroupInfo>>();
      }
      catch (WebException ex)
      {
        string message = ex.Message;
        if (ex.Response != null)
        {
          using (Stream responseStream = ex.Response.GetResponseStream())
          {
            using (StreamReader streamReader = new StreamReader(responseStream))
            {
              string end = streamReader.ReadToEnd();
              message += end;
            }
          }
        }
        string managementGroupsFailed = ServiceEndpointResources.GetAzureManagementGroupsFailed((object) message);
        requestContext.TraceError(34000826, PlatformAzureRmSubscriptionService.SLayer, managementGroupsFailed);
        return (IEnumerable<AzureManagementGroupInfo>) Array.Empty<AzureManagementGroupInfo>();
      }
      catch (Exception ex)
      {
        string managementGroupsFailed = ServiceEndpointResources.GetAzureManagementGroupsFailed((object) ex.Message);
        requestContext.TraceError(34000826, PlatformAzureRmSubscriptionService.SLayer, managementGroupsFailed);
        return (IEnumerable<AzureManagementGroupInfo>) Array.Empty<AzureManagementGroupInfo>();
      }
      finally
      {
        response?.Dispose();
      }
    }

    private JObject CreateResponseBodyObject(HttpWebResponse response)
    {
      if (response.ContentLength > 2097152L)
        throw new InvalidEndpointResponseException(ServiceEndpointResources.ResponseSizeExceeded((object) 2097152));
      using (JsonTextReader reader = new JsonTextReader((TextReader) new StreamReader(response.GetResponseStream())))
        return (JObject) new JsonSerializer().Deserialize((JsonReader) reader);
    }

    private static Uri BuildUriForGetManagementGroups(IVssRequestContext requestContext)
    {
      string uriString = "https://management.azure.com";
      string str = "2018-03-01-preview";
      if (requestContext.ExecutionEnvironment.IsDevFabricDeployment && !AzureAccessTokenProvider.UseProdAzureResourcesOnDevFabric(requestContext))
        uriString = "https://api-dogfood.resources.windows-int.net/";
      return new Uri(new Uri(uriString), "providers/Microsoft.Management/managementGroups?api-version=" + str);
    }

    private static Uri BuildUriForListSubscriptions(IVssRequestContext requestContext)
    {
      string uriString = "https://management.azure.com";
      string str = "2019-06-01";
      if (requestContext.ExecutionEnvironment.IsDevFabricDeployment && !AzureAccessTokenProvider.UseProdAzureResourcesOnDevFabric(requestContext))
        uriString = "https://api-dogfood.resources.windows-int.net/";
      return new Uri(new Uri(uriString), "subscriptions?api-version=" + str);
    }

    private List<Guid> GetTenants(IVssRequestContext requestContext)
    {
      try
      {
        if (requestContext.ExecutionEnvironment.IsDevFabricDeployment && AzureAccessTokenProvider.UseProdAzureResourcesOnDevFabric(requestContext))
          return new List<Guid>()
          {
            new Guid("72f988bf-86f1-41af-91ab-2d7cd011db47")
          };
        IVssRequestContext context1 = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
        AadService service = context1.GetService<AadService>();
        IVssRequestContext context2 = context1;
        GetTenantsRequest request = new GetTenantsRequest();
        request.ToMicrosoftServicesTenant = true;
        List<Guid> list = service.GetTenants(context2, request).Tenants.Select<AadTenant, Guid>((Func<AadTenant, Guid>) (tenant => tenant.ObjectId)).ToList<Guid>();
        if (!list.Any<Guid>())
          list.Add(AadIdentityHelper.GetIdentityTenantId(requestContext.UserContext));
        if (list.Any<Guid>())
        {
          string str = "Tenants returned: " + string.Join(",", list.Select<Guid, string>((Func<Guid, string>) (id => id.ToString())));
        }
        return list;
      }
      catch (Exception ex)
      {
        requestContext.TraceError(34000826, PlatformAzureRmSubscriptionService.SLayer, ex.ToString());
        return new List<Guid>();
      }
    }
  }
}
