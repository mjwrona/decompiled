// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.SourceRepoTrigger
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model
{
  public class SourceRepoTrigger : ReleaseTriggerBase
  {
    private List<string> branchFilters;
    private List<string> pathFilters;

    public SourceRepoTrigger() => this.TriggerType = ReleaseTriggerType.SourceRepo;

    public string Alias { get; set; }

    public List<string> BranchFilters
    {
      get
      {
        if (this.branchFilters == null)
          this.branchFilters = new List<string>();
        return this.branchFilters;
      }
    }

    public List<string> PathFilters
    {
      get
      {
        if (this.pathFilters == null)
          this.pathFilters = new List<string>();
        return this.pathFilters;
      }
    }

    public override ReleaseTriggerBase DeepClone() => (ReleaseTriggerBase) base.DeepClone().ToSourceRepoTrigger();
  }
}
