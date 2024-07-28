// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.LinkQueryMode
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4A32169-9B8B-4726-A9F6-41569B7C3273
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage.dll

namespace Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql
{
  internal enum LinkQueryMode
  {
    Unknown,
    WorkItems,
    LinksMustContain,
    LinksMayContain,
    LinksDoesNotContain,
    LinksRecursive,
    LinksRecursiveReturnMatchingChildren,
  }
}
