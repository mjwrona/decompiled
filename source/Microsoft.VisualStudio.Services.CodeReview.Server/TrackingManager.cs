// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.TrackingManager
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.VisualStudio.Services.CodeReview.Server.Common;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  internal class TrackingManager
  {
    public CommentTrackingCriteria TrackingCriteria;
    public Dictionary<TrackingFileSnapshot, TrackingFileSnapshot> FileSnapshots;
    public Dictionary<TrackingFileSnapshotPair, TrackingFileSnapshotPair> FileSnapshotPairs;

    public TrackingManager(CommentTrackingCriteria trackingCriteria)
    {
      this.TrackingCriteria = trackingCriteria;
      this.FileSnapshots = new Dictionary<TrackingFileSnapshot, TrackingFileSnapshot>();
      this.FileSnapshotPairs = new Dictionary<TrackingFileSnapshotPair, TrackingFileSnapshotPair>();
    }

    public void AddTrackingPairs(List<CommentThread> threadList, TrackingTarget trackingTarget = TrackingTarget.Primary)
    {
      foreach (CommentThread thread in threadList)
      {
        TrackingContext contextForThread = this.GetTrackingContextForThread(thread, trackingTarget);
        TrackingFileSnapshot forTrackingContext1 = this.GetSnapshotForTrackingContext(contextForThread, TrackingSnapshotVersion.Source);
        TrackingFileSnapshot forTrackingContext2 = this.GetSnapshotForTrackingContext(contextForThread, TrackingSnapshotVersion.Target);
        if (!this.FileSnapshots.ContainsKey(forTrackingContext1))
          this.FileSnapshots[forTrackingContext1] = forTrackingContext1;
        if (!this.FileSnapshots.ContainsKey(forTrackingContext2))
          this.FileSnapshots[forTrackingContext2] = forTrackingContext2;
        this.FileSnapshots[forTrackingContext1].IncrementFileCount();
        this.FileSnapshots[forTrackingContext2].IncrementFileCount();
        this.FileSnapshots[forTrackingContext1].AddThread(thread.DiscussionId);
        this.FileSnapshots[forTrackingContext2].AddThread(thread.DiscussionId);
        TrackingFileSnapshotPair key = contextForThread.TrackBackward ? new TrackingFileSnapshotPair(this.FileSnapshots[forTrackingContext2], this.FileSnapshots[forTrackingContext1], contextForThread) : new TrackingFileSnapshotPair(this.FileSnapshots[forTrackingContext1], this.FileSnapshots[forTrackingContext2], contextForThread);
        if (!this.FileSnapshotPairs.ContainsKey(key))
          this.FileSnapshotPairs[key] = key;
        this.FileSnapshotPairs[key].IncrementDiffCount();
      }
    }

    public void AddChangeEntries(List<ChangeEntry> changeEntries)
    {
      foreach (ChangeEntry changeEntry in changeEntries)
      {
        int? iterationId = changeEntry.IterationId;
        TrackingFileSnapshot key1 = new TrackingFileSnapshot(iterationId.GetValueOrDefault(), changeEntry.ChangeTrackingId, TrackingVersion.Base);
        iterationId = changeEntry.IterationId;
        TrackingFileSnapshot key2 = new TrackingFileSnapshot(iterationId.GetValueOrDefault(), changeEntry.ChangeTrackingId, TrackingVersion.Modified);
        if (changeEntry.Base != null && this.FileSnapshots.ContainsKey(key1) && string.IsNullOrEmpty(this.FileSnapshots[key1].FileHash))
        {
          this.FileSnapshots[key1].FileHash = changeEntry.Base.SHA1Hash;
          this.FileSnapshots[key1].Filename = changeEntry.Base.Path;
          this.FileSnapshots[key1].SecondaryFilename = changeEntry.Modified?.Path;
        }
        if (changeEntry.Modified != null && this.FileSnapshots.ContainsKey(key2) && string.IsNullOrEmpty(this.FileSnapshots[key2].FileHash))
        {
          this.FileSnapshots[key2].FileHash = changeEntry.Modified.SHA1Hash;
          this.FileSnapshots[key2].Filename = changeEntry.Modified.Path;
          this.FileSnapshots[key2].SecondaryFilename = changeEntry.Base?.Path;
        }
      }
    }

    public void CleanUpTrackingPairs(List<CommentThread> threadList, ClientTraceData ctData = null)
    {
      ctData?.Add("CommentTrackingSnapshotPairs", (object) this.GetSnapshotPairs().Select<TrackingFileSnapshotPair, object>((Func<TrackingFileSnapshotPair, object>) (pair => pair.FormatCI())).ToArray<object>());
      foreach (CommentThread thread in threadList)
      {
        TrackingContext contextForThread1 = this.GetTrackingContextForThread(thread);
        TrackingContext contextForThread2 = this.GetTrackingContextForThread(thread, TrackingTarget.Secondary);
        TrackingFileSnapshot forTrackingContext1 = this.GetSnapshotForTrackingContext(contextForThread1, TrackingSnapshotVersion.Source);
        TrackingFileSnapshot forTrackingContext2 = this.GetSnapshotForTrackingContext(contextForThread1, TrackingSnapshotVersion.Target);
        TrackingFileSnapshot forTrackingContext3 = this.GetSnapshotForTrackingContext(contextForThread2, TrackingSnapshotVersion.Target);
        TrackingFileSnapshotPair key1 = new TrackingFileSnapshotPair(forTrackingContext1, forTrackingContext2);
        TrackingFileSnapshotPair key2 = new TrackingFileSnapshotPair(forTrackingContext1, forTrackingContext3);
        if (this.FileSnapshotPairs.ContainsKey(key1) && this.FileSnapshotPairs[key1].IsPairValid() && this.FileSnapshotPairs.ContainsKey(key2))
          this.FileSnapshotPairs[key2].DecrementDiffCount();
      }
      List<TrackingFileSnapshotPair> fileSnapshotPairList = new List<TrackingFileSnapshotPair>();
      foreach (TrackingFileSnapshotPair fileSnapshotPair in this.FileSnapshotPairs.Values)
      {
        if (fileSnapshotPair.ClearIfInvalid())
          fileSnapshotPairList.Add(fileSnapshotPair);
      }
      foreach (TrackingFileSnapshotPair key in fileSnapshotPairList)
        this.FileSnapshotPairs.Remove(key);
      ctData?.Add("CommentTrackingCleanSnapshotPairs", (object) this.GetSnapshotPairs().Select<TrackingFileSnapshotPair, object>((Func<TrackingFileSnapshotPair, object>) (pair => pair.FormatCI())).ToArray<object>());
    }

    public void SetFileStreams(
      Dictionary<string, ChangeEntryStream> hashToStream,
      bool determineEncodings = false,
      long fileSizeLimit = 9223372036854775807)
    {
      Dictionary<string, TrackingFileSnapshot> dictionary = new Dictionary<string, TrackingFileSnapshot>();
      foreach (TrackingFileSnapshotPair fileSnapshotPair in this.FileSnapshotPairs.Values)
      {
        if (!fileSnapshotPair.HasCachedDiffData())
        {
          foreach (TrackingFileSnapshot trackingFileSnapshot in new List<TrackingFileSnapshot>()
          {
            fileSnapshotPair.First,
            fileSnapshotPair.Second
          })
          {
            if (trackingFileSnapshot.IsValid() && hashToStream.ContainsKey(trackingFileSnapshot.FileHash))
            {
              if (dictionary.ContainsKey(trackingFileSnapshot.FileHash))
              {
                trackingFileSnapshot.SetFileData(dictionary[trackingFileSnapshot.FileHash].PeekFileData());
                trackingFileSnapshot.SetEncoding(dictionary[trackingFileSnapshot.FileHash].GetFileEncoding());
              }
              else if (hashToStream[trackingFileSnapshot.FileHash].FileStream != null)
              {
                if (hashToStream[trackingFileSnapshot.FileHash].FileStream.Length <= fileSizeLimit)
                {
                  if (determineEncodings)
                    trackingFileSnapshot.SetEncodingFromStream(hashToStream[trackingFileSnapshot.FileHash].FileStream);
                  else
                    trackingFileSnapshot.SetFileDataFromStream(hashToStream[trackingFileSnapshot.FileHash].FileStream);
                }
                else
                  hashToStream[trackingFileSnapshot.FileHash].FileStream.Dispose();
                dictionary[trackingFileSnapshot.FileHash] = trackingFileSnapshot;
              }
            }
          }
        }
      }
    }

    public int TakeFileDiffs()
    {
      int fileDiffs = 0;
      foreach (TrackingFileSnapshotPair fileSnapshotPair in this.FileSnapshotPairs.Values)
      {
        if (!fileSnapshotPair.HasDiffData() && fileSnapshotPair.First.HasFileData() && fileSnapshotPair.Second.HasFileData())
        {
          fileSnapshotPair.DiffFiles();
          ++fileDiffs;
        }
      }
      return fileDiffs;
    }

    public TrackingContext GetTrackingContextForThread(
      CommentThread thread,
      TrackingTarget trackingTarget = TrackingTarget.Primary)
    {
      TrackingContext context = new TrackingContext();
      CommentThreadContext threadContext = thread.ThreadContext;
      context.TrackingTarget = trackingTarget;
      context.FirstComparingIteration = this.TrackingCriteria.FirstComparingIteration;
      context.SecondComparingIteration = this.TrackingCriteria.SecondComparingIteration;
      context.ChangeTrackingId = threadContext.ChangeTrackingId;
      context.SourceFilename = threadContext.FilePath;
      context.TargetFilename = threadContext.FilePath;
      int num = threadContext.RightFileStart != null ? 1 : 0;
      bool flag1 = (int) threadContext.IterationContext.FirstComparingIteration != (int) threadContext.IterationContext.SecondComparingIteration;
      bool flag2 = context.FirstComparingIteration != context.SecondComparingIteration;
      if (num != 0)
      {
        context.SourceIteration = (int) threadContext.IterationContext.SecondComparingIteration;
        context.SourceSide = TrackingSide.Right;
        context.SourceVersion = TrackingVersion.Modified;
        context.OrigStartLine = threadContext.RightFileStart.Line;
        context.OrigStartOffset = threadContext.RightFileStart.Offset;
        context.OrigEndLine = threadContext.RightFileEnd.Line;
        context.OrigEndOffset = threadContext.RightFileEnd.Offset;
      }
      else
      {
        context.SourceIteration = (int) threadContext.IterationContext.FirstComparingIteration;
        context.SourceSide = TrackingSide.Left;
        context.SourceVersion = flag1 ? TrackingVersion.Modified : TrackingVersion.Base;
        context.OrigStartLine = threadContext.LeftFileStart.Line;
        context.OrigStartOffset = threadContext.LeftFileStart.Offset;
        context.OrigEndLine = threadContext.LeftFileEnd.Line;
        context.OrigEndOffset = threadContext.LeftFileEnd.Offset;
      }
      if (!flag2 && context.SourceVersion == TrackingVersion.Base)
      {
        context.TargetIteration = context.FirstComparingIteration;
        context.TargetSide = TrackingSide.Left;
        context.TargetVersion = TrackingVersion.Base;
      }
      else
      {
        context.TargetVersion = TrackingVersion.Modified;
        if (context.SourceIteration <= context.FirstComparingIteration)
        {
          context.TargetIteration = context.FirstComparingIteration;
          context.TargetSide = flag2 ? TrackingSide.Left : TrackingSide.Right;
        }
        else
        {
          context.TargetIteration = context.SecondComparingIteration;
          context.TargetSide = TrackingSide.Right;
        }
      }
      if (trackingTarget == TrackingTarget.Secondary)
      {
        if (flag2)
          context.TargetIteration = context.TargetIteration == context.FirstComparingIteration ? context.SecondComparingIteration : context.FirstComparingIteration;
        else
          context.TargetVersion = context.TargetVersion == TrackingVersion.Base ? TrackingVersion.Modified : TrackingVersion.Base;
        context.TargetSide = context.TargetSide == TrackingSide.Left ? TrackingSide.Right : TrackingSide.Left;
      }
      context.TrackBackward = context.SourceIteration > context.TargetIteration || context.SourceIteration == context.TargetIteration && context.SourceVersion == TrackingVersion.Modified && context.TargetVersion == TrackingVersion.Base;
      TrackingFileSnapshot forTrackingContext = this.GetSnapshotForTrackingContext(context, TrackingSnapshotVersion.Target);
      if (forTrackingContext.Filename != null)
        context.TargetFilename = forTrackingContext.Filename;
      if (context.TargetVersion == TrackingVersion.Base && forTrackingContext.SecondaryFilename != null)
        context.TargetFilename = forTrackingContext.SecondaryFilename;
      return context;
    }

    public TrackingFileSnapshot GetSnapshotForTrackingContext(
      TrackingContext context,
      TrackingSnapshotVersion snapshotVersion)
    {
      int iterationId = snapshotVersion == TrackingSnapshotVersion.Source ? context.SourceIteration : context.TargetIteration;
      TrackingVersion trackingVersion = snapshotVersion == TrackingSnapshotVersion.Source ? context.SourceVersion : context.TargetVersion;
      int changeTrackingId = context.ChangeTrackingId;
      int version = (int) trackingVersion;
      TrackingFileSnapshot key = new TrackingFileSnapshot(iterationId, changeTrackingId, (TrackingVersion) version);
      return this.FileSnapshots.ContainsKey(key) ? this.FileSnapshots[key] : key;
    }

    public bool IsAllDataCached(ClientTraceData ctData = null)
    {
      bool flag = true;
      foreach (TrackingFileSnapshotPair fileSnapshotPair in this.FileSnapshotPairs.Values)
      {
        if (!fileSnapshotPair.HasCachedDiffData())
        {
          flag = false;
          break;
        }
      }
      ctData?.Add("CommentTrackingAllDataCached", (object) flag);
      return flag;
    }

    public List<IDiffChange> GetDiffData(TrackingContext context)
    {
      TrackingFileSnapshotPair key = new TrackingFileSnapshotPair(this.GetSnapshotForTrackingContext(context, TrackingSnapshotVersion.Source), this.GetSnapshotForTrackingContext(context, TrackingSnapshotVersion.Target));
      return this.FileSnapshotPairs.ContainsKey(key) ? this.FileSnapshotPairs[key].GetDiffData() ?? new List<IDiffChange>(0) : (List<IDiffChange>) null;
    }

    public DiffCacheObject GetDiffDataAsCache()
    {
      DiffCacheObject cache = new DiffCacheObject();
      foreach (TrackingFileSnapshotPair fileSnapshotPair in this.FileSnapshotPairs.Values)
        fileSnapshotPair.PopulateCache(cache);
      return cache;
    }

    public void PopulateDiffDataFromCache(DiffCacheObject diffCacheObject, ClientTraceData ctData = null)
    {
      if (diffCacheObject != null)
      {
        foreach (TrackingFileSnapshotPair fileSnapshotPair in this.FileSnapshotPairs.Values)
        {
          if (!fileSnapshotPair.HasCachedDiffData())
          {
            string key = fileSnapshotPair.ToString();
            if (diffCacheObject.DiffCacheEntries.ContainsKey(key))
              fileSnapshotPair.PopulateDiffDataFromCache(diffCacheObject.DiffCacheEntries[key]);
          }
        }
      }
      ctData?.Add("CommentTrackingCacheHit", (object) (diffCacheObject != null));
    }

    public List<int> GetIterations()
    {
      List<int> source = new List<int>();
      foreach (TrackingFileSnapshotPair fileSnapshotPair in this.FileSnapshotPairs.Values)
      {
        source.Add(fileSnapshotPair.First.IterationId);
        source.Add(fileSnapshotPair.Second.IterationId);
      }
      return source.Distinct<int>().ToList<int>();
    }

    public List<string> GetHashes(bool onlyGetUncached = false)
    {
      List<string> source = new List<string>();
      foreach (TrackingFileSnapshotPair fileSnapshotPair in this.FileSnapshotPairs.Values)
      {
        if (!onlyGetUncached || !fileSnapshotPair.HasCachedDiffData())
        {
          if (!string.IsNullOrEmpty(fileSnapshotPair.First.FileHash))
            source.Add(fileSnapshotPair.First.FileHash);
          if (!string.IsNullOrEmpty(fileSnapshotPair.Second.FileHash))
            source.Add(fileSnapshotPair.Second.FileHash);
        }
      }
      return source.Distinct<string>().ToList<string>();
    }

    public bool IsCompareMode() => this.TrackingCriteria.FirstComparingIteration != this.TrackingCriteria.SecondComparingIteration;

    public void Clear()
    {
      this.FileSnapshots.Clear();
      this.FileSnapshotPairs.Clear();
    }

    public IEnumerable<TrackingFileSnapshot> GetSnapshots() => (IEnumerable<TrackingFileSnapshot>) this.FileSnapshots.Keys.ToList<TrackingFileSnapshot>();

    public IEnumerable<TrackingFileSnapshotPair> GetSnapshotPairs() => (IEnumerable<TrackingFileSnapshotPair>) this.FileSnapshotPairs.Keys.ToList<TrackingFileSnapshotPair>();
  }
}
