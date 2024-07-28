// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManagerAPI.ActionData
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManagerAPI, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5B7677EA-AF32-40D9-850C-DD66DED9A2C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManagerAPI.dll

using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.HealthJobData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManagerAPI
{
  public class ActionData
  {
    public ActionData(ActionType actionType, ActionContext actionContext)
    {
      this.ActionType = actionType;
      this.ActionContext = actionContext;
    }

    public ActionType ActionType { get; }

    public ActionContext ActionContext { get; }

    public List<ActionData> GetFlattenedList()
    {
      List<ActionData> flattenedList = new List<ActionData>();
      if (this.ActionContext.RepoIds != null)
      {
        foreach (KeyValuePair<Guid, List<Guid>> repoId in this.ActionContext.RepoIds)
        {
          foreach (Guid guid in repoId.Value)
          {
            HashSet<Scenario> scenario = this.ActionContext.Scenario;
            Dictionary<Guid, List<Guid>> repoIds = new Dictionary<Guid, List<Guid>>();
            repoIds.Add(repoId.Key, new List<Guid>()
            {
              guid
            });
            IEntityType entityType = this.ActionContext.EntityType;
            List<string> indices = this.ActionContext.Indices;
            ActionContext actionContext = new ActionContext(scenario, (List<Guid>) null, (Dictionary<Guid, List<Guid>>) null, repoIds, entityType, indices);
            flattenedList.Add(new ActionData(this.ActionType, actionContext));
          }
        }
      }
      if (this.ActionContext.ProjectIds != null)
      {
        foreach (KeyValuePair<Guid, List<Guid>> projectId in this.ActionContext.ProjectIds)
        {
          foreach (Guid guid in projectId.Value)
          {
            HashSet<Scenario> scenario = this.ActionContext.Scenario;
            Dictionary<Guid, List<Guid>> projectIds = new Dictionary<Guid, List<Guid>>();
            projectIds.Add(projectId.Key, new List<Guid>()
            {
              guid
            });
            IEntityType entityType = this.ActionContext.EntityType;
            List<string> indices = this.ActionContext.Indices;
            ActionContext actionContext = new ActionContext(scenario, (List<Guid>) null, projectIds, (Dictionary<Guid, List<Guid>>) null, entityType, indices);
            flattenedList.Add(new ActionData(this.ActionType, actionContext));
          }
        }
      }
      if (this.ActionContext.CollectionIds != null)
      {
        foreach (Guid collectionId in this.ActionContext.CollectionIds)
        {
          HashSet<Scenario> scenario = this.ActionContext.Scenario;
          List<Guid> collectionIds = new List<Guid>();
          collectionIds.Add(collectionId);
          IEntityType entityType = this.ActionContext.EntityType;
          List<string> indices = this.ActionContext.Indices;
          ActionContext actionContext = new ActionContext(scenario, collectionIds, (Dictionary<Guid, List<Guid>>) null, (Dictionary<Guid, List<Guid>>) null, entityType, indices);
          flattenedList.Add(new ActionData(this.ActionType, actionContext));
        }
      }
      if (flattenedList.Count > 0)
        return flattenedList;
      return new List<ActionData>() { this };
    }

    public Guid? GetCollectionIdForFlattenedObject()
    {
      List<Guid> collectionIds = this.ActionContext.CollectionIds;
      Guid? forFlattenedObject = collectionIds != null ? new Guid?(collectionIds.First<Guid>()) : new Guid?();
      KeyValuePair<Guid, List<Guid>> keyValuePair;
      if (!forFlattenedObject.HasValue)
      {
        Dictionary<Guid, List<Guid>> projectIds = this.ActionContext.ProjectIds;
        Guid? nullable;
        if (projectIds == null)
        {
          nullable = new Guid?();
        }
        else
        {
          keyValuePair = projectIds.First<KeyValuePair<Guid, List<Guid>>>();
          nullable = new Guid?(keyValuePair.Key);
        }
        forFlattenedObject = nullable;
      }
      if (!forFlattenedObject.HasValue)
      {
        Dictionary<Guid, List<Guid>> repoIds = this.ActionContext.RepoIds;
        Guid? nullable;
        if (repoIds == null)
        {
          nullable = new Guid?();
        }
        else
        {
          keyValuePair = repoIds.First<KeyValuePair<Guid, List<Guid>>>();
          nullable = new Guid?(keyValuePair.Key);
        }
        forFlattenedObject = nullable;
      }
      return forFlattenedObject;
    }

    public void Merge(ActionData actionToBeMerged)
    {
      if (this.ActionType != actionToBeMerged.ActionType || this.ActionContext.EntityType != actionToBeMerged.ActionContext.EntityType)
        return;
      this.ActionContext.MergeCollectionIds(actionToBeMerged.ActionContext.CollectionIds);
      this.ActionContext.MergeProjectIds(actionToBeMerged.ActionContext.ProjectIds);
      this.ActionContext.MergeRepoIds(actionToBeMerged.ActionContext.RepoIds);
      this.ActionContext.MergeScenarios(actionToBeMerged.ActionContext.Scenario);
      this.ActionContext.MergeIndices(actionToBeMerged.ActionContext.Indices);
    }

    public override bool Equals(object obj) => this == obj as ActionData;

    public bool Equals(ActionData other) => this == other;

    public override int GetHashCode() => this.ActionType.GetHashCode();

    public static bool operator ==(ActionData a, ActionData b)
    {
      if ((object) a == (object) b)
        return true;
      return (object) a != null && (object) b != null && a.ActionType == b.ActionType && a.ActionContext.Equals((object) b.ActionContext);
    }

    public static bool operator !=(ActionData a, ActionData b) => !(a == b);
  }
}
