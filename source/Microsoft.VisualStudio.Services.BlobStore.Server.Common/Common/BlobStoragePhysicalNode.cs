// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.BlobStoragePhysicalNode
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using Microsoft.VisualStudio.Services.Content.Server.Common;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common
{
  public class BlobStoragePhysicalNode : IPhysicalNode
  {
    public readonly IBlobProvider BlobProvider;

    public BlobStoragePhysicalNode(IBlobProvider blobProvider, string uniqueName)
    {
      this.BlobProvider = blobProvider;
      this.UniqueName = uniqueName;
    }

    public string UniqueName { get; private set; }

    public bool Equals(IPhysicalNode x, IPhysicalNode y) => x.UniqueName.Equals(y.UniqueName);

    public int GetHashCode(IPhysicalNode obj) => this.UniqueName.GetHashCode();
  }
}
