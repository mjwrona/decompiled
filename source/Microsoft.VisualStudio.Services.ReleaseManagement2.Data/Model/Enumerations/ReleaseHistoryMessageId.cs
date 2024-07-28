// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseHistoryMessageId
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations
{
  public enum ReleaseHistoryMessageId
  {
    UnknownChange = 0,
    ReleaseStatusChange = 701000, // 0x000AB248
    EnvironmentStatusChange = 701001, // 0x000AB249
    ApprovalStatusChange = 701002, // 0x000AB24A
    ReassignedApprovalChange = 701003, // 0x000AB24B
    ReleaseCreateOrUpdateChange = 701004, // 0x000AB24C
    EnvironmentStatusChangeByQueuingPolicy = 701005, // 0x000AB24D
    EnvironmentStatusChangeByScheduleDeletion = 701006, // 0x000AB24E
    EnvironmentStatusChangeByReleaseDeletion = 701007, // 0x000AB24F
    EnvironmentStatusChangeByAbandonRelease = 701008, // 0x000AB250
    EnvironmentStatusCanceledByQueuingPolicy = 701009, // 0x000AB251
    DeploymentGateChange = 701010, // 0x000AB252
  }
}
