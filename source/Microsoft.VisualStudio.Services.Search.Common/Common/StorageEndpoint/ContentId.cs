// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.StorageEndpoint.ContentId
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.Search.Common.StorageEndpoint
{
  public class ContentId
  {
    public Hash ObjectId { get; private set; }

    public string ContentKey { get; private set; }

    public string ItemId { get; private set; }

    public ContentId(Hash id) => this.ObjectId = id;

    public ContentId(Hash id, string contentKey)
      : this(id)
    {
      this.ContentKey = contentKey;
    }

    public ContentId(Hash id, string contentKey, string itemId)
      : this(id)
    {
      this.ContentKey = contentKey;
      this.ItemId = itemId;
    }

    public override bool Equals(object obj)
    {
      ContentId contentId = obj as ContentId;
      return obj != null && this.ObjectId.Equals((object) contentId.ObjectId) && string.Equals(this.ContentKey, contentId.ContentKey, StringComparison.Ordinal);
    }

    public override int GetHashCode() => this.ObjectId.GetHashCode();
  }
}
