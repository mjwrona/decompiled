// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemStateDeclaration
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  public class WorkItemStateDeclaration : IComparable<WorkItemStateDeclaration>
  {
    public Guid? Id { get; set; }

    public string WorkItemTypeReferenceName { get; set; }

    public string Name { get; set; }

    public string Color { get; set; }

    public WorkItemStateCategory StateCategory { get; set; }

    public int Order { get; set; }

    public bool Hidden { get; set; }

    public int CompareTo(WorkItemStateDeclaration other)
    {
      int num1 = (int) (this.StateCategory + this.Order);
      int num2 = (int) (other.StateCategory + other.Order);
      if (num1 == num2)
      {
        if (this.Name != null)
          return this.Name.CompareTo(other.Name);
        return other.Name != null ? -1 : 0;
      }
      return num1 >= num2 ? 1 : -1;
    }

    public static WorkItemStateDeclaration Create(WorkItemStateDefinition def) => new WorkItemStateDeclaration()
    {
      Id = new Guid?(def.Id),
      WorkItemTypeReferenceName = def.WorkItemTypeReferenceName,
      Name = def.Name,
      Order = def.Order,
      Color = def.Color,
      StateCategory = def.StateCategory,
      Hidden = def.Hidden
    };
  }
}
