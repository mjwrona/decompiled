// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Work.WebApi.Contracts.Taskboard.UpdateTaskboardColumn
// Assembly: Microsoft.TeamFoundation.Work.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0C4CCFA0-0616-4E48-A4F0-952E1CB10B12
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Work.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Work.WebApi.Contracts.Taskboard
{
  [DataContract]
  public class UpdateTaskboardColumn : ITaskboardColumn
  {
    [DataMember(Name = "Mappings")]
    private IReadOnlyCollection<TaskboardColumnMapping> m_Mappings;

    [DataMember(Name = "Id")]
    public Guid? Id { get; set; }

    [DataMember(Name = "Name")]
    public string Name { get; set; }

    [DataMember(Name = "Order")]
    public int Order { get; set; }

    public IReadOnlyCollection<ITaskboardColumnMapping> Mappings
    {
      get => (IReadOnlyCollection<ITaskboardColumnMapping>) this.m_Mappings;
      set => this.m_Mappings = value as IReadOnlyCollection<TaskboardColumnMapping>;
    }
  }
}
