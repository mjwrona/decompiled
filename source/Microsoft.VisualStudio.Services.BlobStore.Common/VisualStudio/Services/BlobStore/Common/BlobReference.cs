// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.BlobReference
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using Microsoft.VisualStudio.Services.Content.Common;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  public class BlobReference : 
    EquatableTaggedUnion<IdBlobReference, KeepUntilBlobReference>,
    IEquatable<BlobReference>
  {
    private readonly string IdRefScope;

    public BlobReference(IdBlobReference reference)
      : base(reference)
    {
      this.IdRefScope = reference.Scope;
    }

    public BlobReference(KeepUntilBlobReference reference)
      : base(reference)
    {
    }

    public BlobReference(DateTime date)
      : base(new KeepUntilBlobReference(date))
    {
    }

    public BlobReference(string id, string scope)
      : base(new IdBlobReference(id, scope))
    {
      this.IdRefScope = scope;
    }

    public bool Equals(BlobReference other) => this.Equals((EquatableTaggedUnion<IdBlobReference, KeepUntilBlobReference>) other);

    public override bool Equals(object obj) => this.Equals(obj as BlobReference);

    public static bool operator ==(BlobReference r1, BlobReference r2) => (object) r1 != null ? r1.Equals(r2) : (object) r2 == null;

    public static bool operator !=(BlobReference r1, BlobReference r2) => !(r1 == r2);

    public override int GetHashCode() => EqualityHelper.GetCombinedHashCode((object) typeof (BlobReference), (object) base.GetHashCode());

    public bool IsInThePast(IClock clock) => this.Match<bool>((Func<IdBlobReference, bool>) (idRef => false), (Func<KeepUntilBlobReference, bool>) (keepUntilRef => keepUntilRef.KeepUntil < clock.Now.UtcDateTime));

    public bool IsKeepUntil => this.Match<bool>((Func<IdBlobReference, bool>) (idRef => false), (Func<KeepUntilBlobReference, bool>) (keepUntilRef => true));

    public string Scope => this.IdRefScope;
  }
}
