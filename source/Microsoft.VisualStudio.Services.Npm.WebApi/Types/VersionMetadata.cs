// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.WebApi.Types.VersionMetadata
// Assembly: Microsoft.VisualStudio.Services.Npm.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 639B57A1-1338-429F-9659-38C0A0394E05
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.WebApi.dll

using Microsoft.VisualStudio.Services.Npm.WebApi.Converters;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Npm.WebApi.Types
{
  [DataContract]
  public class VersionMetadata : PackageJson
  {
    public VersionMetadata()
    {
    }

    public VersionMetadata(PackageJson packageJson)
      : base(packageJson)
    {
    }

    [DataMember(Name = "_id", EmitDefaultValue = false)]
    public string Id { get; set; }

    [DataMember(Name = "readme", EmitDefaultValue = false)]
    [JsonConverter(typeof (ReadMeJsonConverter))]
    public string Readme { get; set; }

    [IgnoreDataMember]
    public bool IsPublished { get; set; }
  }
}
