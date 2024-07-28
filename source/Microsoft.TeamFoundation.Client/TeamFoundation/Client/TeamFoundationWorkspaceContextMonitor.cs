// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.TeamFoundationWorkspaceContextMonitor
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class TeamFoundationWorkspaceContextMonitor : ITeamFoundationWorkspaceContextMonitor
  {
    private EventHandler<TeamFoundationWorkspaceContextChangedEventArgs> e_contextChanged;

    event EventHandler<TeamFoundationWorkspaceContextChangedEventArgs> ITeamFoundationWorkspaceContextMonitor.ContextChanged
    {
      add => this.e_contextChanged += value;
      remove => this.e_contextChanged -= value;
    }

    void ITeamFoundationWorkspaceContextMonitor.NotifyWorkspaceChanged(
      string oldWorkspace,
      string newWorkspace)
    {
      if (this.e_contextChanged == null)
        return;
      this.e_contextChanged((object) this, new TeamFoundationWorkspaceContextChangedEventArgs(oldWorkspace, newWorkspace));
    }
  }
}
