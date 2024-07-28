// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.ValidateWebLayoutSystemControlErrorActions
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class ValidateWebLayoutSystemControlErrorActions
  {
    public Action<string> SystemControlTypeErrorAction { get; set; }

    public Action<string> SystemControlFieldErrorAction { get; set; }

    public Action<string> SystemControlDuplicateErrorAction { get; set; }

    public Action<string> SystemControlReplacesFieldErrorAction { get; set; }

    public Action<string> SystemControlHiddenFieldErrorAction { get; set; }

    public Action<string> SystemControlReplacesDuplicateFieldErrorAction { get; set; }
  }
}
