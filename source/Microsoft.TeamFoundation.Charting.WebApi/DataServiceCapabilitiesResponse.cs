// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Charting.WebApi.DataServiceCapabilitiesResponse
// Assembly: Microsoft.TeamFoundation.Charting.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D43D663A-A882-4856-B0B7-D0E666C5CC4A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Charting.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Charting.WebApi
{
  [DataContract]
  public class DataServiceCapabilitiesResponse : ChartingRestResponseBase
  {
    public DataServiceCapabilitiesResponse(DataServiceCapabilities capabilities, string uri)
      : base(uri)
    {
      this.DataServiceCapabilities = capabilities;
    }

    public DataServiceCapabilitiesResponse()
    {
    }

    [DataMember]
    public DataServiceCapabilities DataServiceCapabilities { get; private set; }
  }
}
