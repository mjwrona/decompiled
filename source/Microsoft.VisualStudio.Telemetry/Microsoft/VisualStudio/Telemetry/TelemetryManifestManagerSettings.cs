// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetryManifestManagerSettings
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class TelemetryManifestManagerSettings : ITelemetryManifestManagerSettings
  {
    private readonly string urlFilePath;
    private readonly string urlFilePattern = "v{0}/dyntelconfig.json";

    public string BaseUrl => "https://az667904.vo.msecnd.net/pub";

    public string HostId { get; }

    public string RelativePath => this.urlFilePath;

    public TelemetryManifestManagerSettings(string hostName, string theUrlFilePattern = null)
    {
      if (theUrlFilePattern != null)
        this.urlFilePattern = theUrlFilePattern;
      this.HostId = hostName;
      this.urlFilePath = string.Format((IFormatProvider) CultureInfo.InvariantCulture, this.urlFilePattern, new object[1]
      {
        (object) 2U
      });
    }
  }
}
