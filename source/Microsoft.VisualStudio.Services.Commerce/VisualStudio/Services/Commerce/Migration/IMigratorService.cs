// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Migration.IMigratorService
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Commerce.Migration
{
  [DefaultServiceImplementation(typeof (AzCommMigratorService))]
  internal interface IMigratorService : IVssFrameworkService
  {
    void MigrateData(IVssRequestContext requestContext, ServicingContext servicingContext);

    void DualWriteResourceUsage(
      IVssRequestContext requestContext,
      IEnumerable<SubscriptionResourceUsage> resourceUsages,
      Guid hostId);

    void DualWriteResourceAccounts(
      IVssRequestContext requestContext,
      AzureResourceAccount resourceAccount,
      Guid hostId,
      bool isLinkOperation = true);

    void DualWriteSubscription(
      IVssRequestContext requestContext,
      AzureSubscriptionInternal subscriptionInternal,
      Guid subscriptionId);

    void SetStaleOrganization(
      IVssRequestContext requestContext,
      Guid organizationId,
      IEnumerable<int> meterIds);

    void RemoveStaleOrganization(
      IVssRequestContext requestContext,
      Guid organizationId,
      IEnumerable<int> meterIds);

    void DualWriteAccountTags(
      IVssRequestContext requestContext,
      Guid organizationId,
      Dictionary<string, string> tags);

    void DualWriteDefaultAccessLevel(
      IVssRequestContext requestContext,
      Guid organizationId,
      int accessLevel);
  }
}
