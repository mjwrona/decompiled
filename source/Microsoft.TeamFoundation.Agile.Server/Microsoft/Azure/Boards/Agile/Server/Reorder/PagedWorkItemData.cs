// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.Agile.Server.Reorder.PagedWorkItemData
// Assembly: Microsoft.TeamFoundation.Agile.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4912F51-3FCA-4D2B-A7B5-CF15E2F3B46B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Server.dll

namespace Microsoft.Azure.Boards.Agile.Server.Reorder
{
  public class PagedWorkItemData
  {
    public PagedWorkItemData(int id, string type, string state, string iterationPath)
    {
      this.Id = id;
      this.Type = type;
      this.State = state;
      this.IterationPath = iterationPath;
    }

    public int Id { get; }

    public string Type { get; }

    public string State { get; }

    public string IterationPath { get; }
  }
}
