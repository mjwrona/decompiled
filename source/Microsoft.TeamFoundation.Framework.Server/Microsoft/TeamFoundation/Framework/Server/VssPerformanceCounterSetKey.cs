// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VssPerformanceCounterSetKey
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal struct VssPerformanceCounterSetKey
  {
    public readonly string UnqualifiedInstanceName;
    public readonly string ProcessName;
    private const int MAX_INSTANCENAME_LENGTH = 127;
    public static readonly IEqualityComparer<VssPerformanceCounterSetKey> Comparer = (IEqualityComparer<VssPerformanceCounterSetKey>) new VssPerformanceCounterSetKey.CounterSetKeyComparer();

    public VssPerformanceCounterSetKey(string unqualifiedInstanceName, string processName)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(unqualifiedInstanceName, nameof (unqualifiedInstanceName));
      this.UnqualifiedInstanceName = unqualifiedInstanceName;
      this.ProcessName = processName;
    }

    public string GetQualifiedInstanceName()
    {
      string qualifiedInstanceName;
      if (this.ProcessName != null)
      {
        qualifiedInstanceName = this.UnqualifiedInstanceName + "-" + this.ProcessName;
        if (qualifiedInstanceName.Length > (int) sbyte.MaxValue)
          qualifiedInstanceName = this.UnqualifiedInstanceName.Substring(0, this.UnqualifiedInstanceName.Length - (qualifiedInstanceName.Length - (int) sbyte.MaxValue)) + "-" + this.ProcessName;
      }
      else
      {
        qualifiedInstanceName = this.UnqualifiedInstanceName;
        if (qualifiedInstanceName.Length > (int) sbyte.MaxValue)
          qualifiedInstanceName = qualifiedInstanceName.Substring(0, (int) sbyte.MaxValue);
      }
      return qualifiedInstanceName;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int GetEffectiveInstanceNameLength()
    {
      int instanceNameLength = this.UnqualifiedInstanceName.Length;
      if (this.ProcessName != null)
      {
        int num = instanceNameLength + (1 + this.ProcessName.Length) - (int) sbyte.MaxValue;
        if (num > 0)
          instanceNameLength -= num;
      }
      else if (instanceNameLength > (int) sbyte.MaxValue)
        instanceNameLength = (int) sbyte.MaxValue;
      return instanceNameLength;
    }

    private class CounterSetKeyComparer : IEqualityComparer<VssPerformanceCounterSetKey>
    {
      public bool Equals(VssPerformanceCounterSetKey x, VssPerformanceCounterSetKey y)
      {
        if (StringComparer.OrdinalIgnoreCase.Equals(x.ProcessName, y.ProcessName))
        {
          int instanceNameLength1 = x.GetEffectiveInstanceNameLength();
          int instanceNameLength2 = y.GetEffectiveInstanceNameLength();
          if (instanceNameLength1 == instanceNameLength2)
            return string.Compare(x.UnqualifiedInstanceName, 0, y.UnqualifiedInstanceName, 0, instanceNameLength1, StringComparison.OrdinalIgnoreCase) == 0;
        }
        return false;
      }

      public int GetHashCode(VssPerformanceCounterSetKey obj)
      {
        int ordinalIgnoreCaseHash = VssPerformanceCounterSetKey.CounterSetKeyComparer.GetOrdinalIgnoreCaseHash(obj.UnqualifiedInstanceName, obj.GetEffectiveInstanceNameLength());
        if (obj.ProcessName != null)
          ordinalIgnoreCaseHash ^= StringComparer.OrdinalIgnoreCase.GetHashCode(obj.ProcessName);
        return ordinalIgnoreCaseHash;
      }

      private static unsafe int GetOrdinalIgnoreCaseHash(string str, int length)
      {
        if (length > str.Length)
          throw new ArgumentOutOfRangeException(nameof (length));
        fixed (char* chPtr1 = str)
        {
          int num1 = 5381;
          int num2 = num1;
          char* chPtr2 = chPtr1;
          while (true)
          {
            int c1 = (int) *chPtr2;
            if (c1 != 0 && length-- != 0)
            {
              num1 = (num1 << 5) + num1 ^ VssPerformanceCounterSetKey.CounterSetKeyComparer.GetHashCodeContribution(c1);
              int c2 = (int) chPtr2[1];
              if (c2 != 0 && length-- != 0)
              {
                num2 = (num2 << 5) + num2 ^ VssPerformanceCounterSetKey.CounterSetKeyComparer.GetHashCodeContribution(c2);
                chPtr2 += 2;
              }
              else
                break;
            }
            else
              break;
          }
          return num1 + num2 * 1566083941;
        }
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      private static int GetHashCodeContribution(int c)
      {
        if (c > (int) sbyte.MaxValue)
          return 128;
        return 65 <= c && c <= 90 ? c | 32 : c;
      }
    }
  }
}
