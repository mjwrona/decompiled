// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServerVssJsonMediaTypeFormatter
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class ServerVssJsonMediaTypeFormatter : VssJsonMediaTypeFormatter
  {
    public ServerVssJsonMediaTypeFormatter(bool bypassSafeArrayWrapping = false)
      : base(bypassSafeArrayWrapping)
    {
    }

    public ServerVssJsonMediaTypeFormatter(
      bool bypassSafeArrayWrapping,
      bool enumsAsNumbers = false,
      bool useMsDateFormat = false)
      : base(bypassSafeArrayWrapping, enumsAsNumbers, useMsDateFormat)
    {
    }

    public ServerVssJsonMediaTypeFormatter(HttpRequestMessage request, bool bypassSafeArrayWrapping = false)
      : base(request, bypassSafeArrayWrapping)
    {
    }

    protected override IContractResolver GetContractResolver(bool enumsAsNumbers) => enumsAsNumbers ? (IContractResolver) new ServerVssCamelCasePropertyNamesPreserveEnumsContractResolver() : (IContractResolver) new ServerVssCamelCasePropertyNamesContractResolver();

    public override MediaTypeFormatter GetPerRequestFormatterInstance(
      Type type,
      HttpRequestMessage request,
      MediaTypeHeaderValue mediaType)
    {
      return this.GetType().Equals(typeof (ServerVssJsonMediaTypeFormatter)) ? (MediaTypeFormatter) new ServerVssJsonMediaTypeFormatter(request, this.BypassSafeArrayWrapping) : base.GetPerRequestFormatterInstance(type, request, mediaType);
    }

    public override Task WriteToStreamAsync(
      Type type,
      object value,
      Stream writeStream,
      HttpContent content,
      TransportContext transportContext)
    {
      ServerJsonSerializationHelper.EnsureValidRootType(type);
      return base.WriteToStreamAsync(type, value, writeStream, content, transportContext);
    }
  }
}
