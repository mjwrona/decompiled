// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Sitemap.SitemapService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.Server.Components;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Sitemap
{
  internal class SitemapService : ISitemapService, IVssFrameworkService
  {
    private const string RootUrl = "https://marketplace.visualstudio.com/";
    private const string VsRootUrl = "https://marketplace.visualstudio.com/vs";
    private const string VSTSRootUrl = "https://marketplace.visualstudio.com/vsts";
    private const string VscodeRootUrl = "https://marketplace.visualstudio.com/vscode";
    private const string VsForMacRootUrl = "https://marketplace.visualstudio.com/vsformac";
    private const string SubscriptionsRootUrl = "https://marketplace.visualstudio.com/subscriptions";
    private const string ItemsBaseUrl = "https://marketplace.visualstudio.com/items?itemName=";
    private const string ChangeFreq = "Weekly";
    private const string SitemapFileName = "sitemap.xml";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public int GetFileIdOfSitemap(IVssRequestContext requestContext)
    {
      SitemapIndex sitemapIndex;
      using (SitemapIndexComponent component = requestContext.CreateComponent<SitemapIndexComponent>())
        sitemapIndex = component.QuerySitemapIndex();
      if (sitemapIndex != null)
        requestContext.Trace(12061104, TraceLevel.Info, "Gallery", "Sitemap", string.Format("Latest sitemap FileId={0}", (object) sitemapIndex.FileId));
      return sitemapIndex != null ? sitemapIndex.FileId : 0;
    }

    public int CreateOrUpdateSitemap(
      IVssRequestContext requestContext,
      ExtensionQuery extensionQuery,
      bool createNewSitemap)
    {
      IStorageService service = requestContext.GetService<IStorageService>();
      SitemapIndex sitemapIndex = (SitemapIndex) null;
      if (!createNewSitemap)
        sitemapIndex = this.GetSiteMapIndex(requestContext);
      int orUpdateSitemap;
      if (createNewSitemap || sitemapIndex == null)
      {
        int pageSize = 1000;
        orUpdateSitemap = this.CreateSitemap(requestContext, extensionQuery, service, pageSize);
        requestContext.Trace(12061105, TraceLevel.Info, "Gallery", "Sitemap", string.Format("Created new sitemap for total extensions={0}", (object) orUpdateSitemap));
      }
      else
      {
        orUpdateSitemap = this.UpdateSitemap(requestContext, extensionQuery, service, sitemapIndex);
        requestContext.Trace(12061106, TraceLevel.Info, "Gallery", "Sitemap", string.Format("Updated sitemap for total extensions={0}", (object) orUpdateSitemap));
      }
      return orUpdateSitemap;
    }

    public Stream CreateSitemapFile(
      IVssRequestContext requestContext,
      ExtensionQuery extensionQuery,
      int pageSize,
      out int countOfNewExtensions)
    {
      IPublishedExtensionService service = requestContext.GetService<IPublishedExtensionService>();
      UrlSet urlSet1 = new UrlSet();
      urlSet1.Url.Add(new SitemapUrl()
      {
        Loc = "https://marketplace.visualstudio.com/",
        ChangeFreq = "Weekly"
      });
      urlSet1.Url.Add(new SitemapUrl()
      {
        Loc = "https://marketplace.visualstudio.com/vs",
        ChangeFreq = "Weekly"
      });
      urlSet1.Url.Add(new SitemapUrl()
      {
        Loc = "https://marketplace.visualstudio.com/vsts",
        ChangeFreq = "Weekly"
      });
      urlSet1.Url.Add(new SitemapUrl()
      {
        Loc = "https://marketplace.visualstudio.com/vscode",
        ChangeFreq = "Weekly"
      });
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableVsForMac"))
        urlSet1.Url.Add(new SitemapUrl()
        {
          Loc = "https://marketplace.visualstudio.com/vsformac",
          ChangeFreq = "Weekly"
        });
      urlSet1.Url.Add(new SitemapUrl()
      {
        Loc = "https://marketplace.visualstudio.com/subscriptions",
        ChangeFreq = "Weekly"
      });
      countOfNewExtensions = 0;
      bool flag = false;
      int num = 1;
      while (!flag)
      {
        foreach (QueryFilter filter in extensionQuery.Filters)
        {
          filter.PageNumber = num;
          filter.PageSize = pageSize;
        }
        ExtensionQueryResult extensionQueryResult = service.QueryExtensions(requestContext, extensionQuery, (string) null);
        if (extensionQueryResult != null && extensionQueryResult.Results != null && extensionQueryResult.Results.Count > 0)
        {
          foreach (PublishedExtension extension in extensionQueryResult.Results.SelectMany<ExtensionFilterResult, PublishedExtension>((Func<ExtensionFilterResult, IEnumerable<PublishedExtension>>) (extensions => (IEnumerable<PublishedExtension>) extensions.Extensions)).Where<PublishedExtension>((Func<PublishedExtension, bool>) (extension => SitemapService.IsValidExtension(extension))))
          {
            ++countOfNewExtensions;
            UrlSet urlSet2 = urlSet1;
            SitemapService.AddNewUrl(extension, urlSet2);
          }
        }
        if (extensionQueryResult == null || extensionQueryResult.Results == null || extensionQueryResult.Results.Count == 0 || pageSize != extensionQueryResult.Results[0].Extensions.Count)
          flag = true;
        ++num;
      }
      return (Stream) SitemapService.SerializeXmlData(urlSet1);
    }

    public Stream UpdateSitemapFile(
      IVssRequestContext requestContext,
      ExtensionQuery extensionQuery,
      UrlSet urlSet,
      DateTime lastUpdated,
      out int countOfNewExtensions)
    {
      bool flag = false;
      if (urlSet == null)
        urlSet = new UrlSet()
        {
          Url = new List<SitemapUrl>()
        };
      ExtensionQueryResult extensionQueryResult = requestContext.GetService<IPublishedExtensionService>().QueryExtensions(requestContext, extensionQuery, (string) null);
      countOfNewExtensions = 0;
      if (extensionQueryResult.Results != null && extensionQueryResult.Results.Count > 0)
      {
        foreach (ExtensionFilterResult result in extensionQueryResult.Results)
        {
          foreach (PublishedExtension extension in result.Extensions.Where<PublishedExtension>((Func<PublishedExtension, bool>) (extension => SitemapService.IsValidExtension(extension))))
          {
            if (DateTime.Compare(extension.LastUpdated, lastUpdated) > 0)
            {
              flag = true;
              ++countOfNewExtensions;
              SitemapService.AddNewUrl(extension, urlSet);
            }
            else
              break;
          }
        }
      }
      return !flag ? (Stream) null : (Stream) SitemapService.SerializeXmlData(urlSet);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    private SitemapIndex GetSiteMapIndex(IVssRequestContext requestContext)
    {
      using (SitemapIndexComponent component = requestContext.CreateComponent<SitemapIndexComponent>())
        return component.QuerySitemapIndex();
    }

    private int UpdateSitemap(
      IVssRequestContext requestContext,
      ExtensionQuery extensionQuery,
      IStorageService storageService,
      SitemapIndex sitemapIndex)
    {
      int countOfNewExtensions;
      using (StreamReader streamReader = new StreamReader(storageService.RetrieveFile(requestContext, sitemapIndex.FileId)))
      {
        UrlSet urlSet = (UrlSet) new XmlSerializer(typeof (UrlSet)).Deserialize((TextReader) streamReader);
        using (Stream content = this.UpdateSitemapFile(requestContext, extensionQuery, urlSet, sitemapIndex.LastUpdated, out countOfNewExtensions))
        {
          if (content != null)
          {
            int fileId = storageService.UploadFile(requestContext, content, "sitemap.xml");
            using (SitemapIndexComponent component = requestContext.CreateComponent<SitemapIndexComponent>())
              component.CreateSitemapIndex(fileId);
          }
        }
      }
      return countOfNewExtensions;
    }

    private int CreateSitemap(
      IVssRequestContext requestContext,
      ExtensionQuery extensionQuery,
      IStorageService storageService,
      int pageSize)
    {
      int countOfNewExtensions;
      using (Stream sitemapFile = this.CreateSitemapFile(requestContext, extensionQuery, pageSize, out countOfNewExtensions))
      {
        int fileId = storageService.UploadFile(requestContext, sitemapFile, "sitemap.xml");
        using (SitemapIndexComponent component = requestContext.CreateComponent<SitemapIndexComponent>())
          component.CreateSitemapIndex(fileId);
      }
      return countOfNewExtensions;
    }

    private static MemoryStream SerializeXmlData(UrlSet urlSet)
    {
      MemoryStream memoryStream = new MemoryStream();
      XmlSerializer xmlSerializer = new XmlSerializer(typeof (UrlSet));
      XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
      namespaces.Add("", "");
      namespaces.Add("", "http://www.sitemaps.org/schemas/sitemap/0.9");
      StreamWriter streamWriter = new StreamWriter((Stream) memoryStream, Encoding.UTF8);
      xmlSerializer.Serialize((TextWriter) streamWriter, (object) urlSet, namespaces);
      memoryStream.Seek(0L, SeekOrigin.Begin);
      return memoryStream;
    }

    private static void AddNewUrl(PublishedExtension extension, UrlSet urlSet)
    {
      SitemapUrl sitemapUrl = new SitemapUrl()
      {
        Loc = "https://marketplace.visualstudio.com/items?itemName=" + extension.Publisher.PublisherName + "." + extension.ExtensionName,
        ChangeFreq = "Weekly"
      };
      urlSet.Url.Add(sitemapUrl);
    }

    private static bool IsValidExtension(PublishedExtension extension) => extension.Flags.HasFlag((Enum) PublishedExtensionFlags.Public) && !extension.Flags.HasFlag((Enum) PublishedExtensionFlags.System) && !extension.Flags.HasFlag((Enum) PublishedExtensionFlags.Unpublished) && !extension.Flags.HasFlag((Enum) PublishedExtensionFlags.Hidden);
  }
}
