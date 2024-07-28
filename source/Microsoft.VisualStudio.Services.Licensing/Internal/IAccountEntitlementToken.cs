// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.Internal.IAccountEntitlementToken
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Licensing.Internal
{
  internal interface IAccountEntitlementToken
  {
    IDictionary<Guid, LicenseToken> AccountLicenseTokens { get; }

    DateTimeOffset LastUpdated { get; }
  }
}
