// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.SslBindingInfo
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public class SslBindingInfo : BindingInfoBase
  {
    public string IPAddress { get; set; }

    public int Port { get; set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}, ApppId:{2:B}, CertHash:{3}, CertStoreName:{4}", (object) this.IPAddress, (object) this.Port, (object) this.AppGuid, (object) this.SslCertHashHex, (object) this.SslCertStoreName);
  }
}
