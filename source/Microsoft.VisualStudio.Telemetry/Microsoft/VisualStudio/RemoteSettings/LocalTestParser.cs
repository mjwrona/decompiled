// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteSettings.LocalTestParser
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.RemoteSettings
{
  internal class LocalTestParser : ILocalTestParser
  {
    public async Task<IEnumerable<ActionResponse>> ParseStreamAsync(
      DirectoryReaderContext streamContext)
    {
      string json = (string) null;
      IEnumerable<ActionResponse> actions = Enumerable.Empty<ActionResponse>();
      try
      {
        using (StreamReader streamReader = new StreamReader(streamContext.Stream))
          json = await streamReader.ReadToEndAsync().ConfigureAwait(false);
      }
      catch
      {
        return actions;
      }
      if (!string.IsNullOrEmpty(json))
      {
        try
        {
          JToken jtoken = JToken.Parse(json);
          switch (jtoken)
          {
            case JArray _:
              actions = (IEnumerable<ActionResponse>) jtoken.ToObject<IEnumerable<FileActionResponse>>();
              break;
            case JObject _:
              actions = (IEnumerable<ActionResponse>) new ActionResponse[1]
              {
                (ActionResponse) jtoken.ToObject<FileActionResponse>()
              };
              break;
            default:
              throw new JsonException("Stream was neither an array nor object");
          }
        }
        catch (Exception ex)
        {
          throw new TargetedNotificationsException(ex.Message, ex);
        }
      }
      foreach (ActionResponse actionResponse in actions)
        actionResponse.Origin = streamContext.DirectoryName + "-" + streamContext.FileName;
      return actions;
    }
  }
}
