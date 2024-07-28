// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Web.JsonNetResult
// Assembly: Microsoft.VisualStudio.Services.Gallery.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17D36576-2EF3-4ABC-94BA-AF7891D15A3A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Web.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.VisualStudio.Services.Gallery.Web
{
  public class JsonNetResult : JsonResult
  {
    private static readonly JsonSerializerSettings settings = new JsonSerializerSettings()
    {
      ContractResolver = (IContractResolver) new CamelCasePropertyNamesContractResolver()
    };

    public JsonNetResult() => this.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

    public override void ExecuteResult(ControllerContext context)
    {
      HttpResponseBase response = context.HttpContext.Response;
      response.ContentType = !string.IsNullOrEmpty(this.ContentType) ? this.ContentType : "application/json";
      if (this.ContentEncoding != null)
        response.ContentEncoding = this.ContentEncoding;
      if (this.Data == null)
        return;
      response.Write(JsonConvert.SerializeObject(this.Data, JsonNetResult.settings));
    }
  }
}
