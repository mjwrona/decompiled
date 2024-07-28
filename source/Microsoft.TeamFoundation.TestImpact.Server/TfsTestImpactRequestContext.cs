// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestImpact.Server.TfsTestImpactRequestContext
// Assembly: Microsoft.TeamFoundation.TestImpact.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1ECF5BB1-1B8D-4502-95D9-1C6B9B1F7C03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestImpact.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestImpact.Server.Common;
using Microsoft.TeamFoundation.TestManagement.WebApi;

namespace Microsoft.TeamFoundation.TestImpact.Server
{
  public class TfsTestImpactRequestContext : TestImpactRequestContext
  {
    public TfsTestImpactRequestContext(IVssRequestContext requestContext)
      : base(requestContext)
    {
    }

    public override ITestResultsHttpClient TestResultsHttpClient
    {
      get
      {
        if (this.m_testResultsClient == null)
          this.m_testResultsClient = (ITestResultsHttpClient) this.RequestContext.GetClient<TestManagementHttpClient>();
        return this.m_testResultsClient;
      }
      set => this.m_testResultsClient = value;
    }
  }
}
