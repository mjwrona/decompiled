// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.PackageMetadata
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class PackageMetadata
  {
    [DataMember]
    public string Type { get; set; }

    [DataMember]
    public string Platform { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime CreatedOn { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public PackageVersion Version { get; set; }

    [DataMember]
    public string DownloadUrl { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string HashValue { get; set; }

    [DataMember]
    public string InfoUrl { get; set; }

    [DataMember]
    public string Filename { get; set; }
  }
}
