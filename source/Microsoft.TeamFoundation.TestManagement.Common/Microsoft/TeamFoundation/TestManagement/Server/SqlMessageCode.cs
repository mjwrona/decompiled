// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.SqlMessageCode
// Assembly: Microsoft.TeamFoundation.TestManagement.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1401105B-6771-499A-8DF3-F3CBE1BB3AE4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.Common.dll

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public enum SqlMessageCode
  {
    Success = 0,
    GenericError = 1500001, // 0x0016E361
    ObjectNotFound = 1500002, // 0x0016E362
    InvalidOperation = 1500003, // 0x0016E363
    MessageQueueDetailsAlreadyExists = 1500004, // 0x0016E364
    TestEnvironmentAlreadyExists = 1500005, // 0x0016E365
  }
}
