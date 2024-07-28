// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.Presentation.FormattingWorkItem
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.Presentation
{
  internal class FormattingWorkItem
  {
    public int Id { get; private set; }

    public IDictionary<string, object> FieldValues { get; private set; }

    public FormattingWorkItem(int id)
    {
      this.Id = id;
      this.FieldValues = (IDictionary<string, object>) new Dictionary<string, object>();
    }
  }
}
