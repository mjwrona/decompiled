// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ITeamFoundationStrongBoxService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DefaultServiceImplementation(typeof (TeamFoundationStrongBoxService))]
  public interface ITeamFoundationStrongBoxService : IVssFrameworkService
  {
    void AddString(IVssRequestContext requestContext, StrongBoxItemInfo info, string value);

    void AddString(
      IVssRequestContext requestContext,
      Guid drawerId,
      string lookupKey,
      string value);

    void AddStrings(IVssRequestContext requestContext, List<Tuple<StrongBoxItemInfo, string>> items);

    Guid CreateDrawer(IVssRequestContext requestContext, string name);

    Guid CreateDrawerWithExplicitSigningKey(
      IVssRequestContext requestContext,
      string name,
      Guid signingKeyId);

    void DeleteDrawer(IVssRequestContext requestContext, Guid drawerId);

    void DeleteItem(IVssRequestContext requestContext, Guid drawerId, string lookupKey);

    bool IsDrawerEmpty(IVssRequestContext requestContext, Guid drawerId);

    List<StrongBoxItemInfo> GetDrawerContents(IVssRequestContext requestContext, Guid drawerId);

    List<StrongBoxItemInfo> GetDrawerContentsContainingPartialLookupKey(
      IVssRequestContext requestContext,
      Guid drawerId,
      string partialLookupKey);

    ICollection<string> GetDrawerNames(
      IVssRequestContext requestContext,
      SigningKeyType? keyTypeFilter = null);

    StrongBoxItemInfo GetItemInfo(
      IVssRequestContext requestContext,
      Guid drawerId,
      string lookupKey);

    StrongBoxItemInfo GetItemInfo(
      IVssRequestContext requestContext,
      Guid drawerId,
      string lookupKey,
      bool throwIfNotFound);

    StrongBoxItemInfo GetItemInfo(
      IVssRequestContext requestContext,
      string drawerName,
      string lookupKey,
      bool throwIfNotFound);

    SecureString GetSecureString(
      IVssRequestContext requestContext,
      Guid drawerId,
      string lookupKey);

    SecureString GetSecureString(IVssRequestContext requestContext, StrongBoxItemInfo item);

    SigningKeyType GetSigningKeyType(IVssRequestContext requestContext, Guid drawerId);

    string GetString(IVssRequestContext requestContext, Guid drawerId, string lookupKey);

    string GetString(IVssRequestContext requestContext, StrongBoxItemInfo item);

    void RegisterNotification(
      IVssRequestContext requestContext,
      StrongBoxItemChangedCallback callback,
      string drawerName,
      IEnumerable<string> filters);

    Stream RetrieveFile(
      IVssRequestContext requestContext,
      Guid drawerId,
      string lookupKey,
      out long streamLength);

    X509Certificate2 RetrieveFileAsCertificate(
      IVssRequestContext requestContext,
      Guid drawerId,
      string lookupKey,
      bool exportable = false,
      bool throwOnFailure = true);

    void RotateSigningKey(IVssRequestContext requestContext, Guid drawerId, SigningKeyType keyType = SigningKeyType.Default);

    bool ShouldRotate(IVssRequestContext requestContext, Guid drawerId, TimeSpan? maxKeyAge);

    void FinishSigningKeyRotation(IVssRequestContext requestContext, Guid drawerId);

    void FinishSigningKeyRotation2(IVssRequestContext requestContext);

    Guid UnlockDrawer(IVssRequestContext requestContext, string name, bool throwOnFailure);

    Guid UnlockOrCreateDrawer(IVssRequestContext requestContext, string name);

    void UnregisterNotification(
      IVssRequestContext requestContext,
      StrongBoxItemChangedCallback callback);

    void UpdateDrawerSigningKey(
      IVssRequestContext requestContext,
      Guid drawerId,
      Guid signingKeyId);

    void UploadFile(
      IVssRequestContext requestContext,
      Guid drawerId,
      string lookupKey,
      Stream content);

    void UploadFile(IVssRequestContext requestContext, StrongBoxItemInfo item, Stream content);

    void UploadSecureFile(
      IVssRequestContext requestContext,
      Guid drawerId,
      string lookupKey,
      Stream content);

    void DeleteStrongBoxOrphans(IVssRequestContext requestContext);
  }
}
