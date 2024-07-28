// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemCoreFieldUpdatesRecord
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels
{
  internal class WorkItemCoreFieldUpdatesRecord
  {
    private IDictionary<int, object> m_container;

    public WorkItemCoreFieldUpdatesRecord(IDictionary<int, object> container) => this.m_container = container;

    public int Id
    {
      get => this.m_container.GetValueOrDefault<int>(-3);
      set => this.m_container[-3] = (object) value;
    }

    public int Revision
    {
      get => this.m_container.GetValueOrDefault<int>(8);
      set => this.m_container[8] = (object) value;
    }

    public string WorkItemType
    {
      get => this.m_container.GetValueOrDefault<string>(25);
      set => this.m_container[25] = (object) value;
    }

    public int AreaId
    {
      get => this.m_container.GetValueOrDefault<int>(-2);
      set => this.m_container[-2] = (object) value;
    }

    public int IterationId
    {
      get => this.m_container.GetValueOrDefault<int>(-104);
      set => this.m_container[-104] = (object) value;
    }

    public string CreatedBy
    {
      get => this.m_container.GetValueOrDefault<string>(33);
      set => this.m_container[33] = (object) value;
    }

    public DateTime CreatedDate
    {
      get => this.m_container.GetValueOrDefault<DateTime>(32);
      set => this.m_container[32] = (object) value;
    }

    public string ChangedBy
    {
      get => this.m_container.GetValueOrDefault<string>(9);
      set => this.m_container[9] = (object) value;
    }

    public DateTime ChangedDate
    {
      get => this.m_container.GetValueOrDefault<DateTime>(-4);
      set => this.m_container[-4] = (object) value;
    }

    public string State
    {
      get => this.m_container.GetValueOrDefault<string>(2);
      set => this.m_container[2] = (object) value;
    }

    public string Reason
    {
      get => this.m_container.GetValueOrDefault<string>(22);
      set => this.m_container[22] = (object) value;
    }

    public string AssignedTo
    {
      get => this.m_container.GetValueOrDefault<string>(24);
      set => this.m_container[24] = (object) value;
    }

    public Guid ProjectId
    {
      get => this.m_container.GetValue<Guid>(-42);
      set => this.m_container[-42] = (object) value;
    }

    public bool IsDeleted
    {
      get => this.m_container.GetValue<bool>(-404);
      set => this.m_container[-404] = (object) value;
    }
  }
}
