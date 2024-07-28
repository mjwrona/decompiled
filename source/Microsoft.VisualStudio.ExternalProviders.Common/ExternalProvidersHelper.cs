// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ExternalProviders.Common.ExternalProvidersHelper
// Assembly: Microsoft.VisualStudio.ExternalProviders.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7E34B318-B0E9-49BD-88C0-4A425E8D0753
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.ExternalProviders.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.Redis;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.ExternalProviders.Common
{
  public static class ExternalProvidersHelper
  {
    private const string c_area = "ExternalProvidersHelper";
    private const string c_layer = "ExternalProvidersHelper";

    public static T TimedCall<T>(
      IVssRequestContext requestContext,
      Func<Task<T>> f,
      int timeoutSec = 100,
      [CallerMemberName] string caller = null)
    {
      Task<T> realTask = f();
      try
      {
        using (CancellationTokenSource cts = new CancellationTokenSource())
          return requestContext.RunSynchronously<T>((Func<Task<T>>) (async () =>
          {
            Task task = await Task.WhenAny(Task.Delay(TimeSpan.FromSeconds((double) timeoutSec), cts.Token), (Task) realTask);
            if (!realTask.IsCompleted)
              throw new TimeoutException(string.Format("Timeout in {0}! Task did not complete in {1}s", (object) caller, (object) timeoutSec));
            cts.Cancel();
            return await realTask;
          }));
      }
      catch (TimeoutException ex)
      {
        requestContext.TraceException(ExternalProvidersTracePoints.TaskTimedOut, TraceLevel.Error, nameof (ExternalProvidersHelper), nameof (ExternalProvidersHelper), (Exception) ex, new
        {
          Msg = ex.Message,
          TResult = typeof (T).Name,
          Status = realTask.Status,
          TaskObjFields = ExternalProvidersHelper.DumpObjFieldsToJSON((object) realTask),
          Stack = ex.StackTrace
        }.Serialize());
        throw;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(ExternalProvidersTracePoints.GenericExpectionInTimedCall, nameof (ExternalProvidersHelper), nameof (ExternalProvidersHelper), ex);
        throw;
      }
    }

    private static string DumpObjFieldsToJSON(object o)
    {
      try
      {
        return JsonConvert.SerializeObject(o, new JsonSerializerSettings()
        {
          ContractResolver = (IContractResolver) new AllFieldsResolver()
        });
      }
      catch (Exception ex)
      {
        return string.Format("{0} was thrown while trying to dump fields of {1} to JSON: {2}", (object) ex, (object) o.GetType(), (object) ex.Message);
      }
    }
  }
}
