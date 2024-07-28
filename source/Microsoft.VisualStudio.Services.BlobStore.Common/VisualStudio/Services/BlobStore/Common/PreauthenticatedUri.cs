// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.PreauthenticatedUri
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using System;
using System.Web;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  public struct PreauthenticatedUri
  {
    public readonly Uri NotNullUri;
    public readonly EdgeType EdgeType;

    public PreauthenticatedUri(Uri preauthenticatedNotNullUri, EdgeType edgeType)
    {
      this.NotNullUri = !(preauthenticatedNotNullUri == (Uri) null) ? preauthenticatedNotNullUri : throw new ArgumentNullException(nameof (preauthenticatedNotNullUri));
      this.EdgeType = edgeType;
    }

    public static PreauthenticatedUri? FromPossiblyNullString(
      string possiblyNullUri,
      EdgeType edgeType)
    {
      return possiblyNullUri == null ? new PreauthenticatedUri?() : new PreauthenticatedUri?(new PreauthenticatedUri(new Uri(possiblyNullUri), edgeType));
    }

    public static PreauthenticatedUri? FromPossiblyNullUri(Uri possiblyNullUri, EdgeType edgeType) => possiblyNullUri == (Uri) null ? new PreauthenticatedUri?() : new PreauthenticatedUri?(new PreauthenticatedUri(possiblyNullUri, edgeType));

    private DateTimeOffset GetExpiryTime() => DateTimeOffset.Parse(HttpUtility.ParseQueryString(this.NotNullUri.Query).Get("se")).ToUniversalTime();

    public DateTimeOffset ExpiryTime => this.GetExpiryTime();
  }
}
