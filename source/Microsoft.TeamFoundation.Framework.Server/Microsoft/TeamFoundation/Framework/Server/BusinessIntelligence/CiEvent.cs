// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence.CiEvent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence
{
  public class CiEvent
  {
    public const string AreaParam = "area";
    public const string FeatureParam = "feature";

    public string Area { get; private set; }

    public string Feature { get; private set; }

    public Dictionary<string, object> Properties { get; private set; }

    public CiEvent(string area, string feature)
    {
      if (string.IsNullOrWhiteSpace(area))
        throw new ArgumentNullException(nameof (area));
      if (string.IsNullOrWhiteSpace(feature))
        throw new ArgumentNullException(nameof (feature));
      this.Area = area;
      this.Feature = feature;
      this.Properties = new Dictionary<string, object>();
    }

    public virtual void Publish(IVssRequestContext requestContext)
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData((IDictionary<string, object>) this.Properties);
      CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
      try
      {
        service.Publish(requestContext, this.Area, this.Feature, properties);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(522004, nameof (CiEvent), "BusinessIntelligence", ex);
      }
    }
  }
}
