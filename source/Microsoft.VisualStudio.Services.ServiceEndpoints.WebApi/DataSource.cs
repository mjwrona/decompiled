// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.DataSource
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52155B17-64DE-4C30-B15E-F2E70DBED717
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi
{
  [DataContract]
  public class DataSource
  {
    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string EndpointUrl { get; set; }

    [DataMember]
    public string RequestVerb { get; set; }

    [DataMember]
    public string RequestContent { get; set; }

    [DataMember]
    public string ResourceUrl { get; set; }

    [DataMember]
    public string ResultSelector { get; set; }

    [DataMember]
    public string CallbackContextTemplate { get; set; }

    [DataMember]
    public string CallbackRequiredTemplate { get; set; }

    [DataMember]
    public string InitialContextTemplate { get; set; }

    [DataMember]
    public List<AuthorizationHeader> Headers { get; set; }

    [DataMember]
    public AuthenticationSchemeReference AuthenticationScheme { get; set; }
  }
}
