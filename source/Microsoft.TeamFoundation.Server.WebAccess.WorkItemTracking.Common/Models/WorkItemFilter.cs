// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.WorkItemFilter
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models
{
  [DataContract]
  public class WorkItemFilter : ProjectScopedSecuredSetting
  {
    public WorkItemFilter()
      : base(WorkItemsHubSecurityConstants.NamespaceId, 1, "WorkItemsHub", "UserSettings")
    {
    }

    [DataMember(EmitDefaultValue = false)]
    public List<string> Values { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Dictionary<string, object> Options { get; set; }
  }
}
