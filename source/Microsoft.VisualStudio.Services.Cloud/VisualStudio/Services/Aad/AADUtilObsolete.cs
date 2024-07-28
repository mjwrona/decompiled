// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.AADUtilObsolete
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Aad
{
  public class AADUtilObsolete : IAADUtil
  {
    public static readonly IAADUtil Instance = (IAADUtil) new AADUtilObsolete();
    private const string DefaultAadAuthority = "login.microsoftonline.com";
    private const string OpenIdMetadataEndpointPath = ".well-known/openid-configuration";

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
      int maxRetries = 3;
      IAADTenantMetadata result = (IAADTenantMetadata) null;
      JObject metadata = (JObject) null;
      JObject keydata = (JObject) null;
      int index;
      HttpClient client;
      HttpResponseMessage message;
      ITFLogger tfLogger;
      for (index = 1; index <= maxRetries; ++index)
      {
        try
        {
          client = HttpClientFactory.Create(this.CreateMessageHandler());
          try
          {
            message = await client.GetAsync(metadataEndpoint).ConfigureAwait(false);
            message.EnsureSuccessStatusCode();
            metadata = JsonConvert.DeserializeObject<JObject>(await message.Content.ReadAsStringAsync().ConfigureAwait(false));
            if (metadata == null)
            {
              (await message.Content.ReadAsStreamAsync()).Seek(0L, SeekOrigin.Begin);
              logger?.Info(message.Content.Serialize<HttpContent>());
              tfLogger = logger;
              tfLogger?.Info(await message.Content.ReadAsStringAsync().ConfigureAwait(false));
              tfLogger = (ITFLogger) null;
              break;
            }
            break;
          }
          finally
          {
            client?.Dispose();
          }
        }
        catch (Exception ex)
        {
          string message1 = string.Format("Failed to get Ope metadata:\nMetadata endpoint: {0}\nException: {1}", (object) metadataEndpoint.AbsoluteUri, (object) ex.Message);
          logger?.Info(message1);
          if (index == maxRetries)
            TeamFoundationEventLog.Default.Log(message1, TeamFoundationEventId.AadFederationMetadata, EventLogEntryType.Warning);
        }
      }
      if (metadata == null)
        logger?.Info(string.Format("metadata from {0} is empty", (object) metadataEndpoint));
      if (metadata != null)
      {
        string uriString = (string) metadata["jwks_uri"];
        if (!string.IsNullOrEmpty(uriString))
        {
          Uri keyDataUri = new Uri(uriString);
          for (index = 1; index <= maxRetries; ++index)
          {
            try
            {
              client = HttpClientFactory.Create(this.CreateMessageHandler());
              try
              {
                message = await client.GetAsync(keyDataUri).ConfigureAwait(false);
                message.EnsureSuccessStatusCode();
                keydata = JsonConvert.DeserializeObject<JObject>(await message.Content.ReadAsStringAsync().ConfigureAwait(false));
                if (keydata == null)
                {
                  (await message.Content.ReadAsStreamAsync()).Seek(0L, SeekOrigin.Begin);
                  logger?.Info(message.Content.Serialize<HttpContent>());
                  tfLogger = logger;
                  tfLogger?.Info(await message.Content.ReadAsStringAsync().ConfigureAwait(false));
                  tfLogger = (ITFLogger) null;
                  break;
                }
                break;
              }
              finally
              {
                client?.Dispose();
              }
            }
            catch (Exception ex)
            {
              string message2 = string.Format("Failed to get OpenId Configuration key data:\nKeydata endpoint: {0}\nException: {1}", (object) keyDataUri.AbsoluteUri, (object) ex.Message);
              logger?.Info(message2);
              if (index == maxRetries)
                TeamFoundationEventLog.Default.Log(message2, TeamFoundationEventId.AadFederationMetadata, EventLogEntryType.Warning);
            }
          }
          keyDataUri = (Uri) null;
        }
        else
        {
          string message3 = string.Format("OpenId configuration did not contain required element jwks_uri. Cannot acquire key data.");
          logger?.Info(message3);
          TeamFoundationEventLog.Default.Log(message3, TeamFoundationEventId.AadFederationMetadata, EventLogEntryType.Warning);
        }
      }
      if (keydata == null)
        logger?.Info("keydata is empty");
      if (metadata != null && keydata != null)
        result = AADTenantMetadata.FromOpenIdResponse(metadata, keydata, validEndorsements);
      IAADTenantMetadata aadTenantMetadata = result;
      result = (IAADTenantMetadata) null;
      metadata = (JObject) null;
      keydata = (JObject) null;
      return aadTenantMetadata;
    }

    private HttpMessageHandler CreateMessageHandler() => (HttpMessageHandler) new VssHttpRetryMessageHandler(new VssHttpRetryOptions(), (HttpMessageHandler) new HttpClientHandler());
  }
}
