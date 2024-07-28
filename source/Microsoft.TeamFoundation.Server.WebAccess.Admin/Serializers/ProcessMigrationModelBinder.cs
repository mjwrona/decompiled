// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.Serializers.ProcessMigrationModelBinder
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.Serializers
{
  public class ProcessMigrationModelBinder : JsonModelBinder
  {
    public override JsonConverter[] GetConverters() => (JsonConverter[]) new JsonConverterWithCallbacks<ProcessMigrationModel>[1]
    {
      new JsonConverterWithCallbacks<ProcessMigrationModel>(ProcessMigrationModelBinder.\u003C\u003EO.\u003C0\u003E__DeserializeJsonProcessMigration ?? (ProcessMigrationModelBinder.\u003C\u003EO.\u003C0\u003E__DeserializeJsonProcessMigration = new JsonDeserializationCallback<ProcessMigrationModel>(ProcessMigrationModelBinder.DeserializeJsonProcessMigration)))
    };

    public static ProcessMigrationModel DeserializeJsonProcessMigration(
      IDictionary<string, object> dictionary)
    {
      Guid result1 = Guid.Empty;
      Guid result2 = Guid.Empty;
      if (dictionary == null)
        return (ProcessMigrationModel) null;
      object obj1;
      object obj2;
      if (!dictionary.TryGetValue("projectId", out obj1) || !Guid.TryParse(obj1.ToString(), out result1) || !dictionary.TryGetValue("newProcessTypeId", out obj2) || !Guid.TryParse(obj2.ToString(), out result2))
        return (ProcessMigrationModel) null;
      return new ProcessMigrationModel()
      {
        ProjectId = result1,
        NewProcessTypeId = result2
      };
    }

    public override JavaScriptConverter[] GetJsConverters() => (JavaScriptConverter[]) new JsonConverterJsSerializerWithCallbacks<ProcessMigrationModel>[1]
    {
      new JsonConverterJsSerializerWithCallbacks<ProcessMigrationModel>(ProcessMigrationModelBinder.\u003C\u003EO.\u003C1\u003E__DeserializeJsonProcessMigrationJsSerializer ?? (ProcessMigrationModelBinder.\u003C\u003EO.\u003C1\u003E__DeserializeJsonProcessMigrationJsSerializer = new JsonDeserializationJSCallback<ProcessMigrationModel>(ProcessMigrationModelBinder.DeserializeJsonProcessMigrationJsSerializer)))
    };

    public static ProcessMigrationModel DeserializeJsonProcessMigrationJsSerializer(
      IDictionary<string, object> dictionary,
      JavaScriptSerializer serializer)
    {
      Guid result1 = Guid.Empty;
      Guid result2 = Guid.Empty;
      if (dictionary == null)
        return (ProcessMigrationModel) null;
      if (dictionary.ContainsKey("projectId"))
        Guid.TryParse(dictionary["projectId"].ToString(), out result1);
      if (dictionary.ContainsKey("newProcessTypeId"))
        Guid.TryParse(dictionary["newProcessTypeId"].ToString(), out result2);
      if (!(result1 != Guid.Empty) || !(result2 != Guid.Empty))
        return (ProcessMigrationModel) null;
      return new ProcessMigrationModel()
      {
        ProjectId = result1,
        NewProcessTypeId = result2
      };
    }
  }
}
