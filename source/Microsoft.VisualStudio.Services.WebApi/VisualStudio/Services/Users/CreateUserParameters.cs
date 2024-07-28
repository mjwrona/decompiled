// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Users.CreateUserParameters
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Users
{
  [DataContract]
  public class CreateUserParameters
  {
    public CreateUserParameters()
    {
    }

    public CreateUserParameters(CreateUserParameters copy)
    {
      this.Descriptor = copy.Descriptor;
      this.DisplayName = copy.DisplayName;
      this.Mail = copy.Mail;
      this.Country = copy.Country;
      this.Region = copy.Region;
      this.PendingProfileCreation = copy.PendingProfileCreation;
      if (copy.Data == null)
        return;
      this.Data = new Dictionary<string, object>((IDictionary<string, object>) copy.Data);
    }

    [DataMember(IsRequired = true)]
    public SubjectDescriptor Descriptor { get; set; }

    [DataMember(IsRequired = true)]
    public string DisplayName { get; set; }

    [DataMember(IsRequired = true)]
    public string Mail { get; set; }

    [DataMember(IsRequired = true)]
    public string Country { get; set; }

    [DataMember(IsRequired = true)]
    public string Region { get; set; }

    [DataMember(IsRequired = false)]
    public bool PendingProfileCreation { get; set; }

    [DataMember(IsRequired = false)]
    public Dictionary<string, object> Data { get; set; }

    internal CreateUserParameters Clone() => new CreateUserParameters(this);

    internal virtual User ToUser() => new User()
    {
      Descriptor = this.Descriptor,
      DisplayName = this.DisplayName,
      Mail = this.Mail,
      Country = this.Country
    };
  }
}
