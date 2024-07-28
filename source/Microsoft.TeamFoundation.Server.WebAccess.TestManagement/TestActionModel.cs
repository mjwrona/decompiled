// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TestActionModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  [DataContract]
  internal abstract class TestActionModel
  {
    [DataMember(Name = "id")]
    public int Id { get; set; }

    [DataMember(Name = "actionType")]
    public string ActionType { get; set; }

    [DataMember(Name = "action")]
    public string Action { get; set; }

    [DataMember(Name = "index")]
    public int Index { get; set; }

    public abstract bool ProcessTestAttachments(List<WorkItemResourceLinkInfo> links);
  }
}
