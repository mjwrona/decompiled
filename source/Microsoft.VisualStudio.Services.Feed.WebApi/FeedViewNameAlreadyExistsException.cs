// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.WebApi.FeedViewNameAlreadyExistsException
// Assembly: Microsoft.VisualStudio.Services.Feed.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8DACB936-5231-4131-8ED8-082A1F46DC54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Feed.WebApi
{
  [Serializable]
  public class FeedViewNameAlreadyExistsException : VssServiceException
  {
    public FeedViewNameAlreadyExistsException(string message)
      : base(message)
    {
    }

    public static FeedViewNameAlreadyExistsException Create(string viewName, string feedId) => new FeedViewNameAlreadyExistsException(Resources.Error_FeedViewAlreadyExistsMessage((object) viewName, (object) feedId));
  }
}
