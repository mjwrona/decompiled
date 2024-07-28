// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WebApi.Common.Field
// Assembly: Microsoft.Azure.Boards.WebApi.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC99C479-6852-4E74-BCA4-2660760F9D83
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.WebApi.Common.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Boards.WebApi.Common
{
  [DataContract]
  public class Field
  {
    [DataMember(Name = "Id")]
    public string Id { get; set; }

    [DataMember(Name = "Name")]
    public string Name { get; set; }

    [DataMember(Name = "Description")]
    public string Description { get; set; }

    [DataMember(Name = "Type")]
    public string Type { get; set; }

    [DataMember(Name = "Properties")]
    public FieldProperties Properties { get; set; }

    [DataMember(Name = "Usages")]
    public List<FieldUsage> Usages { get; set; }

    [DataMember(Name = "PickListId", IsRequired = false)]
    public Guid? PickListId { get; set; }
  }
}
