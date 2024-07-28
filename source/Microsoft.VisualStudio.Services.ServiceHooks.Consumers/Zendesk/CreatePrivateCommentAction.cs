// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Zendesk.CreatePrivateCommentAction
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using HtmlAgilityPack;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Zendesk
{
  [Export(typeof (ConsumerActionImplementation))]
  public sealed class CreatePrivateCommentAction : ConsumerActionImplementation
  {
    private const string c_id = "createPrivateComment";
    private const string c_workItemHistoryFieldName = "System.History";
    private const string c_resourceLinksJsonPath = "resource.relations";
    private const string c_workItemHistoryJsonPath = "resource.fields['System.History']";
    private const string c_commentPatternPublisherInputId = "commentPattern";
    private const string c_zendeskLinkUrlPattern = "^https://(?<accountName>.+)\\.zendesk\\.com/agent/#/tickets/(?<ticketId>\\d+)$";
    private const string c_basicAuthUsernameFormat = "{0}/token";
    public const string UrlFormatUpdateTicket = "https://{0}.zendesk.com/api/v2/tickets/update_many.json?ids={1}";
    public const string RegistryPathUrlFormatUpdateTicket = "/Service/ServiceHooks/ZendeskConsumer/CreatePrivateComment/UrlFormatUpdateTicket";
    private static readonly string[] s_supportedEventTypes = new string[1]
    {
      "workitem.commented"
    };
    private static readonly IDictionary<string, string[]> s_supportedResourceVersions = (IDictionary<string, string[]>) new Dictionary<string, string[]>()
    {
      {
        "workitem.commented",
        new string[1]{ "1.0" }
      }
    };
    private static Regex s_zendeskLinkUrlRegex = new Regex("^https://(?<accountName>.+)\\.zendesk\\.com/agent/#/tickets/(?<ticketId>\\d+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public override string ConsumerId => "zendesk";

    public override string Id => "createPrivateComment";

    public override string Name => ZendeskConsumerResources.CreatePrivateCommentActionName;

    public override string Description => ZendeskConsumerResources.CreatePrivateCommentActionDescription;

    public override string[] SupportedEventTypes => CreatePrivateCommentAction.s_supportedEventTypes;

    public override IDictionary<string, string[]> SupportedResourceVersions => CreatePrivateCommentAction.s_supportedResourceVersions;

    public override bool AllowResourceVersionOverride => false;

    public override IList<InputDescriptor> InputDescriptors => (IList<InputDescriptor>) new List<InputDescriptor>(0);

    public ActionTask HandleEvent(
      IVssRequestContext requestContext,
      Event raisedEvent,
      HandleEventArgs eventArgs)
    {
      string consumerInput1 = eventArgs.Notification.GetConsumerInput("accountName", true);
      string consumerInput2 = eventArgs.Notification.GetConsumerInput("username", true);
      string consumerInput3 = eventArgs.Notification.GetConsumerInput("apiToken", true);
      string empty = string.Empty;
      eventArgs.Notification.Details.PublisherInputs.TryGetValue("commentPattern", out empty);
      JObject jobject = raisedEvent.ToJObject();
      string[] zendeskLinkedTicketIds = CreatePrivateCommentAction.GetZendeskLinkedTicketIds(jobject, consumerInput1);
      if (zendeskLinkedTicketIds == null || zendeskLinkedTicketIds.Length == 0)
        return (ActionTask) new NoopActionTask(ZendeskConsumerResources.NoopAction_NoZendeskTicket);
      HtmlDocument htmlDocument;
      if (!this.TryParseComment((string) jobject.SelectToken("resource.fields['System.History']", false), empty, out htmlDocument))
        return (ActionTask) new NoopActionTask(string.Format(ZendeskConsumerResources.NoopAction_NoZendeskComment, (object) empty));
      string stringRepresentation = JObject.FromObject((object) new
      {
        ticket = new
        {
          comment = new
          {
            @public = false,
            body = MarkdownMessageUtility.ConvertFromHtml((object) htmlDocument)
          }
        }
      }).GetStringRepresentation();
      string privateCommentUrl = CreatePrivateCommentAction.BuildCreatePrivateCommentUrl(requestContext, consumerInput1, string.Join(",", zendeskLinkedTicketIds));
      ServiceHooksHttpRequestMessage request = new ServiceHooksHttpRequestMessage(requestContext, eventArgs.Notification, HttpMethod.Put, privateCommentUrl, string.Format("{0}/token", (object) consumerInput2), consumerInput3);
      request.Content = (HttpContent) new StringContent(stringRepresentation, Encoding.UTF8, JsonMediaTypeFormatter.DefaultMediaType.MediaType);
      return (ActionTask) new HttpActionTask((HttpRequestMessage) request, request.BuildHttpRequestStringRepresentation(stringRepresentation));
    }

    private bool TryParseComment(
      string comment,
      string commentPattern,
      out HtmlDocument htmlDocument,
      bool removesCommentPattern = true)
    {
      htmlDocument = (HtmlDocument) null;
      if (string.IsNullOrWhiteSpace(comment))
        return false;
      htmlDocument = new HtmlDocument();
      htmlDocument.LoadHtml(comment.TrimStart());
      if (string.IsNullOrWhiteSpace(commentPattern))
        return true;
      if (!(htmlDocument.DocumentNode.DescendantsAndSelf().FirstOrDefault<HtmlNode>((Func<HtmlNode, bool>) (n => n.NodeType == HtmlNodeType.Text && !HtmlNode.IsOverlappedClosingElement(((HtmlTextNode) n).Text) && !string.IsNullOrWhiteSpace(((HtmlTextNode) n).Text))) is HtmlTextNode htmlTextNode) || !htmlTextNode.Text.TrimStart().StartsWith(commentPattern, StringComparison.InvariantCultureIgnoreCase))
      {
        htmlDocument = (HtmlDocument) null;
        return false;
      }
      if (removesCommentPattern)
        htmlTextNode.Text = htmlTextNode.Text.TrimStart().Substring(commentPattern.Length).TrimStart();
      return true;
    }

    private static string BuildCreatePrivateCommentUrl(
      IVssRequestContext requestContext,
      string accountName,
      string ticketIds)
    {
      return string.Format(requestContext.GetService<IVssRegistryService>().GetValue<string>(requestContext, (RegistryQuery) "/Service/ServiceHooks/ZendeskConsumer/CreatePrivateComment/UrlFormatUpdateTicket", true, "https://{0}.zendesk.com/api/v2/tickets/update_many.json?ids={1}"), (object) accountName, (object) Uri.EscapeDataString(ticketIds));
    }

    private static string[] GetZendeskLinkedTicketIds(
      JObject raisedEventJObject,
      string accountName)
    {
      return !(raisedEventJObject.SelectToken("resource.relations", false) is JArray jarray) ? (string[]) null : jarray.Children<JObject>().Select<JObject, string>((Func<JObject, string>) (l =>
      {
        string input = (string) l["url"];
        if (string.IsNullOrEmpty(input))
          return (string) null;
        Match match = CreatePrivateCommentAction.s_zendeskLinkUrlRegex.Match(input);
        if (!match.Success)
          return (string) null;
        return !match.Groups[nameof (accountName)].Value.Equals(accountName, StringComparison.InvariantCultureIgnoreCase) ? (string) null : match.Groups["ticketId"].Value;
      })).Where<string>((Func<string, bool>) (s => !string.IsNullOrEmpty(s))).ToArray<string>();
    }
  }
}
