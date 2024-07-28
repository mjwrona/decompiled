// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.SecretAccess.ISecretsAccessor
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.SecretAccess
{
  public interface ISecretsAccessor
  {
    Guid GetOrCreateDrawer(IVssRequestContext requestContext, string drawerName);

    void StoreValue(IVssRequestContext requestContext, Guid drawerId, string key, string value);

    void StoreValues(
      IVssRequestContext requestContext,
      Guid drawerId,
      string keyPrefix,
      IDictionary<string, VariableValue> secretVariables);

    void DeleteValue(IVssRequestContext requestContext, Guid drawerId, string key);

    void DeleteDrawer(IVssRequestContext requestContext, Guid drawerId);

    void ReadValues(
      IVssRequestContext requestContext,
      Guid drawerId,
      HashSet<string> lookupKeys,
      Action<string, string> secretSetter);

    [Obsolete("Use ReadValues to avoid multiple calls for the same drawer")]
    bool TryGetStrongBoxValue(
      IVssRequestContext requestContext,
      Guid drawerId,
      string key,
      out string value);

    Guid UnlockDrawer(IVssRequestContext requestContext, string drawerName);
  }
}
