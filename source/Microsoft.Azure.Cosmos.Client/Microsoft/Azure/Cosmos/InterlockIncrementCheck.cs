// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.InterlockIncrementCheck
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Threading;

namespace Microsoft.Azure.Cosmos
{
  internal class InterlockIncrementCheck
  {
    private readonly int maxConcurrentOperations;
    private int counter;

    public InterlockIncrementCheck(int maxConcurrentOperations = 1) => this.maxConcurrentOperations = maxConcurrentOperations >= 1 ? maxConcurrentOperations : throw new ArgumentOutOfRangeException(nameof (maxConcurrentOperations), "Cannot be lower than 1.");

    public void EnterLockCheck()
    {
      Interlocked.Increment(ref this.counter);
      if (this.counter > this.maxConcurrentOperations)
        throw new InvalidOperationException(string.Format("InterlockIncrementCheck detected {0} with a maximum of {1}.", (object) this.counter, (object) this.maxConcurrentOperations));
    }
  }
}
