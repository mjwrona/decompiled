// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityExtendedPropertyKeys
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class IdentityExtendedPropertyKeys
  {
    public static readonly HashSet<string> IdentityExtensionSupportedProperties = new HashSet<string>()
    {
      "http://schemas.microsoft.com/identity/claims/objectidentifier",
      "AuthenticationCredentialValidFrom",
      "Microsoft.TeamFoundation.Identity.Image.Id",
      "Microsoft.TeamFoundation.Identity.Image.Type",
      "ConfirmedNotificationAddress",
      "CustomNotificationAddresses"
    };
    public static readonly HashSet<string> IdentityUnsupportedProperties = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      "vss:AadRefreshToken",
      "Microsoft.VisualStudio.Aad.AadRefreshTokenUpdateDate"
    };
  }
}
