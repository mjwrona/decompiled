// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.RenameDetector
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class RenameDetector
  {
    public const int MaxDeletesPerAdd = 4;
    private const int c_maxChangesForRenameDetection = 1000;
    private const double c_minimumSimilarityScoreForRename = 0.5;
    private readonly IGitObjectSet m_objectSet;
    private readonly IObjectMetadata m_objectMetadata;
    private readonly int m_maxRenameDetectionFileSize;

    public RenameDetector(
      IGitObjectSet objectSet,
      IObjectMetadata objectMetadata,
      int maxRenameDetectionFileSize)
    {
      this.m_objectSet = objectSet;
      this.m_objectMetadata = objectMetadata;
      this.m_maxRenameDetectionFileSize = maxRenameDetectionFileSize;
    }

    public IList<TfsGitDiffEntry> Detect(IList<TfsGitDiffEntry> entries)
    {
      HashSet<TfsGitDiffEntry> tfsGitDiffEntrySet1 = new HashSet<TfsGitDiffEntry>();
      HashSet<TfsGitDiffEntry> tfsGitDiffEntrySet2 = new HashSet<TfsGitDiffEntry>();
      HashSet<TfsGitDiffEntry> tfsGitDiffEntrySet3 = new HashSet<TfsGitDiffEntry>();
      HashSet<TfsGitDiffEntry> renames1 = new HashSet<TfsGitDiffEntry>();
      HashSet<Sha1Id> objectIds = new HashSet<Sha1Id>();
      Dictionary<Sha1Id, Queue<TfsGitDiffEntry>> dictionary = new Dictionary<Sha1Id, Queue<TfsGitDiffEntry>>();
      foreach (TfsGitDiffEntry entry in (IEnumerable<TfsGitDiffEntry>) entries)
      {
        tfsGitDiffEntrySet1.Add(entry);
        Sha1Id? nullable;
        if (entry.ChangeType == TfsGitChangeType.Delete)
        {
          tfsGitDiffEntrySet2.Add(entry);
          HashSet<Sha1Id> sha1IdSet = objectIds;
          nullable = entry.OldObjectId;
          Sha1Id sha1Id = nullable.Value;
          sha1IdSet.Add(sha1Id);
          nullable = entry.OldObjectId;
          Sha1Id key = nullable.Value;
          Queue<TfsGitDiffEntry> tfsGitDiffEntryQueue;
          if (!dictionary.TryGetValue(key, out tfsGitDiffEntryQueue))
            dictionary.Add(key, tfsGitDiffEntryQueue = new Queue<TfsGitDiffEntry>(1));
          tfsGitDiffEntryQueue.Enqueue(entry);
        }
        else if (entry.ChangeType == TfsGitChangeType.Add)
        {
          tfsGitDiffEntrySet3.Add(entry);
          HashSet<Sha1Id> sha1IdSet = objectIds;
          nullable = entry.NewObjectId;
          Sha1Id sha1Id = nullable.Value;
          sha1IdSet.Add(sha1Id);
        }
      }
      if (!tfsGitDiffEntrySet2.Any<TfsGitDiffEntry>() || !tfsGitDiffEntrySet3.Any<TfsGitDiffEntry>())
        return entries;
      foreach (TfsGitDiffEntry matchingAdd in tfsGitDiffEntrySet3.ToArray<TfsGitDiffEntry>())
      {
        Queue<TfsGitDiffEntry> tfsGitDiffEntryQueue;
        if (dictionary.TryGetValue(matchingAdd.NewObjectId.Value, out tfsGitDiffEntryQueue) && tfsGitDiffEntryQueue.Count > 0)
        {
          TfsGitDiffEntry matchingDelete = tfsGitDiffEntryQueue.Dequeue();
          if (matchingAdd.NewObjectType != matchingDelete.OldObjectType)
            throw new InvalidOperationException("add.NewObjectType != exactDeleteMatch.OldObjectType");
          RenameDetector.ProcessMatchingAddDelete(tfsGitDiffEntrySet1, tfsGitDiffEntrySet2, tfsGitDiffEntrySet3, renames1, matchingDelete, matchingAdd);
        }
      }
      if (tfsGitDiffEntrySet2.Count + tfsGitDiffEntrySet3.Count <= 1000 && tfsGitDiffEntrySet2.Any<TfsGitDiffEntry>((Func<TfsGitDiffEntry, bool>) (delete => delete.OldObjectType == GitObjectType.Blob)) && tfsGitDiffEntrySet3.Any<TfsGitDiffEntry>((Func<TfsGitDiffEntry, bool>) (add => add.NewObjectType == GitObjectType.Blob)))
      {
        HashSet<Sha1Id> sha1IdSet1 = new HashSet<Sha1Id>();
        foreach (ObjectIdAndSize objectSiz in this.m_objectMetadata.GetObjectSizes((IEnumerable<Sha1Id>) objectIds, continueOnError: true))
        {
          if (objectSiz.Size > (long) this.m_maxRenameDetectionFileSize)
            sha1IdSet1.Add(objectSiz.Id);
        }
        List<RenameDetector.FileContentInfo> fileContentInfoList1 = new List<RenameDetector.FileContentInfo>();
        foreach (TfsGitDiffEntry tfsGitDiffEntry in tfsGitDiffEntrySet2.Where<TfsGitDiffEntry>((Func<TfsGitDiffEntry, bool>) (delete => delete.OldObjectType == GitObjectType.Blob)))
        {
          HashSet<Sha1Id> sha1IdSet2 = sha1IdSet1;
          Sha1Id? oldObjectId = tfsGitDiffEntry.OldObjectId;
          Sha1Id sha1Id = oldObjectId.Value;
          if (!sha1IdSet2.Contains(sha1Id))
          {
            List<RenameDetector.FileContentInfo> fileContentInfoList2 = fileContentInfoList1;
            IGitObjectSet objectSet = this.m_objectSet;
            TfsGitDiffEntry diffEntry = tfsGitDiffEntry;
            oldObjectId = tfsGitDiffEntry.OldObjectId;
            Sha1Id objectId = oldObjectId.Value;
            RenameDetector.FileContentInfo fileContentInfo = RenameDetector.FileContentInfo.Parse(objectSet, diffEntry, objectId);
            fileContentInfoList2.Add(fileContentInfo);
          }
        }
        List<RenameDetector.Match> matchList = new List<RenameDetector.Match>();
        List<RenameDetector.Match> collection = new List<RenameDetector.Match>(4);
        foreach (TfsGitDiffEntry tfsGitDiffEntry in tfsGitDiffEntrySet3.Where<TfsGitDiffEntry>((Func<TfsGitDiffEntry, bool>) (add => add.NewObjectType == GitObjectType.Blob)))
        {
          HashSet<Sha1Id> sha1IdSet3 = sha1IdSet1;
          Sha1Id? newObjectId = tfsGitDiffEntry.NewObjectId;
          Sha1Id sha1Id = newObjectId.Value;
          if (!sha1IdSet3.Contains(sha1Id))
          {
            collection.Clear();
            IGitObjectSet objectSet = this.m_objectSet;
            TfsGitDiffEntry diffEntry = tfsGitDiffEntry;
            newObjectId = tfsGitDiffEntry.NewObjectId;
            Sha1Id objectId = newObjectId.Value;
            RenameDetector.FileContentInfo file1 = RenameDetector.FileContentInfo.Parse(objectSet, diffEntry, objectId);
            foreach (RenameDetector.FileContentInfo file2 in fileContentInfoList1)
            {
              double minimumScore = collection.Count < 4 ? 0.5 : collection[0].MatchPct;
              double similarityScore = RenameDetector.FileContentInfo.GetSimilarityScore(file1, file2, minimumScore);
              if (similarityScore > minimumScore)
              {
                RenameDetector.Match match = new RenameDetector.Match(file1.DiffEntry, file2.DiffEntry, similarityScore);
                if (collection.Count < 4)
                  collection.Add(match);
                else
                  collection[0] = match;
                collection.Sort();
              }
            }
            matchList.AddRange((IEnumerable<RenameDetector.Match>) collection);
          }
        }
        matchList.Sort();
        for (int index = matchList.Count - 1; index >= 0; --index)
        {
          RenameDetector.Match match = matchList[index];
          if (tfsGitDiffEntrySet2.Contains(match.Delete) && tfsGitDiffEntrySet3.Contains(match.Add))
            RenameDetector.ProcessMatchingAddDelete(tfsGitDiffEntrySet1, tfsGitDiffEntrySet2, tfsGitDiffEntrySet3, renames1, match.Delete, match.Add);
        }
      }
      if (tfsGitDiffEntrySet2.Count > 0 && tfsGitDiffEntrySet3.Count > 0)
      {
        HashSet<TfsGitDiffEntry> tfsGitDiffEntrySet4 = new HashSet<TfsGitDiffEntry>(tfsGitDiffEntrySet3.Where<TfsGitDiffEntry>((Func<TfsGitDiffEntry, bool>) (add => add.NewObjectType == GitObjectType.Tree)));
        HashSet<TfsGitDiffEntry> tfsGitDiffEntrySet5 = new HashSet<TfsGitDiffEntry>(tfsGitDiffEntrySet2.Where<TfsGitDiffEntry>((Func<TfsGitDiffEntry, bool>) (delete => delete.OldObjectType == GitObjectType.Tree)));
        if (tfsGitDiffEntrySet4.Count > 0 && tfsGitDiffEntrySet5.Count > 0 && tfsGitDiffEntrySet5.Count + tfsGitDiffEntrySet4.Count <= 1000)
        {
          List<RenameDetector.RenameItemMetadata> renames2 = new List<RenameDetector.RenameItemMetadata>();
          foreach (TfsGitDiffEntry tfsGitDiffEntry in renames1)
            renames2.Add(new RenameDetector.RenameItemMetadata()
            {
              RenameSourceItemFolder = RenameDetector.GetParentFolder(tfsGitDiffEntry.RenameSourceItemPath),
              RenameItemFolder = RenameDetector.GetParentFolder(tfsGitDiffEntry.RelativePath)
            });
          foreach (TfsGitDiffEntry tfsGitDiffEntry in tfsGitDiffEntrySet5)
          {
            int num = 0;
            TfsGitDiffEntry matchingAdd = (TfsGitDiffEntry) null;
            foreach (TfsGitDiffEntry addedFolder in tfsGitDiffEntrySet4)
            {
              int itemsBetweenFolders = RenameDetector.GetNumMovedItemsBetweenFolders(tfsGitDiffEntry, addedFolder, renames2);
              if (itemsBetweenFolders > num)
              {
                num = itemsBetweenFolders;
                matchingAdd = addedFolder;
              }
            }
            if (matchingAdd != null)
            {
              RenameDetector.ProcessMatchingAddDelete(tfsGitDiffEntrySet1, tfsGitDiffEntrySet2, tfsGitDiffEntrySet3, renames1, tfsGitDiffEntry, matchingAdd);
              renames2.Add(new RenameDetector.RenameItemMetadata()
              {
                RenameSourceItemFolder = RenameDetector.GetParentFolder(tfsGitDiffEntry.RelativePath),
                RenameItemFolder = RenameDetector.GetParentFolder(matchingAdd.RelativePath)
              });
              tfsGitDiffEntrySet4.Remove(matchingAdd);
            }
          }
        }
      }
      return (IList<TfsGitDiffEntry>) tfsGitDiffEntrySet1.ToList<TfsGitDiffEntry>();
    }

    private static void ProcessMatchingAddDelete(
      HashSet<TfsGitDiffEntry> entries,
      HashSet<TfsGitDiffEntry> deleteEntries,
      HashSet<TfsGitDiffEntry> addEntries,
      HashSet<TfsGitDiffEntry> renames,
      TfsGitDiffEntry matchingDelete,
      TfsGitDiffEntry matchingAdd)
    {
      entries.Remove(matchingDelete);
      entries.Remove(matchingAdd);
      deleteEntries.Remove(matchingDelete);
      addEntries.Remove(matchingAdd);
      TfsGitDiffEntry renameEntry = TfsGitDiffEntry.CreateRenameEntry(matchingDelete, matchingAdd);
      renames.Add(renameEntry);
      entries.Add(renameEntry);
      TfsGitDiffEntry renameSourceEntry = TfsGitDiffEntry.CreateRenameSourceEntry(matchingDelete);
      entries.Add(renameSourceEntry);
    }

    private static int GetNumMovedItemsBetweenFolders(
      TfsGitDiffEntry deletedFolder,
      TfsGitDiffEntry addedFolder,
      List<RenameDetector.RenameItemMetadata> renames)
    {
      int itemsBetweenFolders = 0;
      foreach (RenameDetector.RenameItemMetadata rename in renames)
      {
        if (string.Equals(deletedFolder.RelativePath, rename.RenameSourceItemFolder, StringComparison.Ordinal) && string.Equals(addedFolder.RelativePath, rename.RenameItemFolder, StringComparison.Ordinal))
          ++itemsBetweenFolders;
      }
      return itemsBetweenFolders;
    }

    private static string GetParentFolder(string path)
    {
      int length = path.LastIndexOf('/');
      return length >= 0 ? path.Substring(0, length) : "/";
    }

    private struct Match : IComparable<RenameDetector.Match>
    {
      public Match(TfsGitDiffEntry add, TfsGitDiffEntry delete, double matchPct)
      {
        this.Add = add;
        this.Delete = delete;
        this.MatchPct = matchPct;
      }

      public TfsGitDiffEntry Add { get; }

      public TfsGitDiffEntry Delete { get; }

      public double MatchPct { get; }

      public int CompareTo(RenameDetector.Match other) => this.MatchPct.CompareTo(other.MatchPct);
    }

    private class FileContentChunk
    {
      public int ChunkHash { get; set; }

      public int ChunkLength { get; set; }

      public int Occurrences { get; set; }
    }

    private class FileContentInfo
    {
      private const int c_chunkLength = 64;
      private const int c_maxChunksPerFile = 10000;

      public TfsGitDiffEntry DiffEntry { get; private set; }

      public long FileLength { get; private set; }

      public Dictionary<int, RenameDetector.FileContentChunk> ChunksByHash { get; private set; }

      public static RenameDetector.FileContentInfo Parse(
        IGitObjectSet objectSet,
        TfsGitDiffEntry diffEntry,
        Sha1Id objectId)
      {
        RenameDetector.FileContentInfo fileContentInfo = new RenameDetector.FileContentInfo();
        fileContentInfo.DiffEntry = diffEntry;
        using (Stream contentAndCheckType = objectSet.GetContentAndCheckType(objectId, GitObjectType.Blob))
          fileContentInfo.ParseChunks(contentAndCheckType);
        return fileContentInfo;
      }

      public static double GetSimilarityScore(
        RenameDetector.FileContentInfo file1,
        RenameDetector.FileContentInfo file2,
        double minimumScore)
      {
        long num1 = Math.Min(file1.FileLength, file2.FileLength);
        long num2 = Math.Max(file1.FileLength, file2.FileLength);
        if ((double) num1 / (double) num2 < minimumScore)
          return 0.0;
        long num3 = 0;
        foreach (RenameDetector.FileContentChunk fileContentChunk1 in file1.ChunksByHash.Values)
        {
          RenameDetector.FileContentChunk fileContentChunk2;
          if (file2.ChunksByHash.TryGetValue(fileContentChunk1.ChunkHash, out fileContentChunk2))
            num3 += (long) (fileContentChunk1.ChunkLength * Math.Min(fileContentChunk1.Occurrences, fileContentChunk2.Occurrences));
        }
        return (double) num3 / (double) num2;
      }

      private void ParseChunks(Stream stream)
      {
        this.ChunksByHash = new Dictionary<int, RenameDetector.FileContentChunk>();
        this.FileLength = 0L;
        int num1 = 0;
        using (StreamReader streamReader = new StreamReader(stream))
        {
          bool flag = false;
          int num2 = 0;
          char[] chArray = new char[2048];
          while (!flag)
          {
            int bufferLength = num2 + streamReader.Read(chArray, num2, chArray.Length - num2);
            flag = bufferLength == num2;
            num2 = 0;
            int num3 = flag ? bufferLength - 1 : bufferLength - 64 - 2;
            int bufferIndex = 0;
            while (bufferIndex <= num3)
            {
              int chunkLength;
              int nextChunk = this.GetNextChunk(chArray, bufferLength, ref bufferIndex, out chunkLength);
              this.FileLength += (long) chunkLength;
              RenameDetector.FileContentChunk fileContentChunk;
              if (!this.ChunksByHash.TryGetValue(nextChunk, out fileContentChunk))
              {
                fileContentChunk = new RenameDetector.FileContentChunk()
                {
                  ChunkHash = nextChunk,
                  ChunkLength = chunkLength,
                  Occurrences = 1
                };
                this.ChunksByHash.Add(nextChunk, fileContentChunk);
              }
              else
                ++fileContentChunk.Occurrences;
              if (++num1 >= 10000)
                return;
            }
            if (bufferIndex < bufferLength)
            {
              num2 = bufferLength - bufferIndex;
              Array.Copy((Array) chArray, bufferIndex, (Array) chArray, 0, num2);
            }
          }
        }
      }

      private int GetNextChunk(
        char[] buffer,
        int bufferLength,
        ref int bufferIndex,
        out int chunkLength)
      {
        chunkLength = 0;
        long num1 = 0;
        long num2 = 0;
        while (chunkLength++ < 64 && bufferIndex < bufferLength)
        {
          char ch = buffer[bufferIndex++];
          if (ch == '\r' && bufferIndex < buffer.Length && buffer[bufferIndex] == '\n')
          {
            ch = '\n';
            ++bufferIndex;
          }
          else if (chunkLength == 0 && (ch == ' ' || ch == '\t'))
            continue;
          long num3 = num1;
          long num4 = num1 << 7 ^ num2 >> 25;
          num2 = num4 << 7 ^ num3 >> 25;
          num1 = num4 + (long) ch;
          if (ch == '\r' || ch == '\n')
            break;
        }
        return (int) ((num1 + num2 * 97L) % 107927L);
      }
    }

    private class RenameItemMetadata
    {
      public string RenameSourceItemFolder { get; set; }

      public string RenameItemFolder { get; set; }
    }
  }
}
