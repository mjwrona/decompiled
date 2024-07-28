// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.BlockInfo
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using Microsoft.VisualStudio.Services.BlobStore.Common;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common
{
  public sealed class BlockInfo
  {
    public BlockInfo(BlobBlockHash blockHash) => this.Name = BlockInfo.ConstructBlockName(blockHash);

    public BlockInfo(string blockName)
    {
      this.Name = blockName;
      this.GetBlockHash();
    }

    public bool Committed { get; set; }

    public long Length { get; set; }

    public string Name { get; private set; }

    public static string ConstructBlockName(BlobBlockHash blockHash) => blockHash.HashString;

    public BlobBlockHash GetBlockHash() => new BlobBlockHash(this.Name);

    public override string ToString() => this.Name;
  }
}
