// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.PropertyMerger`1
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal sealed class PropertyMerger<T> : IDisposable where T : IPropertyMergerItem
  {
    private ITeamFoundationPropertyService m_service;
    private IVssRequestContext m_requestContext;
    private string[] m_propertyNameFilters;
    private TeamFoundationDataReader m_dataReader;
    private IEnumerator<ArtifactPropertyValue> m_dataReaderEnumerator;
    private List<StreamingCollection<T>> m_items;
    private IPropertyMergerItem m_currentItem;
    private ArtifactPropertyValue m_currentValue;
    private VersionControlCommand m_command;
    private int m_currentStreamingCollectionIndex;
    private IEnumerator<T> m_currentEnumerator;
    private const int c_skipSequenceId = -1;
    private Guid m_artifactKind;

    public PropertyMerger(
      VersionControlRequestContext versionControlRequestContext,
      string[] propertyNameFilters,
      VersionControlCommand command,
      Guid artifactKind)
    {
      this.m_service = versionControlRequestContext.VersionControlService.GetPropertyService(versionControlRequestContext);
      this.m_propertyNameFilters = propertyNameFilters;
      this.m_command = command;
      this.m_requestContext = versionControlRequestContext.RequestContext;
      this.m_artifactKind = artifactKind;
      this.TrimCacheSize();
    }

    public void Execute(StreamingCollection<T> item) => this.Execute(new List<StreamingCollection<T>>(1)
    {
      item
    });

    public void Execute(List<StreamingCollection<T>> items)
    {
      List<ArtifactSpec> artifactSpecList = new List<ArtifactSpec>();
      this.m_items = items;
      int num = 0;
      foreach (StreamingCollection<T> streamingCollection in items)
      {
        if (streamingCollection != null)
        {
          IEnumerator<T> queuedItemsEnumerator = streamingCollection.GetQueuedItemsEnumerator();
          while (queuedItemsEnumerator.MoveNext())
          {
            IPropertyMergerItem current = (IPropertyMergerItem) queuedItemsEnumerator.Current;
            ArtifactSpec artifactSpec = current.GetArtifactSpec(this.m_artifactKind);
            if (artifactSpec != null)
            {
              artifactSpecList.Add(artifactSpec);
              current.SequenceId = num;
              ++num;
            }
            else
              current.SequenceId = -1;
          }
        }
      }
      if (this.m_command != null)
        this.m_command.MaxCacheSize = Command.CommandCacheLimit;
      if (artifactSpecList.Count <= 0)
        return;
      this.m_dataReader = this.m_service.GetProperties(this.m_requestContext, (IEnumerable<ArtifactSpec>) artifactSpecList, (IEnumerable<string>) this.m_propertyNameFilters);
      this.m_dataReaderEnumerator = this.m_dataReader.CurrentEnumerable<ArtifactPropertyValue>().GetEnumerator();
      if (this.m_dataReaderEnumerator.MoveNext())
      {
        this.m_currentValue = this.m_dataReaderEnumerator.Current;
        this.m_currentStreamingCollectionIndex = 0;
        this.m_currentEnumerator = (IEnumerator<T>) null;
        this.AdvanceItemUntilMatch();
      }
      else
      {
        this.m_currentValue = (ArtifactPropertyValue) null;
        this.m_currentStreamingCollectionIndex = this.m_items.Count;
      }
    }

    private void AdvanceItemUntilMatch()
    {
      while (this.m_currentStreamingCollectionIndex < this.m_items.Count)
      {
        if (this.m_items[this.m_currentStreamingCollectionIndex] != null)
        {
          bool flag = true;
          if (this.m_currentEnumerator == null)
          {
            this.m_currentEnumerator = this.m_items[this.m_currentStreamingCollectionIndex].GetQueuedItemsEnumerator();
            flag = this.m_currentEnumerator.MoveNext();
          }
          for (; flag; flag = this.m_currentEnumerator.MoveNext())
          {
            if (this.m_currentEnumerator.Current.SequenceId == this.m_currentValue.SequenceId)
            {
              this.m_currentItem = (IPropertyMergerItem) this.m_currentEnumerator.Current;
              return;
            }
          }
        }
        ++this.m_currentStreamingCollectionIndex;
        this.m_currentEnumerator = (IEnumerator<T>) null;
      }
      this.m_currentItem = (IPropertyMergerItem) null;
    }

    public bool TryMergeNextPage()
    {
      while (this.m_currentValue != null && this.m_currentItem != null)
      {
        StreamingCollection<PropertyValue> properties = this.m_currentItem.GetProperties(this.m_artifactKind);
        if (properties == null)
        {
          properties = new StreamingCollection<PropertyValue>((Command) this.m_command)
          {
            HandleExceptions = false
          };
          this.m_currentItem.SetProperties(this.m_artifactKind, properties);
        }
        while ((this.m_command == null || !this.m_command.IsCacheFull) && this.m_currentValue.PropertyValues.MoveNext())
          properties.Enqueue(this.m_currentValue.PropertyValues.Current);
        if (this.m_command != null && this.m_command.IsCacheFull)
          return true;
        properties.IsComplete = true;
        if (this.m_dataReaderEnumerator.MoveNext())
        {
          this.m_currentValue = this.m_dataReaderEnumerator.Current;
          this.AdvanceItemUntilMatch();
        }
        else
          this.m_currentValue = (ArtifactPropertyValue) null;
      }
      if (this.m_dataReader != null)
      {
        this.m_dataReader.Dispose();
        this.m_dataReader = (TeamFoundationDataReader) null;
        this.m_dataReaderEnumerator = (IEnumerator<ArtifactPropertyValue>) null;
      }
      this.TrimCacheSize();
      return false;
    }

    private void TrimCacheSize()
    {
      if (this.m_command == null || this.m_command.MaxCacheSize <= 1)
        return;
      this.m_command.MaxCacheSize = Command.CommandCacheLimit >> 1;
    }

    public void Dispose()
    {
      if (this.m_dataReader == null)
        return;
      this.m_dataReader.Dispose();
      this.m_dataReader = (TeamFoundationDataReader) null;
      this.m_dataReaderEnumerator = (IEnumerator<ArtifactPropertyValue>) null;
    }
  }
}
