// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Web.VSDataProvider
// Assembly: Microsoft.VisualStudio.Services.Gallery.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17D36576-2EF3-4ABC-94BA-AF7891D15A3A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Web.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.Web.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Gallery.Web
{
  public class VSDataProvider : IVSDataProvider
  {
    public IVSGallery m_vsGalleryRESTProvider;
    internal static long s_lastRefreshTimeTicks = 0;
    private static readonly long s_refreshIntervalTicks = TimeSpan.FromHours(6.0).Ticks;
    private Guid _vsTabDataCacheServiceKey = new Guid("95AE11B3-4531-48E9-B379-2C10126AA719");

    public VSDataProvider(IVssRequestContext requestContext) => this.m_vsGalleryRESTProvider = (IVSGallery) new VSGallery();

    public async Task<VSTabData> GetVSTabData(IVssRequestContext requestContext)
    {
      GalleryVSTabDataCacheService vsTabDataCacheService = requestContext.GetService<GalleryVSTabDataCacheService>();
      VSTabData vsTabData1 = (VSTabData) null;
      long ticks = DateTime.Now.Ticks;
      long num = ticks + VSDataProvider.s_refreshIntervalTicks;
      long refreshTimeTicks = VSDataProvider.s_lastRefreshTimeTicks;
      if (((!vsTabDataCacheService.TryGetValue(requestContext, this._vsTabDataCacheServiceKey, out vsTabData1) ? 1 : 0) | (ticks < refreshTimeTicks ? (false ? 1 : 0) : (refreshTimeTicks == Interlocked.CompareExchange(ref VSDataProvider.s_lastRefreshTimeTicks, num, refreshTimeTicks) ? 1 : 0))) != 0)
      {
        vsTabData1 = await this.QueryVSTabDataAsync(requestContext);
        if (this.IsGalleryDataComplete(vsTabData1))
          vsTabDataCacheService.Set(requestContext, this._vsTabDataCacheServiceKey, vsTabData1);
      }
      VSTabData vsTabData2 = vsTabData1;
      vsTabDataCacheService = (GalleryVSTabDataCacheService) null;
      return vsTabData2;
    }

    private bool IsGalleryDataComplete(VSTabData vsTabData) => !vsTabData.FeaturedExtensions.IsNullOrEmpty<VSSearchResult>() && !vsTabData.MostPopularExtensions.IsNullOrEmpty<VSSearchResult>() && !vsTabData.TopRatedExtensions.IsNullOrEmpty<VSSearchResult>() && !((IEnumerable<VSCategory>) vsTabData.Categories).IsNullOrEmpty<VSCategory>();

    private async Task<VSTabData> QueryVSTabDataAsync(IVssRequestContext requestContext)
    {
      List<VSSearchResult> featuredExtensions = await this.m_vsGalleryRESTProvider.GetFeaturedExtensions();
      List<VSSearchResult> mostPopularExtensions = await this.m_vsGalleryRESTProvider.GetMostPopularExtensions();
      List<VSSearchResult> topRatedExtensions = await this.m_vsGalleryRESTProvider.GetTopRatedExtensions();
      VSTabData vsTabData = new VSTabData(featuredExtensions, mostPopularExtensions, topRatedExtensions, (List<VSSearchResult>) null, await this.m_vsGalleryRESTProvider.GetCategories());
      featuredExtensions = (List<VSSearchResult>) null;
      mostPopularExtensions = (List<VSSearchResult>) null;
      topRatedExtensions = (List<VSSearchResult>) null;
      return vsTabData;
    }
  }
}
