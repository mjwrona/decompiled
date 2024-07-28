// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.VssJsonMediaTypeFormatter
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.WebApi
{
  public class VssJsonMediaTypeFormatter : JsonMediaTypeFormatter
  {
    private bool m_bypassSafeArrayWrapping;

    public VssJsonMediaTypeFormatter(bool bypassSafeArrayWrapping = false)
      : this(bypassSafeArrayWrapping, false, false)
    {
    }

    public VssJsonMediaTypeFormatter(
      bool bypassSafeArrayWrapping,
      bool enumsAsNumbers = false,
      bool useMsDateFormat = false)
    {
      this.SetSerializerSettings(bypassSafeArrayWrapping, enumsAsNumbers, useMsDateFormat);
    }

    public VssJsonMediaTypeFormatter(HttpRequestMessage request, bool bypassSafeArrayWrapping = false)
    {
      this.Request = request;
      this.SerializerSettings.Context = new StreamingContext((StreamingContextStates) 0, (object) this.Request);
      bool enumsAsNumbers = string.Equals("true", this.GetAcceptHeaderOptionValue(request, "enumsAsNumbers"), StringComparison.OrdinalIgnoreCase);
      bool useMsDateFormat = string.Equals("true", this.GetAcceptHeaderOptionValue(request, "msDateFormat"), StringComparison.OrdinalIgnoreCase);
      if (!bypassSafeArrayWrapping)
        bypassSafeArrayWrapping = string.Equals("true", this.GetAcceptHeaderOptionValue(request, "noArrayWrap"), StringComparison.OrdinalIgnoreCase);
      this.SetSerializerSettings(bypassSafeArrayWrapping, enumsAsNumbers, useMsDateFormat);
    }

    private void SetSerializerSettings(
      bool bypassSafeArrayWrapping,
      bool enumsAsNumbers,
      bool useMsDateFormat)
    {
      this.SerializerSettings.ContractResolver = this.GetContractResolver(enumsAsNumbers);
      if (!enumsAsNumbers)
        this.SerializerSettings.Converters.Add((JsonConverter) new StringEnumConverter()
        {
          CamelCaseText = true
        });
      if (useMsDateFormat)
        this.SerializerSettings.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat;
      this.m_bypassSafeArrayWrapping = bypassSafeArrayWrapping;
      this.EnumsAsNumbers = enumsAsNumbers;
      this.UseMsDateFormat = useMsDateFormat;
    }

    protected virtual IContractResolver GetContractResolver(bool enumsAsNumbers) => enumsAsNumbers ? (IContractResolver) new VssCamelCasePropertyNamesPreserveEnumsContractResolver() : (IContractResolver) new VssCamelCasePropertyNamesContractResolver();

    protected HttpRequestMessage Request { get; private set; }

    public bool BypassSafeArrayWrapping
    {
      get => this.m_bypassSafeArrayWrapping;
      set => this.m_bypassSafeArrayWrapping = value;
    }

    public bool EnumsAsNumbers { get; private set; }

    public bool UseMsDateFormat { get; private set; }

    public override MediaTypeFormatter GetPerRequestFormatterInstance(
      Type type,
      HttpRequestMessage request,
      MediaTypeHeaderValue mediaType)
    {
      return this.GetType().Equals(typeof (VssJsonMediaTypeFormatter)) ? (MediaTypeFormatter) new VssJsonMediaTypeFormatter(request, this.m_bypassSafeArrayWrapping) : base.GetPerRequestFormatterInstance(type, request, mediaType);
    }

    private string GetAcceptHeaderOptionValue(HttpRequestMessage request, string acceptOptionName)
    {
      foreach (MediaTypeHeaderValue mediaTypeHeaderValue in request.Headers.Accept)
      {
        foreach (NameValueHeaderValue parameter in (IEnumerable<NameValueHeaderValue>) mediaTypeHeaderValue.Parameters)
        {
          if (string.Equals(parameter.Name, acceptOptionName, StringComparison.OrdinalIgnoreCase))
            return parameter.Value;
        }
      }
      return (string) null;
    }

    public override bool CanReadType(Type type) => !type.IsOfType(typeof (IPatchDocument<>));

    public override Task WriteToStreamAsync(
      Type type,
      object value,
      Stream writeStream,
      HttpContent content,
      TransportContext transportContext)
    {
      Type type1 = type;
      if (!this.m_bypassSafeArrayWrapping && typeof (IEnumerable).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()) && !type.Equals(typeof (byte[])) && !type.Equals(typeof (JObject)))
      {
        type1 = typeof (VssJsonCollectionWrapper);
        object source1;
        switch (value)
        {
          case ICollection _:
          case string _:
            source1 = value;
            break;
          default:
            IEnumerable source2 = (IEnumerable) value;
            List<object> list = source2 != null ? source2.Cast<object>().ToList<object>() : (List<object>) null;
            source1 = list != null ? (object) list : value;
            break;
        }
        value = (object) new VssJsonCollectionWrapper((IEnumerable) source1);
      }
      return base.WriteToStreamAsync(type1, value, writeStream, content, transportContext);
    }
  }
}
