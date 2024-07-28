// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.UserIsNotAccountOwnerException
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [ExceptionMapping("0.0", "3.0", "UserIsNotAccountOwnerException", "Microsoft.VisualStudio.Services.Commerce.UserIsNotAccountOwnerException, Microsoft.VisualStudio.Services.WebApi, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public class UserIsNotAccountOwnerException : CommerceException
  {
    public UserIsNotAccountOwnerException(Microsoft.VisualStudio.Services.Identity.Identity identity, string collectionName)
      : base(CommerceResources.UserNotAccountAdministrator((object) identity.DisplayName, (object) collectionName))
    {
      this.Identity = identity;
      this.CollectionName = collectionName;
    }

    public UserIsNotAccountOwnerException(string userEmail, string accountName)
      : base(CommerceResources.UserNotAccountAdministrator((object) userEmail, (object) accountName))
    {
      this.Email = userEmail;
      this.AccountName = accountName;
    }

    public Microsoft.VisualStudio.Services.Identity.Identity Identity { get; set; }

    public string CollectionName { get; set; }

    public Guid IdentityId { get; set; }

    public string Email { get; set; }

    public string AccountName { get; set; }
  }
}
