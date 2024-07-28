// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionRequest
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4FCC2C3-B106-43A6-A409-E4BF8CFC545C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.WebApi
{
  [DataContract]
  public sealed class ExtensionRequest
  {
    [DataMember]
    public IdentityRef RequestedBy { get; set; }

    [DataMember]
    public IdentityRef ResolvedBy { get; set; }

    [DataMember]
    public string RequestMessage { get; set; }

    [DataMember]
    public string RejectMessage { get; set; }

    [DataMember]
    public DateTime RequestDate { get; set; }

    [DataMember]
    public DateTime ResolveDate { get; set; }

    [DataMember]
    public ExtensionRequestState RequestState { get; set; }
  }
}
