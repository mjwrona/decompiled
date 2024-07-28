// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.OnAddServerEventArgs
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;

namespace Microsoft.TeamFoundation.Client
{
  internal class OnAddServerEventArgs : EventArgs
  {
    private TfsConnection _tfs;
    private bool _cancel;

    public OnAddServerEventArgs(TfsConnection tfs)
    {
      this._tfs = tfs;
      this._cancel = false;
    }

    public TfsConnection Server => this._tfs;

    public bool Cancel
    {
      set => this._cancel = value;
      get => this._cancel;
    }
  }
}
