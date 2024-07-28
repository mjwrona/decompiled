// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.BuildQueueWebService4
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Web.Services;

namespace Microsoft.TeamFoundation.Build.Server
{
  [ClientService(ComponentName = "TeamBuild", RegistrationName = "Build", ServiceName = "BuildQueueService4", CollectionServiceIdentifier = "3F6F3C6C-EDF5-429B-A1AE-F7D94306A4C8")]
  [WebService(Name = "BuildQueueService", Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Build")]
  public sealed class BuildQueueWebService4 : BuildWebServiceBase
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
    public StreamingCollection<BuildQueueQueryResult> QueryBuilds(List<BuildQueueSpec> specs)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation("QueryQueuedBuilds", MethodType.Normal, EstimatedMethodCost.Low, false);
        methodInformation.AddArrayParameter<BuildQueueSpec>(nameof (specs), (IList<BuildQueueSpec>) specs);
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.BuildService.QueryQueuedBuilds(this.RequestContext, (IList<BuildQueueSpec>) specs, new Guid());
          this.AddWebServiceResource((IDisposable) resource);
        }
        catch (Exception ex)
        {
          resource?.Dispose();
          throw;
        }
        return resource.Current<StreamingCollection<BuildQueueQueryResult>>();
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
    public BuildQueueQueryResult QueryBuildsById(
      int[] ids,
      string[] informationTypes,
      QueryOptions options)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation("QueryQueuedBuildsById", MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<int>(nameof (ids), (IList<int>) ids);
        methodInformation.AddParameter(nameof (options), (object) options);
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.BuildService.QueryQueuedBuildsById(this.RequestContext, (IList<int>) ids, (IList<string>) informationTypes, options);
          this.AddWebServiceResource((IDisposable) resource);
        }
        catch (Exception ex)
        {
          resource?.Dispose();
          throw;
        }
        return resource.Current<BuildQueueQueryResult>();
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
    public BuildQueueQueryResult QueueBuilds(List<BuildRequest> requests, QueueOptions options)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueueBuilds), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<BuildRequest>(nameof (requests), (IList<BuildRequest>) requests);
        methodInformation.AddParameter(nameof (options), (object) options);
        this.EnterMethod(methodInformation);
        return this.BuildService.QueueBuilds(this.RequestContext, (IList<BuildRequest>) requests, options, new Guid());
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
    public BuildQueueQueryResult StartBuildsNow(int[] queueIds)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (StartBuildsNow), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<int>(nameof (queueIds), (IList<int>) queueIds);
        this.EnterMethod(methodInformation);
        return this.BuildService.StartQueuedBuildsNow(this.RequestContext, queueIds, new Guid());
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
    public BuildQueueQueryResult UpdateBuilds(List<QueuedBuildUpdateOptions> updates)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UpdateBuilds), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<QueuedBuildUpdateOptions>(nameof (updates), (IList<QueuedBuildUpdateOptions>) updates);
        this.EnterMethod(methodInformation);
        return this.BuildService.UpdateQueuedBuilds(this.RequestContext, (IList<QueuedBuildUpdateOptions>) updates, new Guid());
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
