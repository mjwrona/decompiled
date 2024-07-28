// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.ISettingsValidatorDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  [EditorBrowsable(EditorBrowsableState.Advanced)]
  public interface ISettingsValidatorDataProvider
  {
    WorkItemTypeCategory GetCategory(string categoryReferenceName);

    bool CategoryExists(string categoryReferenceName);

    bool WorkItemTypeExists(string typeName);

    IEnumerable<string> GetTypesInCategory(string categoryReferenceName);

    string GetDefaultTypeInCategory(string categoryReferenceName);

    IEnumerable<string> GetTypeStates(string typeName);

    string GetTypeInitialState(string typeName);

    bool FieldExists(string workItemTypeName, string fieldReferenceName);

    InternalFieldType GetFieldType(string workItemTypeName, string fieldReferenceName);
  }
}
