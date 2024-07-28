// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ComparableTask
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents
{
  internal abstract class ComparableTask : 
    IComparableTask,
    IComparable<IComparableTask>,
    IEquatable<IComparableTask>
  {
    protected readonly int schedulePriority;

    protected ComparableTask(int schedulePriority) => this.schedulePriority = schedulePriority;

    public abstract Task StartAsync(CancellationToken token);

    public virtual int CompareTo(IComparableTask other) => other != null ? this.CompareToByPriority(other as ComparableTask) : throw new ArgumentNullException(nameof (other));

    public override bool Equals(object obj) => this.Equals(obj as IComparableTask);

    public abstract bool Equals(IComparableTask other);

    public abstract override int GetHashCode();

    protected int CompareToByPriority(ComparableTask other)
    {
      if (other == null)
        return 1;
      return this == other ? 0 : this.schedulePriority.CompareTo(other.schedulePriority);
    }
  }
}
