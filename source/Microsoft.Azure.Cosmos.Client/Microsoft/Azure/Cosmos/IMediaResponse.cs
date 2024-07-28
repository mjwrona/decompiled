// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.IMediaResponse
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System.Collections.Specialized;
using System.IO;

namespace Microsoft.Azure.Cosmos
{
  internal interface IMediaResponse
  {
    string ActivityId { get; }

    long ContentLength { get; }

    string ContentType { get; }

    long CurrentMediaStorageUsageInMB { get; }

    long MaxMediaStorageUsageInMB { get; }

    Stream Media { get; }

    NameValueCollection ResponseHeaders { get; }

    string Slug { get; }
  }
}
