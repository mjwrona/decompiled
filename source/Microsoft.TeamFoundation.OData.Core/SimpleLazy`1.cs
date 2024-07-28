// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.SimpleLazy`1
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;

namespace Microsoft.OData
{
  internal sealed class SimpleLazy<T>
  {
    private readonly object mutex;
    private readonly Func<T> factory;
    private T value;
    private bool valueCreated;

    internal SimpleLazy(Func<T> factory)
      : this(factory, false)
    {
    }

    internal SimpleLazy(Func<T> factory, bool isThreadSafe)
    {
      this.factory = factory;
      this.valueCreated = false;
      if (!isThreadSafe)
        return;
      this.mutex = new object();
    }

    internal T Value
    {
      get
      {
        if (!this.valueCreated)
        {
          if (this.mutex != null)
          {
            lock (this.mutex)
            {
              if (!this.valueCreated)
                this.CreateValue();
            }
          }
          else
            this.CreateValue();
        }
        return this.value;
      }
    }

    private void CreateValue()
    {
      this.value = this.factory();
      this.valueCreated = true;
    }
  }
}
