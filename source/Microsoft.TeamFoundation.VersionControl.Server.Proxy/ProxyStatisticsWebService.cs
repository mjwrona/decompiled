// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Proxy.ProxyStatisticsWebService
// Assembly: Microsoft.TeamFoundation.VersionControl.Server.Proxy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3F3DC329-13F2-42E8-9562-94C7348523BA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.Proxy.dll

using System;
using System.Web.Services;

namespace Microsoft.TeamFoundation.Framework.Server.Proxy
{
  [WebService(Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/VersionControl/Statistics/03", Description = "DevOps VersionControl Proxy Statistics web service")]
  public class ProxyStatisticsWebService : ProxyWebService
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
