// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleConditionType
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 141BFF87-1CDC-4DC5-9A7C-F7D6EA031F34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models
{
  [DataContract]
  public enum RuleConditionType
  {
    When = 1,
    WhenNot = 2,
    WhenChanged = 3,
    WhenNotChanged = 4,
    WhenWas = 5,
    WhenStateChangedTo = 6,
    WhenStateChangedFromAndTo = 7,
    WhenWorkItemIsCreated = 8,
    WhenValueIsDefined = 9,
    WhenValueIsNotDefined = 10, // 0x0000000A
    WhenCurrentUserIsMemberOfGroup = 11, // 0x0000000B
    WhenCurrentUserIsNotMemberOfGroup = 12, // 0x0000000C
  }
}
