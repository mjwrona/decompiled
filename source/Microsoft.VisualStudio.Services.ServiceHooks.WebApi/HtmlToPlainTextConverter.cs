// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.WebApi.HtmlToPlainTextConverter
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8FEBD486-B6EA-43F6-B878-5BE1581FAD28
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.WebApi.dll

using HtmlAgilityPack;
using System.Web;

namespace Microsoft.VisualStudio.Services.ServiceHooks.WebApi
{
  internal class HtmlToPlainTextConverter : HtmlConverterBase
  {
    private bool m_ignoreUris;

    public HtmlToPlainTextConverter(bool ignoreUris) => this.m_ignoreUris = ignoreUris;

    protected override string ConvertListItem(
      HtmlNode node,
      string innerContent,
      int indentLevel = 0,
      int orderNumber = 0)
    {
      string str = string.Empty;
      if (indentLevel > 1)
        str = new string(' ', 4 * (indentLevel - 1));
      return string.Format("\r\n{2}{0} {1}", node.ParentNode.Name == "ol" ? (object) (orderNumber.ToString() + ".") : (object) "-", (object) innerContent, (object) str);
    }

    protected override string ConvertLink(
      HtmlNode node,
      string innerContent,
      int indentLevel = 0,
      int orderNumber = 0)
    {
      return this.m_ignoreUris || !node.Attributes.Contains("href") ? innerContent : string.Format("{0}({1})", (object) innerContent, (object) HttpUtility.HtmlDecode(node.Attributes["href"].Value));
    }

    protected override string ConvertImage(
      HtmlNode node,
      string innerContent,
      int indentLevel = 0,
      int orderNumber = 0)
    {
      return this.m_ignoreUris || !node.Attributes.Contains("alt") ? innerContent : string.Format("{0}({1})", (object) innerContent, (object) node.Attributes["alt"].Value);
    }
  }
}
