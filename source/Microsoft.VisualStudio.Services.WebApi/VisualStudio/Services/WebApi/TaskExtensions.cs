// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.TaskExtensions
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using AsyncFixer;
using System.ComponentModel;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class TaskExtensions
  {
    [BlockCaller]
    public static void SyncResult(this Task task) => task.GetAwaiter().GetResult();

    [BlockCaller]
    public static T SyncResult<T>(this Task<T> task) => task.GetAwaiter().GetResult();

    [BlockCaller]
    public static void SyncResultConfigured(this Task task) => task.ConfigureAwait(false).GetAwaiter().GetResult();

    [BlockCaller]
    public static T SyncResultConfigured<T>(this Task<T> task) => task.ConfigureAwait(false).GetAwaiter().GetResult();

    [BlockCaller]
    public static HttpResponseMessage SyncResult(this Task<HttpResponseMessage> task) => task.GetAwaiter().GetResult();
  }
}
