// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Slack.SlackConsumerResources
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Slack
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  public class SlackConsumerResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal SlackConsumerResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static ResourceManager ResourceManager
    {
      get
      {
        if (SlackConsumerResources.resourceMan == null)
          SlackConsumerResources.resourceMan = new ResourceManager("Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Slack.SlackConsumerResources", typeof (SlackConsumerResources).Assembly);
        return SlackConsumerResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static CultureInfo Culture
    {
      get => SlackConsumerResources.resourceCulture;
      set => SlackConsumerResources.resourceCulture = value;
    }

    public static string ConsumerDescription => SlackConsumerResources.ResourceManager.GetString(nameof (ConsumerDescription), SlackConsumerResources.resourceCulture);

    public static string ConsumerName => SlackConsumerResources.ResourceManager.GetString(nameof (ConsumerName), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelAction_BuildComplete_Color_Default => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelAction_BuildComplete_Color_Default), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelAction_BuildComplete_Color_Failed => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelAction_BuildComplete_Color_Failed), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelAction_BuildComplete_Color_Succeeded => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelAction_BuildComplete_Color_Succeeded), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelAction_BuildComplete_Field_Definition => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelAction_BuildComplete_Field_Definition), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelAction_BuildComplete_Field_Duration => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelAction_BuildComplete_Field_Duration), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelAction_BuildComplete_Field_RequestedBy => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelAction_BuildComplete_Field_RequestedBy), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelAction_CodeCheckedIn_Field_Comment => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelAction_CodeCheckedIn_Field_Comment), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelAction_GitPullRequest_Field_Reviewers => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelAction_GitPullRequest_Field_Reviewers), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelAction_GitPullRequest_Field_Reviewers_Format => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelAction_GitPullRequest_Field_Reviewers_Format), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelAction_GitPullRequest_Field_Reviewers_UniqueName_Format => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelAction_GitPullRequest_Field_Reviewers_UniqueName_Format), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelAction_GitPullRequestCreated_Field_Description => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelAction_GitPullRequestCreated_Field_Description), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelAction_GitPullRequestCreated_Field_MergeStatus => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelAction_GitPullRequestCreated_Field_MergeStatus), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelAction_GitPullRequestCreated_Field_Source => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelAction_GitPullRequestCreated_Field_Source), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelAction_GitPullRequestCreated_Field_SourceRepo => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelAction_GitPullRequestCreated_Field_SourceRepo), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelAction_GitPullRequestCreated_Field_Status => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelAction_GitPullRequestCreated_Field_Status), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelAction_GitPullRequestCreated_Field_Target => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelAction_GitPullRequestCreated_Field_Target), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelAction_GitPush_Field_Commit_Format => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelAction_GitPush_Field_Commit_Format), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelAction_GitPush_Field_Commits => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelAction_GitPush_Field_Commits), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelAction_GitPush_Field_Commits_Limit_Indication => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelAction_GitPush_Field_Commits_Limit_Indication), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelAction_MessagePosted_Field_Message => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelAction_MessagePosted_Field_Message), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelAction_Release_TriggerReason_ContinuousIntegration => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelAction_Release_TriggerReason_ContinuousIntegration), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelAction_Release_TriggerReason_Manual => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelAction_Release_TriggerReason_Manual), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelAction_Release_TriggerReason_None => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelAction_Release_TriggerReason_None), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelAction_Release_TriggerReason_Schedule => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelAction_Release_TriggerReason_Schedule), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelAction_ReleaseEvent_Field_AbandonedBy => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelAction_ReleaseEvent_Field_AbandonedBy), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelAction_ReleaseEvent_Field_AbandonedOn => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelAction_ReleaseEvent_Field_AbandonedOn), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelAction_ReleaseEvent_Field_ApprovedBy => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelAction_ReleaseEvent_Field_ApprovedBy), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelAction_ReleaseEvent_Field_Build => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelAction_ReleaseEvent_Field_Build), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelAction_ReleaseEvent_Field_Canceled => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelAction_ReleaseEvent_Field_Canceled), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelAction_ReleaseEvent_Field_CreatedOn => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelAction_ReleaseEvent_Field_CreatedOn), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelAction_ReleaseEvent_Field_DeploymentStatus => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelAction_ReleaseEvent_Field_DeploymentStatus), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelAction_ReleaseEvent_Field_LinkedArtifactsFormat => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelAction_ReleaseEvent_Field_LinkedArtifactsFormat), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelAction_ReleaseEvent_Field_NoLinkedArtifacts => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelAction_ReleaseEvent_Field_NoLinkedArtifacts), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelAction_ReleaseEvent_Field_PartiallySucceeded => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelAction_ReleaseEvent_Field_PartiallySucceeded), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelAction_ReleaseEvent_Field_PendingOn => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelAction_ReleaseEvent_Field_PendingOn), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelAction_ReleaseEvent_Field_PendingSince => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelAction_ReleaseEvent_Field_PendingSince), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelAction_ReleaseEvent_Field_Rejected => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelAction_ReleaseEvent_Field_Rejected), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelAction_ReleaseEvent_Field_RejectedBy => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelAction_ReleaseEvent_Field_RejectedBy), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelAction_ReleaseEvent_Field_Release => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelAction_ReleaseEvent_Field_Release), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelAction_ReleaseEvent_Field_ReleaseDefinition => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelAction_ReleaseEvent_Field_ReleaseDefinition), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelAction_ReleaseEvent_Field_RequestedBy => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelAction_ReleaseEvent_Field_RequestedBy), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelAction_ReleaseEvent_Field_Succeeded => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelAction_ReleaseEvent_Field_Succeeded), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelAction_ReleaseEvent_Field_TimeToDeploy => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelAction_ReleaseEvent_Field_TimeToDeploy), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelAction_ReleaseEvent_Field_TriggerReason => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelAction_ReleaseEvent_Field_TriggerReason), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelAction_UrlInputDescription => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelAction_UrlInputDescription), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelAction_UrlInputName => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelAction_UrlInputName), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelAction_VsoUsername => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelAction_VsoUsername), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelAction_WorkItemCommented_Field_Comment => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelAction_WorkItemCommented_Field_Comment), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelAction_WorkItemEvent_Field_Microsoft_VSTS_Common_Priority => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelAction_WorkItemEvent_Field_Microsoft_VSTS_Common_Priority), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelAction_WorkItemEvent_Field_Microsoft_VSTS_Common_Severity => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelAction_WorkItemEvent_Field_Microsoft_VSTS_Common_Severity), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelAction_WorkItemEvent_Field_System_AreaPath => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelAction_WorkItemEvent_Field_System_AreaPath), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelAction_WorkItemEvent_Field_System_AssignedTo => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelAction_WorkItemEvent_Field_System_AssignedTo), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelAction_WorkItemEvent_Field_System_IterationPath => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelAction_WorkItemEvent_Field_System_IterationPath), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelAction_WorkItemEvent_Field_System_Reason => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelAction_WorkItemEvent_Field_System_Reason), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelAction_WorkItemEvent_Field_System_State => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelAction_WorkItemEvent_Field_System_State), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelAction_WorkItemUpdated_Field_New_Format => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelAction_WorkItemUpdated_Field_New_Format), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelAction_WorkItemUpdated_Field_Old_Format => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelAction_WorkItemUpdated_Field_Old_Format), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelActionDescription => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelActionDescription), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelActionDetailedDescription => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelActionDetailedDescription), SlackConsumerResources.resourceCulture);

    public static string PostMessageToChannelActionName => SlackConsumerResources.ResourceManager.GetString(nameof (PostMessageToChannelActionName), SlackConsumerResources.resourceCulture);
  }
}
