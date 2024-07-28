// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Controllers.ApiCustomerIntelligenceController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Controllers
{
  [SupportedRouteArea("Api", NavigationContextLevels.All)]
  [DemandFeature("00000000-0000-0000-0000-000000000000", false)]
  public class ApiCustomerIntelligenceController : TfsController
  {
    private static string s_traceArea = "WebAccess.CustomerIntelligence";

    public override string TraceArea => ApiCustomerIntelligenceController.s_traceArea;

    [HttpPost]
    [TfsTraceFilter(500680, 500690)]
    public ActionResult Publish(string area, string feature, string property, string value)
    {
      this.TfsRequestContext.GetService<CustomerIntelligenceService>().Publish(this.TfsRequestContext, area, feature, property, value);
      return (ActionResult) this.Json((object) new JsObject());
    }

    [HttpPost]
    [TfsTraceFilter(500700, 500710)]
    public ActionResult PublishProperties(
      string area,
      string feature,
      [ModelBinder(typeof (DictionaryModelBinder))] IDictionary<string, object> properties)
    {
      CustomerIntelligenceData properties1 = new CustomerIntelligenceData(properties);
      this.TfsRequestContext.GetService<CustomerIntelligenceService>().Publish(this.TfsRequestContext, area, feature, properties1);
      return (ActionResult) this.Json((object) new JsObject());
    }

    [HttpPost]
    [TfsTraceFilter(500720, 500730)]
    public ActionResult PublishPropertiesBatch([ModelBinder(typeof (DictionaryModelBinder))] IDictionary<string, object>[] properties_array)
    {
      CustomerIntelligenceService service = this.TfsRequestContext.GetService<CustomerIntelligenceService>();
      foreach (IDictionary<string, object> properties1 in properties_array)
      {
        if (properties1.ContainsKey("properties") && properties1.ContainsKey("area") && properties1.ContainsKey("feature"))
        {
          CustomerIntelligenceData properties2 = new CustomerIntelligenceData((IDictionary<string, object>) properties1["properties"]);
          service.Publish(this.TfsRequestContext, (string) properties1["area"], (string) properties1["feature"], properties2);
        }
      }
      return (ActionResult) this.Json((object) new JsObject());
    }
  }
}
