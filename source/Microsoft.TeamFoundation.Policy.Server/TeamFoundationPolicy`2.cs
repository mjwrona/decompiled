// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Policy.Server.TeamFoundationPolicy`2
// Assembly: Microsoft.TeamFoundation.Policy.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C7B03386-B27B-4823-BB4F-89F7D7E42DDD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Policy.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.Policy.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.TeamFoundation.Policy.Server
{
  public abstract class TeamFoundationPolicy<TSettings, TContext> : ITeamFoundationPolicy where TContext : TeamFoundationPolicyEvaluationRecordContext
  {
    private string m_teamProjectUri;

    public abstract string Area { get; }

    public abstract string Description { get; }

    public abstract Guid Id { get; }

    public abstract string DisplayName { get; }

    public abstract bool IsBypassable { get; }

    public virtual bool IsHidden => false;

    public virtual bool AllowSyncPublishNotification => false;

    public PolicyConfigurationRecord Configuration { get; private set; }

    public string TeamProjectUri
    {
      get
      {
        if (string.IsNullOrEmpty(this.m_teamProjectUri))
          this.m_teamProjectUri = LinkingUtilities.EncodeUri(new ArtifactId("Classification", "TeamProject", this.Configuration.ProjectId.ToString()));
        return this.m_teamProjectUri;
      }
    }

    public TSettings Settings { get; private set; }

    string[] ITeamFoundationPolicy.GetScopes(object settings) => this.GetScopes((TSettings) settings);

    public abstract string[] GetScopes(TSettings settings);

    public abstract bool IsApplicableTo(
      IVssRequestContext requestContext,
      ITeamFoundationPolicyTarget target);

    protected virtual void Initialize(IVssRequestContext requestContext)
    {
    }

    public TSettings DeserializeSettings(string settings) => JsonConvert.DeserializeObject<TSettings>(settings);

    object ITeamFoundationPolicy.DeserializeSettings(string settingsString) => (object) this.DeserializeSettings(settingsString);

    public virtual bool CheckSettingsValidity(
      IVssRequestContext requestContext,
      Guid projectId,
      TSettings settings,
      out string errorMessage)
    {
      errorMessage = (string) null;
      return true;
    }

    void ITeamFoundationPolicy.Initialize(
      IVssRequestContext requestContext,
      PolicyConfigurationRecord configuration,
      object settings)
    {
      this.Initialize(requestContext, configuration, (TSettings) settings);
    }

    public void Initialize(
      IVssRequestContext requestContext,
      PolicyConfigurationRecord configuration,
      TSettings settings)
    {
      this.Configuration = configuration;
      this.Settings = settings;
      this.Initialize(requestContext);
    }

    bool ITeamFoundationPolicy.CheckSettingsValidity(
      IVssRequestContext requestContext,
      Guid projectId,
      object settingsObject,
      out string errorMessage)
    {
      return this.CheckSettingsValidity(requestContext, projectId, (TSettings) settingsObject, out errorMessage);
    }

    public virtual TeamFoundationPolicyEvaluationRecordContext ParseContext(
      IVssRequestContext requestContext,
      Guid projectId,
      JObject rawContext)
    {
      return (TeamFoundationPolicyEvaluationRecordContext) rawContext.ToObject<TContext>();
    }

    void ITeamFoundationPolicy.AppendClientTraceData(object settings, ref ClientTraceData eventData)
    {
      if (settings == null)
        return;
      this.AppendClientTraceData((TSettings) settings, ref eventData);
    }

    public virtual void AppendClientTraceData(TSettings settings, ref ClientTraceData eventData)
    {
    }

    protected PolicyNotificationResult<TContext> LogAndReturnPolicyResult(
      PolicyNotificationResult<TContext> result)
    {
      if (result.Status == PolicyEvaluationStatus.Rejected)
        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Policy.Server.PerfCountersInstance_PolicyEvaluationFailuresPerSec", this.Id.ToString()).Increment();
      return result;
    }

    public virtual PolicyNotificationResult Requeue(
      IVssRequestContext requestContext,
      PolicyEvaluationStatus? existingStatus,
      TeamFoundationPolicyEvaluationRecordContext existingContext,
      ITeamFoundationPolicyTarget target)
    {
      return (PolicyNotificationResult) null;
    }

    protected bool IsInitialized => (object) this.Settings != null;

    void ITeamFoundationPolicy.CheckEditPoliciesPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      object settingsObject)
    {
      this.CheckEditPoliciesPermission(requestContext, projectId, (TSettings) settingsObject);
    }

    public abstract void CheckEditPoliciesPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      TSettings settings);

    public void CheckSupportedMatchKind(IVssRequestContext requestContext, object settingsObject) => this.CheckSupportedMatchKind(requestContext, (TSettings) settingsObject);

    public virtual void CheckSupportedMatchKind(
      IVssRequestContext requestContext,
      TSettings settings)
    {
    }
  }
}
