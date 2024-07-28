// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.WebApi.WidgetTypesResponse
// Assembly: Microsoft.TeamFoundation.Dashboards.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 05786B5F-D2ED-4F72-80F1-EA5660131542
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Dashboards.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Dashboards.WebApi
{
  [DataContract]
  public class WidgetTypesResponse
  {
    [DataMember]
    public IEnumerable<WidgetMetadata> WidgetTypes { get; set; }

    [DataMember]
    public string Uri { get; set; }

    [DataMember(Name = "_links", EmitDefaultValue = false)]
    public ReferenceLinks Links { get; set; }

    public WidgetTypesResponse()
    {
    }

    public WidgetTypesResponse(IEnumerable<WidgetMetadata> w, string url)
    {
      this.WidgetTypes = w;
      this.Uri = url;
    }
  }
}
