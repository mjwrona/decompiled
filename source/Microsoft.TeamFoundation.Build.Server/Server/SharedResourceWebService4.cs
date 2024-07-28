// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.SharedResourceWebService4
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Web.Services;

namespace Microsoft.TeamFoundation.Build.Server
{
  [ClientService(ComponentName = "TeamBuild", RegistrationName = "Build", ServiceName = "SharedResourceService4", CollectionServiceIdentifier = "078B3BB3-2FE6-4C32-B59B-7F009F075DEF")]
  [WebService(Name = "SharedResourceService", Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Build")]
  public sealed class SharedResourceWebService4 : BuildWebServiceBase
  {
    [WebMethod]
    public List<SharedResource> QuerySharedResources(string[] resourceNames)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QuerySharedResources), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<string>(nameof (resourceNames), (IList<string>) resourceNames);
        this.EnterMethod(methodInformation);
        return this.BuildResourceService.QuerySharedResources(this.RequestContext, (IList<string>) resourceNames);
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
    [ClientServiceMethod(AsyncPattern = true, SyncPattern = true)]
    public void RequestLock(
      string resourceName,
      string instanceId,
      [ClientType(typeof (Uri))] string requestedBy,
      [ClientType(typeof (Uri))] string buildUri,
      Guid buildProjectId)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (RequestLock), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (resourceName), (object) resourceName);
        methodInformation.AddParameter(nameof (instanceId), (object) instanceId);
        methodInformation.AddParameter(nameof (requestedBy), (object) requestedBy);
        methodInformation.AddParameter(nameof (buildUri), (object) buildUri);
        methodInformation.AddParameter(nameof (buildProjectId), (object) buildUri);
        this.EnterMethod(methodInformation);
        this.BuildResourceService.RequestSharedResourceLock(this.RequestContext, resourceName, instanceId, requestedBy, buildUri, buildProjectId, (string) null, Guid.Empty);
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
    [ClientServiceMethod(AsyncPattern = true, SyncPattern = true)]
    public bool TryRequestLock(
      string resourceName,
      string instanceId,
      [ClientType(typeof (Uri))] string requestedBy,
      [ClientType(typeof (Uri))] string buildUri,
      Guid buildProjectId)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (TryRequestLock), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (resourceName), (object) resourceName);
        methodInformation.AddParameter(nameof (instanceId), (object) instanceId);
        methodInformation.AddParameter(nameof (requestedBy), (object) requestedBy);
        methodInformation.AddParameter(nameof (buildUri), (object) buildUri);
        methodInformation.AddParameter(nameof (buildProjectId), (object) buildProjectId);
        this.EnterMethod(methodInformation);
        return this.BuildResourceService.TryRequestSharedResourceLock(this.RequestContext, resourceName, instanceId, requestedBy, buildUri, buildProjectId);
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
    [ClientServiceMethod(AsyncPattern = true, SyncPattern = true)]
    public void ReleaseLock(string resourceName, string instanceId, [ClientType(typeof (Uri))] string requestedBy)
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
    [ClientServiceMethod(SyncPattern = true)]
    public void ReleaseAllLocks([ClientType(typeof (Uri))] string requestedBy)
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
