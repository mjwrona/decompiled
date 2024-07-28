// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.Common.CouldNotReferenceBlobException
// Assembly: Microsoft.VisualStudio.Services.ItemStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 44753C0C-D541-4975-AF3F-2B606DE6FF70
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.Common.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ItemStore.Common
{
  [Serializable]
  public class CouldNotReferenceBlobException : VssServiceException
  {
    public CouldNotReferenceBlobException(string message, Exception inner)
      : base(message, inner)
    {
    }

    public CouldNotReferenceBlobException(string message)
      : this(message, (Exception) null)
    {
    }

    public CouldNotReferenceBlobException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public static CouldNotReferenceBlobException Create(IEnumerable<BlobIdentifier> missingBlobIds) => new CouldNotReferenceBlobException(CouldNotReferenceBlobException.MakeMessage(missingBlobIds));

    public static string MakeMessage(IEnumerable<BlobIdentifier> missingBlobIds) => "Could not reference blobs with ids: " + string.Join(", ", missingBlobIds.Select<BlobIdentifier, string>((Func<BlobIdentifier, string>) (blobId => blobId.ValueString)).ToArray<string>());
  }
}
