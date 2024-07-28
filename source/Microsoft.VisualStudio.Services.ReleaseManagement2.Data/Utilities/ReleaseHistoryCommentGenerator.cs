// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ReleaseHistoryCommentGenerator
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities
{
  public static class ReleaseHistoryCommentGenerator
  {
    public static string GetComment(string inputMessage)
    {
      if (inputMessage.IsNullOrEmpty<char>())
        return (string) null;
      switch (ReleaseHistoryChangeDetailsExtensions.ExtractInt(inputMessage, "messageId"))
      {
        case 701005:
          return Resources.EnvironmentResetByQueuingPolicyComment;
        case 701006:
          return Resources.EnvironmentResetBySchedulesDeletionComment;
        case 701007:
          return Resources.EnvironmentResetByReleaseDeletionComment;
        case 701008:
          return Resources.EnvironmentResetByAbandonReleaseComment;
        case 701009:
          return Resources.EnvironmentCancelledByQueuingPolicyComment;
        default:
          return (string) null;
      }
    }
  }
}
