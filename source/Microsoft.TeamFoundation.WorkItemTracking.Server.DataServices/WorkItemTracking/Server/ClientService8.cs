// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ClientService8
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 929F0284-16B2-4277-9F4A-B615689A77D1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Web.Services;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [WebService(Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03", Description = "Team Foundation WorkItemTracking ClientService web service")]
  [ClientService(ServiceName = "WorkitemService8", CollectionServiceIdentifier = "1cc519db-7813-49eb-8db5-04003dd776e8")]
  [ProxyParentClass("ClientService7", IgnoreInheritedMethods = true)]
  public class ClientService8 : ClientService7
  {
    public ClientService8()
      : base(8)
    {
    }

    public ClientService8(int clientVersion)
      : base(clientVersion)
    {
    }
  }
}
