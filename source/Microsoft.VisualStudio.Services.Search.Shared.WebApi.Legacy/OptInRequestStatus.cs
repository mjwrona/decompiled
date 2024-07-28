// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.OptInRequestStatus
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8C7C9E57-44B4-4654-9458-CC8B59C2B681
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy
{
  [DataContract]
  public class OptInRequestStatus
  {
    [DataMember(Name = "hostId")]
    public Guid HostId { get; set; }

    [DataMember(Name = "accountName")]
    public string AccountName { get; set; }

    [DataMember(Name = "accountUrl")]
    public string AccountUrl { get; set; }

    [DataMember(Name = "requestorId")]
    public Guid RequestorId { get; set; }

    [DataMember(Name = "requestStatus")]
    public short RequestStatus { get; set; }
  }
}
