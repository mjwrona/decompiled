// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.AutoModal
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class AutoModal : IDisposable
  {
    private bool m_fModal;

    public AutoModal() => this.m_fModal = UIHost.EnableModeless(false);

    void IDisposable.Dispose()
    {
      GC.SuppressFinalize((object) this);
      if (!this.m_fModal)
        return;
      this.m_fModal = false;
      UIHost.EnableModeless(true);
    }
  }
}
