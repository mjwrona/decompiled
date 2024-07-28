// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.CDNPathUtil
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using System.Globalization;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  internal class CDNPathUtil
  {
    public string PublisherName { get; set; }

    public string ExtensionName { get; set; }

    public string ExtensionVersion { get; set; }

    public string AssetRoot { get; set; }

    public string GetExtensionAssetUploadPath() => string.Format("{0}/{1}/{2}/{3}", (object) this.PublisherName.ToLower(CultureInfo.InvariantCulture), (object) this.ExtensionName.ToLower(CultureInfo.InvariantCulture), (object) this.ExtensionVersion.ToLower(CultureInfo.InvariantCulture), (object) this.AssetRoot);

    public string GetPublisherAssetUploadPath() => string.Format("{0}", (object) this.PublisherName.ToLower(CultureInfo.InvariantCulture));
  }
}
