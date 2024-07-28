// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.IFieldTypeDictionary
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  public interface IFieldTypeDictionary
  {
    bool TryGetFieldByNameOrId(string nameOrId, out FieldEntry field);

    bool TryGetField(string name, out FieldEntry field);

    bool TryGetField(int id, out FieldEntry field);

    FieldEntry GetFieldByNameOrId(string nameOrId);

    FieldEntry GetField(string name);

    FieldEntry GetField(int id);

    IReadOnlyCollection<FieldEntry> GetAllFields();

    IReadOnlyCollection<FieldEntry> GetCoreFields();

    ISet<int> GetHistoryDisabledFieldIds();
  }
}
