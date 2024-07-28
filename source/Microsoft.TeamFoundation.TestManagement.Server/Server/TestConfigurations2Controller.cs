// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestConfigurations2Controller
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ControllerApiVersion(2.0)]
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "Configurations", ResourceVersion = 2)]
  public class TestConfigurations2Controller : TestConfigurationsController
  {
    [HttpGet]
    [ClientResponseType(typeof (IPagedList<Microsoft.TeamFoundation.TestManagement.WebApi.TestConfiguration>), null, null)]
    [ClientLocationId("d667591b-b9fd-4263-997a-9a084cca848f")]
    [ClientExample("GET__test_configurations.json", "Get a list of test configurations", null, null)]
    [ClientExample("GET__test_configurations__top-2.json", "A page at a time", null, null)]
    [ClientExample("GET__test_configurations__continuationToken.json", "With continuation token", null, null)]
    [ClientExample("GET__test_configurations__includeAllProperties.json", "Include all properties of the test configurations", null, null)]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    public override HttpResponseMessage GetTestConfigurations(
      [FromUri(Name = "$skip")] int skip = 0,
      [FromUri(Name = "$top")] int top = 0,
      string continuationToken = null,
      bool includeAllProperties = false)
    {
      ArgumentUtility.CheckGreaterThanOrEqualToZero((float) skip, nameof (skip), this.TfsRequestContext.ServiceName);
      ArgumentUtility.CheckGreaterThanOrEqualToZero((float) top, nameof (top), this.TfsRequestContext.ServiceName);
      int topToFetch;
      int watermark;
      int topRemaining;
      Utils.SetParametersForPaging(skip, top, continuationToken, out int _, out topToFetch, out watermark, out topRemaining);
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestConfiguration> configurationsWithPaging = this.TestConfigurationsHelper.GetTestConfigurationsWithPaging(this.ProjectInfo.Name, skip, topToFetch, watermark, includeAllProperties);
      continuationToken = (string) null;
      if (configurationsWithPaging.Count >= topToFetch && configurationsWithPaging[topToFetch - 1] != null)
      {
        continuationToken = Utils.GenerateContinuationToken(configurationsWithPaging[topToFetch - 1].Id, topRemaining);
        configurationsWithPaging.RemoveAt(topToFetch - 1);
      }
      HttpResponseMessage response = this.GenerateResponse<Microsoft.TeamFoundation.TestManagement.WebApi.TestConfiguration>((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestConfiguration>) configurationsWithPaging);
      if (continuationToken != null)
        Utils.SetContinuationToken(response, continuationToken);
      return response;
    }
  }
}
