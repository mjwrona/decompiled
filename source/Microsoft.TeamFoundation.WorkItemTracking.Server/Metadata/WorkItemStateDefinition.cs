// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemStateDefinition
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  public class WorkItemStateDefinition : IComparable<WorkItemStateDefinition>
  {
    public Guid Id { get; protected set; }

    public Guid WorkItemTypeId { get; protected set; }

    public string WorkItemTypeReferenceName { get; protected set; }

    public Guid ProcessId { get; protected set; }

    public string Name { get; protected set; }

    public int Order { get; protected set; }

    public string Color { get; protected set; }

    public WorkItemStateCategory StateCategory { get; protected set; }

    public bool Hidden { get; protected set; }

    public DateTime AuthorizedDate { get; protected set; }

    public CustomizationType Customization { get; internal set; }

    public int CompareTo(WorkItemStateDefinition other)
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

    public static WorkItemStateDefinition Create(
      WorkItemStateDefinitionRecord record,
      string witRefName,
      IEnumerable<WorkItemOobState> oobStates)
    {
      WorkItemStateDefinition state = new WorkItemStateDefinition()
      {
        WorkItemTypeReferenceName = witRefName,
        ProcessId = record.ProcessId,
        Id = record.StateId,
        Name = record.Name,
        Order = record.StateOrder,
        Color = record.Color,
        WorkItemTypeId = record.WorkItemTypeId,
        Hidden = record.Hidden,
        AuthorizedDate = record.AuthorizedDate
      };
      WorkItemStateCategory result;
      if (Enum.TryParse<WorkItemStateCategory>(record.StateCategory.ToString(), out result))
        state.StateCategory = result;
      WorkItemStateDefinition.SetStatesCustomizationProperty(state, oobStates);
      return state;
    }

    private static void SetStatesCustomizationProperty(
      WorkItemStateDefinition state,
      IEnumerable<WorkItemOobState> oobStates)
    {
      bool flag = oobStates.Any<WorkItemOobState>((Func<WorkItemOobState, bool>) (s => s.Id == state.Id));
      if (state.Hidden & flag)
        state.Customization = CustomizationType.Inherited;
      else if (flag)
        state.Customization = CustomizationType.System;
      else
        state.Customization = CustomizationType.Custom;
    }

    public WorkItemStateDefinition Clone(int order)
    {
      WorkItemStateDefinition itemStateDefinition = (WorkItemStateDefinition) this.MemberwiseClone();
      itemStateDefinition.Order = order;
      return itemStateDefinition;
    }
  }
}
