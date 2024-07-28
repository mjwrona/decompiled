// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Rntbd.UserPortPool
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Sockets;

namespace Microsoft.Azure.Documents.Rntbd
{
  internal sealed class UserPortPool
  {
    private readonly int portReuseThreshold;
    private readonly int candidatePortCount;
    private readonly UserPortPool.Pool ipv4Pool = new UserPortPool.Pool();
    private readonly UserPortPool.Pool ipv6Pool = new UserPortPool.Pool();

    public UserPortPool(int portReuseThreshold, int candidatePortCount)
    {
      if (portReuseThreshold <= 0)
        throw new ArgumentException("The port reuse threshold must be positive");
      if (candidatePortCount <= 0)
        throw new ArgumentException("The candidate port count must be positive");
      this.portReuseThreshold = candidatePortCount <= portReuseThreshold ? portReuseThreshold : throw new ArgumentException("The candidate port count must be less than or equal to the port reuse threshold");
      this.candidatePortCount = candidatePortCount;
    }

    public ushort[] GetCandidatePorts(AddressFamily addressFamily)
    {
      UserPortPool.Pool pool = this.GetPool(addressFamily);
      lock (pool.mutex)
        return pool.usablePortCount < this.portReuseThreshold ? (ushort[]) null : UserPortPool.GetRandomSample(pool.ports, this.candidatePortCount, pool.rand);
    }

    public void AddReference(AddressFamily addressFamily, ushort port)
    {
      UserPortPool.Pool pool = this.GetPool(addressFamily);
      lock (pool.mutex)
      {
        UserPortPool.PortState portState = (UserPortPool.PortState) null;
        if (pool.ports.TryGetValue(port, out portState))
        {
          ++portState.referenceCount;
        }
        else
        {
          portState = new UserPortPool.PortState();
          ++portState.referenceCount;
          pool.ports.Add(port, portState);
          ++pool.usablePortCount;
          DefaultTrace.TraceInformation("PrivatePortPool: Port is added to port pool: {0}", (object) port);
        }
      }
    }

    public void RemoveReference(AddressFamily addressFamily, ushort port)
    {
      UserPortPool.Pool pool = this.GetPool(addressFamily);
      lock (pool.mutex)
      {
        UserPortPool.PortState portState = (UserPortPool.PortState) null;
        if (!pool.ports.TryGetValue(port, out portState))
          return;
        --portState.referenceCount;
        if (portState.referenceCount != 0)
          return;
        pool.ports.Remove(port);
        if (portState.usable)
          --pool.usablePortCount;
        else
          --pool.unusablePortCount;
        DefaultTrace.TraceInformation("PrivatePortPool: Port is removed from port pool: {0}", (object) port);
      }
    }

    public void MarkUnusable(AddressFamily addressFamily, ushort port)
    {
      UserPortPool.Pool pool = this.GetPool(addressFamily);
      lock (pool.mutex)
      {
        UserPortPool.PortState portState = (UserPortPool.PortState) null;
        if (!pool.ports.TryGetValue(port, out portState))
          return;
        portState.usable = false;
        --pool.usablePortCount;
        ++pool.unusablePortCount;
        DefaultTrace.TraceInformation("PrivatePortPool: Port is marked as unusable: {0}", (object) port);
      }
    }

    public string DumpStatus() => string.Format("portReuseThreshold:{0};candidatePortCount:{1};ipv4Pool:{2};ipv6Pool:{3}", (object) this.portReuseThreshold, (object) this.candidatePortCount, (object) this.DumpPoolStatus(this.ipv4Pool), (object) this.DumpPoolStatus(this.ipv6Pool));

    private string DumpPoolStatus(UserPortPool.Pool pool)
    {
      lock (pool.mutex)
        return string.Format("{0} totalPorts.{1} usablePorts.", (object) pool.ports.Count, (object) pool.usablePortCount);
    }

    private UserPortPool.Pool GetPool(AddressFamily af)
    {
      if (af == AddressFamily.InterNetwork)
        return this.ipv4Pool;
      if (af == AddressFamily.InterNetworkV6)
        return this.ipv6Pool;
      throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Address family {0} not supported", (object) af));
    }

    private static ushort[] GetRandomSample(
      Dictionary<ushort, UserPortPool.PortState> pool,
      int candidatePortCount,
      Random rng)
    {
      ushort[] sample = UserPortPool.ReservoirSample(pool, candidatePortCount, rng);
      UserPortPool.Shuffle(rng, sample);
      return sample;
    }

    private static ushort[] ReservoirSample(
      Dictionary<ushort, UserPortPool.PortState> pool,
      int candidatePortCount,
      Random rng)
    {
      Dictionary<ushort, UserPortPool.PortState>.KeyCollection keys = pool.Keys;
      ushort[] numArray = new ushort[candidatePortCount];
      int num = 0;
      int index1 = 0;
      foreach (ushort key in (IEnumerable<ushort>) keys)
      {
        if (pool[key].usable)
        {
          if (index1 < numArray.Length)
          {
            numArray[index1] = key;
            ++index1;
          }
          else
          {
            int index2 = rng.Next(num + 1);
            if (index2 < numArray.Length)
              numArray[index2] = key;
          }
          ++num;
        }
      }
      return numArray;
    }

    private static void Shuffle(Random rng, ushort[] sample)
    {
      for (int index1 = sample.Length - 1; index1 > 0; --index1)
      {
        int index2 = rng.Next(index1 + 1);
        ushort num = sample[index2];
        sample[index2] = sample[index1];
        sample[index1] = num;
      }
    }

    private sealed class Pool
    {
      public readonly object mutex = new object();
      public readonly Dictionary<ushort, UserPortPool.PortState> ports = new Dictionary<ushort, UserPortPool.PortState>(192);
      public readonly Random rand = new Random();
      public int usablePortCount;
      public int unusablePortCount;
    }

    private sealed class PortState
    {
      public int referenceCount;
      public bool usable = true;
    }
  }
}
