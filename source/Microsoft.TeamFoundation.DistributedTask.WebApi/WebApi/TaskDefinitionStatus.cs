// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskDefinitionStatus
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public enum TaskDefinitionStatus
  {
    [EnumMember] Preinstalled = 1,
    [EnumMember] ReceivedInstallOrUpdate = 2,
    [EnumMember] Installed = 3,
    [EnumMember] ReceivedUninstall = 4,
    [EnumMember] Uninstalled = 5,
    [EnumMember] RequestedUpdate = 6,
    [EnumMember] Updated = 7,
    [EnumMember] AlreadyUpToDate = 8,
    [EnumMember] InlineUpdateReceived = 9,
  }
}
