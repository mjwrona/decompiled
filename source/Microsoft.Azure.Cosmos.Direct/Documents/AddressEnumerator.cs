// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.AddressEnumerator
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Microsoft.Azure.Documents
{
  internal sealed class AddressEnumerator : IAddressEnumerator
  {
    [ThreadStatic]
    private static Random random;

    private static int GenerateNextRandom(int start, int maxValue)
    {
      if (AddressEnumerator.random == null)
        AddressEnumerator.random = CustomTypeExtensions.GetRandomNumber();
      return AddressEnumerator.random.Next(start, maxValue);
    }

    public IEnumerable<TransportAddressUri> GetTransportAddresses(
      IReadOnlyList<TransportAddressUri> transportAddressUris,
      Lazy<HashSet<TransportAddressUri>> failedEndpoints)
    {
      return failedEndpoints != null ? this.MoveFailedReplicasToTheEnd(this.GetTransportAddresses(transportAddressUris), failedEndpoints) : throw new ArgumentNullException(nameof (failedEndpoints));
    }

    private IEnumerable<TransportAddressUri> GetTransportAddresses(
      IReadOnlyList<TransportAddressUri> transportAddressUris)
    {
      if (transportAddressUris == null)
        throw new ArgumentNullException(nameof (transportAddressUris));
      if (transportAddressUris.Count == 0)
        return Enumerable.Empty<TransportAddressUri>();
      return AddressEnumerator.AddressEnumeratorUsingPermutations.IsSizeInPermutationLimits(transportAddressUris.Count) ? AddressEnumerator.AddressEnumeratorUsingPermutations.GetTransportAddressUrisWithPredefinedPermutation(transportAddressUris) : AddressEnumerator.AddressEnumeratorFisherYateShuffle.GetTransportAddressUrisWithFisherYateShuffle(transportAddressUris);
    }

    private IEnumerable<TransportAddressUri> MoveFailedReplicasToTheEnd(
      IEnumerable<TransportAddressUri> randomPermutation,
      Lazy<HashSet<TransportAddressUri>> lazyFailedReplicasPerRequest)
    {
      List<TransportAddressUri> failedAddressUris = (List<TransportAddressUri>) null;
      HashSet<TransportAddressUri> failedReplicasPerRequest = (HashSet<TransportAddressUri>) null;
      if (lazyFailedReplicasPerRequest != null && lazyFailedReplicasPerRequest.IsValueCreated && lazyFailedReplicasPerRequest.Value.Count > 0)
        failedReplicasPerRequest = lazyFailedReplicasPerRequest.Value;
      foreach (TransportAddressUri transportAddressUri in randomPermutation)
      {
        if (transportAddressUri.IsUnhealthy() || failedReplicasPerRequest != null && failedReplicasPerRequest.Contains(transportAddressUri))
        {
          if (failedAddressUris == null)
            failedAddressUris = new List<TransportAddressUri>();
          failedAddressUris.Add(transportAddressUri);
        }
        else
          yield return transportAddressUri;
      }
      if (failedAddressUris != null)
      {
        foreach (TransportAddressUri transportAddressUri in failedAddressUris)
          yield return transportAddressUri;
      }
    }

    [StructLayout(LayoutKind.Sequential, Size = 1)]
    private readonly struct AddressEnumeratorFisherYateShuffle
    {
      public static IEnumerable<TransportAddressUri> GetTransportAddressUrisWithFisherYateShuffle(
        IReadOnlyList<TransportAddressUri> transportAddressUris)
      {
        List<TransportAddressUri> transportAddressesCopy = transportAddressUris.ToList<TransportAddressUri>();
        for (int i = 0; i < transportAddressUris.Count - 1; ++i)
        {
          AddressEnumerator.AddressEnumeratorFisherYateShuffle.Swap(transportAddressesCopy, i, AddressEnumerator.GenerateNextRandom(i, transportAddressUris.Count));
          yield return transportAddressesCopy[i];
        }
        yield return transportAddressesCopy.Last<TransportAddressUri>();
      }

      private static void Swap(
        List<TransportAddressUri> transportAddressUris,
        int firstIndex,
        int secondIndex)
      {
        if (firstIndex == secondIndex)
          return;
        TransportAddressUri transportAddressUri = transportAddressUris[firstIndex];
        transportAddressUris[firstIndex] = transportAddressUris[secondIndex];
        transportAddressUris[secondIndex] = transportAddressUri;
      }
    }

    [StructLayout(LayoutKind.Sequential, Size = 1)]
    private readonly struct AddressEnumeratorUsingPermutations
    {
      private static readonly IReadOnlyList<IReadOnlyList<IReadOnlyList<int>>> AllPermutationsOfIndexesBySize;

      static AddressEnumeratorUsingPermutations()
      {
        List<IReadOnlyList<IReadOnlyList<int>>> intListListList = new List<IReadOnlyList<IReadOnlyList<int>>>();
        for (int index = 0; index <= 6; ++index)
        {
          List<IReadOnlyList<int>> output = new List<IReadOnlyList<int>>();
          AddressEnumerator.AddressEnumeratorUsingPermutations.PermuteIndexPositions(Enumerable.Range(0, index).ToArray<int>(), 0, index, output);
          intListListList.Add((IReadOnlyList<IReadOnlyList<int>>) output);
        }
        AddressEnumerator.AddressEnumeratorUsingPermutations.AllPermutationsOfIndexesBySize = (IReadOnlyList<IReadOnlyList<IReadOnlyList<int>>>) intListListList;
      }

      public static bool IsSizeInPermutationLimits(int size) => size < AddressEnumerator.AddressEnumeratorUsingPermutations.AllPermutationsOfIndexesBySize.Count;

      public static IEnumerable<TransportAddressUri> GetTransportAddressUrisWithPredefinedPermutation(
        IReadOnlyList<TransportAddressUri> transportAddressUris)
      {
        IReadOnlyList<IReadOnlyList<int>> intListList = AddressEnumerator.AddressEnumeratorUsingPermutations.AllPermutationsOfIndexesBySize[transportAddressUris.Count];
        int nextRandom = AddressEnumerator.GenerateNextRandom(0, intListList.Count);
        foreach (int index in (IEnumerable<int>) intListList[nextRandom])
          yield return transportAddressUris[index];
      }

      private static void PermuteIndexPositions(
        int[] array,
        int start,
        int length,
        List<IReadOnlyList<int>> output)
      {
        if (start == length)
        {
          output.Add((IReadOnlyList<int>) ((IEnumerable<int>) array).ToList<int>());
        }
        else
        {
          for (int index = start; index < length; ++index)
          {
            AddressEnumerator.AddressEnumeratorUsingPermutations.Swap(ref array[start], ref array[index]);
            AddressEnumerator.AddressEnumeratorUsingPermutations.PermuteIndexPositions(array, start + 1, length, output);
            AddressEnumerator.AddressEnumeratorUsingPermutations.Swap(ref array[start], ref array[index]);
          }
        }
      }

      private static void Swap(ref int a, ref int b)
      {
        int num = a;
        a = b;
        b = num;
      }
    }
  }
}
