// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessValidator.ProcessTemplateFieldRenameData
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessValidator
{
  public class ProcessTemplateFieldRenameData
  {
    public ProcessTemplateFieldRenameData()
    {
    }

    public ProcessTemplateFieldRenameData(
      string refName,
      string newName,
      IEnumerable<Guid> templatesToUpdate)
    {
      this.RefName = refName;
      this.NewName = newName;
      this.TemplatesToUpdate = templatesToUpdate;
    }

    public string RefName { get; set; }

    public string NewName { get; set; }

    public IEnumerable<Guid> TemplatesToUpdate { get; set; }
  }
}
