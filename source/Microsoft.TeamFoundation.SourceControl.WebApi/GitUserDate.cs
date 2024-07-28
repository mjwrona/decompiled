// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.GitUserDate
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  public class GitUserDate : VersionControlSecuredObject
  {
    [DataMember(Name = "name", EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(Name = "email", EmitDefaultValue = false)]
    [JsonConverter(typeof (DefaultValueOnPublicAccessJsonConverter<string>))]
    public string Email { get; set; }

    [DataMember(Name = "date", EmitDefaultValue = false)]
    public DateTime Date { get; set; }

    [DataMember(Name = "imageUrl", EmitDefaultValue = false)]
    public string ImageUrl { get; set; }
  }
}
