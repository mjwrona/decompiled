// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.VersionControlIntegration
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Web.Services;
using System.Web.Services.Protocols;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [WebServiceBinding(Name = "IProjectMaintenanceBinding", Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Admin/03")]
  [WebService(Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Linking/03", Description = "DevOps VersionControl Integration web service")]
  public class VersionControlIntegration : VersionControlWebService
  {
    [WebMethod]
    [SoapDocumentMethod(Action = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Linking/03/GetArtifacts")]
    public Artifact[] GetArtifacts(string[] artifactUris)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (GetArtifacts), MethodType.Tool, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<string>(nameof (artifactUris), (IList<string>) artifactUris);
        this.EnterMethod(methodInformation);
        return this.VersionControlService.IntegrationInterface.BisGetArtifacts(this.VersionControlRequestContext, artifactUris);
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
    [SoapDocumentMethod(Binding = "IProjectMaintenanceBinding", Action = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Admin/03/DeleteProject")]
    public bool DeleteProject(string projectUri)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (DeleteProject), MethodType.Tool, EstimatedMethodCost.High, TimeSpan.FromMinutes(60.0));
        methodInformation.AddParameter(nameof (projectUri), (object) projectUri);
        this.EnterMethod(methodInformation);
        if (string.IsNullOrEmpty(projectUri))
          throw new ArgumentNullException(nameof (projectUri));
        Guid projectId;
        try
        {
          projectId = ProjectInfo.GetProjectId(projectUri);
        }
        catch (ArgumentException ex)
        {
          return false;
        }
        IProjectService service = this.RequestContext.GetService<IProjectService>();
        ProjectInfo project;
        try
        {
          project = service.GetProject(this.RequestContext.Elevate(), projectId);
        }
        catch (ProjectDoesNotExistException ex)
        {
          return false;
        }
        return this.VersionControlService.DeleteTeamProjectFolder(this.RequestContext, project.Name, projectUri, true, false);
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
