// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata.VersionSearchFilter
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata
{
  public class VersionSearchFilter
  {
    public VersionSearchFilter()
    {
    }

    public VersionSearchFilter(Guid? viewId, bool? isUpstreamCached = null)
    {
      this.ViewId = viewId;
      this.IsUpstreamCached = isUpstreamCached;
    }

    public VersionSearchFilter(VersionSearchFilter other)
      : this(other.ViewId, other.IsUpstreamCached)
    {
    }

    public Guid? ViewId { get; set; }

    public bool? IsUpstreamCached { get; set; }
  }
}
