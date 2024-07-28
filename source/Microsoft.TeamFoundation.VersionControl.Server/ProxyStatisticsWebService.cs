// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ProxyStatisticsWebService
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Web.Services;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [WebService(Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/VersionControl/Statistics/03", Description = "DevOps VersionControl Proxy Statistics web service")]
  public class ProxyStatisticsWebService : VersionControlWebService
  {
    [WebMethod]
    public ProxyStatisticsInfo[] QueryProxyStatistics()
    {
      try
      {
        this.RequestContext.CheckOnPremisesDeployment();
        this.RequestContext.EnterMethod(new MethodInformation(nameof (QueryProxyStatistics), MethodType.Admin, EstimatedMethodCost.Low));
        return this.RequestContext.To(TeamFoundationHostType.Deployment).GetService<FileCacheService>().Statistics.Info;
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.RequestContext.LeaveMethod();
      }
    }
  }
}
