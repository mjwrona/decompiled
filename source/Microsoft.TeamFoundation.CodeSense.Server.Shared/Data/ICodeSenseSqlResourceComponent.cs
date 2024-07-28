// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.Data.ICodeSenseSqlResourceComponent
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.CodeSense.Server.Data
{
  public interface ICodeSenseSqlResourceComponent : IDisposable
  {
    void AddSlices(IEnumerable<SliceDescriptor> slices);

    IEnumerable<SliceDescriptor> GetSlices(int sliceCount);

    void RemoveSlices(IEnumerable<SliceDescriptor> slices);

    void RemoveAllSlices();

    AggregateDescriptor GetAggregate(string aggregatePath, Guid projectGuid);

    void AddAggregates(
      IEnumerable<AggregateDescriptor> aggregateDescriptor);

    int DeleteOrphanedFiles(DateTime createdBefore);

    IEnumerable<IgnoreListedItem> GetIgnoreListedPaths();

    void AddWorkItemAssociationLookupEntries(
      IEnumerable<WorkItemAssociationLookupEntry> lookupEntries);

    IEnumerable<int> GetWorkItemAssociationLookupEntries(int changesId);

    int RemoveWorkItemAssociationLookupEntries(int minChangesId, int maxChangesId);

    void AddMopupItemToQueue(int changesetId, int sliceSouceId);

    IEnumerable<int> GetMopupQueueItems();

    void RemoveMopupItems(List<int> changesetIds);

    void AddFilesToDelete(IEnumerable<int> fileIds);

    int GetFilesPendingForDeletion(DateTime createdBefore);
  }
}
