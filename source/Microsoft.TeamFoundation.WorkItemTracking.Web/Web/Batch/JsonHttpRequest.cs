// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Batch.JsonHttpRequest
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.WorkItemTracking.Web.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Batch
{
  public class JsonHttpRequest
  {
    [HttpMethodRequired]
    [DataMember]
    public string Method { get; set; }

    [Required(AllowEmptyStrings = false)]
    [DataMember]
    public string Uri { get; set; }

    [DataMember]
    public Dictionary<string, string> Headers { get; set; }

    [DataMember]
    public object Body { get; set; }
  }
}
