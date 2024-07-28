// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Web.ICommerceDataProvider
// Assembly: Microsoft.VisualStudio.Services.Gallery.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17D36576-2EF3-4ABC-94BA-AF7891D15A3A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Commerce;
using Microsoft.VisualStudio.Services.Gallery.Server.Remote;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Web
{
  public interface ICommerceDataProvider
  {
    IOfferMeter GetOfferDetails(
      IVssRequestContext requestContext,
      string fullyQualifiedExtensionName);

    IOfferSubscription GetOfferSubscriptionDetails(
      IVssRequestContext requestContext,
      string fullyQualifiedExtensionName,
      Guid accountId,
      IRemoteServiceClientFactory clientFactory,
      bool nextBillingPeriod = false);

    IEnumerable<IOfferMeterPrice> GetOfferMeterPrice(
      IVssRequestContext requestContext,
      string fullyQualifiedExtensionName);

    ISubscriptionAccount GetSubscriptionAccount(
      IVssRequestContext requestContext,
      Guid accountId,
      AccountProviderNamespace providerNamespace = AccountProviderNamespace.VisualStudioOnline);
  }
}
