// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Types.BaseProjectUpdateJobWorker
// Assembly: Microsoft.TeamFoundation.Server.Types, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC707FA3-32BF-41E4-BD8A-1BB971125382
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Types.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.AuditLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.Types
{
  public abstract class BaseProjectUpdateJobWorker : ITeamFoundationJobExtension
  {
    private bool? m_skipRenameNotification;
    private bool? m_isRenameOperation;
    private const string c_area = "Project";
    private const string c_layer = "BaseProjectUpdateJobWorker";

    public TeamFoundationJobExecutionResult Run(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime,
      out string resultMessage)
    {
      this.StartTime = DateTime.UtcNow;
      this.SetJobDefinitionAndProjectOperation(jobDefinition);
      this.ProjectChanges = new List<ProjectInfo>();
      this.Diagnostics = new List<KeyValuePair<string, object>>();
      requestContext.EnsureAuditCorrelationId(this.ProjectOperation.CorrelationId);
      int num1 = this.Initialize(requestContext, this.ProjectOperation);
      int num2 = 0;
      foreach (ProjectInfo projectChange in this.ProjectChanges)
      {
        try
        {
          this.NotifySubscribers(requestContext, projectChange);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(5500340, "Project", nameof (BaseProjectUpdateJobWorker), ex);
          this.AddDiagnostic("exception", (object) ex);
          this.FinalizeOnFailure(requestContext, ex);
          resultMessage = this.BuildResultMessage(this.ProjectOperation);
          return TeamFoundationJobExecutionResult.Failed;
        }
        this.UpdateWatermark(requestContext, projectChange);
        ++num2;
      }
      long num3 = this.Finalize(requestContext);
      this.AddDiagnostic("total", (object) num1);
      this.AddDiagnostic("processed", (object) num2);
      this.AddDiagnostic("watermark", (object) num3);
      resultMessage = this.BuildResultMessage(this.ProjectOperation);
      return TeamFoundationJobExecutionResult.Succeeded;
    }

    internal void SetJobDefinitionAndProjectOperation(TeamFoundationJobDefinition jobDefinition)
    {
      this.JobDefinition = jobDefinition;
      this.ProjectOperation = TeamFoundationSerializationUtility.Deserialize<ProjectOperation>(jobDefinition.Data);
    }

    internal void NotifySubscribers(IVssRequestContext requestContext, ProjectInfo project)
    {
      object eventData = (object) null;
      requestContext.Trace(5500341, TraceLevel.Info, "Project", nameof (BaseProjectUpdateJobWorker), "Notifying about project {0} update to the revision {1}", (object) project.Id, (object) project.Revision);
      switch (project.State)
      {
        case ProjectState.WellFormed:
          string uri = project.Uri;
          string name = project.Name;
          int state = (int) project.State;
          Guid userId = project.UserId;
          DateTime lastUpdateTime = project.LastUpdateTime;
          long revision1 = project.Revision;
          long revision2 = project.Revision;
          long? revision3 = this.ProjectOperation?.Revision;
          long valueOrDefault = revision3.GetValueOrDefault();
          int num = revision2 == valueOrDefault & revision3.HasValue ? (this.ShouldInvalidateSystemStore ? 1 : 0) : 0;
          IList<ProjectProperty> properties = project.Properties;
          int visibility = (int) project.Visibility;
          eventData = (object) new ProjectUpdatedEvent(uri, name, (ProjectState) state, userId, lastUpdateTime, revision1, num != 0, properties, (ProjectVisibility) visibility)
          {
            ProjectOperationProperties = this.ProjectOperation.Properties.ToDictionary<ProjectOperationProperty, string, object>((Func<ProjectOperationProperty, string>) (x => x.Name), (Func<ProjectOperationProperty, object>) (x => x.Value))
          };
          break;
        case ProjectState.Deleted:
          eventData = (object) new ProjectDeletedEvent(project.Uri)
          {
            Name = project.Name,
            Revision = project.Revision,
            DeletedTime = project.LastUpdateTime.ToString("r", (IFormatProvider) CultureInfo.InvariantCulture),
            UserId = project.UserId
          };
          break;
      }
      if (eventData == null)
        return;
      this.PublishEvent(requestContext, eventData);
    }

    protected void AddDiagnostic(string name, object value) => this.Diagnostics.Add(new KeyValuePair<string, object>(name, value));

    protected virtual void PublishEvent(IVssRequestContext requestContext, object eventData) => requestContext.GetService<ITeamFoundationEventService>().SyncPublishNotification(requestContext, eventData);

    protected abstract int Initialize(
      IVssRequestContext requestContext,
      ProjectOperation projectOperation);

    protected abstract long Finalize(IVssRequestContext requestContext);

    protected abstract void FinalizeOnFailure(IVssRequestContext requestContext, Exception ex);

    protected abstract void UpdateWatermark(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo);

    private string BuildResultMessage(ProjectOperation projectOperation) => string.Format("ProjectUpdateWorker for [{0}, {1}], {2}", (object) projectOperation.ProjectId, (object) projectOperation.Revision, (object) string.Join(", ", this.Diagnostics.Select<KeyValuePair<string, object>, string>((Func<KeyValuePair<string, object>, string>) (d => d.Key + "=" + d.Value))));

    protected bool ShouldInvalidateSystemStore => this.ProjectOperation.GetPropertyValueWithDefault<bool>(nameof (ShouldInvalidateSystemStore), false);

    protected bool SkipRenameNotification
    {
      get
      {
        if (!this.m_skipRenameNotification.HasValue)
          this.m_skipRenameNotification = new bool?(this.ProjectOperation.GetPropertyValueWithDefault<bool>("IsAssignProject", false) || this.ProjectOperation.GetPropertyValueWithDefault<bool>("IsProjectSoftDelete", false) || this.ProjectOperation.GetPropertyValueWithDefault<bool>("IsProjectRecovery", false));
        return this.m_skipRenameNotification.Value;
      }
    }

    protected bool IsRenameOperation
    {
      get
      {
        if (!this.m_isRenameOperation.HasValue)
          this.m_isRenameOperation = new bool?(this.ProjectOperation.GetPropertyValueWithDefault<bool>("IsRename", false));
        return this.m_isRenameOperation.Value;
      }
    }

    protected DateTime StartTime { get; private set; }

    protected TeamFoundationJobDefinition JobDefinition { get; private set; }

    protected ProjectOperation ProjectOperation { get; private set; }

    protected List<ProjectInfo> ProjectChanges { get; private set; }

    protected List<KeyValuePair<string, object>> Diagnostics { get; private set; }
  }
}
