// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamMetadataCacheInfoStoreFactoryBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public class UpstreamMetadataCacheInfoStoreFactoryBootstrapper : 
    IBootstrapper<IFactory<IFeedRequest, IUpstreamMetadataCacheInfoStore>>
  {
    private readonly IVssRequestContext requestContext;
    private readonly IConverter<string, IPackageName> stringToPackageNameConverter;

    public UpstreamMetadataCacheInfoStoreFactoryBootstrapper(
      IVssRequestContext requestContext,
      IConverter<string, IPackageName> stringToPackageNameConverter)
    {
      this.requestContext = requestContext ?? throw new ArgumentNullException(nameof (requestContext));
      this.stringToPackageNameConverter = stringToPackageNameConverter;
      requestContext.CheckProjectCollectionRequestContext();
    }

    public IFactory<IFeedRequest, IUpstreamMetadataCacheInfoStore> Bootstrap()
    {
      IFactory<ContainerAddress, IBlobService> blobServiceFactory = BlobServiceFactoryBootstrapper.CreateLegacyUnsharded(this.requestContext).Bootstrap();
      return (IFactory<IFeedRequest, IUpstreamMetadataCacheInfoStore>) new ByFuncInputFactory<IFeedRequest, IUpstreamMetadataCacheInfoStore>((Func<IFeedRequest, IUpstreamMetadataCacheInfoStore>) (req =>
      {
        string[] strArray = new string[2]
        {
          req.Protocol.ToString(),
          "upstream"
        };
        return (IUpstreamMetadataCacheInfoStore) new UpstreamMetadataCacheInfoStore(blobServiceFactory.Get(new ContainerAddress((CollectionId) this.requestContext.ServiceHost.InstanceId, new Locator(strArray))), (ISerializer<UpstreamMetadataCacheInfo>) new UpstreamMetadataCacheInfoSerializer(this.stringToPackageNameConverter), this.requestContext.GetTracerFacade(), (ICache<string, object>) new RequestContextItemsAsCacheFacade(this.requestContext));
      }));
    }
  }
}
