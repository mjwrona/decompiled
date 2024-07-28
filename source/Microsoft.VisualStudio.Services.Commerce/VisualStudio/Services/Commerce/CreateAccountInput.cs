// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CreateAccountInput
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.VisualStudio.Services.Commerce")]
  public class CreateAccountInput
  {
    [DataMember(IsRequired = true, Order = 0)]
    public string AccountName { get; set; }

    [DataMember(IsRequired = false, Order = 1)]
    public string AccountRegion { get; set; }

    [DataMember(IsRequired = false, Order = 2)]
    public string IdentityDomain { get; set; }

    [DataMember(IsRequired = true, Order = 3)]
    public string IdentityEmail { get; set; }

    [DataMember(IsRequired = false, Order = 4)]
    public string IdentityPuid { get; set; }
  }
}
