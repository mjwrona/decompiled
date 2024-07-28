// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework.CrossCollectionClientCreatorBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework
{
  public class CrossCollectionClientCreatorBootstrapper : 
    IBootstrapper<ICrossCollectionClientCreator>
  {
    private readonly IVssRequestContext requestContext;

    public CrossCollectionClientCreatorBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public ICrossCollectionClientCreator Bootstrap()
    {
      IVssRequestContext vssRequestContext = this.requestContext.To(TeamFoundationHostType.Deployment);
      return (ICrossCollectionClientCreator) new CrossCollectionClientCreator((Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades.IUrlHostResolutionService) new UrlHostResolutionServiceFacade(vssRequestContext, vssRequestContext.GetService<Microsoft.TeamFoundation.Framework.Server.IUrlHostResolutionService>()), (ICrossCollectionLocationDataService) new CrossCollectionLocationDataFacade(vssRequestContext), (Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades.ICreateClient) new CreateClientFacade(this.requestContext, (Microsoft.TeamFoundation.Framework.Server.ICreateClient) this.requestContext.ClientProvider));
    }
  }
}
