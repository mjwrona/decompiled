// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions.WorkItemDocument
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10D2EBC4-B606-4155-939F-EEB226A80181
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions.dll

using System;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions
{
  public class WorkItemDocument : CorePipelineDocument<Guid>
  {
    public WorkItemDocument(int id)
      : base(Guid.NewGuid())
    {
      WorkItemDocument.ValidateId(id);
    }

    private static int ValidateId(int id) => id > 0 ? id : throw new ArgumentOutOfRangeException(nameof (id), "ID must be a positive integer");

    public override string ToString() => FormattableString.Invariant(FormattableStringFactory.Create("Work Item Id: [{0}]", (object) this.Id));
  }
}
