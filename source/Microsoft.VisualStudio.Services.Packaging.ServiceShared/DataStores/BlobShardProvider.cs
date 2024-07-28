// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataStores.BlobShardProvider
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Server.Azure;
using Microsoft.VisualStudio.Services.Content.Server.Azure.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Utils;
using System;
using System.Collections.Generic;
using System.Linq;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataStores
{
  internal class BlobShardProvider : 
    IPackagingShardProvider<string, IResolvedCloudBlobContainerFactory>
  {
    public BlobShardProvider(
      IShardManager<AzureBlobPhysicalNode> shardManager,
      IHasher<ulong> hasher)
    {
      // ISSUE: reference to a compiler-generated field
      this.\u003CshardManager\u003EP = shardManager;
      // ISSUE: reference to a compiler-generated field
      this.\u003Chasher\u003EP = hasher;
      // ISSUE: explicit constructor call
      base.\u002Ector();
    }

    public IResolvedCloudBlobContainerFactory GetShard(string blobName)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      using (VirtualNodeContext<AzureBlobPhysicalNode> node = this.\u003CshardManager\u003EP.FindNode(this.\u003Chasher\u003EP.Hash(blobName)))
        return node.VirtualNode.PhysicalNode.ContainerFactory;
    }

    public IEnumerable<IResolvedCloudBlobContainerFactory> GetAllShards() => this.\u003CshardManager\u003EP.VirtualNodes.Select<VirtualNode<AzureBlobPhysicalNode>, IResolvedCloudBlobContainerFactory>((Func<VirtualNode<AzureBlobPhysicalNode>, IResolvedCloudBlobContainerFactory>) (x => x.PhysicalNode.ContainerFactory)).Distinct<IResolvedCloudBlobContainerFactory>();

    public static IPackagingShardProvider<string, IResolvedCloudBlobContainerFactory> Bootstrap(
      IVssRequestContext requestContext)
    {
      IVssRequestContext context = requestContext.To(TeamFoundationHostType.Deployment);
      IVssRequestContext requestContext1 = context.Elevate();
      IAzureCloudBlobClientProvider clientProviderService = context.GetService<IAzureCloudBlobClientProvider>();
      return (IPackagingShardProvider<string, IResolvedCloudBlobContainerFactory>) new BlobShardProvider((IShardManager<AzureBlobPhysicalNode>) new ConsistentHashShardManager<AzureBlobPhysicalNode>(context.GetService<IStorageAccountConfigurationService>().GetStorageAccounts(requestContext1).Select<StrongBoxConnectionString, AzureBlobPhysicalNode>(new Func<StrongBoxConnectionString, AzureBlobPhysicalNode>(CreatePhysicalNode)), 128), (IHasher<ulong>) FNVHasher64.Instance);

      AzureBlobPhysicalNode CreatePhysicalNode(StrongBoxConnectionString connectionString)
      {
        ICloudBlobClient blobClient = clientProviderService.GetBlobClient(connectionString.StrongBoxItemKey, CodeOnlyDeploymentsConstants.CodeOnlyContainerPrefix);
        return new AzureBlobPhysicalNode(StorageAccountUtilities.GetAccountInfo(connectionString.ConnectionString).Name, (IResolvedCloudBlobContainerFactory) new ShardedCloudBlobContainerFactory((IAzureBlobContainerFactory) blobClient));
      }
    }
  }
}
