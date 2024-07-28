// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Process.ProcessFieldUsageInfo
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 141BFF87-1CDC-4DC5-9A7C-F7D6EA031F34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Process
{
  [DataContract]
  public class ProcessFieldUsageInfo
  {
    [DataMember(Name = "Fields")]
    public IEnumerable<Field> Fields { get; set; }

    [DataMember(Name = "WorkItemTypes")]
    public IEnumerable<WorkItemType> WorkItemTypes { get; set; }
  }
}
