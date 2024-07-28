// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DataTierSigningConsumer
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class DataTierSigningConsumer : ISigningServiceConsumer
  {
    private static readonly string s_area = "DataTierService";
    private static readonly string s_layer = "IVssFrameworkService";

    public IEnumerable<Guid> GetSigningKeysInUse(IVssRequestContext requestContext)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return (IEnumerable<Guid>) Array.Empty<Guid>();
      using (DataTierComponent component = requestContext.CreateComponent<DataTierComponent>())
        return (IEnumerable<Guid>) component.QueryDataTierSigningKeys();
    }

    public ReencryptResults Reencrypt(IVssRequestContext requestContext, Guid identifier)
    {
      ReencryptResults reencryptResults = new ReencryptResults();
      requestContext.Trace(9700, TraceLevel.Verbose, DataTierSigningConsumer.s_area, DataTierSigningConsumer.s_layer, "Re-encrypting data tier passwords");
      using (DataTierComponent component = requestContext.CreateComponent<DataTierComponent>())
      {
        foreach (DataTierInfo dataTierInfo in component.GetDataTierInfo())
        {
          try
          {
            if (dataTierInfo.SigningKeyId != identifier)
            {
              if (dataTierInfo.ConnectionInfo is ISupportSqlCredential connectionInfo)
              {
                requestContext.Trace(9701, TraceLevel.Verbose, DataTierSigningConsumer.s_area, DataTierSigningConsumer.s_layer, "Re-encrypting data tier {0}", (object) dataTierInfo.DataSource);
                byte[] byteArray = connectionInfo.Password.ToByteArray();
                component.PendDataTierReset(dataTierInfo.ConnectionInfo, Encoding.UTF8.GetString(byteArray), identifier);
                component.FlushDataTierReset(dataTierInfo.ConnectionInfo);
                ++reencryptResults.SuccessCount;
              }
            }
          }
          catch (Exception ex)
          {
            requestContext.TraceException(9702, DataTierSigningConsumer.s_area, DataTierSigningConsumer.s_layer, ex);
            reencryptResults.Failures.Add(new Exception(string.Format("Error re-encrypting data tier {0}: {1}", dataTierInfo == null ? (object) "<null>" : (object) dataTierInfo.DataSource, (object) ex), ex));
            ++reencryptResults.FailureCount;
          }
        }
      }
      return reencryptResults;
    }
  }
}
