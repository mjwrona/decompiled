// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.Providers.JiraAccountNameRouter
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.ExternalIntegration.HostIdMapping;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Diagnostics;
using System.Web;

namespace Microsoft.TeamFoundation.Pipelines.Server.Providers
{
  public class JiraAccountNameRouter : IHostIdMappingRouter
  {
    private const int c_maxPostRequestSize = 5242880;

    protected virtual string Layer => nameof (JiraAccountNameRouter);

    protected virtual string Area => TracingPoints.ExternalServiceEventUrlHostRoutingArea;

    protected virtual string QualifierName => (string) null;

    public bool OverrideOnDeletedOrganization(IVssRequestContext requestContext) => false;

    public bool TryExtractMappingData(
      IVssRequestContext requestContext,
      HttpRequest request,
      out HostIdMappingData mappingData)
    {
      string requestBody = request.ReadBodyAsString(5242880);
      return this.TryExtractMappingData(requestContext, requestBody, out mappingData);
    }

    public bool TryExtractMappingData(
      IVssRequestContext requestContext,
      string requestBody,
      out HostIdMappingData mappingData)
    {
      JiraLifecycleEventData payload;
      string jiraAccountName;
      try
      {
        payload = JsonUtilities.Deserialize<JiraLifecycleEventData>(requestBody);
        jiraAccountName = JiraHelper.GetJiraAccountName(payload.BaseUrl);
      }
      catch
      {
        mappingData = (HostIdMappingData) null;
        return false;
      }
      if (string.IsNullOrWhiteSpace(jiraAccountName) || payload.HostId == Guid.Empty || !this.IsHostRoutingRequired(payload))
      {
        requestContext.Trace(TracingPoints.EventsRouting.HostRoutingNotRequired, TraceLevel.Info, this.Area, this.Layer, "TryExtractMappingData - jiraconnectapp");
        mappingData = (HostIdMappingData) null;
        return false;
      }
      mappingData = JiraHelper.GetHostIdMappingData(jiraAccountName, payload.HostId.ToString());
      return true;
    }

    public HostIdMappingData GetMappingData(
      IVssRequestContext requestContext,
      string jiraAccountName,
      string qualifier)
    {
      return JiraHelper.GetHostIdMappingData(jiraAccountName, qualifier);
    }

    public void AddHostIdMapping(
      IVssRequestContext requestContext,
      string providerId,
      string jiraAccountName,
      string qualifier,
      bool overrideExisting = false)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(jiraAccountName, nameof (jiraAccountName));
      Guid instanceId = requestContext.ServiceHost.InstanceId;
      HostIdMappingData hostIdMappingData = JiraHelper.GetHostIdMappingData(jiraAccountName, instanceId.ToString());
      requestContext.GetService<IHostIdMappingService>().AddHostIdMapping(requestContext, providerId, hostIdMappingData, instanceId, true);
    }

    public void RemoveHostIdMapping(
      IVssRequestContext requestContext,
      string providerId,
      string jiraAccountName,
      string qualifier)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(jiraAccountName, nameof (jiraAccountName));
      HostIdMappingData hostIdMappingData = JiraHelper.GetHostIdMappingData(jiraAccountName, qualifier);
      if (string.IsNullOrEmpty(qualifier))
        requestContext.GetService<IHostIdMappingService>().RemoveHostIdMappings(requestContext, providerId, hostIdMappingData);
      else
        requestContext.GetService<IHostIdMappingService>().RemoveHostIdMapping(requestContext, providerId, hostIdMappingData);
    }

    private bool IsHostRoutingRequired(JiraLifecycleEventData payload) => string.Equals(payload.EventType, "deleteproject", StringComparison.OrdinalIgnoreCase) || string.Equals(payload.EventType, "deleteaccount", StringComparison.OrdinalIgnoreCase);
  }
}
