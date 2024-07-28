// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.Admin
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Web.Services;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [WebService(Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/VersionControl/Admin/03", Description = "DevOps VersionControl Admin web service")]
  [ProxyParentClass("VersionControlClientProxy")]
  [ClientService(ServiceName = "ISCCAdmin", CollectionServiceIdentifier = "0ade2b5a-efa4-419e-bf11-24f7cfe7c1a2")]
  public class Admin : VersionControlWebService
  {
    [WebMethod]
    public AdminRepositoryInfo QueryRepositoryInformation()
    {
      try
      {
        this.EnterMethod(new MethodInformation(nameof (QueryRepositoryInformation), MethodType.Admin, EstimatedMethodCost.Moderate));
        return this.VersionControlService.QueryRepositoryInformation(this.RequestContext);
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
    public void OptimizeDatabase(int optimizationFlags)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (OptimizeDatabase), MethodType.Admin, EstimatedMethodCost.VeryHigh);
        methodInformation.AddParameter(nameof (optimizationFlags), (object) optimizationFlags);
        this.EnterMethod(methodInformation);
        this.VersionControlService.OptimizeDatabase(this.RequestContext);
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
    public void GenerateRepositoryKey()
    {
      try
      {
        this.EnterMethod(new MethodInformation(nameof (GenerateRepositoryKey), MethodType.Admin, EstimatedMethodCost.VeryLow));
        this.VersionControlService.GenerateRepositoryKey(this.RequestContext);
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
