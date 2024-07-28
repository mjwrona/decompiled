// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TokenStoreParent`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public struct TokenStoreParent<T>
  {
    public readonly T ReferencedObject;
    public readonly string NoChildrenBelow;
    public readonly bool IsExactMatch;

    public TokenStoreParent(T referencedObject, string noChildrenBelow, bool isExactMatch)
    {
      this.ReferencedObject = referencedObject;
      this.NoChildrenBelow = noChildrenBelow;
      this.IsExactMatch = isExactMatch;
    }
  }
}
