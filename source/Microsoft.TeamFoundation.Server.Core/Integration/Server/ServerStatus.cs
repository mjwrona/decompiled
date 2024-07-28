// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Integration.Server.ServerStatus
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Web.Services;

namespace Microsoft.TeamFoundation.Integration.Server
{
  [WebService(Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/ServerStatus/03", Description = "Azure DevOps Server Status web service")]
  [ClientService(ServiceName = "ServerStatus", CollectionServiceIdentifier = "d395630a-d784-45b9-b8d1-f4b82042a8d0")]
  public class ServerStatus : IntegrationWebService
  {
    [WebMethod]
    public DataChanged[] GetServerStatus()
    {
      try
      {
        this.EnterMethod(new MethodInformation(nameof (GetServerStatus), MethodType.Normal, EstimatedMethodCost.VeryLow));
        TeamFoundationTrace.Info(Microsoft.TeamFoundation.Framework.Server.ServerResources.SERVERSTATUS_ASMX_GETSERVERSTATUS_IN());
        this.RequestContext.GetService<IntegrationSecurityManager>().CheckGlobalPermission(this.RequestContext, "GENERIC_READ");
        return Array.Empty<DataChanged>();
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
    public string CheckAuthentication()
    {
      try
      {
        this.EnterMethod(new MethodInformation(nameof (CheckAuthentication), MethodType.LightWeight, EstimatedMethodCost.VeryLow));
        TeamFoundationTrace.Info(Microsoft.TeamFoundation.Framework.Server.ServerResources.SERVERSTATUS_ASMX_CHECKAUTHENTICATION_IN());
        this.RequestContext.GetService<IntegrationSecurityManager>().CheckGlobalPermission(this.RequestContext, "GENERIC_READ");
        return this.RequestContext.DomainUserName;
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        TeamFoundationTrace.Info(Microsoft.TeamFoundation.Framework.Server.ServerResources.SERVERSTATUS_ASMX_CHECKAUTHENTICATION_OUT());
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public string GetSupportedContractVersion()
    {
      try
      {
        this.EnterMethod(new MethodInformation(nameof (GetSupportedContractVersion), MethodType.LightWeight, EstimatedMethodCost.VeryLow));
        TeamFoundationTrace.Info(Microsoft.TeamFoundation.Framework.Server.ServerResources.SERVERSTATUS_ASMX_GETSUPPORTEDCONTRACTVERSION_IN());
        this.RequestContext.GetService<IntegrationSecurityManager>().CheckGlobalPermission(this.RequestContext, "GENERIC_READ");
        return TeamFoundationVersion.ContractVersionRange;
      }
      finally
      {
        TeamFoundationTrace.Info(Microsoft.TeamFoundation.Framework.Server.ServerResources.SERVERSTATUS_ASMX_GETSUPPORTEDCONTRACTVERSION_OUT());
        this.LeaveMethod();
      }
    }
  }
}
