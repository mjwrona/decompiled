// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeExtensionService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Predicates;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Events;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class WorkItemTypeExtensionService : 
    BaseTeamFoundationWorkItemTrackingService,
    IWorkItemTypeExtensionService,
    IVssFrameworkService,
    IDisposable
  {
    private const int c_ReconcileBatchSize = 200;
    private ReaderWriterLock m_cacheGate = new ReaderWriterLock();
    private Dictionary<Guid, WorkItemTypeExtension> m_extensionCache = new Dictionary<Guid, WorkItemTypeExtension>();
    private Dictionary<string, WorkItemTypeExtensionService.ExtensionsSnapshot> m_snapshots = new Dictionary<string, WorkItemTypeExtensionService.ExtensionsSnapshot>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    private void SetReconciliationWatermarks(
      IVssRequestContext requestContext,
      IEnumerable<WorkItemTypeExtension> extensions,
      Guid reconciliationWatermark)
    {
      using (WorkItemTypeExtensionComponent component = WorkItemTypeExtensionComponent.CreateComponent(requestContext))
        component.SetReconciliationWatermarks((IEnumerable<Guid>) extensions.Select<WorkItemTypeExtension, Guid>((Func<WorkItemTypeExtension, Guid>) (e => e.Id)).ToList<Guid>(), reconciliationWatermark);
      foreach (WorkItemTypeExtension extension in extensions)
      {
        extension.UpdateReconciliationWatermark(reconciliationWatermark);
        extension.UpdateReconciliationStatus(WorkItemTypeExtensionReconciliationStatus.Pending);
      }
    }

    private Guid GetReconciliationWatermark(
      IVssRequestContext requestContext,
      WorkItemTypeExtension extension)
    {
      Guid reconciliationWatermark;
      using (WorkItemTypeExtensionComponent component = WorkItemTypeExtensionComponent.CreateComponent(requestContext))
        reconciliationWatermark = component.GetReconciliationWatermark(extension.Id);
      extension.UpdateReconciliationWatermark(reconciliationWatermark);
      return reconciliationWatermark;
    }

    public ReconcileRequestResult ReconcileExtension(
      IVssRequestContext requestContext,
      WorkItemTypeExtension extension,
      int reconcileTimeout,
      bool skipWITChangeDateUpdate)
    {
      return this.ReconcileExtension(requestContext, extension, (IEnumerable<WorkItemFieldRule>) null, reconcileTimeout, skipWITChangeDateUpdate);
    }

    public ReconcileRequestResult ReconcileExtension(
      IVssRequestContext requestContext,
      WorkItemTypeExtension extension,
      IEnumerable<WorkItemFieldRule> reconciliationScopeRules,
      int reconcileTimeout,
      bool skipWITChangeDateUpdate = false)
    {
      return requestContext.TraceBlock<ReconcileRequestResult>(901270, 901274, 901273, "WorkItemTypeExtensions", "WorkItemTypeExtensionsService", nameof (ReconcileExtension), (Func<ReconcileRequestResult>) (() =>
      {
        Guid reconciliationWatermark = Guid.NewGuid();
        bool flag1 = true;
        ReconcileRequestResult reconcileRequestResult = ReconcileRequestResult.AsyncTaskQueued;
        bool isCancelableReconciliation = reconciliationScopeRules == null;
        IEnumerable<WorkItemFieldRule> workItemFieldRules = extension.FieldRules ?? Enumerable.Empty<WorkItemFieldRule>();
        if (reconciliationScopeRules != null)
        {
          List<WorkItemFieldRule> list = reconciliationScopeRules.Concat<WorkItemFieldRule>(workItemFieldRules).ToList<WorkItemFieldRule>();
          WorkItemTypeExtensionService.FixFieldRules(requestContext, extension, (IEnumerable<WorkItemFieldRule>) list);
          workItemFieldRules = (IEnumerable<WorkItemFieldRule>) RuleEngine.GroupAndMergeFieldRules((IEnumerable<WorkItemFieldRule>) list).ToList<WorkItemFieldRule>();
        }
        bool flag2 = reconcileTimeout != 0;
        if (flag2)
        {
          this.SetReconciliationWatermarks(requestContext, (IEnumerable<WorkItemTypeExtension>) new WorkItemTypeExtension[1]
          {
            extension
          }, reconciliationWatermark);
          long allowedTime = 0;
          allowedTime = reconcileTimeout != -1 ? (long) reconcileTimeout : long.MaxValue;
          Stopwatch syncTimeoutStopWatch = Stopwatch.StartNew();
          bool cancelationRequested = false;
          Func<bool> isCanceled = (Func<bool>) (() =>
          {
            if (allowedTime < syncTimeoutStopWatch.ElapsedMilliseconds)
              return true;
            if (!isCancelableReconciliation || !this.IsReconciliationWatermarkChanged(requestContext, extension, reconciliationWatermark))
              return false;
            cancelationRequested = true;
            return true;
          });
          reconcileRequestResult = ReconcileRequestResult.AsyncTaskQueuedAfterSynchronousTry;
          ILeaseInfo leaseInfo;
          if (CrossProcessLeaseFactory.TryAcquireLeaseForWorkItemTypeExtensionReconciliation(requestContext, extension.Id, TimeSpan.FromMilliseconds((double) reconcileTimeout), out leaseInfo))
          {
            using (leaseInfo)
            {
              try
              {
                IdentityDescriptor userContext = requestContext.UserContext;
                WorkItemTypeExtensionsReconciliationTelemetryParams reconciliationTelemetryParams = this.TryReconcileExtension(requestContext.Elevate().WitContext(), extension, workItemFieldRules, isCanceled, userContext, leaseInfo, skipWITChangeDateUpdate);
                if (reconciliationTelemetryParams.ReconcileResult)
                {
                  flag1 = false;
                  if (!this.IsReconciliationWatermarkChanged(requestContext, extension, reconciliationWatermark))
                  {
                    this.UpdateReconciliationStatus(requestContext, extension, WorkItemTypeExtensionReconciliationStatus.Reconciled, reconciliationTelemetryParams.QueryLimitExceededFallback ? "WARNING: The initial reconcilitation query exceeded the configured limit. Not all work items matching the predicate have been reconciled" : (string) null);
                    reconcileRequestResult = ReconcileRequestResult.CompletedSynchronously;
                  }
                  else
                    reconcileRequestResult = ReconcileRequestResult.Canceled;
                }
                else if (cancelationRequested)
                {
                  flag1 = false;
                  reconcileRequestResult = ReconcileRequestResult.Canceled;
                }
              }
              catch (Exception ex)
              {
                this.UpdateReconciliationStatus(requestContext, extension, WorkItemTypeExtensionReconciliationStatus.Error, ex.ToString());
                reconcileRequestResult = ReconcileRequestResult.Error;
              }
            }
          }
        }
        if (flag1)
          this.QueueReconcileExtensionsJob(requestContext, extension, workItemFieldRules, isCancelableReconciliation, reconciliationWatermark, flag2 ? JobPriorityLevel.Highest : JobPriorityLevel.Normal, skipWITChangeDateUpdate);
        return reconcileRequestResult;
      }));
    }

    internal bool IsReconciliationWatermarkChanged(
      IVssRequestContext requestContext,
      WorkItemTypeExtension extension,
      Guid oldReconciliationWatermark)
    {
      return this.GetReconciliationWatermark(requestContext, extension) != oldReconciliationWatermark;
    }

    internal void UpdateReconciliationStatus(
      IVssRequestContext requestContext,
      WorkItemTypeExtension extension,
      WorkItemTypeExtensionReconciliationStatus reconciliationStatus,
      string reconciliationMessage)
    {
      bool everReconciled;
      using (WorkItemTypeExtensionComponent component = WorkItemTypeExtensionComponent.CreateComponent(requestContext))
        component.UpdateExtensionReconciliationStatus(extension.Id, reconciliationStatus, reconciliationMessage, out everReconciled);
      extension.UpdateReconciliationStatus(reconciliationStatus);
      if (everReconciled || reconciliationStatus != WorkItemTypeExtensionReconciliationStatus.Reconciled)
        return;
      WorkItemTrendService service = requestContext.GetService<WorkItemTrendService>();
      foreach (FieldEntry fieldEntry in extension.Fields.Select<WorkItemTypeExtensionFieldEntry, FieldEntry>((Func<WorkItemTypeExtensionFieldEntry, FieldEntry>) (f => f.Field)).Where<FieldEntry>((Func<FieldEntry, bool>) (f => f.IsUsedInTrendData)))
        service.StampTrendDataBaseline(requestContext, fieldEntry.ReferenceName);
    }

    public WorkItemTypeExtensionReconciliationStatus GetReconciliationStatus(
      IVssRequestContext requestContext,
      WorkItemTypeExtension extension,
      out bool everReconciled)
    {
      using (WorkItemTypeExtensionComponent component = WorkItemTypeExtensionComponent.CreateComponent(requestContext))
      {
        WorkItemTypeExtensionReconciliationStatus reconciliationStatus = component.GetReconciliationStatus(extension.Id, out everReconciled);
        extension.UpdateReconciliationStatus(reconciliationStatus);
        return reconciliationStatus;
      }
    }

    private void QueueReconcileExtensionsJob(
      IVssRequestContext requestContext,
      WorkItemTypeExtension extension,
      IEnumerable<WorkItemFieldRule> reconciliationRules,
      bool isCancelable,
      Guid watermark,
      JobPriorityLevel jobPriority = JobPriorityLevel.Normal,
      bool skipWITChangeDateUpdate = false)
    {
      this.SetReconciliationWatermarks(requestContext, (IEnumerable<WorkItemTypeExtension>) new WorkItemTypeExtension[1]
      {
        extension
      }, watermark);
      IEnumerable<WorkItemFieldRule> source = reconciliationRules ?? Enumerable.Empty<WorkItemFieldRule>();
      WorkItemTypeExtensionReconciliationJobData reconciliationJobData = new WorkItemTypeExtensionReconciliationJobData();
      reconciliationJobData.Details = new WorkItemTypeExtensionReconciliationJobDetail[1]
      {
        new WorkItemTypeExtensionReconciliationJobDetail()
        {
          ExtensionId = extension.Id,
          Rules = source.ToArray<WorkItemFieldRule>()
        }
      };
      reconciliationJobData.IsCancelable = isCancelable;
      reconciliationJobData.ReconciliationWatermark = watermark;
      reconciliationJobData.IdentityDescriptor = requestContext.UserContext;
      reconciliationJobData.StackTrace = WorkItemTypeExtensionService.GetCleanStackTrace();
      reconciliationJobData.SkipWITChangeDateUpdate = skipWITChangeDateUpdate;
      WorkItemTypeExtensionReconciliationJobData jobData = reconciliationJobData;
      WorkItemTypeExtensionService.QueueReconcileExtensionsJob(requestContext, jobData, jobPriority);
    }

    private void QueueReconcileExtensionsJob(
      IVssRequestContext requestContext,
      IEnumerable<WorkItemTypeExtension> extensions,
      Guid watermark,
      bool refreshTree = false)
    {
      requestContext.TraceBlock(901296, 901298, 901297, "WorkItemTypeExtensions", "WorkItemTypeExtensionsService", "QueueReconcileExtensionsJobBulk", (Action) (() =>
      {
        this.SetReconciliationWatermarks(requestContext, extensions, watermark);
        WorkItemTypeExtensionService.QueueReconcileExtensionsJob(requestContext, new WorkItemTypeExtensionReconciliationJobData()
        {
          Details = extensions.Select<WorkItemTypeExtension, WorkItemTypeExtensionReconciliationJobDetail>((Func<WorkItemTypeExtension, WorkItemTypeExtensionReconciliationJobDetail>) (e =>
          {
            IEnumerable<WorkItemFieldRule> source = e.FieldRules ?? Enumerable.Empty<WorkItemFieldRule>();
            return new WorkItemTypeExtensionReconciliationJobDetail()
            {
              ExtensionId = e.Id,
              Rules = source.ToArray<WorkItemFieldRule>()
            };
          })).ToArray<WorkItemTypeExtensionReconciliationJobDetail>(),
          IsCancelable = true,
          RefreshTree = refreshTree,
          ReconciliationWatermark = watermark,
          IdentityDescriptor = requestContext.UserContext,
          StackTrace = WorkItemTypeExtensionService.GetCleanStackTrace()
        });
      }));
    }

    private static string GetCleanStackTrace() => string.Join(Environment.NewLine, ((IEnumerable<string>) new StackTrace(1).ToString().Split(new string[1]
    {
      Environment.NewLine
    }, StringSplitOptions.RemoveEmptyEntries)).Where<string>((Func<string, bool>) (s => !s.Contains("at System."))));

    private static void QueueReconcileExtensionsJob(
      IVssRequestContext requestContext,
      WorkItemTypeExtensionReconciliationJobData jobData,
      JobPriorityLevel jobPriority = JobPriorityLevel.Normal)
    {
      requestContext.TraceBlock(901293, 901295, 901294, "WorkItemTypeExtensions", "WorkItemTypeExtensionsService", nameof (QueueReconcileExtensionsJob), (Action) (() => requestContext.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>().AddTask(requestContext, (TeamFoundationTaskCallback) ((ctx, args) =>
      {
        try
        {
          ctx.GetService<TeamFoundationJobService>().QueueOneTimeJob(ctx, "Work item type extension reconciliation job", "Microsoft.TeamFoundation.WorkItemTracking.Server.Jobs.ReconcileWorkItemTypeExtension", TeamFoundationSerializationUtility.SerializeToXml((object) jobData), jobPriority);
        }
        catch
        {
        }
      }))));
    }

    internal WorkItemTypeExtensionsReconciliationTelemetryParams TryReconcileExtension(
      WorkItemTrackingRequestContext witRequestContext,
      WorkItemTypeExtension extension,
      IEnumerable<WorkItemFieldRule> reconciliationRules,
      Func<bool> isCanceled,
      IdentityDescriptor identityDescriptor,
      ILeaseInfo reconcileLeaseInfo,
      bool skipWITChangeDateUpdate = false)
    {
      return WorkItemTypeExtensionService.TryReconcileExtensionCore(this, witRequestContext, extension, reconciliationRules, isCanceled, identityDescriptor, reconcileLeaseInfo, skipWITChangeDateUpdate);
    }

    internal static WorkItemTypeExtensionsReconciliationTelemetryParams TryReconcileExtensionCore(
      WorkItemTypeExtensionService extensionService,
      WorkItemTrackingRequestContext witRequestContext,
      WorkItemTypeExtension extension,
      IEnumerable<WorkItemFieldRule> reconciliationRules,
      Func<bool> isCanceled,
      IdentityDescriptor identityDescriptor,
      ILeaseInfo reconcileLeaseInfo,
      bool skipWITChangeDateUpdate = false)
    {
      IVssRequestContext tfsContext = witRequestContext.RequestContext;
      List<int> toActivate = (List<int>) null;
      List<int> alreadyActive = (List<int>) null;
      List<FieldEntry> pageFields = (List<FieldEntry>) null;
      HashSet<int> extensionFields = (HashSet<int>) null;
      int markerFieldId = 0;
      Microsoft.VisualStudio.Services.Identity.Identity changedByIdentity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      IServerDefaultValueTransformer serverDefaultValueProvider = (IServerDefaultValueTransformer) null;
      List<WorkItemFieldRule> rules = (List<WorkItemFieldRule>) null;
      List<int> fieldsToEvaluate = (List<int>) null;
      WorkItemTypeExtensionsReconciliationTelemetryParams telemetry = new WorkItemTypeExtensionsReconciliationTelemetryParams()
      {
        Extension = extension,
        NewestWorkItemDateTime = DateTime.MinValue,
        OldestWorkItemDateTime = DateTime.MaxValue
      };
      Stopwatch stopWatch = new Stopwatch();
      bool flag = tfsContext.TraceBlock<bool>(901275, 901289, 901288, "WorkItemTypeExtensions", "WorkItemTypeExtensionsService", "TryReconcileExtension", (Func<bool>) (() =>
      {
        foreach (Func<bool?> func in new List<Func<bool?>>()
        {
          (Func<bool?>) (() =>
          {
            telemetry.LastStepName = "Query";
            tfsContext.TraceBlock(901276, 901278, 901277, "WorkItemTypeExtensions", "WorkItemTypeExtensionsService", "TryReconcileExtension.QueryTargets", (Action) (() =>
            {
              stopWatch.Restart();
              string wiql3 = "SELECT [System.Id] FROM WorkItems WHERE " + extension.Predicate.ToWIQLPredicate((IPredicateValidationHelper) new WorkItemTypeExtensionService.ExtensionPredicateValidator(tfsContext));
              telemetry.Wiql = wiql3;
              IWorkItemQueryService service = tfsContext.GetService<IWorkItemQueryService>();
              QueryResult queryResult;
              try
              {
                queryResult = service.ExecuteQuery(tfsContext, wiql3);
              }
              catch (WorkItemTrackingQueryResultSizeLimitExceededException ex)
              {
                tfsContext.Trace(910707, TraceLevel.Warning, "WorkItemTypeExtensions", "WorkItemTypeExtensionsService", "Initial query exceeded the max query limit. Falling back to an ORDER BY [System.ChangedDate] query.");
                telemetry.QueryLimitExceededFallback = true;
                string str = wiql3 + " ORDER BY [System.ChangedDate] desc";
                IWorkItemQueryService itemQueryService = service;
                IVssRequestContext requestContext = tfsContext;
                string wiql4 = str;
                int num = tfsContext.WitContext().ServerSettings.MaxQueryResultSize - 1;
                Guid? projectId = new Guid?();
                int topCount = num;
                queryResult = itemQueryService.ExecuteQuery(requestContext, wiql4, projectId: projectId, topCount: topCount);
              }
              toActivate = queryResult.WorkItemIds.ToList<int>();
              telemetry.InitialActivateCount = toActivate.Count;
              stopWatch.Stop();
              telemetry.QueryWorkItemsTimeMs = stopWatch.ElapsedMilliseconds;
              reconcileLeaseInfo.Renew();
              ++telemetry.LeaseRenewCount;
            }));
            return new bool?();
          }),
          (Func<bool?>) (() =>
          {
            telemetry.LastStepName = "GetActiveWorkItems";
            tfsContext.TraceBlock(901279, 901281, 901280, "WorkItemTypeExtensions", "WorkItemTypeExtensionsService", "TryReconcileExtension.QueryAlreadyActives", (Action) (() =>
            {
              stopWatch.Restart();
              alreadyActive = extensionService.GetActiveWorkItems(tfsContext, extension.Id, extension.MarkerField.Field.FieldId);
              telemetry.AlreadyActiveCount = alreadyActive.Count;
              stopWatch.Stop();
              telemetry.GetActiveWorkItemsTimeMs = stopWatch.ElapsedMilliseconds;
              reconcileLeaseInfo.Renew();
              ++telemetry.LeaseRenewCount;
            }));
            return new bool?();
          }),
          (Func<bool?>) (() =>
          {
            telemetry.LastStepName = "PageFields";
            tfsContext.TraceBlock(901400, 901401, 901402, "WorkItemTypeExtensions", "WorkItemTypeExtensionsService", "TryReconcileExtension.PageFieldsStep", (Action) (() =>
            {
              IFieldTypeDictionary fieldDict = tfsContext.GetService<WorkItemTrackingFieldService>().GetFieldsSnapshot(tfsContext);
              pageFields = extension.GetReferencedFields().Concat<int>((IEnumerable<int>) new int[4]
              {
                -3,
                8,
                -2,
                -104
              }).Distinct<int>().Select<int, FieldEntry>((Func<int, FieldEntry>) (fid => fieldDict.GetField(fid))).ToList<FieldEntry>();
              reconcileLeaseInfo.Renew();
              ++telemetry.LeaseRenewCount;
            }));
            return new bool?();
          }),
          (Func<bool?>) (() =>
          {
            telemetry.LastStepName = "ReadIdentity";
            tfsContext.TraceBlock(901403, 901404, 901405, "WorkItemTypeExtensions", "WorkItemTypeExtensionsService", "TryReconcileExtension.ReadIdentityStep", (Action) (() =>
            {
              extensionFields = new HashSet<int>(extension.Fields.Select<WorkItemTypeExtensionFieldEntry, int>((Func<WorkItemTypeExtensionFieldEntry, int>) (f => f.Field.FieldId)));
              markerFieldId = extension.MarkerField.Field.FieldId;
              if (identityDescriptor != (IdentityDescriptor) null)
              {
                changedByIdentity = witRequestContext.IdentityService.ReadIdentities(witRequestContext.RequestContext.Elevate(), (IList<IdentityDescriptor>) new IdentityDescriptor[1]
                {
                  identityDescriptor
                }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
                serverDefaultValueProvider = (IServerDefaultValueTransformer) new ServerDefaultValueTransformer(witRequestContext.RequestContext, changedByIdentity);
              }
              else
                serverDefaultValueProvider = witRequestContext.ServerDefaultValueTransformer;
              reconcileLeaseInfo.Renew();
              ++telemetry.LeaseRenewCount;
            }));
            return new bool?();
          }),
          (Func<bool?>) (() =>
          {
            telemetry.LastStepName = "DeActivate";
            return tfsContext.TraceBlock<bool?>(901282, 901284, 901283, "WorkItemTypeExtensions", "WorkItemTypeExtensionsService", "TryReconcileExtension.Deactivate", (Func<bool?>) (() =>
            {
              List<int> list4 = alreadyActive.Except<int>((IEnumerable<int>) toActivate).ToList<int>();
              List<int> source3 = list4;
              telemetry.InitialDeactivateCount = list4.Count;
              while (source3.Any<int>())
              {
                if (isCanceled())
                {
                  telemetry.LastStepName = "DeActivate.BeforePage";
                  return new bool?(false);
                }
                List<int> workItemIds = source3;
                source3 = new List<int>();
                stopWatch.Restart();
                foreach (IEnumerable<WorkItemFieldData> source4 in extensionService.PageWorkItems(tfsContext, (IEnumerable<int>) workItemIds, (IEnumerable<FieldEntry>) pageFields).Page<WorkItemFieldData>(200))
                {
                  ++telemetry.PageWorkItemsDbCount;
                  if (isCanceled())
                  {
                    telemetry.LastStepName = "DeActivate.AfterPage";
                    return new bool?(false);
                  }
                  telemetry.TotalFetchCount += source4.Count<WorkItemFieldData>();
                  List<WorkItemFieldData> list5 = source4.Where<WorkItemFieldData>((Func<WorkItemFieldData, bool>) (w => !w.IsPredicateMatch(witRequestContext, extension))).ToList<WorkItemFieldData>();
                  stopWatch.Stop();
                  telemetry.RecordPageWorkItemsTime(stopWatch.ElapsedMilliseconds);
                  if (list5.Any<WorkItemFieldData>())
                  {
                    telemetry.NewestWorkItemDateTime = WorkItemTypeExtensionService.GetMaxDate(telemetry.NewestWorkItemDateTime, list5.Max<WorkItemFieldData, DateTime>((Func<WorkItemFieldData, DateTime>) (w => w.ModifiedDate)));
                    telemetry.OldestWorkItemDateTime = WorkItemTypeExtensionService.GetMinDate(telemetry.OldestWorkItemDateTime, list5.Min<WorkItemFieldData, DateTime>((Func<WorkItemFieldData, DateTime>) (w => w.ModifiedDate)));
                  }
                  List<WorkItemCustomFieldUpdateRecord> updateRecords = new List<WorkItemCustomFieldUpdateRecord>();
                  List<WorkItemIdRevisionPair> itemIdRevisionPairList = new List<WorkItemIdRevisionPair>();
                  foreach (WorkItemFieldData workItem in list5)
                  {
                    workItem.SetFieldValue(witRequestContext, extension.MarkerField.Field, (object) false);
                    List<WorkItemCustomFieldUpdateRecord> list6 = extensionService.GetFieldUpdates(witRequestContext, workItem, extension, extensionFields, markerFieldId, serverDefaultValueProvider, false).ToList<WorkItemCustomFieldUpdateRecord>();
                    IEnumerable<WorkItemTypeExtension> highestRankedExtensions = ((IEnumerable<WorkItemTypeExtension>) workItem.GetApplicableExtensions(witRequestContext)).OrderBy<WorkItemTypeExtension, int>((Func<WorkItemTypeExtension, int>) (e => e.Rank)).Reverse<WorkItemTypeExtension>();
                    list6.AddRange(extensionService.GetParentFieldUpdates(witRequestContext, highestRankedExtensions, extension, workItem, false));
                    if (list6.Any<WorkItemCustomFieldUpdateRecord>())
                    {
                      itemIdRevisionPairList.Add(new WorkItemIdRevisionPair()
                      {
                        Id = workItem.Id,
                        Revision = workItem.Revision
                      });
                      updateRecords.AddRange((IEnumerable<WorkItemCustomFieldUpdateRecord>) list6);
                    }
                  }
                  if (isCanceled())
                  {
                    telemetry.LastStepName = "DeActivate.BeforeUpdate";
                    return new bool?(false);
                  }
                  if (itemIdRevisionPairList.Any<WorkItemIdRevisionPair>())
                  {
                    stopWatch.Restart();
                    IEnumerable<int> ints = extensionService.UpdateWorkItems(witRequestContext, (IEnumerable<WorkItemIdRevisionPair>) itemIdRevisionPairList, (IEnumerable<WorkItemCustomFieldUpdateRecord>) updateRecords, serverDefaultValueProvider.CurrentIdentity, skipWITChangeDateUpdate);
                    stopWatch.Stop();
                    telemetry.RecordUpdateWorkItemsTime(stopWatch.ElapsedMilliseconds);
                    ++telemetry.UpdateWorkItemsDbCount;
                    telemetry.ActualDeactivateCount += itemIdRevisionPairList.Count;
                    if (ints.Any<int>())
                    {
                      ++telemetry.DeactivateRetryTimes;
                      telemetry.DeactivateRetryCount += ints.Count<int>();
                      source3.AddRange(ints);
                    }
                  }
                  stopWatch.Restart();
                  reconcileLeaseInfo.Renew();
                  ++telemetry.LeaseRenewCount;
                }
              }
              return new bool?();
            }));
          }),
          (Func<bool?>) (() =>
          {
            telemetry.LastStepName = "Activate";
            return tfsContext.TraceBlock<bool?>(901285, 901287, 901286, "WorkItemTypeExtensions", "WorkItemTypeExtensionsService", "TryReconcileExtension.Activate", (Func<bool?>) (() =>
            {
              WorkItemTypeExtensionService.FixFieldRules(tfsContext, extension, reconciliationRules);
              rules = RuleEngine.PrepareRulesForExecution(reconciliationRules, false).ToList<WorkItemFieldRule>();
              fieldsToEvaluate = rules.Select<WorkItemFieldRule, int>((Func<WorkItemFieldRule, int>) (fr => fr.FieldId)).Distinct<int>().ToList<int>();
              List<int> source7 = toActivate;
              while (source7.Any<int>())
              {
                if (isCanceled())
                {
                  telemetry.LastStepName = "Activate.BeforePage";
                  return new bool?(false);
                }
                List<int> workItemIds = source7;
                source7 = new List<int>();
                stopWatch.Restart();
                foreach (IEnumerable<WorkItemFieldData> workItemFieldDatas in extensionService.PageWorkItems(tfsContext, (IEnumerable<int>) workItemIds, (IEnumerable<FieldEntry>) pageFields).Page<WorkItemFieldData>(200))
                {
                  ++telemetry.PageWorkItemsDbCount;
                  if (isCanceled())
                  {
                    telemetry.LastStepName = "Activate.AfterPage";
                    return new bool?(false);
                  }
                  List<WorkItemFieldData> source8 = new List<WorkItemFieldData>();
                  foreach (WorkItemFieldData workItemFieldData in workItemFieldDatas)
                  {
                    ++telemetry.TotalFetchCount;
                    if (workItemFieldData.IsPredicateMatch(witRequestContext, extension))
                      source8.Add(workItemFieldData);
                  }
                  stopWatch.Stop();
                  telemetry.RecordPageWorkItemsTime(stopWatch.ElapsedMilliseconds);
                  if (source8.Any<WorkItemFieldData>())
                  {
                    telemetry.NewestWorkItemDateTime = WorkItemTypeExtensionService.GetMaxDate(telemetry.NewestWorkItemDateTime, source8.Max<WorkItemFieldData, DateTime>((Func<WorkItemFieldData, DateTime>) (w => w.ModifiedDate)));
                    telemetry.OldestWorkItemDateTime = WorkItemTypeExtensionService.GetMinDate(telemetry.OldestWorkItemDateTime, source8.Min<WorkItemFieldData, DateTime>((Func<WorkItemFieldData, DateTime>) (w => w.ModifiedDate)));
                  }
                  List<WorkItemCustomFieldUpdateRecord> updateRecords = new List<WorkItemCustomFieldUpdateRecord>();
                  List<WorkItemIdRevisionPair> itemIdRevisionPairList = new List<WorkItemIdRevisionPair>();
                  foreach (WorkItemFieldData workItem in source8)
                  {
                    workItem.SetFieldValue(witRequestContext, extension.MarkerField.Field, (object) true);
                    workItem.EvaluateRules(witRequestContext, (IEnumerable<WorkItemFieldRule>) rules, RuleEngineConfiguration.ServerFull, (IEnumerable<int>) fieldsToEvaluate);
                    List<WorkItemCustomFieldUpdateRecord> list = extensionService.GetFieldUpdates(witRequestContext, workItem, extension, extensionFields, markerFieldId, serverDefaultValueProvider, true).ToList<WorkItemCustomFieldUpdateRecord>();
                    IEnumerable<WorkItemTypeExtension> highestRankedExtensions = ((IEnumerable<WorkItemTypeExtension>) workItem.GetApplicableExtensions(witRequestContext)).OrderBy<WorkItemTypeExtension, int>((Func<WorkItemTypeExtension, int>) (e => e.Rank)).Reverse<WorkItemTypeExtension>();
                    list.AddRange(extensionService.GetParentFieldUpdates(witRequestContext, highestRankedExtensions, extension, workItem, true));
                    if (list.Any<WorkItemCustomFieldUpdateRecord>())
                    {
                      itemIdRevisionPairList.Add(new WorkItemIdRevisionPair()
                      {
                        Id = workItem.Id,
                        Revision = workItem.Revision
                      });
                      updateRecords.AddRange((IEnumerable<WorkItemCustomFieldUpdateRecord>) list);
                    }
                  }
                  if (isCanceled())
                  {
                    telemetry.LastStepName = "Activate.BeforeUpdate";
                    return new bool?(false);
                  }
                  if (itemIdRevisionPairList.Any<WorkItemIdRevisionPair>())
                  {
                    stopWatch.Restart();
                    IEnumerable<int> ints = extensionService.UpdateWorkItems(witRequestContext, (IEnumerable<WorkItemIdRevisionPair>) itemIdRevisionPairList, (IEnumerable<WorkItemCustomFieldUpdateRecord>) updateRecords, serverDefaultValueProvider.CurrentIdentity, skipWITChangeDateUpdate);
                    stopWatch.Stop();
                    telemetry.RecordUpdateWorkItemsTime(stopWatch.ElapsedMilliseconds);
                    ++telemetry.UpdateWorkItemsDbCount;
                    telemetry.ActualActivateCount += itemIdRevisionPairList.Count;
                    if (ints.Any<int>())
                    {
                      ++telemetry.ActivateRetryTimes;
                      telemetry.ActivateRetryCount += ints.Count<int>();
                      source7.AddRange(ints);
                    }
                  }
                  stopWatch.Restart();
                  reconcileLeaseInfo.Renew();
                  ++telemetry.LeaseRenewCount;
                }
              }
              return new bool?();
            }));
          })
        })
        {
          if (isCanceled())
            return false;
          bool? nullable = func();
          if (nullable.HasValue)
            return nullable.Value;
        }
        return true;
      }));
      if (flag)
        WorkItemKpiTracer.TraceKpi(tfsContext, (WorkItemTrackingKpi) new ReconcileExtensionKpi(tfsContext));
      telemetry.PageFields = pageFields;
      telemetry.ExtensionFields = extensionFields;
      telemetry.ReconcileResult = flag;
      telemetry.FieldsToEvaluate = fieldsToEvaluate;
      WorkItemTrackingTelemetry.TraceCustomerIntelligence(tfsContext, WorkItemTypeExtensionsReconciliationTelemetry.Feature, (object) telemetry);
      return telemetry;
    }

    private static DateTime GetMaxDate(DateTime a, DateTime b) => !(a > b) ? b : a;

    private static DateTime GetMinDate(DateTime a, DateTime b) => !(a < b) ? b : a;

    private IEnumerable<WorkItemCustomFieldUpdateRecord> GetFieldUpdates(
      WorkItemTrackingRequestContext witRequestContext,
      WorkItemFieldData workItem,
      WorkItemTypeExtension extension,
      HashSet<int> extensionFields,
      int markerFieldId,
      IServerDefaultValueTransformer serverDefaultValueProvider,
      bool activating)
    {
      foreach (KeyValuePair<FieldEntry, object> keyValuePair in workItem.GetUpdatesByFieldEntry(witRequestContext))
      {
        FieldEntry key = keyValuePair.Key;
        if (!key.IsReadOnly && key.FieldId != 80 && (key.StorageTarget & FieldStorageTarget.LongTable) != FieldStorageTarget.Unknown)
        {
          TrendDataUpdateOption dataUpdateOption = TrendDataUpdateOption.None;
          if (extensionFields.Contains(key.FieldId))
            dataUpdateOption = this.GetTrendUpdateOption(witRequestContext, workItem, key, markerFieldId);
          object internalValue = FieldValueHelpers.GetInternalValue(serverDefaultValueProvider.TransformValue(keyValuePair.Value, key.FieldType), key.FieldType);
          yield return new WorkItemCustomFieldUpdateRecord()
          {
            WorkItemId = workItem.Id,
            Field = key,
            Value = internalValue,
            TrendOption = dataUpdateOption
          };
        }
      }
      if (activating != workItem.GetFieldValue<bool>(witRequestContext, markerFieldId, true))
      {
        foreach (FieldEntry fieldEntry in extension.Fields.Select<WorkItemTypeExtensionFieldEntry, FieldEntry>((Func<WorkItemTypeExtensionFieldEntry, FieldEntry>) (f => f.Field)))
        {
          if (fieldEntry.IsUsedInTrendData && !workItem.Updates.ContainsKey(fieldEntry.FieldId))
            yield return new WorkItemCustomFieldUpdateRecord()
            {
              WorkItemId = workItem.Id,
              Field = fieldEntry,
              Value = activating ? (object) TrendDataValue.Increase : (object) TrendDataValue.Decrease,
              TrendOption = TrendDataUpdateOption.None
            };
        }
      }
    }

    private TrendDataUpdateOption GetTrendUpdateOption(
      WorkItemTrackingRequestContext witRequestContext,
      WorkItemFieldData workItem,
      FieldEntry field,
      int markerFieldId)
    {
      TrendDataUpdateOption trendUpdateOption = TrendDataUpdateOption.None;
      if (field.IsUsedInTrendData)
      {
        int num = workItem.GetFieldValue<bool>(witRequestContext, markerFieldId, true) ? 1 : 0;
        bool fieldValue = workItem.GetFieldValue<bool>(witRequestContext, markerFieldId);
        if (num != 0)
          trendUpdateOption |= TrendDataUpdateOption.UpdateOldValue;
        if (fieldValue)
          trendUpdateOption |= TrendDataUpdateOption.UpdateNewValue;
      }
      return trendUpdateOption;
    }

    private IEnumerable<WorkItemCustomFieldUpdateRecord> GetParentFieldUpdates(
      WorkItemTrackingRequestContext witRequestContext,
      IEnumerable<WorkItemTypeExtension> highestRankedExtensions,
      WorkItemTypeExtension extension,
      WorkItemFieldData workItem,
      bool activating)
    {
      WorkItemTrackingFieldService service = witRequestContext.RequestContext.GetService<WorkItemTrackingFieldService>();
      List<WorkItemCustomFieldUpdateRecord> parentFieldUpdates = new List<WorkItemCustomFieldUpdateRecord>();
      bool flag = true;
      WorkItemTypeExtension itemTypeExtension;
      if (activating)
      {
        itemTypeExtension = highestRankedExtensions.FirstOrDefault<WorkItemTypeExtension>() ?? extension;
      }
      else
      {
        itemTypeExtension = highestRankedExtensions.Where<WorkItemTypeExtension>((Func<WorkItemTypeExtension, bool>) (e => e.Id != extension.Id)).FirstOrDefault<WorkItemTypeExtension>();
        if (itemTypeExtension == null)
        {
          flag = false;
          itemTypeExtension = extension;
        }
      }
      foreach (WorkItemTypeExtensionFieldEntry extensionFieldEntry in itemTypeExtension.Fields.Where<WorkItemTypeExtensionFieldEntry>((Func<WorkItemTypeExtensionFieldEntry, bool>) (f => f.Field.ParentFieldId != 0)))
      {
        object fieldValue1 = flag ? workItem.GetFieldValue(witRequestContext, extensionFieldEntry.Field.FieldId) : (object) null;
        FieldEntry fieldById = service.GetFieldById(witRequestContext.RequestContext, extensionFieldEntry.Field.ParentFieldId);
        object fieldValue2 = workItem.GetFieldValue(witRequestContext, fieldById.FieldId);
        if (!RuleEngine.FieldRuleEvaluator.CompareFieldValues(fieldValue1, fieldValue2, false))
          parentFieldUpdates.Add(new WorkItemCustomFieldUpdateRecord()
          {
            WorkItemId = workItem.Id,
            Field = fieldById,
            Value = fieldValue1,
            TrendOption = this.GetTrendUpdateOption(witRequestContext, workItem, fieldById, itemTypeExtension.MarkerField.Field.FieldId)
          });
      }
      return (IEnumerable<WorkItemCustomFieldUpdateRecord>) parentFieldUpdates;
    }

    public ReconcileRequestResult ReconcileExtensions(
      IVssRequestContext requestContext,
      HashSet<int> treeIdsToCheck,
      HashSet<string> treePathsToCheck,
      int timeout)
    {
      return requestContext.TraceBlock<ReconcileRequestResult>(901290, 902292, 901291, "WorkItemTypeExtensions", "WorkItemTypeExtensionsService", "ReconcileExtensionsForTreeChanges", (Func<ReconcileRequestResult>) (() =>
      {
        List<WorkItemTypeExtension> extensionsToReconcile = new List<WorkItemTypeExtension>();
        foreach (WorkItemTypeExtension extension1 in this.GetExtensions(requestContext, new Guid?(), new Guid?()))
        {
          WorkItemTypeExtension extension = extension1;
          if (extension.Predicate != null)
            extension.Predicate.Walk((PredicateOperator) null, (PredicateVisitor) ((predicate, parent) =>
            {
              if (predicate is PredicateFieldComparisonOperator comparisonOperator2)
              {
                if (TFStringComparer.WorkItemFieldReferenceName.Equals(comparisonOperator2.Field, "System.AreaPath") || TFStringComparer.WorkItemFieldReferenceName.Equals(comparisonOperator2.Field, "System.IterationPath"))
                {
                  string str = comparisonOperator2.Value as string;
                  if (!string.IsNullOrEmpty(str))
                  {
                    if (treePathsToCheck.Contains(str.Trim('\\')))
                    {
                      extensionsToReconcile.Add(extension);
                      return false;
                    }
                  }
                }
                else if ((TFStringComparer.WorkItemFieldReferenceName.Equals(comparisonOperator2.Field, "System.AreaId") || TFStringComparer.WorkItemFieldReferenceName.Equals(comparisonOperator2.Field, "System.IterationId")) && comparisonOperator2.Value is int && treeIdsToCheck.Contains((int) comparisonOperator2.Value))
                {
                  extensionsToReconcile.Add(extension);
                  return false;
                }
              }
              return true;
            }), (PredicateVisitor) null);
        }
        if (!extensionsToReconcile.Any<WorkItemTypeExtension>())
          return ReconcileRequestResult.CompletedSynchronously;
        Guid watermark = Guid.NewGuid();
        this.QueueReconcileExtensionsJob(requestContext, (IEnumerable<WorkItemTypeExtension>) extensionsToReconcile, watermark, true);
        return ReconcileRequestResult.AsyncTaskQueued;
      }));
    }

    internal ReconcileRequestResult ReconcileExtensions(
      IVssRequestContext requestContext,
      IEnumerable<WorkItemTypeCategoryUpdateEventData> witCategoryChanges)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<WorkItemTypeCategoryUpdateEventData>>(witCategoryChanges, nameof (witCategoryChanges));
      return requestContext.TraceBlock<ReconcileRequestResult>(901350, 901359, 901358, "WorkItemTypeExtensions", "WorkItemTypeExtensionsService", "ReconcileExtensionsForWorkItemTypeCategoryChanges", (Func<ReconcileRequestResult>) (() =>
      {
        HashSet<int> source1 = new HashSet<int>();
        HashSet<int> source2 = new HashSet<int>();
        List<Tuple<int, string>> source3 = new List<Tuple<int, string>>();
        foreach (WorkItemTypeCategoryUpdateEventData witCategoryChange in witCategoryChanges)
        {
          switch (witCategoryChange.ChangeType)
          {
            case WorkItemTypeCategoryUpdateType.InsertCategory:
              source3.Add(new Tuple<int, string>(witCategoryChange.ProjectId, witCategoryChange.ReferenceName));
              continue;
            case WorkItemTypeCategoryUpdateType.DestroyCategory:
              source1.Add(witCategoryChange.CategoryId);
              continue;
            case WorkItemTypeCategoryUpdateType.AddCategoryMember:
              source1.Add(witCategoryChange.CategoryId);
              continue;
            case WorkItemTypeCategoryUpdateType.DeleteCategoryMember:
              source2.Add(witCategoryChange.CategoryMemberId);
              continue;
            default:
              continue;
          }
        }
        if (!source1.Any<int>() && !source2.Any<int>() && !source3.Any<Tuple<int, string>>())
          return ReconcileRequestResult.CompletedSynchronously;
        IEnumerable<IGrouping<Guid, string>> extensionReconciliation;
        using (WorkItemTypeExtensionComponent component = WorkItemTypeExtensionComponent.CreateComponent(requestContext))
          extensionReconciliation = component.GetWorkItemCategoryDetailsForWITExtensionReconciliation((IList<int>) source1.ToArray<int>(), (IList<int>) source2.ToArray<int>(), (IList<Tuple<int, string>>) source3.ToArray());
        List<WorkItemTypeExtension> extensionsToReconcile = new List<WorkItemTypeExtension>();
        foreach (IGrouping<Guid, string> grouping in extensionReconciliation)
        {
          IGrouping<Guid, string> group = grouping;
          foreach (WorkItemTypeExtension extension1 in this.GetExtensions(requestContext, new Guid?(group.Key), new Guid?()))
          {
            WorkItemTypeExtension extension = extension1;
            if (extension.Predicate != null)
              extension.Predicate.Walk((PredicateOperator) null, (PredicateVisitor) ((predicate, parent) =>
              {
                if (predicate is PredicateInGroupOperator predicateInGroupOperator2 && TFStringComparer.WorkItemFieldReferenceName.Equals(predicateInGroupOperator2.Field, "System.WorkItemType"))
                {
                  string str = predicateInGroupOperator2.Value as string;
                  if (!string.IsNullOrEmpty(str) && group.Contains<string>(str, (IEqualityComparer<string>) TFStringComparer.WorkItemCategoryName))
                  {
                    extensionsToReconcile.Add(extension);
                    return false;
                  }
                }
                return true;
              }), (PredicateVisitor) null);
          }
        }
        if (!extensionsToReconcile.Any<WorkItemTypeExtension>())
          return ReconcileRequestResult.CompletedSynchronously;
        Guid watermark = Guid.NewGuid();
        this.QueueReconcileExtensionsJob(requestContext, (IEnumerable<WorkItemTypeExtension>) extensionsToReconcile, watermark);
        return ReconcileRequestResult.AsyncTaskQueued;
      }));
    }

    internal void ReconcileExtensions(
      IVssRequestContext requestContext,
      IEnumerable<WorkItemTypeRenameEventData> workItemTypeRenames)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<WorkItemTypeRenameEventData>>(workItemTypeRenames, nameof (workItemTypeRenames));
      requestContext.TraceBlock(901370, 901389, 901388, "WorkItemTypeExtensions", "WorkItemTypeExtensionsService", "ReconcileExtensionsForWorkItemTypeRenames", (Action) (() =>
      {
        IEnumerable<IGrouping<string, WorkItemTypeRenameEventData>> source3 = workItemTypeRenames.GroupBy<WorkItemTypeRenameEventData, string>((Func<WorkItemTypeRenameEventData, string>) (renameOp => renameOp.ProjectName));
        if (!source3.Any<IGrouping<string, WorkItemTypeRenameEventData>>())
          return;
        IProjectService service = requestContext.GetService<IProjectService>();
        foreach (IGrouping<string, WorkItemTypeRenameEventData> source4 in source3)
        {
          string key1 = source4.Key;
          Guid guid = Guid.Empty;
          try
          {
            guid = service.GetProjectId(requestContext.Elevate(), key1);
          }
          catch
          {
          }
          if (guid != Guid.Empty)
          {
            IEnumerable<WorkItemTypeExtension> extensions = this.GetExtensions(requestContext, new Guid?(guid), new Guid?());
            if (extensions.Any<WorkItemTypeExtension>())
            {
              Dictionary<string, string> map = source4.GroupBy<WorkItemTypeRenameEventData, string>((Func<WorkItemTypeRenameEventData, string>) (rename => rename.OldName)).Select<IGrouping<string, WorkItemTypeRenameEventData>, WorkItemTypeRenameEventData>((Func<IGrouping<string, WorkItemTypeRenameEventData>, WorkItemTypeRenameEventData>) (g => g.First<WorkItemTypeRenameEventData>())).ToDictionary<WorkItemTypeRenameEventData, string, string>((Func<WorkItemTypeRenameEventData, string>) (rename => rename.OldName), (Func<WorkItemTypeRenameEventData, string>) (rename => rename.NewName), (IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
              foreach (WorkItemTypeExtension itemTypeExtension in extensions)
              {
                WorkItemExtensionPredicate updatedPredicate = (WorkItemExtensionPredicate) null;
                if (itemTypeExtension.Predicate != null)
                {
                  WorkItemExtensionPredicate clone = itemTypeExtension.Predicate.Clone() as WorkItemExtensionPredicate;
                  clone.Walk((PredicateOperator) null, (PredicateVisitor) ((predicate, parent) =>
                  {
                    if (predicate is PredicateFieldComparisonOperator comparisonOperator4 && TFStringComparer.WorkItemFieldReferenceName.Equals(comparisonOperator4.Field, "System.WorkItemType"))
                    {
                      PredicateFieldComparisonOperator comparisonOperator5 = (PredicateFieldComparisonOperator) (predicate as PredicateEqualsOperator);
                      PredicateFieldComparisonOperator comparisonOperator6 = (PredicateFieldComparisonOperator) (predicate as PredicateInOperator);
                      if (comparisonOperator5 != null)
                      {
                        string key2 = comparisonOperator5.Value as string;
                        string str;
                        if (!string.IsNullOrEmpty(key2) && map.TryGetValue(key2, out str))
                        {
                          comparisonOperator5.Value = (object) str;
                          updatedPredicate = clone;
                        }
                      }
                      else if (comparisonOperator6 != null && comparisonOperator6.Value is string[] source2 && ((IEnumerable<string>) source2).Any<string>())
                      {
                        bool flag = false;
                        List<string> stringList = new List<string>();
                        foreach (string key3 in source2)
                        {
                          if (!string.IsNullOrEmpty(key3))
                          {
                            string str;
                            if (map.TryGetValue(key3, out str))
                            {
                              stringList.Add(str);
                              flag = true;
                            }
                            else
                              stringList.Add(key3);
                          }
                        }
                        if (flag)
                        {
                          comparisonOperator6.Value = (object) stringList.ToArray();
                          updatedPredicate = clone;
                        }
                      }
                    }
                    return true;
                  }), (PredicateVisitor) null);
                }
                IEnumerable<WorkItemFieldRule> fieldRules = (IEnumerable<WorkItemFieldRule>) null;
                if (itemTypeExtension.FieldRules.Any<WorkItemFieldRule>())
                {
                  List<WorkItemFieldRule> source5 = new List<WorkItemFieldRule>(itemTypeExtension.FieldRules.Select<WorkItemFieldRule, WorkItemFieldRule>((Func<WorkItemFieldRule, WorkItemFieldRule>) (r => r.Clone() as WorkItemFieldRule)));
                  foreach (ConditionalBlockRule conditionalBlockRule in source5.SelectMany<WorkItemFieldRule, ConditionalBlockRule>((Func<WorkItemFieldRule, IEnumerable<ConditionalBlockRule>>) (r => r.SelectRules<ConditionalBlockRule>())))
                  {
                    string str;
                    if (conditionalBlockRule.ValueFrom == RuleValueFrom.Value && TFStringComparer.WorkItemFieldReferenceName.Equals(conditionalBlockRule.Field, "System.WorkItemType") && !string.IsNullOrEmpty(conditionalBlockRule.Value) && map.TryGetValue(conditionalBlockRule.Value, out str))
                    {
                      conditionalBlockRule.Value = str;
                      fieldRules = (IEnumerable<WorkItemFieldRule>) source5;
                    }
                  }
                }
                if (updatedPredicate != null || fieldRules != null)
                  this.UpdateExtension(requestContext.Elevate(), itemTypeExtension.Id, itemTypeExtension.ProjectId, itemTypeExtension.OwnerId, (string) null, (string) null, (IEnumerable<WorkItemTypeExtensionFieldDeclaration>) null, updatedPredicate, fieldRules, new int?(itemTypeExtension.Rank), (string) null, (IEnumerable<WorkItemFieldRule>) null, 0, out ReconcileRequestResult _, true);
              }
            }
          }
        }
      }));
    }

    internal virtual IEnumerable<WorkItemFieldData> PageWorkItems(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds,
      IEnumerable<FieldEntry> pageFields)
    {
      TeamFoundationWorkItemService workItemService = requestContext.GetService<TeamFoundationWorkItemService>();
      foreach (IEnumerable<int> workItemIds1 in workItemIds.Page<int>(200))
      {
        foreach (WorkItemFieldData workItemFieldValue in workItemService.GetWorkItemFieldValues(requestContext, workItemIds1, 0, includeTextFields: false, includeTags: false, includeCountFields: false))
          yield return workItemFieldValue;
      }
    }

    private IEnumerable<int> UpdateWorkItems(
      WorkItemTrackingRequestContext witRequestContext,
      IEnumerable<WorkItemIdRevisionPair> workItemIdRevPairs,
      IEnumerable<WorkItemCustomFieldUpdateRecord> updateRecords,
      Microsoft.VisualStudio.Services.Identity.Identity changedBy,
      bool skipWITChangeDateUpdate = false)
    {
      return !updateRecords.Any<WorkItemCustomFieldUpdateRecord>() ? Enumerable.Empty<int>() : witRequestContext.RequestContext.GetService<TeamFoundationWorkItemService>().UpdateReconciledWorkItems(witRequestContext, workItemIdRevPairs, updateRecords, changedBy, skipWITChangeDateUpdate: skipWITChangeDateUpdate).Where<KeyValuePair<int, int>>((Func<KeyValuePair<int, int>, bool>) (p => p.Value != 480000)).Select<KeyValuePair<int, int>, int>((Func<KeyValuePair<int, int>, int>) (p => p.Key));
    }

    protected override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
      base.ServiceStart(systemRequestContext);
      this.RegisterSqlNotifications(systemRequestContext, BaseTeamFoundationWorkItemTrackingService.SqlNotificationSubscription.Default(SpecialGuids.WorkItemTypeExtensionChanged), BaseTeamFoundationWorkItemTrackingService.SqlNotificationSubscription.Default(SpecialGuids.WorkItemTypeExtensionReconciliationStatusChanged), BaseTeamFoundationWorkItemTrackingService.SqlNotificationSubscription.Default(SpecialGuids.WorkItemTypeExtensionDeleted));
    }

    void IDisposable.Dispose()
    {
    }

    protected override void OnSqlNotification(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      if (eventClass == SpecialGuids.WorkItemTypeExtensionChanged)
      {
        if (string.IsNullOrEmpty(eventData))
          return;
        WorkItemTypeletRecord[] source = TeamFoundationSerializationUtility.Deserialize<WorkItemTypeletRecord[]>(eventData, new XmlRootAttribute("work-item-type-extensions"));
        WorkItemTrackingFieldService fieldService = requestContext.GetService<WorkItemTrackingFieldService>();
        WorkItemTypeExtension[] array = ((IEnumerable<WorkItemTypeletRecord>) source).Where<WorkItemTypeletRecord>((Func<WorkItemTypeletRecord, bool>) (er => er.TypeletType == 0)).Select<WorkItemTypeletRecord, WorkItemTypeExtension>((Func<WorkItemTypeletRecord, WorkItemTypeExtension>) (er => WorkItemTypeExtensionService.CreateExtensionObject(requestContext, er, fieldService))).ToArray<WorkItemTypeExtension>();
        this.UpdateCache(requestContext, (IEnumerable<WorkItemTypeExtension>) array);
      }
      else if (eventClass == SpecialGuids.WorkItemTypeExtensionReconciliationStatusChanged)
      {
        if (string.IsNullOrEmpty(eventData))
          return;
        WorkItemTypeExtensionReconciliationStatusRecord[] source = TeamFoundationSerializationUtility.Deserialize<WorkItemTypeExtensionReconciliationStatusRecord[]>(eventData, new XmlRootAttribute("extension-reconciliation-statuses"));
        Dictionary<Guid, WorkItemTypeExtension> dictionary = this.GetCachedExtensions(((IEnumerable<WorkItemTypeExtensionReconciliationStatusRecord>) source).Select<WorkItemTypeExtensionReconciliationStatusRecord, Guid>((Func<WorkItemTypeExtensionReconciliationStatusRecord, Guid>) (rs => rs.Id))).ToDictionary<WorkItemTypeExtension, Guid>((Func<WorkItemTypeExtension, Guid>) (e => e.Id));
        foreach (WorkItemTypeExtensionReconciliationStatusRecord rStatus in source)
        {
          WorkItemTypeExtension itemTypeExtension;
          if (dictionary.TryGetValue(rStatus.Id, out itemTypeExtension))
            itemTypeExtension.Update(rStatus);
        }
      }
      else
      {
        if (!(eventClass == SpecialGuids.WorkItemTypeExtensionDeleted) || string.IsNullOrEmpty(eventData))
          return;
        WorkItemTypeExtensionDeletionStatusRecord[] source = TeamFoundationSerializationUtility.Deserialize<WorkItemTypeExtensionDeletionStatusRecord[]>(eventData, new XmlRootAttribute("extension-deletion-statuses"));
        this.RemoveExtensionsFromCache(requestContext, ((IEnumerable<WorkItemTypeExtensionDeletionStatusRecord>) source).Select<WorkItemTypeExtensionDeletionStatusRecord, Guid>((Func<WorkItemTypeExtensionDeletionStatusRecord, Guid>) (ds => ds.Id)));
      }
    }

    private WorkItemTypeExtensionService.ExtensionsSnapshot GetSnapshot(
      IVssRequestContext requestContext,
      Guid? projectId,
      Guid? ownerId)
    {
      this.m_cacheGate.AcquireReaderLock(-1);
      try
      {
        string cacheKey = WorkItemTypeExtensionService.ExtensionsSnapshot.ToCacheKey(projectId, ownerId);
        WorkItemTypeExtensionService.ExtensionsSnapshot snapshot1;
        if (this.m_snapshots.TryGetValue(cacheKey, out snapshot1))
          return snapshot1;
        LockCookie writerLock = this.m_cacheGate.UpgradeToWriterLock(-1);
        try
        {
          if (this.m_snapshots.TryGetValue(cacheKey, out snapshot1))
            return snapshot1;
          IEnumerable<WorkItemTypeExtension> extensions;
          try
          {
            IEnumerable<WorkItemTypeletRecord> typeletRecords = this.GetTypeletRecords(requestContext, projectId, ownerId);
            WorkItemTrackingFieldService fieldDict = requestContext.GetService<WorkItemTrackingFieldService>();
            try
            {
              extensions = (IEnumerable<WorkItemTypeExtension>) typeletRecords.Select<WorkItemTypeletRecord, WorkItemTypeExtension>((Func<WorkItemTypeletRecord, WorkItemTypeExtension>) (er => WorkItemTypeExtensionService.CreateExtensionObject(requestContext, er, fieldDict))).ToArray<WorkItemTypeExtension>();
            }
            catch (WorkItemTrackingFieldDefinitionNotFoundException ex)
            {
              requestContext.ResetMetadataDbStamps();
              extensions = (IEnumerable<WorkItemTypeExtension>) typeletRecords.Select<WorkItemTypeletRecord, WorkItemTypeExtension>((Func<WorkItemTypeletRecord, WorkItemTypeExtension>) (er => WorkItemTypeExtensionService.CreateExtensionObject(requestContext, er, fieldDict))).ToArray<WorkItemTypeExtension>();
            }
            this.UpdateCache(requestContext, extensions);
          }
          catch (ServiceNotRegisteredException ex)
          {
            extensions = Enumerable.Empty<WorkItemTypeExtension>();
          }
          WorkItemTypeExtensionService.ExtensionsSnapshot snapshot2 = new WorkItemTypeExtensionService.ExtensionsSnapshot(requestContext, projectId, ownerId, extensions);
          this.m_snapshots.Add(cacheKey, snapshot2);
          return snapshot2;
        }
        finally
        {
          this.m_cacheGate.DowngradeFromWriterLock(ref writerLock);
        }
      }
      finally
      {
        this.m_cacheGate.ReleaseReaderLock();
      }
    }

    private IEnumerable<WorkItemTypeExtension> GetCachedExtensions(IEnumerable<Guid> extensionIds)
    {
      if (extensionIds == null || !extensionIds.Any<Guid>())
        return Enumerable.Empty<WorkItemTypeExtension>();
      List<WorkItemTypeExtension> cachedExtensions = new List<WorkItemTypeExtension>();
      this.m_cacheGate.AcquireReaderLock(-1);
      try
      {
        foreach (Guid extensionId in extensionIds)
        {
          WorkItemTypeExtension itemTypeExtension;
          if (this.m_extensionCache.TryGetValue(extensionId, out itemTypeExtension))
            cachedExtensions.Add(itemTypeExtension);
        }
      }
      finally
      {
        this.m_cacheGate.ReleaseReaderLock();
      }
      return (IEnumerable<WorkItemTypeExtension>) cachedExtensions;
    }

    private void UpdateSnapshots(
      IVssRequestContext requestContext,
      IEnumerable<WorkItemTypeExtension> extensions,
      bool remove)
    {
      foreach (WorkItemTypeExtensionService.ExtensionsSnapshot extensionsSnapshot in this.m_snapshots.Values)
      {
        WorkItemTypeExtensionService.ExtensionsSnapshot snapshot = extensionsSnapshot;
        WorkItemTypeExtension[] array = extensions.Where<WorkItemTypeExtension>((Func<WorkItemTypeExtension, bool>) (e => (!snapshot.ProjectId.HasValue || !(snapshot.ProjectId.Value != e.ProjectId)) && (!snapshot.OwnerId.HasValue || !(snapshot.OwnerId.Value != e.OwnerId)))).ToArray<WorkItemTypeExtension>();
        if (remove)
          snapshot.RemoveExtensions(requestContext, (IEnumerable<WorkItemTypeExtension>) array);
        else
          snapshot.UpdateCache(requestContext, (IEnumerable<WorkItemTypeExtension>) array);
      }
    }

    private void UpdateCache(
      IVssRequestContext requestContext,
      IEnumerable<WorkItemTypeExtension> extensions)
    {
      requestContext.TraceBlock(901394, 901396, 901395, "WorkItemTypeExtensions", "WorkItemTypeExtensionsService", nameof (UpdateCache), (Action) (() =>
      {
        this.m_cacheGate.AcquireWriterLock(-1);
        try
        {
          WorkItemTypeExtension itemTypeExtension;
          WorkItemTypeExtension[] array = extensions.Select<WorkItemTypeExtension, WorkItemTypeExtension>((Func<WorkItemTypeExtension, WorkItemTypeExtension>) (e => this.m_extensionCache.TryGetValue(e.Id, out itemTypeExtension) && (itemTypeExtension.ProjectId != e.ProjectId || itemTypeExtension.OwnerId != e.OwnerId) ? itemTypeExtension : (WorkItemTypeExtension) null)).Where<WorkItemTypeExtension>((Func<WorkItemTypeExtension, bool>) (e => e != null)).ToArray<WorkItemTypeExtension>();
          if (((IEnumerable<WorkItemTypeExtension>) array).Any<WorkItemTypeExtension>())
            this.UpdateSnapshots(requestContext, (IEnumerable<WorkItemTypeExtension>) array, true);
          foreach (WorkItemTypeExtension extension in extensions)
            this.m_extensionCache[extension.Id] = extension;
          this.UpdateSnapshots(requestContext, extensions, false);
        }
        finally
        {
          this.m_cacheGate.ReleaseWriterLock();
        }
      }));
    }

    private void RemoveExtensionsFromCache(
      IVssRequestContext requestContext,
      IEnumerable<Guid> extensionIds)
    {
      requestContext.TraceBlock(901397, 901399, 901398, "WorkItemTypeExtensions", "WorkItemTypeExtensionsService", "RemoveExtensionFromCache", (Action) (() =>
      {
        this.m_cacheGate.AcquireWriterLock(-1);
        try
        {
          WorkItemTypeExtension itemTypeExtension;
          WorkItemTypeExtension[] array = extensionIds.Select<Guid, WorkItemTypeExtension>((Func<Guid, WorkItemTypeExtension>) (id => this.m_extensionCache.TryGetValue(id, out itemTypeExtension) ? itemTypeExtension : (WorkItemTypeExtension) null)).Where<WorkItemTypeExtension>((Func<WorkItemTypeExtension, bool>) (e => e != null)).ToArray<WorkItemTypeExtension>();
          if (!((IEnumerable<WorkItemTypeExtension>) array).Any<WorkItemTypeExtension>())
            return;
          this.UpdateSnapshots(requestContext, (IEnumerable<WorkItemTypeExtension>) array, true);
          foreach (WorkItemTypelet workItemTypelet in array)
            this.m_extensionCache.Remove(workItemTypelet.Id);
        }
        finally
        {
          this.m_cacheGate.ReleaseWriterLock();
        }
      }));
    }

    public virtual IEnumerable<WorkItemTypeExtension> GetExtensions(
      IVssRequestContext requestContext,
      Guid? projectId,
      Guid? ownerId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return requestContext.TraceBlock<IEnumerable<WorkItemTypeExtension>>(901250, 901254, 901253, "WorkItemTypeExtensions", "WorkItemTypeExtensionsService", "GetExtensionsByIds", (Func<IEnumerable<WorkItemTypeExtension>>) (() => this.GetSnapshot(requestContext, projectId, ownerId).GetAllExtensions()));
    }

    internal IWorkItemTypeExtensionsMatcher GetExtensionMatcher(
      IVssRequestContext requestContext,
      Guid? projectId,
      Guid? ownerId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return requestContext.TraceBlock<IWorkItemTypeExtensionsMatcher>(901250, 901254, 901253, "WorkItemTypeExtensions", "WorkItemTypeExtensionsService", nameof (GetExtensionMatcher), (Func<IWorkItemTypeExtensionsMatcher>) (() => this.GetSnapshot(requestContext, projectId, ownerId).GetExtensionMatcher()));
    }

    public virtual IEnumerable<WorkItemTypeExtension> GetExtensions(
      IVssRequestContext requestContext,
      IEnumerable<Guid> extensionIds,
      bool ignoreCache = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<Guid>>(extensionIds, nameof (extensionIds));
      return requestContext.TraceBlock<IEnumerable<WorkItemTypeExtension>>(901255, 901259, 901258, "WorkItemTypeExtensions", "WorkItemTypeExtensionsService", nameof (GetExtensions), (Func<IEnumerable<WorkItemTypeExtension>>) (() =>
      {
        if (!extensionIds.Any<Guid>())
          return Enumerable.Empty<WorkItemTypeExtension>();
        IEnumerable<Guid> source1;
        List<WorkItemTypeExtension> extensions;
        if (ignoreCache)
        {
          source1 = extensionIds;
          extensions = new List<WorkItemTypeExtension>();
        }
        else
        {
          extensions = this.GetCachedExtensions(extensionIds).ToList<WorkItemTypeExtension>();
          HashSet<Guid> guidSet = new HashSet<Guid>(extensionIds);
          foreach (WorkItemTypeExtension itemTypeExtension in extensions)
            guidSet.Remove(itemTypeExtension.Id);
          source1 = (IEnumerable<Guid>) guidSet;
        }
        if (source1.Any<Guid>())
        {
          WorkItemTypeExtensionComponent component;
          try
          {
            component = WorkItemTypeExtensionComponent.CreateComponent(requestContext);
          }
          catch (ServiceNotRegisteredException ex)
          {
            return Enumerable.Empty<WorkItemTypeExtension>();
          }
          List<WorkItemTypeletRecord> source2 = (List<WorkItemTypeletRecord>) null;
          using (component)
            source2 = component.GetExtensionsById((IList<Guid>) source1.ToArray<Guid>());
          WorkItemTrackingFieldService fieldService = requestContext.GetService<WorkItemTrackingFieldService>();
          WorkItemTypeExtension[] array = source2.Select<WorkItemTypeletRecord, WorkItemTypeExtension>((Func<WorkItemTypeletRecord, WorkItemTypeExtension>) (er => WorkItemTypeExtensionService.CreateExtensionObject(requestContext, er, fieldService))).ToArray<WorkItemTypeExtension>();
          this.UpdateCache(requestContext, (IEnumerable<WorkItemTypeExtension>) array);
          extensions.AddRange((IEnumerable<WorkItemTypeExtension>) array);
        }
        return (IEnumerable<WorkItemTypeExtension>) extensions;
      }));
    }

    public virtual IEnumerable<WorkItemTypeletRecord> GetTypeletRecords(
      IVssRequestContext requestContext,
      Guid? projectId,
      Guid? ownerId)
    {
      using (WorkItemTypeExtensionComponent component = WorkItemTypeExtensionComponent.CreateComponent(requestContext))
        return (IEnumerable<WorkItemTypeletRecord>) component.GetExtensions(projectId, ownerId);
    }

    public WorkItemTypeExtension CreateExtension(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid ownerId,
      string name,
      string description,
      IEnumerable<WorkItemTypeExtensionFieldDeclaration> fields,
      WorkItemExtensionPredicate predicate,
      IEnumerable<WorkItemFieldRule> fieldRules)
    {
      return this.CreateExtension(requestContext, projectId, ownerId, name, description, fields, predicate, fieldRules, 0, (string) null);
    }

    public WorkItemTypeExtension CreateExtension(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid ownerId,
      string name,
      string description,
      IEnumerable<WorkItemTypeExtensionFieldDeclaration> fields,
      WorkItemExtensionPredicate predicate,
      IEnumerable<WorkItemFieldRule> fieldRules,
      int rank,
      string form)
    {
      return this.CreateExtension(requestContext, projectId, ownerId, name, description, fields, predicate, fieldRules, rank, form, 0, out ReconcileRequestResult _, false);
    }

    public WorkItemTypeExtension CreateExtension(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid ownerId,
      string name,
      string description,
      IEnumerable<WorkItemTypeExtensionFieldDeclaration> fields,
      WorkItemExtensionPredicate predicate,
      IEnumerable<WorkItemFieldRule> fieldRules,
      int rank,
      string form,
      int reconcileTimeout,
      out ReconcileRequestResult reconcileRequestResult,
      bool skipWITChangeDateUpdate = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<string>(name, nameof (name));
      ReconcileRequestResult reqResult = ReconcileRequestResult.Error;
      WorkItemTypeExtension extension = requestContext.TraceBlock<WorkItemTypeExtension>(901260, 901264, 901263, "WorkItemTypeExtensions", "WorkItemTypeExtensionsService", nameof (CreateExtension), (Func<WorkItemTypeExtension>) (() =>
      {
        predicate?.Validate((IPredicateValidationHelper) new WorkItemTypeExtensionService.ExtensionPredicateValidator(requestContext));
        if (fieldRules != null)
          this.ValidateExtensionRules(requestContext, fieldRules, fields);
        Guid extensionId = Guid.NewGuid();
        Guid id = requestContext.WitContext().RequestIdentity.Id;
        WorkItemTypeletRecord extensionRecord = (WorkItemTypeletRecord) null;
        using (WorkItemTypeExtensionComponent component = WorkItemTypeExtensionComponent.CreateComponent(requestContext))
        {
          IEnumerable<WorkItemTypeExtensionFieldDeclaration> source = fields ?? Enumerable.Empty<WorkItemTypeExtensionFieldDeclaration>();
          extensionRecord = component.CreateExtension(extensionId, projectId, ownerId, id, name, description, (IList<CustomFieldEntry>) source.Select<WorkItemTypeExtensionFieldDeclaration, CustomFieldEntry>((Func<WorkItemTypeExtensionFieldDeclaration, CustomFieldEntry>) (x => x.ToCustomField(extensionId))).ToArray<CustomFieldEntry>(), predicate, (IList<WorkItemFieldRule>) fieldRules.ToArray<WorkItemFieldRule>(), form, rank);
        }
        requestContext.ResetMetadataDbStamps();
        WorkItemTrackingFieldService service = requestContext.GetService<WorkItemTrackingFieldService>();
        service.InvalidateCache(requestContext);
        WorkItemTypeExtension extensionObject = WorkItemTypeExtensionService.CreateExtensionObject(requestContext, extensionRecord, service);
        this.UpdateCache(requestContext, (IEnumerable<WorkItemTypeExtension>) new WorkItemTypeExtension[1]
        {
          extensionObject
        });
        reqResult = this.ReconcileExtension(requestContext, extensionObject, reconcileTimeout, skipWITChangeDateUpdate);
        return extensionObject;
      }));
      reconcileRequestResult = reqResult;
      return extension;
    }

    public WorkItemTypeExtension UpdateExtension(
      IVssRequestContext requestContext,
      Guid extensionId,
      Guid projectId,
      Guid ownerId,
      string name,
      string description,
      IEnumerable<WorkItemTypeExtensionFieldDeclaration> fields,
      WorkItemExtensionPredicate predicate,
      IEnumerable<WorkItemFieldRule> fieldRules)
    {
      return this.UpdateExtension(requestContext, extensionId, projectId, ownerId, name, description, fields, predicate, fieldRules, 0, out ReconcileRequestResult _);
    }

    public WorkItemTypeExtension UpdateExtension(
      IVssRequestContext requestContext,
      Guid extensionId,
      Guid projectId,
      Guid ownerId,
      string name,
      string description,
      IEnumerable<WorkItemTypeExtensionFieldDeclaration> fields,
      WorkItemExtensionPredicate predicate,
      IEnumerable<WorkItemFieldRule> fieldRules,
      int reconcileTimeout,
      out ReconcileRequestResult reconcileRequestResult)
    {
      return this.UpdateExtension(requestContext, extensionId, projectId, ownerId, name, description, fields, predicate, fieldRules, new int?(), (IEnumerable<WorkItemFieldRule>) null, reconcileTimeout, out reconcileRequestResult);
    }

    public WorkItemTypeExtension UpdateExtension(
      IVssRequestContext requestContext,
      Guid extensionId,
      Guid projectId,
      Guid ownerId,
      string name,
      string description,
      IEnumerable<WorkItemTypeExtensionFieldDeclaration> fields,
      WorkItemExtensionPredicate predicate,
      IEnumerable<WorkItemFieldRule> fieldRules,
      int? rank,
      IEnumerable<WorkItemFieldRule> reconciliationScopeRules,
      int reconcileTimeout,
      out ReconcileRequestResult reconcileRequestResult)
    {
      return this.UpdateExtension(requestContext, extensionId, projectId, ownerId, name, description, fields, predicate, fieldRules, rank, reconciliationScopeRules, reconcileTimeout, out reconcileRequestResult, false);
    }

    public WorkItemTypeExtension UpdateExtension(
      IVssRequestContext requestContext,
      Guid extensionId,
      Guid projectId,
      Guid ownerId,
      string name,
      string description,
      IEnumerable<WorkItemTypeExtensionFieldDeclaration> fields,
      WorkItemExtensionPredicate predicate,
      IEnumerable<WorkItemFieldRule> fieldRules,
      int? rank,
      IEnumerable<WorkItemFieldRule> reconciliationScopeRules,
      int reconcileTimeout,
      out ReconcileRequestResult reconcileRequestResult,
      bool skipReconciliation)
    {
      return this.UpdateExtension(requestContext, extensionId, projectId, ownerId, name, description, fields, predicate, fieldRules, rank, (string) null, reconciliationScopeRules, reconcileTimeout, out reconcileRequestResult, skipReconciliation);
    }

    public WorkItemTypeExtension UpdateExtension(
      IVssRequestContext requestContext,
      Guid extensionId,
      Guid projectId,
      Guid ownerId,
      string name,
      string description,
      IEnumerable<WorkItemTypeExtensionFieldDeclaration> fields,
      WorkItemExtensionPredicate predicate,
      IEnumerable<WorkItemFieldRule> fieldRules,
      int? rank,
      string form,
      IEnumerable<WorkItemFieldRule> reconciliationScopeRules,
      int reconcileTimeout,
      out ReconcileRequestResult reconcileRequestResult,
      bool skipReconciliation)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(extensionId, nameof (extensionId));
      ReconcileRequestResult recRequestResult = ReconcileRequestResult.Error;
      WorkItemTypeExtension itemTypeExtension = requestContext.TraceBlock<WorkItemTypeExtension>(901265, 901269, 901268, "WorkItemTypeExtensions", "WorkItemTypeExtensionsService", nameof (UpdateExtension), (Func<WorkItemTypeExtension>) (() =>
      {
        WorkItemTypeExtension extension = this.GetExtensions(requestContext, (IEnumerable<Guid>) new Guid[1]
        {
          extensionId
        }, false).FirstOrDefault<WorkItemTypeExtension>();
        if (extension == null)
          throw new WorkItemTypeExtensionNotFoundException();
        if (name != null)
          ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
        predicate?.Validate((IPredicateValidationHelper) new WorkItemTypeExtensionService.ExtensionPredicateValidator(requestContext));
        if (fieldRules != null)
        {
          if (fields == null)
            this.ValidateExtensionRules(requestContext, fieldRules, extension.Fields);
          else
            this.ValidateExtensionRules(requestContext, fieldRules, fields);
        }
        if (reconciliationScopeRules != null)
        {
          if (fields == null)
            this.ValidateExtensionRules(requestContext, reconciliationScopeRules, extension.Fields);
          else
            this.ValidateExtensionRules(requestContext, reconciliationScopeRules, fields);
        }
        else
          skipReconciliation = skipReconciliation || WorkItemTypeExtensionService.ShouldSkipReconciliation(extension, predicate, fieldRules);
        WorkItemTypeletRecord extensionRecord = (WorkItemTypeletRecord) null;
        Guid id = requestContext.WitContext().RequestIdentity.Id;
        using (WorkItemTypeExtensionComponent component = WorkItemTypeExtensionComponent.CreateComponent(requestContext))
        {
          IEnumerable<WorkItemTypeExtensionFieldDeclaration> source = fields ?? extension.Fields.Select<WorkItemTypeExtensionFieldEntry, WorkItemTypeExtensionFieldDeclaration>((Func<WorkItemTypeExtensionFieldEntry, WorkItemTypeExtensionFieldDeclaration>) (x => new WorkItemTypeExtensionFieldDeclaration(x)));
          extensionRecord = component.UpdateExtension(extensionId, projectId, ownerId, id, name, description, (IList<CustomFieldEntry>) source.Select<WorkItemTypeExtensionFieldDeclaration, CustomFieldEntry>((Func<WorkItemTypeExtensionFieldDeclaration, CustomFieldEntry>) (x => x.ToCustomField(extensionId))).ToArray<CustomFieldEntry>(), predicate, (IList<WorkItemFieldRule>) fieldRules.ToArray<WorkItemFieldRule>(), form, rank.HasValue ? rank.Value : extension.Rank);
        }
        WorkItemTrackingFieldService service = requestContext.GetService<WorkItemTrackingFieldService>();
        if (fields != null)
        {
          requestContext.ResetMetadataDbStamps();
          service.InvalidateCache(requestContext);
        }
        WorkItemTypeExtension extensionObject = WorkItemTypeExtensionService.CreateExtensionObject(requestContext, extensionRecord, service);
        this.UpdateCache(requestContext, (IEnumerable<WorkItemTypeExtension>) new WorkItemTypeExtension[1]
        {
          extensionObject
        });
        recRequestResult = skipReconciliation ? ReconcileRequestResult.Skipped : this.ReconcileExtension(requestContext, extensionObject, reconciliationScopeRules, reconcileTimeout, false);
        return extensionObject;
      }));
      reconcileRequestResult = recRequestResult;
      return itemTypeExtension;
    }

    private static bool ShouldSkipReconciliation(
      WorkItemTypeExtension extension,
      WorkItemExtensionPredicate predicate,
      IEnumerable<WorkItemFieldRule> fieldRules)
    {
      predicate = predicate ?? extension.Predicate;
      if (predicate != extension.Predicate && extension.Predicate != null && !StringComparer.Ordinal.Equals(TeamFoundationSerializationUtility.SerializeToString<WorkItemExtensionPredicate>(predicate), TeamFoundationSerializationUtility.SerializeToString<WorkItemExtensionPredicate>(extension.Predicate)))
        return false;
      fieldRules = fieldRules ?? extension.FieldRules;
      return fieldRules == extension.FieldRules || extension.FieldRules == null || StringComparer.Ordinal.Equals(CommonWITUtils.GetSerializedRuleXML(fieldRules.ToArray<WorkItemFieldRule>()), CommonWITUtils.GetSerializedRuleXML(extension.FieldRules.ToArray<WorkItemFieldRule>()));
    }

    private static WorkItemTypeExtension CreateExtensionObject(
      IVssRequestContext requestContext,
      WorkItemTypeletRecord extensionRecord,
      WorkItemTrackingFieldService fieldService)
    {
      WorkItemTypeExtension extension = WorkItemTypeExtension.Create(requestContext, extensionRecord, fieldService);
      WorkItemTypeExtensionService.FixFieldRules(requestContext, extension, extension.FieldRules);
      if (extension.Predicate != null)
        extension.Predicate.FixFieldReferences((IPredicateValidationHelper) new WorkItemTypeExtensionService.ExtensionPredicateValidator(requestContext));
      return extension;
    }

    protected static void FixFieldRules(
      IVssRequestContext requestContext,
      WorkItemTypeExtension extension,
      IEnumerable<WorkItemFieldRule> fieldRules)
    {
      if (fieldRules == null || !fieldRules.Any<WorkItemFieldRule>())
        return;
      WorkItemTypeExtensionService.ExtensionRuleValidationContext validationHelper = new WorkItemTypeExtensionService.ExtensionRuleValidationContext(requestContext, extension.Fields);
      foreach (WorkItemRule fieldRule in fieldRules)
        fieldRule.FixFieldReferences((IRuleValidationContext) validationHelper);
    }

    private void ValidateExtensionRules(
      IVssRequestContext requestContext,
      IEnumerable<WorkItemFieldRule> fieldRules,
      IEnumerable<WorkItemTypeExtensionFieldEntry> fields)
    {
      WorkItemTypeExtensionService.ExtensionRuleValidationContext ruleValidationContext = new WorkItemTypeExtensionService.ExtensionRuleValidationContext(requestContext, fields);
      WorkItemTypeExtensionService.ValidateRules(fieldRules, ruleValidationContext);
    }

    private void ValidateExtensionRules(
      IVssRequestContext requestContext,
      IEnumerable<WorkItemFieldRule> fieldRules,
      IEnumerable<WorkItemTypeExtensionFieldDeclaration> fields)
    {
      WorkItemTypeExtensionService.ExtensionRuleValidationContext ruleValidationContext = new WorkItemTypeExtensionService.ExtensionRuleValidationContext(requestContext, fields);
      WorkItemTypeExtensionService.ValidateRules(fieldRules, ruleValidationContext);
    }

    private static void ValidateRules(
      IEnumerable<WorkItemFieldRule> fieldRules,
      WorkItemTypeExtensionService.ExtensionRuleValidationContext ruleValidationContext)
    {
      foreach (WorkItemRule fieldRule in fieldRules)
        fieldRule.Validate((IRuleValidationContext) ruleValidationContext);
    }

    public void DeleteExtensions(IVssRequestContext requestContext, IEnumerable<Guid> extensionIds)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<Guid>>(extensionIds, nameof (extensionIds));
      requestContext.TraceBlock(901390, 901393, 901392, "WorkItemTypeExtensions", "WorkItemTypeExtensionsService", nameof (DeleteExtensions), (Action) (() =>
      {
        if (!extensionIds.Any<Guid>())
          return;
        Guid id = requestContext.WitContext().RequestIdentity.Id;
        using (PerformanceScenarioHelper performanceScenarioHelper = new PerformanceScenarioHelper(requestContext, "WorkItemService", nameof (DeleteExtensions)))
        {
          bool flag;
          using (WorkItemTypeExtensionComponent component = WorkItemTypeExtensionComponent.CreateComponent(requestContext))
            flag = component.DeleteExtensions(extensionIds, id);
          if (flag)
          {
            this.RemoveExtensionsFromCache(requestContext, extensionIds);
            requestContext.ResetMetadataDbStamps();
            requestContext.GetService<WorkItemTrackingFieldService>().InvalidateCache(requestContext);
          }
          performanceScenarioHelper.Add("count", (object) extensionIds.Count<Guid>());
          performanceScenarioHelper.Add("deleted", (object) flag);
        }
      }));
    }

    public virtual List<int> GetActiveWorkItems(
      IVssRequestContext requestContext,
      Guid typeletId,
      int markerFieldId = 0)
    {
      using (WorkItemTypeExtensionComponent component = WorkItemTypeExtensionComponent.CreateComponent(requestContext))
        return component.GetExtendedWorkItemIds(typeletId, markerFieldId);
    }

    internal class ExtensionPredicateValidator : IPredicateValidationHelper
    {
      private IVssRequestContext m_requestContext;
      private WorkItemTrackingFieldService m_fieldTypeDictionary;
      private WorkItemTrackingTreeService m_treeService;

      public ExtensionPredicateValidator(IVssRequestContext requestContext)
      {
        this.m_requestContext = requestContext;
        this.m_fieldTypeDictionary = this.m_requestContext.GetService<WorkItemTrackingFieldService>();
        this.m_treeService = requestContext.GetService<WorkItemTrackingTreeService>();
      }

      public InternalFieldType GetFieldType(string fieldReferenceName) => this.m_fieldTypeDictionary.GetField(this.m_requestContext, fieldReferenceName).FieldType;

      public int GetFieldId(string fieldReferenceName)
      {
        FieldEntry field;
        return this.m_fieldTypeDictionary.TryGetField(this.m_requestContext, fieldReferenceName, out field) ? field.FieldId : 0;
      }

      public string GetTreePath(int treeId) => this.m_treeService.LegacyGetTreeNode(this.m_requestContext, treeId, false)?.GetPath(this.m_requestContext);

      public int GetTreeId(string path, TreeStructureType type) => this.m_requestContext.WitContext().TreeService.LegacyGetTreeNodeIdFromPath(this.m_requestContext, path, type);
    }

    private class ExtensionRuleValidationContext : IRuleValidationContext
    {
      private IFieldTypeDictionary m_fieldDict;
      private Dictionary<string, int> m_extensionFields;

      public ExtensionRuleValidationContext(
        IVssRequestContext requestContext,
        IEnumerable<WorkItemTypeExtensionFieldDeclaration> fields)
      {
        this.m_fieldDict = requestContext.GetService<WorkItemTrackingFieldService>().GetFieldsSnapshot(requestContext);
        this.m_extensionFields = new Dictionary<string, int>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        if (fields == null)
          return;
        foreach (WorkItemTypeExtensionFieldDeclaration field in fields)
          this.m_extensionFields[field.ReferenceName] = 0;
      }

      public ExtensionRuleValidationContext(
        IVssRequestContext requestContext,
        IEnumerable<WorkItemTypeExtensionFieldEntry> extensionFields)
      {
        this.m_fieldDict = requestContext.GetService<WorkItemTrackingFieldService>().GetFieldsSnapshot(requestContext);
        this.m_extensionFields = extensionFields.ToDictionary<WorkItemTypeExtensionFieldEntry, string, int>((Func<WorkItemTypeExtensionFieldEntry, string>) (fe => fe.LocalReferenceName), (Func<WorkItemTypeExtensionFieldEntry, int>) (fe => fe.Field.FieldId), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      }

      public bool IsValidField(string fieldReferenceName)
      {
        if (fieldReferenceName.StartsWith("[", StringComparison.OrdinalIgnoreCase) && fieldReferenceName.EndsWith("]", StringComparison.OrdinalIgnoreCase))
          return this.m_extensionFields.ContainsKey(fieldReferenceName.Substring(1, fieldReferenceName.Length - 2));
        return this.m_fieldDict.TryGetField(fieldReferenceName, out FieldEntry _);
      }

      public bool IsValidField(int fieldId) => this.m_fieldDict.TryGetField(fieldId, out FieldEntry _);

      public int GetFieldId(string fieldReferenceName)
      {
        int num;
        FieldEntry field;
        return fieldReferenceName.StartsWith("[", StringComparison.OrdinalIgnoreCase) && fieldReferenceName.EndsWith("]", StringComparison.OrdinalIgnoreCase) ? (this.m_extensionFields.TryGetValue(fieldReferenceName.Substring(1, fieldReferenceName.Length - 2), out num) ? num : 0) : (this.m_fieldDict.TryGetField(fieldReferenceName, out field) ? field.FieldId : 0);
      }
    }

    private class ExtensionsSnapshot
    {
      private ReaderWriterLock m_rwGate = new ReaderWriterLock();
      private Dictionary<Guid, WorkItemTypeExtension> m_extensions;
      private WorkItemTypeExtensionService.ExtensionMatcher m_extensionMatcher;

      public ExtensionsSnapshot(
        IVssRequestContext requestContext,
        Guid? projectId,
        Guid? ownerId,
        IEnumerable<WorkItemTypeExtension> extensions)
      {
        this.ProjectId = projectId;
        this.OwnerId = ownerId;
        this.m_extensions = extensions.ToDictionary<WorkItemTypeExtension, Guid, WorkItemTypeExtension>((Func<WorkItemTypeExtension, Guid>) (e => e.Id), (Func<WorkItemTypeExtension, WorkItemTypeExtension>) (e => e));
        this.m_extensionMatcher = new WorkItemTypeExtensionService.ExtensionMatcher(requestContext, this.m_extensions);
      }

      public Guid? ProjectId { get; set; }

      public Guid? OwnerId { get; set; }

      public IEnumerable<WorkItemTypeExtension> GetAllExtensions()
      {
        this.m_rwGate.AcquireReaderLock(-1);
        try
        {
          return (IEnumerable<WorkItemTypeExtension>) this.m_extensions.Values;
        }
        finally
        {
          this.m_rwGate.ReleaseReaderLock();
        }
      }

      internal void UpdateCache(
        IVssRequestContext requestContext,
        IEnumerable<WorkItemTypeExtension> extensions)
      {
        if (this.m_extensions == null)
          return;
        this.m_rwGate.AcquireWriterLock(-1);
        try
        {
          WorkItemTypeExtension itemTypeExtension;
          WorkItemTypeExtension[] array = extensions.Select<WorkItemTypeExtension, WorkItemTypeExtension>((Func<WorkItemTypeExtension, WorkItemTypeExtension>) (e => this.m_extensions.TryGetValue(e.Id, out itemTypeExtension) ? itemTypeExtension : (WorkItemTypeExtension) null)).Where<WorkItemTypeExtension>((Func<WorkItemTypeExtension, bool>) (e => e != null)).ToArray<WorkItemTypeExtension>();
          Dictionary<Guid, WorkItemTypeExtension> dictionary = new Dictionary<Guid, WorkItemTypeExtension>((IDictionary<Guid, WorkItemTypeExtension>) this.m_extensions);
          foreach (WorkItemTypeExtension extension in extensions)
            dictionary[extension.Id] = extension;
          this.m_extensions = dictionary;
          this.m_extensionMatcher = new WorkItemTypeExtensionService.ExtensionMatcher(requestContext, this.m_extensions, this.m_extensionMatcher, (IEnumerable<WorkItemTypeExtension>) array, extensions);
        }
        finally
        {
          this.m_rwGate.ReleaseWriterLock();
        }
      }

      internal void RemoveExtensions(
        IVssRequestContext requestContext,
        IEnumerable<WorkItemTypeExtension> extensions)
      {
        if (this.m_extensions == null)
          return;
        this.m_rwGate.AcquireWriterLock(-1);
        try
        {
          WorkItemTypeExtension itemTypeExtension1;
          WorkItemTypeExtension[] array = extensions.Select<WorkItemTypeExtension, WorkItemTypeExtension>((Func<WorkItemTypeExtension, WorkItemTypeExtension>) (e => this.m_extensions.TryGetValue(e.Id, out itemTypeExtension1) ? itemTypeExtension1 : (WorkItemTypeExtension) null)).Where<WorkItemTypeExtension>((Func<WorkItemTypeExtension, bool>) (e => e != null)).ToArray<WorkItemTypeExtension>();
          if (!((IEnumerable<WorkItemTypeExtension>) array).Any<WorkItemTypeExtension>())
            return;
          Dictionary<Guid, WorkItemTypeExtension> dictionary = new Dictionary<Guid, WorkItemTypeExtension>((IDictionary<Guid, WorkItemTypeExtension>) this.m_extensions);
          foreach (WorkItemTypeExtension itemTypeExtension2 in array)
            dictionary.Remove(itemTypeExtension2.Id);
          this.m_extensions = dictionary;
          this.m_extensionMatcher = new WorkItemTypeExtensionService.ExtensionMatcher(requestContext, this.m_extensions, this.m_extensionMatcher, (IEnumerable<WorkItemTypeExtension>) array, (IEnumerable<WorkItemTypeExtension>) null);
        }
        finally
        {
          this.m_rwGate.ReleaseWriterLock();
        }
      }

      internal IWorkItemTypeExtensionsMatcher GetExtensionMatcher()
      {
        this.m_rwGate.AcquireReaderLock(-1);
        try
        {
          return (IWorkItemTypeExtensionsMatcher) this.m_extensionMatcher;
        }
        finally
        {
          this.m_rwGate.ReleaseReaderLock();
        }
      }

      public static string ToCacheKey(Guid? projectId, Guid? ownerId)
      {
        StringBuilder stringBuilder = new StringBuilder(42);
        if (projectId.HasValue)
          stringBuilder.Append((object) projectId.Value);
        else
          stringBuilder.Append("all");
        stringBuilder.Append('/');
        if (ownerId.HasValue)
          stringBuilder.Append((object) ownerId.Value);
        else
          stringBuilder.Append("all");
        return stringBuilder.ToString();
      }
    }

    private class ExtensionMatcher : IWorkItemTypeExtensionsMatcher
    {
      private HashSet<WorkItemTypeExtension> m_alwaysInclude = new HashSet<WorkItemTypeExtension>();
      private Dictionary<WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookupKey, WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookupBase> m_partitionLookups = new Dictionary<WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookupKey, WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookupBase>();
      private WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookupKey[] m_partitionLookupKeys;
      private HashSet<int> m_referencedFieldIds;
      private Dictionary<Guid, WorkItemTypeExtension> m_extensions;

      public ExtensionMatcher(
        IVssRequestContext requestContext,
        Dictionary<Guid, WorkItemTypeExtension> extensions)
        : this(requestContext, extensions, (WorkItemTypeExtensionService.ExtensionMatcher) null, (IEnumerable<WorkItemTypeExtension>) null, (IEnumerable<WorkItemTypeExtension>) null)
      {
      }

      public ExtensionMatcher(
        IVssRequestContext requestContext,
        Dictionary<Guid, WorkItemTypeExtension> extensions,
        WorkItemTypeExtensionService.ExtensionMatcher prototypeExtensionMatcher,
        IEnumerable<WorkItemTypeExtension> removedExtensions,
        IEnumerable<WorkItemTypeExtension> addedExtensions)
      {
        this.m_extensions = extensions;
        IFieldTypeDictionary fieldsSnapshot = requestContext.GetService<WorkItemTrackingFieldService>().GetFieldsSnapshot(requestContext);
        this.UpdateReferencedFieldIds();
        if (prototypeExtensionMatcher != null)
        {
          this.CopyPartitionTable(prototypeExtensionMatcher);
          if (removedExtensions != null && removedExtensions.Any<WorkItemTypeExtension>())
          {
            foreach (WorkItemTypeExtension removedExtension in removedExtensions)
              this.RemoveExtensionFromPartitions(removedExtension, fieldsSnapshot);
          }
          if (addedExtensions == null || !addedExtensions.Any<WorkItemTypeExtension>())
            return;
          foreach (WorkItemTypeExtension addedExtension in addedExtensions)
            this.AddExtensionToPartitions(addedExtension, fieldsSnapshot);
        }
        else
          this.BuildPartitionTable(fieldsSnapshot);
      }

      public IEnumerable<int> GetReferencedFieldIds() => (IEnumerable<int>) this.m_referencedFieldIds;

      public IEnumerable<WorkItemTypeExtension> GetMatchingExtensions(
        IExtendedPredicateEvaluationContext predicateEvaluator)
      {
        HashSet<WorkItemTypeExtension> source = new HashSet<WorkItemTypeExtension>();
        foreach (WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookupKey partitionLookupKey in this.m_partitionLookupKeys)
        {
          WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookupBase partitionLookupBase;
          if (this.m_partitionLookups.TryGetValue(partitionLookupKey, out partitionLookupBase))
            source.UnionWith(partitionLookupBase.GetTargetExtensions(predicateEvaluator));
        }
        source.UnionWith((IEnumerable<WorkItemTypeExtension>) this.m_alwaysInclude);
        return (IEnumerable<WorkItemTypeExtension>) source.Where<WorkItemTypeExtension>((Func<WorkItemTypeExtension, bool>) (extension => extension.IsPredicateMatch((IPredicateEvaluationHelper) predicateEvaluator))).ToArray<WorkItemTypeExtension>();
      }

      public IEnumerable<WorkItemTypeExtension> GetAllExtensions() => (IEnumerable<WorkItemTypeExtension>) this.m_extensions.Values;

      public IEnumerable<WorkItemTypeExtension> GetActiveExtensions(
        IExtendedPredicateEvaluationContext predicateEvaluator)
      {
        return (IEnumerable<WorkItemTypeExtension>) this.GetAllExtensions().Where<WorkItemTypeExtension>((Func<WorkItemTypeExtension, bool>) (extension =>
        {
          object fieldValue = predicateEvaluator.GetFieldValue(extension.MarkerField.Field.FieldId);
          if (fieldValue == null)
            return false;
          return fieldValue is bool flag2 ? flag2 : Convert.ToBoolean(fieldValue);
        })).ToArray<WorkItemTypeExtension>();
      }

      private void UpdateReferencedFieldIds()
      {
        this.m_referencedFieldIds = new HashSet<int>();
        foreach (WorkItemTypeExtension itemTypeExtension in this.m_extensions.Values)
        {
          if (itemTypeExtension.Predicate != null)
            this.m_referencedFieldIds.UnionWith(itemTypeExtension.Predicate.GetReferencedFields());
          this.m_referencedFieldIds.UnionWith(itemTypeExtension.GetReferencedFieldsByRules());
        }
      }

      private void CopyPartitionTable(
        WorkItemTypeExtensionService.ExtensionMatcher prototypeExtensionMatcher)
      {
        this.m_partitionLookupKeys = prototypeExtensionMatcher.m_partitionLookupKeys;
        this.m_alwaysInclude = new HashSet<WorkItemTypeExtension>((IEnumerable<WorkItemTypeExtension>) prototypeExtensionMatcher.m_alwaysInclude);
        this.m_partitionLookups = new Dictionary<WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookupKey, WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookupBase>();
        foreach (KeyValuePair<WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookupKey, WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookupBase> partitionLookup in prototypeExtensionMatcher.m_partitionLookups)
          this.m_partitionLookups.Add(partitionLookup.Key, partitionLookup.Value.Clone());
      }

      private void BuildPartitionTable(IFieldTypeDictionary fieldDict)
      {
        this.m_partitionLookupKeys = this.PickMostSelectivePartitionFields();
        foreach (WorkItemTypeExtension extension in this.m_extensions.Values)
          this.AddExtensionToPartitions(extension, fieldDict);
      }

      private WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookupKey[] PickMostSelectivePartitionFields()
      {
        Dictionary<WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookupKey, HashSet<int>> source = new Dictionary<WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookupKey, HashSet<int>>();
        foreach (WorkItemTypeExtension itemTypeExtension in this.m_extensions.Values)
        {
          foreach (WorkItemTypeExtensionPredicateNode[] extensionPredicateNodeArray in itemTypeExtension.GetNormalizedPredicate())
          {
            foreach (WorkItemTypeExtensionPredicateNode extensionPredicateNode in extensionPredicateNodeArray)
            {
              if (!extensionPredicateNode.Inverted)
              {
                int fieldId = extensionPredicateNode.FieldId;
                if (fieldId != 0)
                {
                  PredicateFieldComparisonOperator comparisonOperator = extensionPredicateNode.Operator;
                  WorkItemTypeExtensionService.ExtensionMatcher.PartitionComparisonType comparisonType = comparisonOperator is PredicateUnderOperator ? WorkItemTypeExtensionService.ExtensionMatcher.PartitionComparisonType.Under : WorkItemTypeExtensionService.ExtensionMatcher.PartitionComparisonType.Equals;
                  if (comparisonOperator.Value != null && (comparisonType == WorkItemTypeExtensionService.ExtensionMatcher.PartitionComparisonType.Under || comparisonOperator is PredicateEqualsOperator))
                  {
                    WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookupKey key = new WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookupKey(fieldId, comparisonType);
                    HashSet<int> intSet;
                    if (!source.TryGetValue(key, out intSet))
                    {
                      intSet = new HashSet<int>();
                      source.Add(key, intSet);
                    }
                    intSet.Add(extensionPredicateNode.Operator.Value.GetHashCode());
                  }
                }
              }
            }
          }
        }
        return source.OrderByDescending<KeyValuePair<WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookupKey, HashSet<int>>, int>((Func<KeyValuePair<WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookupKey, HashSet<int>>, int>) (pair => pair.Value.Count)).Select<KeyValuePair<WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookupKey, HashSet<int>>, WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookupKey>((Func<KeyValuePair<WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookupKey, HashSet<int>>, WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookupKey>) (pair => pair.Key)).Take<WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookupKey>(3).ToArray<WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookupKey>();
      }

      private void AddExtensionToPartitions(
        WorkItemTypeExtension extension,
        IFieldTypeDictionary fieldDict)
      {
        IEnumerable<Tuple<WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookupBase, object>> source = this.SelectPartitionLookups(extension, fieldDict);
        if (source.Any<Tuple<WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookupBase, object>>())
        {
          foreach (Tuple<WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookupBase, object> tuple in source)
            tuple.Item1.AddExtension(tuple.Item2, extension);
        }
        else
          this.m_alwaysInclude.Add(extension);
      }

      private void RemoveExtensionFromPartitions(
        WorkItemTypeExtension extension,
        IFieldTypeDictionary fieldDict)
      {
        IEnumerable<Tuple<WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookupBase, object>> source = this.SelectPartitionLookups(extension, fieldDict);
        if (source.Any<Tuple<WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookupBase, object>>())
        {
          foreach (Tuple<WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookupBase, object> tuple in source)
            tuple.Item1.RemoveExtension(tuple.Item2, extension);
        }
        this.m_alwaysInclude.Remove(extension);
      }

      private IEnumerable<Tuple<WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookupBase, object>> SelectPartitionLookups(
        WorkItemTypeExtension extension,
        IFieldTypeDictionary fieldDict)
      {
        WorkItemTypeExtensionPredicateNode[] array = ((IEnumerable<WorkItemTypeExtensionPredicateNode[]>) extension.GetNormalizedPredicate()).Select<WorkItemTypeExtensionPredicateNode[], WorkItemTypeExtensionPredicateNode>((Func<WorkItemTypeExtensionPredicateNode[], WorkItemTypeExtensionPredicateNode>) (path =>
        {
          foreach (WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookupKey partitionLookupKey in this.m_partitionLookupKeys)
          {
            WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookupKey partitionFieldKey = partitionLookupKey;
            WorkItemTypeExtensionPredicateNode extensionPredicateNode = ((IEnumerable<WorkItemTypeExtensionPredicateNode>) path).FirstOrDefault<WorkItemTypeExtensionPredicateNode>((Func<WorkItemTypeExtensionPredicateNode, bool>) (n =>
            {
              if (n.Inverted || n.FieldId != partitionFieldKey.FieldId)
                return false;
              return partitionFieldKey.ComparisonType == WorkItemTypeExtensionService.ExtensionMatcher.PartitionComparisonType.Under ? n.Operator is PredicateUnderOperator : n.Operator is PredicateEqualsOperator;
            }));
            if (extensionPredicateNode != null)
              return extensionPredicateNode;
          }
          return (WorkItemTypeExtensionPredicateNode) null;
        })).ToArray<WorkItemTypeExtensionPredicateNode>();
        if (!((IEnumerable<WorkItemTypeExtensionPredicateNode>) array).All<WorkItemTypeExtensionPredicateNode>((Func<WorkItemTypeExtensionPredicateNode, bool>) (n => n != null)))
          return Enumerable.Empty<Tuple<WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookupBase, object>>();
        List<Tuple<WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookupBase, object>> tupleList = new List<Tuple<WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookupBase, object>>();
        foreach (WorkItemTypeExtensionPredicateNode extensionPredicateNode in array)
        {
          WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookupKey key = new WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookupKey(extensionPredicateNode.FieldId, extensionPredicateNode.Operator is PredicateUnderOperator ? WorkItemTypeExtensionService.ExtensionMatcher.PartitionComparisonType.Under : WorkItemTypeExtensionService.ExtensionMatcher.PartitionComparisonType.Equals);
          WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookupBase partitionLookupBase;
          if (!this.m_partitionLookups.TryGetValue(key, out partitionLookupBase))
          {
            switch (fieldDict.GetField(extensionPredicateNode.FieldId).FieldType)
            {
              case InternalFieldType.String:
              case InternalFieldType.PlainText:
              case InternalFieldType.Html:
              case InternalFieldType.TreePath:
              case InternalFieldType.History:
                partitionLookupBase = (WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookupBase) new WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookup<string>(key, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
                break;
              case InternalFieldType.Integer:
                partitionLookupBase = (WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookupBase) new WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookup<int>(key, (IEqualityComparer<int>) null);
                break;
              case InternalFieldType.DateTime:
                partitionLookupBase = (WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookupBase) new WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookup<DateTime>(key, (IEqualityComparer<DateTime>) null);
                break;
              case InternalFieldType.Double:
                partitionLookupBase = (WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookupBase) new WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookup<double>(key, (IEqualityComparer<double>) null);
                break;
              case InternalFieldType.Guid:
                partitionLookupBase = (WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookupBase) new WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookup<Guid>(key, (IEqualityComparer<Guid>) null);
                break;
              case InternalFieldType.Boolean:
                partitionLookupBase = (WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookupBase) new WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookup<bool>(key, (IEqualityComparer<bool>) null);
                break;
              default:
                partitionLookupBase = (WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookupBase) new WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookup<string>(key, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
                break;
            }
            this.m_partitionLookups.Add(key, partitionLookupBase);
          }
          tupleList.Add(new Tuple<WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookupBase, object>(partitionLookupBase, extensionPredicateNode.Operator.Value));
        }
        return (IEnumerable<Tuple<WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookupBase, object>>) tupleList;
      }

      private enum PartitionComparisonType
      {
        Equals,
        Under,
      }

      private struct PartitionLookupKey
      {
        private int m_fieldId;
        private WorkItemTypeExtensionService.ExtensionMatcher.PartitionComparisonType m_comparisonType;

        public PartitionLookupKey(
          int fieldId,
          WorkItemTypeExtensionService.ExtensionMatcher.PartitionComparisonType comparisonType)
        {
          this.m_fieldId = fieldId;
          this.m_comparisonType = comparisonType;
        }

        public int FieldId => this.m_fieldId;

        public WorkItemTypeExtensionService.ExtensionMatcher.PartitionComparisonType ComparisonType => this.m_comparisonType;

        public override int GetHashCode() => CommonUtils.CombineHashCodes(this.m_fieldId, (int) this.m_comparisonType);

        public override bool Equals(object obj) => obj is WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookupKey partitionLookupKey && partitionLookupKey.FieldId == this.m_fieldId && partitionLookupKey.ComparisonType == this.m_comparisonType;
      }

      private abstract class PartitionLookupBase : ICloneable
      {
        public PartitionLookupBase(
          WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookupKey key)
        {
          this.Key = key;
        }

        public WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookupKey Key { get; private set; }

        public abstract IEnumerable<WorkItemTypeExtension> GetTargetExtensions(
          IExtendedPredicateEvaluationContext evalHelper);

        public abstract void AddExtension(object value, WorkItemTypeExtension extension);

        public abstract void RemoveExtension(object value, WorkItemTypeExtension extension);

        public virtual WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookupBase Clone() => (WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookupBase) this.MemberwiseClone();

        object ICloneable.Clone() => (object) this.Clone();
      }

      private class PartitionLookup<T> : 
        WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookupBase
      {
        private Dictionary<T, HashSet<WorkItemTypeExtension>> m_targets;
        private HashSet<WorkItemTypeExtension> m_defaultTargetExtensions;

        public PartitionLookup(
          WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookupKey key,
          IEqualityComparer<T> comparer)
          : base(key)
        {
          this.m_targets = new Dictionary<T, HashSet<WorkItemTypeExtension>>(comparer);
          this.m_defaultTargetExtensions = new HashSet<WorkItemTypeExtension>();
        }

        public override IEnumerable<WorkItemTypeExtension> GetTargetExtensions(
          IExtendedPredicateEvaluationContext evalHelper)
        {
          object fieldValue = evalHelper.GetFieldValue(this.Key.FieldId);
          if (this.Key.ComparisonType == WorkItemTypeExtensionService.ExtensionMatcher.PartitionComparisonType.Under)
          {
            switch (this.Key.FieldId)
            {
              case -105:
              case -7:
                HashSet<WorkItemTypeExtension> targetExtensions1 = new HashSet<WorkItemTypeExtension>();
                string path = CommonWITUtils.ConvertValue<string>(fieldValue);
                if (!string.IsNullOrEmpty(path))
                {
                  int treeId = evalHelper.GetTreeId(path, this.Key.FieldId == -7 ? TreeStructureType.Area : TreeStructureType.Iteration);
                  if (treeId >= 0)
                  {
                    for (TreeNode treeNode = evalHelper.GetTreeNode(treeId); treeNode != null; treeNode = treeNode.Parent)
                    {
                      string treePath = evalHelper.GetTreePath(treeNode.Id);
                      targetExtensions1.UnionWith(this.GetTargets((T) treePath));
                    }
                  }
                }
                return (IEnumerable<WorkItemTypeExtension>) targetExtensions1;
              case -104:
              case -2:
                int treeId1 = CommonWITUtils.ConvertValue<int>(fieldValue);
                HashSet<WorkItemTypeExtension> targetExtensions2 = new HashSet<WorkItemTypeExtension>();
                for (TreeNode treeNode = evalHelper.GetTreeNode(treeId1); treeNode != null; treeNode = treeNode.Parent)
                  targetExtensions2.UnionWith(this.GetTargets((T) (ValueType) treeNode.Id));
                return (IEnumerable<WorkItemTypeExtension>) targetExtensions2;
            }
          }
          return this.GetTargets(CommonWITUtils.ConvertValue<T>(fieldValue));
        }

        private IEnumerable<WorkItemTypeExtension> GetTargets(T value)
        {
          if ((object) value == null)
            return (IEnumerable<WorkItemTypeExtension>) this.m_defaultTargetExtensions;
          HashSet<WorkItemTypeExtension> itemTypeExtensionSet;
          return this.m_targets.TryGetValue(value, out itemTypeExtensionSet) ? (IEnumerable<WorkItemTypeExtension>) itemTypeExtensionSet : Enumerable.Empty<WorkItemTypeExtension>();
        }

        public override void AddExtension(object partitionKey, WorkItemTypeExtension extension)
        {
          T key = CommonWITUtils.ConvertValue<T>(partitionKey);
          HashSet<WorkItemTypeExtension> itemTypeExtensionSet;
          if ((object) key == null)
            itemTypeExtensionSet = this.m_defaultTargetExtensions;
          else if (!this.m_targets.TryGetValue(key, out itemTypeExtensionSet))
          {
            itemTypeExtensionSet = new HashSet<WorkItemTypeExtension>();
            this.m_targets.Add(key, itemTypeExtensionSet);
          }
          itemTypeExtensionSet.Add(extension);
        }

        public override void RemoveExtension(object partitionKey, WorkItemTypeExtension extension)
        {
          T key = CommonWITUtils.ConvertValue<T>(partitionKey);
          if (partitionKey == null)
          {
            this.m_defaultTargetExtensions.Remove(extension);
          }
          else
          {
            HashSet<WorkItemTypeExtension> itemTypeExtensionSet;
            if (!this.m_targets.TryGetValue(key, out itemTypeExtensionSet))
              return;
            itemTypeExtensionSet.Remove(extension);
            if (itemTypeExtensionSet.Count != 0)
              return;
            this.m_targets.Remove(key);
          }
        }

        public override WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookupBase Clone()
        {
          WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookup<T> partitionLookup = base.Clone() as WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookup<T>;
          partitionLookup.m_targets = new Dictionary<T, HashSet<WorkItemTypeExtension>>(this.m_targets.Comparer);
          foreach (KeyValuePair<T, HashSet<WorkItemTypeExtension>> target in this.m_targets)
            partitionLookup.m_targets.Add(target.Key, new HashSet<WorkItemTypeExtension>((IEnumerable<WorkItemTypeExtension>) target.Value));
          partitionLookup.m_defaultTargetExtensions = new HashSet<WorkItemTypeExtension>((IEnumerable<WorkItemTypeExtension>) this.m_defaultTargetExtensions);
          return (WorkItemTypeExtensionService.ExtensionMatcher.PartitionLookupBase) partitionLookup;
        }
      }
    }
  }
}
