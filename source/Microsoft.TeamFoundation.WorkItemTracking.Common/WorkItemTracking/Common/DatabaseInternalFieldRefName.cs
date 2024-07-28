// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.DatabaseInternalFieldRefName
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct DatabaseInternalFieldRefName
  {
    public const string AttachedFiles = "System.AttachedFiles";
    public const string ChangedSet = "System.ChangedSet";
    public const string InAdminOnlyTreeFlag = "System.InAdminOnlyTreeFlag";
    public const string InDeletedTreeFlag = "System.InDeletedTreeFlag";
    public const string LinkedFiles = "System.LinkedFiles";
    public const string NodeType = "System.NodeType";
    public const string NotAField = "System.NotAField";
    public const string PersonId = "System.PersonId";
    public const string ProjectId = "System.ProjectId";
    public const string RelatedLinks = "System.RelatedLinks";
    public const string TFServer = "System.TFServer";
    public const string WorkItemForm = "System.WorkItemForm";
    public const string WorkItemFormId = "System.WorkItemFormId";
    public const string BisLinks = "System.BISLinks";
  }
}
