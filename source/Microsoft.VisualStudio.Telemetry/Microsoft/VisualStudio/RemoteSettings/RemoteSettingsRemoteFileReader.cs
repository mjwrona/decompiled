// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteSettings.RemoteSettingsRemoteFileReader
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.RemoteControl;
using Microsoft.VisualStudio.Telemetry;
using Microsoft.VisualStudio.Telemetry.Services;
using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.RemoteSettings
{
  internal sealed class RemoteSettingsRemoteFileReader : 
    TelemetryDisposableObject,
    IRemoteFileReader,
    IDisposable
  {
    private readonly IRemoteControlClient remoteControlClient;

    public RemoteSettingsRemoteFileReader(IRemoteControlClient remoteControlClient)
    {
      remoteControlClient.RequiresArgumentNotNull<IRemoteControlClient>(nameof (remoteControlClient));
      this.remoteControlClient = remoteControlClient;
    }

    public async Task<Stream> ReadFileAsync()
    {
      RemoteSettingsRemoteFileReader remoteFileReader = this;
      remoteFileReader.RequiresNotDisposed();
      return await remoteFileReader.remoteControlClient.ReadFileAsync(BehaviorOnStale.ForceDownload).ConfigureAwait(false);
    }

    protected override void DisposeManagedResources() => this.remoteControlClient.Dispose();
  }
}
