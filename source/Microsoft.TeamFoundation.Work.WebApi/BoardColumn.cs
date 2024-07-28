// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Work.WebApi.BoardColumn
// Assembly: Microsoft.TeamFoundation.Work.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0C4CCFA0-0616-4E48-A4F0-952E1CB10B12
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Work.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Work.WebApi
{
  [DataContract]
  public class BoardColumn
  {
    [DataMember]
    public Guid? Id { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? ItemLimit { get; set; }

    [DataMember]
    public Dictionary<string, string> StateMappings { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool? IsSplit { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Description { get; set; }

    [DataMember]
    public BoardColumnType ColumnType { get; set; }
  }
}
