// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementServerConstants
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public static class TestManagementServerConstants
  {
    public const string TestPlanningDatabaseCategory = "TestManagement";
    public static readonly Guid TFSPrincipal = new Guid("00000002-0000-8888-8000-000000000000");
    public static readonly Guid TFSOnPremisesServiceInstanceType = new Guid("000007F5-0000-8888-8000-000000000000");
    public static readonly Guid TFSServiceInstanceType = new Guid("00025394-6065-48CA-87D9-7F5672854EF7");
    public static readonly Guid TCMServiceInstanceType = new Guid("00000054-0000-8888-8000-000000000000");
    internal static readonly int TestManagementRepoulateIntervalInMinutes = 60;
    internal static readonly int TestManagementSuiteSyncIntervalInMinutes = 300;
  }
}
