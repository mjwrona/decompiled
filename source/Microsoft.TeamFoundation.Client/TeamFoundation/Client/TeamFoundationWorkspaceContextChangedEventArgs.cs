// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.TeamFoundationWorkspaceContextChangedEventArgs
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class TeamFoundationWorkspaceContextChangedEventArgs : EventArgs
  {
    public TeamFoundationWorkspaceContextChangedEventArgs(string oldWorkspace, string newWorkspace)
    {
      this.OldWorkspace = oldWorkspace;
      this.NewWorkspace = newWorkspace;
    }

    public string OldWorkspace { get; set; }

    public string NewWorkspace { get; set; }
  }
}
