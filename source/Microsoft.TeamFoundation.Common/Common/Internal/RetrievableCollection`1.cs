// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.Internal.RetrievableCollection`1
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Common.Internal
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class RetrievableCollection<T> : ObservableCollection<T>
  {
    private Exception m_error;
    private string m_infoMessage;
    private RetrievablePropertyState m_state;

    public virtual RetrievablePropertyState State
    {
      get => this.m_state;
      protected set
      {
        if (this.m_state == value)
          return;
        this.m_state = value;
        this.OnPropertyChanged(new PropertyChangedEventArgs(nameof (State)));
      }
    }

    public Exception Error
    {
      get => this.m_error;
      set
      {
        this.m_error = value;
        if (value == null)
          return;
        this.State = RetrievablePropertyState.Error;
      }
    }

    public string InfoMessage
    {
      get => this.m_infoMessage;
      set
      {
        this.m_infoMessage = value;
        if (value == null)
          return;
        this.State = RetrievablePropertyState.Info;
      }
    }
  }
}
