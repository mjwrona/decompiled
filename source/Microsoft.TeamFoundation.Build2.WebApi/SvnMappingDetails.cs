// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.SvnMappingDetails
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [DataContract]
  public class SvnMappingDetails
  {
    [DataMember(Name = "serverPath")]
    public string ServerPath { get; set; }

    [DataMember(Name = "localPath")]
    public string LocalPath { get; set; }

    [DataMember(Name = "revision")]
    public string Revision { get; set; }

    [DataMember(Name = "depth")]
    public int Depth { get; set; }

    [DataMember(Name = "ignoreExternals")]
    public bool IgnoreExternals { get; set; }
  }
}
