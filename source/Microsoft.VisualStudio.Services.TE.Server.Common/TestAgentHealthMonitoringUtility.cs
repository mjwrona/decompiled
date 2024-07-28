// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestExecution.Server.TestAgentHealthMonitoringUtility
// Assembly: Microsoft.VisualStudio.Services.TE.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BC2680F-A5FB-41BE-A4CF-F78BF7AC3E02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.TE.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.TestExecution.Server
{
  [CLSCompliant(false)]
  public static class TestAgentHealthMonitoringUtility
  {
    public static IList<int> PerformAgentHealthMonitoring(TestExecutionRequestContext requestContext)
    {
      List<int> intList1 = new List<int>();
      List<int> intList2 = new List<int>();
      DtaLogger dtaLogger = new DtaLogger(requestContext);
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      TimeSpan valueFromTfsRegistry1 = Utilities.GetValueFromTfsRegistry<TimeSpan>(requestContext, DtaConstants.TfsRegistryPathForAgentConnectionTimeOut, DtaConstants.DefaultAgentConnectionTimeOut, TestAgentHealthMonitoringUtility.\u003C\u003EO.\u003C0\u003E__TryParse ?? (TestAgentHealthMonitoringUtility.\u003C\u003EO.\u003C0\u003E__TryParse = new ParseDelegate<TimeSpan>(TimeSpan.TryParse)));
      dtaLogger.Verbose(6201753, string.Format("Per agent connection timeout is {0} seconds.", (object) valueFromTfsRegistry1));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      TimeSpan valueFromTfsRegistry2 = Utilities.GetValueFromTfsRegistry<TimeSpan>(requestContext, DtaConstants.TfsRegistryPathForAllAgentsConnectionTimeOut, DtaConstants.DefaultAllAgentsConnectionTimeOut, TestAgentHealthMonitoringUtility.\u003C\u003EO.\u003C0\u003E__TryParse ?? (TestAgentHealthMonitoringUtility.\u003C\u003EO.\u003C0\u003E__TryParse = new ParseDelegate<TimeSpan>(TimeSpan.TryParse)));
      dtaLogger.Verbose(6201754, string.Format("All agents connection timeout is {0} seconds.", (object) valueFromTfsRegistry2));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      int valueFromTfsRegistry3 = Utilities.GetValueFromTfsRegistry<int>(requestContext, DtaConstants.TfsRegistryPathForMaxSliceRetryCount, DtaConstants.DefaultMaxSliceRetryCount, TestAgentHealthMonitoringUtility.\u003C\u003EO.\u003C1\u003E__TryParse ?? (TestAgentHealthMonitoringUtility.\u003C\u003EO.\u003C1\u003E__TryParse = new ParseDelegate<int>(int.TryParse)));
      dtaLogger.Verbose(6201755, string.Format("Maximum retry count per slice is {0}", (object) valueFromTfsRegistry3));
      using (DtaSliceDatabase component = requestContext.RequestContext.CreateComponent<DtaSliceDatabase>())
      {
        intList1 = component.RetrySlicesOfUnReachableAgentsAndGetTestRunIds((int) valueFromTfsRegistry1.TotalSeconds, valueFromTfsRegistry3).ToList<int>();
        if (intList1.Count > 0)
          dtaLogger.Error(6201756, string.Format("{0} runs have one or more agents down. Ids of the runs are [{1}]", (object) intList1.Count, (object) string.Join<int>(",", (IEnumerable<int>) intList1)));
        intList2 = component.AbortSlicesIfAllAgentsAreDownAndGetTestRunIds((int) valueFromTfsRegistry2.TotalSeconds).ToList<int>();
      }
      foreach (int num in intList2)
      {
        Dictionary<string, object> eventData = new Dictionary<string, object>()
        {
          {
            "TcmRunId",
            (object) num
          }
        };
        CILogger.Instance.PublishCI(requestContext, "AllAgentsDown", eventData);
      }
      List<int> list = intList1.Union<int>((IEnumerable<int>) intList2).ToList<int>();
      if (list.Count > 0)
        dtaLogger.Error(6201757, string.Format("{0} runs have all agents down. Ids of the runs are {1}", (object) intList2.Count, (object) string.Join<int>(",", (IEnumerable<int>) intList2)));
      return (IList<int>) list;
    }
  }
}
