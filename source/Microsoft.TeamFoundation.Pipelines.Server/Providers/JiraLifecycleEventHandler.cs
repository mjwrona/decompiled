// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.Providers.JiraLifecycleEventHandler
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Microsoft.TeamFoundation.Pipelines.Server.Providers
{
  internal class JiraLifecycleEventHandler : IPipelineEventHandler
  {
    private const string c_layer = "JiraLifecycleEventHandler";

    public bool HandleEvent(
      IVssRequestContext requestContext,
      IPipelineSourceProvider provider,
      string jsonPayload,
      IDictionary<string, string> headers)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(jsonPayload, nameof (jsonPayload));
      JiraLifecycleEventData jiraEventData = JsonUtilities.Deserialize<JiraLifecycleEventData>(jsonPayload);
      ArgumentUtility.CheckStringForNullOrWhiteSpace(jiraEventData.BaseUrl, "BaseUrl");
      ArgumentUtility.CheckIsValidURI(jiraEventData.BaseUrl, UriKind.Absolute, "BaseUrl");
      switch (jiraEventData.EventType)
      {
        case "installed":
          this.HandleInstallationEvent(requestContext, jiraEventData);
          break;
        case "uninstalled":
          this.HandleUninstallEvent(requestContext, jiraEventData);
          break;
        case "deleteproject":
          this.HandleDeleteProjectEvent(requestContext, jiraEventData);
          break;
        case "deleteaccount":
          this.HandleDeleteAccountEvent(requestContext, jiraEventData);
          break;
        default:
          return false;
      }
      return true;
    }

    private void HandleInstallationEvent(
      IVssRequestContext requestContext,
      JiraLifecycleEventData jiraEventData)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(jiraEventData.ClientKey, "ClientKey");
      ArgumentUtility.CheckStringForNullOrWhiteSpace(jiraEventData.SharedSecret, "SharedSecret");
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return;
      string jiraAccountName = JiraHelper.GetJiraAccountName(jiraEventData.BaseUrl);
      requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.HandleEvent, nameof (JiraLifecycleEventHandler), "{0}- handling jira {1} event for jira account {2}", (object) nameof (HandleInstallationEvent), (object) jiraEventData.EventType, (object) jiraAccountName);
      JiraInstallationData toSerialize = new JiraInstallationData()
      {
        ClientKey = jiraEventData.ClientKey,
        SharedSecret = jiraEventData.SharedSecret
      };
      string key = string.Format("{0}-backup", (object) jiraAccountName);
      JiraHelper.SetSecertInKeyVault(requestContext, key, JsonUtility.ToString((object) toSerialize));
      requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.HandleEvent, nameof (JiraLifecycleEventHandler), "{0}- secrets stored in key vault.", (object) nameof (HandleInstallationEvent));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      requestContext.GetService<ITeamFoundationTaskService>().AddTask(requestContext, new TeamFoundationTask(JiraLifecycleEventHandler.\u003C\u003EO.\u003C0\u003E__ValidateInstallationData ?? (JiraLifecycleEventHandler.\u003C\u003EO.\u003C0\u003E__ValidateInstallationData = new TeamFoundationTaskCallback(JiraHelper.ValidateInstallationData)), (object) jiraEventData.BaseUrl, 0));
    }

    private void HandleUninstallEvent(
      IVssRequestContext requestContext,
      JiraLifecycleEventData jiraEventData)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return;
      requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.HandleEvent, nameof (JiraLifecycleEventHandler), "{0}- handling jira {1} event for jira account {2}", (object) nameof (HandleUninstallEvent), (object) jiraEventData.EventType, (object) jiraEventData.BaseUrl);
      ArgumentUtility.CheckStringForNullOrWhiteSpace(jiraEventData.ClientKey, "ClientKey");
      XmlNode xml = TeamFoundationSerializationUtility.SerializeToXml((object) jiraEventData);
      requestContext.GetService<ITeamFoundationJobService>().QueueOneTimeJob(requestContext.Elevate(), "OnJiraAppUninstall", "Microsoft.TeamFoundation.Pipelines.Server.Extensions.JiraAppUninstallHandlerJobExtension", xml, true);
    }

    private void HandleDeleteProjectEvent(
      IVssRequestContext requestContext,
      JiraLifecycleEventData jiraEventData)
    {
      ArgumentUtility.CheckForEmptyGuid(jiraEventData.HostId, "HostId");
      ArgumentUtility.CheckForEmptyGuid(jiraEventData.ProjectId, "ProjectId");
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
      {
        requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.HandleEvent, nameof (JiraLifecycleEventHandler), "{0}- Ignoring jira {1} event at the deployment level.", (object) nameof (HandleDeleteProjectEvent), (object) jiraEventData.EventType);
      }
      else
      {
        requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.HandleEvent, nameof (JiraLifecycleEventHandler), "{0}- handling jira {1} event for jira account {2}", (object) nameof (HandleDeleteProjectEvent), (object) jiraEventData.EventType, (object) jiraEventData.BaseUrl);
        JiraHelper.DeleteProjectConnection(requestContext, jiraEventData.HostId, jiraEventData.ProjectId, jiraEventData.BaseUrl);
      }
    }

    private void HandleDeleteAccountEvent(
      IVssRequestContext requestContext,
      JiraLifecycleEventData jiraEventData)
    {
      ArgumentUtility.CheckForEmptyGuid(jiraEventData.HostId, "HostId");
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
      {
        requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.HandleEvent, nameof (JiraLifecycleEventHandler), "{0}- Ignoring jira {1} event at the deployment level.", (object) nameof (HandleDeleteAccountEvent), (object) jiraEventData.EventType);
      }
      else
      {
        requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.HandleEvent, nameof (JiraLifecycleEventHandler), "{0}- handling jira {1} event for jira account {2}", (object) nameof (HandleDeleteAccountEvent), (object) jiraEventData.EventType, (object) jiraEventData.BaseUrl);
        JiraHelper.DeleteLinkedJiraConfiguration(requestContext, jiraEventData);
      }
    }
  }
}
