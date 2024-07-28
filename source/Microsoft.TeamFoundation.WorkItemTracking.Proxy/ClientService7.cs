// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Proxy.ClientService7
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Proxy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF15D8B4-8AC0-4915-8153-9054E8546EA2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Proxy.dll

using Microsoft.TeamFoundation.Client;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Proxy
{
  internal class ClientService7 : ClientService6
  {
    public ClientService7(TfsTeamProjectCollection connection)
      : base(connection)
    {
    }

    protected override Guid CollectionServiceIdentifier => new Guid("BC9B27AA-EDA2-4FC9-AC3B-644BB7999C19");

    protected override string ServiceType => "WorkitemService7";
  }
}
