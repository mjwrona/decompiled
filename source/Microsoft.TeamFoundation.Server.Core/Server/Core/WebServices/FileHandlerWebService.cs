// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.WebServices.FileHandlerWebService
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Web.Services;

namespace Microsoft.TeamFoundation.Server.Core.WebServices
{
  [WebService(Namespace = "http://microsoft.com/webservices/")]
  [ClientService(ComponentName = "Framework", RegistrationName = "Framework", ServerConfiguration = ServerConfiguration.TfsConnection, ServiceName = "FileHandlerService", CollectionServiceIdentifier = "48850D87-0C57-4265-BC2B-812E445F73C6", ConfigurationServiceIdentifier = "48850D87-0C57-4265-BC2B-812E445F73C6")]
  public class FileHandlerWebService : FrameworkWebService
  {
    private TeamFoundationFileRepositoryService m_proxyService;

    public FileHandlerWebService()
    {
      if (!this.RequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        this.RequestContext.CheckOnPremisesDeployment(true);
      this.m_proxyService = this.RequestContext.GetService<TeamFoundationFileRepositoryService>();
    }

    [WebMethod]
    public FileRepositoryProperties QueryForRepositoryProperties()
    {
      try
      {
        this.EnterMethod(new MethodInformation(nameof (QueryForRepositoryProperties), MethodType.Admin, EstimatedMethodCost.Free));
        return this.m_proxyService.GetProxyProperties(this.RequestContext);
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
