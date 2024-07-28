// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.BuildQueueWebService
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Web.Services;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  [ClientService(ComponentName = "TeamBuild", RegistrationName = "Build", ServiceName = "BuildQueueService", CollectionServiceIdentifier = "984f10cc-bc83-48fc-abe7-27053ca78f20")]
  [WebService]
  public class BuildQueueWebService : BuildWebServiceBase
  {
    [WebMethod]
    public void CancelBuilds(int[] queueIds)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (CancelBuilds), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<int>(nameof (queueIds), (IList<int>) queueIds);
        this.EnterMethod(methodInformation);
        this.BuildService.CancelBuilds(this.RequestContext, queueIds, new Guid());
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(0, "Build", "Service", ex);
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [ClientServiceMethod(AsyncPattern = true, SyncPattern = true)]
    public List<BuildQueueQueryResult2010> QueryBuilds(List<BuildQueueSpec2010> specs)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryBuilds), MethodType.Normal, EstimatedMethodCost.Low, false);
        methodInformation.AddArrayParameter<BuildQueueSpec2010>(nameof (specs), (IList<BuildQueueSpec2010>) specs);
        this.EnterMethod(methodInformation);
        return RosarioHelper.QueryQueuedBuilds(this.RequestContext, specs);
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(0, "Build", "Service", ex);
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public BuildQueueQueryResult2010 QueryBuildsById(int[] ids, QueryOptions2010 options)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryBuildsById), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<int>(nameof (ids), (IList<int>) ids);
        methodInformation.AddParameter(nameof (options), (object) options);
        this.EnterMethod(methodInformation);
        return RosarioHelper.QueryQueuedBuildsById(this.RequestContext, (IList<int>) ids, options);
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(0, "Build", "Service", ex);
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public List<QueuedBuild2010> QueueBuilds(
      List<BuildRequest2010> requests,
      QueueOptions2010 options)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueueBuilds), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<BuildRequest2010>(nameof (requests), (IList<BuildRequest2010>) requests);
        methodInformation.AddParameter(nameof (options), (object) options);
        this.EnterMethod(methodInformation);
        return RosarioHelper.QueueBuilds(this.RequestContext, (IList<BuildRequest2010>) requests, options);
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(0, "Build", "Service", ex);
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public List<QueuedBuild2010> UpdateBuilds(List<QueuedBuildUpdateOptions2010> updates)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UpdateBuilds), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<QueuedBuildUpdateOptions2010>(nameof (updates), (IList<QueuedBuildUpdateOptions2010>) updates);
        this.EnterMethod(methodInformation);
        return RosarioHelper.UpdateQueuedBuilds(this.RequestContext, (IList<QueuedBuildUpdateOptions2010>) updates);
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(0, "Build", "Service", ex);
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }
  }
}
