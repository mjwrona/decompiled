// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.FormLayout
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7BABD213-FC9A-4DAB-8690-D2FF2DA1955C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models
{
  [DataContract]
  public class FormLayout
  {
    [DataMember(EmitDefaultValue = false)]
    public IList<Extension> Extensions { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IList<Page> Pages { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public List<Control> SystemControls { get; set; }
  }
}
