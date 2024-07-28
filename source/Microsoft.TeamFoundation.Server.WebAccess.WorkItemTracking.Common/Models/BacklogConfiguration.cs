// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models
{
  public class BacklogConfiguration
  {
    public BacklogConfiguration() => this.PortfolioBacklogs = (IReadOnlyCollection<BacklogLevelConfiguration>) new List<BacklogLevelConfiguration>();

    public Guid? ProjectId { get; set; }

    public ProcessDescriptor ProcessDescriptor { get; set; }

    public Guid? TeamId { get; set; }

    public BacklogLevelConfiguration TaskBacklog { get; set; }

    public BacklogLevelConfiguration RequirementBacklog { get; set; }

    public IReadOnlyCollection<BacklogLevelConfiguration> PortfolioBacklogs { get; set; }

    public IReadOnlyCollection<WorkItemTypeStateInfo> WorkItemTypeMappedStates { get; set; }

    public BacklogFieldInfo BacklogFields { get; set; }

    public IReadOnlyCollection<string> BugWorkItemTypes { get; set; }

    public Microsoft.TeamFoundation.Work.WebApi.BugsBehavior BugsBehavior { get; set; }

    public bool IsBugsBehaviorConfigValid { get; set; }

    public IReadOnlyCollection<string> HiddenBacklogs { get; set; }

    public virtual bool TryGetBacklogByName(string name, out BacklogLevelConfiguration backlogLevel)
    {
      try
      {
        backlogLevel = this.GetBacklogByName(name);
      }
      catch (BacklogDoesNotExistException ex)
      {
        backlogLevel = (BacklogLevelConfiguration) null;
        return false;
      }
      return true;
    }

    public virtual BacklogLevelConfiguration GetBacklogByName(string name) => this.GetAllBacklogLevels().FirstOrDefault<BacklogLevelConfiguration>((Func<BacklogLevelConfiguration, bool>) (blc => TFStringComparer.BacklogPluralName.Equals(blc.Name, name))) ?? throw new BacklogDoesNotExistException(name);

    public virtual bool TryGetVisibleBacklogByNameOrId(
      string idOrName,
      out BacklogLevelConfiguration backlog)
    {
      try
      {
        backlog = this.GetVisibleBacklogByNameOrId(idOrName);
      }
      catch (BacklogDoesNotExistException ex)
      {
        backlog = (BacklogLevelConfiguration) null;
        return false;
      }
      return true;
    }

    public virtual BacklogLevelConfiguration GetVisibleBacklogByNameOrId(string idOrName)
    {
      IEnumerable<BacklogLevelConfiguration> productBacklogLevels = this.GetVisibleProductBacklogLevels();
      return (productBacklogLevels.FirstOrDefault<BacklogLevelConfiguration>((Func<BacklogLevelConfiguration, bool>) (blc => TFStringComparer.BacklogPluralName.Equals(blc.Name, idOrName))) ?? productBacklogLevels.FirstOrDefault<BacklogLevelConfiguration>((Func<BacklogLevelConfiguration, bool>) (blc => TFStringComparer.BacklogLevelId.Equals(blc.Id, idOrName)))) ?? throw new BacklogDoesNotExistException(idOrName);
    }

    public virtual IEnumerable<BacklogLevelConfiguration> GetVisibleProductBacklogLevels() => this.PortfolioBacklogs.Concat<BacklogLevelConfiguration>((IEnumerable<BacklogLevelConfiguration>) new BacklogLevelConfiguration[1]
    {
      this.RequirementBacklog
    }).OrderBy<BacklogLevelConfiguration, int>((Func<BacklogLevelConfiguration, int>) (blc => blc.Rank)).Where<BacklogLevelConfiguration>((Func<BacklogLevelConfiguration, bool>) (blc => !this.HiddenBacklogs.Contains<string>(blc.Id, (IEqualityComparer<string>) TFStringComparer.BacklogLevelId)));

    public virtual bool TryGetBacklogByWorkItemTypeName(
      string workItemTypeName,
      out BacklogLevelConfiguration backlogLevel)
    {
      try
      {
        backlogLevel = this.GetBacklogByWorkItemTypeName(workItemTypeName);
      }
      catch (InvalidBacklogWorkItemType ex)
      {
        backlogLevel = (BacklogLevelConfiguration) null;
        return false;
      }
      return true;
    }

    public virtual BacklogLevelConfiguration GetBacklogByWorkItemTypeName(string workItemTypeName) => this.GetAllBacklogLevels().FirstOrDefault<BacklogLevelConfiguration>((Func<BacklogLevelConfiguration, bool>) (blc => blc.WorkItemTypes.Contains<string>(workItemTypeName, (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName))) ?? throw new InvalidBacklogWorkItemType(workItemTypeName);

    public virtual BacklogLevelConfiguration GetLowestVisibleBacklogLevel() => this.GetVisibleProductBacklogLevels().FirstOrDefault<BacklogLevelConfiguration>();

    public virtual bool TryGetHighestVisiblePortfolioLevel(
      out BacklogLevelConfiguration backlogLevel)
    {
      backlogLevel = this.PortfolioBacklogs.OrderByDescending<BacklogLevelConfiguration, int>((Func<BacklogLevelConfiguration, int>) (blc => blc.Rank)).FirstOrDefault<BacklogLevelConfiguration>((Func<BacklogLevelConfiguration, bool>) (blc => !this.HiddenBacklogs.Contains<string>(blc.Id, (IEqualityComparer<string>) TFStringComparer.BacklogLevelId)));
      return backlogLevel != null;
    }

    public virtual bool TryGetHighestPortfolioLevel(out BacklogLevelConfiguration backlogLevel)
    {
      backlogLevel = this.PortfolioBacklogs.OrderByDescending<BacklogLevelConfiguration, int>((Func<BacklogLevelConfiguration, int>) (blc => blc.Rank)).FirstOrDefault<BacklogLevelConfiguration>();
      return backlogLevel != null;
    }

    public virtual bool TryGetBacklogLevelConfiguration(
      string backlogLevelId,
      out BacklogLevelConfiguration backlogLevel)
    {
      try
      {
        backlogLevel = this.GetBacklogLevelConfiguration(backlogLevelId);
      }
      catch (BacklogDoesNotExistException ex)
      {
        backlogLevel = (BacklogLevelConfiguration) null;
        return false;
      }
      return true;
    }

    public virtual BacklogLevelConfiguration GetBacklogLevelConfiguration(string backlogLevelId) => this.GetAllBacklogLevels().Where<BacklogLevelConfiguration>((Func<BacklogLevelConfiguration, bool>) (blc => blc != null)).FirstOrDefault<BacklogLevelConfiguration>((Func<BacklogLevelConfiguration, bool>) (blc => TFStringComparer.BacklogLevelId.Equals(blc.Id, backlogLevelId))) ?? throw new BacklogDoesNotExistException(backlogLevelId);

    public virtual IReadOnlyCollection<string> GetWorkItemStates(
      BacklogLevelConfiguration backlogLevel,
      WorkItemStateCategory[] categoryFilter = null)
    {
      ArgumentUtility.CheckForNull<BacklogLevelConfiguration>(backlogLevel, nameof (backlogLevel));
      IEnumerable<KeyValuePair<string, WorkItemStateCategory>> source = this.WorkItemTypeMappedStates.Where<WorkItemTypeStateInfo>((Func<WorkItemTypeStateInfo, bool>) (s => backlogLevel.WorkItemTypes.Contains<string>(s.WorkItemTypeName, (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName))).SelectMany<WorkItemTypeStateInfo, KeyValuePair<string, WorkItemStateCategory>>((Func<WorkItemTypeStateInfo, IEnumerable<KeyValuePair<string, WorkItemStateCategory>>>) (s => (IEnumerable<KeyValuePair<string, WorkItemStateCategory>>) s.States));
      if (categoryFilter != null)
        source = (IEnumerable<KeyValuePair<string, WorkItemStateCategory>>) source.Where<KeyValuePair<string, WorkItemStateCategory>>((Func<KeyValuePair<string, WorkItemStateCategory>, bool>) (s => ((IEnumerable<WorkItemStateCategory>) categoryFilter).Contains<WorkItemStateCategory>(s.Value))).OrderBy<KeyValuePair<string, WorkItemStateCategory>, int>((Func<KeyValuePair<string, WorkItemStateCategory>, int>) (kv => Array.IndexOf<WorkItemStateCategory>(categoryFilter, kv.Value))).ToList<KeyValuePair<string, WorkItemStateCategory>>();
      return (IReadOnlyCollection<string>) source.Select<KeyValuePair<string, WorkItemStateCategory>, string>((Func<KeyValuePair<string, WorkItemStateCategory>, string>) (s => s.Key)).Distinct<string>((IEqualityComparer<string>) TFStringComparer.WorkItemStateName).ToList<string>();
    }

    public virtual IReadOnlyCollection<string> GetWorkItemStates(
      IReadOnlyCollection<string> witNames,
      WorkItemStateCategory[] categoryFilter = null)
    {
      ArgumentUtility.CheckForNull<IReadOnlyCollection<string>>(witNames, nameof (witNames));
      IEnumerable<KeyValuePair<string, WorkItemStateCategory>> source = this.WorkItemTypeMappedStates.Where<WorkItemTypeStateInfo>((Func<WorkItemTypeStateInfo, bool>) (s => witNames.Contains<string>(s.WorkItemTypeName, (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName))).SelectMany<WorkItemTypeStateInfo, KeyValuePair<string, WorkItemStateCategory>>((Func<WorkItemTypeStateInfo, IEnumerable<KeyValuePair<string, WorkItemStateCategory>>>) (s => (IEnumerable<KeyValuePair<string, WorkItemStateCategory>>) s.States));
      if (categoryFilter != null)
        source = (IEnumerable<KeyValuePair<string, WorkItemStateCategory>>) source.Where<KeyValuePair<string, WorkItemStateCategory>>((Func<KeyValuePair<string, WorkItemStateCategory>, bool>) (s => ((IEnumerable<WorkItemStateCategory>) categoryFilter).Contains<WorkItemStateCategory>(s.Value))).OrderBy<KeyValuePair<string, WorkItemStateCategory>, int>((Func<KeyValuePair<string, WorkItemStateCategory>, int>) (kv => Array.IndexOf<WorkItemStateCategory>(categoryFilter, kv.Value))).ToList<KeyValuePair<string, WorkItemStateCategory>>();
      return (IReadOnlyCollection<string>) source.Select<KeyValuePair<string, WorkItemStateCategory>, string>((Func<KeyValuePair<string, WorkItemStateCategory>, string>) (s => s.Key)).Distinct<string>((IEqualityComparer<string>) TFStringComparer.WorkItemStateName).ToList<string>();
    }

    public virtual IReadOnlyCollection<string> GetWorkItemStatesOrderedByCategory(
      BacklogLevelConfiguration backlogLevel,
      bool includeBugStates)
    {
      ArgumentUtility.CheckForNull<BacklogLevelConfiguration>(backlogLevel, nameof (backlogLevel));
      HashSet<string> interestedWits = new HashSet<string>((IEnumerable<string>) backlogLevel.WorkItemTypes, (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
      if (includeBugStates)
        interestedWits.UnionWith((IEnumerable<string>) this.BugWorkItemTypes);
      return (IReadOnlyCollection<string>) this.WorkItemTypeMappedStates.Where<WorkItemTypeStateInfo>((Func<WorkItemTypeStateInfo, bool>) (s => interestedWits.Contains<string>(s.WorkItemTypeName, (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName))).SelectMany<WorkItemTypeStateInfo, KeyValuePair<string, WorkItemStateCategory>>((Func<WorkItemTypeStateInfo, IEnumerable<KeyValuePair<string, WorkItemStateCategory>>>) (s => (IEnumerable<KeyValuePair<string, WorkItemStateCategory>>) s.States)).OrderBy<KeyValuePair<string, WorkItemStateCategory>, WorkItemStateCategory>((Func<KeyValuePair<string, WorkItemStateCategory>, WorkItemStateCategory>) (s => s.Value)).Select<KeyValuePair<string, WorkItemStateCategory>, string>((Func<KeyValuePair<string, WorkItemStateCategory>, string>) (s => s.Key)).Distinct<string>((IEqualityComparer<string>) TFStringComparer.WorkItemStateName).ToList<string>();
    }

    public virtual IEnumerable<Tuple<string, WorkItemStateCategory>> GetWorkItemStatesWithCategory(
      BacklogLevelConfiguration backlogLevel)
    {
      ArgumentUtility.CheckForNull<BacklogLevelConfiguration>(backlogLevel, nameof (backlogLevel));
      return this.WorkItemTypeMappedStates.Where<WorkItemTypeStateInfo>((Func<WorkItemTypeStateInfo, bool>) (s => backlogLevel.WorkItemTypes.Contains<string>(s.WorkItemTypeName, (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName))).SelectMany<WorkItemTypeStateInfo, KeyValuePair<string, WorkItemStateCategory>>((Func<WorkItemTypeStateInfo, IEnumerable<KeyValuePair<string, WorkItemStateCategory>>>) (s => (IEnumerable<KeyValuePair<string, WorkItemStateCategory>>) s.States)).Select<KeyValuePair<string, WorkItemStateCategory>, Tuple<string, WorkItemStateCategory>>((Func<KeyValuePair<string, WorkItemStateCategory>, Tuple<string, WorkItemStateCategory>>) (s => new Tuple<string, WorkItemStateCategory>(s.Key, s.Value)));
    }

    public virtual IEnumerable<Tuple<string, WorkItemStateCategory>> GetWorkItemStatesWithCategoryForWorkItem(
      string workItemTypeName)
    {
      ArgumentUtility.CheckForNull<string>(workItemTypeName, nameof (workItemTypeName));
      return this.WorkItemTypeMappedStates.Where<WorkItemTypeStateInfo>((Func<WorkItemTypeStateInfo, bool>) (s => TFStringComparer.WorkItemTypeName.Equals(s.WorkItemTypeName, workItemTypeName))).SelectMany<WorkItemTypeStateInfo, KeyValuePair<string, WorkItemStateCategory>>((Func<WorkItemTypeStateInfo, IEnumerable<KeyValuePair<string, WorkItemStateCategory>>>) (s => (IEnumerable<KeyValuePair<string, WorkItemStateCategory>>) s.States)).Select<KeyValuePair<string, WorkItemStateCategory>, Tuple<string, WorkItemStateCategory>>((Func<KeyValuePair<string, WorkItemStateCategory>, Tuple<string, WorkItemStateCategory>>) (s => new Tuple<string, WorkItemStateCategory>(s.Key, s.Value)));
    }

    public virtual bool StatesContainCategory(
      BacklogLevelConfiguration backlogLevel,
      WorkItemStateCategory stateCategory)
    {
      ArgumentUtility.CheckForNull<BacklogLevelConfiguration>(backlogLevel, nameof (backlogLevel));
      return this.WorkItemTypeMappedStates.Where<WorkItemTypeStateInfo>((Func<WorkItemTypeStateInfo, bool>) (s => backlogLevel.WorkItemTypes.Contains<string>(s.WorkItemTypeName, (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName))).SelectMany<WorkItemTypeStateInfo, KeyValuePair<string, WorkItemStateCategory>>((Func<WorkItemTypeStateInfo, IEnumerable<KeyValuePair<string, WorkItemStateCategory>>>) (s => (IEnumerable<KeyValuePair<string, WorkItemStateCategory>>) s.States)).Any<KeyValuePair<string, WorkItemStateCategory>>((Func<KeyValuePair<string, WorkItemStateCategory>, bool>) (s => s.Value == stateCategory));
    }

    public virtual IEnumerable<BacklogLevelConfiguration> GetAllBacklogLevels() => this.PortfolioBacklogs.Concat<BacklogLevelConfiguration>((IEnumerable<BacklogLevelConfiguration>) new BacklogLevelConfiguration[2]
    {
      this.RequirementBacklog,
      this.TaskBacklog
    });

    public virtual bool IsBacklogVisible(string backlogLevelId) => !this.HiddenBacklogs.Contains<string>(backlogLevelId, (IEqualityComparer<string>) TFStringComparer.BacklogLevelId);

    public virtual bool TryGetBacklogParent(
      string backlogLevelId,
      out BacklogLevelConfiguration parentBacklogLevel)
    {
      parentBacklogLevel = (BacklogLevelConfiguration) null;
      try
      {
        parentBacklogLevel = this.GetBacklogParent(backlogLevelId);
      }
      catch (Exception ex)
      {
        switch (ex)
        {
          case BacklogDoesNotExistException _:
          case BacklogParentNotFoundException _:
            return false;
          default:
            throw;
        }
      }
      return true;
    }

    public virtual BacklogLevelConfiguration GetBacklogParent(string childBacklogLevelId)
    {
      BacklogLevelConfiguration childBacklogLevel = (BacklogLevelConfiguration) null;
      childBacklogLevel = this.GetBacklogLevelConfiguration(childBacklogLevelId);
      return this.GetAllBacklogLevels().OrderBy<BacklogLevelConfiguration, int>((Func<BacklogLevelConfiguration, int>) (blc => blc.Rank)).FirstOrDefault<BacklogLevelConfiguration>((Func<BacklogLevelConfiguration, bool>) (blc => blc.Rank > childBacklogLevel.Rank)) ?? throw new BacklogParentNotFoundException(childBacklogLevelId);
    }

    public virtual bool TryGetBacklogChild(
      string parentBacklogLevelId,
      out BacklogLevelConfiguration childBacklogLevel)
    {
      BacklogLevelConfiguration parentBacklogLevel = this.GetBacklogLevelConfiguration(parentBacklogLevelId);
      childBacklogLevel = this.GetAllBacklogLevels().OrderByDescending<BacklogLevelConfiguration, int>((Func<BacklogLevelConfiguration, int>) (blc => blc.Rank)).FirstOrDefault<BacklogLevelConfiguration>((Func<BacklogLevelConfiguration, bool>) (blc => blc.Rank < parentBacklogLevel.Rank));
      return childBacklogLevel != null;
    }

    public virtual bool TryGetBacklogParents(
      BacklogLevelConfiguration backlogLevel,
      out IReadOnlyCollection<BacklogLevelConfiguration> parentBacklogLevels)
    {
      if (backlogLevel == null)
      {
        parentBacklogLevels = (IReadOnlyCollection<BacklogLevelConfiguration>) new List<BacklogLevelConfiguration>();
        return false;
      }
      parentBacklogLevels = (IReadOnlyCollection<BacklogLevelConfiguration>) this.GetAllBacklogLevels().OrderBy<BacklogLevelConfiguration, int>((Func<BacklogLevelConfiguration, int>) (blc => blc.Rank)).Where<BacklogLevelConfiguration>((Func<BacklogLevelConfiguration, bool>) (blc => blc.Rank > backlogLevel.Rank)).ToList<BacklogLevelConfiguration>();
      return parentBacklogLevels != null && parentBacklogLevels.Any<BacklogLevelConfiguration>();
    }
  }
}
