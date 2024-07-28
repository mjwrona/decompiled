// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.PublishedExtensionWithState
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Gallery.WebApi;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server
{
  internal class PublishedExtensionWithState
  {
    public PublishedExtension LatestPublishedExtension { get; set; }

    public PublishedExtension PublishedExtension { get; set; }

    public ExtensionState ExtensionState { get; set; }
  }
}
