// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.ProcessBehavior
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 141BFF87-1CDC-4DC5-9A7C-F7D6EA031F34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models
{
  [DataContract]
  public class ProcessBehavior
  {
    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string ReferenceName { get; set; }

    [DataMember]
    public string Color { get; set; }

    [DataMember]
    public int Rank { get; set; }

    [DataMember]
    public string Description { get; set; }

    [DataMember]
    public CustomizationType Customization { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<ProcessBehaviorField> Fields { get; set; }

    [DataMember]
    public ProcessBehaviorReference Inherits { get; set; }

    [DataMember]
    public string Url { get; set; }
  }
}
