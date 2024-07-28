// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.ExtensionQnAItem
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public class ExtensionQnAItem
  {
    public long Id { get; set; }

    public long ParentId { get; set; }

    public string Text { get; set; }

    public Guid ExtensionId { get; set; }

    public Guid UserId { get; set; }

    public bool IsPublisherCreated { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }

    public bool IsQuestion { get; set; }

    public string ExtensionVersion { get; set; }

    public bool IsDeleted { get; set; }
  }
}
