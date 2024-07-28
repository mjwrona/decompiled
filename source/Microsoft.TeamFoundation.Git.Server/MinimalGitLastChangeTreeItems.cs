// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.MinimalGitLastChangeTreeItems
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Graph;
using Microsoft.TeamFoundation.Git.Server.Storage;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Redis;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.Git.Server
{
  [Serializable]
  public class MinimalGitLastChangeTreeItems
  {
    public static readonly IValueSerializer ValueSerializer = (IValueSerializer) new MinimalGitLastChangeTreeItems.BinarySerializer();

    public MinimalGitLastChangeTreeItems(
      ITfsGitRepository repo,
      (List<LatestChange> changes, DateTime lastCommitTime, Sha1Id treeId) changeResults)
    {
      Odb odb = GitServerUtils.GetOdb(repo);
      TfsGitTree tfsGitTree = repo.LookupObject<TfsGitTree>(changeResults.treeId);
      Dictionary<string, int> sortedEntries = new Dictionary<string, int>();
      sortedEntries[""] = int.MaxValue;
      int num = 0;
      foreach (TfsGitTreeEntry treeEntry in tfsGitTree.GetTreeEntries())
      {
        sortedEntries[treeEntry.Name] = num;
        ++num;
      }
      this.CommitIntIds = changeResults.changes.OrderBy<LatestChange, int>((Func<LatestChange, int>) (x => sortedEntries[x.ItemName])).Select<LatestChange, int>((Func<LatestChange, int>) (x => odb.ContentDB.Index.ObjectIds.GetIndex<Sha1Id>(x.CommitId))).ToArray<int>();
      this.LastExploredTime = new DateTime?(changeResults.lastCommitTime);
      Sha1Id? objectOrderEpoch = odb.ContentDB.Index.StableObjectOrderEpoch;
      ref Sha1Id? local = ref objectOrderEpoch;
      this.StableObjectOrderEpoch = local.HasValue ? local.GetValueOrDefault().ToByteArray() : (byte[]) null;
    }

    [JsonConstructor]
    public MinimalGitLastChangeTreeItems(
      byte[] stableObjectOrderEpoch,
      DateTime? lastExploredTime,
      int[] commitIntIds)
    {
      this.StableObjectOrderEpoch = stableObjectOrderEpoch;
      this.LastExploredTime = lastExploredTime;
      this.CommitIntIds = commitIntIds;
    }

    public byte[] StableObjectOrderEpoch { get; set; }

    public DateTime? LastExploredTime { get; set; }

    public int[] CommitIntIds { get; set; }

    public (List<GitLastChangeItem> items, DateTime? lastExploredTime) Expand(
      bool includeCommits,
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      Sha1Id commitId,
      string path)
    {
      Odb odb = GitServerUtils.GetOdb(repository);
      Sha1Id sha1Id = new Sha1Id(this.StableObjectOrderEpoch);
      Sha1Id? objectOrderEpoch = odb.ContentDB.Index.StableObjectOrderEpoch;
      if ((objectOrderEpoch.HasValue ? (sha1Id != objectOrderEpoch.GetValueOrDefault() ? 1 : 0) : 1) != 0)
        throw new InvalidOperationException("Last Changes cache entry is not valid for the current state of the repository");
      IGitCommitGraph commitGraph = repository.GetCommitGraph((IEnumerable<Sha1Id>) new Sha1Id[1]
      {
        commitId
      });
      TfsGitTree tfsGitTree = TfsGitDiffHelper.WalkPath(repository.LookupObject<TfsGitTree>(commitGraph.GetRootTreeId(commitGraph.GetLabel(commitId))), new NormalizedGitPath(path)) as TfsGitTree;
      List<GitLastChangeItem> gitLastChangeItemList = new List<GitLastChangeItem>();
      int index = 0;
      foreach (TfsGitTreeEntry treeEntry in tfsGitTree.GetTreeEntries())
      {
        Sha1Id objectId = odb.ObjectSet.ContentDB.Index.ObjectIds[this.CommitIntIds[index]];
        gitLastChangeItemList.Add(new GitLastChangeItem()
        {
          CommitId = objectId.ToString(),
          Path = treeEntry.Name
        });
        ++index;
      }
      if (gitLastChangeItemList.Count == this.CommitIntIds.Length - 1)
        gitLastChangeItemList.Add(new GitLastChangeItem()
        {
          Path = "",
          CommitId = odb.ObjectSet.ContentDB.Index.ObjectIds[this.CommitIntIds[this.CommitIntIds.Length - 1]].ToString()
        });
      return (gitLastChangeItemList, this.LastExploredTime);
    }

    public static string GetCacheSubKey(ITfsGitRepository repository) => GitServerUtils.GetOdb(repository).ContentDB.Index.StableObjectOrderEpoch.Value.ToString();

    private class BinarySerializer : IValueSerializer
    {
      byte[] IValueSerializer.Serialize<T>(T value) => Encoding.UTF8.GetBytes(JsonConvert.SerializeObject((object) value));

      T IValueSerializer.Deserialize<T>(byte[] data)
      {
        JsonSerializerSettings settings = new JsonSerializerSettings()
        {
          TypeNameHandling = TypeNameHandling.None
        };
        return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(data), settings);
      }
    }
  }
}
