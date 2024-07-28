// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.ContextChangingEventArgs
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Client
{
  [CLSCompliant(false)]
  public abstract class ContextChangingEventArgs : EventArgs
  {
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ContextChangingEventArgs(ITeamFoundationContext newContext, bool collectionChanging)
    {
      this.Cancel = false;
      this.NewContext = newContext;
      this.TeamProjectCollectionChanging = collectionChanging;
    }

    public bool Cancel { get; set; }

    public ITeamFoundationContext NewContext { get; private set; }

    public bool TeamProjectCollectionChanging { get; private set; }

    public abstract void AddHierarchyForPromptAndSave(
      object hierarchy,
      uint parentItemId,
      bool autoClose);
  }
}
