// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.WebApi.WidgetMetadataResponse
// Assembly: Microsoft.TeamFoundation.Dashboards.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 05786B5F-D2ED-4F72-80F1-EA5660131542
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Dashboards.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Dashboards.WebApi
{
  [DataContract]
  public class WidgetMetadataResponse
  {
    [DataMember]
    public WidgetMetadata WidgetMetadata { get; set; }

    [DataMember]
    public string Uri { get; set; }

    public WidgetMetadataResponse()
    {
    }

    public WidgetMetadataResponse(WidgetMetadata metadata, string url)
    {
      this.WidgetMetadata = metadata;
      this.Uri = url;
    }
  }
}
