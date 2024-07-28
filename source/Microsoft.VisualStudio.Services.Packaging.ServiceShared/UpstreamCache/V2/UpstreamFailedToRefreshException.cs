// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache.V2.UpstreamFailedToRefreshException
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache.V2
{
  [Serializable]
  internal class UpstreamFailedToRefreshException : VssServiceException
  {
    public UpstreamFailedToRefreshException(FeedCore feed, string packageName)
      : base(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_UpstreamFailedToRefresh((object) feed.Id, (object) packageName))
    {
    }
  }
}
