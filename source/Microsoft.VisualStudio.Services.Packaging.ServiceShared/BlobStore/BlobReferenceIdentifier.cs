// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobStore.BlobReferenceIdentifier
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using BuildXL.Cache.ContentStore.Hashing;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobStore
{
  [Serializable]
  public class BlobReferenceIdentifier
  {
    public BlobReferenceIdentifier(BlobIdentifier blobIdentifier, string name, string scope)
    {
      this.BlobIdentifier = blobIdentifier;
      this.Name = name;
      this.Scope = scope;
    }

    public BlobIdentifier BlobIdentifier { get; }

    public string Name { get; }

    public string Scope { get; }

    public override string ToString() => string.Format("{0}:{1}:{2}", (object) this.BlobIdentifier, (object) this.Name, (object) this.Scope);
  }
}
