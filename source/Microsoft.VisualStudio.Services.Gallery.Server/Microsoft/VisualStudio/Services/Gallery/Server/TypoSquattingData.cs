// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.TypoSquattingData
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public class TypoSquattingData
  {
    public IReadOnlyList<string> ExtensionDisplayNames { get; set; } = (IReadOnlyList<string>) new List<string>();

    public IReadOnlyList<string> PublisherDisplayNames { get; set; } = (IReadOnlyList<string>) new List<string>();
  }
}
