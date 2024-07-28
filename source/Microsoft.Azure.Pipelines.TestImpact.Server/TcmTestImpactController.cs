// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.TestImpact.Server.TcmTestImpactController
// Assembly: Microsoft.Azure.Pipelines.TestImpact.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0F1F925-F1C1-4718-9E4B-3FB98FCCC30C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.TestImpact.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.TestImpact.Server.Common;
using Microsoft.TeamFoundation.TestImpact.WebApi.Contracts;
using System.Web.Http;

namespace Microsoft.Azure.Pipelines.TestImpact.Server
{
  [VersionedApiControllerCustomName(Area = "TestImpact", ResourceName = "Impact", ResourceVersion = 1)]
  public class TcmTestImpactController : TestImpactApiController
  {
    private ITfsTestImpactCatalogService _tfsTestImpactCatalogService;

    [HttpGet]
    public ImpactedTests QueryImpactedTests(
      [FromUri] DefinitionRunInfo definitionRunInfo,
      [ClientQueryParameter] int currentTestRunId,
      [ClientQueryParameter] TestInclusionOptions typesToInclude = TestInclusionOptions.None)
    {
      this.TestImpactRequestContext.RequestContext.TraceEnter(15113061, TestImpactServiceConstants.TestImpactArea, TestImpactServiceConstants.ControllerLayer, "TestImpactController:QueryImpactedTests ");
      try
      {
        this.ValidateInputs(this.TestImpactRequestContext, definitionRunInfo.DefinitionRunId, definitionRunInfo.BaseLineDefinitionRunId, currentTestRunId);
        return this.TfsTestImpactCatalogService.QueryImpactedTests(this.TestImpactRequestContext, this.ProjectId, definitionRunInfo, currentTestRunId, typesToInclude);
      }
      finally
      {
        this.TestImpactRequestContext.RequestContext.TraceLeave(15113080, TestImpactServiceConstants.TestImpactArea, TestImpactServiceConstants.ControllerLayer, "TestImpactController:QueryImpactedTests");
      }
    }

    [HttpGet]
    [PublicProjectRequestRestrictions]
    public BuildType QueryBuildType([ClientQueryParameter] int buildId)
    {
      this.TestImpactRequestContext.RequestContext.TraceEnter(15113061, TestImpactServiceConstants.TestImpactArea, TestImpactServiceConstants.ControllerLayer, "TestImpactController:QueryImpactedTests ");
      try
      {
        this.ValidateInputs(this.TestImpactRequestContext, buildId);
        return this.TfsTestImpactCatalogService.QueryTIAEnabledRun(this.TestImpactRequestContext, this.ProjectId, buildId);
      }
      finally
      {
        this.TestImpactRequestContext.RequestContext.TraceLeave(15113080, TestImpactServiceConstants.TestImpactArea, TestImpactServiceConstants.ControllerLayer, "TestImpactController:QueryImpactedTests");
      }
    }

    [HttpPatch]
    [ClientResponseType(typeof (TestResultSignaturesInfo), null, null)]
    public TestResultSignaturesInfo PublishCodeSignatures([FromBody] TestResultSignaturesInfo results)
    {
      IVssRequestContext requestContext = this.TestImpactRequestContext.RequestContext;
      this.TestImpactRequestContext.RequestContext.TraceEnter(15113091, TestImpactServiceConstants.TestImpactArea, TestImpactServiceConstants.ControllerLayer, "TestImpactController:PublishCodeSignatures ");
      try
      {
        if (results != null)
          this.TfsTestImpactCatalogService.PublishCodeSignatures(this.TestImpactRequestContext, this.ProjectId, results);
        return results;
      }
      finally
      {
        this.TestImpactRequestContext.RequestContext.TraceLeave(15113100, TestImpactServiceConstants.TestImpactArea, TestImpactServiceConstants.ControllerLayer, "TestImpactController:PublishCodeSignatures");
      }
    }

    public ITfsTestImpactCatalogService TfsTestImpactCatalogService
    {
      get => this._tfsTestImpactCatalogService ?? (this._tfsTestImpactCatalogService = (ITfsTestImpactCatalogService) this.TfsRequestContext.GetService<Microsoft.TeamFoundation.TestImpact.Server.Common.TfsTestImpactCatalogService>());
      set => this._tfsTestImpactCatalogService = value;
    }
  }
}
