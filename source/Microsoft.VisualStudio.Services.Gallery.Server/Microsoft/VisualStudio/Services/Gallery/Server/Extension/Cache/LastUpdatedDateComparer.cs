// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Extension.Cache.LastUpdatedDateComparer
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Extension.Cache
{
  internal class LastUpdatedDateComparer : Comparer<PublishedExtension>
  {
    private readonly SortOrderType sortOrderType;

    public LastUpdatedDateComparer(SortOrderType sortOrderType) => this.sortOrderType = sortOrderType;

    public override int Compare(PublishedExtension extension1, PublishedExtension extension2) => this.sortOrderType != SortOrderType.Ascending ? extension2.LastUpdated.CompareTo(extension1.LastUpdated) : extension1.LastUpdated.CompareTo(extension2.LastUpdated);
  }
}
