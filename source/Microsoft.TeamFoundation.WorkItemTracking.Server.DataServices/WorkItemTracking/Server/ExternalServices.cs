// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalServices
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 929F0284-16B2-4277-9F4A-B615689A77D1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.Compatibility;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [WebService(Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ExternalServices/03", Description = "Team Foundation WorkItemTracking ExternalServices web service")]
  public class ExternalServices : WorkItemTrackingWebService
  {
    [WebMethod(MessageName = "GetWorkitemXml")]
    [SoapHeader("requestHeader", Direction = SoapHeaderDirection.In)]
    [Obsolete("This method has been deprecated and will be removed in a future release.")]
    public XmlElement GetWorkitemXml(int id, int rev)
    {
      this.CheckAndBlockWitSoapAccess();
      XmlDocument xmlData = (XmlDocument) null;
      MethodInformation methodInformation = new MethodInformation(nameof (GetWorkitemXml), MethodType.Normal, EstimatedMethodCost.Low);
      methodInformation.AddParameter(nameof (id), (object) id);
      methodInformation.AddParameter(nameof (rev), (object) rev);
      this.EnterMethod(methodInformation);
      string userLocale = HttpContext.Current.Request.ServerVariables["HTTP_ACCEPT_LANGUAGE"];
      IVssIdentity user = (IVssIdentity) this.RequestContext.GetUserIdentity();
      this.ExecuteWebMethod(methodInformation, nameof (ExternalServices), 900388, 900604, AccessIntent.Read, (Action) (() =>
      {
        if (!this.User.Identity.IsAuthenticated)
          throw new SoapException(ResourceStrings.Get("BadServerConfig"), Soap12FaultCodes.ReceiverFaultCode, WorkItemTrackingFaultCodes.BadServerConfig);
        xmlData = new WorkItemSerializer().Serialize(this.RequestContext, this.RequestContext.GetService<ITeamFoundationWorkItemService>().GetWorkItemById(this.RequestContext, id, includeInRecentActivity: true), rev, userLocale);
      }), (Action<Exception>) (e => ExceptionManager.HandleStaleViewsException(new DataAccessLayerImpl(this.RequestContext), this.RequestContext, (XmlElement) null, user, e)));
      return xmlData.DocumentElement;
    }
  }
}
