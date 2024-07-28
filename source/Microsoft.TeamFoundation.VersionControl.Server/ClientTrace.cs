// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ClientTrace
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal static class ClientTrace
  {
    public static void Publish(
      IVssRequestContext requestContext,
      string command,
      ClientTraceData ctData = null)
    {
      ctData = ctData ?? new ClientTraceData();
      requestContext.GetService<ClientTraceService>().Publish(requestContext, "VersionControl", command, ctData);
    }

    public static void Publish(
      IVssRequestContext requestContext,
      string command,
      CustomerIntelligenceData ciData)
    {
      ClientTrace.Publish(requestContext, command, ClientTrace.ToCTData(ciData));
    }

    public static ClientTraceData ToCTData(CustomerIntelligenceData ciData)
    {
      ClientTraceData ctData = new ClientTraceData();
      foreach (KeyValuePair<string, object> keyValuePair in (IEnumerable<KeyValuePair<string, object>>) ciData.GetData())
        ctData.Add(keyValuePair.Key, keyValuePair.Value);
      return ctData;
    }
  }
}
