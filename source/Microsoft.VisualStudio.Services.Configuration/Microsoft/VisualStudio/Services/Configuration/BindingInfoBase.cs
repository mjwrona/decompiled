// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.BindingInfoBase
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public abstract class BindingInfoBase
  {
    public byte[] AppId { get; set; }

    public Guid AppGuid => this.AppId == null || this.AppId.Length == 0 ? Guid.Empty : new Guid(this.AppId);

    public byte[] SslCertHash { get; set; }

    public string SslCertHashHex => this.SslCertHash == null || this.SslCertHash.Length == 0 ? "" : HexConverter.ToString(this.SslCertHash);

    public string SslCertStoreName { get; set; }
  }
}
