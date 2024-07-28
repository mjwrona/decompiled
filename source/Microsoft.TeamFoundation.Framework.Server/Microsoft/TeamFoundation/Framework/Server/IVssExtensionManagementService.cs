// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IVssExtensionManagementService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DefaultServiceImplementation(typeof (VssExtensionManagementService))]
  public interface IVssExtensionManagementService : IVssFrameworkService
  {
    T GetExtension<T>(
      IVssRequestContext requestContext,
      ExtensionLifetime lifetime = ExtensionLifetime.Instance,
      string strategy = null,
      bool throwOnError = false);

    T GetExtension<T>(IVssRequestContext requestContext, Func<T, bool> filter, bool throwOnError = false);

    IDisposableReadOnlyList<T> GetExtensions<T>(
      IVssRequestContext requestContext,
      ExtensionLifetime lifetime = ExtensionLifetime.Instance,
      string strategy = null,
      bool throwOnError = false);

    IDisposableReadOnlyList<T> GetExtensions<T>(
      IVssRequestContext requestContext,
      Func<T, bool> filter,
      bool throwOnError = false);
  }
}
