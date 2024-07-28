// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.FieldDBType
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common
{
  [Flags]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public enum FieldDBType
  {
    MaskReadOnly = 1,
    MaskIgnore = 2,
    MaskStoreAsReference = 8,
    Keyword = 16, // 0x00000010
    Integer = 32, // 0x00000020
    DateTime = Integer | Keyword, // 0x00000030
    LongText = 64, // 0x00000040
    Files = LongText | Keyword, // 0x00000050
    Object = LongText | Integer, // 0x00000060
    TreeNode = 160, // 0x000000A0
    RelatedLinks = TreeNode | Keyword, // 0x000000B0
    Guid = 208, // 0x000000D0
    Bit = TreeNode | LongText, // 0x000000E0
    Double = Bit | Keyword, // 0x000000F0
    Person = Keyword | MaskStoreAsReference, // 0x00000018
    TreePath = 272, // 0x00000110
    TreeId = 288, // 0x00000120
    TreeNodeName = 528, // 0x00000210
    TreeNodeType = 784, // 0x00000310
    History = 320, // 0x00000140
    Html = 576, // 0x00000240
    PlainText = LongText, // 0x00000040
    MaskDataType = PlainText | RelatedLinks, // 0x000000F0
    MaskDataSubType = 3840, // 0x00000F00
    MaskDataTypeSubType = MaskDataSubType | MaskDataType, // 0x00000FF0
    MaskFieldType = MaskDataTypeSubType | MaskStoreAsReference, // 0x00000FF8
  }
}
