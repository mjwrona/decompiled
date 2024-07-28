// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AuditLog.ProcessAuditConstants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server.AuditLog
{
  public static class ProcessAuditConstants
  {
    public const string PicklistValue = "PicklistValue";
    private static readonly string ProcessArea = "Process.";
    public static readonly string BehaviorCreate = "Behavior.Create";
    public static readonly string BehaviorEdit = "Behavior.Edit";
    public static readonly string BehaviorDelete = "Behavior.Delete";
    public static readonly string WITBehaviorAdd = "Behavior.Add";
    public static readonly string WITBehaviorUpdate = "Behavior.Update";
    public static readonly string WITBehaviorRemove = "Behavior.Remove";
    public static readonly string FieldCreate = "Field.Create";
    public static readonly string FieldEdit = "Field.Edit";
    public static readonly string FieldDelete = "Field.Delete";
    public static readonly string WITFieldAdd = "Field.Add";
    public static readonly string WITFieldUpdate = "Field.Update";
    public static readonly string WITFieldRemove = "Field.Remove";
    public static readonly string ControlCreate = "Control.Create";
    public static readonly string ControlCreateWithoutLabel = "Control.CreateWithoutLabel";
    public static readonly string ControlUpdate = "Control.Update";
    public static readonly string ControlUpdateWithoutLabel = "Control.UpdateWithoutLabel";
    public static readonly string ControlDelete = "Control.Delete";
    public static readonly string GroupAdd = "Group.Add";
    public static readonly string GroupUpdate = "Group.Update";
    public static readonly string ListCreate = "List.Create";
    public static readonly string ListUpdate = "List.Update";
    public static readonly string ListAddValue = "List.ListAddValue";
    public static readonly string ListRemoveValue = "List.ListRemoveValue";
    public static readonly string ListDelete = "List.Delete";
    public static readonly string PageAdd = "Page.Add";
    public static readonly string PageUpdate = "Page.Update";
    public static readonly string PageDelete = "Page.Delete";
    public static readonly string ProcessImported = "Process.Import";
    public static readonly string ProcessCloneXmlToInherited = "Process.CloneXmlToInherited";
    public static readonly string ProcessMigrateXmlToInherited = "Process.MigrateXmlToInherited";
    public static readonly string ProcessCreate = "Process.Create";
    public static readonly string ProcessEdit = "Process.Edit";
    public static readonly string ProcessEditWithoutNewInformation = "Process.EditWithoutNewInformation";
    public static readonly string ProcessDelete = "Process.Delete";
    public static readonly string RuleAdd = "Rule.Add";
    public static readonly string RuleUpdate = "Rule.Update";
    public static readonly string RuleDelete = "Rule.Delete";
    public static readonly string StateCreate = "State.Create";
    public static readonly string StateUpdate = "State.Update";
    public static readonly string StateDelete = "State.Delete";
    public static readonly string SystemControlUpdate = "SystemControl.Update";
    public static readonly string SystemControlDelete = "SystemControl.Delete";
    public static readonly string WorkItemTypeCreate = "WorkItemType.Create";
    public static readonly string WorkItemTypeUpdate = "WorkItemType.Update";
    public static readonly string WorkItemTypeDelete = "WorkItemType.Delete";
    public static readonly string ProcessBehaviorCreate = ProcessAuditConstants.ProcessArea + ProcessAuditConstants.BehaviorCreate;
    public static readonly string ProcessBehaviorEdit = ProcessAuditConstants.ProcessArea + ProcessAuditConstants.BehaviorEdit;
    public static readonly string ProcessBehaviorDelete = ProcessAuditConstants.ProcessArea + ProcessAuditConstants.BehaviorDelete;
    public static readonly string ProcessWITBehaviorAdd = ProcessAuditConstants.ProcessArea + ProcessAuditConstants.WITBehaviorAdd;
    public static readonly string ProcessWITBehaviorUpdate = ProcessAuditConstants.ProcessArea + ProcessAuditConstants.WITBehaviorUpdate;
    public static readonly string ProcessWITBehaviorRemove = ProcessAuditConstants.ProcessArea + ProcessAuditConstants.WITBehaviorRemove;
    public static readonly string ProcessFieldCreate = ProcessAuditConstants.ProcessArea + ProcessAuditConstants.FieldCreate;
    public static readonly string ProcessFieldEdit = ProcessAuditConstants.ProcessArea + ProcessAuditConstants.FieldEdit;
    public static readonly string ProcessFieldDelete = ProcessAuditConstants.ProcessArea + ProcessAuditConstants.FieldDelete;
    public static readonly string ProcessWITFieldAdd = ProcessAuditConstants.ProcessArea + ProcessAuditConstants.WITFieldAdd;
    public static readonly string ProcessWITFieldUpdate = ProcessAuditConstants.ProcessArea + ProcessAuditConstants.WITFieldUpdate;
    public static readonly string ProcessWITFieldRemove = ProcessAuditConstants.ProcessArea + ProcessAuditConstants.WITFieldRemove;
    public static readonly string ProcessControlCreate = ProcessAuditConstants.ProcessArea + ProcessAuditConstants.ControlCreate;
    public static readonly string ProcessControlCreateWithoutLabel = ProcessAuditConstants.ProcessArea + ProcessAuditConstants.ControlCreateWithoutLabel;
    public static readonly string ProcessControlUpdate = ProcessAuditConstants.ProcessArea + ProcessAuditConstants.ControlUpdate;
    public static readonly string ProcessControlUpdateWithoutLabel = ProcessAuditConstants.ProcessArea + ProcessAuditConstants.ControlUpdateWithoutLabel;
    public static readonly string ProcessControlDelete = ProcessAuditConstants.ProcessArea + ProcessAuditConstants.ControlDelete;
    public static readonly string ProcessGroupAdd = ProcessAuditConstants.ProcessArea + ProcessAuditConstants.GroupAdd;
    public static readonly string ProcessGroupUpdate = ProcessAuditConstants.ProcessArea + ProcessAuditConstants.GroupUpdate;
    public static readonly string ProcessListCreate = ProcessAuditConstants.ProcessArea + ProcessAuditConstants.ListCreate;
    public static readonly string ProcessListUpdate = ProcessAuditConstants.ProcessArea + ProcessAuditConstants.ListUpdate;
    public static readonly string ProcessListAddValue = ProcessAuditConstants.ProcessArea + ProcessAuditConstants.ListAddValue;
    public static readonly string ProcessListRemoveValue = ProcessAuditConstants.ProcessArea + ProcessAuditConstants.ListRemoveValue;
    public static readonly string ProcessListDelete = ProcessAuditConstants.ProcessArea + ProcessAuditConstants.ListDelete;
    public static readonly string ProcessPageAdd = ProcessAuditConstants.ProcessArea + ProcessAuditConstants.PageAdd;
    public static readonly string ProcessPageUpdate = ProcessAuditConstants.ProcessArea + ProcessAuditConstants.PageUpdate;
    public static readonly string ProcessPageDelete = ProcessAuditConstants.ProcessArea + ProcessAuditConstants.PageDelete;
    public static readonly string ProcessProcessImport = ProcessAuditConstants.ProcessArea + ProcessAuditConstants.ProcessImported;
    public static readonly string ProcessProcessCloneXmlToInherited = ProcessAuditConstants.ProcessArea + ProcessAuditConstants.ProcessCloneXmlToInherited;
    public static readonly string ProcessProcessMigrateXmlToInherited = ProcessAuditConstants.ProcessArea + ProcessAuditConstants.ProcessMigrateXmlToInherited;
    public static readonly string ProcessProcessCreate = ProcessAuditConstants.ProcessArea + ProcessAuditConstants.ProcessCreate;
    public static readonly string ProcessProcessEdit = ProcessAuditConstants.ProcessArea + ProcessAuditConstants.ProcessEdit;
    public static readonly string ProcessProcessEditWithoutNewInformation = ProcessAuditConstants.ProcessArea + ProcessAuditConstants.ProcessEditWithoutNewInformation;
    public static readonly string ProcessProcessDelete = ProcessAuditConstants.ProcessArea + ProcessAuditConstants.ProcessDelete;
    public static readonly string ProcessRuleAdd = ProcessAuditConstants.ProcessArea + ProcessAuditConstants.RuleAdd;
    public static readonly string ProcessRuleUpdate = ProcessAuditConstants.ProcessArea + ProcessAuditConstants.RuleUpdate;
    public static readonly string ProcessRuleDelete = ProcessAuditConstants.ProcessArea + ProcessAuditConstants.RuleDelete;
    public static readonly string ProcessStateCreate = ProcessAuditConstants.ProcessArea + ProcessAuditConstants.StateCreate;
    public static readonly string ProcessStateUpdate = ProcessAuditConstants.ProcessArea + ProcessAuditConstants.StateUpdate;
    public static readonly string ProcessStateDelete = ProcessAuditConstants.ProcessArea + ProcessAuditConstants.StateDelete;
    public static readonly string ProcessSystemControlUpdate = ProcessAuditConstants.ProcessArea + ProcessAuditConstants.SystemControlUpdate;
    public static readonly string ProcessSystemControlDelete = ProcessAuditConstants.ProcessArea + ProcessAuditConstants.SystemControlDelete;
    public static readonly string ProcessWorkItemTypeCreate = ProcessAuditConstants.ProcessArea + ProcessAuditConstants.WorkItemTypeCreate;
    public static readonly string ProcessWorkItemTypeUpdate = ProcessAuditConstants.ProcessArea + ProcessAuditConstants.WorkItemTypeUpdate;
    public static readonly string ProcessWorkItemTypeDelete = ProcessAuditConstants.ProcessArea + ProcessAuditConstants.WorkItemTypeDelete;
  }
}
