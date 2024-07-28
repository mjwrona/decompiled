// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Account.Models.DetailsSecurityActionUrlModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Account, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC21A176-69BE-407E-B3DD-80612369F784
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Account.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Account.Models
{
  [DataContract]
  public class DetailsSecurityActionUrlModel
  {
    [DataMember(Name = "PersonalAccessToken", EmitDefaultValue = false)]
    public PersonalAccessTokenUrlModel PersonalAccessToken { get; set; }

    [DataMember(Name = "PublicKey", EmitDefaultValue = false)]
    public PublicKeyUrlModel PublicKey { get; set; }
  }
}
