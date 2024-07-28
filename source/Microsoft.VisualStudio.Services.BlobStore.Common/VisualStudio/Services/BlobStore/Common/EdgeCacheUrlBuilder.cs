// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.EdgeCacheUrlBuilder
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  public class EdgeCacheUrlBuilder
  {
    private const string BlobStorageHostSuffix = "blob.core.windows.net";
    private const int DefaultPort = -1;
    private const string BlobStorageEmulatorHost = "localhost";
    private const string BlobStorageEmulatorHost2 = "127.0.0.1";
    private const int BlobStorageEmulatorPort = 10000;
    public static readonly string BlobStorageEmulatorHostPort = string.Format("{0}:{1}", (object) "localhost", (object) 10000);
    private readonly IUrlSigner urlSigner;

    public EdgeCacheUrlBuilder(string hostSuffix, IUrlSigner urlSigner = null)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(hostSuffix, nameof (hostSuffix));
      this.EdgeHostSuffix = hostSuffix.Trim().TrimStart('.');
      this.EdgePort = hostSuffix.Equals(EdgeCacheUrlBuilder.BlobStorageEmulatorHostPort, StringComparison.OrdinalIgnoreCase) ? 10000 : -1;
      this.urlSigner = urlSigner;
    }

    public string EdgeHostSuffix { get; }

    public int EdgePort { get; }

    public Uri Create(Uri uri, DateTime expiryUtc)
    {
      ArgumentUtility.CheckForNull<Uri>(uri, nameof (uri));
      if (expiryUtc.Kind != DateTimeKind.Utc)
        throw new ArgumentException("Must be a UTC date.", nameof (expiryUtc));
      UriBuilder uriBuilder = new UriBuilder(uri);
      uriBuilder.Host = this.GetEdgeHost(uriBuilder.Host, uriBuilder.Port);
      uriBuilder.Port = this.EdgePort;
      Uri uri1 = uriBuilder.Uri;
      if (this.urlSigner != null)
        uri1 = this.urlSigner.Sign(uri1, expiryUtc);
      return uri1;
    }

    private string GetEdgeHost(string host, int port)
    {
      ArgumentUtility.CheckForNull<string>(host, nameof (host));
      if (host.EndsWith("blob.core.windows.net"))
        return host.Substring(0, host.Length - "blob.core.windows.net".Length) + this.EdgeHostSuffix;
      if (!host.Equals("localhost") && !host.Equals("127.0.0.1") || port != 10000)
        throw new ArgumentException(string.Format("Expected that we're only modifying blob storage urls. Encountered host: '{0}', port: {1}", (object) host, (object) port));
      if (this.EdgeHostSuffix != EdgeCacheUrlBuilder.BlobStorageEmulatorHostPort)
        throw new InvalidOperationException("Cannot fake edge cache with storage emulator unless EdgeHostSuffix is set to '" + EdgeCacheUrlBuilder.BlobStorageEmulatorHostPort + "'. Use the LightRail setting 'EdgeServicesBlobCacheHostSuffix'.");
      return "localhost";
    }
  }
}
