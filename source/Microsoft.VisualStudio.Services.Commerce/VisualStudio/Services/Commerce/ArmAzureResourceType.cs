// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.ArmAzureResourceType
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [DataContract]
  internal class ArmAzureResourceType
  {
    [DataMember(Name = "apiVersions")]
    public List<string> ApiVersions { get; set; }

    [DataMember(Name = "locations")]
    public List<string> Locations { get; set; }

    [DataMember(Name = "resourceType")]
    public string ResourceType { get; set; }
  }
}
