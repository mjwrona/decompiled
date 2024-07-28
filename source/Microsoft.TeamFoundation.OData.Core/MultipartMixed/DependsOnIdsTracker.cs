// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.MultipartMixed.DependsOnIdsTracker
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System.Collections.Generic;

namespace Microsoft.OData.MultipartMixed
{
  internal sealed class DependsOnIdsTracker
  {
    private readonly IList<string> topLevelDependsOnIds;
    private readonly IList<string> changeSetDependsOnIds;
    private bool isInChangeSet;

    internal DependsOnIdsTracker()
    {
      this.topLevelDependsOnIds = (IList<string>) new List<string>();
      this.changeSetDependsOnIds = (IList<string>) new List<string>();
      this.isInChangeSet = false;
    }

    internal void ChangeSetStarted() => this.isInChangeSet = true;

    internal void ChangeSetEnded()
    {
      this.isInChangeSet = false;
      this.changeSetDependsOnIds.Clear();
    }

    internal void AddDependsOnId(string id)
    {
      if (this.isInChangeSet)
        this.changeSetDependsOnIds.Add(id);
      else
        this.topLevelDependsOnIds.Add(id);
    }

    internal IEnumerable<string> GetDependsOnIds() => !this.isInChangeSet ? (IEnumerable<string>) this.topLevelDependsOnIds : (IEnumerable<string>) this.changeSetDependsOnIds;
  }
}
