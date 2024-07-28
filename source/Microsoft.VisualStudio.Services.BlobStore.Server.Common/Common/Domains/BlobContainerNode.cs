// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.Domains.BlobContainerNode
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using Microsoft.VisualStudio.Services.Content.Server.Azure;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common.Domains
{
  public sealed class BlobContainerNode : IPhysicalNode
  {
    public readonly ICloudBlobContainer Container;

    public BlobContainerNode(ICloudBlobContainer container)
    {
      this.Container = container;
      this.UniqueName = container.StorageUri.PrimaryUri.AbsoluteUri;
    }

    public string UniqueName { get; }

    public bool Equals(IPhysicalNode x, IPhysicalNode y) => x.UniqueName.Equals(y.UniqueName, StringComparison.Ordinal);

    public int GetHashCode(IPhysicalNode obj) => obj.UniqueName.GetHashCode();
  }
}
