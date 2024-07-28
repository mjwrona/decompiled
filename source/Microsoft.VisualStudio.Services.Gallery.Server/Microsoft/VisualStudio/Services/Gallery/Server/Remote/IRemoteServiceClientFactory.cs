// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Remote.IRemoteServiceClientFactory
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Commerce.WebApi;
using Microsoft.VisualStudio.Services.Gallery.Server.Remote.Billing;
using Microsoft.VisualStudio.Services.Gallery.Server.Remote.ExtensionManagement;
using Microsoft.VisualStudio.Services.Organization.Client;
using System;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Remote
{
  public interface IRemoteServiceClientFactory
  {
    IExtensionManagementClient GetNewExtensionManagementClient(
      IVssRequestContext requestContext,
      Guid hostId,
      Guid serviceOwner);

    IExtensionManagementClient GetEMSClient(
      IVssRequestContext requestContext,
      Guid hostId,
      Guid serviceOwner);

    IBillingClient GetNewNonDeploymentLevelBillingClient(
      IVssRequestContext requestContext,
      Guid hostId);

    OfferSubscriptionHttpClient GetNewNonDeploymentLevelCommerceClient(
      IVssRequestContext requestContext,
      Guid hostId);

    IBillingClient GetNewBillingClient(IVssRequestContext requestContext);

    OrganizationHttpClient GetOrganizationClient(
      IVssRequestContext requestContext,
      Guid organizationId);
  }
}
