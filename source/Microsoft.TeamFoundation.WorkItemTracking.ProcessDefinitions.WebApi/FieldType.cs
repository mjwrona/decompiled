// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.FieldType
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7BABD213-FC9A-4DAB-8690-D2FF2DA1955C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.dll

namespace Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models
{
  public enum FieldType
  {
    String = 1,
    Integer = 2,
    DateTime = 3,
    PlainText = 5,
    Html = 7,
    TreePath = 8,
    History = 9,
    Double = 10, // 0x0000000A
    Guid = 11, // 0x0000000B
    Boolean = 12, // 0x0000000C
    Identity = 13, // 0x0000000D
    PicklistInteger = 14, // 0x0000000E
    PicklistString = 15, // 0x0000000F
    PicklistDouble = 16, // 0x00000010
  }
}
