// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Diagnostics.ActivityLogQueryResultModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Diagnostics, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6A4F2FF9-BE93-434B-9864-FE0D09D21D75
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Diagnostics.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Diagnostics
{
  [DataContract]
  public class ActivityLogQueryResultModel
  {
    public ActivityLogQueryResultModel(
      IEnumerable<int> targetIds,
      IEnumerable<ActivityLogEntry> payload)
    {
      this.TargetIds = ConvertUtility.ToCompactJsArray(targetIds.ToArray<int>());
      this.Payload = payload;
    }

    public JsObject ToJson()
    {
      JsObject json = new JsObject();
      json["targetIds"] = (object) this.TargetIds;
      json["payload"] = this.Payload != null ? (object) this.Payload.Select<ActivityLogEntry, JsObject>((Func<ActivityLogEntry, JsObject>) (entry => entry.ToJson())) : (object) (IEnumerable<JsObject>) null;
      return json;
    }

    public IEnumerable<object> TargetIds { get; private set; }

    public IEnumerable<ActivityLogEntry> Payload { get; private set; }
  }
}
