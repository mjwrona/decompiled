// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Extension.ProductExtensionsIteratorDriverJobData
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

namespace Microsoft.VisualStudio.Services.Gallery.Server.Extension
{
  public class ProductExtensionsIteratorDriverJobData
  {
    public string RunnerJobNamespace { get; set; }

    public string InstallationTargets { get; set; }

    public int BatchSize { get; set; }

    public int IntervalInMinutes { get; set; }

    public int MaxBatches { get; set; }

    public bool IncludePrivate { get; set; }

    public string ExtensionIds { get; set; }
  }
}
