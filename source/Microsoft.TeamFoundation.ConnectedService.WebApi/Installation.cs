// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ConnectedService.WebApi.Installation
// Assembly: Microsoft.TeamFoundation.ConnectedService.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0D040FD2-0366-4FA8-B2F4-4380C0B19F54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.ConnectedService.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.ConnectedService.WebApi
{
  [DataContract]
  public class Installation
  {
    [DataMember]
    public string ImageUrl { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string InstallationId { get; set; }

    [DataMember]
    public string Signature { get; set; }
  }
}
