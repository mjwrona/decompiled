// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataStores.ShardedCloudBlobContainerFactory
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Content.Server.Azure;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataStores
{
  internal class ShardedCloudBlobContainerFactory : 
    IResolvedCloudBlobContainerFactory,
    IFactory<
    #nullable disable
    ContainerAddress, IResolvedCloudBlobContainer>
  {
    public ShardedCloudBlobContainerFactory(
    #nullable enable
    IAzureBlobContainerFactory blobClient)
    {
      // ISSUE: reference to a compiler-generated field
      this.\u003CblobClient\u003EP = blobClient;
      // ISSUE: explicit constructor call
      base.\u002Ector();
    }

    public IResolvedCloudBlobContainer Get(ContainerAddress address)
    {
      // ISSUE: reference to a compiler-generated field
      ICloudBlobContainer containerReference = this.\u003CblobClient\u003EP.CreateContainerReference(BlobContainerUtils.ContainerAddressToName(address), false);
      return (IResolvedCloudBlobContainer) new ResolvedCloudBlobContainer(containerReference, BlobContainerUtils.ContainerNameToAddress(containerReference.Name));
    }
  }
}
