// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.Contracts.Licensing.ExtensionLicenseData
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Licensing;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.WebApi.Contracts.Licensing
{
  [DataContract]
  public class ExtensionLicenseData
  {
    [DataMember]
    public string ExtensionId { get; set; }

    [DataMember]
    public VisualStudioOnlineServiceLevel MinimumRequiredAccessLevel { get; set; }

    [DataMember]
    public bool IsFree { get; set; }

    [DataMember]
    public DateTime CreatedDate { get; set; }

    [DataMember]
    public DateTime UpdatedDate { get; set; }
  }
}
