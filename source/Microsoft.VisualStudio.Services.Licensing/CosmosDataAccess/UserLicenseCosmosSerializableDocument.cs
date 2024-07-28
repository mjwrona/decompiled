// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.CosmosDataAccess.UserLicenseCosmosSerializableDocument
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.Azure.Documents;
using Microsoft.VisualStudio.Services.DocDB;
using Microsoft.VisualStudio.Services.Licensing.CosmosDataAccess.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Licensing.CosmosDataAccess
{
  internal class UserLicenseCosmosSerializableDocument : 
    DocDBSerializableDocument<UserLicenseCosmosItem>
  {
    public virtual string DocumentType => "License";

    protected virtual int ClassVersion => 1;

    public virtual string DocumentId => this.Document.UserId.ToString();

    public UserLicenseCosmosSerializableDocument()
    {
    }

    public UserLicenseCosmosSerializableDocument(Guid userId)
      : base(new UserLicenseCosmosItem(userId), (string) null)
    {
    }

    public UserLicenseCosmosSerializableDocument(
      Guid userId,
      UserLicense license,
      LicensedIdentity licensedIdentity,
      UserLicense previousLicense = null,
      List<UserExtensionLicense> extensions = null)
      : base(new UserLicenseCosmosItem(userId, license, licensedIdentity, previousLicense, extensions), (string) null)
    {
    }

    public UserLicenseCosmosSerializableDocument(UserLicenseCosmosItem item)
      : base(item, item.UserId.ToString())
    {
    }

    public UserLicenseCosmosSerializableDocument Transfer(Guid userId)
    {
      this.Document.PreviousUserId = new Guid?(this.Document.UserId);
      ((Resource) this).Id = userId.ToString();
      this.Document.UserId = userId;
      this.Document.License = this.Document.License?.Transfer(userId);
      this.Document.PreviousLicense = this.Document.PreviousLicense?.Transfer(userId);
      this.Document.ExtensionLicenses = this.Document.ExtensionLicenses.Select<UserExtensionLicense, UserExtensionLicense>((Func<UserExtensionLicense, UserExtensionLicense>) (x => x?.Transfer(userId))).ToList<UserExtensionLicense>();
      return this;
    }
  }
}
