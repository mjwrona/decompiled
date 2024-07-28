// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemChangedEventProcessor
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.Linking;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public static class WorkItemChangedEventProcessor
  {
    private static readonly string AttachmentsRouteString = "/_apis/wit/attachments/";
    private const string InlineImgPattern = "<img[^<>]+?src=[\"'](?<url>[^<>]+?({0}|{1})[^\"']+?)[\"'][^<>]*>";
    private const string VsWorkItemUrlPattern = "href=[\"']x-mvwit:workitem/(?<workItemId>[^<>]+?)[\"']";
    private const string WorkItemInlineImgPlaceHolderHtml = "<a href=\"{0}\"><img src=\"{1}\"></a>";
    private const string VsWorkItemUrlHtml = "href=\"{0}\"";

    public static string ProcessNotificationHtml(
      IVssRequestContext requestContext,
      string richTextContent)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (richTextContent.IsNullOrEmpty<char>())
        return richTextContent;
      string pattern = string.Format("<img[^<>]+?src=[\"'](?<url>[^<>]+?({0}|{1})[^\"']+?)[\"'][^<>]*>", (object) WorkItemChangedEventProcessor.GetInlineImgPath(requestContext), (object) WorkItemChangedEventProcessor.AttachmentsRouteString);
      try
      {
        int result;
        return Regex.Replace(Regex.Replace(richTextContent, pattern, (MatchEvaluator) (match => string.Format("<a href=\"{0}\"><img src=\"{1}\"></a>", (object) match.Groups["url"].Value, (object) ServerResources.WorkItemInlineImgPlaceHolderURL())), RegexOptions.None, TimeSpan.FromSeconds(1.0)), "href=[\"']x-mvwit:workitem/(?<workItemId>[^<>]+?)[\"']", (MatchEvaluator) (match => int.TryParse(match.Groups["workItemId"].Value, out result) ? string.Format("href=\"{0}\"", (object) requestContext.GetService<ITswaServerHyperlinkService>().GetWorkItemEditorUrl(requestContext, result)) : match.Value), RegexOptions.None, TimeSpan.FromSeconds(1.0));
      }
      catch (Exception ex) when (ex is ArgumentException || ex is RegexMatchTimeoutException)
      {
        requestContext.TraceException(909720, "Services", "WorkItemService", ex);
        return richTextContent;
      }
    }

    private static string GetInlineImgPath(IVssRequestContext requestContext) => requestContext.GetService<ILocationService>().FindServiceDefinition(requestContext, "WorkitemAttachmentHandler", "WorkItemTracking").RelativePath;
  }
}
