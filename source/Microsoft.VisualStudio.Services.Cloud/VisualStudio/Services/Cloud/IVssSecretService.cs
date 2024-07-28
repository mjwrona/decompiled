// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.IVssSecretService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Configuration;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Cloud
{
  [DefaultServiceImplementation(typeof (HostedSecretService))]
  public interface IVssSecretService : IVssFrameworkService
  {
    void RegisterSecrets(
      IVssRequestContext requestContext,
      List<ConfigurationSecretData> secrets,
      bool deleteMissing,
      ITFLogger logger = null);

    void UpdateServiceBusSubscriptionFilter(IVssRequestContext requestContext, ITFLogger logger);

    void UpdateServiceBusSubscription(
      IVssRequestContext requestContext,
      MessageBusSubscriberSettings subscriberSettings,
      ITFLogger logger);

    int UpdateRegisteredSecrets(
      IVssRequestContext requestContext,
      bool alertOnStaleValues,
      bool verboseTrace);

    bool UpdateSecret(
      IVssRequestContext requestContext,
      Uri keyVaultSecretIdentifier,
      bool verboseTrace);

    void UpdateBootstrapSettings(IVssRequestContext requestContext, bool verboseTrace);

    RegistryEntryCollection GetRegisteredSecretEntries(IVssRequestContext requestContext);
  }
}
