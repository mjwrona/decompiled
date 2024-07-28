// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Extension.BackConsolidationMappingEntry
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Extension
{
  public class BackConsolidationMappingEntry
  {
    public Guid SourceExtensionId { get; set; }

    public string SourceExtensionVsixId { get; set; }

    public Guid TargetExtensionId { get; set; }

    public string TargetExtensionVsixId { get; set; }
  }
}
