// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime.PipelineAttemptBuilder
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Validation;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class PipelineAttemptBuilder
  {
    private int m_stageOrder = 1;
    private IDictionary<Guid, TimelineRecord> m_recordsById;
    private IDictionary<Guid, IList<TimelineRecord>> m_recordsByParent;
    private IDictionary<string, IList<StageAttempt>> m_stages;

    public PipelineAttemptBuilder(
      IPipelineIdGenerator idGenerator,
      PipelineProcess pipeline,
      params Timeline[] timelines)
    {
      ArgumentUtility.CheckForNull<IPipelineIdGenerator>(idGenerator, nameof (idGenerator));
      ArgumentUtility.CheckForNull<PipelineProcess>(pipeline, nameof (pipeline));
      this.Pipeline = pipeline;
      this.IdGenerator = idGenerator;
      this.m_recordsById = (IDictionary<Guid, TimelineRecord>) new Dictionary<Guid, TimelineRecord>();
      this.m_recordsByParent = (IDictionary<Guid, IList<TimelineRecord>>) new Dictionary<Guid, IList<TimelineRecord>>();
      this.m_stages = (IDictionary<string, IList<StageAttempt>>) new Dictionary<string, IList<StageAttempt>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (timelines == null || timelines.Length == 0)
        return;
      foreach (Timeline timeline in timelines)
        this.AddStageAttempts(timeline, this.m_stages);
    }

    public IPipelineIdGenerator IdGenerator { get; }

    public PipelineProcess Pipeline { get; }

    public IList<StageAttempt> Initialize()
    {
      List<StageAttempt> stageAttemptList = new List<StageAttempt>();
      foreach (Stage stage in (IEnumerable<Stage>) this.Pipeline.Stages)
        stageAttemptList.Add(this.CreateAttempt(stage));
      return (IList<StageAttempt>) stageAttemptList;
    }

    public Tuple<IList<StageAttempt>, IList<StageAttempt>> Retry(
      IList<string> stageNames = null,
      bool forceRetryAllJobs = false,
      bool retryDependencies = true)
    {
      List<StageAttempt> allAttempts = new List<StageAttempt>();
      List<StageAttempt> newAttempts = new List<StageAttempt>();
      HashSet<string> stagesToRetry = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      GraphValidator.Traverse<Stage>(this.Pipeline.Stages, (Action<Stage, ISet<string>>) ((stage, dependencies) =>
      {
        StageAttempt stageAttempt1 = this.GetStageAttempt(stage.Name);
        if (stageAttempt1 == null)
          return;
        stageAttempt1.Stage.Definition = stage;
        IList<string> source = stageNames;
        bool? nullable = source != null ? new bool?(source.Contains<string>(stage.Name, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)) : new bool?();
        bool flag1 = nullable.HasValue && nullable.GetValueOrDefault();
        bool flag2 = PipelineAttemptBuilder.NeedsRetry(stageAttempt1.Stage.Result);
        bool flag3 = retryDependencies && dependencies.Any<string>((Func<string, bool>) (x => stagesToRetry.Contains(x)));
        StageAttempt stageAttempt2 = (StageAttempt) null;
        if (flag3 || stageNames == null & flag2 || stageNames != null & flag1)
          stageAttempt2 = this.CreateAttempt(stage, stageAttempt1, ((!flag1 ? 0 : (!flag2 | forceRetryAllJobs ? 1 : 0)) | (flag3 ? 1 : 0)) != 0, !(flag1 & forceRetryAllJobs));
        if (stageAttempt2 == null)
        {
          allAttempts.Add(stageAttempt1);
        }
        else
        {
          stagesToRetry.Add(stageAttempt1.Stage.Name);
          allAttempts.Add(stageAttempt2);
          newAttempts.Add(stageAttempt2);
        }
      }));
      return Tuple.Create<IList<StageAttempt>, IList<StageAttempt>>((IList<StageAttempt>) allAttempts, (IList<StageAttempt>) newAttempts);
    }

    private StageAttempt CreateAttempt(
      Stage stage,
      StageAttempt previousStageAttempt = null,
      bool forceRetry = false,
      bool copySuccessfulJobs = true)
    {
      StageAttempt stageAttempt1 = new StageAttempt();
      Stage stage1 = stage;
      StageAttempt stageAttempt2 = previousStageAttempt;
      int attempt = stageAttempt2 != null ? stageAttempt2.Stage.Attempt + 1 : 1;
      stageAttempt1.Stage = new StageInstance(stage1, attempt);
      stageAttempt1.Timeline = new Timeline();
      StageAttempt newStageAttempt = stageAttempt1;
      string stageIdentifier = this.IdGenerator.GetStageIdentifier(newStageAttempt.Stage.Name);
      Guid stageId = this.IdGenerator.GetStageInstanceId(newStageAttempt.Stage.Name, newStageAttempt.Stage.Attempt);
      newStageAttempt.Timeline.Id = stageId;
      newStageAttempt.Stage.Identifier = stageIdentifier;
      if (previousStageAttempt != null)
      {
        TimelineRecord record = this.m_recordsById[this.IdGenerator.GetStageInstanceId(previousStageAttempt.Stage.Name, previousStageAttempt.Stage.Attempt)];
        newStageAttempt.Timeline.Records.Add(this.ResetRecord(record, new Guid?(), stageId, newStageAttempt.Stage.Attempt));
      }
      else
        newStageAttempt.Timeline.Records.Add(this.CreateRecord((IGraphNodeInstance) newStageAttempt.Stage, new Guid?(), stageId, stage.DisplayName ?? stage.Name, "Stage", this.m_stageOrder++, stageIdentifier));
      int phaseOrder = 1;
      bool phasesRetried = false;
      HashSet<string> phasesToRetry = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      GraphValidator.Traverse<PhaseNode>(stage.Phases, (Action<PhaseNode, ISet<string>>) ((phase, dependencies) =>
      {
        bool flag1 = false;
        bool flag2 = phase is ProviderPhase;
        StageAttempt stageAttempt3 = previousStageAttempt;
        PhaseAttempt phaseAttempt1 = stageAttempt3 != null ? stageAttempt3.Phases.FirstOrDefault<PhaseAttempt>((Func<PhaseAttempt, bool>) (x => string.Equals(x.Phase.Name, phase.Name, StringComparison.OrdinalIgnoreCase))) : (PhaseAttempt) null;
        bool flag3 = dependencies.Any<string>((Func<string, bool>) (x => phasesToRetry.Contains(x)));
        bool flag4 = PipelineAttemptBuilder.NeedsRetry((TaskResult?) phaseAttempt1?.Phase.Result);
        if (forceRetry | flag3 | flag4)
        {
          flag1 = true;
          phasesToRetry.Add(phase.Name);
        }
        if (!flag1 && phaseAttempt1 != null)
        {
          phaseAttempt1.Phase.Definition = phase;
          newStageAttempt.Phases.Add(phaseAttempt1);
          TimelineRecord root = this.m_recordsById[this.IdGenerator.GetPhaseInstanceId(newStageAttempt.Stage.Name, phaseAttempt1.Phase.Name, phaseAttempt1.Phase.Attempt)].Clone();
          root.ParentId = new Guid?(stageId);
          phaseOrder = (root.Order ?? phaseOrder) + 1;
          newStageAttempt.Timeline.Records.Add(root);
          newStageAttempt.Timeline.Records.AddRange((IEnumerable<TimelineRecord>) this.CollectAllChildren(root));
        }
        else
        {
          phasesRetried = true;
          PhaseAttempt phaseAttempt2 = new PhaseAttempt()
          {
            Phase = new PhaseInstance(phase, phaseAttempt1 != null ? phaseAttempt1.Phase.Attempt + 1 : 1)
          };
          Guid phaseInstanceId = this.IdGenerator.GetPhaseInstanceId(newStageAttempt.Stage.Name, phaseAttempt2.Phase.Name, phaseAttempt2.Phase.Attempt);
          phaseAttempt2.Phase.Identifier = this.IdGenerator.GetPhaseIdentifier(newStageAttempt.Stage.Name, phaseAttempt2.Phase.Name);
          newStageAttempt.Timeline.Records.Add(this.CreateRecord((IGraphNodeInstance) phaseAttempt2.Phase, new Guid?(stageId), phaseInstanceId, phase.DisplayName ?? phase.Name, "Phase", phaseOrder++, phaseAttempt2.Phase.Identifier));
          if (((!flag4 || flag3 ? 0 : (!flag2 ? 1 : 0)) & (copySuccessfulJobs ? 1 : 0)) != 0)
          {
            foreach (JobAttempt job in (IEnumerable<JobAttempt>) phaseAttempt1.Jobs)
            {
              Guid jobInstanceId1 = this.IdGenerator.GetJobInstanceId(newStageAttempt.Stage.Name, phaseAttempt2.Phase.Name, job.Job.Name, job.Job.Attempt);
              if (PipelineAttemptBuilder.NeedsRetry(job.Job.Result))
              {
                JobAttempt jobAttempt = new JobAttempt()
                {
                  Job = new JobInstance(job.Job.Name, job.Job.Attempt + 1)
                };
                jobAttempt.Job.Identifier = this.IdGenerator.GetJobIdentifier(newStageAttempt.Stage.Name, phaseAttempt2.Phase.Name, jobAttempt.Job.Name);
                phaseAttempt2.Jobs.Add(jobAttempt);
                Guid jobInstanceId2 = this.IdGenerator.GetJobInstanceId(newStageAttempt.Stage.Name, phaseAttempt2.Phase.Name, jobAttempt.Job.Name, jobAttempt.Job.Attempt);
                newStageAttempt.Timeline.Records.Add(this.ResetRecord(this.m_recordsById[jobInstanceId1], new Guid?(phaseInstanceId), jobInstanceId2, jobAttempt.Job.Attempt));
              }
              else
              {
                TimelineRecord root = this.m_recordsById[jobInstanceId1].Clone();
                root.ParentId = new Guid?(phaseInstanceId);
                phaseAttempt2.Jobs.Add(job);
                newStageAttempt.Timeline.Records.Add(root);
                newStageAttempt.Timeline.Records.AddRange((IEnumerable<TimelineRecord>) this.CollectAllChildren(root));
              }
            }
          }
          newStageAttempt.Phases.Add(phaseAttempt2);
        }
      }));
      if (!phasesRetried)
        return (StageAttempt) null;
      IList<StageAttempt> stageAttemptList;
      if (!this.m_stages.TryGetValue(stage.Name, out stageAttemptList))
      {
        stageAttemptList = (IList<StageAttempt>) new List<StageAttempt>();
        this.m_stages[stage.Name] = stageAttemptList;
      }
      stageAttemptList.Add(newStageAttempt);
      return newStageAttempt;
    }

    public StageAttempt GetStageAttempt(string name, int attempt = -1)
    {
      IList<StageAttempt> source;
      if (!this.m_stages.TryGetValue(name, out source))
        return (StageAttempt) null;
      return attempt <= 0 ? source.OrderByDescending<StageAttempt, int>((Func<StageAttempt, int>) (x => x.Stage.Attempt)).FirstOrDefault<StageAttempt>() : source.FirstOrDefault<StageAttempt>((Func<StageAttempt, bool>) (x => x.Stage.Attempt == attempt));
    }

    public IList<StageAttempt> GetStageAttempts(string name) => this.m_stages.GetValueOrDefault<string, IList<StageAttempt>>(name, (IList<StageAttempt>) new List<StageAttempt>());

    internal static bool NeedsRetry(TaskResult? result)
    {
      TaskResult? nullable1 = result;
      TaskResult taskResult1 = TaskResult.Abandoned;
      if (!(nullable1.GetValueOrDefault() == taskResult1 & nullable1.HasValue))
      {
        TaskResult? nullable2 = result;
        TaskResult taskResult2 = TaskResult.Canceled;
        if (!(nullable2.GetValueOrDefault() == taskResult2 & nullable2.HasValue))
        {
          TaskResult? nullable3 = result;
          TaskResult taskResult3 = TaskResult.Failed;
          return nullable3.GetValueOrDefault() == taskResult3 & nullable3.HasValue;
        }
      }
      return true;
    }

    private TimelineRecord CreateRecord(
      IGraphNodeInstance node,
      Guid? parentId,
      Guid recordId,
      string name,
      string type,
      int order,
      string identifier)
    {
      return new TimelineRecord()
      {
        Attempt = node.Attempt,
        Id = recordId,
        Identifier = identifier,
        Name = name,
        Order = new int?(order),
        ParentId = parentId,
        RecordType = type,
        RefName = node.Name,
        State = new TimelineRecordState?(TimelineRecordState.Pending)
      };
    }

    private TimelineRecord ResetRecord(
      TimelineRecord record,
      Guid? parentId,
      Guid newId,
      int attempt)
    {
      return new TimelineRecord()
      {
        Attempt = attempt,
        Id = newId,
        ParentId = parentId,
        State = new TimelineRecordState?(TimelineRecordState.Pending),
        Identifier = record.Identifier,
        Name = record.Name,
        Order = record.Order,
        RecordType = record.RecordType,
        RefName = record.RefName
      };
    }

    private void AddStageAttempts(
      Timeline timeline,
      IDictionary<string, IList<StageAttempt>> attempts)
    {
      if (timeline == null)
        return;
      TimelineUtilities.TimelineMapping timeline1 = TimelineUtilities.ParseTimeline(timeline);
      this.m_recordsById = timeline1.RecordsById;
      this.m_recordsByParent = timeline1.RecordsByParentId;
      foreach (TimelineRecord timelineRecord in this.m_recordsById.Values.Where<TimelineRecord>((Func<TimelineRecord, bool>) (x => x.RecordType == "Stage")))
      {
        StageAttempt stageAttempt1 = new StageAttempt();
        StageInstance stageInstance = new StageInstance();
        stageInstance.Attempt = timelineRecord.Attempt;
        stageInstance.FinishTime = timelineRecord.FinishTime;
        stageInstance.Identifier = timelineRecord.Identifier;
        stageInstance.Name = timelineRecord.RefName;
        stageInstance.Result = timelineRecord.Result;
        stageInstance.StartTime = timelineRecord.StartTime;
        stageInstance.State = PipelineAttemptBuilder.Convert(timelineRecord.State.Value);
        stageAttempt1.Stage = stageInstance;
        Timeline timeline2 = new Timeline();
        timeline2.Id = timeline.Id;
        stageAttempt1.Timeline = timeline2;
        StageAttempt stageAttempt2 = stageAttempt1;
        stageAttempt2.Timeline.Records.Add(timelineRecord);
        IList<TimelineRecord> source;
        if (this.m_recordsByParent.TryGetValue(timelineRecord.Id, out source))
          this.AddPhaseAttempts(stageAttempt2, source.Where<TimelineRecord>((Func<TimelineRecord, bool>) (x => x.RecordType == "Phase")), this.m_recordsByParent);
        IList<StageAttempt> stageAttemptList;
        if (!attempts.TryGetValue(stageAttempt2.Stage.Identifier, out stageAttemptList))
        {
          stageAttemptList = (IList<StageAttempt>) new List<StageAttempt>();
          attempts.Add(stageAttempt2.Stage.Identifier, stageAttemptList);
        }
        stageAttemptList.Add(stageAttempt2);
      }
    }

    private void AddPhaseAttempts(
      StageAttempt stageAttempt,
      IEnumerable<TimelineRecord> phaseRecords,
      IDictionary<Guid, IList<TimelineRecord>> recordsByParent)
    {
      foreach (TimelineRecord phaseRecord in phaseRecords)
      {
        TimelineUtilities.FixRecord(phaseRecord);
        PhaseAttempt phaseAttempt1 = new PhaseAttempt();
        PhaseInstance phaseInstance = new PhaseInstance();
        phaseInstance.Attempt = phaseRecord.Attempt;
        phaseInstance.FinishTime = phaseRecord.FinishTime;
        phaseInstance.Identifier = phaseRecord.Identifier;
        phaseInstance.Name = phaseRecord.RefName;
        phaseInstance.Result = phaseRecord.Result;
        phaseInstance.StartTime = phaseRecord.StartTime;
        phaseInstance.State = PipelineAttemptBuilder.Convert(phaseRecord.State.Value);
        phaseAttempt1.Phase = phaseInstance;
        PhaseAttempt phaseAttempt2 = phaseAttempt1;
        stageAttempt.Phases.Add(phaseAttempt2);
        stageAttempt.Timeline.Records.Add(phaseRecord);
        IList<TimelineRecord> source;
        if (recordsByParent.TryGetValue(phaseRecord.Id, out source))
          this.AddJobAttempts(stageAttempt, phaseAttempt2, source.Where<TimelineRecord>((Func<TimelineRecord, bool>) (x => x.RecordType == "Job")), recordsByParent);
      }
    }

    private void AddJobAttempts(
      StageAttempt stageAttempt,
      PhaseAttempt phaseAttempt,
      IEnumerable<TimelineRecord> jobRecords,
      IDictionary<Guid, IList<TimelineRecord>> recordsByParent)
    {
      foreach (TimelineRecord jobRecord in jobRecords)
      {
        TimelineUtilities.FixRecord(jobRecord);
        JobAttempt jobAttempt = new JobAttempt()
        {
          Job = new JobInstance()
          {
            Attempt = jobRecord.Attempt,
            FinishTime = jobRecord.FinishTime,
            Identifier = jobRecord.Identifier,
            Name = jobRecord.RefName,
            Result = jobRecord.Result,
            StartTime = jobRecord.StartTime,
            State = PipelineAttemptBuilder.Convert(jobRecord.State.Value)
          }
        };
        phaseAttempt.Jobs.Add(jobAttempt);
        stageAttempt.Timeline.Records.Add(jobRecord);
        stageAttempt.Timeline.Records.AddRange((IEnumerable<TimelineRecord>) this.CollectAllChildren(jobRecord));
      }
    }

    internal IList<TimelineRecord> CollectAllChildren(TimelineRecord root, int maxDepth = 2147483647) => TimelineUtilities.CollectAllChildren(root.Id, this.m_recordsByParent, maxDepth);

    private static PipelineState Convert(TimelineRecordState state)
    {
      if (state == TimelineRecordState.InProgress)
        return PipelineState.InProgress;
      return state == TimelineRecordState.Completed ? PipelineState.Completed : PipelineState.NotStarted;
    }
  }
}
