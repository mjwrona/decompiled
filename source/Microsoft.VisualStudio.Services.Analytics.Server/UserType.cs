// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.UserType
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

namespace Microsoft.VisualStudio.Services.Analytics
{
  public static class UserType
  {
    public const byte Unknown = 0;
    public const byte Unrecognized = 1;
    public const byte User = 2;
    public const byte Organization = 3;
    public const byte Bot = 4;
    public const string UserStageString = "User";
    public const string OrganizationStageString = "Organization";
    public const string BotStageString = "Bot";
  }
}
