// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestImpact.Server.Common.CILogger
// Assembly: Microsoft.TeamFoundation.TestImpact.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 079E4AEE-0642-4BDD-8B76-CECF38EBB798
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestImpact.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.TestImpact.Server.Common
{
  internal class CILogger : ICILogger
  {
    private static ICILogger _ciLogger;

    private CILogger()
    {
    }

    public void PublishCI(
      IVssRequestContext requestContext,
      string feature,
      Dictionary<string, object> eventData)
    {
      try
      {
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, this.GetServiceHostId(requestContext), TestImpactServiceCIArea.TestImpactService, feature, this.CreateCustomerIntelligenceData(eventData));
      }
      catch (Exception ex)
      {
        requestContext.Trace(15113998, TraceLevel.Warning, TestImpactServiceConstants.TestImpactArea, TestImpactServiceConstants.CIServiceLayer, "Publish CI for Feature {0} failed. Exception : {1}", (object) feature, (object) ex);
      }
    }

    private Guid GetServiceHostId(IVssRequestContext requestContext) => requestContext.ServiceHost.InstanceId;

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

    public static ICILogger Instance
    {
      get
      {
        if (CILogger._ciLogger == null)
          CILogger._ciLogger = (ICILogger) new CILogger();
        return CILogger._ciLogger;
      }
      set => CILogger._ciLogger = value;
    }
  }
}
