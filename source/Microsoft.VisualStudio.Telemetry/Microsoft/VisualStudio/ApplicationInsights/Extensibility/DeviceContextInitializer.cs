// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Extensibility.DeviceContextInitializer
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.ApplicationInsights.DataContracts;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.ApplicationInsights.Extensibility
{
  public class DeviceContextInitializer : IContextInitializer
  {
    public void Initialize(TelemetryContext context)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      IDeviceContextReader instance = DeviceContextReader.Instance;
      context.Device.Type = instance.GetDeviceType();
      context.Device.Id = instance.GetDeviceUniqueId();
      instance.GetOperatingSystemAsync().ContinueWith((Action<Task<string>>) (task =>
      {
        if (!task.IsCompleted)
          return;
        context.Device.OperatingSystem = task.Result;
      }), TaskScheduler.Default);
      context.Device.OemName = instance.GetOemName();
      context.Device.Model = instance.GetDeviceModel();
      context.Device.NetworkType = instance.GetNetworkType().ToString((IFormatProvider) CultureInfo.InvariantCulture);
      instance.GetScreenResolutionAsync().ContinueWith((Action<Task<string>>) (task =>
      {
        if (!task.IsCompleted)
          return;
        context.Device.ScreenResolution = task.Result;
      }), TaskScheduler.Default);
      context.Device.Language = instance.GetHostSystemLocale();
    }
  }
}
