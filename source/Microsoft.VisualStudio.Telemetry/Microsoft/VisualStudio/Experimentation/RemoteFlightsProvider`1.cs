// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Experimentation.RemoteFlightsProvider`1
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry.Services;
using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Experimentation
{
  internal sealed class RemoteFlightsProvider<T> : CachedRemotePollerFlightsProviderBase<T> where T : IFlightsData
  {
    private const int DefaultPollingIntervalInSecs = 1800000;
    private readonly string flightsKey;
    private readonly Lazy<IRemoteFileReader> remoteFileReader;

    public RemoteFlightsProvider(
      IKeyValueStorage keyValueStorage,
      string flightsKey,
      IRemoteFileReaderFactory remoteFileFactory,
      IFlightsStreamParser flightsStreamParser)
      : base(keyValueStorage, flightsStreamParser, 1800000)
    {
      remoteFileFactory.RequiresArgumentNotNull<IRemoteFileReaderFactory>(nameof (remoteFileFactory));
      flightsKey.RequiresArgumentNotNullAndNotEmpty(nameof (flightsKey));
      this.remoteFileReader = new Lazy<IRemoteFileReader>((Func<IRemoteFileReader>) (() => remoteFileFactory.Instance()));
      this.flightsKey = flightsKey;
    }

    protected override void InternalDispose()
    {
      if (!this.remoteFileReader.IsValueCreated)
        return;
      this.remoteFileReader.Value.Dispose();
    }

    protected override async Task<Stream> SendRemoteRequestInternalAsync() => await this.remoteFileReader.Value.ReadFileAsync().ConfigureAwait(false);

    protected override string BuildFlightsKey() => this.flightsKey;
  }
}
