// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.ApprovalValidatorCalls
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server
{
  public class ApprovalValidatorCalls
  {
    public ApprovalValidatorCalls.Validator IsRequestorApprover { get; set; }

    public Func<ReleaseEnvironmentStep, bool> IsRequestorAuthorizedGroupMember { get; set; }

    public Func<int, bool> IsRequestorReleaseManager { get; set; }

    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Needed for testability")]
    public delegate bool Validator(int approvalId, ReleaseEnvironmentStep releaseStep);
  }
}
