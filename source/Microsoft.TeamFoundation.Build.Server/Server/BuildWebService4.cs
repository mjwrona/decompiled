// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.BuildWebService4
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Web.Services;

namespace Microsoft.TeamFoundation.Build.Server
{
  [ClientService(ComponentName = "TeamBuild", RegistrationName = "Build", ServiceName = "BuildService4", CollectionServiceIdentifier = "AAE1325C-E97F-4A15-B557-9D1620D5D5F4")]
  [WebService(Name = "BuildService", Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Build")]
  public sealed class BuildWebService4 : BuildWebServiceBase
  {
    [WebMethod]
    public List<BuildDefinition> AddBuildDefinitions(BuildDefinition[] definitions)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (AddBuildDefinitions), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<BuildDefinition>(nameof (definitions), (IList<BuildDefinition>) definitions);
        this.EnterMethod(methodInformation);
        return this.BuildService.AddBuildDefinitions(this.RequestContext, (IList<BuildDefinition>) definitions);
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
    public List<ProcessTemplate> AddProcessTemplates(ProcessTemplate[] processTemplates)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (AddProcessTemplates), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<ProcessTemplate>(nameof (processTemplates), (IList<ProcessTemplate>) processTemplates);
        this.EnterMethod(methodInformation);
        return this.BuildService.AddProcessTemplates(this.RequestContext, (IList<ProcessTemplate>) processTemplates);
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
    public List<BuildDeletionResult> DeleteBuilds([ClientType(typeof (Uri[]))] string[] uris, DeleteOptions deleteOptions)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (DeleteBuilds), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<string>(nameof (uris), (IList<string>) uris);
        this.EnterMethod(methodInformation);
        return this.BuildService.DeleteBuilds(this.RequestContext, (IList<string>) uris, deleteOptions, false, new Guid(), false);
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
    public void DestroyBuilds([ClientType(typeof (Uri[]))] string[] uris)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (DestroyBuilds), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<string>(nameof (uris), (IList<string>) uris);
        this.EnterMethod(methodInformation);
        this.BuildService.DestroyBuilds(this.RequestContext, (IList<string>) uris);
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
    public void DeleteProcessTemplates(int[] processTemplateIds)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (DeleteProcessTemplates), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<int>(nameof (processTemplateIds), (IList<int>) processTemplateIds);
        this.EnterMethod(methodInformation);
        this.BuildService.DeleteProcessTemplates(this.RequestContext, (IList<int>) processTemplateIds);
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
    public List<BuildDefinition> GetAffectedBuildDefinitions(
      string[] serverItems,
      DefinitionTriggerType continuousIntegrationType)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (GetAffectedBuildDefinitions), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<string>(nameof (serverItems), (IList<string>) serverItems);
        this.EnterMethod(methodInformation);
        return this.BuildService.GetAffectedBuildDefinitions(this.RequestContext, (IList<string>) serverItems, continuousIntegrationType);
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
        ArgumentValidation.Check(nameof (teamProject), teamProject, false, (string) null);
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
    public List<ProcessTemplate> QueryProcessTemplates(
      string teamProject,
      ProcessTemplateType[] queryTypes)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryProcessTemplates), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (teamProject), (object) teamProject);
        methodInformation.AddParameter(nameof (queryTypes), (object) queryTypes);
        this.EnterMethod(methodInformation);
        return this.BuildService.QueryProcessTemplates(this.RequestContext, teamProject, (IList<ProcessTemplateType>) queryTypes);
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
    public BuildDetail NotifyBuildCompleted([ClientType(typeof (Uri))] string buildUri)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (NotifyBuildCompleted), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (buildUri), (object) buildUri);
        this.EnterMethod(methodInformation);
        ArgumentValidation.CheckUri(nameof (buildUri), buildUri, "Build", false, (string) null);
        return this.BuildService.NotifyBuildCompleted(this.RequestContext, buildUri);
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
    public Guid RequestIntermediateLogs([ClientType(typeof (Uri))] string buildUri)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (RequestIntermediateLogs), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (buildUri), (object) buildUri);
        this.EnterMethod(methodInformation);
        return this.BuildService.RequestIntermediateLogs(this.RequestContext, buildUri);
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
    public List<BuildDefinitionQueryResult> QueryBuildDefinitions(
      BuildDefinitionSpec[] specs,
      bool strict)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation("QueryBuildGroups", MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<BuildDefinitionSpec>(nameof (specs), (IList<BuildDefinitionSpec>) specs);
        this.EnterMethod(methodInformation);
        return this.BuildService.QueryBuildDefinitions(this.RequestContext, (IList<BuildDefinitionSpec>) specs, strict);
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
    public BuildDefinitionQueryResult QueryBuildDefinitionsByUri(
      [ClientType(typeof (Uri[]))] string[] uris,
      string[] propertyNameFilters,
      QueryOptions options)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryBuildDefinitionsByUri), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<string>(nameof (uris), (IList<string>) uris);
        this.EnterMethod(methodInformation);
        return this.BuildService.QueryBuildDefinitionsByUri(this.RequestContext, (IList<string>) uris, (IList<string>) propertyNameFilters, options, new Guid());
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
    public StreamingCollection<BuildQueryResult> QueryBuilds(BuildDetailSpec[] specs)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryBuilds), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<BuildDetailSpec>(nameof (specs), (IList<BuildDetailSpec>) specs);
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.BuildService.QueryBuilds(this.RequestContext, (IList<BuildDetailSpec>) specs, new Guid());
          this.AddWebServiceResource((IDisposable) resource);
        }
        catch (Exception ex)
        {
          resource?.Dispose();
          throw;
        }
        return resource.Current<StreamingCollection<BuildQueryResult>>();
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
    public BuildQueryResult QueryBuildsByUri(
      [ClientType(typeof (Uri[]))] string[] uris,
      string[] informationTypes,
      QueryOptions options,
      QueryDeletedOption deletedOption)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryBuildsByUri), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<string>(nameof (uris), (IList<string>) uris);
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.BuildService.QueryBuildsByUri(this.RequestContext, (IList<string>) uris, (IList<string>) informationTypes, options, deletedOption, new Guid(), false);
          this.AddWebServiceResource((IDisposable) resource);
        }
        catch (Exception ex)
        {
          resource?.Dispose();
          throw;
        }
        return resource.Current<BuildQueryResult>();
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
        this.BuildService.StopBuilds(this.RequestContext, (IList<string>) uris, new Guid());
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
    public List<BuildDefinition> UpdateBuildDefinitions(BuildDefinition[] updates)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UpdateBuildDefinitions), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<BuildDefinition>(nameof (updates), (IList<BuildDefinition>) updates);
        this.EnterMethod(methodInformation);
        return this.BuildService.UpdateBuildDefinitions(this.RequestContext, (IList<BuildDefinition>) updates);
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
    public List<BuildInformationNode> UpdateBuildInformation(InformationChangeRequest[] changes)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UpdateBuildInformation), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<InformationChangeRequest>(nameof (changes), (IList<InformationChangeRequest>) changes);
        this.EnterMethod(methodInformation);
        return this.BuildService.UpdateBuildInformation(this.RequestContext, (IList<InformationChangeRequest>) changes);
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
    public List<BuildDetail> UpdateBuilds(BuildUpdateOptions[] updateOptions)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UpdateBuilds), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<BuildUpdateOptions>(nameof (updateOptions), (IList<BuildUpdateOptions>) updateOptions);
        this.EnterMethod(methodInformation);
        return this.BuildService.UpdateBuilds(this.RequestContext, (IList<BuildUpdateOptions>) updateOptions, new Guid());
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
    public List<ProcessTemplate> UpdateProcessTemplates(ProcessTemplate[] processTemplates)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UpdateProcessTemplates), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<ProcessTemplate>(nameof (processTemplates), (IList<ProcessTemplate>) processTemplates);
        this.EnterMethod(methodInformation);
        return this.BuildService.UpdateProcessTemplates(this.RequestContext, (IList<ProcessTemplate>) processTemplates);
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
    public void CreateTeamProjectComponents(
      [ClientType(typeof (Uri))] string projectUri,
      [ClientType(typeof (IEnumerable<BuildTeamProjectPermission>))] List<BuildTeamProjectPermission> permissions)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation("CreateTeamProjectBuildComponents", MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (projectUri), (object) projectUri);
        methodInformation.AddArrayParameter<BuildTeamProjectPermission>(nameof (permissions), (IList<BuildTeamProjectPermission>) permissions);
        this.EnterMethod(methodInformation);
        this.BuildService.CreateTeamProject(this.RequestContext, projectUri, (IList<BuildTeamProjectPermission>) permissions);
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
