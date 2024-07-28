// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CustomerIntelligence
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal static class CustomerIntelligence
  {
    internal static void PublishEvent(
      IVssRequestContext requestContext,
      string feature,
      IEnumerable<KeyValue<string, string>> eventDataCollection)
    {
      if (eventDataCollection.IsNullOrEmpty<KeyValue<string, string>>())
        throw new ArgumentException(nameof (eventDataCollection));
      CustomerIntelligenceData eventData = new CustomerIntelligenceData();
      eventDataCollection.Where<KeyValue<string, string>>((Func<KeyValue<string, string>, bool>) (d => !string.IsNullOrEmpty(d.Key))).ToList<KeyValue<string, string>>().ForEach((Action<KeyValue<string, string>>) (dataItem => eventData.Add(dataItem.Key, dataItem.Value ?? string.Empty)));
      CustomerIntelligence.PublishEvent(requestContext, feature, eventData);
    }

    internal static void PublishEvent(
      IVssRequestContext requestContext,
      string feature,
      CustomerIntelligenceData eventData)
    {
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, CustomerIntelligenceArea.Commerce, feature, eventData);
    }
  }
}
