// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentStatus
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations
{
  [Flags]
  [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Zero value is defined already")]
  public enum DeploymentStatus
  {
    Undefined = 0,
    NotDeployed = 1,
    InProgress = 2,
    Succeeded = 4,
    PartiallySucceeded = 8,
    Failed = 16, // 0x00000010
    All = Failed | PartiallySucceeded | Succeeded | InProgress | NotDeployed, // 0x0000001F
  }
}
