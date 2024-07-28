// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Experimentation.FlightsRemoteFileReaderFactoryBase
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.RemoteControl;
using Microsoft.VisualStudio.Telemetry.Services;
using Microsoft.VisualStudio.Utilities.Internal;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Experimentation
{
  [ExcludeFromCodeCoverage]
  internal class FlightsRemoteFileReaderFactoryBase : IRemoteFileReaderFactory
  {
    private const int DownloadIntervalInMin = 60;
    private const string DefaultBaseUrl = "https://az700632.vo.msecnd.net/pub";
    private const string DefaultHostId = "FlightsData";
    private readonly string configPath;

    public FlightsRemoteFileReaderFactoryBase(string configPath)
    {
      configPath.RequiresArgumentNotNullAndNotWhiteSpace(nameof (configPath));
      this.configPath = configPath;
    }

    public IRemoteFileReader Instance() => (IRemoteFileReader) new FlightsRemoteFileReader((IRemoteControlClient) new RemoteControlClient("FlightsData", "https://az700632.vo.msecnd.net/pub", this.configPath, 60));
  }
}
