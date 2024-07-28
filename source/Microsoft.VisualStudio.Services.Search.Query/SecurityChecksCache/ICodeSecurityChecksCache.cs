// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.SecurityChecksCache.ICodeSecurityChecksCache
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Query.SecurityChecksCache
{
  public interface ICodeSecurityChecksCache
  {
    void UpdateRepositorySetsCache(
      IVssRequestContext userRequestContext,
      List<byte[]> userRepositorySets);

    bool TryGetUserRepositorySets(
      IVssRequestContext userRequestContext,
      out List<byte[]> userRepositorySets);
  }
}
