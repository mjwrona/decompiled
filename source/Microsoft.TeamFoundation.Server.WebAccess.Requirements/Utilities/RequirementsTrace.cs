// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Requirements.Utilities.RequirementsTrace
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Requirements, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6C113FD4-8DA1-49E9-A859-47B7ED9A5698
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Requirements.dll

namespace Microsoft.TeamFoundation.Server.WebAccess.Requirements.Utilities
{
  internal static class RequirementsTrace
  {
    public static readonly string[] Keywords = new string[1]
    {
      TraceKeywords.TSWebAccess
    };
    public const string TraceArea = "WebAccess.Feedback";
    private const int RequirementsStart = 300000;
    private const int ApiControllersBase = 301000;
    private const int ControllersBase = 310000;
    private const int BusinessLogicBase = 330000;
    private const int FeedbackApiControllerBase = 301000;
    private const int FeedbackControllerBase = 310000;
    public const int DefaultPageTraceBegin = 310000;
    public const int DefaultPageTraceEnd = 310010;
    public const int ConfigurationTraceBegin = 301000;
    public const int ConfigurationTraceEnd = 301010;
  }
}
