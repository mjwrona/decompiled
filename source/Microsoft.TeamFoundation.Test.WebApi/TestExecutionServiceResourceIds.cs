// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Test.WebApi.TestExecutionServiceResourceIds
// Assembly: Microsoft.TeamFoundation.Test.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17829F78-DAC0-47C1-AC4C-95D401C54448
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Test.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Test.WebApi
{
  [GenerateAllConstants(null)]
  public static class TestExecutionServiceResourceIds
  {
    public const string TestExecutionServiceArea = "Test";
    public const string TestExecutionServiceAreaId = "3b95fb80-fdda-4218-b60e-1052d070ae6b";
    public const string TestExecutionResource = "TestExecution";
    public const string AgentResource = "Agents";
    public static readonly Guid AgentLocationId = new Guid("0F1857DE-6E56-4010-9EA7-F29B80B911C4");
    public const string AutomationRunResource = "AutomationRuns";
    public static readonly Guid AutomationRunLocationId = new Guid("315806B7-1F2B-4368-B94B-0E469F5E12FC");
    public const string SliceResource = "Slices";
    public static readonly Guid SliceLocationId = new Guid("575891B2-50A3-474F-A963-7CA011C97500");
    public const string CommandResource = "Commands";
    public static readonly Guid CommandLocationId = new Guid("5B78449B-A866-4726-B989-9083EB2D977C");
    public const string DistributedTestRunResource = "DistributedTestRuns";
    public static readonly Guid DistributedTestRunLocationId = new Guid("B7C4FE2A-9DD1-4DAE-8B77-8412002DE5A4");
    public const string TestExecutionConfigurationResource = "TestExecutionConfiguration";
    public static readonly Guid TestExecutionConfigurationLocationId = new Guid("30421B98-AC6A-48AD-A2BF-0CAD4528183F");
    public const string TestExecutionControlOptionsResource = "TestExecutionControlOptions";
    public static readonly Guid TestExecutionControlOptionsLocationId = new Guid("CBD7E2A6-A3BA-4C32-825F-2F48896CCCA7");
  }
}
