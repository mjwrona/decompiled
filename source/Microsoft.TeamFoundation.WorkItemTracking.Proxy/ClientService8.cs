// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Proxy.ClientService8
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Proxy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF15D8B4-8AC0-4915-8153-9054E8546EA2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Proxy.dll

using Microsoft.TeamFoundation.Client;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Proxy
{
  internal class ClientService8 : ClientService7
  {
    public ClientService8(TfsTeamProjectCollection connection)
      : base(connection)
    {
    }

    protected override Guid CollectionServiceIdentifier => new Guid("1cc519db-7813-49eb-8db5-04003dd776e8");

    protected override string ServiceType => "WorkitemService8";
  }
}
