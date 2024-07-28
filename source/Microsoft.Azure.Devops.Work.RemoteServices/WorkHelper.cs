// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Work.RemoteServices.WorkHelper
// Assembly: Microsoft.Azure.Devops.Work.RemoteServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C97796CA-4166-42B2-B96F-9A166B07FF72
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Devops.Work.RemoteServices.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System.Collections.Generic;

namespace Microsoft.Azure.Devops.Work.RemoteServices
{
  public static class WorkHelper
  {
    public static JsonPatchDocument ConvertToJsonPatchDocument(
      IVssRequestContext context,
      IDictionary<string, object> fieldsToUpdate,
      IList<WorkItemRelation> relations)
    {
      JsonPatchDocument jsonPatchDocument = new JsonPatchDocument();
      if (fieldsToUpdate != null)
      {
        foreach (KeyValuePair<string, object> keyValuePair in (IEnumerable<KeyValuePair<string, object>>) fieldsToUpdate)
          jsonPatchDocument.Add(new JsonPatchOperation()
          {
            Operation = Operation.Add,
            Path = string.Format("/fields/{0}", (object) keyValuePair.Key),
            Value = keyValuePair.Value
          });
      }
      if (relations != null)
      {
        foreach (WorkItemRelation relation in (IEnumerable<WorkItemRelation>) relations)
          jsonPatchDocument.Add(new JsonPatchOperation()
          {
            Operation = Operation.Add,
            Path = "/relations/-",
            Value = (object) relation
          });
      }
      return jsonPatchDocument;
    }
  }
}
