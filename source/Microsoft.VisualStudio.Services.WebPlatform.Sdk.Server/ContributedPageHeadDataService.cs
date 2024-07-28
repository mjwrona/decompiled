// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.ContributedPageHeadDataService
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server
{
  internal class ContributedPageHeadDataService : 
    IContributedPageHeadDataService,
    IVssFrameworkService
  {
    private const string c_requestItemsKey = "_contributedPageHeadData";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public IEnumerable<MetaTag> GetMetaTags(IVssRequestContext requestContext) => (IEnumerable<MetaTag>) this.GetPageHeadData(requestContext).MetaTags;

    public void AddMetaTag(IVssRequestContext requestContext, string name, string content)
    {
      ContributedPageHeadDataService.ContributedPageHeadData pageHeadData = this.GetPageHeadData(requestContext);
      if (pageHeadData.MetaTags == null)
        pageHeadData.MetaTags = new List<MetaTag>();
      pageHeadData.MetaTags.Add(new MetaTag()
      {
        Name = name,
        Content = content
      });
    }

    public void ClearMetaTags(IVssRequestContext requestContext) => this.GetPageHeadData(requestContext).MetaTags = (List<MetaTag>) null;

    private ContributedPageHeadDataService.ContributedPageHeadData GetPageHeadData(
      IVssRequestContext requestContext)
    {
      object obj;
      if (requestContext.Items.TryGetValue("_contributedPageHeadData", out obj) && obj is ContributedPageHeadDataService.ContributedPageHeadData pageHeadData1)
        return pageHeadData1;
      ContributedPageHeadDataService.ContributedPageHeadData pageHeadData2 = new ContributedPageHeadDataService.ContributedPageHeadData();
      requestContext.Items["_contributedPageHeadData"] = (object) pageHeadData2;
      return pageHeadData2;
    }

    private class ContributedPageHeadData
    {
      public List<MetaTag> MetaTags;
    }
  }
}
