// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ContainerImageTrigger
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model
{
  public class ContainerImageTrigger : ReleaseTriggerBase
  {
    public ContainerImageTrigger() => this.TriggerType = ReleaseTriggerType.ContainerImage;

    public string Alias { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter needed for deserialization")]
    public IList<TagFilter> TagFilters { get; set; }

    public override ReleaseTriggerBase DeepClone() => (ReleaseTriggerBase) base.DeepClone().ToContainerImageTrigger();

    public override bool Equals(object obj)
    {
      if (obj == null || this.GetType() != obj.GetType())
        return false;
      ContainerImageTrigger trigger = (ContainerImageTrigger) obj;
      return string.Equals(this.Alias, trigger.Alias) && this.TriggerType == trigger.TriggerType && this.IsTagFiltersEqual(trigger);
    }

    public override int GetHashCode() => this.Alias.GetHashCode();

    private bool IsTagFiltersEqual(ContainerImageTrigger trigger) => this.TagFilters.IsNullOrEmpty<TagFilter>() && trigger.TagFilters.IsNullOrEmpty<TagFilter>() || !this.TagFilters.IsNullOrEmpty<TagFilter>() && !trigger.TagFilters.IsNullOrEmpty<TagFilter>() && this.TagFilters.SequenceEqual<TagFilter>((IEnumerable<TagFilter>) trigger.TagFilters);
  }
}
