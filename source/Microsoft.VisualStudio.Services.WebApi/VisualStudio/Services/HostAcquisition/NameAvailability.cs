// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.HostAcquisition.NameAvailability
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.HostAcquisition
{
  [DataContract]
  public sealed class NameAvailability
  {
    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public bool IsAvailable { get; set; }

    [DataMember(IsRequired = false)]
    public string UnavailabilityReason { get; set; }
  }
}
