// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ItemsToUndo
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class ItemsToUndo
  {
    private HashSet<string> m_teamProjectsNotFoundMessages;
    private List<Failure> m_failures;
    private VersionedItemComponent m_db;
    private VersionControlRequestContext m_requestContext;
    private bool m_hasWorkspacePermission;
    private ItemSpec[] m_items;
    private bool m_maskLocalWorkspaces;
    private Workspace m_workspace;
    private List<string> m_firstPage;
    private List<ItemPathPair> m_totalUndoList;
    private List<ItemSpec> m_processBatchList;

    public ItemsToUndo(
      VersionControlRequestContext requestContext,
      ItemSpec[] items,
      VersionedItemComponent db,
      Workspace workspace,
      List<Failure> failures,
      HashSet<string> teamProjectsNotFound,
      bool maskLocalWorkspaces)
    {
      this.m_requestContext = requestContext;
      this.m_items = items;
      this.m_db = db;
      this.m_failures = failures;
      this.m_teamProjectsNotFoundMessages = teamProjectsNotFound;
      this.m_workspace = workspace;
      this.m_hasWorkspacePermission = this.m_requestContext.VersionControlService.SecurityWrapper.HasWorkspacePermission(this.m_requestContext, 2, workspace);
      this.m_maskLocalWorkspaces = maskLocalWorkspaces;
      this.m_firstPage = new List<string>(items.Length);
    }

    public List<ItemPathPair> ToList()
    {
      if (this.m_totalUndoList == null)
        this.Populate();
      return this.m_totalUndoList;
    }

    private void Populate()
    {
      this.m_totalUndoList = new List<ItemPathPair>(this.m_items.Length);
      this.m_processBatchList = new List<ItemSpec>();
      foreach (ItemSpec itemSpec in this.m_items)
      {
        if (itemSpec.RecursionType == RecursionType.None && itemSpec.isServerItem)
          this.AddToUndoList(itemSpec.ItemPathPair);
        else
          this.m_processBatchList.Add(itemSpec);
      }
      this.ConvertToServerPaths(this.m_processBatchList);
      if (this.m_firstPage.Count > 0)
        this.m_requestContext.RequestContext.GetService<ITeamFoundationEventService>().PublishDecisionPoint(this.m_requestContext.RequestContext, (object) new UndoPendingChangesNotification(this.m_requestContext.RequestContext.ContextId, this.m_requestContext.RequestContext.GetUserIdentity(), this.m_workspace.Name, this.m_workspace.Owner, this.m_workspace.Computer, this.m_firstPage)
        {
          HasAllItems = (this.m_firstPage.Count == this.m_totalUndoList.Count)
        });
      if (this.m_processBatchList.Count <= 200)
        return;
      this.m_requestContext.RequestContext.Trace(700284, TraceLevel.Info, TraceArea.Undo, TraceLayer.Command, "Avoided :" + this.m_processBatchList.Count.ToString() + "prc_qpc calls");
    }

    private void AddToUndoList(ItemPathPair item)
    {
      try
      {
        if (!this.m_hasWorkspacePermission)
          this.m_requestContext.VersionControlService.SecurityWrapper.CheckItemPermission(this.m_requestContext, VersionedItemPermissions.UndoOther, item);
        if (this.m_firstPage.Count < this.m_requestContext.VersionControlService.GetXmlParameterChunkThreshold(this.m_requestContext))
          this.m_firstPage.Add(item.ProjectNamePath);
        this.m_totalUndoList.Add(item);
      }
      catch (TeamProjectNotFoundException ex)
      {
        this.m_requestContext.RequestContext.TraceException(700281, TraceLevel.Info, TraceArea.Undo, TraceLayer.Command, (Exception) ex);
        if (this.m_teamProjectsNotFoundMessages.Contains(ex.Message))
          return;
        this.m_failures.Add(new Failure((Exception) ex));
        this.m_teamProjectsNotFoundMessages.Add(ex.Message);
      }
      catch (ApplicationException ex)
      {
        this.m_requestContext.RequestContext.TraceException(700067, TraceLevel.Info, TraceArea.Undo, TraceLayer.Command, (Exception) ex);
        this.m_failures.Add(new Failure((Exception) ex, item.ProjectNamePath, RequestType.None));
      }
    }

    private void ConvertToServerPaths(List<ItemSpec> itemSpecs)
    {
      if (itemSpecs.Count == 0)
        return;
      bool[] flagArray = new bool[itemSpecs.Count];
      ResultCollection resultCollection = this.m_db.QueryPendingChanges(this.m_workspace, this.m_workspace.Name, this.m_workspace.OwnerId, 0, PendingSetType.Workspace, itemSpecs.ToArray(), (string) null, this.m_maskLocalWorkspaces, this.m_requestContext.MaxSupportedServerPathLength);
      int index1 = 0;
      if (resultCollection != null)
      {
        try
        {
          ObjectBinder<PendingChange> current1 = resultCollection.GetCurrent<PendingChange>();
          while (current1.MoveNext())
          {
            PendingChange current2 = current1.Current;
            index1 = current2.InputIndex;
            flagArray[index1] = true;
            if (itemSpecs[index1].postMatch(current2.ServerItem))
              this.AddToUndoList(current2.ItemPathPair);
          }
        }
        catch (TeamProjectNotFoundException ex)
        {
          this.m_requestContext.RequestContext.TraceException(700280, TraceLevel.Info, TraceArea.Undo, TraceLayer.Command, (Exception) ex);
          if (!this.m_teamProjectsNotFoundMessages.Contains(ex.Message))
          {
            this.m_failures.Add(new Failure((Exception) ex));
            this.m_teamProjectsNotFoundMessages.Add(ex.Message);
          }
        }
        catch (ApplicationException ex)
        {
          this.m_requestContext.RequestContext.TraceException(700066, TraceLevel.Info, TraceArea.Undo, TraceLayer.Command, (Exception) ex);
          this.m_failures.Add(new Failure((Exception) ex, itemSpecs[index1].Item, RequestType.None));
        }
        finally
        {
          resultCollection.Dispose();
        }
      }
      for (int index2 = 0; index2 < flagArray.Length; ++index2)
      {
        if (!flagArray[index2])
          this.m_failures.Add(new Failure((Exception) new ItemNotCheckedOutException("NoPendingChanges", itemSpecs[index2].Item)));
      }
    }
  }
}
