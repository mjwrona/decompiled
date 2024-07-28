// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.BuildWebService
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Web.Services;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  [ClientService(ComponentName = "TeamBuild", RegistrationName = "Build", ServiceName = "BuildService3", CollectionServiceIdentifier = "427febc8-f703-482b-9f79-bfe1bb4631bc")]
  [WebService(Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/Build/BuildService/03", Description = "DevOps Build web service")]
  public sealed class BuildWebService : BuildWebServiceBase
  {
    [WebMethod]
    public List<BuildDefinition2010> AddBuildDefinitions(BuildDefinition2010[] definitions)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (AddBuildDefinitions), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<BuildDefinition2010>(nameof (definitions), (IList<BuildDefinition2010>) definitions);
        this.EnterMethod(methodInformation);
        return RosarioHelper.AddBuildDefinitions(this.RequestContext, (IList<BuildDefinition2010>) definitions);
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
    public List<ProcessTemplate2010> AddProcessTemplates(ProcessTemplate2010[] processTemplates)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (AddProcessTemplates), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<ProcessTemplate2010>(nameof (processTemplates), (IList<ProcessTemplate2010>) processTemplates);
        this.EnterMethod(methodInformation);
        return RosarioHelper.AddProcessTemplates(this.RequestContext, (IList<ProcessTemplate2010>) processTemplates);
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
    public List<BuildDeletionResult2010> DeleteBuilds(
      [ClientType(typeof (Uri[]))] string[] uris,
      DeleteOptions2010 deleteOptions)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (DeleteBuilds), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<string>(nameof (uris), (IList<string>) uris);
        this.EnterMethod(methodInformation);
        return RosarioHelper.DeleteBuilds(this.RequestContext, (IList<string>) uris, deleteOptions, false);
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
    public List<BuildDefinition2010> GetAffectedBuildDefinitions(
      string[] serverItems,
      ContinuousIntegrationType continuousIntegrationType)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (GetAffectedBuildDefinitions), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<string>(nameof (serverItems), (IList<string>) serverItems);
        this.EnterMethod(methodInformation);
        return RosarioHelper.GetAffectedBuildDefinitions(this.RequestContext, (IList<string>) serverItems, continuousIntegrationType);
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
    public List<ProcessTemplate2010> QueryProcessTemplates(
      string teamProject,
      ProcessTemplateType2010[] queryTypes)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryProcessTemplates), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (teamProject), (object) teamProject);
        methodInformation.AddParameter(nameof (queryTypes), (object) queryTypes);
        this.EnterMethod(methodInformation);
        return RosarioHelper.QueryProcessTemplates(this.RequestContext, teamProject, (IList<ProcessTemplateType2010>) queryTypes);
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
    public BuildDetail2010 NotifyBuildCompleted([ClientType(typeof (Uri))] string buildUri)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (NotifyBuildCompleted), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (buildUri), (object) buildUri);
        this.EnterMethod(methodInformation);
        return RosarioHelper.NotifyBuildCompleted(this.RequestContext, buildUri);
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
    public List<BuildDefinitionQueryResult2010> QueryBuildDefinitions(
      BuildDefinitionSpec2010[] specs)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation("QueryBuildGroups", MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<BuildDefinitionSpec2010>(nameof (specs), (IList<BuildDefinitionSpec2010>) specs);
        this.EnterMethod(methodInformation);
        return RosarioHelper.QueryBuildDefinitions(this.RequestContext, (IList<BuildDefinitionSpec2010>) specs);
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
    public BuildDefinitionQueryResult2010 QueryBuildDefinitionsByUri(
      [ClientType(typeof (Uri[]))] string[] uris,
      QueryOptions2010 options)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryBuildDefinitionsByUri), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<string>(nameof (uris), (IList<string>) uris);
        this.EnterMethod(methodInformation);
        return RosarioHelper.QueryBuildDefinitionsByUri(this.RequestContext, (IList<string>) uris, options);
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
    public StreamingCollection<BuildQueryResult2010> QueryBuilds(BuildDetailSpec2010[] specs)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryBuilds), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<BuildDetailSpec2010>(nameof (specs), (IList<BuildDetailSpec2010>) specs);
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = RosarioHelper.QueryBuilds(this.RequestContext, (IList<BuildDetailSpec2010>) specs);
          this.AddWebServiceResource((IDisposable) resource);
        }
        catch (Exception ex)
        {
          resource?.Dispose();
          throw;
        }
        return resource.Current<StreamingCollection<BuildQueryResult2010>>();
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
    public BuildQueryResult2010 QueryBuildsByUri(
      [ClientType(typeof (Uri[]))] string[] uris,
      string[] informationTypes,
      QueryOptions2010 options,
      QueryDeletedOption2010 deletedOption)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryBuildsByUri), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<string>(nameof (uris), (IList<string>) uris);
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = RosarioHelper.QueryBuildsByUri(this.RequestContext, (IList<string>) uris, (IList<string>) informationTypes, options, deletedOption);
          this.AddWebServiceResource((IDisposable) resource);
        }
        catch (Exception ex)
        {
          resource?.Dispose();
          throw;
        }
        return resource.Current<BuildQueryResult2010>();
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
        MethodInformation methodInformation = new MethodInformation(nameof (StopBuilds), MethodType.ReadWrite, EstimatedMethodCost.Low);
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
    public List<BuildDefinition2010> UpdateBuildDefinitions(BuildDefinition2010[] updates)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UpdateBuildDefinitions), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<BuildDefinition2010>(nameof (updates), (IList<BuildDefinition2010>) updates);
        this.EnterMethod(methodInformation);
        return RosarioHelper.UpdateBuildDefinitions(this.RequestContext, (IList<BuildDefinition2010>) updates);
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
    public List<BuildInformationNode2010> UpdateBuildInformation(
      InformationChangeRequest2010[] changes)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UpdateBuildInformation), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<InformationChangeRequest2010>(nameof (changes), (IList<InformationChangeRequest2010>) changes);
        this.EnterMethod(methodInformation);
        return RosarioHelper.UpdateBuildInformation(this.RequestContext, (IList<InformationChangeRequest2010>) changes);
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
        return RosarioHelper.UpdateBuilds(this.RequestContext, (IList<BuildUpdateOptions2010>) updateOptions);
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
    public List<ProcessTemplate2010> UpdateProcessTemplates(ProcessTemplate2010[] processTemplates)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UpdateProcessTemplates), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<ProcessTemplate2010>(nameof (processTemplates), (IList<ProcessTemplate2010>) processTemplates);
        this.EnterMethod(methodInformation);
        return RosarioHelper.UpdateProcessTemplates(this.RequestContext, (IList<ProcessTemplate2010>) processTemplates);
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
      [ClientType(typeof (IEnumerable<BuildTeamProjectPermission2010>))] List<BuildTeamProjectPermission2010> permissions)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation("CreateTeamProjectBuildComponents", MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (projectUri), (object) projectUri);
        methodInformation.AddArrayParameter<BuildTeamProjectPermission2010>(nameof (permissions), (IList<BuildTeamProjectPermission2010>) permissions);
        this.EnterMethod(methodInformation);
        RosarioHelper.CreateTeamProject(this.RequestContext, projectUri, (IList<BuildTeamProjectPermission2010>) permissions);
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
