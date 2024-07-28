// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DataImport.ILicensingHelper
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Licensing;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.DataImport
{
  public interface ILicensingHelper
  {
    IEnumerable<AccountEntitlement> GetAccountEntitlements(IVssRequestContext requestContext);

    IEnumerable<AccountEntitlement> GetAccountEntitlements(
      IVssRequestContext requestContext,
      Guid hostId);

    int GetNonStakeholderLicenseCount(IVssRequestContext requestContext, ITFLogger logger = null);

    int GetNonStakeholderLicenseCount(
      IEnumerable<AccountEntitlement> accountEntitlements,
      ITFLogger logger = null);
  }
}
