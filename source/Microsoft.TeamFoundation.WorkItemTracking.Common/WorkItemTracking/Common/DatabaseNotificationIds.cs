// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.DatabaseNotificationIds
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct DatabaseNotificationIds
  {
    public const string LinkTypeChanged = "EFD81C28-8A72-4DD7-94FF-16AA16543D81";
    public const string QueryItemChanged = "8E616C67-502E-4BB5-8037-B6D49CAA6E73";
    public const string FieldChanged = "92778466-E528-4569-8736-A7CBD9983DB7";
    public const string TreeChanged = "34B8C348-F677-4A6B-AAE3-2C4EDFC9E959";
    public const string WorkItemTypeChanged = "F673515C-E7EF-4E75-9F90-9D392A1283A7";
    public const string WorkItemTypeExtensionChanged = "B68CB193-D28A-4E0A-BF66-B469C5A9F5CF";
    public const string WorkItemTypeExtensionReconciliationStatusChanged = "621A4440-52DA-414E-AAEC-CCAAE77DED7D";
    public const string PicklistChanged = "51387D6F-334E-4DE9-A686-3BEC22B03A76";
    public const string PicklistDeleted = "E90595D4-AF22-4228-A9F6-09EA5AAB9F08";
    public const string WorkItemTypeletFieldPropertiesChanged = "78A7B60C-AA2F-4F58-8BFB-3833A8B4F09E";
  }
}
