// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CheckAccountOwnershipInput
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.VisualStudio.Services.Commerce")]
  public class CheckAccountOwnershipInput
  {
    [DataMember(IsRequired = true, Order = 0)]
    public string AccountName;
    [DataMember(IsRequired = true, Order = 1)]
    public string EmailAddress;
    [DataMember(IsRequired = true, Order = 3)]
    public string Puid;
    [DataMember(IsRequired = true, Order = 2)]
    public string IdType;
  }
}
