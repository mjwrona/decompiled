// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.RawFeedId
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

namespace Microsoft.VisualStudio.Services.Npm.Server
{
  public class RawFeedId
  {
    public string FeedId { get; }

    public string ProjectId { get; }

    public RawFeedId(string rawFeedId, string rawProjectId)
    {
      this.FeedId = rawFeedId;
      this.ProjectId = rawProjectId;
    }

    public RawFeedId(string feedId) => this.FeedId = feedId;
  }
}
