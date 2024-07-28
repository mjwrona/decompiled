// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.ReferenceFactory
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts
{
  public class ReferenceFactory
  {
    public static Reference MakeReference(
      BlobReference blobReference,
      string blobIdStr,
      bool? isMissing = null)
    {
      return ReferenceFactory.MakeReference(blobReference, BlobIdentifier.Deserialize(blobIdStr), isMissing);
    }

    public static Reference MakeReference(
      BlobReference blobReference,
      BlobIdentifier blobIdentifier,
      bool? isMissing = null)
    {
      return ReferenceFactory.MakeReference(blobReference, new Blob(blobIdentifier), isMissing);
    }

    public static Reference MakeReference(
      BlobReference blobReference,
      Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks blobIdentifier,
      bool? isMissing = null)
    {
      return ReferenceFactory.MakeReference(blobReference, new Blob(blobIdentifier), isMissing);
    }

    public static Reference MakeReference(BlobReference blobReference, Blob blob, bool? isMissing = null) => blobReference.Match<Reference>((Func<IdBlobReference, Reference>) (idRef => (Reference) new IdReference(idRef, blob, isMissing)), (Func<KeepUntilBlobReference, Reference>) (keepUntilRef => (Reference) new KeepUntilReference(keepUntilRef, blob, isMissing)));
  }
}
