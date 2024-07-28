// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.AADUtilImproved
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Retry;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Aad
{
  public class AADUtilImproved : IAADUtil
  {
    public static readonly IAADUtil Instance = (IAADUtil) new AADUtilImproved();
    public const int MaxAttempts = 3;
    private const string DefaultAadAuthority = "login.microsoftonline.com";
    private const string OpenIdMetadataEndpointPath = ".well-known/openid-configuration";
    private const string UserAgentValue = "AzureDevOps";
    private const int TooManyRequests = 429;
    private const int DefaultRetryDelay = 10;

    private Uri FormatMetadataUri(string tenantId, string authority, string endpointPath) => new Uri(string.Format("https://{0}/{1}/{2}", (object) authority, (object) tenantId, (object) endpointPath));

    public Task<IAADTenantMetadata> AcquireAADOpenIdConfigMetadata(
      string tenantId,
      string authority = null,
      CancellationToken cancellationToken = default (CancellationToken),
      ITFLogger logger = null)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(tenantId, nameof (tenantId));
      if (string.IsNullOrEmpty(authority))
        authority = "login.microsoftonline.com";
      return this.AcquireOpenIdConfigMetadata(this.FormatMetadataUri(tenantId, authority, ".well-known/openid-configuration"), (IEnumerable<string>) null, cancellationToken, logger);
    }

    public async Task<IAADTenantMetadata> AcquireOpenIdConfigMetadata(
      Uri metadataEndpoint,
      IEnumerable<string> validEndorsements = null,
      CancellationToken cancellationToken = default (CancellationToken),
      ITFLogger logger = null)
    {
      if (logger == null)
        logger = (ITFLogger) new NullLogger();
      JObject metadata = (JObject) null;
      try
      {
        metadata = await AADUtilImproved.GetJsonObject(await this.SendRetriableHttpGetRequest(metadataEndpoint, logger, cancellationToken).ConfigureAwait(false), logger).ConfigureAwait(false);
      }
      catch (Exception ex)
      {
        AADUtilImproved.LogMessage("Failed to get Ope metadata:\nMetadata endpoint: " + metadataEndpoint.AbsoluteUri + string.Format("\nException: {0}", (object) ex), logger);
      }
      if (metadata == null)
      {
        logger?.Info(string.Format("metadata from {0} is empty", (object) metadataEndpoint));
        return (IAADTenantMetadata) null;
      }
      JObject keydata = (JObject) null;
      string uriString = (string) metadata["jwks_uri"];
      if (!string.IsNullOrEmpty(uriString))
      {
        Uri keyDataUri = new Uri(uriString);
        try
        {
          keydata = await AADUtilImproved.GetJsonObject(await this.SendRetriableHttpGetRequest(keyDataUri, logger, cancellationToken).ConfigureAwait(false), logger).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
          AADUtilImproved.LogMessage("Failed to get OpenId Configuration key data:\nKeydata endpoint: " + keyDataUri.AbsoluteUri + string.Format("\nException: {0}", (object) ex), logger);
        }
        keyDataUri = (Uri) null;
      }
      else
        AADUtilImproved.LogMessage("OpenId configuration did not contain required element jwks_uri. Cannot acquire key data.", logger);
      IAADTenantMetadata aadTenantMetadata = (IAADTenantMetadata) null;
      if (keydata != null)
        aadTenantMetadata = AADTenantMetadata.FromOpenIdResponse(metadata, keydata, validEndorsements);
      else
        logger?.Info("keydata is empty");
      return aadTenantMetadata;
    }

    private static async Task<JObject> GetJsonObject(HttpResponseMessage response, ITFLogger logger)
    {
      string message = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
      JObject jsonObject = JsonConvert.DeserializeObject<JObject>(message);
      if (jsonObject == null)
      {
        logger?.Info(response.Content.Serialize<HttpContent>());
        logger?.Info(message);
      }
      return jsonObject;
    }

    private static void LogMessage(string message, ITFLogger logger)
    {
      logger?.Info(message);
      TeamFoundationEventLog.Default.Log(message, TeamFoundationEventId.AadFederationMetadata, EventLogEntryType.Warning);
    }

    private async Task<HttpResponseMessage> SendRetriableHttpGetRequest(
      Uri dataEndpoint,
      ITFLogger logger,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpResponseMessage response = (HttpResponseMessage) null;
      for (int attempt = 1; attempt <= 3; ++attempt)
      {
        using (HttpClient client = HttpClientFactory.Create(this.CreateMessageHandler()))
        {
          client.DefaultRequestHeaders.Add("User-Agent", "AzureDevOps");
          response = await client.GetAsync(dataEndpoint).ConfigureAwait(false);
          if (!response.IsSuccessStatusCode)
          {
            int int32 = Convert.ToInt32((object) response.StatusCode);
            if (int32 != 429 || attempt >= 3)
              throw new HttpRequestException(string.Format("Response status code does not indicate success: {0} ({1}).", (object) int32, (object) response.ReasonPhrase));
            TimeSpan delay = response.Headers.RetryAfter.Delta ?? TimeSpan.FromSeconds(10.0);
            AADUtilImproved.LogMessage(string.Format("Attemtp: {0}, HttpStatusCode: {1}, RetryAfter: {2} seconds", (object) attempt, (object) int32, (object) delay.Seconds), logger);
            await Task.Delay(delay, cancellationToken);
          }
          else
            break;
        }
      }
      HttpResponseMessage request = response;
      response = (HttpResponseMessage) null;
      return request;
    }

    protected virtual HttpMessageHandler CreateMessageHandler() => (HttpMessageHandler) new AadVssHttpRetryMessageHandler(new VssHttpRetryOptions(), (HttpMessageHandler) new HttpClientHandler());
  }
}
