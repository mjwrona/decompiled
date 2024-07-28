// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CommandQueryChurnItemPairs
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class CommandQueryChurnItemPairs : VersionControlCommand
  {
    private CodeChurnComponent m_db;
    private ResultCollection m_results;
    private ObjectBinder<ItemPair> m_itemPairBinder;
    private StreamingCollection<ItemPair> m_itemPairs;
    private PropertyMerger<ItemPair> m_merger;
    private List<ArtifactPropertyValue> m_failedProperties;
    private List<ArtifactSpec> m_failedItemPairSpecs;
    private List<int> m_failedRetryCounts;
    private int m_failedRetryCountsIndex;
    private CommandQueryChurnItemPairs.State m_state = CommandQueryChurnItemPairs.State.ItemPairs;

    public CommandQueryChurnItemPairs(
      VersionControlRequestContext versionControlRequestContext)
      : base(versionControlRequestContext)
    {
    }

    public void Execute(int batchSize)
    {
      using (TeamFoundationDataReader properties = this.RequestContext.GetService<ITeamFoundationPropertyService>().GetProperties(this.RequestContext, VersionControlPropertyKinds.VersionedItem, (IEnumerable<string>) new string[1]
      {
        CodeChurnConstants.RetryCountPropertyName
      }, new Guid?()))
      {
        StreamingCollection<ArtifactPropertyValue> streamingCollection = properties.Current<StreamingCollection<ArtifactPropertyValue>>();
        this.m_failedProperties = new List<ArtifactPropertyValue>();
        int num = 0;
        while (streamingCollection.MoveNext())
        {
          if (num++ < batchSize)
            this.m_failedProperties.Add(streamingCollection.Current);
          else
            break;
        }
      }
      this.m_failedItemPairSpecs = new List<ArtifactSpec>();
      this.m_failedRetryCounts = new List<int>();
      this.m_db = this.m_versionControlRequestContext.VersionControlService.GetCodeChurnComponent(this.m_versionControlRequestContext);
      this.m_state = CommandQueryChurnItemPairs.State.GetFailedItemPairs;
      this.ExecuteInternal();
    }

    public void Execute(int changeset, string lastServerItem, int batchSize)
    {
      this.m_db = this.m_versionControlRequestContext.VersionControlService.GetCodeChurnComponent(this.m_versionControlRequestContext);
      this.m_results = this.m_db.GetChurnItemPairs(changeset, lastServerItem, batchSize);
      this.m_itemPairBinder = this.m_results.GetCurrent<ItemPair>();
      this.ExecuteInternal();
    }

    private void ExecuteInternal()
    {
      this.m_itemPairs = new StreamingCollection<ItemPair>((Command) this)
      {
        HandleExceptions = false
      };
      this.m_merger = new PropertyMerger<ItemPair>(this.m_versionControlRequestContext, new string[1]
      {
        CodeChurnConstants.DetailsPropertyName
      }, (VersionControlCommand) this, VersionControlPropertyKinds.VersionedItem);
      this.ContinueExecution();
      if (!this.IsCacheFull)
        return;
      this.RequestContext.PartialResultsReady();
    }

    public override void ContinueExecution()
    {
      if (this.m_state == CommandQueryChurnItemPairs.State.GetFailedItemPairs)
      {
        this.m_failedItemPairSpecs.Clear();
        this.m_failedRetryCounts.Clear();
        this.m_failedRetryCountsIndex = 0;
        foreach (ArtifactPropertyValue failedProperty in this.m_failedProperties)
        {
          this.m_failedItemPairSpecs.Add(failedProperty.Spec);
          using (IEnumerator<PropertyValue> enumerator = failedProperty.PropertyValues.GetEnumerator())
          {
            if (enumerator.MoveNext())
              this.m_failedRetryCounts.Add((int) enumerator.Current.Value);
          }
        }
        if (this.m_results != null)
          this.m_results.Dispose();
        this.m_results = this.m_db.GetFailedItemPairs((IEnumerable<ArtifactSpec>) this.m_failedItemPairSpecs);
        this.m_itemPairBinder = this.m_results.GetCurrent<ItemPair>();
        this.m_state = CommandQueryChurnItemPairs.State.ItemPairs;
      }
      if (this.m_state == CommandQueryChurnItemPairs.State.ItemPairs)
      {
        bool flag = true;
        while (!this.IsCacheFull && (flag = this.m_itemPairBinder.MoveNext()))
        {
          ItemPair current = this.m_itemPairBinder.Current;
          if (this.m_failedRetryCounts != null)
            current.RetryCount = this.m_failedRetryCounts[this.m_failedRetryCountsIndex++];
          this.m_itemPairs.Enqueue(current);
        }
        if (!flag)
          this.m_itemPairs.IsComplete = true;
        this.m_merger.Execute(this.m_itemPairs);
        this.m_state = CommandQueryChurnItemPairs.State.ItemPairProperties;
      }
      if (this.m_state != CommandQueryChurnItemPairs.State.ItemPairProperties || this.m_merger.TryMergeNextPage())
        return;
      this.m_state = CommandQueryChurnItemPairs.State.ItemPairs;
    }

    public StreamingCollection<ItemPair> ItemPairs => this.m_itemPairs;

    protected override void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      if (this.m_db != null)
      {
        this.m_db.Dispose();
        this.m_db = (CodeChurnComponent) null;
      }
      if (this.m_results != null)
      {
        this.m_results.Dispose();
        this.m_results = (ResultCollection) null;
      }
      if (this.m_merger == null)
        return;
      this.m_merger.Dispose();
      this.m_merger = (PropertyMerger<ItemPair>) null;
    }

    private enum State
    {
      GetFailedItemPairs,
      ItemPairs,
      ItemPairProperties,
    }
  }
}
