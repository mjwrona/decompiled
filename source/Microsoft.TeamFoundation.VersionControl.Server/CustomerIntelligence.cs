// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CustomerIntelligence
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal static class CustomerIntelligence
  {
    public static void Publish(
      IVssRequestContext requestContext,
      string command,
      CustomerIntelligenceData ciData = null)
    {
      ciData = ciData ?? new CustomerIntelligenceData();
      ciData.Add(command, true);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "VersionControl", "TFVC", ciData);
    }

    public static void Publish(
      IVssRequestContext requestContext,
      string command,
      ClientTraceData ctData)
    {
      CustomerIntelligence.Publish(requestContext, command, CustomerIntelligence.ToCIData(ctData));
    }

    public static CustomerIntelligenceData ToCIData(ClientTraceData ctData)
    {
      CustomerIntelligenceData ciData = new CustomerIntelligenceData();
      foreach (KeyValuePair<string, object> keyValuePair in (IEnumerable<KeyValuePair<string, object>>) ctData.GetData())
        ciData.Add(keyValuePair.Key, keyValuePair.Value);
      return ciData;
    }
  }
}
