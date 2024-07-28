// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.PermissionsReport.Client.PermissionsReportResource
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.PermissionsReport.Client
{
  [DataContract]
  public sealed class PermissionsReportResource
  {
    [DataMember(Name = "ResourceType", IsRequired = true)]
    [JsonConverter(typeof (StringEnumConverter))]
    public ResourceType ResourceType { get; set; }

    [DataMember(Name = "ResourceId", IsRequired = true)]
    public string ResourceId { get; set; }

    [DataMember(Name = "ResourceName", IsRequired = false)]
    public string ResourceName { get; set; }
  }
}
