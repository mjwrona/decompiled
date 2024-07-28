// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.WebApi.PlainTextMessageUtility
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8FEBD486-B6EA-43F6-B878-5BE1581FAD28
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.WebApi.dll

using HtmlAgilityPack;
using System;

namespace Microsoft.VisualStudio.Services.ServiceHooks.WebApi
{
  public static class PlainTextMessageUtility
  {
    private static readonly HtmlToPlainTextConverter s_ignoreUrisConverter = new HtmlToPlainTextConverter(true);
    private static readonly HtmlToPlainTextConverter s_converter = new HtmlToPlainTextConverter(false);

    public static string ConvertFromHtml(string html) => !string.IsNullOrWhiteSpace(html) ? PlainTextMessageUtility.ConvertFromHtml(html, false) : string.Empty;

    public static string ConvertFromHtml(string html, bool ignoreUris) => PlainTextMessageUtility.Converter(ignoreUris).ConvertHtmlString(html);

    private static HtmlToPlainTextConverter Converter(bool ignoreUris) => !ignoreUris ? PlainTextMessageUtility.s_converter : PlainTextMessageUtility.s_ignoreUrisConverter;

    public static string ConvertFromHtml(object html) => html is HtmlDocument doc ? PlainTextMessageUtility.Converter(false).ConvertHtmlDocument(doc) : throw new ArgumentException(ServiceHooksWebApiResources.IncorrectParameterTypeExceptionFormat((object) "HtmlAgilityPack.HtmlDocument", (object) html.GetType().Name), nameof (html));
  }
}
