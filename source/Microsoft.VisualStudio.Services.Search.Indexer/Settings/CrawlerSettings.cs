// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.Settings.CrawlerSettings
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Search.Indexer.Settings
{
  public class CrawlerSettings
  {
    public IEnumerable<string> UnsupportedFileExtensionsForContentCrawl { get; internal set; }

    public IEnumerable<string> SupportedFileExtensionsForContentCrawl { get; internal set; }

    public IEnumerable<string> UnsupportedFileExtensionsForIndexing { get; internal set; }

    public IEnumerable<string> SupportedFileExtensionsForIndexing { get; internal set; }

    public IEntityType EntityType { get; internal set; }

    internal CrawlerSettings(IVssRequestContext requestContext, IEntityType entityType)
    {
      this.EntityType = entityType;
      switch (entityType.Name)
      {
        case "Code":
          this.IntializeCrawlerSettingsForCode(requestContext);
          break;
        case "Wiki":
          this.IntializeCrawlerSettingsForWiki(requestContext);
          break;
      }
    }

    private void IntializeCrawlerSettingsForCode(IVssRequestContext requestContext) => this.UnsupportedFileExtensionsForContentCrawl = this.GetExtensionListFromListString(requestContext.GetConfigValue<string>("/Service/ALMSearch/Settings/CodeUnsupportedFileExtensionListForContentCrawl"));

    private void IntializeCrawlerSettingsForWiki(IVssRequestContext requestContext)
    {
      this.SupportedFileExtensionsForContentCrawl = this.GetExtensionListFromListString(requestContext.GetConfigValue<string>("/Service/ALMSearch/Settings/WikiSupportedFileExtensionListForContentCrawl"));
      this.SupportedFileExtensionsForIndexing = this.GetExtensionListFromListString(requestContext.GetConfigValue<string>("/Service/ALMSearch/Settings/WikiSupportedFileExtensionListForIndexing"));
    }

    private IEnumerable<string> GetExtensionListFromListString(string extensionsList) => (IEnumerable<string>) extensionsList.ToLower(CultureInfo.InvariantCulture).Split(new char[1]
    {
      ','
    }, StringSplitOptions.RemoveEmptyEntries);
  }
}
