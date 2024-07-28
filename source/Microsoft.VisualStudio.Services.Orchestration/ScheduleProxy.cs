// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.ScheduleProxy
// Assembly: Microsoft.VisualStudio.Services.Orchestration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C0C603F4-BE31-455B-860A-9FD3B046611C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.dll

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Orchestration
{
  internal class ScheduleProxy : DynamicObject
  {
    private OrchestrationContext context;
    private Type interfaze;
    private IDictionary<string, Type> returnTypes;
    private bool useFullyQualifiedMethodNames;
    private string activityDispatcherType;

    public ScheduleProxy(OrchestrationContext context, Type @interface)
      : this(context, @interface, false)
    {
    }

    public ScheduleProxy(
      OrchestrationContext context,
      Type @interface,
      bool useFullyQualifiedMethodNames)
      : this(context, @interface, useFullyQualifiedMethodNames, (string) null)
    {
    }

    public ScheduleProxy(
      OrchestrationContext context,
      Type @interface,
      bool useFullyQualifiedMethodNames,
      string dispatcherType)
    {
      this.context = context;
      this.interfaze = @interface;
      this.useFullyQualifiedMethodNames = useFullyQualifiedMethodNames;
      if (string.IsNullOrEmpty(dispatcherType))
      {
        ActivityDispatcherTypeAttribute customAttribute = @interface.GetCustomAttribute<ActivityDispatcherTypeAttribute>();
        if (customAttribute != null)
          this.activityDispatcherType = customAttribute.DispatcherType;
      }
      else
        this.activityDispatcherType = dispatcherType;
      this.returnTypes = (IDictionary<string, Type>) ((IEnumerable<MethodInfo>) this.interfaze.GetMethods()).Where<MethodInfo>((Func<MethodInfo, bool>) (method => !method.IsSpecialName)).GroupBy<MethodInfo, string>((Func<MethodInfo, string>) (method => NameVersionHelper.GetDefaultName((object) method))).Where<IGrouping<string, MethodInfo>>((Func<IGrouping<string, MethodInfo>, bool>) (group => group.Select<MethodInfo, Type>((Func<MethodInfo, Type>) (method => method.ReturnType)).Distinct<Type>().Count<Type>() == 1)).Select(group => new
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
      string name = this.useFullyQualifiedMethodNames ? NameVersionHelper.GetFullyQualifiedMethodName(this.interfaze.Name, NameVersionHelper.GetDefaultName((object) binder)) : NameVersionHelper.GetDefaultName((object) binder);
      if (type.Equals(typeof (Task)))
      {
        result = (object) this.context.ScheduleTask<object>(name, NameVersionHelper.GetDefaultVersion((object) binder), this.activityDispatcherType, args);
      }
      else
      {
        Type[] typeArray = type.IsGenericType ? type.GetGenericArguments() : throw new Exception("Return type is not a generic type. Type Name: " + type.FullName);
        if (typeArray.Length != 1)
          throw new Exception("Generic Parameters are not equal to 1. Type Name: " + type.FullName);
        MethodInfo methodInfo = typeof (OrchestrationContext).GetMethod("ScheduleTask", new Type[4]
        {
          typeof (string),
          typeof (string),
          typeof (string),
          typeof (object[])
        }).MakeGenericMethod(typeArray[0]);
        result = methodInfo.Invoke((object) this.context, new object[4]
        {
          (object) name,
          (object) NameVersionHelper.GetDefaultVersion((object) binder),
          (object) this.activityDispatcherType,
          (object) args
        });
      }
      return true;
    }
  }
}
