// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.TestPointFiltersCacheConstants
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;

namespace Microsoft.TeamFoundation.TestManagement
{
  public static class TestPointFiltersCacheConstants
  {
    public const int TestPointFiltersCacheMaxElements = 100;
    public static readonly TimeSpan TestPointFiltersCacheExpirationInterval = TimeSpan.FromHours(2.0);
    public static readonly TimeSpan TestPointFiltersCacheCleanupInterval = TimeSpan.FromHours(24.0);
    public static readonly Guid TestPointFiltersTestersChangedEventClass = new Guid("58324AD3-FB4E-4DCC-8060-2519E1C5D324");
    public static readonly Guid TestPointFiltersConfigurationsChangedEventClass = new Guid("58324AD3-FB4E-4DCC-8060-2519E1C5D324");
  }
}
