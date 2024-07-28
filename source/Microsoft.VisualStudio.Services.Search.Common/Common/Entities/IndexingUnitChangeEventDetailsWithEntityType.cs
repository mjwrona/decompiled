// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Entities.IndexingUnitChangeEventDetailsWithEntityType
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Search.Common.Entities
{
  public class IndexingUnitChangeEventDetailsWithEntityType : IndexingUnitChangeEventDetails
  {
    public IEntityType EntityType { get; set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "(IndexingUnitChangeEvent : {0}, AssociatedJobId: {1}, EntityType: {2})", (object) this.IndexingUnitChangeEvent, (object) (this.AssociatedJobId ?? Guid.Empty), (object) (this.EntityType?.Name ?? "Not set"));
  }
}
