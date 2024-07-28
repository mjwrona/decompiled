// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseSqlComponent42
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Versioning mechanism")]
  public class ReleaseSqlComponent42 : ReleaseSqlComponent41
  {
    [SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", Justification = "max value for defaultNumberOfDaysToRetainRelease is imposed by release settings service already")]
    protected override void BindDefaultMaxTimeToRetainRelease(int defaultNumberOfDaysToRetainRelease) => this.BindDateTime("defaultMaxTimeToRetainRelease", DateTimeExtensions.GetMaxYesterdayDateTimeInUtc().AddDaysSaturating((double) -defaultNumberOfDaysToRetainRelease));
  }
}
