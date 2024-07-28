// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Web.GalleryDataProviderService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17D36576-2EF3-4ABC-94BA-AF7891D15A3A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Web.Routing;

namespace Microsoft.VisualStudio.Services.Gallery.Web
{
  public class GalleryDataProviderService : IGalleryDataProvider, IVssFrameworkService
  {
    private ICommerceDataProvider m_commerceDataProvider;
    private IProductExtensionsDataProvider m_productExtensionsProvider;
    private IVSDataProvider m_vsDataProvider;
    private IPageContextProvider m_pageContextProvider;

    public ICommerceDataProvider GetCommerceDataProvider(IVssRequestContext requestContext)
    {
      if (this.m_commerceDataProvider == null)
        this.m_commerceDataProvider = (ICommerceDataProvider) new CommerceDataProvider();
      return this.m_commerceDataProvider;
    }

    public IProductExtensionsDataProvider GetProductExtensionsProvider(
      IVssRequestContext requestContext)
    {
      if (this.m_productExtensionsProvider == null)
        this.m_productExtensionsProvider = (IProductExtensionsDataProvider) requestContext.GetService<CachedProductExtensionsService>();
      return this.m_productExtensionsProvider;
    }

    public IVSDataProvider GetVSDataProvider(IVssRequestContext requestContext)
    {
      if (this.m_vsDataProvider == null)
        this.m_vsDataProvider = (IVSDataProvider) new VSDataProvider(requestContext);
      return this.m_vsDataProvider;
    }

    public IPageContextProvider GetPageContextProvider(RequestContext requestContext)
    {
      if (this.m_pageContextProvider == null)
        this.m_pageContextProvider = (IPageContextProvider) new PageContextProvider();
      return this.m_pageContextProvider;
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }
  }
}
