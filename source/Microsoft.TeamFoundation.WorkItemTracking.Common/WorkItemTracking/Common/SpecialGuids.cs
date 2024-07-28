// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.SpecialGuids
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct SpecialGuids
  {
    public static readonly string ProcessChangesLock = "6E1B14E3-FCAA-46AD-AF37-5E2DE3C1588D";
    internal static readonly Guid ProcessIdentitiesLock = new Guid("EEF73B91-3D98-4950-AE5F-2B297AE4F656");
    internal static readonly Guid GlobalListsSetConstant = new Guid("299f07ef-6201-41b3-90fc-03eeb3977587");
    public static readonly Guid ServerDefaultGuidRuleConstant = new Guid("96EBBCBD-535E-4AE4-AD11-75C08AD55A0D");
    public static readonly Guid QueryItemChanged = new Guid("8E616C67-502E-4BB5-8037-B6D49CAA6E73");
    public static readonly Guid LinkTypeChanged = new Guid("EFD81C28-8A72-4DD7-94FF-16AA16543D81");
    public static readonly Guid FieldChanged = new Guid("92778466-E528-4569-8736-A7CBD9983DB7");
    public static readonly Guid TreeChanged = new Guid("34B8C348-F677-4A6B-AAE3-2C4EDFC9E959");
    public static readonly Guid WorkItemTypeChanged = new Guid("F673515C-E7EF-4E75-9F90-9D392A1283A7");
    public static readonly Guid WorkItemTypeExtensionChanged = new Guid("B68CB193-D28A-4E0A-BF66-B469C5A9F5CF");
    public static readonly Guid WorkItemTypeExtensionDeleted = new Guid("7DFEEA68-54B9-4949-8079-85BBCC422736");
    public static readonly Guid WorkItemTypeExtensionReconciliationStatusChanged = new Guid("621A4440-52DA-414E-AAEC-CCAAE77DED7D");
    public static readonly Guid WorkItemTypeletChanged = new Guid("4999F82D-F8B0-4951-90ED-1FD68C8829D5");
    public static readonly Guid WorkItemTypeletDeleted = new Guid("461F29EE-ECE9-480F-B657-64FB2B7EA315");
    public static readonly Guid WorkItemStateDefinitionModified = new Guid("9B4EF705-8408-4B4D-A575-BC0FE81304A6");
    public static readonly Guid WorkItemTypeBehaviorReferenceModified = new Guid("9C2ACAD5-16AF-457D-8E68-E12B0437871F");
    public static readonly Guid WorkItemTypeletFieldPropertiesChanged = new Guid("78A7B60C-AA2F-4F58-8BFB-3833A8B4F09E");
    public static readonly Guid WorkItemArtifactKind = new Guid("E7626DBD-6075-416C-A31E-DFD48FE3CFDE");
  }
}
