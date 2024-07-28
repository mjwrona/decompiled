// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Users.Server.UserValidationOptions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Users.Server
{
  [DataContract]
  public class UserValidationOptions
  {
    public static readonly UserValidationOptions Default = new UserValidationOptions();

    [DataMember]
    public bool ValidateDescriptor { get; private set; } = true;

    [DataMember]
    public bool ValidateDisplayName { get; private set; } = true;

    [DataMember]
    public bool ValidateMail { get; private set; } = true;

    [DataMember]
    public bool ValidateCountry { get; private set; } = true;

    [DataMember]
    public bool ValidateRegion { get; private set; } = true;

    private UserValidationOptions()
    {
    }

    public UserValidationOptions(
      bool validateDescriptor = true,
      bool validateDisplayName = true,
      bool validateMail = true,
      bool validateCountry = true,
      bool validateRegion = true)
    {
      this.ValidateDescriptor = validateDescriptor;
      this.ValidateDisplayName = validateDisplayName;
      this.ValidateMail = validateMail;
      this.ValidateCountry = validateCountry;
      this.ValidateRegion = validateRegion;
    }

    public UserValidationOptions(UserValidationOptions clone)
    {
      this.ValidateDescriptor = clone.ValidateDescriptor;
      this.ValidateDisplayName = clone.ValidateDisplayName;
      this.ValidateMail = clone.ValidateMail;
      this.ValidateCountry = clone.ValidateCountry;
      this.ValidateRegion = clone.ValidateRegion;
    }
  }
}
