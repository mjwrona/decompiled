// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Jenkins.TriggerGitBuildAction
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Net.Http;
using System.Text;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Jenkins
{
  [Export(typeof (ConsumerActionImplementation))]
  public sealed class TriggerGitBuildAction : ConsumerActionImplementation
  {
    private const string c_id = "triggerGitBuild";
    private const string c_reporUrlJsonPath = "resource.repository.remoteUrl";
    private const string c_commitIdJsonPath = "resource.refUpdates[0].newObjectId";
    private const string c_branchRawNameJsonPath = "resource.refUpdates[0].name";
    private const string c_lastMergeCommitIdJsonPath = "resource.lastMergeCommit.commitId";
    private const string c_targetBranchRawNameJsonPath = "resource.targetRefName";
    private const string c_refUpdateOnBranchPrefix = "refs/heads/";
    private const string c_notifyRequiredParamsQueryFormat = "?url={0}";
    private const string c_notifyCommitIdParamQueryFormat = "&sha1={0}";
    private const string c_notifyBranchParamQueryFormat = "&branches={0}";
    private const string c_emptyCommitSha1 = "0000000000000000000000000000000000000000";
    private const string c_contentTypeJson = "application/json";
    private const string c_gitPushUrlFormatTfsPluginNotifyChanges = "{0}/team-events/gitPush";
    private const string c_pullRequestMergedUrlFormatTfsPluginNotifyChanges = "{0}/team-events/gitPullRequestMerged";
    private const string c_urlFormatNotifyChanges = "{0}/git/notifyCommit";
    public const string RegistryPathUrlFormatNotifyChanges = "/Service/ServiceHooks/JenkinsConsumer/TriggerGitBuildAction/UrlFormatNotifyChanges";
    public const string RegistryPathUrlFormatTfsPluginNotifyChanges = "/Service/ServiceHooks/JenkinsConsumer/TriggerGitBuildAction/UrlFormatTfsPluginNotifyChanges";
    private static readonly string[] s_supportedEventTypes = new string[2]
    {
      "git.push",
      "git.pullrequest.merged"
    };
    private static readonly IDictionary<string, string[]> s_supportedResourceVersions = (IDictionary<string, string[]>) new Dictionary<string, string[]>();

    public override string ConsumerId => "jenkins";

    public override string Id => "triggerGitBuild";

    public override string Name => JenkinsConsumerResources.TriggerGitBuildActionName;

    public override string Description => JenkinsConsumerResources.TriggerGitBuildActionDescription;

    public override string[] SupportedEventTypes => TriggerGitBuildAction.s_supportedEventTypes;

    public override IDictionary<string, string[]> SupportedResourceVersions => TriggerGitBuildAction.s_supportedResourceVersions;

    public override bool AllowResourceVersionOverride => false;

    public override IList<InputDescriptor> InputDescriptors => (IList<InputDescriptor>) new List<InputDescriptor>()
    {
      JenkinsConsumer.GetIntegrationLeveInputDescriptor()
    };

    public override InputValues GetInputValues(
      IVssRequestContext requestContext,
      string inputId,
      IDictionary<string, string> currentConsumerInputValues)
    {
      return inputId == "useTfsPlugin" ? JenkinsConsumer.GetInputValuesForIntegrationLevel(requestContext, currentConsumerInputValues, new Func<IVssRequestContext, HttpClient>(((ConsumerActionImplementation) this).GetHttpClient)) : base.GetInputValues(requestContext, inputId, currentConsumerInputValues);
    }

    public override string GetActionDescription(
      IVssRequestContext requestContext,
      IDictionary<string, string> consumerInputValues)
    {
      string uriString = (string) null;
      consumerInputValues.TryGetValue("serverBaseUrl", out uriString);
      if (uriString == null)
        throw new ArgumentNullException();
      try
      {
        return string.Format(JenkinsConsumerResources.TriggerGitBuildAction_DetailedDescriptionFormat, (object) new Uri(uriString).Host);
      }
      catch (UriFormatException ex)
      {
        return uriString;
      }
    }

    public ActionTask HandleEvent(
      IVssRequestContext requestContext,
      Event raisedEvent,
      HandleEventArgs eventArgs)
    {
      string consumerInput1 = eventArgs.Notification.GetConsumerInput("serverBaseUrl", true);
      string consumerInput2 = eventArgs.Notification.GetConsumerInput("username", true);
      string consumerInput3 = eventArgs.Notification.GetConsumerInput("password", true);
      bool useTfsPluginIntegration = string.Equals(eventArgs.Notification.GetConsumerInput("useTfsPlugin"), "tfs-plugin", StringComparison.OrdinalIgnoreCase);
      int num = raisedEvent.EventType.Equals("git.pullrequest.merged", StringComparison.Ordinal) ? 1 : 0;
      JObject jobject = raisedEvent.ToJObject();
      string repoUrl = (string) jobject.SelectToken("resource.repository.remoteUrl", true);
      string commitId;
      string fromRawBranchName;
      string tfsPluginUrlFormat;
      if (num != 0)
      {
        commitId = (string) jobject.SelectToken("resource.lastMergeCommit.commitId", true);
        fromRawBranchName = TriggerGitBuildAction.GetBranchNameFromRawBranchName((string) jobject.SelectToken("resource.targetRefName", false));
        tfsPluginUrlFormat = "{0}/team-events/gitPullRequestMerged";
      }
      else
      {
        commitId = (string) jobject.SelectToken("resource.refUpdates[0].newObjectId", true);
        fromRawBranchName = TriggerGitBuildAction.GetBranchNameFromRawBranchName((string) jobject.SelectToken("resource.refUpdates[0].name", false));
        tfsPluginUrlFormat = "{0}/team-events/gitPush";
      }
      string url = TriggerGitBuildAction.BuildNotifyChangesUrl(requestContext, tfsPluginUrlFormat, consumerInput1, repoUrl, commitId, fromRawBranchName, useTfsPluginIntegration);
      HttpRequestMessage httpRequestMessage = (HttpRequestMessage) new ServiceHooksHttpRequestMessage(requestContext, eventArgs.Notification, useTfsPluginIntegration ? HttpMethod.Post : HttpMethod.Get, url, consumerInput2, consumerInput3);
      string requestAsString = string.Empty;
      if (useTfsPluginIntegration)
      {
        this.AddRequestBody(httpRequestMessage, jobject, out requestAsString);
        JenkinsConsumer.AddCrumbHeaderIfNeeded(requestContext, httpRequestMessage, consumerInput1, consumerInput2, consumerInput3, new Func<IVssRequestContext, HttpClient>(((ConsumerActionImplementation) this).GetHttpClient), out string _);
      }
      return (ActionTask) new HttpActionTask(httpRequestMessage, httpRequestMessage.BuildHttpRequestStringRepresentation(requestAsString));
    }

    private static string BuildNotifyChangesUrl(
      IVssRequestContext requestContext,
      string tfsPluginUrlFormat,
      string serverBaseUrl,
      string repoUrl,
      string commitId,
      string branchName,
      bool useTfsPluginIntegration)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      if (useTfsPluginIntegration)
        return string.Format(service.GetValue<string>(requestContext, (RegistryQuery) "/Service/ServiceHooks/JenkinsConsumer/TriggerGitBuildAction/UrlFormatTfsPluginNotifyChanges", true, tfsPluginUrlFormat), (object) serverBaseUrl);
      StringBuilder stringBuilder = new StringBuilder(string.Format(service.GetValue<string>(requestContext, (RegistryQuery) "/Service/ServiceHooks/JenkinsConsumer/TriggerGitBuildAction/UrlFormatNotifyChanges", true, "{0}/git/notifyCommit"), (object) serverBaseUrl));
      stringBuilder.AppendFormat("?url={0}", (object) Uri.EscapeDataString(repoUrl));
      if (!string.IsNullOrEmpty(commitId) && commitId != "0000000000000000000000000000000000000000")
        stringBuilder.AppendFormat("&sha1={0}", (object) Uri.EscapeDataString(commitId));
      if (!string.IsNullOrWhiteSpace(branchName))
        stringBuilder.AppendFormat("&branches={0}", (object) Uri.EscapeDataString(branchName));
      return stringBuilder.ToString();
    }

    private static string GetBranchNameFromRawBranchName(string rawBranchName)
    {
      if (string.IsNullOrWhiteSpace(rawBranchName))
        return (string) null;
      return !rawBranchName.StartsWith("refs/heads/", StringComparison.OrdinalIgnoreCase) ? (string) null : rawBranchName.Substring("refs/heads/".Length);
    }

    private void AddRequestBody(
      HttpRequestMessage requestMessage,
      JObject raisedEventJObject,
      out string requestAsString)
    {
      requestAsString = raisedEventJObject.GetStringRepresentation();
      requestMessage.Content = (HttpContent) new StringContent(requestAsString, Encoding.UTF8, "application/json");
    }
  }
}
