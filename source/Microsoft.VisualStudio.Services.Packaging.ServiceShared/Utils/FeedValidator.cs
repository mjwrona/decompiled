// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils.FeedValidator
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ValidationUtils;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils
{
  public static class FeedValidator
  {
    public static IValidator<FeedCore> GetFeedIsNotReadOnlyValidator() => (IValidator<FeedCore>) FeedCoreValidator.GetFeedIsNotReadOnlyValidator((Action<FeedCore>) (elementalFeed =>
    {
      throw new ReadOnlyViewOperationException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_CannotPerformOperationOnReadOnlyFeed((object) elementalFeed.FullyQualifiedName));
    }));
  }
}
