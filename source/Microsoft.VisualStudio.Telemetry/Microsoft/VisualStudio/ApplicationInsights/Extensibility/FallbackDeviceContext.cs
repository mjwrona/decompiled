// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Extensibility.FallbackDeviceContext
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Xml.Linq;

namespace Microsoft.VisualStudio.ApplicationInsights.Extensibility
{
  internal class FallbackDeviceContext : IFallbackContext
  {
    public string DeviceUniqueId { get; private set; }

    public void Initialize()
    {
      byte[] numArray = new byte[20];
      new Random().NextBytes(numArray);
      this.DeviceUniqueId = Convert.ToBase64String(numArray);
    }

    public void Serialize(XElement rootElement) => rootElement.Add((object) new XElement(XName.Get("DeviceUniqueId"), (object) this.DeviceUniqueId));

    public bool Deserialize(XElement rootElement)
    {
      if (rootElement == null)
        return false;
      XElement xelement = rootElement.Element(XName.Get("DeviceUniqueId"));
      if (xelement == null)
        return false;
      this.DeviceUniqueId = xelement.Value;
      return !this.DeviceUniqueId.IsNullOrWhiteSpace();
    }
  }
}
