// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureResourceManagerService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Tokens;
using Microsoft.VisualStudio.Services.Cloud.AzureResourceManager;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class AzureResourceManagerService : IAzureResourceManagerService, IVssFrameworkService
  {
    private static readonly int TracePoint = 10011300;
    private static readonly string s_Area = nameof (AzureResourceManagerService);
    private static readonly string s_Layer = nameof (AzureResourceManagerService);
    private const string ProviderName = "Microsoft.VisualStudio";
    private const string ManagementUrl = "https://management.core.windows.net/";
    private const string ResourceProviderApiVersion = "/Service/Commerce/ResourceHydration/ApiVersion";
    private const string ClientPropertyName = "ms-client-name";
    private const string ClientPropertyValue = "ms-vso-client";

    public void ServiceStart(IVssRequestContext requestContext)
    {
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        requestContext.Trace(AzureResourceManagerService.TracePoint + 1, TraceLevel.Error, AzureResourceManagerService.s_Area, AzureResourceManagerService.s_Layer, "The Azure resource manager service is not supported in on-prem");
        throw new InvalidOperationException(FrameworkResources.ServiceAvailableInHostedTfsOnly());
      }
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public HttpResponseMessage RenameAzureResource(
      IVssRequestContext requestContext,
      string subscriptionId,
      string resourceGroup,
      string resourceType,
      string resourceOldName,
      string resourceNewName,
      Guid tenantId)
    {
      string apiVersion = requestContext.GetService<IVssRegistryService>().GetValue<string>(requestContext, (RegistryQuery) "/Service/Commerce/ResourceHydration/ApiVersion", string.Empty);
      requestContext.Trace(AzureResourceManagerService.TracePoint + 5, TraceLevel.Info, AzureResourceManagerService.s_Area, AzureResourceManagerService.s_Layer, "Resource Provider Api Version {0}", (object) apiVersion);
      using (IAzureResourceManagerClient resourceManagerClient = this.GetAzureResourceManagerClient(requestContext, this.GetApplicationAccessToken(requestContext, tenantId)))
      {
        string uriString = AzureResourceManagerService.AppendApiVersion(string.Format("{0}subscriptions/{1}/resourceGroups/{2}/providers/{3}/{4}/{5}/move", (object) resourceManagerClient.BaseAddress, (object) subscriptionId, (object) resourceGroup, (object) "Microsoft.VisualStudio", (object) resourceType, (object) resourceOldName), apiVersion);
        using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, new Uri(uriString)))
        {
          request.Headers.Add("ms-client-name", "ms-vso-client");
          string content = JsonConvert.SerializeObject((object) new AzureRenameData()
          {
            Id = string.Format("/subscriptions/{0}/resourceGroups/{1}/providers/{2}/{3}/{4}", (object) subscriptionId, (object) resourceGroup, (object) "Microsoft.VisualStudio", (object) resourceType, (object) resourceNewName)
          });
          request.Content = (HttpContent) new StringContent(content);
          request.Content.Headers.ContentType = (MediaTypeHeaderValue) new MediaTypeWithQualityHeaderValue("application/json");
          try
          {
            requestContext.Trace(AzureResourceManagerService.TracePoint + 6, TraceLevel.Info, AzureResourceManagerService.s_Area, AzureResourceManagerService.s_Layer, "Renaming resource: Location {0}; Body {1}", (object) uriString, (object) content);
            return resourceManagerClient.SendAsync(request).Result;
          }
          catch (Exception ex)
          {
            requestContext.TraceException(AzureResourceManagerService.TracePoint + 7, AzureResourceManagerService.s_Area, AzureResourceManagerService.s_Layer, ex);
            throw;
          }
        }
      }
    }

    protected virtual IAzureResourceManagerClient GetAzureResourceManagerClient(
      IVssRequestContext context,
      JwtSecurityToken token)
    {
      return (IAzureResourceManagerClient) new AzureResourceManagerClient(context, token);
    }

    private JwtSecurityToken GetApplicationAccessToken(IVssRequestContext context, Guid tenantId)
    {
      try
      {
        IVssRequestContext vssRequestContext = context.To(TeamFoundationHostType.Deployment).Elevate();
        return vssRequestContext.GetService<IAadTokenService>().AcquireAppToken(vssRequestContext, "https://management.core.windows.net/", tenantId.ToString());
      }
      catch (Exception ex)
      {
        context.TraceException(AzureResourceManagerService.TracePoint + 9, AzureResourceManagerService.s_Area, AzureResourceManagerService.s_Layer, ex);
        throw;
      }
    }

    private static T FromJson<T>(string json)
    {
      JsonSerializerSettings settings = new JsonSerializerSettings()
      {
        ContractResolver = (IContractResolver) new CamelCasePropertyNamesContractResolver(),
        Converters = (IList<JsonConverter>) new List<JsonConverter>()
        {
          (JsonConverter) new StringEnumConverter(),
          (JsonConverter) new IsoDateTimeConverter()
          {
            DateTimeStyles = DateTimeStyles.AssumeUniversal
          }
        }
      };
      return JsonConvert.DeserializeObject<T>(json, settings);
    }

    private static string AppendApiVersion(string url, string apiVersion) => !string.IsNullOrWhiteSpace(apiVersion) ? string.Format("{0}?api-version={1}", (object) url, (object) apiVersion) : url;
  }
}
