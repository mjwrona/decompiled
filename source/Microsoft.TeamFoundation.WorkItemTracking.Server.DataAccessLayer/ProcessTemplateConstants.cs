// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessTemplateConstants
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal static class ProcessTemplateConstants
  {
    public const string ProcessConfiguration = "PROCESSCONFIGURATION";
    public const string LinkTypes = "LINKTYPES";
    public const string LinkType = "LINKTYPE";
    public const string WorkItems = "WORKITEMS";
    public const string WorkItemTypes = "WORKITEMTYPES";
    public const string WorkItemType = "WORKITEMTYPE";
    public const string Categories = "CATEGORIES";
    public const string Query = "Query";
    public const string QueryFolder = "QueryFolder";
    public const string Queries = "QUERIES";
    public const string PermissionNode = "Permission";
    public const string PermissionIdentityAttribute = "identity";
    public const string PermissionAllowAttribute = "allow";
    public const string PermissionDenyAttribute = "deny";
    public static char[] PermissionSplitChars = new char[5]
    {
      ',',
      ' ',
      '\t',
      '\r',
      '\n'
    };
    public const string CommonConfiguration = "CommonConfiguration";
    public const string AgileConfiguration = "AgileConfiguration";
    public const string ProjectConfiguration = "ProjectConfiguration";
    public const string QueryNameAttribute = "name";
    public const string FileNameAttribute = "fileName";
    public const string WorkItemFieldReferenceNameAttribute = "refname";
    public const string WorkItemFieldValueAttribute = "value";
    public const string WorkItemTypeAttribute = "type";
  }
}
