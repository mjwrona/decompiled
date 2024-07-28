// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.DataStore.PsFieldDefinitionTypeEnum
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.DataStore
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public enum PsFieldDefinitionTypeEnum
  {
    FieldDefinitionTypeSingleValuedKeyword = 16, // 0x00000010
    FieldDefinitionTypeSingleValuedKeyword_Person = 24, // 0x00000018
    FieldDefinitionTypeSingleValuedInteger = 32, // 0x00000020
    FieldDefinitionTypeSingleValuedDatetime = 48, // 0x00000030
    FieldDefinitionTypeSingleValuedLargeText_PlainText = 64, // 0x00000040
    FieldDefinitionTypeTreeNode = 160, // 0x000000A0
    FieldDefinitionTypeSingleValuedGuid = 208, // 0x000000D0
    FieldDefinitionTypeSingleValuedBoolean = 224, // 0x000000E0
    FieldDefinitionTypeSingleValuedDouble = 240, // 0x000000F0
    FieldDefinitionTypeSingleValuedKeyword_TreePath = 272, // 0x00000110
    FieldDefinitionTypeSingleValuedInteger_TreeID = 288, // 0x00000120
    FieldDefinitionTypeSingleValuedLargeText_History = 320, // 0x00000140
    FieldDefinitionTypeSingleValuedKeyword_TreeNodeName = 528, // 0x00000210
    FieldDefinitionTypeSingleValuedLargeText_HTML = 576, // 0x00000240
    FieldDefinitionTypeSingleValuedKeyword_TreeNodeType = 784, // 0x00000310
  }
}
