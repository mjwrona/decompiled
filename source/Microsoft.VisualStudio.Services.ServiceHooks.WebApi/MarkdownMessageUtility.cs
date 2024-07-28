// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.WebApi.MarkdownMessageUtility
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8FEBD486-B6EA-43F6-B878-5BE1581FAD28
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.WebApi.dll

using HtmlAgilityPack;
using System;

namespace Microsoft.VisualStudio.Services.ServiceHooks.WebApi
{
  public static class MarkdownMessageUtility
  {
    private static HtmlToMarkdownConverter s_converter;

    public static string GetLink(string linkText, string url) => string.Format("[{0}]({1})", (object) linkText.Replace("[", "\\[").Replace("]", "\\]"), (object) url);

    public static string ConvertFromHtml(string html) => !string.IsNullOrWhiteSpace(html) ? MarkdownMessageUtility.Converter.ConvertHtmlString(html) : string.Empty;

    public static string ConvertFromHtml(object html) => html is HtmlDocument doc ? MarkdownMessageUtility.Converter.ConvertHtmlDocument(doc) : throw new ArgumentException(ServiceHooksWebApiResources.IncorrectParameterTypeExceptionFormat((object) "HtmlAgilityPack.HtmlDocument", (object) html.GetType().Name), nameof (html));

    private static HtmlToMarkdownConverter Converter => MarkdownMessageUtility.s_converter ?? (MarkdownMessageUtility.s_converter = new HtmlToMarkdownConverter());
  }
}
