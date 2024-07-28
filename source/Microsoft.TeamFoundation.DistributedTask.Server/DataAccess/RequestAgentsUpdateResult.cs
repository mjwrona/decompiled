// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.RequestAgentsUpdateResult
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal sealed class RequestAgentsUpdateResult
  {
    private List<TaskAgent> m_newUpdates;
    private List<TaskAgent> m_existingUpdates;

    public List<TaskAgent> NewUpdates
    {
      get
      {
        if (this.m_newUpdates == null)
          this.m_newUpdates = new List<TaskAgent>();
        return this.m_newUpdates;
      }
    }

    public List<TaskAgent> ExistingUpdates
    {
      get
      {
        if (this.m_existingUpdates == null)
          this.m_existingUpdates = new List<TaskAgent>();
        return this.m_existingUpdates;
      }
    }
  }
}
