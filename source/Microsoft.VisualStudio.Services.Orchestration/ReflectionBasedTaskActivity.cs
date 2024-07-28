// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.ReflectionBasedTaskActivity
// Assembly: Microsoft.VisualStudio.Services.Orchestration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C0C603F4-BE31-455B-860A-9FD3B046611C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.dll

using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Orchestration
{
  public class ReflectionBasedTaskActivity : TaskActivity
  {
    private static readonly object s_voidResult = new object();

    public ReflectionBasedTaskActivity(
      OrchestrationSerializer serializer,
      ITaskActivityInvoker activityInvoker,
      MethodInfo methodInfo)
    {
      this.MethodInfo = methodInfo;
      this.ActivityInvoker = activityInvoker;
      this.Serializer = serializer;
    }

    public OrchestrationSerializer Serializer { get; private set; }

    public ITaskActivityInvoker ActivityInvoker { get; private set; }

    public MethodInfo MethodInfo { get; private set; }

    public override string Run(TaskContext context, string input) => string.Empty;

    public override async Task<string> RunAsync(TaskContext context, string input)
    {
      JArray jarray = JArray.Parse(input);
      int count = jarray.Count;
      ParameterInfo[] parameters = this.MethodInfo.GetParameters();
      if (parameters.Length < count)
        throw new TaskFailureException("TaskActivity implementation cannot be invoked due to more than expected input parameters.  Signature mismatch.");
      object[] inputParameters = new object[parameters.Length];
      for (int index = 0; index < parameters.Length; ++index)
      {
        Type parameterType = parameters[index].ParameterType;
        if (index < count)
        {
          JToken jtoken = jarray[index];
          if (jtoken is JValue jvalue)
          {
            inputParameters[index] = jvalue.ToObject(parameterType);
          }
          else
          {
            string data = jtoken.ToString();
            inputParameters[index] = this.Serializer.Deserialize(data, parameterType);
          }
        }
        else
          inputParameters[index] = !parameters[index].HasDefaultValue ? (parameterType.IsValueType ? Activator.CreateInstance(parameterType) : (object) null) : Type.Missing;
      }
      string str;
      try
      {
        str = await this.InvokeActivityAsync(context, inputParameters);
      }
      catch (TargetInvocationException ex)
      {
        Exception originalException = ex.InnerException ?? (Exception) ex;
        throw new TaskFailureException(originalException.Message, Utils.SerializeCause(originalException, this.Serializer));
      }
      catch (Exception ex)
      {
        OrchestrationSerializer serializer = this.Serializer;
        string details = Utils.SerializeCause(ex, serializer);
        throw new TaskFailureException(ex.Message, details);
      }
      return str;
    }

    public async Task<string> InvokeActivityAsync(TaskContext context, object[] inputParameters)
    {
      object obj1 = await this.ActivityInvoker.Invoke(context, this.MethodInfo.Name, (Func<object, Task<object>>) (async activityObject =>
      {
        object obj2 = this.MethodInfo.Invoke(activityObject, inputParameters);
        if (!(obj2 is Task task2))
          return obj2;
        if (this.MethodInfo.ReturnType.IsGenericType)
        {
          // ISSUE: reference to a compiler-generated field
          if (ReflectionBasedTaskActivity.\u003C\u003Ec__DisplayClass15_0.\u003C\u003CInvokeActivityAsync\u003Eb__0\u003Ed.\u003C\u003Eo.\u003C\u003Ep__0 == null)
          {
            // ISSUE: reference to a compiler-generated field
            ReflectionBasedTaskActivity.\u003C\u003Ec__DisplayClass15_0.\u003C\u003CInvokeActivityAsync\u003Eb__0\u003Ed.\u003C\u003Eo.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "GetAwaiter", (IEnumerable<Type>) null, typeof (ReflectionBasedTaskActivity), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj3 = ReflectionBasedTaskActivity.\u003C\u003Ec__DisplayClass15_0.\u003C\u003CInvokeActivityAsync\u003Eb__0\u003Ed.\u003C\u003Eo.\u003C\u003Ep__0.Target((CallSite) ReflectionBasedTaskActivity.\u003C\u003Ec__DisplayClass15_0.\u003C\u003CInvokeActivityAsync\u003Eb__0\u003Ed.\u003C\u003Eo.\u003C\u003Ep__0, (object) task2);
          // ISSUE: reference to a compiler-generated field
          if (ReflectionBasedTaskActivity.\u003C\u003Ec__DisplayClass15_0.\u003C\u003CInvokeActivityAsync\u003Eb__0\u003Ed.\u003C\u003Eo.\u003C\u003Ep__2 == null)
          {
            // ISSUE: reference to a compiler-generated field
            ReflectionBasedTaskActivity.\u003C\u003Ec__DisplayClass15_0.\u003C\u003CInvokeActivityAsync\u003Eb__0\u003Ed.\u003C\u003Eo.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (bool), typeof (ReflectionBasedTaskActivity)));
          }
          // ISSUE: reference to a compiler-generated field
          Func<CallSite, object, bool> target = ReflectionBasedTaskActivity.\u003C\u003Ec__DisplayClass15_0.\u003C\u003CInvokeActivityAsync\u003Eb__0\u003Ed.\u003C\u003Eo.\u003C\u003Ep__2.Target;
          // ISSUE: reference to a compiler-generated field
          CallSite<Func<CallSite, object, bool>> p2 = ReflectionBasedTaskActivity.\u003C\u003Ec__DisplayClass15_0.\u003C\u003CInvokeActivityAsync\u003Eb__0\u003Ed.\u003C\u003Eo.\u003C\u003Ep__2;
          // ISSUE: reference to a compiler-generated field
          if (ReflectionBasedTaskActivity.\u003C\u003Ec__DisplayClass15_0.\u003C\u003CInvokeActivityAsync\u003Eb__0\u003Ed.\u003C\u003Eo.\u003C\u003Ep__1 == null)
          {
            // ISSUE: reference to a compiler-generated field
            ReflectionBasedTaskActivity.\u003C\u003Ec__DisplayClass15_0.\u003C\u003CInvokeActivityAsync\u003Eb__0\u003Ed.\u003C\u003Eo.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "IsCompleted", typeof (ReflectionBasedTaskActivity), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj4 = ReflectionBasedTaskActivity.\u003C\u003Ec__DisplayClass15_0.\u003C\u003CInvokeActivityAsync\u003Eb__0\u003Ed.\u003C\u003Eo.\u003C\u003Ep__1.Target((CallSite) ReflectionBasedTaskActivity.\u003C\u003Ec__DisplayClass15_0.\u003C\u003CInvokeActivityAsync\u003Eb__0\u003Ed.\u003C\u003Eo.\u003C\u003Ep__1, obj3);
          if (!target((CallSite) p2, obj4))
          {
            int num;
            // ISSUE: explicit reference operation
            // ISSUE: reference to a compiler-generated field
            (^this).\u003C\u003E1__state = num = 0;
            object obj = obj3;
            if (!(obj3 is ICriticalNotifyCompletion awaiter3))
            {
              INotifyCompletion awaiter2 = (INotifyCompletion) obj3;
              // ISSUE: explicit reference operation
              // ISSUE: reference to a compiler-generated field
              (^this).\u003C\u003Et__builder.AwaitOnCompleted<INotifyCompletion, ReflectionBasedTaskActivity.\u003C\u003Ec__DisplayClass15_0.\u003C\u003CInvokeActivityAsync\u003Eb__0\u003Ed>(ref awaiter2, this);
              awaiter2 = (INotifyCompletion) null;
            }
            else
            {
              // ISSUE: explicit reference operation
              // ISSUE: reference to a compiler-generated field
              (^this).\u003C\u003Et__builder.AwaitUnsafeOnCompleted<ICriticalNotifyCompletion, ReflectionBasedTaskActivity.\u003C\u003Ec__DisplayClass15_0.\u003C\u003CInvokeActivityAsync\u003Eb__0\u003Ed>(ref awaiter3, this);
            }
            awaiter3 = (ICriticalNotifyCompletion) null;
          }
          else
          {
            // ISSUE: reference to a compiler-generated field
            if (ReflectionBasedTaskActivity.\u003C\u003Ec__DisplayClass15_0.\u003C\u003CInvokeActivityAsync\u003Eb__0\u003Ed.\u003C\u003Eo.\u003C\u003Ep__3 == null)
            {
              // ISSUE: reference to a compiler-generated field
              ReflectionBasedTaskActivity.\u003C\u003Ec__DisplayClass15_0.\u003C\u003CInvokeActivityAsync\u003Eb__0\u003Ed.\u003C\u003Eo.\u003C\u003Ep__3 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "GetResult", (IEnumerable<Type>) null, typeof (ReflectionBasedTaskActivity), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
              {
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
              }));
            }
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            return ReflectionBasedTaskActivity.\u003C\u003Ec__DisplayClass15_0.\u003C\u003CInvokeActivityAsync\u003Eb__0\u003Ed.\u003C\u003Eo.\u003C\u003Ep__3.Target((CallSite) ReflectionBasedTaskActivity.\u003C\u003Ec__DisplayClass15_0.\u003C\u003CInvokeActivityAsync\u003Eb__0\u003Ed.\u003C\u003Eo.\u003C\u003Ep__3, obj3);
          }
        }
        else
        {
          await task2;
          return ReflectionBasedTaskActivity.s_voidResult;
        }
      }));
      return obj1 != ReflectionBasedTaskActivity.s_voidResult ? this.Serializer.Serialize(obj1) : string.Empty;
    }
  }
}
