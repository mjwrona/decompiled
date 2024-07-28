// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.PayloadConverter
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class PayloadConverter
  {
    private Dictionary<int, PayloadTableConverter> m_tableConverters = new Dictionary<int, PayloadTableConverter>();
    private List<ProcessColumnCallback> m_globalProcessColumnCallbacks;

    public PayloadTableConverter this[int index]
    {
      get => this.m_tableConverters.ContainsKey(index) ? this.m_tableConverters[index] : (PayloadTableConverter) null;
      set
      {
        this.m_tableConverters[index] = value;
        if (value == null)
          return;
        value.PayloadConverter = this;
      }
    }

    public void AddGlobalProcessColumnCallback(ProcessColumnCallback processColumnCallback)
    {
      if (this.m_globalProcessColumnCallbacks != null && this.m_globalProcessColumnCallbacks.Contains(processColumnCallback))
        throw new ArgumentException("processColumnCallback was already added to this payload");
      if (this.m_globalProcessColumnCallbacks == null)
        this.m_globalProcessColumnCallbacks = new List<ProcessColumnCallback>();
      this.m_globalProcessColumnCallbacks.Add(processColumnCallback);
    }

    internal void ExecuteGlobalProcessColumnCallbacks(PayloadTable table, PayloadColumn column)
    {
      if (table == null)
        throw new ArgumentNullException(nameof (table));
      if (column == null)
        throw new ArgumentNullException(nameof (column));
      if (this.m_globalProcessColumnCallbacks == null)
        return;
      foreach (ProcessColumnCallback processColumnCallback in this.m_globalProcessColumnCallbacks)
        processColumnCallback(table, column);
    }
  }
}
