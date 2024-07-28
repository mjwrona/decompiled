// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.DeferredQuery
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class DeferredQuery
  {
    protected DeletedState m_deletedState;
    protected Exception m_exception;
    protected string m_filePattern;
    protected ItemSpec m_itemSpec;
    protected ItemType m_itemType;
    protected string m_optimizationRoot;
    protected string m_queryPath;
    protected bool m_useMappings;
    protected VersionSpec m_versionSpec;
    protected Item m_result;
    private bool m_executed;
    protected int m_options;
    private VersionControlRequestContext m_versionControlRequestContext;

    internal DeferredQuery(
      VersionControlRequestContext versionControlRequestContext,
      ItemSpec itemSpec,
      DeletedState deletedState,
      ItemType itemType,
      VersionSpec versionSpec,
      bool isMapped,
      int options)
    {
      this.m_versionControlRequestContext = versionControlRequestContext;
      this.m_itemSpec = itemSpec;
      this.m_itemType = itemType;
      this.m_deletedState = deletedState;
      this.m_versionSpec = versionSpec;
      this.m_useMappings = isMapped;
      this.m_options = options;
      if (this.m_itemSpec.isServerItem | this.m_versionSpec is WorkspaceVersionSpec && !this.m_itemSpec.isWildcard && this.m_itemSpec.RecursionType == RecursionType.None)
      {
        if (itemSpec.isServerItem)
          this.m_optimizationRoot = VersionControlPath.GetFolderName(itemSpec.Item);
        else
          this.m_optimizationRoot = FileSpec.GetDirectoryName(itemSpec.Item);
      }
      else
        this.m_optimizationRoot = (string) null;
    }

    internal static void ExecuteOptimizedQueries(
      List<DeferredQuery> queries,
      Workspace localWorkspace,
      VersionedItemComponent db)
    {
      foreach (DeferredQuery optimizeQuery in DeferredQuery.OptimizeQueries(new List<DeferredQuery>((IEnumerable<DeferredQuery>) queries)))
        optimizeQuery.Execute(localWorkspace, db);
    }

    private static List<OptimizedQuery> OptimizeQueries(List<DeferredQuery> queries)
    {
      DeferredQuery.OptimizationComparer optimizationComparer = new DeferredQuery.OptimizationComparer();
      int index = 0;
      List<OptimizedQuery> optimizedQueryList = new List<OptimizedQuery>();
      queries.Sort((IComparer<DeferredQuery>) optimizationComparer);
      for (; index < queries.Count; ++index)
      {
        if (queries[index].m_optimizationRoot != null)
        {
          int count = 1;
          while (index + count < queries.Count && optimizationComparer.Compare(queries[index], queries[index + count]) == 0)
            ++count;
          if (count > 1)
          {
            DeferredQuery[] deferredQueryArray = new DeferredQuery[count];
            queries.CopyTo(index, deferredQueryArray, 0, count);
            OptimizedQuery optimizedQuery = new OptimizedQuery(deferredQueryArray[0].m_versionControlRequestContext, deferredQueryArray);
            queries.RemoveRange(index + 1, count - 1);
            optimizedQueryList.Add(optimizedQuery);
          }
        }
      }
      return optimizedQueryList;
    }

    internal virtual void Execute(Workspace localWorkspace, VersionedItemComponent db)
    {
      try
      {
        this.m_executed = true;
        this.m_versionSpec.UseMappings = this.m_useMappings;
        this.m_versionSpec.QueryItems(this.m_versionControlRequestContext, this.m_itemSpec, localWorkspace, db, this.m_deletedState, this.m_itemType, out this.m_queryPath, out this.m_filePattern, this.m_options);
      }
      catch (RequestCanceledException ex)
      {
        throw;
      }
      catch (ApplicationException ex)
      {
        this.m_versionControlRequestContext.RequestContext.TraceException(700074, TraceLevel.Info, TraceArea.QueryItems, TraceLayer.BusinessLogic, (Exception) ex);
        this.m_exception = (Exception) ex;
      }
    }

    internal bool Match(Item item)
    {
      if (!item.MatchDeletedState(this.m_deletedState) || this.m_itemType != ItemType.Any && item.ItemType != this.m_itemType)
        return false;
      if (this.m_itemSpec.isServerItem)
      {
        if (!VersionControlPath.Equals(this.m_itemSpec.Item, item.ServerItem))
          return false;
      }
      else if (!FileSpec.Equals(((WorkspaceItem) item).LocalItem, this.m_itemSpec.Item))
        return false;
      return true;
    }

    internal DeletedState DeletedState => this.m_deletedState;

    internal string OptimizationRoot => this.m_optimizationRoot;

    internal ItemType ItemType => this.m_itemType;

    internal VersionSpec VersionSpec => this.m_versionSpec;

    internal bool UseMappings => this.m_useMappings;

    internal ItemSpec ItemSpec => this.m_itemSpec;

    internal int Options => this.m_options;

    internal virtual bool TryGetNextItem(out Item item)
    {
      item = (Item) null;
      bool nextItem;
      if (this.m_result != null)
      {
        nextItem = true;
        item = this.m_result;
        this.m_result = (Item) null;
      }
      else
        nextItem = this.m_versionSpec.TryGetNextItem(out item);
      return nextItem;
    }

    internal string QueryPath => this.m_queryPath;

    internal string FilePattern => this.m_filePattern;

    internal Exception Exception => this.m_exception;

    internal bool Executed => this.m_executed;

    internal void ProcessResult(Item result)
    {
      this.m_result = result;
      this.m_executed = true;
      this.m_queryPath = result.ServerItem;
      this.m_filePattern = (string) null;
    }

    internal void ProcessException(Exception e)
    {
      this.m_exception = e;
      this.m_executed = true;
    }

    public virtual void Reset()
    {
      if (this.m_result != null)
        return;
      this.m_executed = false;
    }

    private class OptimizationComparer : IComparer<DeferredQuery>
    {
      public int Compare(DeferredQuery x, DeferredQuery y)
      {
        int num = TFStringComparer.VersionControlPath.Compare(x.m_optimizationRoot, y.m_optimizationRoot);
        if (num == 0)
          num = x.m_versionSpec.CompareTo(y.m_versionSpec);
        return num;
      }
    }
  }
}
