// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.MigratePlanConstants
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

namespace Microsoft.TeamFoundation.TestManagement
{
  public static class MigratePlanConstants
  {
    public const int TestSettingsPageSize = 1000;
    public const int FetchLimit = 10;
    public const int UpdateArtifactLimit = 100000;
    public const int MigratePlanRetryCount = 3;
    public const bool ByPassWitValidationChecks = true;
    public const int IdReplacementAtrifactLimit = 5000;
    public const string XPathToProjectNameInMigratePlansJobData = "//@teamProjectName";
    public const string MigratePlansJobExtensionName = "Microsoft.TeamFoundation.TestManagement.Server.Jobs.MigratePlansJob";
    public const string MigratePlansJobDataXml = "<MigratePlansJob teamProjectName=\"{0}\"/>";
  }
}
