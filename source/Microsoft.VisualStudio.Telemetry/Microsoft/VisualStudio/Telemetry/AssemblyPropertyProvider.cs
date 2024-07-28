// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.AssemblyPropertyProvider
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Microsoft.VisualStudio.Telemetry
{
  internal sealed class AssemblyPropertyProvider : IPropertyProvider
  {
    public void AddSharedProperties(
      List<KeyValuePair<string, object>> sharedProperties,
      TelemetryContext telemetryContext)
    {
      try
      {
        FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(this.GetType().Assembly.Location);
        sharedProperties.Add(new KeyValuePair<string, object>("VS.Core.TelemetryApi.ProductVersion", (object) versionInfo.ProductVersion));
      }
      catch (FileNotFoundException ex)
      {
      }
    }

    public void PostProperties(TelemetryContext telemetryContext, CancellationToken token)
    {
    }
  }
}
