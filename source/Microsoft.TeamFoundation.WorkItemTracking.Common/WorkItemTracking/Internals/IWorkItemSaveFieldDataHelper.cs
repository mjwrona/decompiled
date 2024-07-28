// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Internals.IWorkItemSaveFieldDataHelper
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.WorkItemTracking.Internals
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public interface IWorkItemSaveFieldDataHelper
  {
    string UserDisplayName { get; }

    bool IsDirty { get; }

    bool HasField(int fieldId);

    Dictionary<int, object> FieldUpdates { get; }

    bool IsLongTextField(int fieldId);

    string GetFieldReferenceName(int fieldId);

    string GetFieldName(int fieldId);

    Type GetFieldSystemType(int fieldId);
  }
}
