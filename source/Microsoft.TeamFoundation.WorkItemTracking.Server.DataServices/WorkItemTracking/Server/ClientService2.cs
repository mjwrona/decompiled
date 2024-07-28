// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ClientService2
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 929F0284-16B2-4277-9F4A-B615689A77D1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Web.Services;
using System.Web.Services.Protocols;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [WebService(Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03", Description = "Team Foundation WorkItemTracking ClientService web service")]
  [ClientService(ServiceName = "WorkitemService2", CollectionServiceIdentifier = "7EDE8C17-7965-4AEE-874D-ED9B25276DEB")]
  [ProxyParentClass("ClientService", IgnoreInheritedMethods = true)]
  public class ClientService2 : ClientService
  {
    public ClientService2()
      : base(2)
    {
    }

    public ClientService2(int clientVersion)
      : base(clientVersion)
    {
    }

    [WebMethod]
    [SoapHeader("requestHeader", Direction = SoapHeaderDirection.In)]
    public virtual void GetStoredQueryItems(
      long rowVersion,
      int projectId,
      out Payload queryItemsPayload)
    {
      this.CheckAndBlockWitSoapAccess();
      Payload queryItemsPayloadTemp = queryItemsPayload = (Payload) null;
      MethodInformation methodInformation = new MethodInformation(nameof (GetStoredQueryItems), MethodType.Normal, EstimatedMethodCost.Low);
      methodInformation.AddParameter(nameof (rowVersion), (object) rowVersion);
      methodInformation.AddParameter(nameof (projectId), (object) projectId);
      this.ExecuteWebMethod(methodInformation, nameof (ClientService2), 900372, 900638, AccessIntent.Read, (Action) (() =>
      {
        queryItemsPayloadTemp = new Payload();
        this.GetQueries(projectId, true, queryItemsPayloadTemp);
      }));
      queryItemsPayload = queryItemsPayloadTemp;
    }
  }
}
