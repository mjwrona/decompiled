// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Utils.NpmElementalFeedValidator
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Npm.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ValidationUtils;
using System;

namespace Microsoft.VisualStudio.Services.Npm.Server.Utils
{
  public static class NpmElementalFeedValidator
  {
    public static IValidator<FeedCore> GetFeedIsNotReadOnlyValidator() => (IValidator<FeedCore>) FeedCoreValidator.GetFeedIsNotReadOnlyValidator((Action<FeedCore>) (elementalFeed =>
    {
      throw new ReadOnlyViewOperationException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_CannotPerformOperationOnReadOnlyFeed((object) elementalFeed.FullyQualifiedName));
    }));
  }
}
