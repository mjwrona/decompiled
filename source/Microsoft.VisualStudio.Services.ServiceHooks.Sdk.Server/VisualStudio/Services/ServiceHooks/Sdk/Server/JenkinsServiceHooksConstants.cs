// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.JenkinsServiceHooksConstants
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3575E571-FF3A-4E7B-A8CC-64FFB01E8C91
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.dll

namespace Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server
{
  public class JenkinsServiceHooksConstants
  {
    public const string JenkinsPublisherId = "jenkins";
    public const string JenkinsApiVersion = "2.0";
    public const string JenkinsJobCompletionEventType = "job:completion";
    public const string JenkinsDrawerName = "Jenkins";

    public class InputId
    {
      public const string Server = "server";
      public const string JobDetails = "job.details";
      public const string JobName = "job.name";
      public const string JobResult = "job.result";
      public const string JobUrl = "job.url";
    }
  }
}
