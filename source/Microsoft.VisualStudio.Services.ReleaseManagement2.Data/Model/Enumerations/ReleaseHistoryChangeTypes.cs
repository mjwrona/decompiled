// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseHistoryChangeTypes
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations
{
  [Flags]
  [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Zero value is defined already")]
  public enum ReleaseHistoryChangeTypes
  {
    Undefined = 0,
    Create = 1,
    Start = 2,
    Update = 4,
    Deploy = 8,
    Approve = 16, // 0x00000010
    Abandon = 32, // 0x00000020
    Delete = 64, // 0x00000040
    Undelete = 128, // 0x00000080
    GateUpdate = 256, // 0x00000100
  }
}
