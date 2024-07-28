// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Policy.Server.Framework.ActivePolicyEvaluationCacheItem
// Assembly: Microsoft.TeamFoundation.Policy.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C7B03386-B27B-4823-BB4F-89F7D7E42DDD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Policy.Server.dll

using System;

namespace Microsoft.TeamFoundation.Policy.Server.Framework
{
  public struct ActivePolicyEvaluationCacheItem : IEquatable<ActivePolicyEvaluationCacheItem>
  {
    public ActivePolicyEvaluationCacheItem(bool isApplicable) => this.IsApplicable = isApplicable;

    public override bool Equals(object obj) => obj is ActivePolicyEvaluationCacheItem other && this.Equals(other);

    public bool Equals(ActivePolicyEvaluationCacheItem other) => this.IsApplicable == other.IsApplicable;

    public static bool operator ==(
      ActivePolicyEvaluationCacheItem left,
      ActivePolicyEvaluationCacheItem right)
    {
      return left.Equals(right);
    }

    public static bool operator !=(
      ActivePolicyEvaluationCacheItem left,
      ActivePolicyEvaluationCacheItem right)
    {
      return !left.Equals(right);
    }

    public bool IsApplicable { get; }

    public override int GetHashCode() => this.IsApplicable.GetHashCode();

    public override string ToString() => "IsApplicable: " + this.IsApplicable.ToString();
  }
}
