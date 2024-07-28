// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.StorageReadAccessSecondaryRegionHelper
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  public static class StorageReadAccessSecondaryRegionHelper
  {
    private const string blobStorageHostName = ".blob.core.windows.net";
    private const string secondaryEndpointSuffix = "-secondary";
    private const int secondaryProbeTimeoutInSeconds = 10;

    public static async Task<Uri> ProcessRetries(
      Uri storageAccountUri,
      HttpClient client,
      CancellationToken cancellationToken)
    {
      (string storageAccountName, string storageDomainName) parsedAzureBlobUri;
      if (StorageReadAccessSecondaryRegionHelper.TryParseAzureBlobUri(storageAccountUri.Host, out parsedAzureBlobUri))
      {
        string secondaryStorageAccount = StorageReadAccessSecondaryRegionHelper.GetReadAccessSecondaryStorageAccount(parsedAzureBlobUri.storageAccountName);
        UriBuilder uriBuilder = new UriBuilder(storageAccountUri)
        {
          Host = secondaryStorageAccount + parsedAzureBlobUri.storageDomainName
        };
        try
        {
          if (await StorageReadAccessSecondaryRegionHelper.ProbeReadAvailableSecondaryAvailability(uriBuilder.Uri, client, cancellationToken).ConfigureAwait(false))
            storageAccountUri = uriBuilder.Uri;
        }
        catch (Exception ex)
        {
        }
        uriBuilder = (UriBuilder) null;
      }
      return storageAccountUri;
    }

    private static async Task<bool> ProbeReadAvailableSecondaryAvailability(
      Uri secondaryStorageUri,
      HttpClient client,
      CancellationToken cancellationToken)
    {
      try
      {
        using (CancellationTokenSource combinedSource = CancellationTokenSource.CreateLinkedTokenSource(new CancellationTokenSource(TimeSpan.FromSeconds(10.0)).Token, cancellationToken))
        {
          using (HttpResponseMessage httpResponseMessage = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, secondaryStorageUri), cancellationToken).EnforceCancellation<HttpResponseMessage>(combinedSource.Token, (Func<string>) (() => string.Format("Timed out waiting for response for {0}.", (object) secondaryStorageUri)), "D:\\a\\_work\\1\\s\\ArtifactServices\\Shared\\BlobStore.Common\\StorageClientFallBackHelper.cs", nameof (ProbeReadAvailableSecondaryAvailability), 82).ConfigureAwait(false))
            return httpResponseMessage.IsSuccessStatusCode;
        }
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    private static bool TryParseAzureBlobUri(
      string host,
      out (string storageAccountName, string storageDomainName) parsedAzureBlobUri)
    {
      if (host.EndsWith(".blob.core.windows.net", StringComparison.OrdinalIgnoreCase))
      {
        string str = host.Substring(0, host.Length - ".blob.core.windows.net".Length);
        parsedAzureBlobUri = (str, ".blob.core.windows.net");
        return true;
      }
      parsedAzureBlobUri = ();
      return false;
    }

    private static string GetReadAccessSecondaryStorageAccount(string storageAccountName) => !storageAccountName.EndsWith("-secondary") ? storageAccountName + "-secondary" : storageAccountName.Substring(0, storageAccountName.Length - "-secondary".Length);
  }
}
