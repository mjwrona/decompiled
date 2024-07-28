// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestImpact.Server.TestImpactApiController
// Assembly: Microsoft.TeamFoundation.TestImpact.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1ECF5BB1-1B8D-4502-95D9-1C6B9B1F7C03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestImpact.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Integration.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.TestImpact.Server.Common;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.TestImpact.Server
{
  public abstract class TestImpactApiController : TfsProjectApiController
  {
    private TestImpactRequestContext _testImpactRequestContext;

    internal static Uri GetProjectUri(IVssRequestContext requestContext, Guid projectName) => new Uri((requestContext.GetService<ICommonStructureService>().GetProject(requestContext.Elevate(), projectName) ?? throw new InvalidProjectNameException()).Uri);

    public TestImpactRequestContext TestImpactRequestContext
    {
      get
      {
        if (this._testImpactRequestContext == null)
          this._testImpactRequestContext = (TestImpactRequestContext) new TfsTestImpactRequestContext(this.TfsRequestContext);
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
      if (runId != int.MinValue)
        ArgumentUtility.CheckGreaterThanZero((float) runId, nameof (runId), testImpactRequestContext.RequestContext.ServiceName);
      ArgumentUtility.CheckForEmptyGuid(this.ProjectId, "projectId", testImpactRequestContext.RequestContext.ServiceName);
    }
  }
}
