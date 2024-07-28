// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.EndpointUrl
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52155B17-64DE-4C30-B15E-F2E70DBED717
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.dll

using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi
{
  [DataContract]
  [JsonConverter(typeof (EndpointUrlJsonConverter))]
  public class EndpointUrl
  {
    [DataMember(EmitDefaultValue = false)]
    public string DisplayName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string HelpText { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Uri Value { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string IsVisible { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DependsOn DependsOn { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Format { get; set; }
  }
}
