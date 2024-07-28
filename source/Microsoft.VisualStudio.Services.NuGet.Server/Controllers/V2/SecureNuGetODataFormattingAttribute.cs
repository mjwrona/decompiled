// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Controllers.V2.SecureNuGetODataFormattingAttribute
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http.Controllers;
using System.Web.Http.OData.Formatter;
using System.Web.Http.OData.Formatter.Deserialization;
using System.Web.Http.OData.Formatter.Serialization;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Controllers.V2
{
  public class SecureNuGetODataFormattingAttribute : Attribute, IControllerConfiguration
  {
    public void Initialize(
      HttpControllerSettings controllerSettings,
      HttpControllerDescriptor controllerDescriptor)
    {
      ArgumentUtility.CheckForNull<HttpControllerSettings>(controllerSettings, nameof (controllerSettings));
      ArgumentUtility.CheckForNull<HttpControllerDescriptor>(controllerDescriptor, nameof (controllerDescriptor));
      IEnumerable<ODataMediaTypeFormatter> items = ODataMediaTypeFormatters.Create((ODataSerializerProvider) new SecureNuGetODataSerializerProvider(), (ODataDeserializerProvider) new DefaultODataDeserializerProvider()).Where<ODataMediaTypeFormatter>((Func<ODataMediaTypeFormatter, bool>) (f => !f.SupportedMediaTypes.Any<MediaTypeHeaderValue>((Func<MediaTypeHeaderValue, bool>) (m => m.MediaType.Equals("application/atomsvc+xml", StringComparison.OrdinalIgnoreCase) || m.MediaType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase)))));
      foreach (ODataMediaTypeFormatter mediaTypeFormatter in items)
        mediaTypeFormatter.MessageWriterSettings.Indent = false;
      controllerSettings.Formatters.Clear();
      controllerSettings.Formatters.AddRange((IEnumerable<MediaTypeFormatter>) items);
    }
  }
}
