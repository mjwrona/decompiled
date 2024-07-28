// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.TablePhysicalNode
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public class TablePhysicalNode : IPhysicalNode
  {
    private readonly Lazy<ITable> lazyTable;

    public TablePhysicalNode(Func<ITable> table, string uniqueName)
    {
      this.lazyTable = new Lazy<ITable>(table, LazyThreadSafetyMode.ExecutionAndPublication);
      this.UniqueName = uniqueName;
    }

    public ITable Table => this.lazyTable.Value;

    public string UniqueName { get; private set; }

    public bool Equals(IPhysicalNode x, IPhysicalNode y) => x.UniqueName.Equals(y.UniqueName);

    public int GetHashCode(IPhysicalNode obj) => this.UniqueName.GetHashCode();
  }
}
