// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.EmailTelemetryUtils
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class EmailTelemetryUtils
  {
    private const string c_feature = "SendMail";

    public static void TraceData(
      IVssRequestContext requestContext,
      string area,
      Dictionary<string, object> data)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(area, nameof (area));
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("ActivityId", (object) requestContext.ActivityId);
      foreach (KeyValuePair<string, object> keyValuePair in data)
        properties.Add(keyValuePair.Key, keyValuePair.Value);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "SendMail", area, properties);
    }
  }
}
