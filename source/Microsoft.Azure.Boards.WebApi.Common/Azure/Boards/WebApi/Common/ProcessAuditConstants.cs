// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WebApi.Common.ProcessAuditConstants
// Assembly: Microsoft.Azure.Boards.WebApi.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC99C479-6852-4E74-BCA4-2660760F9D83
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.WebApi.Common.dll

namespace Microsoft.Azure.Boards.WebApi.Common
{
  public class ProcessAuditConstants
  {
    public static string GetActionId(string entity, string action) => "Process." + entity + "." + action;

    public static string GetProjectActionId(string entity, string action) => "Project." + entity + "." + action;

    public class Area
    {
      public const string Process = "Process";
      public const string Project = "Project";
    }

    public class Entity
    {
      public const string IterationPath = "IterationPath";
      public const string AreaPath = "AreaPath";
      public const string Behavior = "Behavior";
      public const string Control = "Control";
      public const string Field = "Field";
      public const string Group = "Group";
      public const string Layout = "Layout";
      public const string List = "List";
      public const string Page = "Page";
      public const string Process = "Process";
      public const string Rule = "Rule";
      public const string State = "State";
      public const string SystemControl = "SystemControl";
      public const string WorkItemType = "WorkItemType";
      public const string WorkItemTypeBehavior = "WorkItemTypeBehavior";
    }

    public class Action
    {
      public const string Add = "Add";
      public const string CloneXmlToInherited = "CloneXmlToInherited";
      public const string MigrateXmlToInherited = "MigrateXmlToInherited";
      public const string Create = "Create";
      public const string CreateWithoutLabel = "CreateWithoutLabel";
      public const string Delete = "Delete";
      public const string Edit = "Edit";
      public const string EditWithoutNewInformation = "EditWithoutNewInformation";
      public const string Hide = "Hide";
      public const string Import = "Import";
      public const string Update = "Update";
      public const string UpdateWithoutLabel = "UpdateWithoutLabel";
      public const string Remove = "Remove";
    }
  }
}
