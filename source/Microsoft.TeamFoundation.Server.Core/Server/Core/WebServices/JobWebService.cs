// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.WebServices.JobWebService
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Web.Services;
using System.Xml;

namespace Microsoft.TeamFoundation.Server.Core.WebServices
{
  [WebService(Namespace = "http://microsoft.com/webservices/")]
  [ClientService(ComponentName = "Framework", RegistrationName = "Framework", ServerConfiguration = ServerConfiguration.TfsConnection, ServiceName = "JobService", CollectionServiceIdentifier = "22da67e4-dbe5-4e09-9bd3-02ff4b620aa7", ConfigurationServiceIdentifier = "DA1C0184-14FE-4E13-B7FC-6EAA07D84BE8")]
  public class JobWebService : FrameworkWebService
  {
    private readonly SecuredJobManager m_jobManager;

    public JobWebService() => this.m_jobManager = this.RequestContext.GetService<SecuredJobManager>();

    protected override void EnterMethod(MethodInformation methodInformation)
    {
      base.EnterMethod(methodInformation);
      this.RequestContext.CheckOnPremisesDeployment(true);
    }

    [WebMethod]
    public void UpdateJobDefinitions(
      [ClientType(typeof (IEnumerable<Guid>))] List<Guid> jobsToDelete,
      [ClientType(typeof (IEnumerable<TeamFoundationJobDefinition>))] List<TeamFoundationJobDefinition> jobUpdates)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UpdateJobDefinitions), MethodType.Admin, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<Guid>(nameof (jobsToDelete), (IList<Guid>) jobsToDelete);
        methodInformation.AddArrayParameter<TeamFoundationJobDefinition>(nameof (jobUpdates), (IList<TeamFoundationJobDefinition>) jobUpdates);
        this.EnterMethod(methodInformation);
        this.m_jobManager.UpdateJobDefinitions(this.RequestContext, (IEnumerable<Guid>) jobsToDelete, (IEnumerable<TeamFoundationJobDefinition>) jobUpdates);
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
    public List<TeamFoundationJobDefinition> QueryJobDefinitions([ClientType(typeof (IEnumerable<Guid>))] Guid[] jobIds)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryJobDefinitions), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<Guid>(nameof (jobIds), (IList<Guid>) jobIds);
        this.EnterMethod(methodInformation);
        return this.m_jobManager.QueryJobDefinitions(this.RequestContext, (IEnumerable<Guid>) jobIds);
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
    public int QueueJobs([ClientType(typeof (IEnumerable<Guid>))] List<Guid> jobIds, bool highPriority, int maxDelaySeconds)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueueJobs), MethodType.Admin, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<Guid>(nameof (jobIds), (IList<Guid>) jobIds);
        methodInformation.AddParameter(nameof (highPriority), (object) highPriority);
        methodInformation.AddParameter(nameof (maxDelaySeconds), (object) maxDelaySeconds);
        this.EnterMethod(methodInformation);
        return this.m_jobManager.QueueJobsInternal(this.RequestContext, (IEnumerable<Guid>) jobIds, highPriority, maxDelaySeconds);
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
    public Guid QueueOneTimeJob(
      string jobName,
      string extensionName,
      XmlNode jobData,
      bool highPriority)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueueOneTimeJob), MethodType.Admin, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (jobName), (object) jobName);
        methodInformation.AddParameter(nameof (extensionName), (object) extensionName);
        methodInformation.AddParameter(nameof (highPriority), (object) highPriority);
        this.EnterMethod(methodInformation);
        throw new QueueOneTimeJobNotPermittedException(Guid.NewGuid());
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
    public bool StopJob(Guid jobId)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (StopJob), MethodType.Admin, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (jobId), (object) jobId);
        this.EnterMethod(methodInformation);
        return this.m_jobManager.StopJob(this.RequestContext, jobId);
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
    public bool PauseJob(Guid jobId) => throw new NotImplementedException("Pausing jobs is no longer supported.");

    [WebMethod]
    public bool ResumeJob(Guid jobId) => throw new NotImplementedException("Pausing jobs is no longer supported.");

    [WebMethod]
    public List<TeamFoundationJobHistoryEntry> QueryJobHistory([ClientType(typeof (IEnumerable<Guid>))] List<Guid> jobIds)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryJobHistory), MethodType.Admin, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<Guid>(nameof (jobIds), (IList<Guid>) jobIds);
        this.EnterMethod(methodInformation);
        return this.m_jobManager.QueryJobHistory(this.RequestContext, (IEnumerable<Guid>) jobIds);
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
    public List<TeamFoundationJobHistoryEntry> QueryLatestJobHistory([ClientType(typeof (IEnumerable<Guid>))] List<Guid> jobIds)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryLatestJobHistory), MethodType.Admin, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<Guid>(nameof (jobIds), (IList<Guid>) jobIds);
        this.EnterMethod(methodInformation);
        return this.m_jobManager.QueryLatestJobHistory(this.RequestContext, (IEnumerable<Guid>) jobIds);
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
