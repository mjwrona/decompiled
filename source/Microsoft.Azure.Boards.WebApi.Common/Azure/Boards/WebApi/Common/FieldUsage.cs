// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WebApi.Common.FieldUsage
// Assembly: Microsoft.Azure.Boards.WebApi.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC99C479-6852-4E74-BCA4-2660760F9D83
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.WebApi.Common.dll

using System.Runtime.Serialization;

namespace Microsoft.Azure.Boards.WebApi.Common
{
  [DataContract]
  public class FieldUsage
  {
    [DataMember(Name = "WorkItemTypeId")]
    public string WorkItemTypeId { get; set; }

    [DataMember(Name = "IsInherited")]
    public bool IsInherited { get; set; }

    [DataMember(Name = "IsSystem")]
    public bool IsSystem { get; set; }

    [DataMember(Name = "IsBehaviorField")]
    public bool IsBehaviorField { get; set; }

    [DataMember(Name = "CanEditFieldProperties")]
    public bool CanEditFieldProperties { get; set; }

    [DataMember(Name = "Properties")]
    public UsageProperties Properties { get; set; }
  }
}
