// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.AuthorSummary
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.CodeSense.Server
{
  public sealed class AuthorSummary
  {
    [JsonConstructor]
    public AuthorSummary()
      : this(string.Empty, string.Empty, Enumerable.Empty<AuthorSummary.ChangeSummary>())
    {
    }

    public AuthorSummary(
      string name,
      string displayName,
      IEnumerable<AuthorSummary.ChangeSummary> changes)
    {
      this.AuthorUniqueName = name;
      this.AuthorDisplayName = displayName;
      this.ChangeSummaries = changes.ToList<AuthorSummary.ChangeSummary>();
    }

    internal AuthorSummary(AuthorSummary currentSummary, CodeElementChangeResult changeResult)
      : this(currentSummary.AuthorUniqueName, currentSummary.AuthorDisplayName, AuthorSummary.JoinChanges((IEnumerable<AuthorSummary.ChangeSummary>) currentSummary.ChangeSummaries, changeResult))
    {
    }

    internal AuthorSummary(CodeElementChangeResult changeResult)
      : this(changeResult.AuthorUniqueName, changeResult.AuthorDisplayName, AuthorSummary.JoinChanges(Enumerable.Empty<AuthorSummary.ChangeSummary>(), changeResult))
    {
    }

    [JsonProperty]
    public string AuthorDisplayName { get; private set; }

    [JsonProperty]
    public string AuthorUniqueName { get; private set; }

    [JsonProperty]
    public List<AuthorSummary.ChangeSummary> ChangeSummaries { get; private set; }

    public void AppendChangeSummaries(IEnumerable<AuthorSummary.ChangeSummary> summaries) => this.ChangeSummaries = this.ChangeSummaries.Concat<AuthorSummary.ChangeSummary>(summaries).ToList<AuthorSummary.ChangeSummary>();

    private static IEnumerable<AuthorSummary.ChangeSummary> JoinChanges(
      IEnumerable<AuthorSummary.ChangeSummary> currentChanges,
      CodeElementChangeResult change)
    {
      return currentChanges.Select<AuthorSummary.ChangeSummary, AuthorSummary.ChangeSummary>((Func<AuthorSummary.ChangeSummary, AuthorSummary.ChangeSummary>) (c => c)).Union<AuthorSummary.ChangeSummary>((IEnumerable<AuthorSummary.ChangeSummary>) new AuthorSummary.ChangeSummary[1]
      {
        new AuthorSummary.ChangeSummary(change.ChangesId, change.WorkItems)
      });
    }

    public sealed class ChangeSummary
    {
      [JsonConstructor]
      public ChangeSummary()
        : this("0", (IEnumerable<WorkItemData>) null)
      {
      }

      public ChangeSummary(string id, IEnumerable<WorkItemData> workItems)
      {
        this.Id = id;
        if (workItems == null)
          return;
        this.WorkItems = (IEnumerable<AuthorSummary.WorkItem>) workItems.Select<WorkItemData, AuthorSummary.WorkItem>((Func<WorkItemData, AuthorSummary.WorkItem>) (w => new AuthorSummary.WorkItem(w))).ToArray<AuthorSummary.WorkItem>();
      }

      [JsonProperty]
      public string Id { get; private set; }

      [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
      public IEnumerable<AuthorSummary.WorkItem> WorkItems { get; private set; }
    }

    public sealed class WorkItem
    {
      [JsonConstructor]
      public WorkItem()
        : this(0, string.Empty, string.Empty)
      {
      }

      public WorkItem(WorkItemData workItemData)
        : this(workItemData.Id, workItemData.Type, workItemData.Category)
      {
      }

      public WorkItem(int id, string typeName, string categoryName)
      {
        this.WorkItemId = id;
        this.Type = typeName;
        this.Category = categoryName;
      }

      [JsonProperty]
      public int WorkItemId { get; private set; }

      [JsonProperty]
      public string Type { get; private set; }

      [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
      public string Category { get; private set; }
    }
  }
}
