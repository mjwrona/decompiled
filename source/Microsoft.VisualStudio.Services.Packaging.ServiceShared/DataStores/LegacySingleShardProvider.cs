// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataStores.LegacySingleShardProvider
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using System.Collections.Generic;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataStores
{
  public class LegacySingleShardProvider : 
    IPackagingShardProvider<string, IResolvedCloudBlobContainerFactory>
  {
    public LegacySingleShardProvider(
      IResolvedCloudBlobContainerFactory containerFactory)
    {
      // ISSUE: reference to a compiler-generated field
      this.\u003CcontainerFactory\u003EP = containerFactory;
      // ISSUE: explicit constructor call
      base.\u002Ector();
    }

    public IResolvedCloudBlobContainerFactory GetShard(string key) => this.\u003CcontainerFactory\u003EP;

    public IEnumerable<IResolvedCloudBlobContainerFactory> GetAllShards() => (IEnumerable<IResolvedCloudBlobContainerFactory>) new IResolvedCloudBlobContainerFactory[1]
    {
      this.\u003CcontainerFactory\u003EP
    };

    public static LegacySingleShardProvider Bootstrap(IVssRequestContext vssRequestContext)
    {
      CloudBlobContainerFactoryService service = vssRequestContext.To(TeamFoundationHostType.Deployment).Elevate().GetService<CloudBlobContainerFactoryService>();
      return new LegacySingleShardProvider((IResolvedCloudBlobContainerFactory) new CloudBlobContainerFactoryServiceFacade(vssRequestContext, (ICloudBlobContainerFactoryService) service));
    }
  }
}
