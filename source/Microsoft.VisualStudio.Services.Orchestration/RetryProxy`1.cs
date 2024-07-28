// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.RetryProxy`1
// Assembly: Microsoft.VisualStudio.Services.Orchestration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C0C603F4-BE31-455B-860A-9FD3B046611C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.dll

using ImpromptuInterface;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Orchestration
{
  internal class RetryProxy<T> : DynamicObject
  {
    private OrchestrationContext context;
    private RetryOptions retryOptions;
    private T wrappedObject;
    private IDictionary<string, Type> returnTypes;

    public RetryProxy(OrchestrationContext context, RetryOptions retryOptions, T wrappedObject)
    {
      this.context = context;
      this.retryOptions = retryOptions;
      this.wrappedObject = wrappedObject;
      this.returnTypes = (IDictionary<string, Type>) ((IEnumerable<MethodInfo>) typeof (T).GetMethods()).Where<MethodInfo>((Func<MethodInfo, bool>) (method => !method.IsSpecialName)).GroupBy<MethodInfo, string>((Func<MethodInfo, string>) (method => method.Name)).Where<IGrouping<string, MethodInfo>>((Func<IGrouping<string, MethodInfo>, bool>) (group => group.Select<MethodInfo, Type>((Func<MethodInfo, Type>) (method => method.ReturnType)).Distinct<Type>().Count<Type>() == 1)).Select(group => new
      {
        Name = group.Key,
        ReturnType = group.Select<MethodInfo, Type>((Func<MethodInfo, Type>) (method => method.ReturnType)).Distinct<Type>().Single<Type>()
      }).ToDictionary(info => info.Name, info => info.ReturnType);
    }

    public override bool TryInvokeMember(
      InvokeMemberBinder binder,
      object[] args,
      out object result)
    {
      Type type = (Type) null;
      if (!this.returnTypes.TryGetValue(binder.Name, out type))
        throw new Exception("Method name '" + binder.Name + "' not known.");
      if (type.Equals(typeof (Task)))
      {
        result = (object) this.InvokeWithRetry<object>(binder.Name, args);
      }
      else
      {
        Type[] typeArray = type.IsGenericType ? type.GetGenericArguments() : throw new Exception("Return type is not a generic type. Type Name: " + type.FullName);
        MethodInfo methodInfo = typeArray.Length == 1 ? this.GetType().GetMethod("InvokeWithRetry").MakeGenericMethod(typeArray[0]) : throw new Exception("Generic Parameters are not equal to 1. Type Name: " + type.FullName);
        result = methodInfo.Invoke((object) this, new object[2]
        {
          (object) binder.Name,
          (object) args
        });
      }
      return true;
    }

    public async Task<ReturnType> InvokeWithRetry<ReturnType>(string methodName, object[] args) => await new RetryInterceptor<ReturnType>(this.context, this.retryOptions, (Func<Task<ReturnType>>) (() =>
    {
      // ISSUE: reference to a compiler-generated field
      if (RetryProxy<T>.\u003C\u003Eo__6<ReturnType>.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        RetryProxy<T>.\u003C\u003Eo__6<ReturnType>.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, Task<ReturnType>>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, typeof (Task<ReturnType>), typeof (RetryProxy<T>)));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return RetryProxy<T>.\u003C\u003Eo__6<ReturnType>.\u003C\u003Ep__0.Target((CallSite) RetryProxy<T>.\u003C\u003Eo__6<ReturnType>.\u003C\u003Ep__0, Impromptu.InvokeMember((object) this.\u003C\u003E4__this.wrappedObject, (String_OR_InvokeMemberName) methodName, args));
    })).Invoke();
  }
}
