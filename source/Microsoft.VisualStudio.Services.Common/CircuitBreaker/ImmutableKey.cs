// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CircuitBreaker.ImmutableKey
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.CircuitBreaker
{
  public abstract class ImmutableKey : IEquatable<ImmutableKey>
  {
    private readonly string name;

    protected ImmutableKey(string name) => this.name = !string.IsNullOrWhiteSpace(name) ? name : throw new ArgumentNullException(nameof (name));

    public string Name => this.name;

    public static bool operator ==(ImmutableKey left, ImmutableKey right) => (object) left == null ? (object) right == null : left.Equals(right);

    public static bool operator !=(ImmutableKey left, ImmutableKey right) => !(left == right);

    public override bool Equals(object obj) => obj != null && this.Equals(obj as ImmutableKey);

    public override int GetHashCode() => this.name.GetHashCode();

    public override string ToString() => this.name;

    public bool Equals(ImmutableKey other) => (object) other != null && !(this.GetType() != other.GetType()) && this.name.Equals(other.name, StringComparison.Ordinal);
  }
}
