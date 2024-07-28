// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ISecuredRegistryManager
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DefaultServiceImplementation(typeof (SecuredRegistryManager))]
  internal interface ISecuredRegistryManager : IVssFrameworkService
  {
    List<RegistryEntry> QueryRegistryEntries(
      IVssRequestContext requestContext,
      string registryPathPattern,
      bool includeFolders);

    bool UserHiveOperationsAllowed(IVssRequestContext requestContext);

    List<RegistryEntry> QueryUserEntries(
      IVssRequestContext requestContext,
      string registryPathPattern,
      bool includeFolders);

    void UpdateRegistryEntries(IVssRequestContext requestContext, RegistryEntry[] registryEntries);

    void UpdateUserEntries(IVssRequestContext requestContext, RegistryEntry[] registryEntries);

    int RemoveRegistryEntries(IVssRequestContext requestContext, string[] registryPathPatterns);

    int RemoveUserEntries(IVssRequestContext requestContext, string[] registryPathPatterns);

    List<RegistryAuditEntry> QueryAuditLog(
      IVssRequestContext requestContext,
      int changeIndex,
      bool returnOlder);
  }
}
