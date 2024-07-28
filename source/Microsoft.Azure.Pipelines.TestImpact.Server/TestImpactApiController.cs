// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.TestImpact.Server.TestImpactApiController
// Assembly: Microsoft.Azure.Pipelines.TestImpact.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0F1F925-F1C1-4718-9E4B-3FB98FCCC30C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.TestImpact.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.TestImpact.Server.Common;
using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.Azure.Pipelines.TestImpact.Server
{
  public abstract class TestImpactApiController : TfsProjectApiController
  {
    private TestImpactRequestContext _testImpactRequestContext;

    public TestImpactRequestContext TestImpactRequestContext
    {
      get
      {
        if (this._testImpactRequestContext == null)
          this._testImpactRequestContext = new TestImpactRequestContext(this.TfsRequestContext);
        return this._testImpactRequestContext;
      }
      set => this._testImpactRequestContext = value;
    }

    public override string TraceArea => TestImpactServiceConstants.TestImpactArea;

    public override string ActivityLogArea => TestImpactServiceConstants.TestImpactArea;

    internal void ValidateInputs(
      TestImpactRequestContext testImpactRequestContext,
      int currentBuildId,
      int baseLineBuildId = -2147483648,
      int runId = -2147483648)
    {
      ArgumentUtility.CheckForNull<TestImpactRequestContext>(testImpactRequestContext, nameof (testImpactRequestContext));
      ArgumentUtility.CheckForNull<IVssRequestContext>(testImpactRequestContext.RequestContext, "requestContext");
      ArgumentUtility.CheckGreaterThanZero((float) currentBuildId, nameof (currentBuildId), testImpactRequestContext.RequestContext.ServiceName);
      if (baseLineBuildId != int.MinValue)
        ArgumentUtility.CheckGreaterThanOrEqualToZero((float) baseLineBuildId, nameof (baseLineBuildId), testImpactRequestContext.RequestContext.ServiceName);
      if (runId == int.MinValue)
        return;
      ArgumentUtility.CheckGreaterThanZero((float) runId, nameof (runId), testImpactRequestContext.RequestContext.ServiceName);
    }
  }
}
