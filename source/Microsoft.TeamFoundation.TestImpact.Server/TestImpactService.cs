// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestImpact.Server.TestImpactService
// Assembly: Microsoft.TeamFoundation.TestImpact.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1ECF5BB1-1B8D-4502-95D9-1C6B9B1F7C03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestImpact.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Build.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Integration.Server;
using Microsoft.TeamFoundation.Server.Core.WebServices;
using Microsoft.TeamFoundation.TestImpact.Server.Common;
using System;
using System.Collections.Generic;
using System.Web.Services;

namespace Microsoft.TeamFoundation.TestImpact.Server
{
  [WebService(Namespace = "http://schemas.microsoft.com/TeamFoundation/2007/02/TCM/TestImpact/01", Description = "Test Impact Service", Name = "TestImpactService")]
  [ClientService(ComponentName = "TestManagement", RegistrationName = "TestManagement", ServiceName = "TestImpactService", CollectionServiceIdentifier = "FF3695CF-406C-46D9-96FD-70AA11D8681A")]
  public class TestImpactService : TeamFoundationWebService
  {
    [WebMethod]
    public void PublishBuildChanges(string buildUri, CodeChange[] changes)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (PublishBuildChanges), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (buildUri), (object) buildUri);
        methodInformation.AddArrayParameter<CodeChange>(nameof (changes), (IList<CodeChange>) changes);
        this.EnterMethod(methodInformation);
        string projectName;
        Uri buildDefinitionUri;
        this.GetBuildInfo(this.RequestContext, Utility.CheckUri(buildUri, nameof (buildUri), this.RequestContext.ServiceName), out projectName, out buildDefinitionUri);
        Uri projectUri = TestImpactService.GetProjectUri(this.RequestContext, projectName);
        this.RequestContext.GetService<Microsoft.TeamFoundation.TestImpact.Server.Common.SecurityManager>().DemandUpdateBuildAccess(this.RequestContext, projectUri, buildDefinitionUri);
        TestImpactServer.PublishBuildChanges(this.RequestContext, projectUri, buildUri, (IEnumerable<CodeChange>) changes);
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
    public void DeleteBuildImpact(string buildDefinitionUri, string buildUri)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (DeleteBuildImpact), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (buildDefinitionUri), (object) buildDefinitionUri);
        methodInformation.AddParameter(nameof (buildUri), (object) buildUri);
        this.EnterMethod(methodInformation);
        Utility.CheckUri(buildUri, nameof (buildUri), this.RequestContext.ServiceName);
        Uri buildDefinitionUri1 = Utility.CheckUri(buildDefinitionUri, nameof (buildDefinitionUri), this.RequestContext.ServiceName);
        Uri projectUri = TestImpactService.GetProjectUri(this.RequestContext, this.GetBuildDefinitionTeamProject(this.RequestContext, buildDefinitionUri1));
        this.RequestContext.GetService<Microsoft.TeamFoundation.TestImpact.Server.Common.SecurityManager>().DemandDeleteBuildsAccess(this.RequestContext, projectUri, buildDefinitionUri1);
        TestImpactServer.DeleteBuildImpact(this.RequestContext, buildUri);
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
    public List<CodeChange> QueryBuildCodeChanges(string buildUri)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryBuildCodeChanges), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (buildUri), (object) buildUri);
        this.EnterMethod(methodInformation);
        string projectName;
        this.GetBuildInfo(this.RequestContext, Utility.CheckUri(buildUri, nameof (buildUri), this.RequestContext.ServiceName), out projectName, out Uri _);
        Uri projectUri = TestImpactService.GetProjectUri(this.RequestContext, projectName);
        this.RequestContext.GetService<Microsoft.TeamFoundation.TestImpact.Server.Common.SecurityManager>().DemandProjectReadAccess(this.RequestContext, projectUri);
        return TestImpactServer.QueryBuildCodeChanges(this.RequestContext, buildUri);
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
    public BuildImpactedTests QueryImpactedTests(string buildUri)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryImpactedTests), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (buildUri), (object) buildUri);
        this.EnterMethod(methodInformation);
        string projectName;
        this.GetBuildInfo(this.RequestContext, Utility.CheckUri(buildUri, nameof (buildUri), this.RequestContext.ServiceName), out projectName, out Uri _);
        Uri projectUri = TestImpactService.GetProjectUri(this.RequestContext, projectName);
        this.RequestContext.GetService<Microsoft.TeamFoundation.TestImpact.Server.Common.SecurityManager>().DemandProjectReadAccess(this.RequestContext, projectUri);
        return TestImpactServer.QueryImpactedTests(this.RequestContext, projectName, buildUri);
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
    public TestSignatureData QueryTestCaseSignatures(
      string[] buildDefinitionUris,
      ClientTestInfo[] clientTests)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation("QueryTestCodeSignatures", MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<string>(nameof (buildDefinitionUris), (IList<string>) buildDefinitionUris);
        methodInformation.AddArrayParameter<ClientTestInfo>(nameof (clientTests), (IList<ClientTestInfo>) clientTests);
        this.EnterMethod(methodInformation);
        if (buildDefinitionUris == null || buildDefinitionUris.Length == 0)
          return new TestSignatureData();
        Microsoft.TeamFoundation.TestImpact.Server.Common.SecurityManager service = this.RequestContext.GetService<Microsoft.TeamFoundation.TestImpact.Server.Common.SecurityManager>();
        string definitionTeamProject = this.GetBuildDefinitionTeamProject(this.RequestContext, Utility.CheckUri(buildDefinitionUris[0], nameof (buildDefinitionUris), this.RequestContext.ServiceName));
        Uri projectUri1 = TestImpactService.GetProjectUri(this.RequestContext, definitionTeamProject);
        IVssRequestContext requestContext = this.RequestContext;
        Uri projectUri2 = projectUri1;
        service.DemandProjectReadAccess(requestContext, projectUri2);
        for (int index = 1; index < buildDefinitionUris.Length; ++index)
        {
          if (this.GetBuildDefinitionTeamProject(this.RequestContext, Utility.CheckUri(buildDefinitionUris[index], nameof (buildDefinitionUris), this.RequestContext.ServiceName)) != definitionTeamProject)
            return new TestSignatureData();
        }
        return TestImpactServer.QueryTestCaseSignatures(this.RequestContext, definitionTeamProject, (IList<string>) buildDefinitionUris, (IEnumerable<ClientTestInfo>) clientTests);
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

    public void GetBuildInfo(
      IVssRequestContext requestContext,
      Uri buildUri,
      out string projectName,
      out Uri buildDefinitionUri)
    {
      BuildDetail buildDetail = TestImpactServer.GetBuildDetail(requestContext, buildUri);
      projectName = buildDetail.TeamProject;
      buildDefinitionUri = new Uri(buildDetail.BuildDefinitionUri);
    }

    private string GetBuildDefinitionTeamProject(
      IVssRequestContext requestContext,
      Uri buildDefinitionUri)
    {
      return BuildPath.GetTeamProject(TestImpactServer.GetBuildDefinition(requestContext, buildDefinitionUri).FullPath);
    }

    internal static Uri GetProjectUri(IVssRequestContext requestContext, string projectName) => new Uri((requestContext.GetService<CommonStructureService>().GetProjectFromName(requestContext.Elevate(), projectName) ?? throw new InvalidProjectNameException()).Uri);
  }
}
