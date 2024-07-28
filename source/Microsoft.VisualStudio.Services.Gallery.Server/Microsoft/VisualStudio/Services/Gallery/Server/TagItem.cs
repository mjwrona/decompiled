// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.TagItem
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public class TagItem
  {
    public Guid ReferenceId { get; set; }

    public TagType TagType { get; set; }

    public string TagName { get; set; }

    public string Comment { get; set; }

    public int Id { get; set; }
  }
}
