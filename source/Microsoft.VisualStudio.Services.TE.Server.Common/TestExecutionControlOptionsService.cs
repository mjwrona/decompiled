// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestExecution.Server.TestExecutionControlOptionsService
// Assembly: Microsoft.VisualStudio.Services.TE.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BC2680F-A5FB-41BE-A4CF-F78BF7AC3E02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.TE.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Test.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestExecution.Server
{
  [CLSCompliant(false)]
  public class TestExecutionControlOptionsService : 
    ITestExecutionControlOptionsService,
    IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public TestExecutionControlOptions GetTestExecutionControlOptions(
      TestExecutionRequestContext teamFoundationRequestContext,
      string envUrl)
    {
      IDictionary<string, object> dictionary = TestRunPropertiesService.PropertyServiceHelper.Get(teamFoundationRequestContext, teamFoundationRequestContext.AutomationRunArtifactKindId, envUrl, (IEnumerable<string>) null);
      bool result = false;
      object obj;
      if (dictionary != null && dictionary.TryGetValue(TestPropertiesConstants.AutomationRunExecutionFlowInTcm, out obj))
      {
        bool.TryParse(obj.ToString(), out result);
      }
      else
      {
        result = false;
        if (teamFoundationRequestContext.RequestContext.IsFeatureEnabled(DtaConstants.UseTcmServiceFeatureFlag) && teamFoundationRequestContext.RequestContext.ExecutionEnvironment.IsHostedDeployment)
          result = true;
        IDictionary<string, object> properties = (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            TestPropertiesConstants.AutomationRunExecutionFlowInTcm,
            (object) result
          }
        };
        TestRunPropertiesService.PropertyServiceHelper.AddOrUpdate(teamFoundationRequestContext, teamFoundationRequestContext.AutomationRunArtifactKindId, envUrl, properties);
      }
      return new TestExecutionControlOptions()
      {
        UseTcmService = result
      };
    }
  }
}
