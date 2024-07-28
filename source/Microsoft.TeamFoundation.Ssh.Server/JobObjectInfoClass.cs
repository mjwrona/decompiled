// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Ssh.Server.JobObjectInfoClass
// Assembly: Microsoft.TeamFoundation.Ssh.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9CD799E8-FCEB-494E-B317-B5A120D16BCC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Ssh.Server.dll

namespace Microsoft.TeamFoundation.Ssh.Server
{
  internal enum JobObjectInfoClass
  {
    BasicAccountingInformation = 1,
    BasicLimitInformation = 2,
    BasicProcessIdList = 3,
    BasicUIRestrictions = 4,
    SecurityLimitInformation = 5,
    EndOfJobTimeInformation = 6,
    AssociateCompletionPortInformation = 7,
    BasicAndIoAccountingInformation = 8,
    ExtendedLimitInformation = 9,
    CpuRateControlInformation = 15, // 0x0000000F
  }
}
