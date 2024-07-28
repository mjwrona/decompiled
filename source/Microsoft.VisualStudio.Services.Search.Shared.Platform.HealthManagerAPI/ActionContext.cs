// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManagerAPI.ActionContext
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManagerAPI, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5B7677EA-AF32-40D9-850C-DD66DED9A2C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManagerAPI.dll

using Microsoft.VisualStudio.Services.Search.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManagerAPI
{
  public class ActionContext
  {
    public ActionContext()
    {
    }

    public ActionContext(
      HashSet<Microsoft.VisualStudio.Services.Search.Common.Entities.HealthJobData.Scenario> scenarios,
      List<Guid> collectionIds,
      Dictionary<Guid, List<Guid>> projectIds,
      Dictionary<Guid, List<Guid>> repoIds,
      IEntityType entityType,
      List<string> indices)
    {
      this.Scenario = scenarios;
      this.CollectionIds = collectionIds;
      this.ProjectIds = projectIds;
      this.RepoIds = repoIds;
      this.EntityType = entityType;
      this.Indices = indices;
    }

    public HashSet<Microsoft.VisualStudio.Services.Search.Common.Entities.HealthJobData.Scenario> Scenario { get; set; }

    public List<Guid> CollectionIds { get; set; }

    public Dictionary<Guid, List<Guid>> ProjectIds { get; set; }

    public Dictionary<Guid, List<Guid>> RepoIds { get; set; }

    public IEntityType EntityType { get; set; }

    public List<string> Indices { get; set; }

    public override int GetHashCode() => this.CollectionIds.GetHashCode();

    public override bool Equals(object obj)
    {
      ActionContext actionContext = obj as ActionContext;
      if (this == actionContext)
        return true;
      if (this != null && actionContext != null)
      {
        if (this.CollectionIds != null || actionContext.CollectionIds != null)
        {
          int? count1 = this.CollectionIds?.Count;
          int? count2 = actionContext.CollectionIds?.Count;
          if (!(count1.GetValueOrDefault() == count2.GetValueOrDefault() & count1.HasValue == count2.HasValue) || !this.CollectionIds.All<Guid>(new Func<Guid, bool>(actionContext.CollectionIds.Contains)))
            goto label_10;
        }
        if ((this.ProjectIds == null && actionContext.ProjectIds == null || this.CheckRepoOrProjectEquality(this.ProjectIds, actionContext.ProjectIds)) && (this.RepoIds == null && actionContext.RepoIds == null || this.CheckRepoOrProjectEquality(this.RepoIds, actionContext.RepoIds)) && this.EntityType == actionContext.EntityType)
        {
          if (this.Indices == null && actionContext.Indices == null)
            return true;
          int? count3 = this.Indices?.Count;
          int? count4 = actionContext.Indices?.Count;
          return count3.GetValueOrDefault() == count4.GetValueOrDefault() & count3.HasValue == count4.HasValue && this.Indices.All<string>(new Func<string, bool>(actionContext.Indices.Contains));
        }
      }
label_10:
      return false;
    }

    public void MergeCollectionIds(List<Guid> collectionIds)
    {
      if (this.CollectionIds == null)
      {
        this.CollectionIds = collectionIds;
      }
      else
      {
        if (collectionIds == null)
          return;
        foreach (Guid collectionId in collectionIds)
        {
          if (!this.CollectionIds.Contains(collectionId))
            this.CollectionIds.Add(collectionId);
        }
      }
    }

    public void MergeScenarios(HashSet<Microsoft.VisualStudio.Services.Search.Common.Entities.HealthJobData.Scenario> scenarios)
    {
      if (this.Scenario == null)
      {
        this.Scenario = scenarios;
      }
      else
      {
        if (scenarios == null)
          return;
        this.Scenario = new HashSet<Microsoft.VisualStudio.Services.Search.Common.Entities.HealthJobData.Scenario>(this.Scenario.Union<Microsoft.VisualStudio.Services.Search.Common.Entities.HealthJobData.Scenario>((IEnumerable<Microsoft.VisualStudio.Services.Search.Common.Entities.HealthJobData.Scenario>) scenarios));
      }
    }

    public void MergeIndices(List<string> indices)
    {
      if (this.Indices == null)
      {
        this.Indices = indices;
      }
      else
      {
        if (indices == null)
          return;
        this.Indices.AddRange((IEnumerable<string>) indices);
      }
    }

    public void MergeRepoIds(Dictionary<Guid, List<Guid>> repoIds)
    {
      if (this.RepoIds == null)
      {
        this.RepoIds = repoIds;
      }
      else
      {
        if (repoIds == null)
          return;
        foreach (Guid key in repoIds.Keys)
        {
          if (this.RepoIds.ContainsKey(key))
            this.RepoIds[key].AddRange((IEnumerable<Guid>) repoIds[key]);
          else
            this.RepoIds.Add(key, repoIds[key]);
        }
      }
    }

    public void MergeProjectIds(Dictionary<Guid, List<Guid>> projectIds)
    {
      if (this.ProjectIds == null)
      {
        this.ProjectIds = projectIds;
      }
      else
      {
        if (projectIds == null)
          return;
        foreach (Guid key in projectIds.Keys)
        {
          if (this.ProjectIds.ContainsKey(key))
            this.ProjectIds[key].AddRange((IEnumerable<Guid>) projectIds[key]);
          else
            this.ProjectIds.Add(key, projectIds[key]);
        }
      }
    }

    private bool CheckRepoOrProjectEquality(
      Dictionary<Guid, List<Guid>> object1,
      Dictionary<Guid, List<Guid>> object2)
    {
      if (object1 == null && object2 == null)
        return true;
      int? count1 = object1?.Count;
      int? count2 = object2?.Count;
      if (!(count1.GetValueOrDefault() == count2.GetValueOrDefault() & count1.HasValue == count2.HasValue))
        return false;
      foreach (KeyValuePair<Guid, List<Guid>> keyValuePair in object1)
      {
        List<Guid> guidList;
        object2.TryGetValue(keyValuePair.Key, out guidList);
        count2 = guidList?.Count;
        count1 = keyValuePair.Value?.Count;
        if (!(count2.GetValueOrDefault() == count1.GetValueOrDefault() & count2.HasValue == count1.HasValue) || !keyValuePair.Value.All<Guid>(new Func<Guid, bool>(guidList.Contains)))
          return false;
      }
      return true;
    }
  }
}
