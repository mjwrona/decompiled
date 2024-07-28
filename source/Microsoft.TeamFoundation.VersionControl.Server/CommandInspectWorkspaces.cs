// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CommandInspectWorkspaces
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class CommandInspectWorkspaces : VersionControlCommand
  {
    private List<InspectWorkspaceInfo> m_Workspaces;

    public CommandInspectWorkspaces(VersionControlRequestContext context)
      : base(context)
    {
    }

    public void Execute()
    {
      this.m_Workspaces = new List<InspectWorkspaceInfo>();
      using (VersionedItemComponent versionedItemComponent = this.m_versionControlRequestContext.VersionControlService.GetVersionedItemComponent(this.m_versionControlRequestContext))
      {
        ResultCollection resultCollection = versionedItemComponent.InspectWorkspaces();
        ObjectBinder<InspectWorkspaceInfo> current = resultCollection.GetCurrent<InspectWorkspaceInfo>();
        while (current.MoveNext())
        {
          this.m_Workspaces.Add(current.Current);
          resultCollection.IncrementRowCounter();
        }
      }
      StreamingCollection<InspectWorkspaceInfo> streamingCollection = new StreamingCollection<InspectWorkspaceInfo>((Command) this)
      {
        HandleExceptions = false
      };
      foreach (InspectWorkspaceInfo workspace in this.m_Workspaces)
      {
        string identityName;
        string displayName;
        this.m_versionControlRequestContext.VersionControlService.SecurityWrapper.FindIdentityNames(this.m_versionControlRequestContext.RequestContext, workspace.OwnerId, out identityName, out displayName);
        workspace.Owner = identityName;
        workspace.OwnerDisplayName = displayName;
        streamingCollection.Enqueue(workspace);
      }
      streamingCollection.IsComplete = true;
      if (!this.IsCacheFull)
        return;
      this.RequestContext.PartialResultsReady();
    }

    protected override void Dispose(bool disposing)
    {
      int num = disposing ? 1 : 0;
    }

    public List<InspectWorkspaceInfo> Workspaces => this.m_Workspaces;
  }
}
