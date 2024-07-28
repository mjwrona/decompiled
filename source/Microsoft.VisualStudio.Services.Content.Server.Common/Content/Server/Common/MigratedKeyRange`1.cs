// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.MigratedKeyRange`1
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Content.Server.Common
{
  public struct MigratedKeyRange<TPhysicalNode> : IEquatable<MigratedKeyRange<TPhysicalNode>> where TPhysicalNode : class, IPhysicalNode
  {
    public static readonly IComparer<MigratedKeyRange<TPhysicalNode>> ComparerInstance = (IComparer<MigratedKeyRange<TPhysicalNode>>) new MigratedKeyRange<TPhysicalNode>.KeyRangeComparer();
    public readonly ulong StartKey;
    public readonly ulong EndKey;
    public readonly VirtualNode<TPhysicalNode> OriginalNode;
    public readonly VirtualNode<TPhysicalNode> NewNode;

    public MigratedKeyRange(
      ulong startKey,
      ulong endKey,
      VirtualNode<TPhysicalNode> originalNode,
      VirtualNode<TPhysicalNode> newNode)
    {
      this.StartKey = startKey;
      this.EndKey = endKey;
      this.OriginalNode = originalNode;
      this.NewNode = newNode;
    }

    public ulong KeyCount => this.EndKey - this.StartKey + 1UL;

    public bool Equals(MigratedKeyRange<TPhysicalNode> other) => MigratedKeyRange<TPhysicalNode>.ComparerInstance.Compare(this, other) == 0;

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Keys:[{0},{1}] from '{2}' to '{3}'", (object) this.StartKey, (object) this.EndKey, (object) this.OriginalNode.PhysicalNode.UniqueName, (object) this.NewNode.PhysicalNode.UniqueName);

    private class KeyRangeComparer : IComparer<MigratedKeyRange<TPhysicalNode>>
    {
      public int Compare(MigratedKeyRange<TPhysicalNode> x, MigratedKeyRange<TPhysicalNode> y)
      {
        int num = x.StartKey.CompareTo(y.StartKey);
        if (num == 0)
          num = x.EndKey.CompareTo(y.EndKey);
        return num;
      }
    }
  }
}
