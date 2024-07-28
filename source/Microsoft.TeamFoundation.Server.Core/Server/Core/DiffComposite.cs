// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.DiffComposite
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class DiffComposite : DiffSpec
  {
    public DiffComposite(IEnumerable<DiffSpec> items) => this.Items = items;

    public IEnumerable<DiffSpec> Items { get; private set; }

    public override string ToString() => "[" + string.Join<DiffSpec>(", ", this.Items) + "]";
  }
}
