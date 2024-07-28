// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.Services.AggregationStore
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Microsoft.TeamFoundation.CodeSense.Common;
using Microsoft.TeamFoundation.CodeSense.Platform.Abstraction;
using Microsoft.TeamFoundation.CodeSense.Server.Contracts;
using Microsoft.TeamFoundation.CodeSense.Server.Data;
using Microsoft.TeamFoundation.CodeSense.Server.Extensions;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Microsoft.TeamFoundation.CodeSense.Server.Services
{
  public sealed class AggregationStore : IAggregationStore
  {
    private static readonly Guid AggregatorJobGuid = new Guid("93D4DB79-9357-4B47-BF5A-ED4139FC87B5");
    private readonly IFileDataService fileDataService;
    private static readonly Lazy<AggregationStore> s_instance = new Lazy<AggregationStore>((Func<AggregationStore>) (() => new AggregationStore(FileDataService.GetInstance())), LazyThreadSafetyMode.ExecutionAndPublication);

    public static AggregationStore GetInstance() => AggregationStore.s_instance.Value;

    internal AggregationStore(IFileDataService fileDataService) => this.fileDataService = fileDataService;

    public static void StartAggregatorJob(IVssRequestContext requestContext)
    {
      int num = 0;
      using (new CodeSenseTraceWatch(requestContext, 1025025, TraceLayer.ExternalFramework, "Queueing AggregatorJob", Array.Empty<object>()))
        num = requestContext.GetService<TeamFoundationJobService>().QueueJobsNow(requestContext, (IEnumerable<Guid>) new Guid[1]
        {
          AggregationStore.AggregatorJobGuid
        });
      if (num <= 0)
        return;
      requestContext.Trace(1025005, TraceLayer.Job, "Queued aggregation job");
    }

    IAggregateReader IAggregationStore.CreateReader(IVssRequestContext requestContext) => (IAggregateReader) this.CreateReaderWriter(requestContext);

    IAggregateWriter IAggregationStore.CreateWriter(
      IVssRequestContext requestContext,
      SliceSource source)
    {
      return (IAggregateWriter) new SliceWriter(requestContext, this.fileDataService, source);
    }

    public AggregationStore.AggregateReaderWriter CreateReaderWriter(
      IVssRequestContext requestContext)
    {
      return new AggregationStore.AggregateReaderWriter(requestContext, this.fileDataService);
    }

    public sealed class AggregateReaderWriter : IAggregateReader, IDisposable
    {
      private const string WorkItemsNodeString = "workItems";
      private const string SummaryWorkItemIdNodeString = "workItemId";
      private const string DetailsWorkItemIdNodeString = "id";
      private const string RestrictedBranchFormat = "RestrictedBranch_{0}";
      private const string ChangesNodeId = "Microsoft.Changes";
      private const string VersionOneString = "1.0";
      private static readonly TimeSpan FlushFrequency = TimeSpan.FromMinutes(10.0);
      private readonly IVssRequestContext requestContext;
      private readonly IFileDataService fileDataService;
      private readonly List<SliceDescriptor> slicesToRemove = new List<SliceDescriptor>();
      private readonly Dictionary<string, AggregateDescriptor> aggregatesToAdd = new Dictionary<string, AggregateDescriptor>((IEqualityComparer<string>) TFStringComparer.VersionControlPath);
      private readonly HashSet<int> fileIdsToDelete = new HashSet<int>();
      private readonly Stopwatch lastFlushStopWatch = new Stopwatch();
      private long storageGrowthCounter;
      private ProjectMapCache projectMapCache = new ProjectMapCache();

      public AggregateReaderWriter(
        IVssRequestContext requestContext,
        IFileDataService fileDataService)
      {
        this.requestContext = requestContext;
        this.fileDataService = fileDataService;
      }

      public int LatestChangesetRemoved { get; private set; }

      public void RemoveSlices(IEnumerable<SliceDescriptor> sliceDescriptors)
      {
        this.slicesToRemove.AddRange(sliceDescriptors);
        this.FlushIfNeeded();
      }

      public void WriteAggregate(AggregateV3 aggregate, string path)
      {
        aggregate.Metadata.UpdateTimeStamp();
        long length;
        int fileId = this.fileDataService.PersistData<AggregateV3>(this.requestContext, aggregate, out length);
        this.ManageStorageGrowth(length);
        foreach (string contributingServerPath in aggregate.ContributingServerPaths)
        {
          string embeddedProjectGuid = contributingServerPath.GetEmbeddedProjectGuid();
          Guid empty = Guid.Empty;
          ref Guid local = ref empty;
          if (Guid.TryParse(embeddedProjectGuid, out local))
          {
            if (!empty.Equals(Guid.Empty))
            {
              if (this.aggregatesToAdd.ContainsKey(contributingServerPath))
                this.fileIdsToDelete.Add(this.aggregatesToAdd[contributingServerPath].FileId);
              this.aggregatesToAdd[contributingServerPath] = new AggregateDescriptor(contributingServerPath, fileId, empty);
            }
          }
          else
          {
            string str = string.Format("Path : {0} does not belong to an existing project. Skipping aggregate update.", (object) contributingServerPath);
            this.requestContext.Trace(1024475, "CodeSense", TraceLayer.Job, (object) str);
          }
        }
        this.FlushIfNeeded();
      }

      public VersionedContent GetDetailsAggregate(
        string path,
        CodeSenseResourceVersion targetResourceVersion = CodeSenseResourceVersion.Dev12Update3)
      {
        string projectName = path.GetProjectName();
        Guid projectId = ProjectServiceHelper.GetProjectId(this.requestContext, projectName, this.projectMapCache);
        if (projectId.Equals(Guid.Empty))
          return (VersionedContent) null;
        string pathToCompare = path.GetRelativePath().GetCompletePath(projectId.ToString());
        AggregateDescriptor aggregateDescriptor = this.GetAggregateDescriptor(pathToCompare);
        if (aggregateDescriptor == null)
          return (VersionedContent) null;
        var exampleObject = new
        {
          Metadata = new
          {
            AggregateVersion = 0,
            BranchLinks = (List<BranchLinkData>) null,
            IncludedChanges = (object) null,
            TimeStamp = new DateTime()
          },
          Details = (string) null,
          CodeElements = new CodeElementIdentityCollectionV3(),
          SourceControlData = new SourceControlDataV3(),
          UnrestrictedDetails = (string) null
        };
        var dataOfAnonymousType = this.GetDataOfAnonymousType(aggregateDescriptor.FileId, exampleObject);
        if (!this.IsAggregateUpdated(dataOfAnonymousType.Metadata.TimeStamp))
          return new VersionedContent(string.Empty, targetResourceVersion, false);
        ITeamFoundationSecurityServiceProxy service = (ITeamFoundationSecurityServiceProxy) this.requestContext.GetService<TeamFoundationSecurityServiceProxy>();
        service.CheckPermissions(this.requestContext, SecurityConstants.RepositorySecurity2NamespaceGuid, pathToCompare, 1);
        string json = dataOfAnonymousType.Details;
        CodeSenseResourceVersion version = CodeSenseResourceVersion.Dev12RTM;
        if (dataOfAnonymousType.Metadata.AggregateVersion != 7)
          ConverterUtilities.UpdateBranchMapData<BranchLinkData>(this.requestContext, (IEnumerable<BranchLinkData>) dataOfAnonymousType.Metadata.BranchLinks, this.projectMapCache);
        if (dataOfAnonymousType.Metadata.AggregateVersion >= 6)
        {
          if (targetResourceVersion == CodeSenseResourceVersion.Dev12Update3)
            return ResponseCreator.CreateV3Response(this.requestContext, dataOfAnonymousType.Details, dataOfAnonymousType.Metadata.TimeStamp == new DateTime() ? AggregateV3.DefaultTimeStamp : dataOfAnonymousType.Metadata.TimeStamp, dataOfAnonymousType.Metadata.AggregateVersion, dataOfAnonymousType.Metadata.BranchLinks, dataOfAnonymousType.CodeElements, dataOfAnonymousType.SourceControlData, false, path, projectId, this.projectMapCache);
          json = AggregateDetailsConverter.GetV2Details(dataOfAnonymousType.Details, dataOfAnonymousType.CodeElements, dataOfAnonymousType.SourceControlData);
          ConverterUtilities.FillBranchLinks((IEnumerable<BranchLinkData>) dataOfAnonymousType.Metadata.BranchLinks, dataOfAnonymousType.SourceControlData);
        }
        string summary;
        if (dataOfAnonymousType.Metadata.AggregateVersion > 3)
        {
          string str = this.FilterDetailsOnPermissions(json, out int _);
          var anonymousTypeObject = new
          {
            ResourceVersion = (string) null,
            BranchList = (List<BranchDetailsResult>) null
          };
          var data = JsonConvert.DeserializeAnonymousType(str, anonymousTypeObject);
          FilterUtilities.ByRetentionV2Details(this.requestContext, data.BranchList, dataOfAnonymousType.Metadata.BranchLinks);
          if (dataOfAnonymousType.Metadata.AggregateVersion != 7)
            ConverterUtilities.ReplaceServerPathsWithGuid<BranchDetailsResult>(this.requestContext, data.BranchList, this.projectMapCache);
          if (targetResourceVersion == CodeSenseResourceVersion.Dev12RTM)
          {
            IEnumerable<FileChangeAggregateResult> first = Enumerable.Empty<FileChangeAggregateResult>();
            BranchDetailsResult branchDetailsResult = data.BranchList.SingleOrDefault<BranchDetailsResult>((Func<BranchDetailsResult, bool>) (branch => TFStringComparer.VersionControlPath.Equals(pathToCompare, branch.ServerPath)));
            if (branchDetailsResult != null)
              first = (IEnumerable<FileChangeAggregateResult>) branchDetailsResult.Details;
            foreach (BranchLinkData codeflowChange in AggregationStore.AggregateReaderWriter.GetCodeflowChanges((IEnumerable<BranchLinkData>) dataOfAnonymousType.Metadata.BranchLinks, pathToCompare))
            {
              Dictionary<CodeElementIdentity, Tuple<ElementChangeKind, CodeElementKind>> codeElementsToAdd = new Dictionary<CodeElementIdentity, Tuple<ElementChangeKind, CodeElementKind>>();
              AggregationStore.AggregateReaderWriter.GetCodeElementsToAddForDetails((IEnumerable<BranchDetailsResult>) data.BranchList, (IEnumerable<BranchLinkData>) dataOfAnonymousType.Metadata.BranchLinks, codeflowChange, codeElementsToAdd);
              List<CodeElementDetailsResult> newCodeElements = AggregationStore.AggregateReaderWriter.GetNewCodeElements(codeflowChange, codeElementsToAdd);
              if (newCodeElements.Count > 0)
                first = first.Concat<FileChangeAggregateResult>((IEnumerable<FileChangeAggregateResult>) new FileChangeAggregateResult[1]
                {
                  new FileChangeAggregateResult((IEnumerable<CodeElementDetailsResult>) newCodeElements)
                });
            }
            summary = JsonConvert.SerializeObject((object) new
            {
              ResourceVersion = "1.1",
              ServerPath = path,
              Details = first
            }, CodeSenseSerializationSettings.JsonSerializerSettings);
          }
          else
          {
            version = CodeSenseResourceVersion.Dev12Update1;
            List<BranchLinkData> branchLinkData = dataOfAnonymousType.Metadata.BranchLinks;
            ICollection<KeyValuePair<string, JToken>> source = (ICollection<KeyValuePair<string, JToken>>) dataOfAnonymousType.Metadata.IncludedChanges;
            if (dataOfAnonymousType.Metadata.AggregateVersion != 7)
            {
              List<KeyValuePair<string, JToken>> keyValuePairList = new List<KeyValuePair<string, JToken>>();
              foreach (KeyValuePair<string, JToken> keyValuePair1 in (IEnumerable<KeyValuePair<string, JToken>>) source)
              {
                KeyValuePair<string, JToken> keyValuePair2 = new KeyValuePair<string, JToken>(keyValuePair1.Key.ReplaceProjectNameWithGuid(this.requestContext, this.projectMapCache), keyValuePair1.Value);
                keyValuePairList.Add(keyValuePair2);
              }
              source = (ICollection<KeyValuePair<string, JToken>>) keyValuePairList;
            }
            Dictionary<string, bool> permissions = service.ObtainPermissions(this.requestContext, SecurityConstants.RepositorySecurity2NamespaceGuid, source.Select<KeyValuePair<string, JToken>, string>((Func<KeyValuePair<string, JToken>, string>) (item => item.Key)), 1);
            if (permissions.Any<KeyValuePair<string, bool>>((Func<KeyValuePair<string, bool>, bool>) (tokenPermissionPair => !tokenPermissionPair.Value)))
            {
              using (new CodeSenseTraceWatch(this.requestContext, 1025035, TraceLayer.ExternalFramework, "Stripping branches from details aggregate: '{0}'", new object[1]
              {
                (object) path
              }))
              {
                data.BranchList.RemoveAll((Predicate<BranchDetailsResult>) (branch => !permissions.Single<KeyValuePair<string, bool>>((Func<KeyValuePair<string, bool>, bool>) (permissionedItem => TFStringComparer.VersionControlPath.Equals(branch.ServerPath, permissionedItem.Key))).Value));
                branchLinkData = AggregationStore.AggregateReaderWriter.SanitizeBranchLinkData(branchLinkData, permissions);
              }
            }
            data.BranchList.UpdateServerPaths<BranchDetailsResult>(this.requestContext, projectName, projectId, this.projectMapCache);
            branchLinkData.UpdateBranchMapPaths<BranchLinkData>(this.requestContext, projectName, projectId, this.projectMapCache);
            summary = JsonConvert.SerializeObject((object) new
            {
              BranchMap = branchLinkData,
              ResourceVersion = "2.0",
              BranchList = data.BranchList
            }, CodeSenseSerializationSettings.JsonSerializerSettings);
          }
        }
        else
        {
          int filteredCount;
          summary = this.FilterDetailsOnPermissions(json, out filteredCount);
          if (filteredCount > 0)
          {
            summary = dataOfAnonymousType.UnrestrictedDetails;
            VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.CodeSense.Server.Shared.PerformanceCounters.RestrictedDataStrippedToFilesRead").Increment();
          }
        }
        return new VersionedContent(this.InsertIndexingStatus(summary), version);
      }

      public VersionedContent GetSummaryAggregate(
        string path,
        CodeSenseResourceVersion targetResourceVersion = CodeSenseResourceVersion.Dev12Update3)
      {
        string projectName = path.GetProjectName();
        Guid projectId = ProjectServiceHelper.GetProjectId(this.requestContext, projectName, this.projectMapCache);
        if (projectId.Equals(Guid.Empty))
          return (VersionedContent) null;
        string pathToCompare = path.GetRelativePath().GetCompletePath(projectId.ToString());
        AggregateDescriptor aggregateDescriptor = this.GetAggregateDescriptor(pathToCompare);
        if (aggregateDescriptor == null)
          return (VersionedContent) null;
        var exampleObject = new
        {
          Metadata = new
          {
            AggregateVersion = 0,
            BranchLinks = (List<BranchLinkData>) null,
            IncludedChanges = (object) null,
            TimeStamp = new DateTime()
          },
          Summary = (string) null,
          Details = (string) null,
          CodeElements = new CodeElementIdentityCollectionV3(),
          SourceControlData = new SourceControlDataV3(),
          UnrestrictedSummary = (string) null
        };
        var dataOfAnonymousType = this.GetDataOfAnonymousType(aggregateDescriptor.FileId, exampleObject);
        if (!this.IsAggregateUpdated(dataOfAnonymousType.Metadata.TimeStamp))
          return new VersionedContent(string.Empty, targetResourceVersion, false);
        ITeamFoundationSecurityServiceProxy service = (ITeamFoundationSecurityServiceProxy) this.requestContext.GetService<TeamFoundationSecurityServiceProxy>();
        service.CheckPermissions(this.requestContext, SecurityConstants.RepositorySecurity2NamespaceGuid, pathToCompare, 1);
        string json = dataOfAnonymousType.Summary;
        string details = dataOfAnonymousType.Details;
        CodeSenseResourceVersion version = CodeSenseResourceVersion.Dev12RTM;
        if (dataOfAnonymousType.Metadata.AggregateVersion != 7)
          ConverterUtilities.UpdateBranchMapData<BranchLinkData>(this.requestContext, (IEnumerable<BranchLinkData>) dataOfAnonymousType.Metadata.BranchLinks, this.projectMapCache);
        if (dataOfAnonymousType.Metadata.AggregateVersion >= 6)
        {
          if (targetResourceVersion == CodeSenseResourceVersion.Dev12Update3)
            return ResponseCreator.CreateV3Response(this.requestContext, dataOfAnonymousType.Details, dataOfAnonymousType.Metadata.TimeStamp == new DateTime() ? AggregateV3.DefaultTimeStamp : dataOfAnonymousType.Metadata.TimeStamp, dataOfAnonymousType.Metadata.AggregateVersion, dataOfAnonymousType.Metadata.BranchLinks, dataOfAnonymousType.CodeElements, dataOfAnonymousType.SourceControlData, true, path, projectId, this.projectMapCache);
          json = AggregateSummaryConverter.GetV2Summary(dataOfAnonymousType.Details, dataOfAnonymousType.CodeElements, dataOfAnonymousType.SourceControlData);
          details = AggregateDetailsConverter.GetV2Details(dataOfAnonymousType.Details, dataOfAnonymousType.CodeElements, dataOfAnonymousType.SourceControlData);
          ConverterUtilities.FillBranchLinks((IEnumerable<BranchLinkData>) dataOfAnonymousType.Metadata.BranchLinks, dataOfAnonymousType.SourceControlData);
        }
        string summary;
        if (dataOfAnonymousType.Metadata.AggregateVersion > 3)
        {
          string str = this.FilterSummaryOnPermissions(json, out int _);
          var anonymousTypeObject = new
          {
            ResourceVersion = (string) null,
            BranchList = (List<BranchSummaryResult>) null
          };
          var data = JsonConvert.DeserializeAnonymousType(str, anonymousTypeObject);
          FilterUtilities.ByRetentionV2Summary(this.requestContext, details, data.BranchList, dataOfAnonymousType.Metadata.BranchLinks);
          if (dataOfAnonymousType.Metadata.AggregateVersion != 7)
            ConverterUtilities.ReplaceServerPathsWithGuid<BranchSummaryResult>(this.requestContext, data.BranchList, this.projectMapCache);
          if (targetResourceVersion == CodeSenseResourceVersion.Dev12RTM)
          {
            IEnumerable<CodeElementSummaryResult> summaryResult = Enumerable.Empty<CodeElementSummaryResult>();
            BranchSummaryResult branchSummaryResult = data.BranchList.SingleOrDefault<BranchSummaryResult>((Func<BranchSummaryResult, bool>) (branch => TFStringComparer.VersionControlPath.Equals(pathToCompare, branch.ServerPath)));
            if (branchSummaryResult != null)
              summaryResult = (IEnumerable<CodeElementSummaryResult>) branchSummaryResult.CodeElements;
            foreach (BranchLinkData codeflowChange in AggregationStore.AggregateReaderWriter.GetCodeflowChanges((IEnumerable<BranchLinkData>) dataOfAnonymousType.Metadata.BranchLinks, pathToCompare))
            {
              Dictionary<CodeElementIdentity, CodeElementKind> codeElementsToAdd = new Dictionary<CodeElementIdentity, CodeElementKind>();
              AggregationStore.AggregateReaderWriter.GetCodeElementsToAddForSummary((IEnumerable<BranchSummaryResult>) data.BranchList, (IEnumerable<BranchLinkData>) dataOfAnonymousType.Metadata.BranchLinks, codeflowChange, codeElementsToAdd);
              summaryResult = this.AddCodeflowChangeSummary(summaryResult, codeflowChange, codeElementsToAdd);
            }
            summary = JsonConvert.SerializeObject((object) new
            {
              ResourceVersion = "1.1",
              ServerPath = path,
              CodeElements = summaryResult
            }, CodeSenseSerializationSettings.JsonSerializerSettings);
          }
          else
          {
            version = CodeSenseResourceVersion.Dev12Update1;
            List<BranchLinkData> branchLinkData = dataOfAnonymousType.Metadata.BranchLinks;
            ICollection<KeyValuePair<string, JToken>> source = (ICollection<KeyValuePair<string, JToken>>) dataOfAnonymousType.Metadata.IncludedChanges;
            if (dataOfAnonymousType.Metadata.AggregateVersion != 7)
            {
              List<KeyValuePair<string, JToken>> keyValuePairList = new List<KeyValuePair<string, JToken>>();
              foreach (KeyValuePair<string, JToken> keyValuePair1 in (IEnumerable<KeyValuePair<string, JToken>>) source)
              {
                KeyValuePair<string, JToken> keyValuePair2 = new KeyValuePair<string, JToken>(keyValuePair1.Key.ReplaceProjectNameWithGuid(this.requestContext, this.projectMapCache), keyValuePair1.Value);
                keyValuePairList.Add(keyValuePair2);
              }
              source = (ICollection<KeyValuePair<string, JToken>>) keyValuePairList;
            }
            Dictionary<string, bool> permissions = service.ObtainPermissions(this.requestContext, SecurityConstants.RepositorySecurity2NamespaceGuid, source.Select<KeyValuePair<string, JToken>, string>((Func<KeyValuePair<string, JToken>, string>) (item => item.Key)), 1);
            if (permissions.Any<KeyValuePair<string, bool>>((Func<KeyValuePair<string, bool>, bool>) (tokenPermissionPair => !tokenPermissionPair.Value)))
            {
              using (new CodeSenseTraceWatch(this.requestContext, 1025035, TraceLayer.ExternalFramework, "Stripping branches from details aggregate: '{0}'", new object[1]
              {
                (object) path
              }))
              {
                data.BranchList.RemoveAll((Predicate<BranchSummaryResult>) (branch => !permissions.Single<KeyValuePair<string, bool>>((Func<KeyValuePair<string, bool>, bool>) (permissionedItem => TFStringComparer.VersionControlPath.Equals(branch.ServerPath, permissionedItem.Key))).Value));
                branchLinkData = AggregationStore.AggregateReaderWriter.SanitizeBranchLinkData(branchLinkData, permissions);
              }
            }
            data.BranchList.UpdateServerPaths<BranchSummaryResult>(this.requestContext, projectName, projectId, this.projectMapCache);
            branchLinkData.UpdateBranchMapPaths<BranchLinkData>(this.requestContext, projectName, projectId, this.projectMapCache);
            summary = JsonConvert.SerializeObject((object) new
            {
              BranchMap = branchLinkData,
              ResourceVersion = "2.0",
              BranchList = data.BranchList
            }, CodeSenseSerializationSettings.JsonSerializerSettings);
          }
        }
        else
        {
          int filteredCount;
          summary = this.FilterSummaryOnPermissions(json, out filteredCount);
          if (filteredCount > 0)
          {
            summary = dataOfAnonymousType.UnrestrictedSummary;
            VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.CodeSense.Server.Shared.PerformanceCounters.RestrictedDataStrippedToFilesRead").Increment();
          }
        }
        return new VersionedContent(this.InsertIndexingStatus(summary), version);
      }

      public AggregateV3 GetAggregate(
        ProjectMapCache projectMapCache,
        string path,
        int retentionPeriod = -1)
      {
        AggregateDescriptor aggregateDescriptor = this.GetLatestAggregateDescriptor(path);
        if (aggregateDescriptor == null)
          return (AggregateV3) null;
        this.projectMapCache = projectMapCache;
        return this.GetAggregate(path, aggregateDescriptor.FileId, retentionPeriod);
      }

      public IEnumerable<AggregateV3> GetAggregateMergeCandidates(
        string targetPath,
        IEnumerable<string> contributingServerPaths,
        int retentionPeriod = -1)
      {
        List<AggregateV3> aggregateMergeCandidates = new List<AggregateV3>();
        if (contributingServerPaths.Any<string>())
        {
          AggregateDescriptor aggregateDescriptor1 = this.GetLatestAggregateDescriptor(targetPath);
          List<int> intList = new List<int>()
          {
            aggregateDescriptor1 == null ? -1 : aggregateDescriptor1.FileId
          };
          foreach (string str in contributingServerPaths.Except<string>((IEnumerable<string>) new string[1]
          {
            targetPath
          }, (IEqualityComparer<string>) TFStringComparer.VersionControlPath))
          {
            AggregateDescriptor aggregateDescriptor2 = this.GetLatestAggregateDescriptor(str);
            if (aggregateDescriptor2 == null)
              aggregateMergeCandidates.Add(AggregateV3.FromContributor(str));
            else if (!intList.Contains(aggregateDescriptor2.FileId))
            {
              AggregateV3 aggregate = this.GetAggregate(aggregateDescriptor2.Path, aggregateDescriptor2.FileId, retentionPeriod);
              if (aggregate != null)
              {
                aggregateMergeCandidates.Add(aggregate);
                intList.Add(aggregateDescriptor2.FileId);
              }
              else
                aggregateMergeCandidates.Add(AggregateV3.FromContributor(str));
            }
          }
        }
        return (IEnumerable<AggregateV3>) aggregateMergeCandidates;
      }

      public string GetSlices(int fileId) => this.fileDataService.GetData(this.requestContext, fileId);

      public IEnumerable<SliceDescriptor> PeekSlices(int count)
      {
        using (this.requestContext.AcquireExemptionLock())
        {
          using (ICodeSenseSqlResourceComponent component = this.requestContext.CreateComponent<ICodeSenseSqlResourceComponent, CodeSenseSqlResourceComponent>())
            return component.GetSlices(count);
        }
      }

      public void Flush()
      {
        using (this.requestContext.AcquireExemptionLock())
        {
          using (ICodeSenseSqlResourceComponent component = this.requestContext.CreateComponent<ICodeSenseSqlResourceComponent, CodeSenseSqlResourceComponent>())
          {
            if (this.aggregatesToAdd.Any<KeyValuePair<string, AggregateDescriptor>>())
            {
              try
              {
                this.requestContext.TraceConditionallyInChunks(1024505, TraceLevel.Info, "CodeSense", TraceLayer.Job, (Func<StringBuilder>) (() =>
                {
                  StringBuilder message = new StringBuilder("Current batch of aggregates: ");
                  this.aggregatesToAdd.Values.ForEach<AggregateDescriptor>((Action<AggregateDescriptor>) (x => message.AppendLine(string.Format("{0}-{1}", (object) x.Path, (object) x.FileId))));
                  return message;
                }));
                component.AddAggregates((IEnumerable<AggregateDescriptor>) this.aggregatesToAdd.Values);
                this.aggregatesToAdd.Clear();
              }
              catch (Exception ex)
              {
                int[] array = this.aggregatesToAdd.Values.Select<AggregateDescriptor, int>((Func<AggregateDescriptor, int>) (x => x.FileId)).ToArray<int>();
                this.requestContext.Trace(1024510, TraceLevel.Info, "CodeSense", TraceLayer.Job, string.Format("Add aggregates batch failed. Current batch of fileIds: {0}", (object) string.Join<int>(",", (IEnumerable<int>) array)));
                throw;
              }
            }
            if (this.slicesToRemove.Any<SliceDescriptor>())
            {
              component.RemoveSlices((IEnumerable<SliceDescriptor>) this.slicesToRemove);
              this.slicesToRemove.Clear();
            }
            if (this.fileIdsToDelete.Any<int>())
            {
              this.requestContext.Trace(1024515, TraceLevel.Info, "CodeSense", TraceLayer.Job, string.Format("FileIds added tbl_FilesToDelete table: {0}", (object) string.Join<int>(",", (IEnumerable<int>) this.fileIdsToDelete.ToArray<int>())));
              component.AddFilesToDelete((IEnumerable<int>) this.fileIdsToDelete);
              this.fileIdsToDelete.Clear();
            }
          }
        }
        this.lastFlushStopWatch.Stop();
        this.lastFlushStopWatch.Reset();
      }

      public void Dispose()
      {
        this.Flush();
        this.ManageStorageGrowth(0L, true);
      }

      private bool IsAggregateUpdated(DateTime serverTimeStamp)
      {
        if (this.requestContext.Items == null)
          return true;
        object obj;
        this.requestContext.Items.TryGetValue("timeStamp", out obj);
        if (!(obj is DateTime dateTime))
          return true;
        if (serverTimeStamp == new DateTime())
          serverTimeStamp = AggregateV3.DefaultTimeStamp;
        if (dateTime == serverTimeStamp)
          return false;
        if (dateTime > serverTimeStamp)
          this.requestContext.Trace(1025032, TraceLevel.Error, "CodeSense", TraceLayer.Service, "Client time stamp is higher than server time stamp");
        return true;
      }

      private void ManageStorageGrowth(long increment, bool forcedUpdate = false)
      {
        this.storageGrowthCounter += increment;
        this.requestContext.GetService<TeamFoundationCounterService>().UpdateStorageGrowth(this.requestContext, ref this.storageGrowthCounter, forcedUpdate);
      }

      internal static List<BranchLinkData> SanitizeBranchLinkData(
        List<BranchLinkData> branchLinkData,
        Dictionary<string, bool> permissions)
      {
        List<BranchLinkData> branchLinkDataList = new List<BranchLinkData>();
        int num = 0;
        Dictionary<string, string> dictionary = new Dictionary<string, string>((IEqualityComparer<string>) TFStringComparer.VersionControlPath);
        foreach (KeyValuePair<string, bool> keyValuePair in permissions.Where<KeyValuePair<string, bool>>((Func<KeyValuePair<string, bool>, bool>) (item => !item.Value)))
          dictionary.Add(keyValuePair.Key, string.Format("RestrictedBranch_{0}", (object) num++));
        foreach (BranchLinkData branchLinkData1 in branchLinkData)
        {
          string str1;
          bool flag1 = dictionary.TryGetValue(branchLinkData1.SourcePath, out str1);
          string str2;
          bool flag2 = dictionary.TryGetValue(branchLinkData1.TargetPath, out str2);
          if (!flag1 && !flag2)
            branchLinkDataList.Add(branchLinkData1);
          else
            branchLinkDataList.Add(new BranchLinkData(flag1 ? str1 : branchLinkData1.SourcePath, branchLinkData1.SourceChangesIdFrom, branchLinkData1.SourceChangesIdTo, flag2 ? str2 : branchLinkData1.TargetPath, branchLinkData1.TargetChangesId, branchLinkData1.ChangeType, string.Empty, string.Empty, string.Empty, string.Empty, branchLinkData1.Date));
        }
        return branchLinkDataList;
      }

      private AggregateV3 GetAggregate(string path, int fileId, int retentionPeriod)
      {
        AggregateV3 aggregate = (AggregateV3) null;
        string data = this.fileDataService.GetData(this.requestContext, fileId);
        if (data == null)
        {
          AggregateNotFoundException notFoundException = new AggregateNotFoundException(string.Format("Aggregate with fileId: '{0}' for path: '{1}' is not found.", (object) fileId, (object) path));
          this.requestContext.TraceException(1024495, "CodeSense", TraceLayer.Job, (Exception) notFoundException);
          throw notFoundException;
        }
        if (this.HasExceededAggregateSizeThreshold(data, path))
          return (AggregateV3) null;
        if (!string.IsNullOrEmpty(data))
        {
          aggregate = ConverterUtilities.Deserialize<AggregateV3>(this.requestContext, data);
          if (aggregate != null && aggregate.Metadata != null && aggregate.Metadata.AggregateVersion < 6)
          {
            if (aggregate.Metadata.AggregateVersion != 5)
              return (AggregateV3) null;
            Aggregate oldAggregate = ConverterUtilities.Deserialize<Aggregate>(this.requestContext, data);
            if (oldAggregate != null)
              aggregate = new AggregateV3(oldAggregate, retentionPeriod);
          }
          if (aggregate != null)
            this.LatestChangesetRemoved = Math.Max(this.LatestChangesetRemoved, aggregate.Filter(retentionPeriod));
        }
        if (aggregate != null && aggregate.Metadata != null && aggregate.Metadata.AggregateVersion < 7)
        {
          aggregate.Metadata.AggregateVersion = 7;
          aggregate.Metadata.IncludedChanges = ConverterUtilities.ReplaceServerPathsInChanges(this.requestContext, aggregate.Metadata.IncludedChanges, this.projectMapCache);
          ConverterUtilities.ReplaceServerPathsWithGuid<BranchDetailsResultV3>(this.requestContext, aggregate.Details.BranchList, this.projectMapCache);
          ConverterUtilities.UpdateBranchMapData<BranchLinkDataV3>(this.requestContext, (IEnumerable<BranchLinkDataV3>) aggregate.Metadata.BranchLinks, this.projectMapCache);
        }
        return aggregate;
      }

      private bool HasExceededAggregateSizeThreshold(string aggregateString, string path)
      {
        int sizeThresholdInMb = this.requestContext.GetService<IVssRegistryService>().GetAggregateFileSizeThresholdInMB(this.requestContext);
        if (aggregateString.Length * 2 <= sizeThresholdInMb * 1024 * 1024)
          return false;
        this.requestContext.Trace(1024490, TraceLevel.Error, "CodeSense", TraceLayer.Job, "Dropping the old aggregate for the file '{0}' since the size threshold is reached", (object) path);
        return true;
      }

      private AggregateDescriptor GetAggregateDescriptor(string path)
      {
        AggregateDescriptor aggregateDescriptor = (AggregateDescriptor) null;
        Guid result = Guid.Empty;
        if (!Guid.TryParse(path.GetEmbeddedProjectGuid(), out result))
          result = ProjectServiceHelper.GetProjectId(this.requestContext, path.GetProjectName());
        if (result.Equals(Guid.Empty))
        {
          string str = "GetAggregateDescriptor(): The contributing path '{0}' belongs to a non-existing project. Returning null";
          this.requestContext.Trace(1024480, "CodeSense", TraceLayer.Job, (object) str);
          return (AggregateDescriptor) null;
        }
        using (new CodeSenseTraceWatch(this.requestContext, 1025750, true, TraceLayer.ExternalSql, "GetAggregate {0}", new object[1]
        {
          (object) path
        }))
        {
          using (this.requestContext.AcquireExemptionLock())
          {
            using (ICodeSenseSqlResourceComponent component = this.requestContext.CreateComponent<ICodeSenseSqlResourceComponent, CodeSenseSqlResourceComponent>())
            {
              aggregateDescriptor = component.GetAggregate(path, result);
              if (aggregateDescriptor != null)
                aggregateDescriptor.Path = path;
            }
          }
        }
        return aggregateDescriptor;
      }

      private T GetDataOfAnonymousType<T>(int fileId, T exampleObject) where T : class => this.fileDataService.GetCachedData<T>(this.requestContext, fileId);

      private void FlushIfNeeded()
      {
        this.lastFlushStopWatch.Start();
        if (!(this.lastFlushStopWatch.Elapsed > AggregationStore.AggregateReaderWriter.FlushFrequency))
          return;
        this.Flush();
      }

      private string FilterDetailsOnPermissions(string json, out int filteredCount) => this.FilterOnPermissions(json, (Func<JToken, int>) (node => node.Value<int>((object) "id")), out filteredCount);

      private string FilterSummaryOnPermissions(string json, out int filteredCount) => this.FilterOnPermissions(json, (Func<JToken, int>) (node => node.Value<int>((object) "workItemId")), out filteredCount);

      private string FilterOnPermissions(
        string json,
        Func<JToken, int> workItemIdGetter,
        out int filteredCount)
      {
        filteredCount = 0;
        try
        {
          JObject json1 = JObject.Parse(json);
          JToken[] array = this.GetWorkItemNodes(json1).ToArray<JToken>();
          IEnumerable<int> workItemIds = ((IEnumerable<JToken>) array).Select<JToken, int>(workItemIdGetter).Distinct<int>();
          IDictionary<int, bool> workItemPermissions = this.requestContext.GetService<TeamFoundationSecurityServiceProxy>().GetWorkItemPermissions(this.requestContext, workItemIds);
          VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.CodeSense.Server.Shared.PerformanceCounters.PermissionedFilesReadBase").Increment();
          VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.CodeSense.Server.Shared.PerformanceCounters.PermissionsCheckedPerFileRead").IncrementBy((long) workItemPermissions.Count);
          using (new CodeSenseTraceWatch(this.requestContext, 1025042, TraceLayer.ExternalFramework, "Pruning the restricted paths from the result", Array.Empty<object>()))
          {
            for (int index = 0; index < array.Length; ++index)
            {
              int key = workItemIdGetter(array[index]);
              if (!workItemPermissions[key])
              {
                array[index].Remove();
                ++filteredCount;
              }
            }
            return json1.ToString(Formatting.None);
          }
        }
        catch (JsonReaderException ex)
        {
          return json;
        }
      }

      private IEnumerable<JToken> GetWorkItemNodes(JObject json) => json.Descendants().OfType<JProperty>().Where<JProperty>((Func<JProperty, bool>) (p => p.Name == "workItems")).Select<JProperty, JToken>((Func<JProperty, JToken>) (p => p.Value)).SelectMany<JToken, JToken>((Func<JToken, IEnumerable<JToken>>) (arr => (IEnumerable<JToken>) arr));

      private string InsertIndexingStatus(string summary)
      {
        IVssRegistryService service = this.requestContext.GetService<IVssRegistryService>();
        bool indexComplete;
        DateTime indexCompleteTo;
        service.GetIndexingStatus(this.requestContext, out indexComplete, out indexCompleteTo);
        int retentionPeriod = service.GetRetentionPeriod(this.requestContext);
        string str1 = JsonConvert.SerializeObject((object) indexComplete, CodeSenseSerializationSettings.JsonSerializerSettings);
        string str2;
        if (indexComplete)
        {
          str2 = string.Format("\"indexComplete\":{0},\"retentionPeriod\":{1},", (object) str1, (object) retentionPeriod);
        }
        else
        {
          string str3 = JsonConvert.SerializeObject((object) indexCompleteTo, CodeSenseSerializationSettings.JsonSerializerSettings);
          str2 = string.Format("\"indexComplete\":{0},\"indexCompleteTo\":{1},\"retentionPeriod\":{2},", (object) str1, (object) str3, (object) retentionPeriod);
        }
        return summary.Insert(summary.IndexOf('{') + 1, str2);
      }

      private string InsertBranchMap(string summary, IEnumerable<BranchLinkData> branchLinks)
      {
        string str = string.Format("\"branchMap\":{0},", (object) JsonConvert.SerializeObject((object) branchLinks, CodeSenseSerializationSettings.JsonSerializerSettings));
        return summary.Insert(summary.IndexOf('{') + 1, str);
      }

      private AggregateDescriptor GetLatestAggregateDescriptor(string path) => !this.aggregatesToAdd.ContainsKey(path) ? this.GetAggregateDescriptor(path) : this.aggregatesToAdd[path];

      private static IEnumerable<BranchLinkData> GetCodeflowChanges(
        IEnumerable<BranchLinkData> branchLinks,
        string path,
        int startId = -2147483648,
        int endId = 2147483647)
      {
        return (IEnumerable<BranchLinkData>) branchLinks.Where<BranchLinkData>((Func<BranchLinkData, bool>) (change =>
        {
          if (!TFStringComparer.VersionControlPath.Equals(change.TargetPath, path) || (change.ChangeType & (VersionControlChangeType.Branch | VersionControlChangeType.Merge)) == VersionControlChangeType.None)
            return false;
          return int.Parse(change.SourceChangesIdFrom) >= startId && int.Parse(change.SourceChangesIdTo) <= endId || int.Parse(change.TargetChangesId) == startId;
        })).OrderBy<BranchLinkData, string>((Func<BranchLinkData, string>) (branchLink => branchLink.TargetChangesId));
      }

      private static IEnumerable<CodeElementSummaryResult> GetCodeElements(
        IEnumerable<BranchSummaryResult> branchSummary,
        string path)
      {
        return branchSummary.Where<BranchSummaryResult>((Func<BranchSummaryResult, bool>) (summary => TFStringComparer.VersionControlPath.Equals(path, summary.ServerPath))).Select<BranchSummaryResult, List<CodeElementSummaryResult>>((Func<BranchSummaryResult, List<CodeElementSummaryResult>>) (summary => summary.CodeElements)).SelectMany<List<CodeElementSummaryResult>, CodeElementSummaryResult>((Func<List<CodeElementSummaryResult>, IEnumerable<CodeElementSummaryResult>>) (element => (IEnumerable<CodeElementSummaryResult>) element));
      }

      private static IEnumerable<FileChangeAggregateResult> GetFileChanges(
        IEnumerable<BranchDetailsResult> branchDetails,
        string path)
      {
        return branchDetails.Where<BranchDetailsResult>((Func<BranchDetailsResult, bool>) (details => TFStringComparer.VersionControlPath.Equals(path, details.ServerPath))).Select<BranchDetailsResult, List<FileChangeAggregateResult>>((Func<BranchDetailsResult, List<FileChangeAggregateResult>>) (details => details.Details)).SelectMany<List<FileChangeAggregateResult>, FileChangeAggregateResult>((Func<List<FileChangeAggregateResult>, IEnumerable<FileChangeAggregateResult>>) (change => (IEnumerable<FileChangeAggregateResult>) change));
      }

      private static void GetCodeElementsToAddForSummary(
        IEnumerable<BranchSummaryResult> branchList,
        IEnumerable<BranchLinkData> branchLinks,
        BranchLinkData codeflowChange,
        Dictionary<CodeElementIdentity, CodeElementKind> codeElementsToAdd)
      {
        int startId = int.Parse(codeflowChange.SourceChangesIdFrom);
        int endId = int.Parse(codeflowChange.SourceChangesIdTo);
        foreach (BranchLinkData codeflowChange1 in AggregationStore.AggregateReaderWriter.GetCodeflowChanges(branchLinks, codeflowChange.SourcePath, startId, endId))
          AggregationStore.AggregateReaderWriter.GetCodeElementsToAddForSummary(branchList, branchLinks, codeflowChange1, codeElementsToAdd);
        AggregationStore.AggregateReaderWriter.GetCodeElementsToAddForSummary(AggregationStore.AggregateReaderWriter.GetCodeElements(branchList, codeflowChange.SourcePath), startId, endId, codeElementsToAdd);
      }

      private static void GetCodeElementsToAddForSummary(
        IEnumerable<CodeElementSummaryResult> codeElements,
        int startId,
        int endId,
        Dictionary<CodeElementIdentity, CodeElementKind> codeElementsToAdd)
      {
        foreach (CodeElementSummaryResult codeElement in codeElements)
        {
          foreach (CollectorResult collectorResult in codeElement.ElementSummaries.Where<CollectorResult>((Func<CollectorResult, bool>) (d => d.Id.Equals("Microsoft.Changes"))))
          {
            foreach (AuthorSummary authorSummary in (List<AuthorSummary>) collectorResult.GetData<AuthorSummaryCollection>())
            {
              foreach (AuthorSummary.ChangeSummary changeSummary in authorSummary.ChangeSummaries)
              {
                int num = int.Parse(changeSummary.Id);
                if (num >= startId && num <= endId && !codeElementsToAdd.ContainsKey(codeElement.Id))
                  codeElementsToAdd.Add(codeElement.Id, codeElement.Kind);
              }
            }
          }
        }
      }

      private IEnumerable<CodeElementSummaryResult> AddCodeflowChangeSummary(
        IEnumerable<CodeElementSummaryResult> summaryResult,
        BranchLinkData codeflowChange,
        Dictionary<CodeElementIdentity, CodeElementKind> codeElementsToAdd)
      {
        if (codeElementsToAdd.Count > 0)
          AggregationStore.AggregateReaderWriter.AddCodeflowChangeSummaryForExistingCodeElements(summaryResult, codeflowChange, codeElementsToAdd);
        if (codeElementsToAdd.Count > 0)
        {
          IEnumerable<CodeElementSummaryResult> second = AggregationStore.AggregateReaderWriter.AddCodeflowChangeSummaryForNewCodeElements(codeflowChange, codeElementsToAdd);
          summaryResult = summaryResult.Concat<CodeElementSummaryResult>(second);
        }
        return summaryResult;
      }

      private static IEnumerable<CodeElementSummaryResult> AddCodeflowChangeSummaryForNewCodeElements(
        BranchLinkData codeflowChange,
        Dictionary<CodeElementIdentity, CodeElementKind> codeElementsToAdd)
      {
        List<CodeElementSummaryResult> elementSummaryResultList = new List<CodeElementSummaryResult>();
        foreach (KeyValuePair<CodeElementIdentity, CodeElementKind> keyValuePair in codeElementsToAdd)
        {
          AuthorSummary.ChangeSummary changeSummary = new AuthorSummary.ChangeSummary(codeflowChange.TargetChangesId, (IEnumerable<WorkItemData>) null);
          AuthorSummaryCollection data = new AuthorSummaryCollection();
          data.Add(new AuthorSummary(codeflowChange.AuthorUniqueName, codeflowChange.AuthorDisplayName, (IEnumerable<AuthorSummary.ChangeSummary>) new AuthorSummary.ChangeSummary[1]
          {
            changeSummary
          }));
          CollectorResult collectorResult = new CollectorResult("Microsoft.Changes", (object) data, "1.0");
          CodeElementSummaryResult elementSummaryResult = new CodeElementSummaryResult(keyValuePair.Key, keyValuePair.Value, (IEnumerable<CollectorResult>) new CollectorResult[1]
          {
            collectorResult
          });
          elementSummaryResultList.Add(elementSummaryResult);
        }
        return (IEnumerable<CodeElementSummaryResult>) elementSummaryResultList;
      }

      private static void AddCodeflowChangeSummaryForExistingCodeElements(
        IEnumerable<CodeElementSummaryResult> summaryResult,
        BranchLinkData codeflowChange,
        Dictionary<CodeElementIdentity, CodeElementKind> codeElementsToAdd)
      {
        foreach (CodeElementSummaryResult elementSummaryResult in summaryResult)
        {
          if (codeElementsToAdd.ContainsKey(elementSummaryResult.Id))
          {
            foreach (CollectorResult collectorResult in elementSummaryResult.ElementSummaries.Where<CollectorResult>((Func<CollectorResult, bool>) (d => d.Id.Equals("Microsoft.Changes"))))
            {
              bool flag = false;
              AuthorSummaryCollection data = collectorResult.GetData<AuthorSummaryCollection>();
              foreach (AuthorSummary authorSummary in (List<AuthorSummary>) data)
              {
                if (authorSummary.AuthorUniqueName == codeflowChange.AuthorUniqueName)
                {
                  AuthorSummary.ChangeSummary changeSummary = new AuthorSummary.ChangeSummary(codeflowChange.TargetChangesId, (IEnumerable<WorkItemData>) null);
                  authorSummary.AppendChangeSummaries((IEnumerable<AuthorSummary.ChangeSummary>) new AuthorSummary.ChangeSummary[1]
                  {
                    changeSummary
                  });
                  flag = true;
                  break;
                }
              }
              if (!flag)
              {
                AuthorSummary.ChangeSummary changeSummary = new AuthorSummary.ChangeSummary(codeflowChange.TargetChangesId, (IEnumerable<WorkItemData>) null);
                AuthorSummary authorSummary = new AuthorSummary(codeflowChange.AuthorUniqueName, codeflowChange.AuthorDisplayName, (IEnumerable<AuthorSummary.ChangeSummary>) new AuthorSummary.ChangeSummary[1]
                {
                  changeSummary
                });
                data.Add(authorSummary);
              }
              collectorResult.Data = (object) data;
            }
            codeElementsToAdd.Remove(elementSummaryResult.Id);
          }
        }
      }

      private static List<CodeElementDetailsResult> GetNewCodeElements(
        BranchLinkData codeflowChange,
        Dictionary<CodeElementIdentity, Tuple<ElementChangeKind, CodeElementKind>> codeElementsToAdd)
      {
        List<CodeElementDetailsResult> newCodeElements = new List<CodeElementDetailsResult>();
        foreach (KeyValuePair<CodeElementIdentity, Tuple<ElementChangeKind, CodeElementKind>> keyValuePair in codeElementsToAdd)
        {
          CollectorResult collectorResult = new CollectorResult("Microsoft.Changes", (object) new CodeElementChangeResult(keyValuePair.Value.Item1, codeflowChange.TargetChangesId, codeflowChange.ChangesComment, codeflowChange.Date, (IEnumerable<WorkItemData>) null, new UserData(codeflowChange.AuthorUniqueName, codeflowChange.AuthorDisplayName, codeflowChange.AuthorEmail)), "1.0");
          CodeElementDetailsResult elementDetailsResult = new CodeElementDetailsResult(keyValuePair.Key, keyValuePair.Value.Item2, (IEnumerable<CollectorResult>) new CollectorResult[1]
          {
            collectorResult
          });
          newCodeElements.Add(elementDetailsResult);
        }
        return newCodeElements;
      }

      private static void GetCodeElementsToAddForDetails(
        IEnumerable<BranchDetailsResult> branchList,
        IEnumerable<BranchLinkData> branchLinks,
        BranchLinkData codeflowChange,
        Dictionary<CodeElementIdentity, Tuple<ElementChangeKind, CodeElementKind>> codeElementsToAdd)
      {
        int startId = int.Parse(codeflowChange.SourceChangesIdFrom);
        int endId = int.Parse(codeflowChange.SourceChangesIdTo);
        foreach (BranchLinkData codeflowChange1 in AggregationStore.AggregateReaderWriter.GetCodeflowChanges(branchLinks, codeflowChange.SourcePath, startId, endId))
          AggregationStore.AggregateReaderWriter.GetCodeElementsToAddForDetails(branchList, branchLinks, codeflowChange1, codeElementsToAdd);
        AggregationStore.AggregateReaderWriter.GetCodeElementsToAddForDetails(AggregationStore.AggregateReaderWriter.GetFileChanges(branchList, codeflowChange.SourcePath), startId, endId, codeElementsToAdd);
      }

      private static void GetCodeElementsToAddForDetails(
        IEnumerable<FileChangeAggregateResult> fileChanges,
        int startId,
        int endId,
        Dictionary<CodeElementIdentity, Tuple<ElementChangeKind, CodeElementKind>> codeElementsToAdd)
      {
        foreach (FileChangeAggregateResult fileChange in fileChanges)
        {
          foreach (CodeElementDetailsResult codeElement in fileChange.CodeElements)
          {
            foreach (CollectorResult collectorResult in codeElement.ElementDetails.Where<CollectorResult>((Func<CollectorResult, bool>) (d => d.Id.Equals("Microsoft.Changes"))))
            {
              CodeElementChangeResult data = collectorResult.GetData<CodeElementChangeResult>();
              int num = int.Parse(data.ChangesId);
              if (num >= startId && num <= endId)
              {
                ElementChangeKind changeKind = data.ChangeKind;
                if (!codeElementsToAdd.ContainsKey(codeElement.Id))
                  codeElementsToAdd.Add(codeElement.Id, new Tuple<ElementChangeKind, CodeElementKind>(changeKind, codeElement.Kind));
                else if (changeKind == ElementChangeKind.Remove || codeElementsToAdd[codeElement.Id].Item1 == ElementChangeKind.Remove)
                  codeElementsToAdd[codeElement.Id] = new Tuple<ElementChangeKind, CodeElementKind>(changeKind, codeElement.Kind);
              }
            }
          }
        }
      }
    }
  }
}
