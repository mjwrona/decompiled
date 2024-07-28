// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Actions.ReindexAccountAction
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Actions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DE7D0F19-C193-43CC-9602-3C8794FE9CA0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Actions.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Constants;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Api;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManagerAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Actions
{
  public class ReindexAccountAction : AbstractAction
  {
    public ReindexAccountAction(ActionType actionType, ActionContext actionContext)
      : base(Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.DataAccessFactory.GetInstance(), actionType, actionContext)
    {
    }

    internal ReindexAccountAction(
      IDataAccessFactory dataAccessFactory,
      ActionType healthAction,
      ActionContext actionContext)
      : base(dataAccessFactory, healthAction, actionContext)
    {
    }

    public override bool IsLongRunning() => true;

    public override bool IsCompleted(IVssRequestContext requestContext)
    {
      List<Guid> collectionIds = this.ActionContext.CollectionIds;
      if ((collectionIds != null ? (!collectionIds.Any<Guid>() ? 1 : 0) : 1) != 0)
        return true;
      List<ReindexingStatusEntry> reindexingStatus = this.GetReindexingStatus(IVssRequestContextExtensions.ElevateAsNeeded(requestContext), this.ActionContext.CollectionIds, this.ActionContext.EntityType, this.DataAccessFactory.GetReindexingStatusDataAccess());
      List<ReindexingStatusEntry> list1 = reindexingStatus.Where<ReindexingStatusEntry>((Func<ReindexingStatusEntry, bool>) (it => it.IsReindexingInProgress())).ToList<ReindexingStatusEntry>();
      List<ReindexingStatusEntry> list2 = reindexingStatus.Where<ReindexingStatusEntry>((Func<ReindexingStatusEntry, bool>) (it => it.Status == Microsoft.VisualStudio.Services.Search.Common.Enums.ReindexingStatus.Failed)).ToList<ReindexingStatusEntry>();
      if (list2.Any<ReindexingStatusEntry>())
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1083059, "Indexing Pipeline", "IndexingOperation", "Account migration failed for :" + list2.Select<ReindexingStatusEntry, string>((Func<ReindexingStatusEntry, string>) (it => it.CollectionId.ToString())).Aggregate<string>((Func<string, string, string>) ((i, j) => i + "," + j)));
      Dictionary<Microsoft.VisualStudio.Services.Search.Common.Enums.ReindexingStatus, List<ReindexingStatusEntry>> dictionary = list1.GroupBy<ReindexingStatusEntry, Microsoft.VisualStudio.Services.Search.Common.Enums.ReindexingStatus>((Func<ReindexingStatusEntry, Microsoft.VisualStudio.Services.Search.Common.Enums.ReindexingStatus>) (it => it.Status)).ToDictionary<IGrouping<Microsoft.VisualStudio.Services.Search.Common.Enums.ReindexingStatus, ReindexingStatusEntry>, Microsoft.VisualStudio.Services.Search.Common.Enums.ReindexingStatus, List<ReindexingStatusEntry>>((Func<IGrouping<Microsoft.VisualStudio.Services.Search.Common.Enums.ReindexingStatus, ReindexingStatusEntry>, Microsoft.VisualStudio.Services.Search.Common.Enums.ReindexingStatus>) (statusGroup => statusGroup.Key), (Func<IGrouping<Microsoft.VisualStudio.Services.Search.Common.Enums.ReindexingStatus, ReindexingStatusEntry>, List<ReindexingStatusEntry>>) (statusGroup => statusGroup.ToList<ReindexingStatusEntry>()));
      foreach (KeyValuePair<Microsoft.VisualStudio.Services.Search.Common.Enums.ReindexingStatus, List<ReindexingStatusEntry>> keyValuePair in dictionary)
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1083059, "Indexing Pipeline", "IndexingOperation", FormattableString.Invariant(FormattableStringFactory.Create("Status: {0} , Number of Collections in this state : {1}", (object) keyValuePair.Key.ToString(), (object) dictionary[keyValuePair.Key])));
      return list1.Count == 0;
    }

    public override void Invoke(IVssRequestContext requestContext, out string resultMessage)
    {
      IVssRequestContext vssRequestContext = IVssRequestContextExtensions.ElevateAsNeeded(requestContext);
      IReindexingStatusDataAccess statusDataAccess = this.DataAccessFactory.GetReindexingStatusDataAccess();
      List<ReindexingStatusEntry> list = this.GetReindexingStatus(vssRequestContext, this.ActionContext.CollectionIds, this.ActionContext.EntityType, statusDataAccess).Where<ReindexingStatusEntry>((Func<ReindexingStatusEntry, bool>) (it => it.Status == Microsoft.VisualStudio.Services.Search.Common.Enums.ReindexingStatus.Completed || it.Status == Microsoft.VisualStudio.Services.Search.Common.Enums.ReindexingStatus.NotRequired || it.Status == Microsoft.VisualStudio.Services.Search.Common.Enums.ReindexingStatus.Failed)).Select<ReindexingStatusEntry, Guid>((Func<ReindexingStatusEntry, Guid>) (it => it.CollectionId)).Select<Guid, ReindexingStatusEntry>((Func<Guid, ReindexingStatusEntry>) (it => new ReindexingStatusEntry(it, this.ActionContext.EntityType)
      {
        Priority = (short) 0,
        Status = Microsoft.VisualStudio.Services.Search.Common.Enums.ReindexingStatus.ReindexingRequired
      })).ToList<ReindexingStatusEntry>();
      statusDataAccess.AddOrUpdateReindexingStatusEntries(vssRequestContext, list);
      vssRequestContext.QueueDelayedNamedJob(JobConstants.TriggerAndMonitorReindexingJob, 30, JobPriorityLevel.Normal);
      resultMessage = FormattableString.Invariant(FormattableStringFactory.Create("Queued reindexing for {0} collections", (object) list.Count));
    }

    private List<ReindexingStatusEntry> GetReindexingStatus(
      IVssRequestContext deploymentContext,
      List<Guid> collections,
      IEntityType entityType,
      IReindexingStatusDataAccess reindexingStatusDataAccess)
    {
      List<KeyValuePair<Guid, IEntityType>> list = collections.Select<Guid, KeyValuePair<Guid, IEntityType>>((Func<Guid, KeyValuePair<Guid, IEntityType>>) (it => new KeyValuePair<Guid, IEntityType>(it, entityType))).ToList<KeyValuePair<Guid, IEntityType>>();
      return reindexingStatusDataAccess.GetReindexingStatusEntries(deploymentContext, list);
    }
  }
}
