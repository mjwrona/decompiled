// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.BuildControllerWebService
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Web.Services;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  [WebService(Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/Build/BuildController/03", Description = "DevOps Build Controller web service")]
  [ClientService(ComponentName = "TeamBuild", RegistrationName = "Build", ServiceName = "BuildControllerService", CollectionServiceIdentifier = "36cffc58-f0d7-4b48-8e2d-6c79ab4447cb")]
  public sealed class BuildControllerWebService : BuildWebServiceBase
  {
    [WebMethod]
    public bool DeleteBuild([ClientType(typeof (Uri))] string buildUri, out string failureMessage)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (DeleteBuild), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (buildUri), (object) buildUri);
        this.EnterMethod(methodInformation);
        ArgumentValidation.CheckUri(nameof (buildUri), buildUri, false, (string) null);
        failureMessage = (string) null;
        RosarioHelper.DeleteBuilds(this.RequestContext, (IList<string>) new string[1]
        {
          buildUri
        }, DeleteOptions2010.All, true);
        return true;
      }
      catch (BuildServerException ex)
      {
        this.RequestContext.TraceException(0, "BuildAdministration", "Service", (Exception) ex);
        failureMessage = ex.Message;
        return false;
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(0, "BuildAdministration", "Service", ex);
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void InsertBuildQuality(string teamProject, string buildQuality)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (InsertBuildQuality), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (teamProject), (object) teamProject);
        methodInformation.AddParameter(nameof (buildQuality), (object) buildQuality);
        this.EnterMethod(methodInformation);
        ArgumentValidation.Check(nameof (teamProject), teamProject, false, (string) null);
        ArgumentValidation.Check(nameof (buildQuality), buildQuality, false, (string) null);
        this.BuildService.AddBuildQualities(this.RequestContext, teamProject, (IList<string>) new string[1]
        {
          buildQuality
        });
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(0, "BuildAdministration", "Service", ex);
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void RemoveBuildQuality(string teamProject, string buildQuality)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (RemoveBuildQuality), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (teamProject), (object) teamProject);
        methodInformation.AddParameter(nameof (buildQuality), (object) buildQuality);
        this.EnterMethod(methodInformation);
        ArgumentValidation.Check(nameof (teamProject), teamProject, false, (string) null);
        ArgumentValidation.Check(nameof (buildQuality), buildQuality, false, (string) null);
        this.BuildService.DeleteBuildQualities(this.RequestContext, teamProject, (IList<string>) new string[1]
        {
          buildQuality
        });
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(0, "BuildAdministration", "Service", ex);
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public bool RemoveRun([ClientType(typeof (Uri))] string buildUri, Guid runId)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (RemoveRun), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (buildUri), (object) buildUri);
        methodInformation.AddParameter(nameof (runId), (object) runId);
        this.EnterMethod(methodInformation);
        return true;
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(0, "BuildAdministration", "Service", ex);
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public string StartBuild(BuildParameters buildParameters)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (StartBuild), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (buildParameters), (object) buildParameters);
        this.EnterMethod(methodInformation);
        throw new NotSupportedException();
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(0, "BuildAdministration", "Service", ex);
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public bool StopBuild([ClientType(typeof (Uri))] string buildUri, out string failureMessage)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (StopBuild), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (buildUri), (object) buildUri);
        this.EnterMethod(methodInformation);
        ArgumentValidation.CheckUri(nameof (buildUri), buildUri, false, (string) null);
        failureMessage = (string) null;
        RosarioHelper.StopBuilds(this.RequestContext, (IList<string>) new string[1]
        {
          buildUri
        });
        return true;
      }
      catch (BuildServerException ex)
      {
        this.RequestContext.TraceException(0, "BuildAdministration", "Service", (Exception) ex);
        failureMessage = ex.Message;
        return false;
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(0, "BuildAdministration", "Service", ex);
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void BuildCompleted([ClientType(typeof (Uri))] string buildUri) => throw new NotImplementedException();

    [WebMethod]
    public void ReportBuildError([ClientType(typeof (Uri))] string buildUri, int exitCode) => throw new NotImplementedException();

    [WebMethod]
    public bool ValidateBuildStart(string teamProject, string buildType, string buildMachine) => throw new NotImplementedException();
  }
}
