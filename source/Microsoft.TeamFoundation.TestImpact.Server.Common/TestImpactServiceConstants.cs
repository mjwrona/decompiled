// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestImpact.Server.Common.TestImpactServiceConstants
// Assembly: Microsoft.TeamFoundation.TestImpact.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 079E4AEE-0642-4BDD-8B76-CECF38EBB798
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestImpact.Server.Common.dll

namespace Microsoft.TeamFoundation.TestImpact.Server.Common
{
  public class TestImpactServiceConstants
  {
    public const int TestImpactTracePointStart = 15113000;
    public const int TestImpactTracePointEnd = 15113999;
    public const int PublishBuildChangesStart = 15113001;
    public const int PublishBuildChangesEnd = 15113020;
    public const int DeleteBuildImpactStart = 15113021;
    public const int DeleteBuildImpactEnd = 15113040;
    public const int QueryBuildCodeChangesStart = 15113041;
    public const int QueryBuildCodeChangesEnd = 15113060;
    public const int QueryImpactedTestsStart = 15113061;
    public const int QueryImpactedTests = 15113061;
    public const int QueryImpactedTestsException = 15113071;
    public const int QueryImpactedTestsEnd = 15113080;
    public const int QueryTestCaseSignaturesStart = 15113081;
    public const int QueryTestCaseSignaturesEnd = 15113090;
    public const int PublishCodeSignaturesStart = 15113091;
    public const int PublishCodeSignaturesException = 15113095;
    public const int PublishCodeSignaturessEnd = 15113100;
    public const int BuildDeletionEventStart = 15113101;
    public const int BuildDeletionEventEnd = 15113110;
    public const int BuildDefinitionChangeEventStart = 15113111;
    public const int BuildDefinitionChangeEventEnd = 15113120;
    public const int CleanUpJobStart = 15113121;
    public const int CleanUpJobEnd = 15113140;
    public const int XAMLBuildDeletionEventStart = 15113141;
    public const int XAMLBuildDeletionEventEnd = 15113150;
    public const int LegacyTIACleanUpJobStart = 15113151;
    public const int LegacyTIACleanUpJobEnd = 15113170;
    public const int LegacyTIASoftDeleteJobStart = 15113171;
    public const int LegacyTIASoftDeleteJobEnd = 15113190;
    public static readonly string TestImpactArea = "TestImpact";
    public static readonly string ControllerLayer = "Controller";
    public static readonly string ServiceLayer = "Service";
    public static readonly string DataBaseLayer = "Database";
    public static readonly string ListenerLayer = "Listener";
    public static readonly string JobsLayer = "Jobs";
    public static readonly string CIServiceLayer = "CIService";
    public const int CIServicePublish = 15113998;
    public const int PubishCodeSignaturesPageSize = 500;
  }
}
