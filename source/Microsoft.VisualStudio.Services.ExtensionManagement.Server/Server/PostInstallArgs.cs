// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.PostInstallArgs
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server
{
  internal class PostInstallArgs
  {
    public PublishedExtension PublishedExtension { get; set; }

    public string Version { get; set; }

    public Guid ActivityId { get; set; }
  }
}
