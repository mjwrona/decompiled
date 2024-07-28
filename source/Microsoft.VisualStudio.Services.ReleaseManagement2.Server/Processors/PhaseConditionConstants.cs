// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Processors.PhaseConditionConstants
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Processors
{
  public static class PhaseConditionConstants
  {
    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "By design.")]
    public static class Expressions
    {
      public static readonly string Failed = nameof (Failed);
      public static readonly string Succeeded = nameof (Succeeded);
      public static readonly string SucceededOrFailed = nameof (SucceededOrFailed);
      public static readonly string Variables = nameof (Variables);
    }
  }
}
