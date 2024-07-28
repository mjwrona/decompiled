// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.PageHelper
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using Microsoft.TeamFoundation.Wiki.WebApi;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Wiki.Server
{
  public static class PageHelper
  {
    public static WikiPage ConstructWikiPage(
      Dictionary<WikiPagePath, WikiPage> wikiPagesMap,
      WikiPagePath givenPageFilePath)
    {
      if (wikiPagesMap == null)
        return (WikiPage) null;
      foreach (KeyValuePair<WikiPagePath, WikiPage> wikiPages in wikiPagesMap)
      {
        WikiPage wikiPage1 = wikiPages.Value;
        WikiPagePath key = wikiPages.Key;
        WikiPage wikiPage2;
        if (!key.IsWikiRootPage() && wikiPagesMap.TryGetValue(key.GetParentPagePath(), out wikiPage2))
          wikiPage2.SubPages.Add(wikiPage1);
      }
      WikiPage wikiPage;
      return wikiPagesMap.TryGetValue(givenPageFilePath, out wikiPage) ? wikiPage : (WikiPage) null;
    }

    public static List<WikiPage> FlattenWikiPage(WikiPage page)
    {
      if (page == null)
        return new List<WikiPage>();
      List<WikiPage> wikiPageList1 = new List<WikiPage>();
      List<WikiPage> wikiPageList2 = new List<WikiPage>()
      {
        page
      };
      while (wikiPageList2.Count > 0)
      {
        List<WikiPage> collection = new List<WikiPage>();
        foreach (WikiPage wikiPage in wikiPageList2)
        {
          wikiPageList1.Add(wikiPage);
          collection.AddRange((IEnumerable<WikiPage>) wikiPage.SubPages);
        }
        wikiPageList2.Clear();
        wikiPageList2.AddRange((IEnumerable<WikiPage>) collection);
      }
      return wikiPageList1;
    }

    public static bool IsPageWithoutAssociatedContent(WikiPage page) => page != null && page.IsParentPage && page.GitItemPath == null;
  }
}
