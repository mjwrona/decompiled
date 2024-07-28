// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.IdBlobReference
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using Microsoft.VisualStudio.Services.Content.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  [DataContract]
  [Serializable]
  public struct IdBlobReference : IEquatable<IdBlobReference>
  {
    public IdBlobReference(string name, string scope)
    {
      this.Name = name;
      this.Scope = scope;
      this.Validate();
    }

    [DataMember]
    public string Name { get; }

    [DataMember]
    public string Scope { get; }

    public override string ToString() => string.Format("{0}/{1}", (object) (this.Scope ?? "null"), (object) this.Name);

    public bool Equals(IdBlobReference other) => (ValueType) other != null && string.Equals(this.Scope, other.Scope, StringComparison.Ordinal) && string.Equals(this.Name, other.Name, StringComparison.Ordinal);

    public override bool Equals(object obj) => obj is IdBlobReference other && this.Equals(other);

    public static bool operator ==(IdBlobReference r1, IdBlobReference r2) => r1.Equals(r2);

    public static bool operator !=(IdBlobReference r1, IdBlobReference r2) => !(r1 == r2);

    public override int GetHashCode() => EqualityHelper.GetCombinedHashCode((object) this.Scope, (object) this.Name);

    public void Validate()
    {
      if (string.IsNullOrWhiteSpace(this.Name))
        throw new ArgumentOutOfRangeException("Name", "must not be null, empty, or whitespace");
      if (this.Scope != null && string.IsNullOrWhiteSpace(this.Scope))
        throw new ArgumentOutOfRangeException("Scope", "must not be empty or whitespace");
    }

    [System.Runtime.Serialization.OnSerialized]
    private void OnSerialized(StreamingContext context) => this.Validate();

    [System.Runtime.Serialization.OnDeserialized]
    private void OnDeserialized(StreamingContext context) => this.Validate();
  }
}
