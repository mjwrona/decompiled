// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.EdgeCacheUrlBuilder
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class EdgeCacheUrlBuilder
  {
    private const string BlobStorageHostSuffix = "blob.core.windows.net";
    private const int DefaultPort = -1;
    private const string BlobStorageEmulatorHost = "localhost";
    private const string BlobStorageEmulatorHost2 = "127.0.0.1";
    private const int BlobStorageEmulatorPort = 10000;
    public static readonly string BlobStorageEmulatorHostPort = string.Format("{0}:{1}", (object) "localhost", (object) 10000);

    public EdgeCacheUrlBuilder(string hostSuffix)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(hostSuffix, nameof (hostSuffix));
      this.EdgeHostSuffix = hostSuffix.Trim().TrimStart('.');
      this.EdgePort = hostSuffix.Equals(EdgeCacheUrlBuilder.BlobStorageEmulatorHostPort, StringComparison.OrdinalIgnoreCase) ? 10000 : -1;
    }

    public string EdgeHostSuffix { get; }

    public int EdgePort { get; }

    public Uri Create(Uri uri)
    {
      ArgumentUtility.CheckForNull<Uri>(uri, nameof (uri));
      UriBuilder uriBuilder = new UriBuilder(uri);
      uriBuilder.Host = this.GetEdgeHost(uriBuilder.Host, uriBuilder.Port);
      uriBuilder.Port = this.EdgePort;
      return uriBuilder.Uri;
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
