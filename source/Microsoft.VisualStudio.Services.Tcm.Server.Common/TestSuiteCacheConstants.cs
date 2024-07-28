// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.TestSuiteCacheConstants
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;

namespace Microsoft.TeamFoundation.TestManagement
{
  public static class TestSuiteCacheConstants
  {
    public const int TestSuiteCacheMaxElements = 1000;
    public static readonly TimeSpan TestSuiteCacheExpirationInterval = TimeSpan.FromHours(2.0);
    public static readonly TimeSpan TestSuiteCacheCleanupInterval = TimeSpan.FromHours(24.0);
    public static readonly Guid TestSuiteChangedEventClass = new Guid("6CA78412-60BE-4F93-A090-09D76FF0E635");
  }
}
