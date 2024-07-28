// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.GetVirtualMachinesResult
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal struct GetVirtualMachinesResult
  {
    private IEnumerable<VirtualMachine> m_virtualMachines;

    public IEnumerable<VirtualMachine> VirtualMachines
    {
      get
      {
        if (this.m_virtualMachines == null)
          this.m_virtualMachines = (IEnumerable<VirtualMachine>) Array.Empty<VirtualMachine>();
        return this.m_virtualMachines;
      }
      set => this.m_virtualMachines = value;
    }

    public VirtualMachineGroup VirtualMachineGroup { get; set; }
  }
}
