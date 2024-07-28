// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Profile.TracePoints
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server.Profile
{
  public static class TracePoints
  {
    public static class UserPreferencesServiceTracePoints
    {
      public const int LoadUserPreferencesFromUserException = 21003006;
      public const int SetUserPreferencesEnter = 21003007;
      public const int SetUserPreferencesLeave = 21003008;
      public const int SetUserPreferencesException = 21003009;
      public const int GetUserPreferencesEnter = 21003010;
      public const int GetUserPreferencesLeave = 21003011;
      public const int GetUserPreferencesException = 21003012;
    }

    public static class UserPreferencesCacheServiceTracePoints
    {
      public const int ServiceStartEnter = 21003500;
      public const int ServiceStartLeave = 21003501;
      public const int ServiceEndEnter = 21003502;
      public const int ServiceEndLeave = 21003503;
    }
  }
}
