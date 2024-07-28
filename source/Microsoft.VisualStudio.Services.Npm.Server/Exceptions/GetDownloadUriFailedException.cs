// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Exceptions.GetDownloadUriFailedException
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Npm.Server.Exceptions
{
  public class GetDownloadUriFailedException : VssServiceException
  {
    public GetDownloadUriFailedException(
      IEnumerable<Tuple<BlobIdentifier, string>> missingBlobs,
      Exception exception = null)
      : base(GetDownloadUriFailedException.MakeMessage(missingBlobs), exception)
    {
    }

    public GetDownloadUriFailedException(
      BlobIdentifier blobIdentifier,
      string packageIdentifier,
      Exception exception = null)
      : this((IEnumerable<Tuple<BlobIdentifier, string>>) new Tuple<BlobIdentifier, string>[1]
      {
        Tuple.Create<BlobIdentifier, string>(blobIdentifier, packageIdentifier)
      }, exception)
    {
    }

    private static string MakeMessage(
      IEnumerable<Tuple<BlobIdentifier, string>> missingBlobs)
    {
      return Resources.Error_GetDownloadUriFailed((object) string.Join(", ", missingBlobs.Select<Tuple<BlobIdentifier, string>, string>((Func<Tuple<BlobIdentifier, string>, string>) (x => string.Format("{1} ({0})", (object) x.Item1, (object) x.Item2)))));
    }
  }
}
