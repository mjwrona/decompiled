// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.WebApi.AzurePublisherExistsException
// Assembly: Microsoft.VisualStudio.Services.Gallery.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EE9D0AAA-B110-4AD6-813B-50FA04AC401A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Gallery.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Gallery.WebApi
{
  [Serializable]
  public class AzurePublisherExistsException : VssServiceException
  {
    public AzurePublisherExistsException(string message)
      : base(message)
    {
    }

    public AzurePublisherExistsException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    public AzurePublisherExistsException(string message, Exception innerException, int errorNumber)
      : base(GalleryWebApiResources.PublisherAlreadyExists(), innerException)
    {
    }
  }
}
