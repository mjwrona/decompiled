// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ClientService7
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 929F0284-16B2-4277-9F4A-B615689A77D1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Web.Services;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [WebService(Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03", Description = "Team Foundation WorkItemTracking ClientService web service")]
  [ClientService(ServiceName = "WorkitemService7", CollectionServiceIdentifier = "BC9B27AA-EDA2-4FC9-AC3B-644BB7999C19")]
  [ProxyParentClass("ClientService6", IgnoreInheritedMethods = true)]
  public class ClientService7 : ClientService6
  {
    public ClientService7()
      : base(7)
    {
    }

    public ClientService7(int clientVersion)
      : base(clientVersion)
    {
    }
  }
}
