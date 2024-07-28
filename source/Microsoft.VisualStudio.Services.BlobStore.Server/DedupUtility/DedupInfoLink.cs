// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.DedupUtility.DedupInfoLink
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.DedupUtility
{
  public class DedupInfoLink
  {
    public static readonly DedupInfoLink Null = new DedupInfoLink((IDedupInfo) null, (IDedupInfo) null, (ICollection<IDedupInfo>) null);

    public IDedupInfo Parent { get; }

    public IDedupInfo Child { get; }

    public ICollection<IDedupInfo> Ancestors { get; }

    private DedupInfoLink(IDedupInfo parent, IDedupInfo child, ICollection<IDedupInfo> ancestors)
    {
      this.Parent = parent;
      this.Child = child;
      this.Ancestors = ancestors;
    }

    public DedupLink ToDedupLink() => DedupLink.Create(this.Parent?.Identifier, this.Child.Identifier);

    public static DedupInfoLink Create(
      IDedupInfo parent,
      IDedupInfo child,
      ICollection<IDedupInfo> ancestors = null)
    {
      return new DedupInfoLink(parent, child, ancestors);
    }
  }
}
