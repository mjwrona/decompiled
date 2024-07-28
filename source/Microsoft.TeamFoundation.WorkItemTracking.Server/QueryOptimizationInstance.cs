// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.QueryOptimizationInstance
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class QueryOptimizationInstance : BufferableItem
  {
    public readonly QueryOptimizationStrategy OptimizationStrategy;
    public readonly Guid? QueryId;
    public readonly string QueryHash;

    public short StrategyIndex { get; private set; }

    public short SlowRunCountInCurrentOpt { get; private set; }

    public short NormalRunCountInCurrentOpt { get; private set; }

    public short DeltaSlowRunCount { get; private set; }

    public short DeltaNormalRunCount { get; private set; }

    public bool IsSlowRunCountReset { get; private set; }

    public bool IsNormalRunCountReset { get; private set; }

    public QueryOptimizationState OptimizationState { get; private set; }

    public DateTime LastStateChangeTime { get; private set; }

    public int SlownessThresholdInMsFromHistory { get; private set; }

    public DateTime LastRunTime { get; private set; }

    public bool IsStateForkedFromOtherInstance { get; private set; }

    public QueryOptimizationInstance(
      Guid? id,
      string hash,
      QueryOptimizationStrategy strategy,
      QueryOptimizationState intialState = QueryOptimizationState.InEvaluation)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(hash, nameof (hash));
      this.QueryId = id;
      this.QueryHash = hash;
      this.OptimizationStrategy = strategy;
      this.OptimizationState = intialState;
      this.StrategyIndex = (short) -1;
      this.LastRunTime = DateTime.MinValue;
      this.LastStateChangeTime = DateTime.UtcNow;
    }

    public void SetOptimizationState(
      QueryOptimizationState optimizationState,
      short? currentIndex = null,
      short? normalRunCount = null,
      short? slowRunCount = null,
      DateTime? lastRunTime = null,
      DateTime? lastStateChangeTime = null)
    {
      lock (this.LockObject)
        this.SetOptimizationStateInternal(optimizationState, currentIndex, normalRunCount, slowRunCount, lastRunTime, lastStateChangeTime);
    }

    public void SetSlownessThresholdInMsFromHistory(int slownessThresholdInMsFromHistory)
    {
      lock (this.LockObject)
        this.SlownessThresholdInMsFromHistory = slownessThresholdInMsFromHistory;
    }

    public void ResetDeltaRunCounts()
    {
      lock (this.LockObject)
      {
        this.DeltaNormalRunCount = (short) 0;
        this.DeltaSlowRunCount = (short) 0;
      }
    }

    public QueryOptimization GetCurrentQueryOptimization()
    {
      QueryOptimizationStrategy optimizationStrategy = this.OptimizationStrategy;
      return optimizationStrategy == null ? QueryOptimization.None : optimizationStrategy.GetOptimization(this.StrategyIndex);
    }

    public short GetCurrentSlowRunCount() => this.IsSlowRunCountReset ? (short) 0 : (short) ((int) this.SlowRunCountInCurrentOpt + (int) this.DeltaSlowRunCount);

    public short GetCurrentNormalRunCount() => this.IsNormalRunCountReset ? (short) 0 : (short) ((int) this.NormalRunCountInCurrentOpt + (int) this.DeltaNormalRunCount);

    public void MoveNext(
      IVssRequestContext requestContext,
      int runTime,
      DateTime lastRunTime,
      out bool needResetHistory,
      int timeoutThresholdInMs)
    {
      lock (this.LockObject)
      {
        needResetHistory = false;
        if (this.OptimizationState == QueryOptimizationState.None)
        {
          if (runTime < (int) QueryOptimizationStrategy.SlowRunTimeMinThresholdInMs)
            return;
          this.OptimizationState = QueryOptimizationState.InEvaluation;
        }
        if (this.SlownessThresholdInMsFromHistory == 0)
          return;
        this.LastRunTime = lastRunTime;
        this.IsSlowRunCountReset = false;
        this.IsNormalRunCountReset = false;
        if (this.OptimizationState == QueryOptimizationState.InEvaluation)
        {
          if (this.SlownessThresholdInMsFromHistory > (int) QueryOptimizationStrategy.SlowRunTimeMinThresholdInMs)
          {
            short? currentIndex = new short?((short) 0);
            DateTime? nullable = new DateTime?(DateTime.UtcNow);
            short? normalRunCount = new short?();
            short? slowRunCount = new short?();
            DateTime? lastRunTime1 = new DateTime?();
            DateTime? lastStateChangeTime = nullable;
            int? slownessThresholdInMsFromHistory = new int?();
            bool? isNormalRunCountReset = new bool?();
            bool? isSlowRunCountReset = new bool?();
            short? deltaNormalRunCount = new short?();
            short? deltaSlowRunCount = new short?();
            this.SetOptimizationStateInternal(QueryOptimizationState.InOptimization, currentIndex, normalRunCount, slowRunCount, lastRunTime1, lastStateChangeTime, slownessThresholdInMsFromHistory, isNormalRunCountReset, isSlowRunCountReset, deltaNormalRunCount, deltaSlowRunCount);
            this.ResetNormalRunCounts();
            this.ResetSlowRunCounts();
          }
          else
          {
            float num = Math.Min((float) this.SlownessThresholdInMsFromHistory / (float) QueryOptimizationStrategy.DesiredOptimizationRatio, (float) (int) QueryOptimizationStrategy.SlowRunTimeMinThresholdInMs);
            int thresholdInMsFromHistory = this.SlownessThresholdInMsFromHistory;
            if ((double) runTime > (double) num)
              ++this.DeltaSlowRunCount;
            else if (runTime < thresholdInMsFromHistory)
            {
              ++this.DeltaNormalRunCount;
              this.ResetSlowRunCounts();
            }
            if ((int) this.GetCurrentSlowRunCount() >= (int) QueryOptimizationStrategy.TrialLimit)
            {
              short? currentIndex = new short?((short) 0);
              DateTime? nullable = new DateTime?(DateTime.UtcNow);
              short? normalRunCount = new short?();
              short? slowRunCount = new short?();
              DateTime? lastRunTime2 = new DateTime?();
              DateTime? lastStateChangeTime = nullable;
              int? slownessThresholdInMsFromHistory = new int?();
              bool? isNormalRunCountReset = new bool?();
              bool? isSlowRunCountReset = new bool?();
              short? deltaNormalRunCount = new short?();
              short? deltaSlowRunCount = new short?();
              this.SetOptimizationStateInternal(QueryOptimizationState.InOptimization, currentIndex, normalRunCount, slowRunCount, lastRunTime2, lastStateChangeTime, slownessThresholdInMsFromHistory, isNormalRunCountReset, isSlowRunCountReset, deltaNormalRunCount, deltaSlowRunCount);
              this.ResetNormalRunCounts();
              this.ResetSlowRunCounts();
            }
            if ((int) this.GetCurrentNormalRunCount() < (int) QueryOptimizationStrategy.TrialLimit)
              return;
            short? currentIndex1 = new short?((short) -1);
            DateTime? nullable1 = new DateTime?(DateTime.UtcNow);
            short? normalRunCount1 = new short?();
            short? slowRunCount1 = new short?();
            DateTime? lastRunTime3 = new DateTime?();
            DateTime? lastStateChangeTime1 = nullable1;
            int? slownessThresholdInMsFromHistory1 = new int?();
            bool? isNormalRunCountReset1 = new bool?();
            bool? isSlowRunCountReset1 = new bool?();
            short? deltaNormalRunCount1 = new short?();
            short? deltaSlowRunCount1 = new short?();
            this.SetOptimizationStateInternal(QueryOptimizationState.None, currentIndex1, normalRunCount1, slowRunCount1, lastRunTime3, lastStateChangeTime1, slownessThresholdInMsFromHistory1, isNormalRunCountReset1, isSlowRunCountReset1, deltaNormalRunCount1, deltaSlowRunCount1);
            this.ResetNormalRunCounts();
            this.ResetSlowRunCounts();
          }
        }
        else if (this.OptimizationState == QueryOptimizationState.InOptimization)
        {
          float num = Math.Min((float) timeoutThresholdInMs, Math.Min((float) this.SlownessThresholdInMsFromHistory / (float) QueryOptimizationStrategy.DesiredOptimizationRatio, (float) (this.SlownessThresholdInMsFromHistory + (int) QueryOptimizationStrategy.ToleratedSlownessInMs)));
          int thresholdInMsFromHistory = this.SlownessThresholdInMsFromHistory;
          if ((double) runTime > (double) num)
            ++this.DeltaSlowRunCount;
          else if (runTime < thresholdInMsFromHistory)
            ++this.DeltaNormalRunCount;
          if (this.GetCurrentSlowRunCount() >= (short) 1)
          {
            this.ResetNormalRunCounts();
            this.ResetSlowRunCounts();
            if (!this.MoveToNextOptimization(requestContext))
            {
              short? currentIndex = new short?((short) -1);
              DateTime? nullable = new DateTime?(DateTime.UtcNow);
              short? normalRunCount = new short?();
              short? slowRunCount = new short?();
              DateTime? lastRunTime4 = new DateTime?();
              DateTime? lastStateChangeTime = nullable;
              int? slownessThresholdInMsFromHistory = new int?();
              bool? isNormalRunCountReset = new bool?();
              bool? isSlowRunCountReset = new bool?();
              short? deltaNormalRunCount = new short?();
              short? deltaSlowRunCount = new short?();
              this.SetOptimizationStateInternal(QueryOptimizationState.NotOptimizable, currentIndex, normalRunCount, slowRunCount, lastRunTime4, lastStateChangeTime, slownessThresholdInMsFromHistory, isNormalRunCountReset, isSlowRunCountReset, deltaNormalRunCount, deltaSlowRunCount);
              needResetHistory = true;
            }
          }
          if ((int) this.GetCurrentNormalRunCount() < (int) QueryOptimizationStrategy.TrialLimit)
            return;
          this.SetOptimizationStateInternal(QueryOptimizationState.Optimized, lastStateChangeTime: new DateTime?(DateTime.UtcNow));
          this.ResetNormalRunCounts();
          this.ResetSlowRunCounts();
        }
        else if (this.OptimizationState == QueryOptimizationState.Optimized)
        {
          float num1 = Math.Max(Math.Min((float) timeoutThresholdInMs, Math.Min((float) this.SlownessThresholdInMsFromHistory / (float) QueryOptimizationStrategy.DesiredOptimizationRatio, (float) (this.SlownessThresholdInMsFromHistory + (int) QueryOptimizationStrategy.ToleratedSlownessInMs))), (float) (int) QueryOptimizationStrategy.SlowRunTimeMinThresholdInMs);
          float num2 = (float) this.SlownessThresholdInMsFromHistory / (float) QueryOptimizationStrategy.OptimizedNormalCriteriaSlackRatio;
          if ((double) runTime > (double) num1)
            ++this.DeltaSlowRunCount;
          else if ((double) runTime < (double) num2)
            this.ResetSlowRunCounts();
          if ((int) this.GetCurrentSlowRunCount() < (int) QueryOptimizationStrategy.TrialLimit)
            return;
          short? currentIndex = new short?((short) -1);
          DateTime? nullable = new DateTime?(DateTime.UtcNow);
          short? normalRunCount = new short?();
          short? slowRunCount = new short?();
          DateTime? lastRunTime5 = new DateTime?();
          DateTime? lastStateChangeTime = nullable;
          int? slownessThresholdInMsFromHistory = new int?();
          bool? isNormalRunCountReset = new bool?();
          bool? isSlowRunCountReset = new bool?();
          short? deltaNormalRunCount = new short?();
          short? deltaSlowRunCount = new short?();
          this.SetOptimizationStateInternal(QueryOptimizationState.None, currentIndex, normalRunCount, slowRunCount, lastRunTime5, lastStateChangeTime, slownessThresholdInMsFromHistory, isNormalRunCountReset, isSlowRunCountReset, deltaNormalRunCount, deltaSlowRunCount);
          this.ResetNormalRunCounts();
          this.ResetSlowRunCounts();
          needResetHistory = true;
        }
        else
        {
          if (this.OptimizationState != QueryOptimizationState.NotOptimizable || !(this.LastStateChangeTime < lastRunTime.AddDays((double) (-1 * (int) QueryOptimizationStrategy.MaxDaysInNotOptimizable))) || !WorkItemTrackingFeatureFlags.IsResetNonOptimizableQueriesEnabled(requestContext))
            return;
          short? currentIndex = new short?((short) -1);
          DateTime? nullable = new DateTime?(DateTime.UtcNow);
          short? normalRunCount = new short?();
          short? slowRunCount = new short?();
          DateTime? lastRunTime6 = new DateTime?();
          DateTime? lastStateChangeTime = nullable;
          int? slownessThresholdInMsFromHistory = new int?();
          bool? isNormalRunCountReset = new bool?();
          bool? isSlowRunCountReset = new bool?();
          short? deltaNormalRunCount = new short?();
          short? deltaSlowRunCount = new short?();
          this.SetOptimizationStateInternal(QueryOptimizationState.None, currentIndex, normalRunCount, slowRunCount, lastRunTime6, lastStateChangeTime, slownessThresholdInMsFromHistory, isNormalRunCountReset, isSlowRunCountReset, deltaNormalRunCount, deltaSlowRunCount);
          this.ResetNormalRunCounts();
          this.ResetSlowRunCounts();
          needResetHistory = true;
        }
      }
    }

    public void GetStateClonedFrom(QueryOptimizationInstance source)
    {
      ArgumentUtility.CheckForNull<QueryOptimizationInstance>(source, nameof (source));
      lock (this.LockObject)
      {
        this.SetOptimizationStateInternal(source.OptimizationState, new short?(source.StrategyIndex), new short?(source.NormalRunCountInCurrentOpt), new short?(source.SlowRunCountInCurrentOpt), new DateTime?(source.LastRunTime), new DateTime?(source.LastStateChangeTime), new int?(source.SlownessThresholdInMsFromHistory), new bool?(source.IsNormalRunCountReset), new bool?(source.IsSlowRunCountReset), new short?(source.DeltaNormalRunCount), new short?(source.DeltaSlowRunCount));
        this.IsStateForkedFromOtherInstance = true;
      }
    }

    public QueryOptimizationInstance Clone()
    {
      lock (this.LockObject)
      {
        QueryOptimizationInstance optimizationInstance = new QueryOptimizationInstance(this.QueryId, this.QueryHash, this.OptimizationStrategy, this.OptimizationState);
        optimizationInstance.SetOptimizationStateInternal(this.OptimizationState, new short?(this.StrategyIndex), new short?(this.NormalRunCountInCurrentOpt), new short?(this.SlowRunCountInCurrentOpt), new DateTime?(this.LastRunTime), new DateTime?(this.LastStateChangeTime), new int?(this.SlownessThresholdInMsFromHistory), new bool?(this.IsNormalRunCountReset), new bool?(this.IsSlowRunCountReset), new short?(this.DeltaNormalRunCount), new short?(this.DeltaSlowRunCount));
        optimizationInstance.IsStateForkedFromOtherInstance = this.IsStateForkedFromOtherInstance;
        return optimizationInstance;
      }
    }

    protected override BufferableItem Combine(BufferableItem existingItem, BufferableItem newItem)
    {
      QueryOptimizationInstance existingInstance = (QueryOptimizationInstance) existingItem;
      if (newItem != null)
      {
        QueryOptimizationInstance newInstance = (QueryOptimizationInstance) newItem;
        if (QueryOptimizationInstance.IsOldInstanceReplaceable(existingInstance, newInstance))
        {
          existingInstance.SlowRunCountInCurrentOpt = newInstance.SlowRunCountInCurrentOpt;
          existingInstance.NormalRunCountInCurrentOpt = newInstance.NormalRunCountInCurrentOpt;
          existingInstance.DeltaSlowRunCount = newInstance.DeltaSlowRunCount;
          existingInstance.DeltaNormalRunCount = newInstance.DeltaNormalRunCount;
          existingInstance.OptimizationState = newInstance.OptimizationState;
          existingInstance.StrategyIndex = newInstance.StrategyIndex;
          existingInstance.LastRunTime = newInstance.LastRunTime;
          existingInstance.LastStateChangeTime = newInstance.LastStateChangeTime;
          existingInstance.SlownessThresholdInMsFromHistory = newInstance.SlownessThresholdInMsFromHistory;
          existingInstance.IsNormalRunCountReset = newInstance.IsNormalRunCountReset;
          existingInstance.IsSlowRunCountReset = newInstance.IsSlowRunCountReset;
          existingInstance.IsStateForkedFromOtherInstance = newInstance.IsStateForkedFromOtherInstance;
        }
      }
      return (BufferableItem) existingInstance;
    }

    internal void SetDeltaRunCounts(short? deltaNormalRunCount, short? deltaSlowRunCount)
    {
      if (deltaNormalRunCount.HasValue)
        this.DeltaNormalRunCount = deltaNormalRunCount.Value;
      if (!deltaSlowRunCount.HasValue)
        return;
      this.DeltaSlowRunCount = deltaSlowRunCount.Value;
    }

    internal void ResetSlowRunCounts()
    {
      this.DeltaSlowRunCount = (short) 0;
      this.SlowRunCountInCurrentOpt = (short) 0;
      this.IsSlowRunCountReset = true;
    }

    internal void ResetNormalRunCounts()
    {
      this.DeltaNormalRunCount = (short) 0;
      this.NormalRunCountInCurrentOpt = (short) 0;
      this.IsNormalRunCountReset = true;
    }

    private void SetOptimizationStateInternal(
      QueryOptimizationState optimizationState,
      short? currentIndex = null,
      short? normalRunCount = null,
      short? slowRunCount = null,
      DateTime? lastRunTime = null,
      DateTime? lastStateChangeTime = null,
      int? slownessThresholdInMsFromHistory = null,
      bool? isNormalRunCountReset = null,
      bool? isSlowRunCountReset = null,
      short? deltaNormalRunCount = null,
      short? deltaSlowRunCount = null)
    {
      this.OptimizationState = optimizationState;
      if (currentIndex.HasValue)
        this.StrategyIndex = currentIndex.Value;
      if (normalRunCount.HasValue)
        this.NormalRunCountInCurrentOpt = normalRunCount.Value;
      if (slowRunCount.HasValue)
        this.SlowRunCountInCurrentOpt = slowRunCount.Value;
      if (lastRunTime.HasValue)
        this.LastRunTime = lastRunTime.Value;
      if (lastStateChangeTime.HasValue)
        this.LastStateChangeTime = lastStateChangeTime.Value;
      if (slownessThresholdInMsFromHistory.HasValue)
        this.SlownessThresholdInMsFromHistory = slownessThresholdInMsFromHistory.Value;
      if (isNormalRunCountReset.HasValue)
        this.IsNormalRunCountReset = isNormalRunCountReset.Value;
      if (isSlowRunCountReset.HasValue)
        this.IsSlowRunCountReset = isSlowRunCountReset.Value;
      if (deltaNormalRunCount.HasValue)
        this.DeltaNormalRunCount = deltaNormalRunCount.Value;
      if (!deltaSlowRunCount.HasValue)
        return;
      this.DeltaSlowRunCount = deltaSlowRunCount.Value;
    }

    private bool MoveToNextOptimization(IVssRequestContext requestContext)
    {
      short? optimizationIndex = this.OptimizationStrategy.GetNextOptimizationIndex(requestContext, this.StrategyIndex);
      if (!optimizationIndex.HasValue)
        return false;
      this.LastStateChangeTime = DateTime.UtcNow;
      this.StrategyIndex = optimizationIndex.Value;
      return true;
    }

    public static bool IsOldInstanceReplaceable(
      QueryOptimizationInstance existingInstance,
      QueryOptimizationInstance newInstance)
    {
      if (newInstance == null || !QueryOptimizationInstanceComparer.Instance.Equals(existingInstance, newInstance))
        return false;
      DateTime dateTime1 = newInstance.LastStateChangeTime.AddTicks(-(newInstance.LastRunTime.Ticks % TimeSpan.FromMilliseconds(100.0).Ticks));
      DateTime dateTime2 = existingInstance.LastStateChangeTime.AddTicks(-(existingInstance.LastRunTime.Ticks % TimeSpan.FromMilliseconds(100.0).Ticks));
      DateTime dateTime3 = newInstance.LastRunTime.AddTicks(-(newInstance.LastRunTime.Ticks % TimeSpan.FromMilliseconds(100.0).Ticks));
      DateTime dateTime4 = existingInstance.LastRunTime.AddTicks(-(existingInstance.LastRunTime.Ticks % TimeSpan.FromMilliseconds(100.0).Ticks));
      return newInstance.OptimizationState > existingInstance.OptimizationState || newInstance.OptimizationState == QueryOptimizationState.InEvaluation && existingInstance.OptimizationState == QueryOptimizationState.None && dateTime1 >= dateTime2 || newInstance.IsStateForkedFromOtherInstance && dateTime3 >= dateTime4 || newInstance.OptimizationState == existingInstance.OptimizationState && ((int) newInstance.StrategyIndex > (int) existingInstance.StrategyIndex || (int) newInstance.StrategyIndex == (int) existingInstance.StrategyIndex && dateTime3 >= dateTime4);
    }
  }
}
