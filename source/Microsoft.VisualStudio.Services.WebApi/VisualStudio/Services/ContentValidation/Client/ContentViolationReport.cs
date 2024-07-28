// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ContentValidation.Client.ContentViolationReport
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ContentValidation.Client
{
  [DataContract]
  public class ContentViolationReport
  {
    [DataMember]
    public Guid HostId { get; set; }

    [DataMember]
    public Guid ContainerId { get; set; }

    [DataMember]
    public string ContentUrl { get; set; }

    [DataMember]
    public ContentViolationCategory ViolationCategory { get; set; }

    [DataMember]
    public string AdditionalDetails { get; set; }
  }
}
