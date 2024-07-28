// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.WorkItemTypeFieldModel
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7BABD213-FC9A-4DAB-8690-D2FF2DA1955C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models
{
  [DataContract]
  public class WorkItemTypeFieldModel
  {
    [DataMember]
    public string ReferenceName { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public FieldType Type { get; set; }

    [DataMember]
    public PickListMetadataModel PickList { get; set; }

    [DataMember]
    public bool ReadOnly { get; set; }

    [DataMember]
    public bool Required { get; set; }

    [DataMember]
    public string DefaultValue { get; set; }

    [DataMember]
    public string Url { get; set; }

    [DataMember(IsRequired = true)]
    public bool? AllowGroups { get; set; }
  }
}
