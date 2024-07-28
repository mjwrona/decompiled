// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteSettings.RemoteSettingsRemoteFileReaderFactory
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.RemoteControl;
using Microsoft.VisualStudio.Telemetry.Services;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.RemoteSettings
{
  [ExcludeFromCodeCoverage]
  internal sealed class RemoteSettingsRemoteFileReaderFactory : IRemoteFileReaderFactory
  {
    private static readonly TimeSpan DownloadInterval = TimeSpan.FromMinutes(60.0);
    private const string DefaultBaseUrl = "https://az700632.vo.msecnd.net/pub";
    private const string DefaultHostId = "RemoteSettings";
    private const string DefaultPath = "RemoteSettings.json";
    private string fileNameOverride;

    public RemoteSettingsRemoteFileReaderFactory(string fileNameOverride) => this.fileNameOverride = fileNameOverride;

    public IRemoteFileReader Instance() => (IRemoteFileReader) new RemoteSettingsRemoteFileReader((IRemoteControlClient) new RemoteControlClient("RemoteSettings", "https://az700632.vo.msecnd.net/pub", this.fileNameOverride ?? "RemoteSettings.json", (int) RemoteSettingsRemoteFileReaderFactory.DownloadInterval.TotalMinutes));
  }
}
