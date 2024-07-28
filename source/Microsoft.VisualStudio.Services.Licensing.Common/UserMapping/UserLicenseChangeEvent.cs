// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.UserMapping.UserLicenseChangeEvent
// Assembly: Microsoft.VisualStudio.Services.Licensing.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F3070F25-7414-49A0-9C00-005379F04A49
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.Common.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Licensing;
using System;

namespace Microsoft.VisualStudio.Services.UserMapping
{
  public class UserLicenseChangeEvent
  {
    public UserLicenseChangeEvent(Guid userId, Guid collectionId, License newLicense)
    {
      ArgumentUtility.CheckForEmptyGuid(userId, nameof (userId));
      ArgumentUtility.CheckForEmptyGuid(collectionId, nameof (collectionId));
      ArgumentUtility.CheckForNull<License>(newLicense, nameof (newLicense));
      this.UserId = userId;
      this.CollectionId = collectionId;
      this.NewLicense = newLicense;
    }

    public Guid CollectionId { get; }

    public Guid UserId { get; }

    public License NewLicense { get; }
  }
}
