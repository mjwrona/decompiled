// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Actions.ActionSanitizer
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Actions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DE7D0F19-C193-43CC-9602-3C8794FE9CA0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Actions.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManagerAPI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Actions
{
  public class ActionSanitizer
  {
    [StaticSafe]
    internal static Dictionary<ActionType, int> ActionPriority = new Dictionary<ActionType, int>()
    {
      {
        ActionType.ReIndex,
        0
      },
      {
        ActionType.RepoHeal,
        1
      },
      {
        ActionType.CollectionFinalize,
        0
      },
      {
        ActionType.IndexOptimize,
        0
      },
      {
        ActionType.TrackKusto,
        0
      }
    };
    [StaticSafe]
    internal static Dictionary<ActionType, int> ActionGroup = new Dictionary<ActionType, int>()
    {
      {
        ActionType.ReIndex,
        1
      },
      {
        ActionType.RepoHeal,
        1
      },
      {
        ActionType.CollectionFinalize,
        2
      },
      {
        ActionType.IndexOptimize,
        3
      },
      {
        ActionType.TrackKusto,
        4
      }
    };

    public virtual List<ActionData> Sanitize(List<ActionData> actionData) => this.MergeFlattenedActionData(this.FilterActionsBasedOnScope(this.RemoveDuplicates(actionData)));

    public List<ActionData> RemoveDuplicates(List<ActionData> actionDataList)
    {
      List<ActionData> actionDataList1 = new List<ActionData>();
      foreach (ActionData actionData1 in actionDataList)
      {
        foreach (ActionData flattened in actionData1.GetFlattenedList())
        {
          ActionData flattenedActionData = flattened;
          ActionData actionData2 = actionDataList1.Find((Predicate<ActionData>) (x => x == flattenedActionData));
          if (actionData2 == (ActionData) null)
          {
            actionDataList1.Add(flattenedActionData);
          }
          else
          {
            flattenedActionData.ActionContext.MergeScenarios(actionData2.ActionContext.Scenario);
            actionDataList1.Remove(actionData2);
            actionDataList1.Add(flattenedActionData);
          }
        }
      }
      return actionDataList1;
    }

    public List<ActionData> FilterActionsBasedOnScope(List<ActionData> inputActionDataList)
    {
      List<ActionData> actionDataList = new List<ActionData>((IEnumerable<ActionData>) inputActionDataList);
      foreach (ActionData inputActionData in inputActionDataList)
      {
        Guid? collectionId = inputActionData.GetCollectionIdForFlattenedObject();
        int group;
        ActionSanitizer.ActionGroup.TryGetValue(inputActionData.ActionType, out group);
        int priority;
        ActionSanitizer.ActionPriority.TryGetValue(inputActionData.ActionType, out priority);
        int groupVal;
        int priorityVal;
        actionDataList.RemoveAll((Predicate<ActionData>) (x =>
        {
          Guid? forFlattenedObject = x.GetCollectionIdForFlattenedObject();
          Guid? nullable = collectionId;
          return (forFlattenedObject.HasValue == nullable.HasValue ? (forFlattenedObject.HasValue ? (forFlattenedObject.GetValueOrDefault() == nullable.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0 && ActionSanitizer.ActionGroup.TryGetValue(x.ActionType, out groupVal) && groupVal == group && ActionSanitizer.ActionPriority.TryGetValue(x.ActionType, out priorityVal) && priorityVal > priority;
        }));
      }
      return actionDataList;
    }

    internal List<ActionData> MergeFlattenedActionData(List<ActionData> inputActionDataList)
    {
      List<ActionData> actionDataList = new List<ActionData>();
      foreach (List<ActionData> source in inputActionDataList.GroupBy(x => new
      {
        ActionType = x.ActionType,
        EntityType = x.ActionContext.EntityType
      }).Select<IGrouping<\u003C\u003Ef__AnonymousType0<ActionType, IEntityType>, ActionData>, List<ActionData>>(grp => grp.ToList<ActionData>()).ToList<List<ActionData>>())
      {
        ActionData actionData = source.First<ActionData>();
        if (source.Count > 1)
        {
          foreach (ActionData actionToBeMerged in source.Skip<ActionData>(1))
            actionData.Merge(actionToBeMerged);
        }
        actionDataList.Add(actionData);
      }
      return actionDataList;
    }
  }
}
