// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.AadApplication
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Aad
{
  [DataContract]
  public class AadApplication : AadObject
  {
    [DataMember]
    public Guid AppId { get; }

    [DataMember]
    public IList<AadPasswordCredential> PasswordCredentials { get; }

    [DataMember]
    public IList<string> ReplyUrls { get; }

    [DataMember]
    public string HomePage { get; }

    protected AadApplication()
    {
    }

    private AadApplication(
      Guid objectId,
      string displayName,
      Guid appId,
      IList<AadPasswordCredential> passwordCredentials,
      IList<string> replyUrls,
      string homePage)
      : base(objectId, displayName)
    {
      this.AppId = appId;
      this.PasswordCredentials = passwordCredentials;
      this.ReplyUrls = replyUrls;
      this.HomePage = homePage;
    }

    public class Factory
    {
      public AadApplication Create() => new AadApplication(this.ObjectId, this.DisplayName, this.AppId, this.PasswordCredentials, this.ReplyUrls, this.HomePage);

      public Guid ObjectId { get; set; }

      public string DisplayName { get; set; }

      public Guid AppId { get; set; }

      public IList<AadPasswordCredential> PasswordCredentials { get; set; }

      public IList<string> ReplyUrls { get; set; }

      public string HomePage { get; set; }
    }
  }
}
