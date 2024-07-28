// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.TestImpact.Server.TcmTestBuildChangesController
// Assembly: Microsoft.Azure.Pipelines.TestImpact.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0F1F925-F1C1-4718-9E4B-3FB98FCCC30C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.TestImpact.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestImpact.Server.Common;
using Microsoft.TeamFoundation.TestImpact.WebApi.Contracts;
using System.Diagnostics;
using System.Web.Http;

namespace Microsoft.Azure.Pipelines.TestImpact.Server
{
  [VersionedApiControllerCustomName(Area = "TestImpact", ResourceName = "Change", ResourceVersion = 1)]
  public class TcmTestBuildChangesController : TestImpactApiController
  {
    private ITestBuildChangesCatalogService _tfsTestImpactCatalogService;

    [HttpPost]
    [ClientResponseType(typeof (TestImpactBuildData), null, null)]
    public TestImpactBuildData PublishTestImpactBuildData(
      [FromUri] DefinitionRunInfo definitionRunInfo,
      [FromBody] TestImpactBuildData testImapctBuildData)
    {
      IVssRequestContext requestContext = this.TestImpactRequestContext.RequestContext;
      requestContext.TraceEnter(15113001, TestImpactServiceConstants.TestImpactArea, TestImpactServiceConstants.ControllerLayer, "TestImpactController:PublishBuildChanges");
      try
      {
        this.ValidateInputs(this.TestImpactRequestContext, definitionRunInfo.DefinitionRunId);
        if (Utility.CheckCodeChanges(testImapctBuildData.CodeChanges))
        {
          requestContext.Trace(15113002, TraceLevel.Info, TestImpactServiceConstants.TestImpactArea, TestImpactServiceConstants.ServiceLayer, "PublishBuildChanges: Using empty code change list for publishing build code changes");
          testImapctBuildData.CodeChanges = new Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.CodeChange[0];
        }
        this.TestBuildChangesCatalogService.PublishBuildChanges(this.TestImpactRequestContext, this.ProjectId, definitionRunInfo, testImapctBuildData);
        return testImapctBuildData;
      }
      finally
      {
        requestContext.TraceLeave(15113020, TestImpactServiceConstants.TestImpactArea, TestImpactServiceConstants.ControllerLayer, "TestImpactController:PublishBuildChanges");
      }
    }

    [HttpGet]
    public TestImpactBuildData QueryCodeChanges([FromUri] DefinitionRunInfo definitionRunInfo)
    {
      IVssRequestContext requestContext = this.TestImpactRequestContext.RequestContext;
      try
      {
        this.ValidateInputs(this.TestImpactRequestContext, definitionRunInfo.DefinitionRunId);
        return this.TestBuildChangesCatalogService.QueryCodeChanges(this.TestImpactRequestContext, this.ProjectId, definitionRunInfo);
      }
      finally
      {
        requestContext.TraceLeave(15113060, TestImpactServiceConstants.TestImpactArea, TestImpactServiceConstants.ControllerLayer, "TestImpactController:PublishBuildChanges");
      }
    }

    public ITestBuildChangesCatalogService TestBuildChangesCatalogService
    {
      get => this._tfsTestImpactCatalogService ?? (this._tfsTestImpactCatalogService = this.TfsRequestContext.GetService<ITestBuildChangesCatalogService>());
      set => this._tfsTestImpactCatalogService = value;
    }
  }
}
