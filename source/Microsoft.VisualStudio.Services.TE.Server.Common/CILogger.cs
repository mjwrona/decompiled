// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestExecution.Server.CILogger
// Assembly: Microsoft.VisualStudio.Services.TE.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BC2680F-A5FB-41BE-A4CF-F78BF7AC3E02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.TE.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestExecution.Server
{
  [CLSCompliant(false)]
  public class CILogger : ICILogger
  {
    static CILogger() => CILogger.Instance = (ICILogger) new CILogger();

    public void PublishCI(
      TestExecutionRequestContext requestContext,
      string feature,
      Dictionary<string, object> eventData)
    {
      DtaLogger dtaLogger = new DtaLogger(requestContext);
      try
      {
        CustomerIntelligenceService service = requestContext.RequestContext.GetService<CustomerIntelligenceService>();
        Guid instanceId = requestContext.RequestContext.ServiceHost.InstanceId;
        CustomerIntelligenceData intelligenceData = this.CreateCustomerIntelligenceData(eventData);
        IVssRequestContext requestContext1 = requestContext.RequestContext;
        Guid hostId = instanceId;
        string feature1 = feature;
        CustomerIntelligenceData properties = intelligenceData;
        service.Publish(requestContext1, hostId, "TestExecutionService", feature1, properties);
        dtaLogger.Verbose(6209999, "CIData : " + eventData.Serialize<Dictionary<string, object>>());
      }
      catch (Exception ex)
      {
        dtaLogger.Error(6209998, string.Format("Publish CI for Feature {0} failed. Exception : {1}", (object) feature, (object) ex));
      }
    }

    private CustomerIntelligenceData CreateCustomerIntelligenceData(
      Dictionary<string, object> eventData)
    {
      CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
      if (eventData == null)
        return (CustomerIntelligenceData) null;
      foreach (KeyValuePair<string, object> keyValuePair in eventData)
        intelligenceData.Add(keyValuePair.Key, keyValuePair.Value);
      return intelligenceData;
    }

    public static ICILogger Instance { get; set; }
  }
}
