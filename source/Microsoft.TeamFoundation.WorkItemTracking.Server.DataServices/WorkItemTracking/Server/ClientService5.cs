// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ClientService5
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 929F0284-16B2-4277-9F4A-B615689A77D1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [WebService(Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03", Description = "Team Foundation WorkItemTracking ClientService web service")]
  [ClientService(ServiceName = "WorkitemService5", CollectionServiceIdentifier = "4C5EB288-4C0A-4888-BB1B-742A4B5B706E")]
  [ProxyParentClass("ClientService4", IgnoreInheritedMethods = true)]
  public class ClientService5 : ClientService4
  {
    public ClientService5()
      : base(5)
    {
    }

    public ClientService5(int clientVersion)
      : base(clientVersion)
    {
    }

    [WebMethod]
    [SoapHeader("requestHeader", Direction = SoapHeaderDirection.In)]
    public virtual ConstantRecord[] GetConstantRecords(
      string[] searchValues,
      ConstantRecordSearchFactor searchFactor)
    {
      this.CheckAndBlockWitSoapAccess();
      ConstantRecord[] constantRecords = (ConstantRecord[]) null;
      MethodInformation methodInformation = new MethodInformation(nameof (GetConstantRecords), MethodType.Normal, EstimatedMethodCost.Low);
      methodInformation.AddArrayParameter<string>(nameof (searchValues), (IList<string>) searchValues);
      methodInformation.AddParameter(nameof (searchFactor), (object) searchFactor);
      this.ExecuteWebMethod(methodInformation, nameof (ClientService5), 900380, 900381, AccessIntent.Read, (Action) (() =>
      {
        ArgumentUtility.CheckForNull<string[]>(searchValues, nameof (searchValues));
        if (searchValues.Length == 0)
        {
          constantRecords = Array.Empty<ConstantRecord>();
        }
        else
        {
          if (!this.User.Identity.IsAuthenticated)
            throw new SoapException(ResourceStrings.Get("BadServerConfig"), Soap12FaultCodes.ReceiverFaultCode, WorkItemTrackingFaultCodes.BadServerConfig);
          constantRecords = new DataAccessLayerImpl(this.RequestContext).GetConstantRecords(searchValues, searchFactor);
        }
      }));
      return constantRecords;
    }

    public override void PageItemsOnBehalfOf(
      string userName,
      int[] ids,
      string[] columns,
      out Payload items)
    {
      throw new NotImplementedException();
    }

    public override void QueryWorkitemCountOnBehalfOf(
      string userName,
      XmlElement query,
      out int count)
    {
      throw new NotImplementedException();
    }

    [WebMethod]
    [SoapHeader("requestHeader", Direction = SoapHeaderDirection.In)]
    public virtual void DestroyAttachments(int[] workItemIds)
    {
      this.CheckAndBlockWitSoapAccess();
      MethodInformation methodInformation = new MethodInformation(nameof (DestroyAttachments), MethodType.Normal, EstimatedMethodCost.Moderate);
      methodInformation.AddArrayParameter<int>(nameof (workItemIds), (IList<int>) workItemIds);
      this.ExecuteWebMethod(methodInformation, nameof (ClientService5), 900664, 900665, AccessIntent.Write, (Action) (() =>
      {
        ArgumentUtility.CheckForNull<int[]>(workItemIds, nameof (workItemIds));
        if (!this.User.Identity.IsAuthenticated)
          throw new SoapException(ResourceStrings.Get("BadServerConfig"), Soap12FaultCodes.ReceiverFaultCode, WorkItemTrackingFaultCodes.BadServerConfig);
        if (workItemIds.Length == 0)
          return;
        new DataAccessLayerImpl(this.RequestContext).DestroyAttachments((IEnumerable<int>) workItemIds);
      }));
    }
  }
}
