// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.WebApi.TemplateAsset
// Assembly: Microsoft.TeamFoundation.Pipelines.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 29F2A1B3-A3F7-4291-91FA-6C4508EECB65
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Pipelines.WebApi
{
  [DataContract]
  public class TemplateAsset
  {
    [DataMember]
    public string Type { get; set; }

    [DataMember]
    public string Path { get; set; }

    [DataMember]
    public string Content { get; set; }

    [DataMember]
    public string DestinationPath { get; set; }

    [DataMember]
    public string Description { get; set; }
  }
}
