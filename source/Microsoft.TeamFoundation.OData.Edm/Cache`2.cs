// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Cache`2
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System;

namespace Microsoft.OData.Edm
{
  internal class Cache<TContainer, TProperty>
  {
    private object value = CacheHelper.Unknown;

    public TProperty GetValue(
      TContainer container,
      Func<TContainer, TProperty> compute,
      Func<TContainer, TProperty> onCycle)
    {
      object obj1 = (object) onCycle ?? (object) this;
      object obj2 = this.value;
      if (obj2 == CacheHelper.Unknown)
      {
        lock (obj1)
        {
          if (this.value == CacheHelper.Unknown)
          {
            this.value = CacheHelper.CycleSentinel;
            TProperty property;
            try
            {
              property = compute(container);
            }
            catch
            {
              this.value = CacheHelper.Unknown;
              throw;
            }
            if (this.value == CacheHelper.CycleSentinel)
              this.value = typeof (TProperty) == typeof (bool) ? CacheHelper.BoxedBool((bool) (object) property) : (object) property;
          }
          obj2 = this.value;
        }
      }
      else if (obj2 == CacheHelper.CycleSentinel)
      {
        lock (obj1)
        {
          if (this.value == CacheHelper.CycleSentinel)
          {
            this.value = CacheHelper.SecondPassCycleSentinel;
            try
            {
              TProperty property = compute(container);
            }
            catch
            {
              this.value = CacheHelper.CycleSentinel;
              throw;
            }
            if (this.value == CacheHelper.SecondPassCycleSentinel)
              this.value = (object) onCycle(container);
          }
          else if (this.value == CacheHelper.Unknown)
            return this.GetValue(container, compute, onCycle);
          obj2 = this.value;
        }
      }
      else if (obj2 == CacheHelper.SecondPassCycleSentinel)
      {
        lock (obj1)
        {
          if (this.value == CacheHelper.SecondPassCycleSentinel)
            this.value = (object) onCycle(container);
          else if (this.value == CacheHelper.Unknown)
            return this.GetValue(container, compute, onCycle);
          obj2 = this.value;
        }
      }
      return (TProperty) obj2;
    }

    public void Clear(Func<TContainer, TProperty> onCycle)
    {
      lock ((object) onCycle ?? (object) this)
      {
        if (this.value == CacheHelper.CycleSentinel || this.value == CacheHelper.SecondPassCycleSentinel)
          return;
        this.value = CacheHelper.Unknown;
      }
    }
  }
}
