// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.BuildCoverageHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class BuildCoverageHelper : RestApiHelper
  {
    internal BuildCoverageHelper(TestManagementRequestContext requestContext)
      : base(requestContext)
    {
    }

    public List<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage> GetBuildCoverage(
      string projectId,
      int runId,
      string buildUri,
      Microsoft.TeamFoundation.TestManagement.Client.CoverageQueryFlags flags)
    {
      this.RequestContext.TraceInfo("RestLayer", "BuildCoverageHelper.GetBuildCoverage projectId = {0}, buildUri = {1}, flags = {2}", (object) projectId, (object) buildUri, (object) flags);
      return this.ExecuteAction<List<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage>>("BuildCoverageHelper.GetBuildCoverage", (Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage>>) (() =>
      {
        string name = this.GetProjectReference(projectId).Name;
        this.CheckForViewTestResultPermission(name);
        List<BuildCoverage> buildCoverageList = BuildCoverage.Query(this.TestManagementRequestContext, name, buildUri, flags);
        List<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage> buildCoverage1 = new List<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage>();
        foreach (BuildCoverage buildCoverage2 in buildCoverageList)
          buildCoverage1.Add(this.ConvertBuildCoverageToDataContract(buildCoverage2));
        return buildCoverage1;
      }), 1015055, "TestManagement");
    }

    private Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage ConvertBuildCoverageToDataContract(
      BuildCoverage buildCoverage)
    {
      Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage dataContract = new Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage();
      dataContract.LastError = buildCoverage.LastError;
      dataContract.State = buildCoverage.State.ToString();
      dataContract.Configuration = this.ConvertBuildConfigurationToDataContract(buildCoverage.Configuration);
      dataContract.Modules = new List<Microsoft.TeamFoundation.TestManagement.WebApi.ModuleCoverage>();
      foreach (ModuleCoverage module in buildCoverage.Modules)
        dataContract.Modules.Add(this.ConvertModuleToDataContract(module));
      return dataContract;
    }

    private Microsoft.TeamFoundation.TestManagement.WebApi.BuildConfiguration ConvertBuildConfigurationToDataContract(
      BuildConfiguration buildConfiguration)
    {
      return new Microsoft.TeamFoundation.TestManagement.WebApi.BuildConfiguration()
      {
        Id = buildConfiguration.BuildConfigurationId,
        Uri = buildConfiguration.BuildUri,
        Flavor = buildConfiguration.BuildFlavor,
        Platform = buildConfiguration.BuildPlatform,
        Project = new ShallowReference()
        {
          Name = buildConfiguration.TeamProjectName
        }
      };
    }

    private Microsoft.TeamFoundation.TestManagement.WebApi.ModuleCoverage ConvertModuleToDataContract(
      ModuleCoverage module)
    {
      Microsoft.TeamFoundation.TestManagement.WebApi.ModuleCoverage dataContract = new Microsoft.TeamFoundation.TestManagement.WebApi.ModuleCoverage();
      dataContract.BlockCount = module.BlockCount;
      dataContract.BlockData = module.BlockData;
      dataContract.Name = module.Name;
      dataContract.Signature = module.Signature;
      dataContract.SignatureAge = module.SignatureAge;
      dataContract.Statistics = this.ConvertStatisticsToDataContract(module.Statistics);
      dataContract.Functions = new List<Microsoft.TeamFoundation.TestManagement.WebApi.FunctionCoverage>();
      foreach (FunctionCoverage function in module.Functions)
        dataContract.Functions.Add(this.ConvertFunctionToDataContract(function));
      return dataContract;
    }

    private Microsoft.TeamFoundation.TestManagement.WebApi.CoverageStatistics ConvertStatisticsToDataContract(
      CoverageStatistics statistics)
    {
      return new Microsoft.TeamFoundation.TestManagement.WebApi.CoverageStatistics()
      {
        BlocksCovered = statistics.BlocksCovered,
        BlocksNotCovered = statistics.BlocksNotCovered,
        LinesCovered = statistics.LinesCovered,
        LinesNotCovered = statistics.LinesNotCovered,
        LinesPartiallyCovered = statistics.LinesPartiallyCovered
      };
    }

    private Microsoft.TeamFoundation.TestManagement.WebApi.FunctionCoverage ConvertFunctionToDataContract(
      FunctionCoverage function)
    {
      return new Microsoft.TeamFoundation.TestManagement.WebApi.FunctionCoverage()
      {
        Class = function.Class,
        Name = function.Name,
        Namespace = function.Namespace,
        SourceFile = function.SourceFile,
        Statistics = this.ConvertStatisticsToDataContract(function.Statistics)
      };
    }
  }
}
