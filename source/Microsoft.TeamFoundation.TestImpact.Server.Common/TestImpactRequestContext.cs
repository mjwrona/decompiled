// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestImpact.Server.Common.TestImpactRequestContext
// Assembly: Microsoft.TeamFoundation.TestImpact.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 079E4AEE-0642-4BDD-8B76-CECF38EBB798
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestImpact.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.TestImpact.Server.Common
{
  [CLSCompliant(false)]
  public class TestImpactRequestContext
  {
    public ITestResultsHttpClient m_testResultsClient;

    public TestImpactRequestContext(IVssRequestContext tfsRequestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(tfsRequestContext, nameof (tfsRequestContext));
      this.RequestContext = tfsRequestContext;
    }

    public virtual ITestResultsHttpClient TestResultsHttpClient
    {
      get
      {
        if (this.m_testResultsClient == null)
          this.m_testResultsClient = (ITestResultsHttpClient) this.RequestContext.GetClient<Microsoft.VisualStudio.Services.TestResults.WebApi.TestResultsHttpClient>();
        return this.m_testResultsClient;
      }
      set => this.m_testResultsClient = value;
    }

    public IVssRequestContext RequestContext { get; private set; }
  }
}
