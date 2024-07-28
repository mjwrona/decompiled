// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.ODataFormattingAttribute
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Formatter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Services;

namespace Microsoft.AspNet.OData
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
  public class ODataFormattingAttribute : Attribute, IControllerConfiguration
  {
    public void Initialize(
      HttpControllerSettings controllerSettings,
      HttpControllerDescriptor controllerDescriptor)
    {
      if (controllerSettings == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (controllerSettings));
      if (controllerDescriptor == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (controllerDescriptor));
      MediaTypeFormatterCollection formatters = controllerSettings.Formatters;
      if (!formatters.Where<MediaTypeFormatter>((Func<MediaTypeFormatter, bool>) (f => f != null && Decorator.GetInner<MediaTypeFormatter>(f) is ODataMediaTypeFormatter)).Any<MediaTypeFormatter>())
      {
        ODataFormattingAttribute.RemoveFormatters(formatters, formatters.Where<MediaTypeFormatter>((Func<MediaTypeFormatter, bool>) (f => f is XmlMediaTypeFormatter || f is JsonMediaTypeFormatter)));
        formatters.InsertRange(0, (IEnumerable<MediaTypeFormatter>) this.CreateODataFormatters());
      }
      ServicesContainer services = controllerSettings.Services;
      IActionValueBinder service1 = (IActionValueBinder) new PerRequestActionValueBinder(services.GetActionValueBinder());
      controllerSettings.Services.Replace(typeof (IActionValueBinder), (object) service1);
      IContentNegotiator service2 = (IContentNegotiator) new PerRequestContentNegotiator(services.GetContentNegotiator());
      controllerSettings.Services.Replace(typeof (IContentNegotiator), (object) service2);
    }

    public virtual IList<ODataMediaTypeFormatter> CreateODataFormatters() => ODataMediaTypeFormatters.Create();

    private static void RemoveFormatters(
      MediaTypeFormatterCollection formatterCollection,
      IEnumerable<MediaTypeFormatter> formattersToRemove)
    {
      foreach (MediaTypeFormatter mediaTypeFormatter in formattersToRemove.ToArray<MediaTypeFormatter>())
        formatterCollection.Remove(mediaTypeFormatter);
    }
  }
}
