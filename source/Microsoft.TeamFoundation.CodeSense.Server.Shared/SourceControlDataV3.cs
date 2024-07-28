// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.SourceControlDataV3
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.CodeSense.Server
{
  public class SourceControlDataV3
  {
    private List<WorkItemDataV3> _workItems;
    private ConcurrentDictionary<string, CommitDataV3> _changesetMap = new ConcurrentDictionary<string, CommitDataV3>();
    private ConcurrentDictionary<string, UserData> _userMap = new ConcurrentDictionary<string, UserData>();
    private ConcurrentDictionary<int, WorkItemDataV3> _workItemMap = new ConcurrentDictionary<int, WorkItemDataV3>();

    public SourceControlDataV3()
      : this(new List<CommitDataV3>(), new List<WorkItemDataV3>(), new List<UserData>())
    {
    }

    public SourceControlDataV3(
      List<CommitDataV3> changesets,
      List<WorkItemDataV3> workItems,
      List<UserData> users)
    {
      this.Changesets = changesets;
      this.WorkItems = workItems;
      this.Users = users;
    }

    [JsonProperty]
    public List<CommitDataV3> Changesets { get; private set; }

    [JsonProperty]
    public List<UserData> Users { get; private set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public List<WorkItemDataV3> WorkItems
    {
      get => this._workItems == null || !this._workItems.Any<WorkItemDataV3>() ? (List<WorkItemDataV3>) null : this._workItems;
      set => this._workItems = value;
    }

    public void Merge(SourceControlDataV3 other)
    {
      if (other == null)
        return;
      this.UpdateChangesets((IEnumerable<CommitDataV3>) other.Changesets);
      this.UpdateWorkItems((IEnumerable<WorkItemDataV3>) other.WorkItems);
      this.UpdateUsers((IEnumerable<UserData>) other.Users);
    }

    public void UpdateChangeset(CommitDataV3 changeset)
    {
      if (changeset == null)
        return;
      CommitDataV3 changeset1 = this.FindChangeset(changeset.ChangesId);
      if (changeset1 == null)
      {
        this.Changesets.Add(changeset);
        this._changesetMap.TryAdd(changeset.ChangesId, changeset);
      }
      else
      {
        this.Changesets[this.Changesets.FindIndex((Predicate<CommitDataV3>) (c => c.ChangesId == changeset.ChangesId))] = changeset;
        this._changesetMap.TryUpdate(changeset.ChangesId, changeset, changeset1);
      }
    }

    public CommitDataV3 FindChangeset(string changesetId)
    {
      CommitDataV3 changeset = (CommitDataV3) null;
      if (!string.IsNullOrEmpty(changesetId))
      {
        this.InitializeChangesetLookupIfRequired();
        this._changesetMap.TryGetValue(changesetId, out changeset);
      }
      return changeset;
    }

    public void UpdateUser(UserData user)
    {
      if (user == null || user.UniqueName == null)
        return;
      UserData user1 = this.FindUser(user.UniqueName);
      if (user1 == null)
      {
        this.Users.Add(user);
        this._userMap.TryAdd(user.UniqueName, user);
      }
      else
      {
        this.Users[this.Users.FindIndex((Predicate<UserData>) (u => u.UniqueName == user.UniqueName))] = user;
        this._userMap.TryUpdate(user.UniqueName, user, user1);
      }
    }

    public void UpdateWorkItems(IEnumerable<WorkItemDataV3> workItems)
    {
      if (workItems == null || !workItems.Any<WorkItemDataV3>())
        return;
      foreach (WorkItemDataV3 workItem in workItems)
        this.UpdateWorkItem(workItem);
    }

    public WorkItemDataV3 FindWorkItem(int workItemId)
    {
      this.InitializeWorkItemLookupIfRequired();
      WorkItemDataV3 workItem = (WorkItemDataV3) null;
      this._workItemMap.TryGetValue(workItemId, out workItem);
      return workItem;
    }

    public UserData FindUser(string uniqueName)
    {
      UserData user = (UserData) null;
      if (!string.IsNullOrEmpty(uniqueName))
      {
        this.InitializeUserLookupIfRequired();
        this._userMap.TryGetValue(uniqueName, out user);
      }
      return user;
    }

    public void RemoveChangesetDetails()
    {
      if (this.Changesets == null)
        return;
      foreach (CommitDataV3 changeset in this.Changesets)
        changeset.ChangesComment = (string) null;
    }

    public void RemoveWorkItemDetails()
    {
      if (this.WorkItems == null)
        return;
      foreach (WorkItemDataV3 workItem in this.WorkItems)
        workItem.RemoveDetails();
    }

    public HashSet<string> FilterData(int retentionPeriod)
    {
      HashSet<string> changesetIds = new HashSet<string>();
      if (this.Changesets != null)
      {
        DateTime dateTime = DateTime.UtcNow.AddMonths(-retentionPeriod);
        HashSet<int> workItems = new HashSet<int>(this._workItems.Select<WorkItemDataV3, int>((Func<WorkItemDataV3, int>) (w => w.Id)));
        HashSet<string> uniqueNames = new HashSet<string>(this.Users.Select<UserData, string>((Func<UserData, string>) (u => u.UniqueName)));
        foreach (CommitDataV3 changeset in this.Changesets)
        {
          if (changeset.Date < dateTime)
          {
            changesetIds.Add(changeset.ChangesId);
          }
          else
          {
            uniqueNames.Remove(changeset.AuthorUniqueName);
            if (changeset.WorkItems != null)
            {
              foreach (int workItem in changeset.WorkItems)
                workItems.Remove(workItem);
            }
          }
        }
        this.RemoveWorkItems(workItems);
        this.RemoveUsers(uniqueNames);
        this.RemoveChangesets(changesetIds);
      }
      return changesetIds;
    }

    private void UpdateChangesets(IEnumerable<CommitDataV3> changesets)
    {
      if (changesets == null || !changesets.Any<CommitDataV3>())
        return;
      foreach (CommitDataV3 changeset in changesets)
        this.UpdateChangeset(changeset);
    }

    private void UpdateUsers(IEnumerable<UserData> users)
    {
      if (users == null || !users.Any<UserData>())
        return;
      foreach (UserData user in users)
        this.UpdateUser(user);
    }

    public void UpdateWorkItem(WorkItemDataV3 workItem)
    {
      if (workItem == null)
        return;
      WorkItemDataV3 workItem1 = this.FindWorkItem(workItem.Id);
      if (workItem1 == null)
      {
        this._workItems.Add(workItem);
        this._workItemMap.TryAdd(workItem.Id, workItem);
      }
      else
      {
        this._workItems[this._workItems.FindIndex((Predicate<WorkItemDataV3>) (w => w.Id == workItem.Id))] = workItem;
        this._workItemMap.TryUpdate(workItem.Id, workItem, workItem1);
      }
    }

    private void RemoveWorkItems(HashSet<int> workItems)
    {
      if (workItems == null || !workItems.Any<int>())
        return;
      this._workItems.RemoveAll((Predicate<WorkItemDataV3>) (w => workItems.Contains(w.Id)));
      foreach (int workItem in workItems)
        this._workItemMap.TryRemove(workItem, out WorkItemDataV3 _);
    }

    private void RemoveUsers(HashSet<string> uniqueNames)
    {
      if (uniqueNames == null || !uniqueNames.Any<string>())
        return;
      this.Users.RemoveAll((Predicate<UserData>) (u => uniqueNames.Contains(u.UniqueName)));
      foreach (string uniqueName in uniqueNames)
        this._userMap.TryRemove(uniqueName, out UserData _);
    }

    private void RemoveChangesets(HashSet<string> changesetIds)
    {
      if (changesetIds == null || !changesetIds.Any<string>())
        return;
      this.Changesets.RemoveAll((Predicate<CommitDataV3>) (c => changesetIds.Contains(c.ChangesId)));
      foreach (string changesetId in changesetIds)
        this._changesetMap.TryRemove(changesetId, out CommitDataV3 _);
    }

    private void InitializeChangesetLookupIfRequired()
    {
      if (this._changesetMap.Count != 0 || this.Changesets.Count <= 0)
        return;
      foreach (CommitDataV3 changeset in this.Changesets)
        this._changesetMap.TryAdd(changeset.ChangesId, changeset);
    }

    private void InitializeUserLookupIfRequired()
    {
      if (this._userMap.Count != 0 || this.Users.Count <= 0)
        return;
      foreach (UserData userData in this.Users.Where<UserData>((Func<UserData, bool>) (u => u.UniqueName != null)))
        this._userMap.TryAdd(userData.UniqueName, userData);
    }

    private void InitializeWorkItemLookupIfRequired()
    {
      if (this._workItemMap.Count != 0 || this._workItems.Count <= 0)
        return;
      foreach (WorkItemDataV3 workItem in this._workItems)
        this._workItemMap.TryAdd(workItem.Id, workItem);
    }
  }
}
