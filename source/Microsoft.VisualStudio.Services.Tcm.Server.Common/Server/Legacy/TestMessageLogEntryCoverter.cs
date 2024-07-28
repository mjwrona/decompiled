// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Legacy.TestMessageLogEntryCoverter
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

namespace Microsoft.TeamFoundation.TestManagement.Server.Legacy
{
  internal static class TestMessageLogEntryCoverter
  {
    public static Microsoft.TeamFoundation.TestManagement.Server.TestMessageLogEntry Convert(
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestMessageLogEntry logEntry)
    {
      if (logEntry == null)
        return (Microsoft.TeamFoundation.TestManagement.Server.TestMessageLogEntry) null;
      return new Microsoft.TeamFoundation.TestManagement.Server.TestMessageLogEntry()
      {
        TestMessageLogId = logEntry.TestMessageLogId,
        DateCreated = logEntry.DateCreated,
        EntryId = logEntry.EntryId,
        LogLevel = logEntry.LogLevel,
        LogUser = logEntry.LogUser,
        LogUserName = logEntry.LogUserName,
        Message = logEntry.Message
      };
    }

    public static Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestMessageLogEntry Convert(
      Microsoft.TeamFoundation.TestManagement.Server.TestMessageLogEntry logEntry)
    {
      if (logEntry == null)
        return (Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestMessageLogEntry) null;
      return new Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestMessageLogEntry()
      {
        TestMessageLogId = logEntry.TestMessageLogId,
        DateCreated = logEntry.DateCreated,
        EntryId = logEntry.EntryId,
        LogLevel = logEntry.LogLevel,
        LogUser = logEntry.LogUser,
        LogUserName = logEntry.LogUserName,
        Message = logEntry.Message
      };
    }
  }
}
