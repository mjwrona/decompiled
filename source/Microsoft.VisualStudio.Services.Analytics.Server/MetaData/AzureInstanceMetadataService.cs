// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.MetaData.AzureInstanceMetadataService
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CloudConfiguration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Analytics.MetaData
{
  public class AzureInstanceMetadataService : IAzureInstanceMetadataService, IVssFrameworkService
  {
    public const string Area = "Analytics";
    public const string Layer = "AzureInstanceMetadata";
    private const string c_azureMetadataEndpoint = "http://169.254.169.254/metadata/instance?api-version=2018-10-01";

    public void ServiceStart(IVssRequestContext requestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public async Task<AzureInstanceMetadata> GetInstanceMetadataAsync(HttpClient httpClient)
    {
      HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new Uri("http://169.254.169.254/metadata/instance?api-version=2018-10-01"));
      request.Headers.Add("Metadata", "true");
      HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead);
      AzureInstanceMetadata instanceMetadataAsync = response.IsSuccessStatusCode ? JsonConvert.DeserializeObject<AzureInstanceMetadata>(await response.Content.ReadAsStringAsync()) : throw new Exception(string.Format("Error computer metadata. Received status {0}: {1}", (object) response.StatusCode, (object) await response.Content.ReadAsStringAsync()));
      response = (HttpResponseMessage) null;
      return instanceMetadataAsync;
    }

    public TimeZoneInfo GetTimeZone(IVssRequestContext requestContext, HttpClient httpClient) => AzureRegions.GetTimeZone(this.GetInstanceMetadataAsync(httpClient).GetAwaiter().GetResult().Compute.Location);
  }
}
