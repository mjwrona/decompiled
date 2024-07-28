// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.ArmAzureResourceProvider
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [DataContract]
  internal class ArmAzureResourceProvider
  {
    [DataMember(Name = "id")]
    public string Id { get; set; }

    [DataMember(Name = "namespace")]
    public string ProviderName { get; set; }

    [DataMember(Name = "registrationState")]
    public string RegistrationState { get; set; }

    [DataMember(Name = "resourceTypes")]
    public List<ArmAzureResourceType> ResourceTypes { get; set; }
  }
}
