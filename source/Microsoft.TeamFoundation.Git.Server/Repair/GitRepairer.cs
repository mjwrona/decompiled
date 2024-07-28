// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Repair.GitRepairer
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.TeamFoundation.Git.Server.Repair
{
  internal class GitRepairer
  {
    private readonly IVssRequestContext m_rc;
    private readonly Odb m_odb;
    private readonly ConcatGitPackIndex m_indexToRepair;
    private readonly byte[] m_buffer;
    private List<KeyValuePair<string, KnownFile>> m_sortedIndexes;
    private readonly ITfsGitBlobProvider m_blobPrv;
    private readonly IGitDataFileProvider m_dataFilePrv;
    private readonly IGitKnownFilesProvider m_knownFilePrv;
    private readonly GitPackIndexLoader m_packIndexLoader;
    private readonly IGitPackIndexPointerProvider m_packIndexPtrPrv;
    private readonly Func<GitPackIndexTransaction> m_packIndexTranFactory;
    private const string c_layer = "GitRepairer";
    private const int c_tracepoint = 1013766;

    public GitRepairer(
      IVssRequestContext rc,
      Odb odb,
      ConcatGitPackIndex indexToRepair,
      ITfsGitBlobProvider blobProvider,
      IGitDataFileProvider dataFilePrv,
      IGitKnownFilesProvider knownFilePrv,
      GitPackIndexLoader packIndexLoader,
      IGitPackIndexPointerProvider packIndexPointerProvider,
      Func<GitPackIndexTransaction> packIndexTranFactory)
    {
      this.m_rc = rc;
      this.m_odb = odb;
      this.m_indexToRepair = indexToRepair;
      this.m_blobPrv = blobProvider;
      this.m_dataFilePrv = dataFilePrv;
      this.m_knownFilePrv = knownFilePrv;
      this.m_packIndexLoader = packIndexLoader;
      this.m_packIndexPtrPrv = packIndexPointerProvider;
      this.m_packIndexTranFactory = packIndexTranFactory;
      this.m_buffer = new byte[GitStreamUtil.OptimalBufferSize];
    }

    public bool TryRepair()
    {
      List<Sha1Id> badObjects = this.FindBadObjects();
      if (badObjects.Count == 0)
      {
        this.m_rc.TraceAlways(1013766, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (GitRepairer), "No objects to repair");
        return true;
      }
      this.LoadIndexes();
      Dictionary<Sha1Id, GitRepairer.GoodObjectLocation> goodCopies = new Dictionary<Sha1Id, GitRepairer.GoodObjectLocation>();
      foreach (Sha1Id sha1Id in badObjects)
      {
        GitRepairer.GoodObjectLocation location;
        if (this.TryFindGoodCopy(sha1Id, out location))
          goodCopies[sha1Id] = location;
      }
      this.m_rc.TraceAlways(1013766, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (GitRepairer), "Found good copies for {0} of {1} objects", (object) goodCopies.Count, (object) badObjects.Count);
      this.UpdateIndexWithNewPackfiles(goodCopies);
      return goodCopies.Count == badObjects.Count;
    }

    private List<Sha1Id> FindBadObjects()
    {
      List<Sha1Id> badObjects = new List<Sha1Id>();
      List<string> stringList = new List<string>();
      foreach (Sha1Id objectId in (IEnumerable<Sha1Id>) this.m_indexToRepair.ObjectIds)
      {
        using (Stream rawContent = this.m_odb.ContentDB.GetRawContent(this.m_indexToRepair.LookupObject(objectId).Location))
        {
          string errorMessage;
          if (!this.IsValidFullObjectOrDelta(rawContent, objectId, out errorMessage))
          {
            stringList.Add(errorMessage);
            badObjects.Add(objectId);
          }
        }
      }
      if (badObjects.Count > 0)
      {
        StringBuilder stringBuilder = new StringBuilder();
        for (int index = 0; index < badObjects.Count; ++index)
          stringBuilder.AppendLine(string.Format("{0} : {1}", (object) badObjects[index], (object) stringList[index]));
        this.m_rc.TraceAlways(1013766, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (GitRepairer), "Found {0} objects to repair.\n {1}", (object) badObjects.Count, (object) stringBuilder);
      }
      return badObjects;
    }

    private bool IsValidFullObjectOrDelta(
      Stream rawContent,
      Sha1Id expectedObjectId,
      out string errorMessage)
    {
      try
      {
        GitPackObjectType type;
        long uncompressedSize;
        GitServerUtils.ReadPackEntryHeader(rawContent, out type, out uncompressedSize);
        using (Stream inflateStream = GitServerUtils.CreateInflateStream(rawContent, uncompressedLength: uncompressedSize))
        {
          if (type == GitPackObjectType.RefDelta || type == GitPackObjectType.OfsDelta)
          {
            Sha1Id.FromStream(rawContent);
            GitStreamUtil.EnsureDrained(inflateStream, this.m_buffer);
          }
          else
          {
            Sha1Id sha1Id;
            using (HashingStream<SHA1Cng> hashingStream = new HashingStream<SHA1Cng>(inflateStream, FileAccess.Read))
            {
              byte[] objectHeader = GitServerUtils.CreateObjectHeader(type, uncompressedSize);
              hashingStream.AddToHash(objectHeader, 0, objectHeader.Length);
              GitStreamUtil.EnsureDrained((Stream) hashingStream, this.m_buffer);
              sha1Id = new Sha1Id(hashingStream.Hash);
            }
            if (sha1Id != expectedObjectId)
            {
              errorMessage = string.Format("{0}'s data results in a different sha1: {1}.", (object) expectedObjectId, (object) sha1Id);
              return false;
            }
          }
        }
      }
      catch (Exception ex)
      {
        errorMessage = ex.Message;
        return false;
      }
      errorMessage = (string) null;
      return true;
    }

    private void LoadIndexes()
    {
      IReadOnlyDictionary<string, KnownFile> readOnlyDictionary = this.m_knownFilePrv.Read();
      this.m_sortedIndexes = new List<KeyValuePair<string, KnownFile>>();
      foreach (KeyValuePair<string, KnownFile> keyValuePair in (IEnumerable<KeyValuePair<string, KnownFile>>) readOnlyDictionary)
      {
        if (keyValuePair.Value.Type == KnownFileType.Index)
          this.m_sortedIndexes.Add(keyValuePair);
      }
      this.m_sortedIndexes.Sort((Comparison<KeyValuePair<string, KnownFile>>) ((i1, i2) => DateTime.Compare(i1.Value.CreatedDate, i2.Value.CreatedDate)));
    }

    private bool TryFindGoodCopy(Sha1Id objectId, out GitRepairer.GoodObjectLocation location)
    {
      GitPackIndexEntry gitPackIndexEntry = this.m_indexToRepair.LookupObject(objectId);
      Sha1Id packId1 = this.m_indexToRepair.PackIds[(int) gitPackIndexEntry.Location.PackIntId];
      for (int index1 = this.m_sortedIndexes.Count - 1; index1 >= 0; --index1)
      {
        using (ConcatGitPackIndex index2 = this.m_packIndexLoader.LoadIndex(new Sha1Id?(Sha1Id.Parse(this.m_sortedIndexes[index1].Key.Substring(0, 40)))))
        {
          GitPackIndexEntry entry;
          if (index2.TryLookupObject(objectId, out entry))
          {
            Sha1Id packId2 = index2.PackIds[(int) entry.Location.PackIntId];
            if (!(packId2 == packId1))
            {
              if (this.IsValidEntry(packId2, entry, objectId))
              {
                location = new GitRepairer.GoodObjectLocation(packId2, entry.Location.Offset, entry.Location.Length);
                return true;
              }
            }
          }
        }
      }
      this.m_rc.TraceAlways(1013766, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (GitRepairer), "Unable to find good copy of object {0} with offset {1} and length {2} in pack {3}.", (object) objectId, (object) gitPackIndexEntry.Location.Offset, (object) gitPackIndexEntry.Location.Length, (object) packId1);
      location = (GitRepairer.GoodObjectLocation) null;
      return false;
    }

    private bool IsValidEntry(Sha1Id packId, GitPackIndexEntry entry, Sha1Id expectedObjectId)
    {
      try
      {
        using (Stream stream = this.m_dataFilePrv.GetStream(StorageUtils.GetPackFileName(packId), entry.Location.Offset, entry.Location.Length))
        {
          string errorMessage;
          if (!this.IsValidFullObjectOrDelta(stream, expectedObjectId, out errorMessage))
          {
            this.m_rc.TraceAlways(1013766, TraceLevel.Error, GitServerUtils.TraceArea, nameof (GitRepairer), errorMessage);
            return false;
          }
        }
        return true;
      }
      catch (Exception ex)
      {
        this.m_rc.TraceException(1013766, GitServerUtils.TraceArea, nameof (GitRepairer), ex);
        return false;
      }
    }

    private void UpdateIndexWithNewPackfiles(
      Dictionary<Sha1Id, GitRepairer.GoodObjectLocation> goodCopies)
    {
      GitPackIndexTransaction tran = this.m_packIndexTranFactory();
      try
      {
        GitPackIndexer indexer = new GitPackIndexer();
        indexer.SetBaseIndex(this.m_indexToRepair.GetRange(0, 0));
        List<GitRepacker.GitPack> curPacksLayout = GitRepacker.CollectCurrentPacksLayout((IGitPackIndex) this.m_indexToRepair, out List<ObjectIdAndGitPackIndexEntry> _);
        this.ApplyPackingResults(tran.KnownFilesBuilder, indexer, (IGitPackIndex) this.m_indexToRepair, curPacksLayout, goodCopies);
        indexer.PreserveStableObjectOrderIfCompatible((IGitPackIndex) this.m_indexToRepair);
        Sha1Id optimisticIndexId = GitDataFileUtil.WriteIndex(this.m_rc, this.m_blobPrv, this.m_odb.Id, tran.KnownFilesBuilder, indexer, this.m_odb.GetAggressivePackIndexMergeStrategy());
        this.MergeConflictingIndexesAndUpdatePointer(tran, optimisticIndexId);
      }
      finally
      {
        tran.TryExpirePendingExtantAndDispose();
      }
    }

    private List<GitRepacker.PackingResult> ComputePackingResults(
      List<GitRepacker.GitPack> curPacksLayout,
      Dictionary<Sha1Id, GitRepairer.GoodObjectLocation> goodCopies)
    {
      List<GitRepacker.PackingResult> packingResults = new List<GitRepacker.PackingResult>(curPacksLayout.Count);
      foreach (GitRepacker.GitPack gitPack in curPacksLayout)
      {
        bool flag = false;
        foreach (ObjectIdAndGitPackIndexEntry gitPackIndexEntry in gitPack)
        {
          if (goodCopies.ContainsKey(gitPackIndexEntry.ObjectId))
          {
            flag = true;
            break;
          }
        }
        if (flag)
          packingResults.Add(new GitRepacker.PackingResult()
          {
            Pack = gitPack,
            Action = GitRepacker.GitPackingAction.CopyEntriesAndContent
          });
        else
          packingResults.Add(new GitRepacker.PackingResult()
          {
            Pack = gitPack,
            Action = GitRepacker.GitPackingAction.CopyEntriesOnly
          });
      }
      return packingResults;
    }

    private void ApplyPackingResults(
      GitKnownFilesBuilder knownFiles,
      GitPackIndexer indexer,
      IGitPackIndex subidx,
      List<GitRepacker.GitPack> curPacksLayout,
      Dictionary<Sha1Id, GitRepairer.GoodObjectLocation> goodCopies)
    {
      foreach (GitRepacker.GitPack pack in curPacksLayout)
      {
        bool flag = false;
        foreach (ObjectIdAndGitPackIndexEntry gitPackIndexEntry in pack)
        {
          if (goodCopies.ContainsKey(gitPackIndexEntry.ObjectId))
          {
            flag = true;
            break;
          }
        }
        if (flag)
          this.CopyIndexEntriesAndObjectsContentForRepairedPackFile(knownFiles, indexer, subidx, pack, goodCopies);
        else
          GitRepacker.CopyIndexEntriesOnlyForUntouchedPackFile(indexer, subidx, pack);
      }
    }

    private void CopyIndexEntriesAndObjectsContentForRepairedPackFile(
      GitKnownFilesBuilder knownFiles,
      GitPackIndexer indexer,
      IGitPackIndex subidx,
      GitRepacker.GitPack pack,
      Dictionary<Sha1Id, GitRepairer.GoodObjectLocation> goodCopies)
    {
      Sha1Id uniqueId = StorageUtils.CreateUniqueId();
      string packFileName = StorageUtils.GetPackFileName(uniqueId);
      indexer.BeginPackFile(new Sha1Id?(uniqueId), GitPackStates.Derived);
      using (BlobProviderChunkingWriterStream chunkingWriterStream = new BlobProviderChunkingWriterStream(this.m_rc, this.m_blobPrv, this.m_odb.Id, packFileName))
      {
        using (WriteBufferStream writeBufferStream = new WriteBufferStream((Stream) chunkingWriterStream, 4194304))
        {
          using (GitPackSerializer serializer = new GitPackSerializer((Stream) writeBufferStream, pack.Count, true))
          {
            long packHeaderLength = (long) GitPackSerializer.PackHeaderLength;
            long currentContiguousOffset = -1;
            long currentContiguousLength = 0;
            int objectCount = 0;
            ushort packIntId = ushort.MaxValue;
            foreach (ObjectIdAndGitPackIndexEntry gitPackIndexEntry in pack)
            {
              GitRepairer.GoodObjectLocation location;
              if (goodCopies.TryGetValue(gitPackIndexEntry.ObjectId, out location))
              {
                if (objectCount > 0)
                {
                  GitRepacker.SerializeContiguousArea(this.m_dataFilePrv, serializer, currentContiguousOffset, currentContiguousLength, objectCount, packIntId, subidx);
                  packIntId = gitPackIndexEntry.Location.PackIntId;
                  currentContiguousOffset = -1L;
                  currentContiguousLength = -1L;
                  objectCount = 0;
                }
                long length = this.SerializeReplacementObject(serializer, location);
                indexer.AddObject(gitPackIndexEntry.ObjectId, gitPackIndexEntry.ObjectType, packHeaderLength, length);
                packHeaderLength += length;
              }
              else
              {
                indexer.AddObject(gitPackIndexEntry.ObjectId, gitPackIndexEntry.ObjectType, packHeaderLength, gitPackIndexEntry.Location.Length);
                packHeaderLength += gitPackIndexEntry.Location.Length;
                if ((int) packIntId == (int) gitPackIndexEntry.Location.PackIntId && currentContiguousOffset + currentContiguousLength == gitPackIndexEntry.Location.Offset)
                {
                  currentContiguousLength += gitPackIndexEntry.Location.Length;
                  ++objectCount;
                }
                else
                {
                  if (objectCount > 0)
                    GitRepacker.SerializeContiguousArea(this.m_dataFilePrv, serializer, currentContiguousOffset, currentContiguousLength, objectCount, packIntId, subidx);
                  packIntId = gitPackIndexEntry.Location.PackIntId;
                  currentContiguousOffset = gitPackIndexEntry.Location.Offset;
                  currentContiguousLength = gitPackIndexEntry.Location.Length;
                  objectCount = 1;
                }
              }
            }
            if (objectCount > 0)
              GitRepacker.SerializeContiguousArea(this.m_dataFilePrv, serializer, currentContiguousOffset, currentContiguousLength, objectCount, packIntId, subidx);
            serializer.Complete();
          }
        }
      }
      indexer.EndPackFile(uniqueId);
      knownFiles.QueueExtant(packFileName, KnownFileType.DerivedPackFile);
    }

    private long SerializeReplacementObject(
      GitPackSerializer serializer,
      GitRepairer.GoodObjectLocation location)
    {
      serializer.AddMultipleRaw(this.m_dataFilePrv.GetStream(StorageUtils.GetPackFileName(location.PackId), location.Offset, location.Length), 0L, location.Length, 1);
      return location.Length;
    }

    private void MergeConflictingIndexesAndUpdatePointer(
      GitPackIndexTransaction tran,
      Sha1Id optimisticIndexId)
    {
      using (ConcatGitPackIndex optimisticIndex = this.m_packIndexLoader.LoadIndex(new Sha1Id?(optimisticIndexId)))
        GitRepacker.INTERNAL_MergeConflictingIndexesAndUpdatePointer(this.m_rc, this.m_blobPrv, this.m_odb, this.m_packIndexLoader, this.m_packIndexPtrPrv, tran, this.m_indexToRepair, optimisticIndex, false);
    }

    private class GoodObjectLocation
    {
      public GoodObjectLocation(Sha1Id packId, long offset, long length)
      {
        this.PackId = packId;
        this.Offset = offset;
        this.Length = length;
      }

      public Sha1Id PackId { get; }

      public long Offset { get; }

      public long Length { get; }
    }
  }
}
