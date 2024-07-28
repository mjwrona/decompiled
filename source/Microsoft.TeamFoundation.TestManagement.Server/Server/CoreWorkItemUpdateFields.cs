// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.CoreWorkItemUpdateFields
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class CoreWorkItemUpdateFields
  {
    public static string[] CoreWorkItemFields = new string[5]
    {
      "System.Title",
      "System.AreaPath",
      "System.IterationPath",
      "System.State",
      "System.WorkItemType"
    };

    public string AreaPath { get; set; }

    public string IterationPath { get; set; }

    public string State { get; set; }

    public string Title { get; set; }

    public string WokItemType { get; set; }
  }
}
