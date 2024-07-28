// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.Link
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model
{
  [DataContract]
  public abstract class Link
  {
    [DataMember]
    public WorkItemReference Source { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Comment { get; set; }
  }
}
