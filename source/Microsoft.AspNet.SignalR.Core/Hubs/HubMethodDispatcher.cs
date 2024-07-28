// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Hubs.HubMethodDispatcher
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.AspNet.SignalR.Hubs
{
  internal class HubMethodDispatcher
  {
    private HubMethodDispatcher.HubMethodExecutor _executor;

    public HubMethodDispatcher(MethodInfo methodInfo)
    {
      this._executor = HubMethodDispatcher.GetExecutor(methodInfo);
      this.MethodInfo = methodInfo;
    }

    public MethodInfo MethodInfo { get; private set; }

    public object Execute(IHub hub, object[] parameters) => this._executor(hub, parameters);

    private static HubMethodDispatcher.HubMethodExecutor GetExecutor(MethodInfo methodInfo)
    {
      ParameterExpression parameterExpression = Expression.Parameter(typeof (IHub), "hub");
      ParameterExpression array = Expression.Parameter(typeof (object[]), "parameters");
      List<Expression> arguments = new List<Expression>();
      ParameterInfo[] parameters = methodInfo.GetParameters();
      for (int index = 0; index < parameters.Length; ++index)
      {
        ParameterInfo parameterInfo = parameters[index];
        UnaryExpression unaryExpression = Expression.Convert((Expression) Expression.ArrayIndex((Expression) array, (Expression) Expression.Constant((object) index)), parameterInfo.ParameterType);
        arguments.Add((Expression) unaryExpression);
      }
      MethodCallExpression body = Expression.Call(!methodInfo.IsStatic ? (Expression) Expression.Convert((Expression) parameterExpression, methodInfo.ReflectedType) : (Expression) null, methodInfo, (IEnumerable<Expression>) arguments);
      return body.Type == typeof (void) ? HubMethodDispatcher.WrapVoidAction(Expression.Lambda<HubMethodDispatcher.VoidHubMethodExecutor>((Expression) body, parameterExpression, array).Compile()) : Expression.Lambda<HubMethodDispatcher.HubMethodExecutor>((Expression) Expression.Convert((Expression) body, typeof (object)), parameterExpression, array).Compile();
    }

    private static HubMethodDispatcher.HubMethodExecutor WrapVoidAction(
      HubMethodDispatcher.VoidHubMethodExecutor executor)
    {
      return (HubMethodDispatcher.HubMethodExecutor) ((hub, parameters) =>
      {
        executor(hub, parameters);
        return (object) null;
      });
    }

    private delegate object HubMethodExecutor(IHub hub, object[] parameters);

    private delegate void VoidHubMethodExecutor(IHub hub, object[] parameters);
  }
}
