// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Hosting.IAzureProviderSettings
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Hosting
{
  public interface IAzureProviderSettings
  {
    TimeSpan DownloadToStreamClientTimeout { get; }

    TimeSpan EnumerateBlobsClientTimeout { get; }

    TimeSpan FetchAttributesClientTimeout { get; }

    TimeSpan GetCloudBlobContainerClientTimeout { get; }

    TimeSpan GetStreamClientTimeout { get; }

    TimeSpan DeleteBlobClientTimeout { get; }

    TimeSpan DeleteContainerClientTimeout { get; }

    TimeSpan PutBlockClientTimeout { get; }

    TimeSpan ReadBlobMetadataClientTimeout { get; }

    TimeSpan ReadBlobPropertiesClientTimeout { get; }

    TimeSpan RenameBlobClientTimeout { get; }

    TimeSpan WriteBlobMetadataClientTimeout { get; }

    TimeSpan BlobExistsClientTimeout { get; }

    TimeSpan GetPageRangesClientTimeout { get; }

    TimeSpan DefaultBlobRequestClientTimeout { get; }

    TimeSpan DefaultTableRequestClientTimeout { get; }

    TimeSpan DefaultQueueRequestClientTimeout { get; }

    int NotificationThreshold { get; }
  }
}
