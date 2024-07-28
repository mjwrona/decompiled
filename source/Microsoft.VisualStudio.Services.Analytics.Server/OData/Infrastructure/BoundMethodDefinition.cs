// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.BoundMethodDefinition
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.AspNet.OData.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  internal abstract class BoundMethodDefinition
  {
    public static BoundMethodDefinition Create(
      MethodInfo method,
      Dictionary<Type, string> entitySets,
      int min,
      int max)
    {
      Type returnType = method.ReturnType;
      BoundMethodDefinition methodDefinition = (BoundMethodDefinition) null;
      if (returnType.IsValueType || returnType == typeof (string))
      {
        methodDefinition = (BoundMethodDefinition) Activator.CreateInstance(typeof (BoundMethodDefinition.PrimitiveBoundMethod<>).MakeGenericType(returnType), (object) method, (object) min, (object) max);
      }
      else
      {
        Type key = typeof (IQueryable).IsAssignableFrom(returnType) && returnType.IsGenericType ? returnType.GetGenericArguments()[0] : throw new NotImplementedException(string.Format("Model bound method {0} with {1} return type not supported. Please, implement it there.", (object) method.Name, (object) returnType));
        string str;
        if (entitySets.TryGetValue(key, out str))
          methodDefinition = (BoundMethodDefinition) Activator.CreateInstance(typeof (BoundMethodDefinition.EntityCollectionBoundMethod<>).MakeGenericType(key), (object) method, (object) min, (object) max, (object) str);
      }
      foreach (ParameterInfo parameter in method.GetParameters())
        methodDefinition.Parameters.Add(new Tuple<string, Type>(parameter.Name, parameter.ParameterType));
      return methodDefinition;
    }

    private BoundMethodDefinition(MethodInfo method, int min, int max)
    {
      this.Method = method;
      this.Name = method.Name;
      this.Min = min;
      this.Max = max;
      this.Parameters = new List<Tuple<string, Type>>();
    }

    public MethodInfo Method { get; private set; }

    public string Name { get; private set; }

    public int Min { get; private set; }

    public int Max { get; private set; }

    public List<Tuple<string, Type>> Parameters { get; private set; }

    public FunctionConfiguration Generate<T>(
      EntityTypeConfiguration<T> entityType,
      ODataConventionModelBuilder builder)
      where T : class
    {
      FunctionConfiguration func = entityType.Function(this.Name);
      func.Namespace = "Microsoft.VisualStudio.Services.Analytics";
      foreach (Tuple<string, Type> parameter in this.Parameters)
        func.AddParameter(parameter.Item1, (IEdmTypeConfiguration) builder.AddEnumType(parameter.Item2));
      this.AddReturn(func);
      return func;
    }

    protected abstract void AddReturn(FunctionConfiguration func);

    private class PrimitiveBoundMethod<T> : BoundMethodDefinition
    {
      public PrimitiveBoundMethod(MethodInfo method, int min, int max)
        : base(method, min, max)
      {
      }

      protected override void AddReturn(FunctionConfiguration func) => func.Returns<T>();
    }

    private class EntityCollectionBoundMethod<T> : BoundMethodDefinition where T : class
    {
      private string _entitySet;

      public EntityCollectionBoundMethod(MethodInfo method, int min, int max, string entitySet)
        : base(method, min, max)
      {
        this._entitySet = entitySet;
      }

      protected override void AddReturn(FunctionConfiguration func) => func.ReturnsCollectionFromEntitySet<T>(this._entitySet);
    }
  }
}
