// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Integration.Server.Classification
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Notifications;
using Microsoft.VisualStudio.Services.Notifications.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Services;
using System.Xml;

namespace Microsoft.TeamFoundation.Integration.Server
{
  [WebService(Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Classification/03", Description = "DevOps Classification web service")]
  [ClientService(ServiceName = "CommonStructure", CollectionServiceIdentifier = "d9c3f8ff-8938-4193-919b-7588e81cb730")]
  public class Classification : IntegrationWebService
  {
    private static readonly HashSet<string> s_backCompatPropertyNames = new HashSet<string>((IEqualityComparer<string>) TFStringComparer.TeamProjectPropertyName)
    {
      "System.MSPROJ",
      ProjectApiConstants.ProcessTemplateNameProjectPropertyName
    };

    public Classification() => this.ServiceVersion = 1;

    public Classification(int serviceVersion) => this.ServiceVersion = serviceVersion;

    [WebMethod]
    public CommonStructureProjectInfo CreateProject(string projectName, XmlElement structure)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (CreateProject), MethodType.Admin, EstimatedMethodCost.Moderate);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        methodInformation.AddParameter(nameof (structure), (object) structure?.InnerXml);
        this.EnterMethod(methodInformation);
        if (this.ServiceVersion == 1)
          throw new UnauthorizedAccessException(Microsoft.Azure.Boards.CssNodes.ServerResources.CSS_INCOMPATIBLE_CLIENT_FOR_PCW());
        this.RequestContext.CheckOnPremisesDeployment(true);
        return this.RequestContext.GetService<CommonStructureService>().CreateProject(this.RequestContext, projectName, structure, new Guid?());
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
    public void DeleteProject(string projectUri)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (DeleteProject), MethodType.Tool, EstimatedMethodCost.Moderate);
        methodInformation.AddParameter(nameof (projectUri), (object) projectUri);
        this.EnterMethod(methodInformation);
        this.RequestContext.GetService<CommonStructureService>().HardDeleteProject(this.RequestContext, projectUri);
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
    public void GetProjectProperties(
      string projectUri,
      out string name,
      out string state,
      out int templateId,
      out CommonStructureProjectProperty[] properties)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (GetProjectProperties), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (projectUri), (object) projectUri);
        this.EnterMethod(methodInformation);
        templateId = -1;
        Guid id;
        projectUri = CommonStructureUtils.CheckAndNormalizeUri(projectUri, nameof (projectUri), true, out id);
        IProjectService service = this.RequestContext.GetService<IProjectService>();
        ProjectInfo project = service.GetProject(this.RequestContext, id);
        name = project.Name;
        state = project.State.ToString();
        IEnumerable<ProjectProperty> projectProperties = service.GetProjectProperties(this.RequestContext, id, (IEnumerable<string>) InternalCommonStructureUtils.BackCompatSystemPropertyNames);
        properties = CommonStructureProjectProperty.ConvertProjectProperties((IList<ProjectProperty>) projectProperties.ToList<ProjectProperty>());
        if (this.ServiceVersion < 3 && this.ContainsProjectMappingProperty(properties))
          this.RequestContext.GetService<CommonStructureService>().TransformProjectMappingProperty("V1ProjectMapping.xslt", properties);
        InternalCommonStructureUtils.TranslatePropertyNames(properties, true);
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
    public void UpdateProjectProperties(
      string projectUri,
      string state,
      CommonStructureProjectProperty[] properties)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UpdateProjectProperties), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (projectUri), (object) projectUri);
        methodInformation.AddParameter(nameof (state), (object) state);
        this.EnterMethod(methodInformation);
        InternalCommonStructureUtils.TranslatePropertyNames(properties);
        if (this.ServiceVersion < 4 && this.ContainsProjectMappingProperty(properties))
          throw new UnauthorizedAccessException(Microsoft.Azure.Boards.CssNodes.ServerResources.CSS_INVALID_PROPERTY_UPDATE());
        CommonStructureProjectState result;
        if (Enum.TryParse<CommonStructureProjectState>(state, out result) && result == CommonStructureProjectState.New)
          this.RequestContext.CheckOnPremisesDeployment(true);
        Guid id;
        projectUri = CommonStructureUtils.CheckAndNormalizeUri(projectUri, nameof (projectUri), true, out id);
        CommonStructureProjectState projectState = CommonStructureUtils.ParseProjectState(state, nameof (state));
        IProjectService service = this.RequestContext.GetService<IProjectService>();
        ProjectInfo projectToUpdate = ProjectInfo.GetProjectToUpdate(id);
        projectToUpdate.State = CommonStructureProjectInfo.ConvertToProjectState(projectState);
        ProjectInfo updatedProject;
        service.UpdateProject(this.RequestContext, projectToUpdate, out updatedProject);
        if (properties != null)
        {
          IDictionary<string, ProjectProperty> dictionary1 = CommonStructureUtils.NormalizeAndToDictionary((IEnumerable<ProjectProperty>) CommonStructureProjectProperty.ConvertToProjectProperties(properties));
          IDictionary<string, ProjectProperty> dictionary2 = CommonStructureUtils.NormalizeAndToDictionary(service.GetProjectProperties(this.RequestContext, id, (IEnumerable<string>) InternalCommonStructureUtils.BackCompatSystemPropertyNames));
          this.CheckPermissionsAndSetProperties(id, CommonStructureUtils.GetPropertyChanges(dictionary1, dictionary2), Classification.s_backCompatPropertyNames);
        }
        if (properties != null || (CommonStructureProjectState) Enum.Parse(typeof (CommonStructureProjectState), state) != CommonStructureProjectState.WellFormed)
          return;
        ProjectCreatedEvent projectCreatedEvent = new ProjectCreatedEvent(updatedProject.Uri, updatedProject.Name);
        VssNotificationEvent theEvent = new VssNotificationEvent((object) projectCreatedEvent)
        {
          SourceEventCreatedTime = new DateTime?(projectToUpdate.LastUpdateTime),
          ItemId = projectCreatedEvent.Name
        };
        theEvent.AddSystemInitiatorActor();
        theEvent.AddArtifactUri(projectCreatedEvent.Uri);
        this.RequestContext.GetService<NotificationEventService>().PublishSystemEvent(this.RequestContext, theEvent);
        this.RequestContext.GetService<TeamFoundationEventService>().PublishNotification(this.RequestContext, (object) projectCreatedEvent);
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
    public CommonStructureProjectInfo GetProject(string projectUri)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (GetProject), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (projectUri), (object) projectUri);
        this.EnterMethod(methodInformation);
        return this.RequestContext.GetService<CommonStructureService>().GetProject(this.RequestContext, projectUri);
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
    public CommonStructureProjectInfo GetProjectFromName(string projectName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (GetProjectFromName), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        this.EnterMethod(methodInformation);
        return this.RequestContext.GetService<CommonStructureService>().GetProjectFromName(this.RequestContext, projectName);
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
    public CommonStructureProjectInfo[] ListProjects()
    {
      try
      {
        this.EnterMethod(new MethodInformation(nameof (ListProjects), MethodType.Normal, EstimatedMethodCost.Low));
        return this.RequestContext.GetService<CommonStructureService>().GetWellFormedProjects(this.RequestContext);
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
    public CommonStructureProjectInfo[] ListAllProjects()
    {
      try
      {
        this.EnterMethod(new MethodInformation(nameof (ListAllProjects), MethodType.Normal, EstimatedMethodCost.Low));
        return this.RequestContext.GetService<CommonStructureService>().GetProjects(this.RequestContext);
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
    public CommonStructureNodeInfo[] ListStructures(string projectUri)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (ListStructures), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (projectUri), (object) projectUri);
        this.EnterMethod(methodInformation);
        return this.RequestContext.GetService<CommonStructureService>().GetRootNodes(this.RequestContext, projectUri);
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
    public XmlElement GetNodesXml(string[] nodeUris, bool childNodes)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (GetNodesXml), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<string>(nameof (nodeUris), (IList<string>) nodeUris);
        methodInformation.AddParameter(nameof (childNodes), (object) childNodes);
        this.EnterMethod(methodInformation);
        return this.RequestContext.GetService<CommonStructureService>().GetNodesXml(this.RequestContext, nodeUris, childNodes);
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
    public string CreateNode(string nodeName, string parentNodeUri)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (CreateNode), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (nodeName), (object) nodeName);
        methodInformation.AddParameter(nameof (parentNodeUri), (object) parentNodeUri);
        this.EnterMethod(methodInformation);
        return this.RequestContext.GetService<CommonStructureService>().CreateNode(this.RequestContext, nodeName, parentNodeUri);
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
    public void RenameNode(string nodeUri, string newNodeName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (RenameNode), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (nodeUri), (object) nodeUri);
        methodInformation.AddParameter(nameof (newNodeName), (object) newNodeName);
        this.EnterMethod(methodInformation);
        this.RequestContext.GetService<CommonStructureService>().RenameNode(this.RequestContext, nodeUri, newNodeName);
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
    public void MoveBranch(string nodeUri, string newParentNodeUri)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (MoveBranch), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (nodeUri), (object) nodeUri);
        methodInformation.AddParameter(nameof (newParentNodeUri), (object) newParentNodeUri);
        this.EnterMethod(methodInformation);
        this.RequestContext.GetService<CommonStructureService>().MoveBranch(this.RequestContext, nodeUri, newParentNodeUri);
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
    public void ReorderNode(string nodeUri, int moveBy)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (ReorderNode), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (nodeUri), (object) nodeUri);
        methodInformation.AddParameter(nameof (moveBy), (object) moveBy);
        this.EnterMethod(methodInformation);
        this.RequestContext.GetService<CommonStructureService>().ReorderNode(this.RequestContext, nodeUri, moveBy);
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
    public void DeleteBranches(string[] nodeUris, string reclassifyUri)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (DeleteBranches), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<string>(nameof (nodeUris), (IList<string>) nodeUris);
        methodInformation.AddParameter(nameof (reclassifyUri), (object) reclassifyUri);
        this.EnterMethod(methodInformation);
        this.RequestContext.GetService<CommonStructureService>().DeleteBranches(this.RequestContext, nodeUris, reclassifyUri);
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
    public XmlElement GetDeletedNodesXml(string projectUri, DateTime since)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (GetDeletedNodesXml), MethodType.Tool, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (projectUri), (object) projectUri);
        methodInformation.AddParameter(nameof (since), (object) since);
        this.EnterMethod(methodInformation);
        return this.RequestContext.GetService<CommonStructureService>().GetDeletedNodes(this.RequestContext, projectUri, since);
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
    public CommonStructureNodeInfo GetNode(string nodeUri)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (GetNode), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (nodeUri), (object) nodeUri);
        this.EnterMethod(methodInformation);
        return this.RequestContext.GetService<CommonStructureService>().GetNode(this.RequestContext, nodeUri);
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
    public CommonStructureNodeInfo GetNodeFromPath(string nodePath)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (GetNodeFromPath), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (nodePath), (object) nodePath);
        this.EnterMethod(methodInformation);
        return this.RequestContext.GetService<CommonStructureService>().GetNodeFromPath(this.RequestContext, nodePath);
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
    public string GetChangedNodes(int firstSequenceId)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (GetChangedNodes), MethodType.Normal, EstimatedMethodCost.Moderate);
        methodInformation.AddParameter(nameof (firstSequenceId), (object) firstSequenceId);
        this.EnterMethod(methodInformation);
        return this.RequestContext.GetService<CommonStructureService>().GetChangedNodes(this.RequestContext, firstSequenceId);
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

    protected override Exception HandleException(Exception e)
    {
      switch (e)
      {
        case GroupSecuritySubsystemServiceException _:
        case AuthorizationSubsystemServiceException _:
          e = (Exception) new CommonStructureSubsystemServiceException(e.Message, e.InnerException);
          break;
      }
      return base.HandleException(e);
    }

    protected void CheckPermissionsAndSetProperties(
      Guid projectId,
      IEnumerable<ProjectProperty> projectProperties,
      HashSet<string> backCompatPropertyNames)
    {
      if (!projectProperties.Any<ProjectProperty>())
        return;
      IEnumerable<ProjectProperty> source = projectProperties.Where<ProjectProperty>((Func<ProjectProperty, bool>) (p => !backCompatPropertyNames.Contains(p.Name)));
      if (source.Any<ProjectProperty>())
        throw new InvalidOperationException(Resources.PropertiesCannotBeUpdated((object) string.Join(",", source.Select<ProjectProperty, string>((Func<ProjectProperty, string>) (p => p.Name)))));
      IProjectService service = this.RequestContext.GetService<IProjectService>();
      service.CheckProjectPermission(this.RequestContext, ProjectInfo.GetProjectUri(projectId), TeamProjectPermissions.GenericWrite);
      service.SetProjectProperties(this.RequestContext.Elevate(), projectId, projectProperties);
    }

    private bool ContainsProjectMappingProperty(CommonStructureProjectProperty[] properties)
    {
      bool flag = false;
      if (properties != null)
      {
        foreach (CommonStructureProjectProperty property in properties)
        {
          if (property != null && TFStringComparer.CssProjectPropertyName.Equals(property.Name, "System.MSPROJ"))
          {
            flag = true;
            break;
          }
        }
      }
      return flag;
    }

    protected int ServiceVersion { get; set; }
  }
}
