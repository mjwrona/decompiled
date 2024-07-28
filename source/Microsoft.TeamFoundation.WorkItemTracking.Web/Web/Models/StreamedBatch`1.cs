// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Models.StreamedBatch`1
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Framework.Server.WebApis;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Models
{
  [DataContract]
  public class StreamedBatch<T>
  {
    [DataMember(Order = 0)]
    [JsonConverter(typeof (IEnumerableStreamingJsonConverter))]
    public virtual IEnumerable<T> Values { get; set; }

    [DataMember(Order = 1)]
    public virtual string NextLink { get; set; }

    [DataMember(Order = 2)]
    public virtual string ContinuationToken { get; set; }

    [DataMember(Order = 3)]
    public virtual bool IsLastBatch { get; set; }
  }
}
