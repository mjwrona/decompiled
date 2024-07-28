// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.IO.RowReaderExtensions
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.IO
{
  public static class RowReaderExtensions
  {
    public static Result ReadList<TItem>(
      ref this RowReader reader,
      RowReaderExtensions.DeserializerFunc<TItem> deserializer,
      out List<TItem> list)
    {
      RowReaderExtensions.ListContext<TItem> context = new RowReaderExtensions.ListContext<TItem>()
      {
        List = new List<TItem>(),
        Deserializer = deserializer
      };
      Result result1 = reader.ReadScope<RowReaderExtensions.ListContext<TItem>>(context, (RowReader.ReaderFunc<RowReaderExtensions.ListContext<TItem>>) ((ref RowReader arrayReader, RowReaderExtensions.ListContext<TItem> ctx1) =>
      {
        while (arrayReader.Read())
        {
          Result result3 = arrayReader.ReadScope<RowReaderExtensions.ListContext<TItem>>(ctx1, (RowReader.ReaderFunc<RowReaderExtensions.ListContext<TItem>>) ((ref RowReader itemReader, RowReaderExtensions.ListContext<TItem> ctx2) =>
          {
            TItem obj;
            Result result4 = ctx2.Deserializer(ref itemReader, out obj);
            if (result4 != Result.Success)
              return result4;
            ctx2.List.Add(obj);
            return Result.Success;
          }));
          if (result3 != Result.Success)
            return result3;
        }
        return Result.Success;
      }));
      if (result1 != Result.Success)
      {
        list = (List<TItem>) null;
        return result1;
      }
      list = context.List;
      return Result.Success;
    }

    public delegate Result DeserializerFunc<TItem>(ref RowReader reader, out TItem item);

    private struct ListContext<TItem>
    {
      public List<TItem> List;
      public RowReaderExtensions.DeserializerFunc<TItem> Deserializer;
    }
  }
}
