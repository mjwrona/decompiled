// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.NamespaceKeyPair
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public struct NamespaceKeyPair : IEquatable<NamespaceKeyPair>
  {
    public readonly Guid NamespaceId;
    public readonly string Key;
    public static readonly NamespaceKeyPair Empty = new NamespaceKeyPair(Guid.Empty, string.Empty);

    public NamespaceKeyPair(Guid namespaceId, string key)
    {
      this.NamespaceId = namespaceId;
      this.Key = key != null ? key.ToLowerInvariant() : string.Empty;
    }

    public bool Equals(NamespaceKeyPair other) => this.NamespaceId == other.NamespaceId && string.Equals(this.Key, other.Key);

    public override bool Equals(object obj) => obj is NamespaceKeyPair other && this.Equals(other);

    public static bool operator ==(NamespaceKeyPair op1, NamespaceKeyPair op2) => op1.Equals(op2);

    public static bool operator !=(NamespaceKeyPair op1, NamespaceKeyPair op2) => !op1.Equals(op2);

    public override int GetHashCode() => (1231 * 3037 + this.NamespaceId.GetHashCode()) * 3037 + this.Key.GetHashCode();
  }
}
