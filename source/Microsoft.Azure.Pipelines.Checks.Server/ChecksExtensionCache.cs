// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.Server.ChecksExtensionCache
// Assembly: Microsoft.Azure.Pipelines.Checks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D08C7285-654E-4A4D-BA46-748F0D1E96AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Pipelines.Checks.Server
{
  internal sealed class ChecksExtensionCache : IDisposable
  {
    private readonly IDisposableReadOnlyList<ICheckType> m_checksExtensions;
    private readonly Dictionary<Guid, ICheckType> m_checkIdTypeMap;
    private readonly Dictionary<string, ICheckType> m_checkNameTypeMap;

    internal ChecksExtensionCache(IDisposableReadOnlyList<ICheckType> allExtensions)
    {
      ArgumentUtility.CheckForNull<IDisposableReadOnlyList<ICheckType>>(allExtensions, nameof (allExtensions));
      this.m_checksExtensions = allExtensions;
      this.m_checkIdTypeMap = allExtensions.ToDictionary<ICheckType, Guid>((Func<ICheckType, Guid>) (p => p.CheckTypeId));
      this.m_checkNameTypeMap = allExtensions.ToDictionary<ICheckType, string>((Func<ICheckType, string>) (p => p.CheckTypeName), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    public void Dispose() => this.m_checksExtensions.Dispose();

    public bool TryGetValue(Guid key, out ICheckType value) => this.m_checkIdTypeMap.TryGetValue(key, out value);

    public bool TryGetValue(string key, out ICheckType value) => this.m_checkNameTypeMap.TryGetValue(key, out value);
  }
}
