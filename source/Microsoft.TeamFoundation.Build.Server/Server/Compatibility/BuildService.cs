// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.BuildService
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Web.Services;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  [ClientService(ComponentName = "TeamBuild", RegistrationName = "Build", ServiceName = "BuildService", CollectionServiceIdentifier = "543cf133-319b-4c7b-800a-fafff734f291")]
  [WebService(Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/Build/BuildService/03", Description = "DevOps Build web service")]
  public sealed class BuildService : BuildWebServiceBase
  {
    [WebMethod]
    public List<BuildDefinition2010> AddBuildDefinitions(BuildDefinition2010[] definitions)
    {
      try
      {
        this.EnterMethod(new MethodInformation(nameof (AddBuildDefinitions), MethodType.ReadWrite, EstimatedMethodCost.Low));
        throw new NotSupportedException(ResourceStrings.AddingDefinitionsDeprecated());
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
    public void AddBuildQualities(string teamProject, string[] qualities)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (AddBuildQualities), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (teamProject), (object) teamProject);
        methodInformation.AddArrayParameter<string>(nameof (qualities), (IList<string>) qualities);
        this.EnterMethod(methodInformation);
        this.BuildService.AddBuildQualities(this.RequestContext, teamProject, (IList<string>) qualities);
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
    public void CancelBuilds(int[] ids)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (CancelBuilds), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<int>(nameof (ids), (IList<int>) ids);
        this.EnterMethod(methodInformation);
        this.BuildService.CancelBuilds(this.RequestContext, ids, new Guid());
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
    public void DeleteBuildDefinitions([ClientType(typeof (Uri[]))] string[] uris)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (DeleteBuildDefinitions), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<string>(nameof (uris), (IList<string>) uris);
        this.EnterMethod(methodInformation);
        this.BuildService.DeleteBuildDefinitions(this.RequestContext, (IList<string>) uris);
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
    public void DeleteBuildQualities(string teamProject, string[] qualities)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (DeleteBuildQualities), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (teamProject), (object) teamProject);
        methodInformation.AddArrayParameter<string>(nameof (qualities), (IList<string>) qualities);
        this.EnterMethod(methodInformation);
        this.BuildService.DeleteBuildQualities(this.RequestContext, teamProject, (IList<string>) qualities);
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
    public List<BuildDeletionResult2010> DeleteBuilds([ClientType(typeof (Uri[]))] string[] uris)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (DeleteBuilds), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<string>(nameof (uris), (IList<string>) uris);
        this.EnterMethod(methodInformation);
        throw new NotSupportedException();
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
    public void EvaluateSchedules()
    {
      try
      {
        this.EnterMethod(new MethodInformation(nameof (EvaluateSchedules), MethodType.ReadWrite, EstimatedMethodCost.VeryLow));
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
    public List<BuildDefinition2010> GetAffectedBuildDefinitions(string[] serverItems)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (GetAffectedBuildDefinitions), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<string>(nameof (serverItems), (IList<string>) serverItems);
        this.EnterMethod(methodInformation);
        throw new NotSupportedException();
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
    public List<string> GetBuildQualities(string teamProject)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (GetBuildQualities), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (teamProject), (object) teamProject);
        this.EnterMethod(methodInformation);
        return this.BuildService.GetBuildQualities(this.RequestContext, teamProject);
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
    public List<BuildAgent2008> QueryBuildAgentsByUri([ClientType(typeof (Uri[]))] string[] uris)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryBuildAgentsByUri), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<string>(nameof (uris), (IList<string>) uris);
        this.EnterMethod(methodInformation);
        throw new NotSupportedException();
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
    public BuildGroupQueryResult QueryBuildDefinitionsByUri([ClientType(typeof (Uri[]))] string[] uris)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryBuildDefinitionsByUri), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<string>(nameof (uris), (IList<string>) uris);
        this.EnterMethod(methodInformation);
        throw new NotSupportedException();
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
    [XmlInclude(typeof (BuildGroupItem2010))]
    [ClientServiceMethod(AsyncPattern = true, SyncPattern = true)]
    public List<BuildGroupQueryResult> QueryBuildGroups(BuildGroupItemSpec2010[] specs)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryBuildGroups), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<BuildGroupItemSpec2010>(nameof (specs), (IList<BuildGroupItemSpec2010>) specs);
        this.EnterMethod(methodInformation);
        throw new NotSupportedException();
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
    public StreamingCollection<BuildQueryResult2008> QueryBuilds(BuildDetailSpec2010[] specs)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryBuilds), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<BuildDetailSpec2010>(nameof (specs), (IList<BuildDetailSpec2010>) specs);
        this.EnterMethod(methodInformation);
        throw new NotSupportedException();
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
    public BuildQueryResult2008 QueryBuildsByUri(
      [ClientType(typeof (Uri[]))] string[] uris,
      string[] informationTypes,
      QueryOptions2010 options)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryBuildsByUri), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<string>(nameof (uris), (IList<string>) uris);
        this.EnterMethod(methodInformation);
        throw new NotSupportedException();
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
    public List<BuildQueueQueryResult2008> QueryBuildQueue(BuildQueueSpec2008[] specs)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryBuildQueue), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<BuildQueueSpec2008>(nameof (specs), (IList<BuildQueueSpec2008>) specs);
        this.EnterMethod(methodInformation);
        throw new NotSupportedException();
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
    public BuildQueueQueryResult2008 QueryBuildQueueById(int[] ids, QueryOptions2010 options)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryBuildQueueById), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<int>(nameof (ids), (IList<int>) ids);
        this.EnterMethod(methodInformation);
        throw new NotSupportedException();
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
    public QueuedBuild2008 QueueBuild(BuildRequest2008 buildRequest, QueueOptions2010 options)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueueBuild), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (buildRequest), (object) buildRequest);
        this.EnterMethod(methodInformation);
        throw new NotSupportedException();
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
    public void StopBuilds([ClientType(typeof (Uri[]))] string[] uris)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (StopBuilds), MethodType.ReadWrite, EstimatedMethodCost.Low, true, true);
        methodInformation.AddArrayParameter<string>(nameof (uris), (IList<string>) uris);
        this.EnterMethod(methodInformation);
        RosarioHelper.StopBuilds(this.RequestContext, (IList<string>) uris);
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
    public List<BuildDetail2010> UpdateBuilds(BuildUpdateOptions2010[] updateOptions)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UpdateBuilds), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<BuildUpdateOptions2010>(nameof (updateOptions), (IList<BuildUpdateOptions2010>) updateOptions);
        this.EnterMethod(methodInformation);
        throw new NotSupportedException();
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
    public List<QueuedBuild2008> UpdateQueuedBuilds(QueuedBuildUpdateOptions2010[] updateOptions)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UpdateQueuedBuilds), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<QueuedBuildUpdateOptions2010>(nameof (updateOptions), (IList<QueuedBuildUpdateOptions2010>) updateOptions);
        this.EnterMethod(methodInformation);
        throw new NotSupportedException();
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
    public List<BuildAgent2008> AddBuildAgents(BuildAgent2008[] agents)
    {
      try
      {
        this.EnterMethod(new MethodInformation(nameof (AddBuildAgents), MethodType.ReadWrite, EstimatedMethodCost.VeryLow));
        throw new NotSupportedException(ResourceStrings.AddingAgentsDeprecated());
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
    public void DeleteBuildAgents([ClientType(typeof (Uri[]))] string[] uris)
    {
      try
      {
        this.EnterMethod(new MethodInformation(nameof (DeleteBuildAgents), MethodType.ReadWrite, EstimatedMethodCost.VeryLow));
        throw new NotSupportedException(ResourceStrings.DeletingAgentsDeprecated());
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
    public void ProcessChangeset(int changesetId)
    {
      try
      {
        this.EnterMethod(new MethodInformation(nameof (ProcessChangeset), MethodType.ReadWrite, EstimatedMethodCost.VeryLow));
        throw new NotSupportedException(ResourceStrings.ProcessingChangesetDeprecated());
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
    public List<BuildAgent2008> UpdateBuildAgents(BuildAgent2008[] updates)
    {
      try
      {
        this.EnterMethod(new MethodInformation(nameof (UpdateBuildAgents), MethodType.ReadWrite, EstimatedMethodCost.VeryLow));
        throw new NotSupportedException(ResourceStrings.UpdatingAgentsDeprecated());
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
    public List<BuildDefinition2010> UpdateBuildDefinitions(BuildDefinition2010[] updates)
    {
      try
      {
        this.EnterMethod(new MethodInformation(nameof (UpdateBuildDefinitions), MethodType.ReadWrite, EstimatedMethodCost.VeryLow));
        throw new NotSupportedException(ResourceStrings.UpdatingDefinitionsDeprecated());
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
    public List<BuildInformationNode2010> UpdateBuildInformation(
      InformationChangeRequest2010[] changes)
    {
      try
      {
        this.EnterMethod(new MethodInformation(nameof (UpdateBuildInformation), MethodType.ReadWrite, EstimatedMethodCost.VeryLow));
        throw new NotSupportedException(ResourceStrings.UpdatingBuildInformationDeprecated());
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
