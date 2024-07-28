// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Extension.SyncMessageEnqueuerJobData
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

namespace Microsoft.VisualStudio.Services.Gallery.Server.Extension
{
  public class SyncMessageEnqueuerJobData
  {
    public string PublisherName { get; set; }

    public string ExtensionName { get; set; }

    public string ConflictName { get; set; }

    public string Version { get; set; }

    public string TargetPlatform { get; set; }

    public OperationCode OperationCode { get; set; }
  }
}
