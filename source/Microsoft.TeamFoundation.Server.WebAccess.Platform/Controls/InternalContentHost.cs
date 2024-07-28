// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Controls.InternalContentHost
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Controls
{
  public class InternalContentHost
  {
    public IHtmlString ToHtml(
      IHtmlString content,
      ContributionNode contributionNode,
      string className = null)
    {
      TagBuilder tagBuilder = new TagBuilder("div");
      tagBuilder.AddClass("internal-content-host");
      if (!string.IsNullOrEmpty(className))
        tagBuilder.AddClass(className);
      tagBuilder.MergeAttribute("id", contributionNode.Id.Replace(".", "-"));
      if (content != null)
        tagBuilder.Append((object) content);
      return (IHtmlString) MvcHtmlString.Create(tagBuilder.ToString());
    }
  }
}
