// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseStatus
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations
{
  [Flags]
  [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "We use Undefined in place of None")]
  public enum DeployPhaseStatus
  {
    Undefined = 0,
    NotStarted = 1,
    InProgress = 2,
    PartiallySucceeded = 4,
    Succeeded = 8,
    Failed = 16, // 0x00000010
    Canceled = 32, // 0x00000020
    Skipped = 64, // 0x00000040
    [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Cancelling", Justification = "State name")] Cancelling = 128, // 0x00000080
  }
}
