// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.PropertyProviders.Linux.LinuxOSPropertyProvider
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System.Collections.Generic;
using System.Threading;

namespace Microsoft.VisualStudio.Telemetry.PropertyProviders.Linux
{
  internal class LinuxOSPropertyProvider : IPropertyProvider
  {
    public void AddSharedProperties(
      List<KeyValuePair<string, object>> sharedProperties,
      TelemetryContext telemetryContext)
    {
    }

    public void PostProperties(TelemetryContext telemetryContext, CancellationToken token)
    {
      telemetryContext.PostProperty("VS.Core.OS.ProductName", (object) this.GetOSProductName());
      int num = token.IsCancellationRequested ? 1 : 0;
    }

    private string GetOSProductName() => "linux";
  }
}
