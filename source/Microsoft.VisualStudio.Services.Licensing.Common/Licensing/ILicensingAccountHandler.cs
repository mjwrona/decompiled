// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.ILicensingAccountHandler
// Assembly: Microsoft.VisualStudio.Services.Licensing.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F3070F25-7414-49A0-9C00-005379F04A49
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Account;
using System;
using System.ComponentModel.Composition;

namespace Microsoft.VisualStudio.Services.Licensing
{
  [InheritedExport]
  public interface ILicensingAccountHandler
  {
    void PublishAccountMembershipEvent(
      IVssRequestContext requestContext,
      AccountUser accountUser,
      AccountMembershipKind membershipKind);

    void PublishUserStatusChangedEvent(
      IVssRequestContext collectionContext,
      Guid userId,
      AccountUserStatus status,
      AccountMembershipKind operationKind);

    void PublishUserLicenseChangedEvent(
      IVssRequestContext collectionContext,
      Guid userId,
      License license);

    void PublishUserExtensionChangedEvent(
      IVssRequestContext collectionContext,
      Guid userId,
      string extensionId);
  }
}
