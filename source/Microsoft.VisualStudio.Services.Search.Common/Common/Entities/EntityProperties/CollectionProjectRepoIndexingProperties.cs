// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties.CollectionProjectRepoIndexingProperties
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties
{
  [DataContract]
  [Export(typeof (IndexingProperties))]
  public class CollectionProjectRepoIndexingProperties : IndexingProperties
  {
    [DataMember]
    public Dictionary<Guid, HashSet<Guid>> IndexedProjectRepoInfo { get; set; }

    public CollectionProjectRepoIndexingProperties() => this.IndexedProjectRepoInfo = new Dictionary<Guid, HashSet<Guid>>();

    public void ResetWaterMark() => this.IndexedProjectRepoInfo = new Dictionary<Guid, HashSet<Guid>>();

    public HashSet<Guid> GetListOfAllKnownRepositories()
    {
      HashSet<Guid> knownRepositories = new HashSet<Guid>();
      foreach (KeyValuePair<Guid, HashSet<Guid>> keyValuePair in this.IndexedProjectRepoInfo)
      {
        foreach (Guid guid in keyValuePair.Value)
          knownRepositories.Add(guid);
      }
      return knownRepositories;
    }

    public HashSet<Guid> GetListOfAllKnownRepositories(Guid projectId) => this.IndexedProjectRepoInfo.ContainsKey(projectId) ? this.IndexedProjectRepoInfo[projectId] : new HashSet<Guid>();

    public virtual HashSet<Guid> GetListOfAllKnownProjects()
    {
      HashSet<Guid> allKnownProjects = new HashSet<Guid>();
      foreach (KeyValuePair<Guid, HashSet<Guid>> keyValuePair in this.IndexedProjectRepoInfo)
        allKnownProjects.Add(keyValuePair.Key);
      return allKnownProjects;
    }

    public void DeleteRepository(Guid repositoryId)
    {
      foreach (KeyValuePair<Guid, HashSet<Guid>> keyValuePair in this.IndexedProjectRepoInfo)
      {
        if (keyValuePair.Value.Contains(repositoryId))
          keyValuePair.Value.Remove(repositoryId);
      }
    }
  }
}
