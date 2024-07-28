// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.WebServices.ConnectedServicesWebService
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Web.Services;

namespace Microsoft.TeamFoundation.Server.Core.WebServices
{
  [WebService(Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Framework")]
  [ClientService(ComponentName = "Framework", RegistrationName = "Framework", ServerConfiguration = ServerConfiguration.TfsTeamProjectCollection, ServiceName = "ConnectedServicesService", CollectionServiceIdentifier = "DF24EDB3-CE89-4907-B1A2-9041F646121E")]
  public class ConnectedServicesWebService : FrameworkWebService
  {
    private TeamFoundationConnectedServicesService m_ConnectedServicesService;

    public ConnectedServicesWebService() => this.m_ConnectedServicesService = this.RequestContext.GetService<TeamFoundationConnectedServicesService>();

    [WebMethod]
    public ConnectedServiceMetadata CreateConnectedService(
      ConnectedServiceCreationData connectedServiceCreationData)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (CreateConnectedService), MethodType.LightWeight, EstimatedMethodCost.VeryLow);
        methodInformation.AddParameter(nameof (connectedServiceCreationData), (object) connectedServiceCreationData);
        this.EnterMethod(methodInformation);
        return this.m_ConnectedServicesService.CreateConnectedService(this.RequestContext, connectedServiceCreationData);
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
    public List<ConnectedServiceMetadata> QueryConnectedServices(string teamProject)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation("GetConnectedServices", MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter("projectId", (object) teamProject);
        this.EnterMethod(methodInformation);
        List<ConnectedServiceMetadata> connectedServiceMetadataList = new List<ConnectedServiceMetadata>();
        foreach (ConnectedServiceMetadata connectedService in this.m_ConnectedServicesService.QueryConnectedServices(this.RequestContext, teamProject))
        {
          if (connectedService.Kind.Equals((object) ConnectedServiceKind.AzureSubscription))
            connectedServiceMetadataList.Add(connectedService);
        }
        return connectedServiceMetadataList;
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
    public bool DoesConnectedServiceExist(string name, string teamProject)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (DoesConnectedServiceExist), MethodType.LightWeight, EstimatedMethodCost.VeryLow);
        methodInformation.AddParameter(nameof (name), (object) name);
        methodInformation.AddParameter(nameof (teamProject), (object) teamProject);
        this.EnterMethod(methodInformation);
        return this.m_ConnectedServicesService.DoesConnectedServiceExist(this.RequestContext, name, teamProject);
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
    public ConnectedService GetConnectedService(string name, string teamProject)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (GetConnectedService), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (name), (object) name);
        methodInformation.AddParameter(nameof (teamProject), (object) teamProject);
        this.EnterMethod(methodInformation);
        return this.m_ConnectedServicesService.GetConnectedService(this.RequestContext, name, teamProject);
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
    public void DeleteConnectedService(string name, string teamProject)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (DeleteConnectedService), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (name), (object) name);
        methodInformation.AddParameter("projectId", (object) teamProject);
        this.EnterMethod(methodInformation);
        this.m_ConnectedServicesService.DeleteConnectedService(this.RequestContext, name, teamProject);
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
