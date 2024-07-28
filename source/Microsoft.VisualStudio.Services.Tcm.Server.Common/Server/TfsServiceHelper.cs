// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TfsServiceHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TfsServiceHelper : ITfsServiceHelper
  {
    private IVssRequestContext m_requestContext;
    private TestManagementHttpClient m_testManagementHttpClient;

    public TfsServiceHelper(IVssRequestContext context) => this.m_requestContext = context;

    public bool TryGetTestResultRetentionSettings(
      TestManagementRequestContext context,
      Guid projectId,
      out ResultRetentionSettings retentionSettings)
    {
      using (PerfManager.Measure(context.RequestContext, "CrossService", TraceUtils.GetActionName(nameof (TryGetTestResultRetentionSettings), "TestManagement")))
      {
        retentionSettings = (ResultRetentionSettings) null;
        try
        {
          if (this.TestManagementClient != null)
          {
            retentionSettings = this.TestManagementClient.GetResultRetentionSettingsAsync(projectId)?.Result;
            return true;
          }
        }
        catch (Exception ex)
        {
          this.m_requestContext.TraceException("CrossService", ex);
        }
        return false;
      }
    }

    internal TestManagementHttpClient TestManagementClient
    {
      get
      {
        if (this.m_testManagementHttpClient == null)
          this.m_testManagementHttpClient = this.GetTestManagementHttpClient();
        return this.m_testManagementHttpClient;
      }
    }

    private TestManagementHttpClient GetTestManagementHttpClient()
    {
      try
      {
        return this.m_requestContext.GetClient<TestManagementHttpClient>();
      }
      catch (Exception ex)
      {
        this.m_requestContext.TraceException("CrossService", ex);
        throw;
      }
    }
  }
}
