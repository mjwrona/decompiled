// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.UploadExtensionAssetsToCdnJobData
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public class UploadExtensionAssetsToCdnJobData
  {
    public string InstallationTargets { get; set; }

    public string PublisherName { get; set; }

    public string ExtensionName { get; set; }

    public string Version { get; set; }

    public string TargetPlatform { get; set; }

    public bool NoCompression { get; set; }

    public bool ShouldConvertSvgToPng { get; set; }
  }
}
