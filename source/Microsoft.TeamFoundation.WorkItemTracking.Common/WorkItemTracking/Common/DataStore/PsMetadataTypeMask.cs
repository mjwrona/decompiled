// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.DataStore.PsMetadataTypeMask
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.DataStore
{
  [Flags]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public enum PsMetadataTypeMask
  {
    MetadataTypeHierarchy = 1,
    MetadataTypeFields = 2,
    MetadataTypeConstants = 4,
    MetadataTypeRules = 8,
    MetadataTypeConstantSets = 16, // 0x00000010
    MetadataTypeHierarchyProperties = 32, // 0x00000020
    MetadataTypeFieldUsages = 64, // 0x00000040
    MetadataTypeWorkItemTypes = 128, // 0x00000080
    MetadataTypeWorkItemTypeFieldUsages = 256, // 0x00000100
    MetadataTypeStateTransition = 512, // 0x00000200
    MetadataTypeLinkTypes = 1024, // 0x00000400
    MetadataTypeCategories = 2048, // 0x00000800
    MetadataTypeCategoryMembers = 4096, // 0x00001000
    MetadataAllTypes = MetadataTypeCategoryMembers | MetadataTypeCategories | MetadataTypeLinkTypes | MetadataTypeStateTransition | MetadataTypeWorkItemTypeFieldUsages | MetadataTypeWorkItemTypes | MetadataTypeFieldUsages | MetadataTypeHierarchyProperties | MetadataTypeConstantSets | MetadataTypeRules | MetadataTypeConstants | MetadataTypeFields | MetadataTypeHierarchy, // 0x00001FFF
  }
}
