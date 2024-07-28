// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointRequestResult
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52155B17-64DE-4C30-B15E-F2E70DBED717
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.dll

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi
{
  [DataContract]
  public class ServiceEndpointRequestResult
  {
    [DataMember(EmitDefaultValue = false)]
    public JToken Result { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public HttpStatusCode StatusCode { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ErrorMessage { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid ActivityId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool CallbackRequired { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IDictionary<string, string> CallbackContextParameters { get; set; }
  }
}
