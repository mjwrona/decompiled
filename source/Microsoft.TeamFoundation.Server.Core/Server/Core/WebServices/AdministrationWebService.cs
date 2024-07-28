// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.WebServices.AdministrationWebService
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Web.Services;

namespace Microsoft.TeamFoundation.Server.Core.WebServices
{
  [WebService(Namespace = "http://microsoft.com/webservices/")]
  [ClientService(ComponentName = "Framework", RegistrationName = "Framework", ServerConfiguration = ServerConfiguration.TfsConfigurationServer, ServiceName = "AdministrationService", ConfigurationServiceIdentifier = "C18D6E34-68E8-40D2-A619-E7477558976E")]
  public class AdministrationWebService : FrameworkWebService
  {
    private SecuredHostManager m_hostManager;

    public AdministrationWebService()
    {
      this.CheckOnPremises();
      this.m_hostManager = this.RequestContext.GetService<SecuredHostManager>();
    }

    [WebMethod]
    public void CancelRequest(Guid hostId, long requestId, string reason)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (CancelRequest), MethodType.Admin, EstimatedMethodCost.Free);
        methodInformation.AddParameter(nameof (hostId), (object) hostId);
        methodInformation.AddParameter(nameof (requestId), (object) requestId);
        methodInformation.AddParameter(nameof (reason), (object) reason);
        this.EnterMethod(methodInformation);
        this.m_hostManager.CancelRequest(this.RequestContext, hostId, requestId, reason);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public List<TeamFoundationServiceHostActivity> QueryActiveRequests(
      [ClientType(typeof (IEnumerable<Guid>))] Guid[] hostIds,
      bool includeDetails)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryActiveRequests), MethodType.Admin, EstimatedMethodCost.Free);
        methodInformation.AddArrayParameter<Guid>(nameof (hostIds), (IList<Guid>) hostIds);
        methodInformation.AddParameter(nameof (includeDetails), (object) includeDetails);
        this.EnterMethod(methodInformation);
        return this.m_hostManager.QueryActiveRequests(this.RequestContext, hostIds, includeDetails);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }
  }
}
