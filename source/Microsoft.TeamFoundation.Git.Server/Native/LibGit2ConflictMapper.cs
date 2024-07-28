// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Native.LibGit2ConflictMapper
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using LibGit2Sharp;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server.Native
{
  internal class LibGit2ConflictMapper
  {
    private readonly IVssRequestContext requestContext;
    private readonly GitConflictSourceType conflictSourceType;
    private readonly int conflictSourceId;
    private readonly Sha1Id mergeBase;
    private readonly Sha1Id mergeSource;
    private readonly Sha1Id mergeTarget;
    private readonly IEnumerable<Conflict> nativeConflicts;
    private readonly IEnumerable<IndexReucEntry> conflictsResolvedByLibGit2;
    private readonly IEnumerable<LibGit2Sharp.IndexEntry> changedFiles;
    private readonly bool detectRenameFalsePositives;
    private Dictionary<ObjectId, HashSet<LibGit2ConflictMapper.ConflictEntry>> conflictLookupByFileHash = new Dictionary<ObjectId, HashSet<LibGit2ConflictMapper.ConflictEntry>>(300);
    private Dictionary<ObjectId, HashSet<IndexReucEntry>> resolvedConflictsByHash = new Dictionary<ObjectId, HashSet<IndexReucEntry>>(300);
    private Dictionary<ObjectId, HashSet<LibGit2Sharp.IndexEntry>> nonConflictFilesByHash = new Dictionary<ObjectId, HashSet<LibGit2Sharp.IndexEntry>>(300);
    private Dictionary<string, LibGit2ConflictMapper.ConflictEntry> allConflicts;
    private List<LibGit2ConflictMapper.Rename> sourceRenames;
    private List<LibGit2ConflictMapper.Rename> targetRenames;
    private const string traceLayer = "LibGit2ConflictMapper";
    private List<Microsoft.TeamFoundation.Git.Server.GitConflict> conflictList;
    private List<TrivialConflict> trivialConflictList;

    public LibGit2ConflictMapper(
      IVssRequestContext requestContext,
      GitMergeOriginRef mergeOrigin,
      Sha1Id mergeBase,
      Sha1Id mergeSource,
      Sha1Id mergeTarget,
      IEnumerable<Conflict> nativeConflicts,
      IEnumerable<IndexReucEntry> conflictsAutoResolvedByLibGit2 = null,
      IEnumerable<LibGit2Sharp.IndexEntry> changedFiles = null,
      bool detectRenameFalsePositives = false)
    {
      this.requestContext = requestContext;
      this.conflictSourceId = mergeOrigin.PullRequestId.Value;
      this.conflictSourceType = GitConflictSourceType.PullRequest;
      this.mergeBase = mergeBase;
      this.mergeSource = mergeSource;
      this.mergeTarget = mergeTarget;
      this.nativeConflicts = nativeConflicts;
      this.changedFiles = changedFiles;
      this.conflictsResolvedByLibGit2 = conflictsAutoResolvedByLibGit2;
      this.detectRenameFalsePositives = detectRenameFalsePositives;
    }

    public void MapLibGit2Conflicts(
      out List<Microsoft.TeamFoundation.Git.Server.GitConflict> conflicts,
      out List<TrivialConflict> trivialConflicts)
    {
      Stopwatch timer = Stopwatch.StartNew();
      this.allConflicts = this.nativeConflicts.Select<Conflict, LibGit2ConflictMapper.ConflictEntry>((Func<Conflict, LibGit2ConflictMapper.ConflictEntry>) (n => new LibGit2ConflictMapper.ConflictEntry()
      {
        Path = (n.Ancestor?.Path ?? n.Ours?.Path ?? n.Theirs?.Path)?.Replace('\\', '/'),
        Base = n.Ancestor?.Id == ObjectId.Zero ? (ObjectId) null : n.Ancestor?.Id,
        Source = n.Theirs?.Id == ObjectId.Zero ? (ObjectId) null : n.Theirs?.Id,
        Target = n.Ours?.Id == ObjectId.Zero ? (ObjectId) null : n.Ours?.Id,
        Mapped = false
      })).Where<LibGit2ConflictMapper.ConflictEntry>((Func<LibGit2ConflictMapper.ConflictEntry, bool>) (c => c.Path != null)).ToDictionary<LibGit2ConflictMapper.ConflictEntry, string>((Func<LibGit2ConflictMapper.ConflictEntry, string>) (c => c.Path));
      try
      {
        TracepointUtils.Tracepoint(this.requestContext, 1013048, GitServerUtils.TraceArea, nameof (LibGit2ConflictMapper), (Func<object>) (() => (object) new
        {
          conflictSourceType = this.conflictSourceType,
          conflictSourceId = this.conflictSourceId,
          mergeBase = this.mergeBase,
          mergeSource = this.mergeSource,
          mergeTarget = this.mergeTarget,
          count = this.allConflicts.Count,
          nativeConflicts = this.allConflicts.Values.Select(c => new
          {
            path = c.Path,
            baseObjectId = c.Base?.ToString() ?? "",
            sourceObjectId = c.Source?.ToString() ?? "",
            targetObjectId = c.Target?.ToString() ?? ""
          }).ToList()
        }), TraceLevel.Info, caller: nameof (MapLibGit2Conflicts));
      }
      catch
      {
      }
      this.conflictList = conflicts = new List<Microsoft.TeamFoundation.Git.Server.GitConflict>(this.allConflicts.Count);
      this.trivialConflictList = trivialConflicts = new List<TrivialConflict>(this.allConflicts.Count);
      this.BuildHashLookups();
      this.BuildRenameLists();
      this.FindAllRename2to1Conflicts();
      this.FindAllRename1to2Conflicts();
      this.FindAllRenameRenameConflicts();
      this.FindAllAddRenameConflicts();
      this.FindAllRenameAddConflicts();
      this.FindAllRenameDeleteConflicts();
      this.FindAllDeleteRenameConflicts();
      this.FindAllEditEditConflicts();
      this.FindAllEditDeleteConflicts();
      this.FindAllDeleteEditConflicts();
      this.FindAllAddAddConflicts();
      this.FindAllTrivialConflicts();
      try
      {
        TracepointUtils.Tracepoint(this.requestContext, 1013049, GitServerUtils.TraceArea, nameof (LibGit2ConflictMapper), (Func<object>) (() => (object) new
        {
          conflictSourceType = this.conflictSourceType,
          conflictSourceId = this.conflictSourceId,
          mergeBase = this.mergeBase,
          mergeSource = this.mergeSource,
          mergeTarget = this.mergeTarget,
          elapsedMs = timer.ElapsedMilliseconds,
          restCount = this.conflictList.Count,
          nativeCount = this.allConflicts.Count,
          restConflicts = this.conflictList.Select(c =>
          {
            string str1 = c.ConflictType.ToString() ?? "";
            string str2 = c.ConflictPath?.ToString() ?? "";
            string str3 = c.SourcePath?.ToString() ?? "";
            string str4 = c.TargetPath?.ToString() ?? "";
            Sha1Id sha1Id = c.BaseObjectId;
            string str5 = sha1Id.ToString() ?? "";
            sha1Id = c.BaseObjectIdForTarget;
            string str6 = sha1Id.ToString() ?? "";
            sha1Id = c.SourceObjectId;
            string str7 = sha1Id.ToString() ?? "";
            sha1Id = c.TargetObjectId;
            string str8 = sha1Id.ToString() ?? "";
            return new
            {
              conflictType = str1,
              conflictPath = str2,
              sourcePath = str3,
              targetPath = str4,
              baseObjectId = str5,
              baseObjectIdForTarget = str6,
              sourceObjectId = str7,
              targetObjectId = str8
            };
          }).ToList(),
          nativeConflicts = this.allConflicts.Values.Select(c => new
          {
            path = c.Path,
            baseObjectId = c.Base?.ToString() ?? "",
            sourceObjectId = c.Source?.ToString() ?? "",
            targetObjectId = c.Target?.ToString() ?? ""
          }).ToList()
        }), !this.allConflicts.Any<KeyValuePair<string, LibGit2ConflictMapper.ConflictEntry>>() ? TraceLevel.Info : TraceLevel.Error, caller: nameof (MapLibGit2Conflicts));
      }
      catch
      {
      }
    }

    private void BuildHashLookups()
    {
      foreach (LibGit2ConflictMapper.ConflictEntry conflictEntry in this.allConflicts.Values)
        this.AddConflictToLookupByFileHash(conflictEntry);
      if (this.detectRenameFalsePositives && this.conflictsResolvedByLibGit2 != null)
      {
        foreach (IndexReucEntry indexReucEntry in this.conflictsResolvedByLibGit2)
          this.AddResolvedConflictToLookupByFileHash(indexReucEntry);
      }
      if (!this.detectRenameFalsePositives || this.changedFiles == null)
        return;
      foreach (LibGit2Sharp.IndexEntry changedFile in this.changedFiles)
      {
        if (changedFile.StageLevel == StageLevel.Staged)
          this.AddNonConflictFileToLookupByFileHash(changedFile);
      }
    }

    private void FoundGitConflict(
      GitConflictType conflictType,
      string conflictPath,
      string sourcePath = null,
      string targetPath = null,
      ObjectId baseObjectId = null,
      ObjectId baseObjectIdForTarget = null,
      ObjectId sourceObjectId = null,
      ObjectId targetObjectId = null)
    {
      this.conflictList.Add(new Microsoft.TeamFoundation.Git.Server.GitConflict(this.conflictSourceType, this.conflictSourceId, this.conflictList.Count + 1, this.mergeBase, this.mergeSource, this.mergeTarget, conflictType, new NormalizedGitPath(conflictPath), sourcePath != null ? new NormalizedGitPath(sourcePath) : (NormalizedGitPath) null, targetPath != null ? new NormalizedGitPath(targetPath) : (NormalizedGitPath) null, baseObjectId != (ObjectId) null ? new Sha1Id(baseObjectId.RawId) : Sha1Id.Empty, baseObjectIdForTarget != (ObjectId) null ? new Sha1Id(baseObjectIdForTarget.RawId) : Sha1Id.Empty, sourceObjectId != (ObjectId) null ? new Sha1Id(sourceObjectId.RawId) : Sha1Id.Empty, targetObjectId != (ObjectId) null ? new Sha1Id(targetObjectId.RawId) : Sha1Id.Empty));
    }

    private void ConflictEntryIsMapped(LibGit2ConflictMapper.ConflictEntry conflictEntry)
    {
      conflictEntry.Mapped = true;
      this.RemoveConflictFromLookupByFileHash(conflictEntry);
      this.allConflicts.Remove(conflictEntry.Path);
    }

    private void FindAllRename2to1Conflicts()
    {
      foreach (var data in this.sourceRenames.Join((IEnumerable<LibGit2ConflictMapper.Rename>) this.targetRenames, (Func<LibGit2ConflictMapper.Rename, LibGit2ConflictMapper.ConflictEntry>) (sourceRename => sourceRename.Destination), (Func<LibGit2ConflictMapper.Rename, LibGit2ConflictMapper.ConflictEntry>) (targetRename => targetRename.Destination), (sourceRename, targetRename) => new
      {
        sourceRename = sourceRename,
        targetRename = targetRename
      }).Where(_param1 => _param1.sourceRename.Origin != _param1.targetRename.Origin).Select(_param1 => new
      {
        sourceRename = _param1.sourceRename,
        targetRename = _param1.targetRename
      }).ToList())
      {
        LibGit2ConflictMapper.ConflictEntry sourceOrigin = data.sourceRename.Origin;
        LibGit2ConflictMapper.ConflictEntry targetOrigin = data.targetRename.Origin;
        LibGit2ConflictMapper.ConflictEntry destination = data.sourceRename.Destination;
        if (sourceOrigin.Mapped || targetOrigin.Mapped || destination.Mapped)
        {
          TracepointUtils.Tracepoint(this.requestContext, 1013000, GitServerUtils.TraceArea, nameof (LibGit2ConflictMapper), (Func<object>) (() => (object) new
          {
            conflictSourceType = this.conflictSourceType,
            conflictSourceId = this.conflictSourceId,
            mergeBase = this.mergeBase,
            mergeTarget = this.mergeTarget,
            mergeSource = this.mergeSource,
            conflictPath = destination.Path,
            sourcePath = sourceOrigin.Path,
            targetPath = targetOrigin.Path
          }), TraceLevel.Error, caller: nameof (FindAllRename2to1Conflicts));
        }
        else
        {
          this.ConflictEntryIsMapped(sourceOrigin);
          this.ConflictEntryIsMapped(targetOrigin);
          this.ConflictEntryIsMapped(destination);
          this.sourceRenames.Remove(data.sourceRename);
          this.targetRenames.Remove(data.targetRename);
          if (this.detectRenameFalsePositives && destination.Source.Equals(destination.Target))
          {
            this.FoundTrivialConflict(destination.Path, TrivialConflictResolution.TakeTarget);
            this.FoundTrivialConflict(sourceOrigin.Path, TrivialConflictResolution.Delete);
            this.FoundTrivialConflict(targetOrigin.Path, TrivialConflictResolution.Delete);
          }
          else
            this.FoundGitConflict(GitConflictType.Rename2to1, destination.Path, sourceOrigin.Path, targetOrigin.Path, sourceOrigin.Base, targetOrigin.Base, destination.Source, destination.Target);
        }
      }
    }

    private void FoundTrivialConflict(string path, TrivialConflictResolution resolution) => this.trivialConflictList.Add(new TrivialConflict()
    {
      Path = new NormalizedGitPath(path),
      Resolution = resolution
    });

    private void FindAllRename1to2Conflicts()
    {
      foreach (var data in this.sourceRenames.Join((IEnumerable<LibGit2ConflictMapper.Rename>) this.targetRenames, (Func<LibGit2ConflictMapper.Rename, LibGit2ConflictMapper.ConflictEntry>) (sourceRename => sourceRename.Origin), (Func<LibGit2ConflictMapper.Rename, LibGit2ConflictMapper.ConflictEntry>) (targetRename => targetRename.Origin), (sourceRename, targetRename) => new
      {
        sourceRename = sourceRename,
        targetRename = targetRename
      }).Where(_param1 => _param1.sourceRename.Destination != _param1.targetRename.Destination).Select(_param1 => new
      {
        sourceRename = _param1.sourceRename,
        targetRename = _param1.targetRename
      }).ToList())
      {
        LibGit2ConflictMapper.ConflictEntry origin = data.sourceRename.Origin;
        LibGit2ConflictMapper.ConflictEntry sourceDestination = data.sourceRename.Destination;
        LibGit2ConflictMapper.ConflictEntry targetDestination = data.targetRename.Destination;
        if (origin.Mapped || sourceDestination.Mapped || targetDestination.Mapped)
        {
          TracepointUtils.Tracepoint(this.requestContext, 1013001, GitServerUtils.TraceArea, nameof (LibGit2ConflictMapper), (Func<object>) (() => (object) new
          {
            conflictSourceType = this.conflictSourceType,
            conflictSourceId = this.conflictSourceId,
            mergeBase = this.mergeBase,
            mergeTarget = this.mergeTarget,
            mergeSource = this.mergeSource,
            conflictPath = origin.Path,
            sourcePath = sourceDestination.Path,
            targetPath = targetDestination.Path
          }), TraceLevel.Error, caller: nameof (FindAllRename1to2Conflicts));
        }
        else
        {
          this.ConflictEntryIsMapped(origin);
          this.ConflictEntryIsMapped(sourceDestination);
          this.ConflictEntryIsMapped(targetDestination);
          this.sourceRenames.Remove(data.sourceRename);
          this.targetRenames.Remove(data.targetRename);
          this.FoundGitConflict(GitConflictType.Rename1to2, origin.Path, sourceDestination.Path, targetDestination.Path, origin.Base, sourceObjectId: sourceDestination.Source, targetObjectId: targetDestination.Target);
        }
      }
    }

    private void FindAllRenameRenameConflicts()
    {
      foreach (var data in this.sourceRenames.Join((IEnumerable<LibGit2ConflictMapper.Rename>) this.targetRenames, (Func<LibGit2ConflictMapper.Rename, LibGit2ConflictMapper.ConflictEntry>) (sourceRename => sourceRename.Origin), (Func<LibGit2ConflictMapper.Rename, LibGit2ConflictMapper.ConflictEntry>) (targetRename => targetRename.Origin), (sourceRename, targetRename) => new
      {
        sourceRename = sourceRename,
        targetRename = targetRename
      }).Where(_param1 => _param1.sourceRename.Destination == _param1.targetRename.Destination).Select(_param1 => new
      {
        sourceRename = _param1.sourceRename,
        targetRename = _param1.targetRename
      }).ToList())
      {
        LibGit2ConflictMapper.ConflictEntry origin = data.sourceRename.Origin;
        LibGit2ConflictMapper.ConflictEntry destination = data.targetRename.Destination;
        if (origin.Mapped || destination.Mapped)
        {
          TracepointUtils.Tracepoint(this.requestContext, 1013002, GitServerUtils.TraceArea, nameof (LibGit2ConflictMapper), (Func<object>) (() => (object) new
          {
            conflictSourceType = this.conflictSourceType,
            conflictSourceId = this.conflictSourceId,
            mergeBase = this.mergeBase,
            mergeTarget = this.mergeTarget,
            mergeSource = this.mergeSource,
            sourcePath = origin.Path,
            conflictPath = destination.Path
          }), TraceLevel.Error, caller: nameof (FindAllRenameRenameConflicts));
        }
        else
        {
          this.ConflictEntryIsMapped(origin);
          this.ConflictEntryIsMapped(destination);
          this.sourceRenames.Remove(data.sourceRename);
          this.targetRenames.Remove(data.targetRename);
          this.FoundGitConflict(GitConflictType.RenameRename, destination.Path, origin.Path, baseObjectId: origin.Base, sourceObjectId: destination.Source, targetObjectId: destination.Target);
        }
      }
    }

    private void FindAllRenameDeleteConflicts()
    {
      foreach (LibGit2ConflictMapper.Rename rename in this.sourceRenames.Where<LibGit2ConflictMapper.Rename>((Func<LibGit2ConflictMapper.Rename, bool>) (rename => rename.Origin.Target == (ObjectId) null && rename.Destination.Target == (ObjectId) null)).ToList<LibGit2ConflictMapper.Rename>())
      {
        LibGit2ConflictMapper.ConflictEntry origin = rename.Origin;
        LibGit2ConflictMapper.ConflictEntry destination = rename.Destination;
        if (origin.Mapped || destination.Mapped)
        {
          TracepointUtils.Tracepoint(this.requestContext, 1013003, GitServerUtils.TraceArea, nameof (LibGit2ConflictMapper), (Func<object>) (() => (object) new
          {
            conflictSourceType = this.conflictSourceType,
            conflictSourceId = this.conflictSourceId,
            mergeBase = this.mergeBase,
            mergeTarget = this.mergeTarget,
            mergeSource = this.mergeSource,
            conflictPath = origin.Path,
            sourcePath = destination.Path
          }), TraceLevel.Error, caller: nameof (FindAllRenameDeleteConflicts));
        }
        else
        {
          this.ConflictEntryIsMapped(origin);
          this.ConflictEntryIsMapped(destination);
          this.sourceRenames.Remove(rename);
          this.FoundGitConflict(GitConflictType.RenameDelete, origin.Path, destination.Path, baseObjectId: origin.Base, sourceObjectId: destination.Source);
        }
      }
    }

    private void FindAllDeleteRenameConflicts()
    {
      foreach (LibGit2ConflictMapper.Rename rename in this.targetRenames.Where<LibGit2ConflictMapper.Rename>((Func<LibGit2ConflictMapper.Rename, bool>) (rename => rename.Origin.Source == (ObjectId) null && rename.Destination.Source == (ObjectId) null)).ToList<LibGit2ConflictMapper.Rename>())
      {
        LibGit2ConflictMapper.ConflictEntry origin = rename.Origin;
        LibGit2ConflictMapper.ConflictEntry destination = rename.Destination;
        if (origin.Mapped || destination.Mapped)
        {
          TracepointUtils.Tracepoint(this.requestContext, 1013005, GitServerUtils.TraceArea, nameof (LibGit2ConflictMapper), (Func<object>) (() => (object) new
          {
            conflictSourceType = this.conflictSourceType,
            conflictSourceId = this.conflictSourceId,
            mergeBase = this.mergeBase,
            mergeTarget = this.mergeTarget,
            mergeSource = this.mergeSource,
            conflictPath = origin.Path,
            targetPath = destination.Path
          }), TraceLevel.Error, caller: nameof (FindAllDeleteRenameConflicts));
        }
        else
        {
          this.ConflictEntryIsMapped(origin);
          this.ConflictEntryIsMapped(destination);
          this.targetRenames.Remove(rename);
          this.FoundGitConflict(GitConflictType.DeleteRename, origin.Path, targetPath: destination.Path, baseObjectId: origin.Base, targetObjectId: destination.Target);
        }
      }
    }

    private void FindAllRenameAddConflicts()
    {
      foreach (LibGit2ConflictMapper.Rename rename in this.sourceRenames.Where<LibGit2ConflictMapper.Rename>((Func<LibGit2ConflictMapper.Rename, bool>) (rename => rename.Origin.Target == rename.Origin.Base && rename.Destination.Target != rename.Destination.Source)).ToList<LibGit2ConflictMapper.Rename>())
      {
        LibGit2ConflictMapper.ConflictEntry origin = rename.Origin;
        LibGit2ConflictMapper.ConflictEntry destination = rename.Destination;
        if (origin.Mapped || destination.Mapped)
        {
          TracepointUtils.Tracepoint(this.requestContext, 1013006, GitServerUtils.TraceArea, nameof (LibGit2ConflictMapper), (Func<object>) (() => (object) new
          {
            conflictSourceType = this.conflictSourceType,
            conflictSourceId = this.conflictSourceId,
            mergeBase = this.mergeBase,
            mergeTarget = this.mergeTarget,
            mergeSource = this.mergeSource,
            conflictPath = destination.Path,
            sourcePath = origin.Path
          }), TraceLevel.Error, caller: nameof (FindAllRenameAddConflicts));
        }
        else
        {
          this.ConflictEntryIsMapped(origin);
          this.ConflictEntryIsMapped(destination);
          this.sourceRenames.Remove(rename);
          this.FoundGitConflict(GitConflictType.RenameAdd, destination.Path, origin.Path, baseObjectId: origin.Base, sourceObjectId: destination.Source, targetObjectId: destination.Target);
        }
      }
    }

    private void FindAllAddRenameConflicts()
    {
      foreach (LibGit2ConflictMapper.Rename rename in this.targetRenames.Where<LibGit2ConflictMapper.Rename>((Func<LibGit2ConflictMapper.Rename, bool>) (rename => rename.Origin.Source == rename.Origin.Base && rename.Destination.Source != rename.Destination.Target)).ToList<LibGit2ConflictMapper.Rename>())
      {
        LibGit2ConflictMapper.ConflictEntry origin = rename.Origin;
        LibGit2ConflictMapper.ConflictEntry destination = rename.Destination;
        if (origin.Mapped || destination.Mapped)
        {
          TracepointUtils.Tracepoint(this.requestContext, 1013010, GitServerUtils.TraceArea, nameof (LibGit2ConflictMapper), (Func<object>) (() => (object) new
          {
            conflictSourceType = this.conflictSourceType,
            conflictSourceId = this.conflictSourceId,
            mergeBase = this.mergeBase,
            mergeTarget = this.mergeTarget,
            mergeSource = this.mergeSource,
            conflictPath = origin.Path,
            targetPath = destination.Path
          }), TraceLevel.Error, caller: nameof (FindAllAddRenameConflicts));
        }
        else
        {
          this.ConflictEntryIsMapped(origin);
          this.ConflictEntryIsMapped(destination);
          this.targetRenames.Remove(rename);
          this.FoundGitConflict(GitConflictType.AddRename, destination.Path, targetPath: origin.Path, baseObjectId: origin.Base, sourceObjectId: destination.Source, targetObjectId: destination.Target);
        }
      }
    }

    private void FindAllEditEditConflicts()
    {
      foreach (LibGit2ConflictMapper.ConflictEntry conflictEntry in this.allConflicts.Values.Where<LibGit2ConflictMapper.ConflictEntry>((Func<LibGit2ConflictMapper.ConflictEntry, bool>) (c => c.Base != (ObjectId) null && c.Source != (ObjectId) null && c.Target != (ObjectId) null && c.Base != c.Source && c.Base != c.Target && c.Source != c.Target)).ToList<LibGit2ConflictMapper.ConflictEntry>())
      {
        LibGit2ConflictMapper.ConflictEntry conflict = conflictEntry;
        if (conflict.Mapped)
        {
          TracepointUtils.Tracepoint(this.requestContext, 1013014, GitServerUtils.TraceArea, nameof (LibGit2ConflictMapper), (Func<object>) (() => (object) new
          {
            conflictSourceType = this.conflictSourceType,
            conflictSourceId = this.conflictSourceId,
            mergeBase = this.mergeBase,
            mergeTarget = this.mergeTarget,
            mergeSource = this.mergeSource,
            conflictPath = conflict.Path
          }), TraceLevel.Error, caller: nameof (FindAllEditEditConflicts));
        }
        else
        {
          this.ConflictEntryIsMapped(conflict);
          this.FoundGitConflict(GitConflictType.EditEdit, conflict.Path, baseObjectId: conflict.Base, sourceObjectId: conflict.Source, targetObjectId: conflict.Target);
        }
      }
    }

    private void FindAllEditDeleteConflicts()
    {
      foreach (LibGit2ConflictMapper.ConflictEntry conflictEntry in this.allConflicts.Values.Where<LibGit2ConflictMapper.ConflictEntry>((Func<LibGit2ConflictMapper.ConflictEntry, bool>) (c => c.Base != (ObjectId) null && c.Source != (ObjectId) null && c.Target == (ObjectId) null && c.Base != c.Source)).ToList<LibGit2ConflictMapper.ConflictEntry>())
      {
        LibGit2ConflictMapper.ConflictEntry conflict = conflictEntry;
        if (conflict.Mapped)
        {
          TracepointUtils.Tracepoint(this.requestContext, 1013015, GitServerUtils.TraceArea, nameof (LibGit2ConflictMapper), (Func<object>) (() => (object) new
          {
            conflictSourceType = this.conflictSourceType,
            conflictSourceId = this.conflictSourceId,
            mergeBase = this.mergeBase,
            mergeTarget = this.mergeTarget,
            mergeSource = this.mergeSource,
            conflictPath = conflict.Path
          }), TraceLevel.Error, caller: nameof (FindAllEditDeleteConflicts));
        }
        else
        {
          this.ConflictEntryIsMapped(conflict);
          this.FoundGitConflict(GitConflictType.EditDelete, conflict.Path, baseObjectId: conflict.Base, sourceObjectId: conflict.Source);
        }
      }
    }

    private void FindAllDeleteEditConflicts()
    {
      foreach (LibGit2ConflictMapper.ConflictEntry conflictEntry in this.allConflicts.Values.Where<LibGit2ConflictMapper.ConflictEntry>((Func<LibGit2ConflictMapper.ConflictEntry, bool>) (c => c.Base != (ObjectId) null && c.Source == (ObjectId) null && c.Target != (ObjectId) null && c.Base != c.Target)).ToList<LibGit2ConflictMapper.ConflictEntry>())
      {
        LibGit2ConflictMapper.ConflictEntry conflict = conflictEntry;
        if (conflict.Mapped)
        {
          TracepointUtils.Tracepoint(this.requestContext, 1013023, GitServerUtils.TraceArea, nameof (LibGit2ConflictMapper), (Func<object>) (() => (object) new
          {
            conflictSourceType = this.conflictSourceType,
            conflictSourceId = this.conflictSourceId,
            mergeBase = this.mergeBase,
            mergeTarget = this.mergeTarget,
            mergeSource = this.mergeSource,
            conflictPath = conflict.Path
          }), TraceLevel.Error, caller: nameof (FindAllDeleteEditConflicts));
        }
        else
        {
          this.ConflictEntryIsMapped(conflict);
          this.FoundGitConflict(GitConflictType.DeleteEdit, conflict.Path, baseObjectId: conflict.Base, targetObjectId: conflict.Target);
        }
      }
    }

    private void FindAllAddAddConflicts()
    {
      foreach (LibGit2ConflictMapper.ConflictEntry conflictEntry in this.allConflicts.Values.Where<LibGit2ConflictMapper.ConflictEntry>((Func<LibGit2ConflictMapper.ConflictEntry, bool>) (c => c.Base == (ObjectId) null && c.Source != (ObjectId) null && c.Target != (ObjectId) null && c.Source != c.Target)).ToList<LibGit2ConflictMapper.ConflictEntry>())
      {
        LibGit2ConflictMapper.ConflictEntry conflict = conflictEntry;
        if (conflict.Mapped)
        {
          TracepointUtils.Tracepoint(this.requestContext, 1013023, GitServerUtils.TraceArea, nameof (LibGit2ConflictMapper), (Func<object>) (() => (object) new
          {
            conflictSourceType = this.conflictSourceType,
            conflictSourceId = this.conflictSourceId,
            mergeBase = this.mergeBase,
            mergeTarget = this.mergeTarget,
            mergeSource = this.mergeSource,
            conflictPath = conflict.Path
          }), TraceLevel.Error, caller: nameof (FindAllAddAddConflicts));
        }
        else
        {
          this.ConflictEntryIsMapped(conflict);
          this.FoundGitConflict(GitConflictType.AddAdd, conflict.Path, sourceObjectId: conflict.Source, targetObjectId: conflict.Target);
        }
      }
    }

    private void FindAllTrivialConflicts()
    {
      foreach (LibGit2ConflictMapper.ConflictEntry conflictEntry in this.allConflicts.Values.ToList<LibGit2ConflictMapper.ConflictEntry>())
      {
        if ((conflictEntry.Source == (ObjectId) null || conflictEntry.Base == conflictEntry.Source) && conflictEntry.Base != conflictEntry.Target)
        {
          this.FoundTrivialConflict(conflictEntry.Path, TrivialConflictResolution.TakeTarget);
          this.ConflictEntryIsMapped(conflictEntry);
        }
        else if ((conflictEntry.Target == (ObjectId) null || conflictEntry.Base == conflictEntry.Target) && conflictEntry.Base != conflictEntry.Source)
        {
          this.FoundTrivialConflict(conflictEntry.Path, TrivialConflictResolution.TakeSource);
          this.ConflictEntryIsMapped(conflictEntry);
        }
        else if (conflictEntry.Source == conflictEntry.Target && conflictEntry.Base != conflictEntry.Source)
        {
          if (conflictEntry.Source == (ObjectId) null)
            this.FoundTrivialConflict(conflictEntry.Path, TrivialConflictResolution.Delete);
          else
            this.FoundTrivialConflict(conflictEntry.Path, TrivialConflictResolution.TakeExisting);
          this.ConflictEntryIsMapped(conflictEntry);
        }
      }
    }

    private void BuildRenameLists()
    {
      this.sourceRenames = new List<LibGit2ConflictMapper.Rename>();
      this.targetRenames = new List<LibGit2ConflictMapper.Rename>();
      foreach (KeyValuePair<ObjectId, HashSet<LibGit2ConflictMapper.ConflictEntry>> keyValuePair in this.conflictLookupByFileHash)
      {
        ObjectId key = keyValuePair.Key;
        HashSet<LibGit2ConflictMapper.ConflictEntry> source = keyValuePair.Value;
        LibGit2ConflictMapper.ConflictEntry[] array1 = source.Where<LibGit2ConflictMapper.ConflictEntry>((Func<LibGit2ConflictMapper.ConflictEntry, bool>) (c => c.Base != (ObjectId) null && c.Source == (ObjectId) null)).OrderBy<LibGit2ConflictMapper.ConflictEntry, string>((Func<LibGit2ConflictMapper.ConflictEntry, string>) (c => c.Path), (IComparer<string>) StringComparer.InvariantCultureIgnoreCase).ToArray<LibGit2ConflictMapper.ConflictEntry>();
        LibGit2ConflictMapper.ConflictEntry[] array2 = source.Where<LibGit2ConflictMapper.ConflictEntry>((Func<LibGit2ConflictMapper.ConflictEntry, bool>) (c => c.Base == (ObjectId) null && c.Source != (ObjectId) null)).OrderBy<LibGit2ConflictMapper.ConflictEntry, string>((Func<LibGit2ConflictMapper.ConflictEntry, string>) (c => c.Path), (IComparer<string>) StringComparer.InvariantCultureIgnoreCase).ToArray<LibGit2ConflictMapper.ConflictEntry>();
        LibGit2ConflictMapper.ConflictEntry[] array3 = source.Where<LibGit2ConflictMapper.ConflictEntry>((Func<LibGit2ConflictMapper.ConflictEntry, bool>) (c => c.Base != (ObjectId) null && c.Target == (ObjectId) null)).OrderBy<LibGit2ConflictMapper.ConflictEntry, string>((Func<LibGit2ConflictMapper.ConflictEntry, string>) (c => c.Path), (IComparer<string>) StringComparer.InvariantCultureIgnoreCase).ToArray<LibGit2ConflictMapper.ConflictEntry>();
        LibGit2ConflictMapper.ConflictEntry[] array4 = source.Where<LibGit2ConflictMapper.ConflictEntry>((Func<LibGit2ConflictMapper.ConflictEntry, bool>) (c => c.Base == (ObjectId) null && c.Target != (ObjectId) null)).OrderBy<LibGit2ConflictMapper.ConflictEntry, string>((Func<LibGit2ConflictMapper.ConflictEntry, string>) (c => c.Path), (IComparer<string>) StringComparer.InvariantCultureIgnoreCase).ToArray<LibGit2ConflictMapper.ConflictEntry>();
        HashSet<IndexReucEntry> indexReucEntrySet;
        if (!this.resolvedConflictsByHash.TryGetValue(key, out indexReucEntrySet))
          indexReucEntrySet = new HashSet<IndexReucEntry>();
        HashSet<LibGit2Sharp.IndexEntry> indexEntrySet;
        if (!this.nonConflictFilesByHash.TryGetValue(key, out indexEntrySet))
          indexEntrySet = new HashSet<LibGit2Sharp.IndexEntry>();
        if (this.detectRenameFalsePositives & (array3.Length > 1 || array1.Length > 1 || array2.Length > 1 || array4.Length > 1 || indexReucEntrySet.Count > 0 || indexEntrySet.Count > 0 || source.Count > 3))
        {
          HashSet<string> stringSet = new HashSet<string>();
          foreach (LibGit2Sharp.IndexEntry indexEntry in indexEntrySet)
          {
            this.FoundTrivialConflict(indexEntry.Path, TrivialConflictResolution.TakeExisting);
            stringSet.Add(indexEntry.Path);
          }
          foreach (IndexReucEntry indexReucEntry in indexReucEntrySet)
          {
            if (!stringSet.Contains(indexReucEntry.Path))
            {
              int num1 = indexReucEntry.TheirId != ObjectId.Zero ? 1 : 0;
              bool flag1 = indexReucEntry.OurId != ObjectId.Zero;
              bool flag2 = indexReucEntry.AncestorId != ObjectId.Zero;
              int num2 = flag1 ? 1 : 0;
              if ((num1 & num2) != 0)
                this.FoundTrivialConflict(indexReucEntry.Path, TrivialConflictResolution.TakeExisting);
              else if (flag2)
                this.FoundTrivialConflict(indexReucEntry.Path, TrivialConflictResolution.Delete);
              else
                this.FoundTrivialConflict(indexReucEntry.Path, TrivialConflictResolution.TakeExisting);
            }
          }
        }
        else
        {
          this.sourceRenames.AddRange(((IEnumerable<LibGit2ConflictMapper.ConflictEntry>) array1).Zip<LibGit2ConflictMapper.ConflictEntry, LibGit2ConflictMapper.ConflictEntry, LibGit2ConflictMapper.Rename>((IEnumerable<LibGit2ConflictMapper.ConflictEntry>) array2, (Func<LibGit2ConflictMapper.ConflictEntry, LibGit2ConflictMapper.ConflictEntry, LibGit2ConflictMapper.Rename>) ((origin, destination) => new LibGit2ConflictMapper.Rename(origin, destination))));
          this.targetRenames.AddRange(((IEnumerable<LibGit2ConflictMapper.ConflictEntry>) array3).Zip<LibGit2ConflictMapper.ConflictEntry, LibGit2ConflictMapper.ConflictEntry, LibGit2ConflictMapper.Rename>((IEnumerable<LibGit2ConflictMapper.ConflictEntry>) array4, (Func<LibGit2ConflictMapper.ConflictEntry, LibGit2ConflictMapper.ConflictEntry, LibGit2ConflictMapper.Rename>) ((origin, destination) => new LibGit2ConflictMapper.Rename(origin, destination))));
        }
      }
    }

    private void AddConflictToLookupByFileHash(LibGit2ConflictMapper.ConflictEntry value)
    {
      this.AddConflictToLookupByFileHash(value.Base, value);
      this.AddConflictToLookupByFileHash(value.Source, value);
      this.AddConflictToLookupByFileHash(value.Target, value);
    }

    private void AddConflictToLookupByFileHash(
      ObjectId key,
      LibGit2ConflictMapper.ConflictEntry value)
    {
      LibGit2ConflictMapper.AddToLookup<LibGit2ConflictMapper.ConflictEntry>(this.conflictLookupByFileHash, key, value);
    }

    private void AddResolvedConflictToLookupByFileHash(IndexReucEntry value)
    {
      this.AddOtherChangeToLookupByFileHash(value.AncestorId, value);
      this.AddOtherChangeToLookupByFileHash(value.OurId, value);
      this.AddOtherChangeToLookupByFileHash(value.TheirId, value);
    }

    private void AddOtherChangeToLookupByFileHash(ObjectId key, IndexReucEntry value) => LibGit2ConflictMapper.AddToLookup<IndexReucEntry>(this.resolvedConflictsByHash, key, value);

    private void AddNonConflictFileToLookupByFileHash(LibGit2Sharp.IndexEntry value) => this.AddOtherChangeToLookupByFileHash2(value.Id, value);

    private void AddOtherChangeToLookupByFileHash2(ObjectId key, LibGit2Sharp.IndexEntry value) => LibGit2ConflictMapper.AddToLookup<LibGit2Sharp.IndexEntry>(this.nonConflictFilesByHash, key, value);

    private static void AddToLookup<TValue>(
      Dictionary<ObjectId, HashSet<TValue>> lookup,
      ObjectId key,
      TValue value)
    {
      if (key == (ObjectId) null || key == ObjectId.Zero)
        return;
      HashSet<TValue> objSet;
      lookup.TryGetValue(key, out objSet);
      if (objSet == null)
      {
        objSet = new HashSet<TValue>();
        lookup[key] = objSet;
      }
      objSet.Add(value);
    }

    private void RemoveConflictFromLookupByFileHash(LibGit2ConflictMapper.ConflictEntry value)
    {
      this.RemoveConflictFromLookupByFileHash(value.Base, value);
      this.RemoveConflictFromLookupByFileHash(value.Source, value);
      this.RemoveConflictFromLookupByFileHash(value.Target, value);
    }

    private void RemoveConflictFromLookupByFileHash(
      ObjectId key,
      LibGit2ConflictMapper.ConflictEntry value)
    {
      if (key == (ObjectId) null)
        return;
      HashSet<LibGit2ConflictMapper.ConflictEntry> conflictEntrySet;
      this.conflictLookupByFileHash.TryGetValue(key, out conflictEntrySet);
      conflictEntrySet?.Remove(value);
    }

    private class ConflictEntry
    {
      public string Path;
      public ObjectId Base;
      public ObjectId Source;
      public ObjectId Target;
      public bool Mapped;
    }

    private class Rename : 
      Tuple<LibGit2ConflictMapper.ConflictEntry, LibGit2ConflictMapper.ConflictEntry>
    {
      public Rename(
        LibGit2ConflictMapper.ConflictEntry origin,
        LibGit2ConflictMapper.ConflictEntry destination)
        : base(origin, destination)
      {
      }

      public LibGit2ConflictMapper.ConflictEntry Origin => this.Item1;

      public LibGit2ConflictMapper.ConflictEntry Destination => this.Item2;
    }
  }
}
