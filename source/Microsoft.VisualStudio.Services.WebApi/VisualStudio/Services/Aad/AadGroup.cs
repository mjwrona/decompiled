// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.AadGroup
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Aad
{
  [DataContract]
  public class AadGroup : AadObject
  {
    [DataMember]
    private string description;
    [DataMember]
    private string mailNickname;
    [DataMember]
    private string mail;
    [DataMember]
    private string onPremisesSecurityIdentifier;

    protected AadGroup()
    {
    }

    private AadGroup(
      Guid objectId,
      string displayName,
      string description,
      string mailNickname,
      string mail,
      string onPremisesSecurityIdentifier)
      : base(objectId, displayName)
    {
      this.description = description;
      this.mailNickname = mailNickname;
      this.mail = mail;
      this.onPremisesSecurityIdentifier = onPremisesSecurityIdentifier;
    }

    public string Description
    {
      get => this.description;
      set => this.description = value;
    }

    public string MailNickname
    {
      get => this.mailNickname;
      set => this.mailNickname = value;
    }

    public string Mail
    {
      get => this.mail;
      set => this.mail = value;
    }

    public string OnPremisesSecurityIdentifier => this.onPremisesSecurityIdentifier;

    public class Factory
    {
      public AadGroup Create() => new AadGroup(this.ObjectId, this.DisplayName, this.Description, this.MailNickname, this.Mail, this.OnPremisesSecurityIdentifier);

      public Guid ObjectId { get; set; }

      public string DisplayName { get; set; }

      public string Description { get; set; }

      public string MailNickname { get; set; }

      public string Mail { get; set; }

      public string OnPremisesSecurityIdentifier { get; set; }
    }
  }
}
