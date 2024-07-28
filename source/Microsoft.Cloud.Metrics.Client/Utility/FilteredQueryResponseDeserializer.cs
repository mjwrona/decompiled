// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Utility.FilteredQueryResponseDeserializer
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Microsoft.Cloud.Metrics.Client.Query;
using Microsoft.Online.Metrics.Serialization;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Cloud.Metrics.Client.Utility
{
  public sealed class FilteredQueryResponseDeserializer
  {
    public static IReadOnlyList<IFilteredTimeSeriesQueryResponse> Deserialize(Stream stream)
    {
      using (BinaryReader reader = new BinaryReader(stream))
      {
        int capacity = (int) SerializationUtils.ReadUInt32FromBase128(reader);
        List<FilteredTimeSeriesQueryResponse> seriesQueryResponseList = new List<FilteredTimeSeriesQueryResponse>(capacity);
        for (int index1 = 0; index1 < capacity; ++index1)
        {
          int num1 = (int) reader.ReadByte();
          int num2 = reader.ReadInt32();
          for (int index2 = 0; index2 < num2; ++index2)
          {
            FilteredTimeSeriesQueryResponse seriesQueryResponse = new FilteredTimeSeriesQueryResponse();
            seriesQueryResponse.Deserialize(reader);
            seriesQueryResponseList.Add(seriesQueryResponse);
          }
        }
        return (IReadOnlyList<IFilteredTimeSeriesQueryResponse>) seriesQueryResponseList;
      }
    }
  }
}
