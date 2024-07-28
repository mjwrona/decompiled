// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ItemsToUnshelveEnumerator
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class ItemsToUnshelveEnumerator : 
    IEnumerable<ItemPathPair>,
    IEnumerable,
    IEnumerator<ItemPathPair>,
    IDisposable,
    IEnumerator
  {
    private int m_currentIndex;
    private int m_itemSpecCounter;
    private string m_shelvesetName;
    private Guid m_ownerId;
    private List<ItemsToUnshelveEnumerator.ItemSpecWithServerItem> m_serverItems;
    private List<Failure> m_failures;
    private ItemPathPair m_currentItem;
    private VersionedItemComponent m_db;
    private ResultCollection m_rc;
    private VersionControlRequestContext m_requestContext;

    public ItemsToUnshelveEnumerator(
      VersionControlRequestContext requestContext,
      ItemSpec[] items,
      Workspace workspace,
      string shelvesetName,
      Guid ownerId,
      List<Failure> failures)
    {
      this.m_requestContext = requestContext;
      this.m_shelvesetName = shelvesetName;
      this.m_ownerId = ownerId;
      this.m_serverItems = new List<ItemsToUnshelveEnumerator.ItemSpecWithServerItem>();
      this.m_failures = failures;
      if (items == null)
        return;
      foreach (ItemSpec itemSpec in items)
      {
        try
        {
          this.m_serverItems.Add(new ItemsToUnshelveEnumerator.ItemSpecWithServerItem()
          {
            ItemSpec = itemSpec,
            ItemPathPair = itemSpec.toServerItem(requestContext.RequestContext, workspace, true)
          });
        }
        catch (RequestCanceledException ex)
        {
          throw;
        }
        catch (Exception ex)
        {
          this.m_requestContext.RequestContext.TraceException(700072, TraceArea.Unshelve, TraceLayer.Command, ex);
          this.m_failures.Add(new Failure(ex));
        }
      }
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    public IEnumerator<ItemPathPair> GetEnumerator() => (IEnumerator<ItemPathPair>) this;

    public void Reset()
    {
      this.m_currentIndex = 0;
      if (this.m_rc == null)
        return;
      this.m_rc.Dispose();
      this.m_rc = (ResultCollection) null;
    }

    public bool MoveNext()
    {
      this.m_currentItem = new ItemPathPair();
      while (this.m_currentIndex < this.m_serverItems.Count)
      {
        ItemPathPair itemPathPair = this.m_serverItems[this.m_currentIndex].ItemPathPair;
        ItemSpec itemSpec = this.m_serverItems[this.m_currentIndex].ItemSpec;
        try
        {
          if (this.m_rc != null)
          {
            ObjectBinder<PendingChange> current1 = this.m_rc.GetCurrent<PendingChange>();
            while (current1.MoveNext())
            {
              PendingChange current2 = current1.Current;
              if (itemSpec.postMatch(current2.ServerItem))
              {
                this.m_currentItem = current2.ItemPathPair;
                ++this.m_itemSpecCounter;
                return true;
              }
            }
            if (this.m_itemSpecCounter == 0)
              this.m_failures.Add(new Failure((Exception) new ShelvedChangeNotFoundException(itemPathPair.ProjectNamePath)));
            this.m_rc.Dispose();
            this.m_rc = (ResultCollection) null;
            ++this.m_currentIndex;
            this.m_itemSpecCounter = 0;
          }
          else
          {
            if (itemSpec.RecursionType == RecursionType.None)
            {
              ++this.m_currentIndex;
              this.m_currentItem = itemPathPair;
              return true;
            }
            if (this.m_db == null)
              this.m_db = this.m_requestContext.VersionControlService.GetVersionedItemComponent(this.m_requestContext);
            this.m_rc = this.m_db.QueryPendingChanges((Workspace) null, this.m_shelvesetName, this.m_ownerId, 0, PendingSetType.Shelveset, new ItemSpec[1]
            {
              new ItemSpec(itemPathPair, itemSpec.RecursionType)
            }, (string) null, false, this.m_requestContext.MaxSupportedServerPathLength);
          }
        }
        catch (RequestCanceledException ex)
        {
          this.CloseConnection();
          throw;
        }
        catch (ShelvesetNotFoundException ex)
        {
          this.CloseConnection();
          throw;
        }
        catch (Exception ex)
        {
          this.m_requestContext.RequestContext.TraceException(700073, TraceArea.Unshelve, TraceLayer.Command, ex);
          this.m_failures.Add(new Failure(ex));
          this.m_rc = (ResultCollection) null;
          ++this.m_currentIndex;
        }
      }
      this.CloseConnection();
      if (this.m_failures.Count > 0)
        throw new AbortStreamingParameterException();
      return false;
    }

    object IEnumerator.Current => (object) this.Current;

    public ItemPathPair Current => this.m_currentItem;

    public void Dispose() => this.CloseConnection();

    private void CloseConnection()
    {
      if (this.m_db != null)
      {
        this.m_db.Dispose();
        this.m_db = (VersionedItemComponent) null;
      }
      if (this.m_rc == null)
        return;
      this.m_rc.Dispose();
      this.m_rc = (ResultCollection) null;
    }

    public List<string> ServerItems
    {
      get
      {
        List<string> serverItems = new List<string>(this.m_serverItems.Count);
        foreach (ItemsToUnshelveEnumerator.ItemSpecWithServerItem serverItem in this.m_serverItems)
          serverItems.Add(serverItem.ItemPathPair.ProjectNamePath);
        return serverItems;
      }
    }

    public List<ItemPathPair> Items
    {
      get
      {
        List<ItemPathPair> items = new List<ItemPathPair>(this.m_serverItems.Count);
        foreach (ItemsToUnshelveEnumerator.ItemSpecWithServerItem serverItem in this.m_serverItems)
          items.Add(serverItem.ItemPathPair);
        return items;
      }
    }

    private class ItemSpecWithServerItem
    {
      public ItemSpec ItemSpec;
      public ItemPathPair ItemPathPair;
    }
  }
}
