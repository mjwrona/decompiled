// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.ContextChangedEventArgs
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Client
{
  public class ContextChangedEventArgs : EventArgs
  {
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ContextChangedEventArgs(
      ITeamFoundationContext oldContext,
      ITeamFoundationContext newContext,
      bool collectionChanged,
      bool teamProjectChanged,
      bool teamChanged,
      bool repositoryChanged)
    {
      this.NewContext = newContext;
      this.OldContext = oldContext;
      this.TeamProjectCollectionChanged = collectionChanged;
      this.TeamProjectChanged = teamProjectChanged;
      this.TeamChanged = teamChanged;
      this.RepositoryChanged = repositoryChanged;
    }

    public ITeamFoundationContext NewContext { get; private set; }

    public ITeamFoundationContext OldContext { get; private set; }

    public bool TeamProjectCollectionChanged { get; private set; }

    public bool TeamProjectChanged { get; private set; }

    public bool TeamChanged { get; private set; }

    public bool RepositoryChanged { get; private set; }
  }
}
