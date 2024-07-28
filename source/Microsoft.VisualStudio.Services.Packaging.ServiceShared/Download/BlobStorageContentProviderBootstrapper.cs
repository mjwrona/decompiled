// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download.BlobStorageContentProviderBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download
{
  public class BlobStorageContentProviderBootstrapper : 
    IBootstrapper<ISpecificStorageContentProvider>
  {
    private readonly IVssRequestContext requestContext;

    public BlobStorageContentProviderBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public ISpecificStorageContentProvider Bootstrap() => (ISpecificStorageContentProvider) new DownloadBlobPackageFileAsContentResultHandler(this.requestContext.GetExecutionEnvironmentFacade(), new DownloadBlobPackageFileAsUriHandlerBootstrapper(this.requestContext).Bootstrap(), new ContentBlobStoreFacadeBootstrapper(this.requestContext).Bootstrap());
  }
}
