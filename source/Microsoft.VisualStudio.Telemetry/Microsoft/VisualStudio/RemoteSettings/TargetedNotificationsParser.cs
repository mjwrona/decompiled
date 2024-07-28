// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteSettings.TargetedNotificationsParser
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry.Services;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.RemoteSettings
{
  internal sealed class TargetedNotificationsParser : ITargetedNotificationsParser
  {
    public async Task<ActionResponseBag> ParseStreamAsync(
      Stream stream,
      CancellationToken cancellationToken)
    {
      string str = (string) null;
      ActionResponseBag actions = new ActionResponseBag();
      if (stream == null)
        return actions;
      try
      {
        using (StreamReader streamReader = new StreamReader(stream))
          str = await streamReader.ReadToEndAsync().WithCancellation<string>(cancellationToken).ConfigureAwait(false);
        if (!string.IsNullOrEmpty(str))
          actions = JsonConvert.DeserializeObject<ActionResponseBag>(str);
      }
      catch (Exception ex)
      {
        throw new TargetedNotificationsException(ex.Message, ex);
      }
      return actions;
    }
  }
}
