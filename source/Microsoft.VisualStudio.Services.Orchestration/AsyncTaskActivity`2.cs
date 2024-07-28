// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.AsyncTaskActivity`2
// Assembly: Microsoft.VisualStudio.Services.Orchestration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C0C603F4-BE31-455B-860A-9FD3B046611C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.dll

using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Orchestration
{
  public abstract class AsyncTaskActivity<TInput, TResult> : TaskActivity
  {
    protected AsyncTaskActivity(OrchestrationSerializer serializer) => this.Serializer = serializer != null ? serializer : throw new ArgumentNullException(nameof (serializer));

    public OrchestrationSerializer Serializer { get; private set; }

    public override string Run(TaskContext context, string input) => string.Empty;

    protected abstract Task<TResult> ExecuteAsync(TaskContext context, TInput input);

    public override async Task<string> RunAsync(TaskContext context, string input)
    {
      TInput input1 = default (TInput);
      JArray jarray = JArray.Parse(input);
      if (jarray != null)
      {
        int count = jarray.Count;
        if (count > 1)
          throw new TaskFailureException("TaskActivity implementation cannot be invoked due to more than expected input parameters.  Signature mismatch.");
        if (count == 1)
        {
          JToken jtoken = jarray[0];
          input1 = !(jtoken is JValue jvalue) ? this.Serializer.Deserialize<TInput>(jtoken.ToString()) : jvalue.ToObject<TInput>();
        }
      }
      TResult result;
      try
      {
        result = await this.ExecuteAsync(context, input1);
      }
      catch (Exception ex)
      {
        OrchestrationSerializer serializer = this.Serializer;
        string details = Utils.SerializeCause(ex, serializer);
        throw new TaskFailureException(ex.Message, details);
      }
      return this.Serializer.Serialize((object) result);
    }
  }
}
