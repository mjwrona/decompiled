// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Slack.PostMessageToChannelAction
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Slack
{
  [Export(typeof (ConsumerActionImplementation))]
  public class PostMessageToChannelAction : ConsumerActionImplementation
  {
    private const char c_space = ' ';
    private const string c_crLf = "\r\n";
    private const string c_newLine = "\n";
    private const string c_id = "postMessageToChannel";
    private const string c_contentTypeJson = "application/json";
    private const string c_slackLinkFormat = "<{0}|{1}>";
    private const string c_markdownCodeFormat = "`{0}`";
    private const string c_markdownInlineLinkPattern = "\\[\\s*([^\\]]+)\\s*\\]\\(\\s*([^)]+)\\s*\\)";
    private const string c_webUrlCommitFormat = "{0}/commit/{1}";
    private const int c_commitShortSHA1Length = 7;
    private const string c_buildSucceededValue = "succeeded";
    private const string c_buildFailedValue = "failed";
    private const string c_workItemFieldResourceName = "PostMessageToChannelAction_WorkItemEvent_Field_{0}";
    private const string c_durationTimeSpanFormat = "hh\\:mm\\:ss";
    private const int c_commitsIncludedLimit = 10;
    private const int c_commitCommentMaxLength = 80;
    private const string c_deploymentSucceededStatusValue = "succeeded";
    private const string c_deploymentCanceledStatusValue = "canceled";
    private const string c_deploymentRejectedStatusValue = "rejected";
    private const string c_deploymentPartiallySucceededStatusValue = "partiallysucceeded";
    private const string c_deploymentApprovalPendingStatusValue = "pending";
    private const string c_deploymentApprovalApprovedStatusValue = "approved";
    private const string c_deploymentApprovalRejectedStatusValue = "rejected";
    private const string c_deploymentPostDeploymentApprovalType = "postdeploy";
    private const string c_releaseTriggerNone = "none";
    private const string c_releaseTriggerManual = "manual";
    private const string c_releaseTriggerContinuousIntegration = "continuousintegration";
    private const string c_releaseTriggerSchedule = "schedule";
    private const string c_releaseEventAttachementColourBlue = "#007acc";
    private const string c_releaseEventAttachementColourRed = "danger";
    private const string c_releaseEventAttachementColourGreen = "good";
    private const string c_releaseEventAttachementOrange = "#FF8000";
    private const string c_releaseEventAttachmentDateTimeFormat = "dd-MMMM-yyyy hh\\:mm\\:ss tt (UTC)";
    public const string UrlInputId = "url";
    private static readonly char[] s_lineBreakChars = new char[2]
    {
      '\r',
      '\n'
    };
    private static readonly string[] s_resourceVersion10 = new string[1]
    {
      "1.0"
    };
    private static readonly string[] s_resourceVersion10Preview1 = new string[1]
    {
      "1.0-preview.1"
    };
    private static readonly string[] s_resourceVersion30Preview1 = new string[1]
    {
      "3.0-preview.1"
    };
    private static readonly string[] s_supportedEventTypes = new string[1]
    {
      "*"
    };
    private static readonly IDictionary<string, string[]> s_supportedResourceVersions = (IDictionary<string, string[]>) new Dictionary<string, string[]>()
    {
      {
        "build.complete",
        PostMessageToChannelAction.s_resourceVersion10
      },
      {
        "git.push",
        PostMessageToChannelAction.s_resourceVersion10
      },
      {
        "git.pullrequest.created",
        PostMessageToChannelAction.s_resourceVersion10Preview1
      },
      {
        "git.pullrequest.updated",
        PostMessageToChannelAction.s_resourceVersion10Preview1
      },
      {
        "tfvc.checkin",
        PostMessageToChannelAction.s_resourceVersion10
      },
      {
        "workitem.created",
        PostMessageToChannelAction.s_resourceVersion10
      },
      {
        "workitem.updated",
        PostMessageToChannelAction.s_resourceVersion10
      },
      {
        "workitem.commented",
        PostMessageToChannelAction.s_resourceVersion10
      },
      {
        "message.posted",
        PostMessageToChannelAction.s_resourceVersion10
      },
      {
        "ms.vss-release.release-created-event",
        PostMessageToChannelAction.s_resourceVersion30Preview1
      },
      {
        "ms.vss-release.release-abandoned-event",
        PostMessageToChannelAction.s_resourceVersion30Preview1
      },
      {
        "ms.vss-release.deployment-approval-pending-event",
        PostMessageToChannelAction.s_resourceVersion30Preview1
      },
      {
        "ms.vss-release.deployment-approval-completed-event",
        PostMessageToChannelAction.s_resourceVersion30Preview1
      },
      {
        "ms.vss-release.deployment-started-event",
        PostMessageToChannelAction.s_resourceVersion30Preview1
      },
      {
        "ms.vss-release.deployment-completed-event",
        PostMessageToChannelAction.s_resourceVersion30Preview1
      }
    };
    private static readonly Regex s_markdownInlineLinkRegex = new Regex("\\[\\s*([^\\]]+)\\s*\\]\\(\\s*([^)]+)\\s*\\)", RegexOptions.Compiled);
    private static readonly IDictionary<string, Func<JObject, JObject>> s_eventAttachmentBuilders = (IDictionary<string, Func<JObject, JObject>>) new Dictionary<string, Func<JObject, JObject>>()
    {
      {
        "build.complete",
        new Func<JObject, JObject>(PostMessageToChannelAction.BuildAttachmentForBuildComplete)
      },
      {
        "git.push",
        new Func<JObject, JObject>(PostMessageToChannelAction.BuildAttachmentForGitPush)
      },
      {
        "git.pullrequest.created",
        new Func<JObject, JObject>(PostMessageToChannelAction.BuildAttachmentForGitPullRequestCreated)
      },
      {
        "git.pullrequest.updated",
        new Func<JObject, JObject>(PostMessageToChannelAction.BuildAttachmentForGitPullRequestUpdated)
      },
      {
        "tfvc.checkin",
        new Func<JObject, JObject>(PostMessageToChannelAction.BuildAttachmentForTfvcCheckin)
      },
      {
        "workitem.created",
        new Func<JObject, JObject>(PostMessageToChannelAction.BuildAttachmentForWorkItemCreated)
      },
      {
        "workitem.updated",
        new Func<JObject, JObject>(PostMessageToChannelAction.BuildAttachmentForWorkItemUpdated)
      },
      {
        "workitem.commented",
        new Func<JObject, JObject>(PostMessageToChannelAction.BuildAttachmentForWorkItemCommented)
      },
      {
        "message.posted",
        new Func<JObject, JObject>(PostMessageToChannelAction.BuildAttachmentForMessagePosted)
      },
      {
        "ms.vss-release.release-created-event",
        new Func<JObject, JObject>(PostMessageToChannelAction.BuildAttachmentForReleaseCreated)
      },
      {
        "ms.vss-release.release-abandoned-event",
        new Func<JObject, JObject>(PostMessageToChannelAction.BuildAttachmentForReleaseAbandoned)
      },
      {
        "ms.vss-release.deployment-approval-pending-event",
        new Func<JObject, JObject>(PostMessageToChannelAction.BuildAttachmentForReleaseApprovalPending)
      },
      {
        "ms.vss-release.deployment-approval-completed-event",
        new Func<JObject, JObject>(PostMessageToChannelAction.BuildAttachmentForReleaseApprovalCompleted)
      },
      {
        "ms.vss-release.deployment-started-event",
        new Func<JObject, JObject>(PostMessageToChannelAction.BuildAttachmentForReleaseDeploymentStarted)
      },
      {
        "ms.vss-release.deployment-completed-event",
        new Func<JObject, JObject>(PostMessageToChannelAction.BuildAttachmentForReleaseDeploymentCompleted)
      }
    };
    private static readonly string[] s_workItemEventFieldsToWatch = new string[7]
    {
      "System.AreaPath",
      "System.IterationPath",
      "System.State",
      "System.Reason",
      "System.AssignedTo",
      "Microsoft.VSTS.Common.Priority",
      "Microsoft.VSTS.Common.Severity"
    };

    public static string ConsumerActionId => "postMessageToChannel";

    public override string ConsumerId => SlackConsumer.ConsumerId;

    public override string Id => "postMessageToChannel";

    public override string Name => SlackConsumerResources.PostMessageToChannelActionName;

    public override string Description => SlackConsumerResources.PostMessageToChannelActionDescription;

    public override string[] SupportedEventTypes => PostMessageToChannelAction.s_supportedEventTypes;

    public override IDictionary<string, string[]> SupportedResourceVersions => PostMessageToChannelAction.s_supportedResourceVersions;

    public override bool AllowResourceVersionOverride => false;

    public override IList<InputDescriptor> InputDescriptors => (IList<InputDescriptor>) new List<InputDescriptor>()
    {
      new InputDescriptor()
      {
        Name = SlackConsumerResources.PostMessageToChannelAction_UrlInputName,
        Description = SlackConsumerResources.PostMessageToChannelAction_UrlInputDescription,
        InputMode = InputMode.TextBox,
        Id = "url",
        IsConfidential = false,
        Validation = new InputValidation()
        {
          DataType = InputDataType.Uri,
          IsRequired = true
        }
      }
    };

    public override string GetActionDescription(
      IVssRequestContext requestContext,
      IDictionary<string, string> consumerInputValues)
    {
      return SlackConsumerResources.PostMessageToChannelActionDetailedDescription;
    }

    public ActionTask HandleEvent(
      IVssRequestContext requestContext,
      Event raisedEvent,
      HandleEventArgs e)
    {
      string consumerInput = e.Notification.GetConsumerInput("url", true);
      JObject jObject = this.TransformEvent(raisedEvent, e);
      HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, consumerInput);
      string stringRepresentation = jObject.GetStringRepresentation();
      string requestBody = jObject.GetStringRepresentation(false, Formatting.Indented).Replace("\r\n", "\n");
      httpRequestMessage.Content = (HttpContent) new StringContent(stringRepresentation, Encoding.UTF8, "application/json");
      HttpRequestStringRepresentationBuilder representationBuilder = new HttpRequestStringRepresentationBuilder(httpRequestMessage, requestBody);
      return (ActionTask) new HttpActionTask(httpRequestMessage, representationBuilder.ToString());
    }

    private JObject TransformEvent(Event raisedEvent, HandleEventArgs e)
    {
      JObject jobject1 = new JObject();
      if (PostMessageToChannelAction.s_eventAttachmentBuilders.ContainsKey(raisedEvent.EventType))
      {
        if (!(raisedEvent.Resource is JObject jobject2) && raisedEvent.Resource != null)
          jobject2 = JObject.FromObject(raisedEvent.Resource, JsonSerializer.Create(CommonConsumerSettings.JsonSerializerSettings));
        JObject jobject3 = PostMessageToChannelAction.s_eventAttachmentBuilders[raisedEvent.EventType](jobject2) ?? new JObject();
        jobject3["pretext"] = jobject3["pretext"] ?? (JToken) PostMessageToChannelAction.TransformInlineLinksToSlackFormat(raisedEvent.Message.Markdown);
        JArray jarray = (JArray) jobject3["mrkdwn_in"] ?? new JArray();
        if (!jarray.Contains((JToken) "pretext"))
          jarray.AddFirst((object) "pretext");
        jobject3["mrkdwn_in"] = (JToken) jarray;
        jobject3["fallback"] = jobject3["fallback"] ?? (JToken) raisedEvent.DetailedMessage.Text;
        jobject1["attachments"] = JToken.FromObject((object) new JObject[1]
        {
          jobject3
        });
      }
      else
        jobject1["text"] = (JToken) PostMessageToChannelAction.TransformInlineLinksToSlackFormat(raisedEvent.Message.Markdown);
      return jobject1;
    }

    private static JObject BuildAttachmentForBuildComplete(JObject resource) => JObject.FromObject((object) new
    {
      color = PostMessageToChannelAction.GetBuildAttachmentColor((string) resource["status"]),
      fields = new JObject[3]
      {
        PostMessageToChannelAction.BuildAttachmentField(SlackConsumerResources.PostMessageToChannelAction_BuildComplete_Field_RequestedBy, (string) resource["requests"].First<JToken>()[(object) "requestedFor"][(object) "displayName"]),
        PostMessageToChannelAction.BuildAttachmentField(SlackConsumerResources.PostMessageToChannelAction_BuildComplete_Field_Duration, ((DateTime) resource["finishTime"] - (DateTime) resource["startTime"]).ToString("hh\\:mm\\:ss")),
        PostMessageToChannelAction.BuildAttachmentField(SlackConsumerResources.PostMessageToChannelAction_BuildComplete_Field_Definition, (string) resource["definition"][(object) "name"])
      }
    });

    private static JObject BuildAttachmentForGitPush(JObject resource)
    {
      JObject jobject = PostMessageToChannelAction.BuildGitPushCommitsAttachmentField(resource);
      if (jobject == null)
        return (JObject) null;
      return JObject.FromObject((object) new
      {
        fields = new JObject[1]{ jobject },
        mrkdwn_in = new string[1]{ "fields" }
      });
    }

    private static JObject BuildGitPushCommitsAttachmentField(JObject resource)
    {
      if (resource["commits"] == null)
        return (JObject) null;
      string repoRemoteUrl = (string) resource["repository"][(object) "remoteUrl"];
      IEnumerable<JObject> source = resource["commits"].Values<JObject>();
      int num = source.Count<JObject>();
      StringBuilder stringBuilder = source.Take<JObject>(10).Aggregate<JObject, StringBuilder>(new StringBuilder(), (Func<StringBuilder, JObject, StringBuilder>) ((acc, commit) => acc.AppendFormat(SlackConsumerResources.PostMessageToChannelAction_GitPush_Field_Commit_Format, (object) PostMessageToChannelAction.BuildSlackLink(PostMessageToChannelAction.GetCommitShortSHA1((string) commit["commitId"]), string.Format("{0}/commit/{1}", (object) repoRemoteUrl, (object) (string) commit["commitId"])), (object) PostMessageToChannelAction.GetCommitShortComment((string) commit["comment"])).Append(Environment.NewLine)));
      return PostMessageToChannelAction.BuildAttachmentField(string.Format(SlackConsumerResources.PostMessageToChannelAction_GitPush_Field_Commits, num <= 10 ? (object) num.ToString() : (object) string.Format(SlackConsumerResources.PostMessageToChannelAction_GitPush_Field_Commits_Limit_Indication, (object) 10)), stringBuilder.ToString(), false);
    }

    private static JObject BuildAttachmentForGitPullRequestCreated(JObject resource)
    {
      List<JObject> jobjectList = new List<JObject>();
      if (resource.TryGetValue("forkSource", out JToken _))
      {
        jobjectList.Add(PostMessageToChannelAction.BuildAttachmentField(SlackConsumerResources.PostMessageToChannelAction_GitPullRequestCreated_Field_SourceRepo, PostMessageToChannelAction.ConvertToMarkdownCode((string) resource["forkSource"][(object) "repository"][(object) "name"])));
        jobjectList.Add(PostMessageToChannelAction.BuildAttachmentField(SlackConsumerResources.PostMessageToChannelAction_GitPullRequestCreated_Field_Source, PostMessageToChannelAction.ConvertToMarkdownCode((string) resource["forkSource"][(object) "name"])));
      }
      else
        jobjectList.Add(PostMessageToChannelAction.BuildAttachmentField(SlackConsumerResources.PostMessageToChannelAction_GitPullRequestCreated_Field_Source, PostMessageToChannelAction.ConvertToMarkdownCode((string) resource["sourceRefName"])));
      jobjectList.Add(PostMessageToChannelAction.BuildAttachmentField(SlackConsumerResources.PostMessageToChannelAction_GitPullRequestCreated_Field_Target, PostMessageToChannelAction.ConvertToMarkdownCode((string) resource["targetRefName"])));
      jobjectList.Add(PostMessageToChannelAction.BuildAttachmentField(SlackConsumerResources.PostMessageToChannelAction_GitPullRequestCreated_Field_Status, PostMessageToChannelAction.ConvertToMarkdownCode((string) resource["status"])));
      jobjectList.Add(PostMessageToChannelAction.BuildAttachmentField(SlackConsumerResources.PostMessageToChannelAction_GitPullRequestCreated_Field_MergeStatus, PostMessageToChannelAction.ConvertToMarkdownCode((string) resource["mergeStatus"])));
      JObject jobject1 = PostMessageToChannelAction.BuildAttachmentField(SlackConsumerResources.PostMessageToChannelAction_GitPullRequestCreated_Field_Description, (string) resource["description"], false);
      if (jobject1 != null)
        jobjectList.Add(jobject1);
      JObject jobject2 = PostMessageToChannelAction.BuildGitPullRequestReviewersAttachmentField(resource);
      if (jobject2 != null)
        jobjectList.Add(jobject2);
      return JObject.FromObject((object) new
      {
        fields = jobjectList.ToArray(),
        mrkdwn_in = new string[1]{ "fields" }
      });
    }

    private static JObject BuildGitPullRequestReviewersAttachmentField(JObject resource) => resource["reviewers"] == null ? (JObject) null : PostMessageToChannelAction.BuildAttachmentField(SlackConsumerResources.PostMessageToChannelAction_GitPullRequest_Field_Reviewers, resource["reviewers"].Values<JObject>().Aggregate<JObject, StringBuilder>(new StringBuilder(), (Func<StringBuilder, JObject, StringBuilder>) ((acc, reviewer) => acc.AppendFormat(SlackConsumerResources.PostMessageToChannelAction_GitPullRequest_Field_Reviewers_Format, (object) reviewer["displayName"], (object) PostMessageToChannelAction.GetGitPullRequestReviewerUniqueNameReference(reviewer)).Append(Environment.NewLine))).ToString(), false);

    private static string GetGitPullRequestReviewerUniqueNameReference(JObject reviewer)
    {
      JToken jtoken = reviewer["isContainer"];
      if (jtoken != null && jtoken.Value<bool>())
        return string.Empty;
      string str = (string) reviewer["uniqueName"];
      return string.IsNullOrEmpty(str) ? string.Empty : string.Format(SlackConsumerResources.PostMessageToChannelAction_GitPullRequest_Field_Reviewers_UniqueName_Format, (object) str);
    }

    private static JObject BuildAttachmentForGitPullRequestUpdated(JObject resource) => (JObject) null;

    private static JObject BuildAttachmentForTfvcCheckin(JObject resource) => JObject.FromObject((object) new
    {
      fields = new JObject[1]
      {
        PostMessageToChannelAction.BuildAttachmentField(SlackConsumerResources.PostMessageToChannelAction_CodeCheckedIn_Field_Comment, (string) resource["comment"], false)
      }
    });

    private static JObject BuildAttachmentForWorkItemCreated(JObject resource) => JObject.FromObject((object) new
    {
      fields = ((IEnumerable<string>) PostMessageToChannelAction.s_workItemEventFieldsToWatch).Select<string, JObject>((Func<string, JObject>) (fieldName => PostMessageToChannelAction.BuildAttachmentField(PostMessageToChannelAction.GetWorkItemFieldDisplayName(fieldName), (string) resource["fields"][(object) fieldName]))).Where<JObject>((Func<JObject, bool>) (field => field != null)).ToArray<JObject>()
    });

    private static JObject BuildAttachmentForWorkItemUpdated(JObject resource) => resource["fields"] == null ? (JObject) null : JObject.FromObject((object) new
    {
      fields = resource["fields"].Children<JProperty>().Where<JProperty>((Func<JProperty, bool>) (p => ((IEnumerable<string>) PostMessageToChannelAction.s_workItemEventFieldsToWatch).Contains<string>(p.Name))).Aggregate<JProperty, List<JObject>>(new List<JObject>(), (Func<List<JObject>, JProperty, List<JObject>>) ((currFields, field) =>
      {
        currFields.Add(PostMessageToChannelAction.BuildAttachmentField(PostMessageToChannelAction.GetAttachmentOldFieldNameForWorkItemUpdated(field.Name), (string) field.Value[(object) "oldValue"], nullIfNoValue: false));
        currFields.Add(PostMessageToChannelAction.BuildAttachmentField(PostMessageToChannelAction.GetAttachmentNewFieldNameForWorkItemUpdated(field.Name), (string) field.Value[(object) "newValue"], nullIfNoValue: false));
        return currFields;
      })).ToArray()
    });

    private static JObject BuildAttachmentForWorkItemCommented(JObject resource) => JObject.FromObject((object) new
    {
      fields = new JObject[1]
      {
        PostMessageToChannelAction.BuildAttachmentField(SlackConsumerResources.PostMessageToChannelAction_WorkItemCommented_Field_Comment, PlainTextMessageUtility.ConvertFromHtml((string) resource["fields"][(object) "System.History"]), false)
      }
    });

    private static JObject BuildAttachmentForMessagePosted(JObject resource) => JObject.FromObject((object) new
    {
      fields = new JObject[1]
      {
        PostMessageToChannelAction.BuildAttachmentField(SlackConsumerResources.PostMessageToChannelAction_MessagePosted_Field_Message, (string) resource["content"], false)
      }
    });

    private static JObject BuildAttachmentForReleaseCreated(JObject resource) => JObject.FromObject((object) new
    {
      color = string.Empty,
      fields = new JObject[4]
      {
        PostMessageToChannelAction.BuildAttachmentField(SlackConsumerResources.PostMessageToChannelAction_ReleaseEvent_Field_RequestedBy, (string) resource["release"][(object) "createdBy"][(object) "displayName"]),
        PostMessageToChannelAction.BuildAttachmentField(SlackConsumerResources.PostMessageToChannelAction_ReleaseEvent_Field_TriggerReason, PostMessageToChannelAction.GetReleaseTriggerAttachmentValue((string) resource["release"][(object) "reason"])),
        PostMessageToChannelAction.BuildAttachmentField(SlackConsumerResources.PostMessageToChannelAction_ReleaseEvent_Field_ReleaseDefinition, PostMessageToChannelAction.GetReleaseDefinitionLinkValueFromRelease(resource)),
        PostMessageToChannelAction.BuildAttachmentField(SlackConsumerResources.PostMessageToChannelAction_ReleaseEvent_Field_Build, PostMessageToChannelAction.GetReleaseArtifactAttachmentDetails(resource))
      }
    });

    private static JObject BuildAttachmentForReleaseAbandoned(JObject resource) => JObject.FromObject((object) new
    {
      color = string.Empty,
      fields = new JObject[4]
      {
        PostMessageToChannelAction.BuildAttachmentField(SlackConsumerResources.PostMessageToChannelAction_ReleaseEvent_Field_AbandonedBy, (string) resource["release"][(object) "modifiedBy"][(object) "displayName"]),
        PostMessageToChannelAction.BuildAttachmentField(SlackConsumerResources.PostMessageToChannelAction_ReleaseEvent_Field_AbandonedOn, ((DateTime) resource["release"][(object) "modifiedOn"]).ToString("dd-MMMM-yyyy hh\\:mm\\:ss tt (UTC)", (IFormatProvider) CultureInfo.InvariantCulture)),
        PostMessageToChannelAction.BuildAttachmentField(SlackConsumerResources.PostMessageToChannelAction_ReleaseEvent_Field_ReleaseDefinition, PostMessageToChannelAction.GetReleaseDefinitionLinkValueFromRelease(resource)),
        PostMessageToChannelAction.BuildAttachmentField(SlackConsumerResources.PostMessageToChannelAction_ReleaseEvent_Field_Build, PostMessageToChannelAction.GetReleaseArtifactAttachmentDetails(resource))
      }
    });

    private static JObject BuildAttachmentForReleaseApprovalPending(JObject resource) => JObject.FromObject((object) new
    {
      color = PostMessageToChannelAction.GetDeploymentApprovalAttachmentColor(resource),
      fields = new JObject[3]
      {
        PostMessageToChannelAction.BuildAttachmentField(SlackConsumerResources.PostMessageToChannelAction_ReleaseEvent_Field_PendingOn, (string) resource["approval"][(object) "approver"][(object) "displayName"]),
        PostMessageToChannelAction.BuildAttachmentField(SlackConsumerResources.PostMessageToChannelAction_ReleaseEvent_Field_PendingSince, ((DateTime) resource["approval"][(object) "modifiedOn"]).ToString("dd-MMMM-yyyy hh\\:mm\\:ss tt (UTC)", (IFormatProvider) CultureInfo.InvariantCulture)),
        PostMessageToChannelAction.BuildAttachmentField(SlackConsumerResources.PostMessageToChannelAction_ReleaseEvent_Field_ReleaseDefinition, PostMessageToChannelAction.GetReleaseDefinitionLinkValueFromRelease(resource))
      }
    });

    private static JObject BuildAttachmentForReleaseApprovalCompleted(JObject resource)
    {
      string title = (string) resource["approval"][(object) "status"] == "approved" ? SlackConsumerResources.PostMessageToChannelAction_ReleaseEvent_Field_ApprovedBy : SlackConsumerResources.PostMessageToChannelAction_ReleaseEvent_Field_RejectedBy;
      return JObject.FromObject((object) new
      {
        color = PostMessageToChannelAction.GetDeploymentApprovalAttachmentColor(resource),
        fields = new JObject[3]
        {
          PostMessageToChannelAction.BuildAttachmentField(title, (string) resource["approval"][(object) "approvedBy"][(object) "displayName"]),
          PostMessageToChannelAction.BuildAttachmentField(SlackConsumerResources.PostMessageToChannelAction_ReleaseEvent_Field_Release, PostMessageToChannelAction.GetReleaseNameAttachmentValueFromRelease(resource)),
          PostMessageToChannelAction.BuildAttachmentField(SlackConsumerResources.PostMessageToChannelAction_ReleaseEvent_Field_ReleaseDefinition, PostMessageToChannelAction.GetReleaseDefinitionLinkValueFromRelease(resource))
        }
      });
    }

    private static JObject BuildAttachmentForReleaseDeploymentStarted(JObject resource) => JObject.FromObject((object) new
    {
      color = "#007acc",
      fields = new JObject[3]
      {
        PostMessageToChannelAction.BuildAttachmentField(SlackConsumerResources.PostMessageToChannelAction_ReleaseEvent_Field_Release, PostMessageToChannelAction.GetReleaseNameAttachmentValueFromEnvironmentRelease(resource)),
        PostMessageToChannelAction.BuildAttachmentField(SlackConsumerResources.PostMessageToChannelAction_ReleaseEvent_Field_TriggerReason, (string) resource["environment"][(object) "triggerReason"]),
        PostMessageToChannelAction.BuildAttachmentField(SlackConsumerResources.PostMessageToChannelAction_ReleaseEvent_Field_ReleaseDefinition, PostMessageToChannelAction.GetReleaseDefinitionLinkValueFromRelease(resource))
      }
    });

    private static JObject BuildAttachmentForReleaseDeploymentCompleted(JObject resource) => JObject.FromObject((object) new
    {
      color = PostMessageToChannelAction.GetDeploymentStatusAttachmentColor((string) resource["environment"][(object) "status"]),
      fields = new JObject[4]
      {
        PostMessageToChannelAction.BuildAttachmentField(SlackConsumerResources.PostMessageToChannelAction_ReleaseEvent_Field_DeploymentStatus, PostMessageToChannelAction.GetDeploymentStatusAttachmentValue((string) resource["environment"][(object) "status"])),
        PostMessageToChannelAction.BuildAttachmentField(SlackConsumerResources.PostMessageToChannelAction_ReleaseEvent_Field_TimeToDeploy, PostMessageToChannelAction.GetReleaseTimeToDeployAttachmentValue((string) resource["environment"][(object) "timeToDeploy"])),
        PostMessageToChannelAction.BuildAttachmentField(SlackConsumerResources.PostMessageToChannelAction_ReleaseEvent_Field_Release, PostMessageToChannelAction.GetReleaseNameAttachmentValueFromEnvironmentRelease(resource)),
        PostMessageToChannelAction.BuildAttachmentField(SlackConsumerResources.PostMessageToChannelAction_ReleaseEvent_Field_ReleaseDefinition, PostMessageToChannelAction.GetReleaseDefinitionLinkValueFromEnvironmentRelease(resource))
      }
    });

    private static string GetAttachmentOldFieldNameForWorkItemUpdated(string fieldRefName) => string.Format(SlackConsumerResources.PostMessageToChannelAction_WorkItemUpdated_Field_Old_Format, (object) PostMessageToChannelAction.GetWorkItemFieldDisplayName(fieldRefName));

    private static string GetAttachmentNewFieldNameForWorkItemUpdated(string fieldRefName) => string.Format(SlackConsumerResources.PostMessageToChannelAction_WorkItemUpdated_Field_New_Format, (object) PostMessageToChannelAction.GetWorkItemFieldDisplayName(fieldRefName));

    private static string GetWorkItemFieldDisplayName(string fieldRefName) => SlackConsumerResources.ResourceManager.GetString(string.Format("PostMessageToChannelAction_WorkItemEvent_Field_{0}", (object) fieldRefName.Replace('.', '_')));

    private static string GetBuildAttachmentColor(string status)
    {
      status = status.ToLower();
      switch (status)
      {
        case "succeeded":
          return SlackConsumerResources.PostMessageToChannelAction_BuildComplete_Color_Succeeded;
        case "failed":
          return SlackConsumerResources.PostMessageToChannelAction_BuildComplete_Color_Failed;
        default:
          return SlackConsumerResources.PostMessageToChannelAction_BuildComplete_Color_Default;
      }
    }

    private static JObject BuildAttachmentField(
      string title,
      string value,
      bool @short = true,
      bool nullIfNoValue = true)
    {
      return nullIfNoValue && string.IsNullOrEmpty(value) ? (JObject) null : JObject.FromObject((object) new
      {
        title = title,
        value = value,
        @short = @short
      });
    }

    private static string GetReleaseTriggerAttachmentValue(string triggerReason)
    {
      if (string.IsNullOrEmpty(triggerReason))
        return string.Empty;
      switch (triggerReason.ToLower())
      {
        case "none":
          return SlackConsumerResources.PostMessageToChannelAction_Release_TriggerReason_None;
        case "manual":
          return SlackConsumerResources.PostMessageToChannelAction_Release_TriggerReason_Manual;
        case "continuousintegration":
          return SlackConsumerResources.PostMessageToChannelAction_Release_TriggerReason_ContinuousIntegration;
        case "schedule":
          return SlackConsumerResources.PostMessageToChannelAction_Release_TriggerReason_Schedule;
        default:
          return string.Empty;
      }
    }

    private static string GetDeploymentStatusAttachmentValue(string status)
    {
      if (string.IsNullOrEmpty(status))
        return string.Empty;
      switch (status.ToLower())
      {
        case "succeeded":
          return SlackConsumerResources.PostMessageToChannelAction_ReleaseEvent_Field_Succeeded;
        case "partiallysucceeded":
          return SlackConsumerResources.PostMessageToChannelAction_ReleaseEvent_Field_PartiallySucceeded;
        case "rejected":
          return SlackConsumerResources.PostMessageToChannelAction_ReleaseEvent_Field_Rejected;
        case "canceled":
          return SlackConsumerResources.PostMessageToChannelAction_ReleaseEvent_Field_Canceled;
        default:
          return string.Empty;
      }
    }

    private static string GetDeploymentStatusAttachmentColor(string status)
    {
      if (string.IsNullOrEmpty(status))
        return string.Empty;
      switch (status.ToLower())
      {
        case "succeeded":
          return "good";
        case "partiallysucceeded":
          return "#FF8000";
        case "rejected":
          return "danger";
        case "canceled":
          return "danger";
        default:
          return string.Empty;
      }
    }

    private static string GetDeploymentApprovalAttachmentColor(JObject resource)
    {
      string str1 = (string) resource["approval"][(object) "approvalType"];
      string str2 = (string) resource["approval"][(object) "status"];
      if (string.IsNullOrEmpty(str2))
        return string.Empty;
      switch (str2.ToLower())
      {
        case "approved":
          return "good";
        case "pending":
          return !(str1 == "postdeploy") ? string.Empty : "#007acc";
        case "rejected":
          return "danger";
        default:
          return string.Empty;
      }
    }

    private static string GetReleaseNameAttachmentValueFromRelease(JObject resource)
    {
      string text = (string) resource["release"][(object) "name"];
      JToken url = resource["release"].SelectToken("_links")?.SelectToken("web")?.SelectToken("href");
      return url != null ? PostMessageToChannelAction.BuildSlackLink(text, (string) url) : text;
    }

    private static string GetReleaseDefinitionLinkValueFromRelease(JObject resource)
    {
      string text = (string) resource["release"][(object) "releaseDefinition"][(object) "name"];
      JToken url = resource["release"][(object) "releaseDefinition"].SelectToken("_links")?.SelectToken("web")?.SelectToken("href");
      return url != null ? PostMessageToChannelAction.BuildSlackLink(text, (string) url) : text;
    }

    private static string GetReleaseNameAttachmentValueFromEnvironmentRelease(JObject resource)
    {
      string text = (string) resource["environment"][(object) "release"][(object) "name"];
      JToken url = resource["environment"][(object) "release"].SelectToken("_links")?.SelectToken("web")?.SelectToken("href");
      return url != null ? PostMessageToChannelAction.BuildSlackLink(text, (string) url) : text;
    }

    private static string GetReleaseDefinitionLinkValueFromEnvironmentRelease(JObject resource)
    {
      string text = (string) resource["environment"][(object) "releaseDefinition"][(object) "name"];
      JToken url = resource["environment"][(object) "releaseDefinition"].SelectToken("_links")?.SelectToken("web")?.SelectToken("href");
      return url != null ? PostMessageToChannelAction.BuildSlackLink(text, (string) url) : text;
    }

    private static string GetReleaseTimeToDeployAttachmentValue(string timeInMinutes)
    {
      if (string.IsNullOrEmpty(timeInMinutes))
        return string.Empty;
      double result;
      if (!double.TryParse(timeInMinutes, NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result))
        result = 0.0;
      return TimeSpan.FromMilliseconds(result * 60.0 * 1000.0).ToString("hh\\:mm\\:ss");
    }

    private static string GetReleaseArtifactAttachmentDetails(JObject resource)
    {
      JToken source = resource["release"][(object) "artifacts"];
      if (source.Any<JToken>())
      {
        JToken jtoken1 = (JToken) null;
        foreach (JToken jtoken2 in (IEnumerable<JToken>) source)
        {
          JToken jtoken3 = jtoken2[(object) "isPrimary"];
          if (jtoken3 != null && (bool) jtoken3)
          {
            jtoken1 = jtoken2;
            break;
          }
        }
        if (jtoken1 != null && jtoken1[(object) "definitionReference"] != null && jtoken1[(object) "definitionReference"][(object) "version"] != null)
          return source.Count<JToken>() != 1 ? string.Format((IFormatProvider) CultureInfo.CurrentCulture, SlackConsumerResources.PostMessageToChannelAction_ReleaseEvent_Field_LinkedArtifactsFormat, (object) (string) jtoken1[(object) "definitionReference"][(object) "version"][(object) "name"], (object) (source.Count<JToken>() - 1)) : (string) jtoken1[(object) "definitionReference"][(object) "version"][(object) "name"];
      }
      return SlackConsumerResources.PostMessageToChannelAction_ReleaseEvent_Field_NoLinkedArtifacts;
    }

    private static string TransformInlineLinksToSlackFormat(string markdownText) => PostMessageToChannelAction.s_markdownInlineLinkRegex.Replace(markdownText, "<$2|$1>");

    private static string BuildSlackLink(string text, string url) => string.Format("<{0}|{1}>", (object) url, (object) text);

    private static string ConvertToMarkdownCode(string text) => string.Format("`{0}`", (object) text);

    private static string GetCommitShortSHA1(string commitId) => commitId.Substring(0, 7);

    private static string GetCommitShortComment(string comment)
    {
      if (string.IsNullOrEmpty(comment))
        return string.Empty;
      if (comment.Length > 80)
        comment = comment.Substring(0, 80);
      int length = comment.IndexOfAny(PostMessageToChannelAction.s_lineBreakChars);
      if (length >= 0)
        comment = comment.Substring(0, length);
      return comment;
    }
  }
}
