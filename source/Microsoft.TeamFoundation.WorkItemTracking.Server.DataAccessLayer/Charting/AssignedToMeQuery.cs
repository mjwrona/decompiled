// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Charting.AssignedToMeQuery
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Charting
{
  internal class AssignedToMeQuery
  {
    public const string IdString = "a2108d31-086c-4fb0-afda-097e4cc46df4";
    public static readonly Guid Id = new Guid("a2108d31-086c-4fb0-afda-097e4cc46df4");
    public const string Wiql = "SELECT [System.Id], [System.WorkItemType], [System.Title], [System.State], [System.AreaPath], [System.IterationPath] FROM WorkItems WHERE [System.TeamProject] = @project AND [System.AssignedTo] = @me ORDER BY [System.ChangedDate] DESC";
    public const string RegistryPath = "/Users/{0}/WebAccess/Projects/{1}/Queries/a2108d31-086c-4fb0-afda-097e4cc46df4/Query";
  }
}
