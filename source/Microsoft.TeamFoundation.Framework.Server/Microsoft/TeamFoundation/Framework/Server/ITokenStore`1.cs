// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ITokenStore`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public interface ITokenStore<T>
  {
    int Count { get; }

    bool TryGetValue(string token, out T referencedObject);

    T GetOrAdd<X>(string token, Func<X, T> valueFactory, X valueFactoryParameter = null);

    bool HasSubItem(string token);

    bool Remove(string token, bool recurse);

    T this[string token] { get; set; }

    ITokenStore<T> Copy(IVssRequestContext requestContext);

    void Clear();

    IEnumerable<T> EnumSubTree(string token, bool enumSubTreeRoot = true);

    void EnumAndEvaluateParents(
      string token,
      bool includeSparseNodes,
      Func<string, T, string, bool, bool> evaluate);

    bool IsSubItem(string token, string parentToken);

    void Seal();
  }
}
