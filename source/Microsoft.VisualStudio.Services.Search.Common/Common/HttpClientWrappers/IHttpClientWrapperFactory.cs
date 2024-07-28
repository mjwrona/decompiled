// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers.IHttpClientWrapperFactory
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

namespace Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers
{
  public interface IHttpClientWrapperFactory
  {
    ProjectHttpClientWrapper GetProjectHttpClient(ExecutionContext executionContext);

    GitHttpClientWrapper GetGitHttpClient(ExecutionContext executionContext);

    WorkItemHttpClientWrapper GetWorkItemHttpClient(ExecutionContext executionContext);

    TfvcHttpClientWrapper GetTfvcHttpClient(ExecutionContext executionContext);

    TfsContributionsHttpClientWrapper GetTfsContributionsHttpClient(
      ExecutionContext executionContext);

    ProjectAnalysisHttpClientWrapper GetProjectAnalysisHttpClient(ExecutionContext executionContext);

    FeedHttpClientWrapper GetFeedHttpClient(ExecutionContext executionContext);

    SocialHttpClientWrapper GetSocialHttpClient(ExecutionContext executionContext);

    BoardHttpClientWrapper GetBoardHttpClient(ExecutionContext executionContext);
  }
}
