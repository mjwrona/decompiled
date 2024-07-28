// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.DataRetention.Constants
// Assembly: Microsoft.TeamFoundation.VersionControl.Server.DataRetention, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACAD6A6E-265A-4AD6-8B83-B0DD75035C8B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.DataRetention.dll

namespace Microsoft.TeamFoundation.VersionControl.Server.DataRetention
{
  internal class Constants
  {
    public static readonly string RegistryRoot = "/Service/DataRetentionJob/";
    public static readonly string WorkspaceDaysSetting = Constants.RegistryRoot + "Settings/DaysUntilWorkspaceWarning";
    public static readonly string ShelvesetDaysSetting = Constants.RegistryRoot + "Settings/DaysUntilShelvesetWarning";
    public static readonly int InvalidDaysSetting = -100;
  }
}
