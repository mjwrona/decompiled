// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Web.VSGallery
// Assembly: Microsoft.VisualStudio.Services.Gallery.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17D36576-2EF3-4ABC-94BA-AF7891D15A3A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.Web.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Microsoft.VisualStudio.Services.Gallery.Web
{
  public class VSGallery : IVSGallery
  {
    public const string c_VSGalleryRESTServer = "https://visualstudiogallery.msdn.microsoft.com";
    public const string c_VSGalleryRESTapiRoute = "/api/v1/";
    private readonly string[] featuredExtensionIds = new string[6]
    {
      "a23de100-31a1-405c-b4b7-d6be40c3dfff",
      "069a905d-387d-4415-bc37-665a5ac9caba",
      "a058d5d3-e654-43f8-a308-c3bdfdd0be4a",
      "f3b504c6-0095-42f1-a989-51d5fc2a8459",
      "0e313dfd-be80-4afb-b5e9-6e74d369f7a1",
      "6f4b51b6-5c6b-4a81-9cb5-f2daa560430b"
    };
    private HttpClient m_Client;

    public VSGallery(string serverURL = "https://visualstudiogallery.msdn.microsoft.com")
    {
      this.m_Client = new HttpClient();
      this.m_Client.BaseAddress = new Uri(serverURL + "/api/v1/");
    }

    public Task<List<VSSearchResult>> GetMostPopularExtensions() => this.GetExtensions(VSSortByValue.Popularity, GalleryEventId.VSGalleryMostPopularExtensionsException);

    public Task<List<VSSearchResult>> GetTopRatedExtensions() => this.GetExtensions(VSSortByValue.Ratings, GalleryEventId.VSGalleryTopRatedExtensionsException);

    public Task<List<VSSearchResult>> GetRecentlyAddedExtensions() => this.GetExtensions(VSSortByValue.Date, GalleryEventId.VSGalleryRecentlyAddedExtensionsException);

    private async Task<List<VSSearchResult>> GetExtensions(VSSortByValue order, int eventId)
    {
      try
      {
        return new List<VSSearchResult>((IEnumerable<VSSearchResult>) new JavaScriptSerializer().Deserialize<VSSearchResultsResponse>(await this.m_Client.GetStringAsync(string.Format("extensions?VisualStudioVersion=14.0&sort={0}", (object) order))).Items);
      }
      catch (Exception ex)
      {
        TeamFoundationEventLog.Default.Log(ex.Message, eventId, EventLogEntryType.Error);
        return (List<VSSearchResult>) null;
      }
    }

    public async Task<VSCategory[]> GetCategories()
    {
      try
      {
        return new JavaScriptSerializer().Deserialize<VSCategory[]>(await this.m_Client.GetStringAsync("categories?filterEmptyCategory=true"));
      }
      catch (Exception ex)
      {
        TeamFoundationEventLog.Default.Log(ex.Message, GalleryEventId.VSGalleryCategoriesException, EventLogEntryType.Error);
        return (VSCategory[]) null;
      }
    }

    public async Task<List<VSSearchResult>> GetFeaturedExtensions()
    {
      List<VSSearchResult> extensions = new List<VSSearchResult>();
      int count = this.featuredExtensionIds.Length;
      for (int i = 0; i < count; ++i)
      {
        VSSearchResult extensionById = await this.GetExtensionById(this.featuredExtensionIds[i]);
        extensions?.Add(extensionById);
      }
      List<VSSearchResult> featuredExtensions = extensions;
      extensions = (List<VSSearchResult>) null;
      return featuredExtensions;
    }

    private async Task<VSSearchResult> GetExtensionById(string id)
    {
      try
      {
        VSSearchResultsResponse searchResultsResponse = new JavaScriptSerializer().Deserialize<VSSearchResultsResponse>(await this.m_Client.GetStringAsync(string.Format("extensions?SearchText={0}", (object) id)));
        return searchResultsResponse.Items == null || searchResultsResponse.Items.Length == 0 ? (VSSearchResult) null : (searchResultsResponse.Items.Length > 1 ? this.GetExtensionByName(searchResultsResponse.Items, id) : searchResultsResponse.Items[0]);
      }
      catch
      {
        return (VSSearchResult) null;
      }
    }

    private VSSearchResult GetExtensionByName(VSSearchResult[] extensions, string name)
    {
      VSSearchResult extensionByName = (VSSearchResult) null;
      if (extensions == null || string.IsNullOrEmpty(name))
        return extensionByName;
      for (int index = 0; index < extensions.Length; ++index)
      {
        if (extensions[index].Name.Equals(name, StringComparison.OrdinalIgnoreCase))
        {
          extensionByName = extensions[index];
          break;
        }
      }
      return extensionByName;
    }
  }
}
