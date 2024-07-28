// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.SharedResourceWebService
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Web.Services;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  [ClientService(ComponentName = "TeamBuild", RegistrationName = "Build", ServiceName = "SharedResourceService", CollectionServiceIdentifier = "1066C205-8F03-491b-B0D5-B6BB358D623E")]
  [WebService]
  public sealed class SharedResourceWebService : BuildWebServiceBase
  {
    [WebMethod]
    public void RequestLock(
      string resourceName,
      string instanceId,
      string requestedBy,
      [ClientType(typeof (Uri))] string buildUri)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (RequestLock), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (resourceName), (object) resourceName);
        methodInformation.AddParameter(nameof (instanceId), (object) instanceId);
        methodInformation.AddParameter(nameof (requestedBy), (object) requestedBy);
        methodInformation.AddParameter(nameof (buildUri), (object) buildUri);
        this.EnterMethod(methodInformation);
        string requestBuildUri = buildUri;
        if (!string.IsNullOrEmpty(buildUri))
        {
          List<string> uris = new List<string>((IEnumerable<string>) new string[1]
          {
            buildUri
          });
          RosarioHelper.ConvertBuildUris(this.RequestContext, (IList<string>) uris, false, false);
          buildUri = uris[0];
        }
        this.BuildResourceService.RequestSharedResourceLock(this.RequestContext, resourceName, instanceId, requestedBy, buildUri, Guid.Empty, requestBuildUri, Guid.Empty);
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(0, "BuildAdministration", "Service", ex);
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void ReleaseLock(string resourceName, string instanceId, string requestedBy)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (ReleaseLock), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (resourceName), (object) resourceName);
        methodInformation.AddParameter(nameof (instanceId), (object) instanceId);
        methodInformation.AddParameter(nameof (requestedBy), (object) requestedBy);
        this.EnterMethod(methodInformation);
        this.BuildResourceService.ReleaseSharedResourceLock(this.RequestContext, resourceName, instanceId, requestedBy);
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(0, "BuildAdministration", "Service", ex);
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void ReleaseAllLocks(string requestedBy)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (ReleaseAllLocks), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (requestedBy), (object) requestedBy);
        this.EnterMethod(methodInformation);
        this.BuildResourceService.ReleaseAllSharedResourceLocks(this.RequestContext, requestedBy);
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(0, "BuildAdministration", "Service", ex);
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }
  }
}
