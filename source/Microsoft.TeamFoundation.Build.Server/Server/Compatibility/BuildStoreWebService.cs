// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.BuildStoreWebService
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Web.Services;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  [ClientService(ComponentName = "TeamBuild", RegistrationName = "Build", ServiceName = "BuildStoreService", CollectionServiceIdentifier = "c13c2a8e-4a9f-4fd4-8225-6e40cc733787")]
  [WebService(Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/Build/BuildInfo/03", Description = "DevOps Build Info web service")]
  public sealed class BuildStoreWebService : BuildWebServiceBase
  {
    [WebMethod]
    [return: ClientType(typeof (Uri))]
    public string GetBuildUri(string teamProject, string buildNumber)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (GetBuildUri), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (teamProject), (object) teamProject);
        methodInformation.AddParameter(nameof (buildNumber), (object) buildNumber);
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
    public BuildData[] GetListOfBuilds(string teamProject, string buildType)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (GetListOfBuilds), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (teamProject), (object) teamProject);
        methodInformation.AddParameter(nameof (buildType), (object) buildType);
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
    public BuildData GetBuildDetails([ClientType(typeof (Uri))] string buildUri)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (GetBuildDetails), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (buildUri), (object) buildUri);
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
    public string[] GetBuildQualities()
    {
      try
      {
        this.EnterMethod(new MethodInformation(nameof (GetBuildQualities), MethodType.Normal, EstimatedMethodCost.Low));
        return this.BuildService.GetBuildQualities(this.RequestContext, (string) null).ToArray();
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
    public PlatformFlavorData[] GetPlatformFlavorsForBuild([ClientType(typeof (Uri))] string buildUri)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (GetPlatformFlavorsForBuild), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (buildUri), (object) buildUri);
        this.EnterMethod(methodInformation);
        ArgumentValidation.CheckUri(nameof (buildUri), buildUri, false, (string) null);
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
    public ChangeSetData[] GetChangeSetsForBuild([ClientType(typeof (Uri))] string buildUri)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (GetChangeSetsForBuild), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (buildUri), (object) buildUri);
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
    public WorkItemData[] GetWorkItemsForBuild([ClientType(typeof (Uri))] string buildUri)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (GetWorkItemsForBuild), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (buildUri), (object) buildUri);
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
    public WorkItemData[] GetOpenedWorkItemsForBuild([ClientType(typeof (Uri))] string buildUri)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (GetOpenedWorkItemsForBuild), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (buildUri), (object) buildUri);
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
    public TestResultData[] GetTestResultsForBuild([ClientType(typeof (Uri))] string buildUri, string platform, string flavor)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (GetTestResultsForBuild), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (buildUri), (object) buildUri);
        methodInformation.AddParameter(nameof (platform), (object) platform);
        methodInformation.AddParameter(nameof (flavor), (object) flavor);
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
    public CodeCoverageData[] GetCodeCoverageForBuild(
      [ClientType(typeof (Uri))] string buildUri,
      string platform,
      string flavor)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (GetCodeCoverageForBuild), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (buildUri), (object) buildUri);
        methodInformation.AddParameter(nameof (platform), (object) platform);
        methodInformation.AddParameter(nameof (flavor), (object) flavor);
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
    public CodeCoverageStatus GetCodeCoverageStatusForBuild(
      [ClientType(typeof (Uri))] string buildUri,
      string platform,
      string flavor)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (GetCodeCoverageStatusForBuild), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (buildUri), (object) buildUri);
        methodInformation.AddParameter(nameof (platform), (object) platform);
        methodInformation.AddParameter(nameof (flavor), (object) flavor);
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
    public void UpdateBuildQuality([ClientType(typeof (Uri))] string buildUri, string buildQuality)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UpdateBuildQuality), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (buildUri), (object) buildUri);
        methodInformation.AddParameter(nameof (buildQuality), (object) buildQuality);
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
    public CompilationSummaryData GetCompilationSummaryForBuild(
      [ClientType(typeof (Uri))] string buildUri,
      string platform,
      string flavor)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (GetCompilationSummaryForBuild), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (buildUri), (object) buildUri);
        methodInformation.AddParameter(nameof (platform), (object) platform);
        methodInformation.AddParameter(nameof (flavor), (object) flavor);
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
    public BuildStepData[] GetBuildSteps([ClientType(typeof (Uri))] string buildUri)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (GetBuildSteps), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (buildUri), (object) buildUri);
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
    public string AddBuild(string teamProject, BuildData buildData)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (AddBuild), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (teamProject), (object) teamProject);
        methodInformation.AddParameter(nameof (buildData), (object) buildData);
        this.EnterMethod(methodInformation);
        WhidbeyHelper.CheckBuildData(nameof (buildData), buildData, false);
        return WhidbeyHelper.AddBuild(this.RequestContext, teamProject, buildData);
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
    public void AddChangeSetsForBuild([ClientType(typeof (Uri))] string buildUri, ChangeSetData[] changesets) => throw new NotImplementedException();

    [WebMethod]
    public void AddOpenedWorkItemsForBuild([ClientType(typeof (Uri))] string buildUri, WorkItemData[] workItems) => throw new NotImplementedException();

    [WebMethod]
    public void AddWorkItemsForBuild([ClientType(typeof (Uri))] string buildUri, WorkItemData[] workItems) => throw new NotImplementedException();

    [WebMethod]
    public void UpdateBuildNumberAndDropLocation(
      [ClientType(typeof (Uri))] string buildUri,
      string buildNumber,
      string dropLocation)
    {
      throw new NotImplementedException();
    }

    [WebMethod]
    public void UpdateBuildFlag([ClientType(typeof (Uri))] string buildUri, bool isGoodBuild) => throw new NotImplementedException();

    [WebMethod]
    public void UpdateBuildLogLocation([ClientType(typeof (Uri))] string buildUri, string logLocation) => throw new NotImplementedException();

    [WebMethod]
    public void UpdateBuildFinishTime([ClientType(typeof (Uri))] string buildUri, DateTime finishTime) => throw new NotImplementedException();

    [WebMethod]
    public void UpdateBuildStatus([ClientType(typeof (Uri))] string buildUri, string buildStatus) => throw new NotImplementedException();

    [WebMethod]
    public void AddProjectDetailsForBuild([ClientType(typeof (Uri))] string buildUri, ProjectData project) => throw new NotImplementedException();

    [WebMethod]
    public void AddBuildStep(
      [ClientType(typeof (Uri))] string buildUri,
      string buildStepName,
      string stepMessage,
      DateTime startTime,
      long parentBuildStepId,
      out long buildStepId)
    {
      throw new NotImplementedException();
    }

    [WebMethod]
    public void UpdateBuildStep(
      [ClientType(typeof (Uri))] string buildUri,
      string buildStepName,
      DateTime finishTime,
      BuildStepStatus status,
      long buildStepId)
    {
      throw new NotImplementedException();
    }

    [WebMethod]
    public void AddPlatformFlavorLogForBuild(
      [ClientType(typeof (Uri))] string buildUri,
      string platform,
      string flavor,
      string logFileName)
    {
      throw new NotImplementedException();
    }
  }
}
