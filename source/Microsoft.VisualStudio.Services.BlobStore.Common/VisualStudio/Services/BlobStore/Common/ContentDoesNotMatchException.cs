// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.ContentDoesNotMatchException
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using BuildXL.Cache.ContentStore.Hashing;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  [Serializable]
  public class ContentDoesNotMatchException : Exception
  {
    public ContentDoesNotMatchException()
      : base("The calculated content identity does not match with the content identity provided")
    {
    }

    public ContentDoesNotMatchException(string message)
      : base(message)
    {
    }

    public ContentDoesNotMatchException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    public ContentDoesNotMatchException(BlobIdentifier givenId, BlobIdentifier computedId)
      : this(string.Format("The given BlobIdentifier {0} does not match the computed BlobIdentifier {1}.", (object) givenId.ValueString, (object) computedId.ValueString))
    {
    }

    protected ContentDoesNotMatchException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
