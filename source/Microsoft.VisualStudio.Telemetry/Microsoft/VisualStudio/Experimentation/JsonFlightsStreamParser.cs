// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Experimentation.JsonFlightsStreamParser
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Experimentation
{
  internal sealed class JsonFlightsStreamParser : IFlightsStreamParser
  {
    public async Task<T> ParseStreamAsync<T>(Stream stream) where T : IFlightsData
    {
      string str = (string) null;
      T flights = default (T);
      try
      {
        using (StreamReader streamReader = new StreamReader(stream))
          str = await streamReader.ReadToEndAsync().ConfigureAwait(false);
      }
      catch
      {
        return flights;
      }
      if (!string.IsNullOrEmpty(str))
      {
        try
        {
          flights = JsonConvert.DeserializeObject<T>(str);
        }
        catch
        {
        }
      }
      return flights;
    }
  }
}
