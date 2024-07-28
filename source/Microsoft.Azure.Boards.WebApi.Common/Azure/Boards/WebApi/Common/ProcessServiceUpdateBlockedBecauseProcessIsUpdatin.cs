// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WebApi.Common.ProcessServiceUpdateBlockedBecauseProcessIsUpdatingException
// Assembly: Microsoft.Azure.Boards.WebApi.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC99C479-6852-4E74-BCA4-2660760F9D83
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.WebApi.Common.dll

namespace Microsoft.Azure.Boards.WebApi.Common
{
  public class ProcessServiceUpdateBlockedBecauseProcessIsUpdatingException : 
    ProcessAdminServiceException
  {
    public ProcessServiceUpdateBlockedBecauseProcessIsUpdatingException(string processName)
      : base(ResourceStrings.ProcessServiceUpdateBlocked((object) processName), 1300701)
    {
    }
  }
}
