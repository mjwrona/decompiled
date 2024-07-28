// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ExpandedChangeDbPagingManager
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class ExpandedChangeDbPagingManager : IDisposable
  {
    private bool m_recursiveRequest;
    private VersionedItemComponent m_vic;
    private ResultCollection m_resultCollection;
    private SortedDictionary<string, ExpandedChange> m_expandedChanges = new SortedDictionary<string, ExpandedChange>((IComparer<string>) TFStringComparer.VersionControlPath);
    private SortedDictionary<string, ExpandedChange>.ValueCollection.Enumerator m_changeEnumerator;
    private bool m_enumeratorSet;
    private bool m_isPaged;
    private int m_pageThreshold;
    private VersionControlRequestContext m_versionControlRequestContext;

    public ExpandedChangeDbPagingManager(
      VersionControlRequestContext versionControlRequestContext,
      ExpandedChangeEnumerator changeEnum,
      bool recursiveRequest)
    {
      this.m_recursiveRequest = recursiveRequest;
      this.m_versionControlRequestContext = versionControlRequestContext;
      this.m_pageThreshold = this.m_versionControlRequestContext.VersionControlService.GetXmlParameterChunkThreshold(this.m_versionControlRequestContext);
      this.EnqueueAll(changeEnum);
    }

    private void EnqueueAll(ExpandedChangeEnumerator changeEnum)
    {
      foreach (ExpandedChange expandedChange in changeEnum)
      {
        this.Enqueue(expandedChange);
        if (this.m_expandedChanges.Count > this.m_pageThreshold)
        {
          this.m_isPaged = true;
          break;
        }
      }
      if (!this.m_isPaged)
        return;
      changeEnum.Reset();
      this.m_vic = this.m_versionControlRequestContext.VersionControlService.GetVersionedItemComponent(this.m_versionControlRequestContext);
      this.m_resultCollection = this.m_vic.QueryExpandedChanges((IEnumerable<ExpandedChange>) changeEnum, this.m_recursiveRequest);
    }

    private void Enqueue(ExpandedChange item)
    {
      if (this.m_expandedChanges.ContainsKey(item.serverItem))
        return;
      if (this.m_recursiveRequest)
      {
        List<string> stringList = (List<string>) null;
        foreach (string key in this.m_expandedChanges.Keys)
        {
          if (VersionControlPath.IsSubItem(item.serverItem, key))
            return;
          if (VersionControlPath.IsSubItem(key, item.serverItem))
          {
            if (stringList == null)
              stringList = new List<string>();
            stringList.Add(key);
          }
        }
        if (stringList != null)
        {
          foreach (string key in stringList)
            this.m_expandedChanges.Remove(key);
        }
      }
      this.m_expandedChanges.Add(item.serverItem, item);
    }

    public bool TryGetNextItem(out ExpandedChange change)
    {
      change = (ExpandedChange) null;
      if (!this.m_isPaged)
      {
        if (!this.m_enumeratorSet)
        {
          this.m_changeEnumerator = this.m_expandedChanges.Values.GetEnumerator();
          this.m_enumeratorSet = true;
        }
        if (this.m_changeEnumerator.MoveNext())
          change = this.m_changeEnumerator.Current;
      }
      else if (this.m_resultCollection != null)
      {
        ObjectBinder<ExpandedChange> current = this.m_resultCollection.GetCurrent<ExpandedChange>();
        if (current.MoveNext())
          change = current.Current;
      }
      return change != null;
    }

    public List<ExpandedChange> GetFirstPage()
    {
      List<ExpandedChange> firstPage = new List<ExpandedChange>(this.m_expandedChanges.Count);
      foreach (ExpandedChange expandedChange in this.m_expandedChanges.Values)
        firstPage.Add(expandedChange);
      return firstPage;
    }

    public void Dispose()
    {
      if (this.m_vic == null)
        return;
      this.m_vic.Dispose();
      this.m_vic = (VersionedItemComponent) null;
    }
  }
}
