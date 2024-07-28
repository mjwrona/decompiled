// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.WebApi.FeedViewNotFoundException
// Assembly: Microsoft.VisualStudio.Services.Feed.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8DACB936-5231-4131-8ED8-082A1F46DC54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Feed.WebApi
{
  [Serializable]
  public class FeedViewNotFoundException : VssServiceException
  {
    public FeedViewNotFoundException(string message)
      : base(message)
    {
    }

    public static FeedViewNotFoundException Create(string feedId, string viewId) => new FeedViewNotFoundException(Resources.Error_FeedViewIdNotFoundMessage((object) viewId, (object) feedId));

    public static FeedViewNotFoundException Create(string viewName) => new FeedViewNotFoundException(Resources.Error_FeedViewNotFoundMessage((object) viewName));
  }
}
