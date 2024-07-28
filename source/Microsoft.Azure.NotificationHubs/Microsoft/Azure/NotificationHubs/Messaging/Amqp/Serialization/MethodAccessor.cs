// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Serialization.MethodAccessor
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Serialization
{
  internal abstract class MethodAccessor
  {
    private static readonly Type[] delegateParamsType = new Type[2]
    {
      typeof (object),
      typeof (object[])
    };
    private bool isStatic;
    private MethodAccessor.MethodDelegate methodDelegate;

    public static MethodAccessor Create(MethodInfo methodInfo) => (MethodAccessor) new MethodAccessor.TypeMethodAccessor(methodInfo);

    public static MethodAccessor Create(ConstructorInfo constructorInfo) => (MethodAccessor) new MethodAccessor.ConstructorAccessor(constructorInfo);

    public object Invoke(object[] parameters)
    {
      if (!this.isStatic)
        throw new InvalidOperationException("Instance required to call an instance method.");
      return this.Invoke((object) null, parameters);
    }

    public object Invoke(object container, object[] parameters)
    {
      if (this.isStatic && container != null)
        throw new InvalidOperationException("Static method must be called with null instance.");
      return this.methodDelegate(container, parameters);
    }

    private Type[] GetParametersType(ParameterInfo[] paramsInfo)
    {
      Type[] parametersType = new Type[paramsInfo.Length];
      for (int index = 0; index < paramsInfo.Length; ++index)
        parametersType[index] = paramsInfo[index].ParameterType.IsByRef ? paramsInfo[index].ParameterType.GetElementType() : paramsInfo[index].ParameterType;
      return parametersType;
    }

    private void LoadArguments(ILGenerator generator, Type[] paramsType)
    {
      for (int index = 0; index < paramsType.Length; ++index)
      {
        generator.Emit(OpCodes.Ldarg_1);
        switch (index)
        {
          case 0:
            generator.Emit(OpCodes.Ldc_I4_0);
            break;
          case 1:
            generator.Emit(OpCodes.Ldc_I4_1);
            break;
          case 2:
            generator.Emit(OpCodes.Ldc_I4_2);
            break;
          case 3:
            generator.Emit(OpCodes.Ldc_I4_3);
            break;
          case 4:
            generator.Emit(OpCodes.Ldc_I4_4);
            break;
          case 5:
            generator.Emit(OpCodes.Ldc_I4_5);
            break;
          case 6:
            generator.Emit(OpCodes.Ldc_I4_6);
            break;
          case 7:
            generator.Emit(OpCodes.Ldc_I4_7);
            break;
          case 8:
            generator.Emit(OpCodes.Ldc_I4_8);
            break;
          default:
            generator.Emit(OpCodes.Ldc_I4, index);
            break;
        }
        generator.Emit(OpCodes.Ldelem_Ref);
        if (paramsType[index].IsValueType)
          generator.Emit(OpCodes.Unbox_Any, paramsType[index]);
        else if (paramsType[index] != typeof (object))
          generator.Emit(OpCodes.Castclass, paramsType[index]);
      }
    }

    private delegate object MethodDelegate(object container, object[] parameters);

    private sealed class ConstructorAccessor : MethodAccessor
    {
      public ConstructorAccessor(ConstructorInfo constructorInfo)
      {
        this.isStatic = true;
        DynamicMethod dynamicMethod = new DynamicMethod("ctor_" + constructorInfo.DeclaringType.Name, typeof (object), MethodAccessor.delegateParamsType, true);
        Type[] parametersType = this.GetParametersType(constructorInfo.GetParameters());
        ILGenerator ilGenerator = dynamicMethod.GetILGenerator();
        this.LoadArguments(ilGenerator, parametersType);
        ilGenerator.Emit(OpCodes.Newobj, constructorInfo);
        if (constructorInfo.DeclaringType.IsValueType)
          ilGenerator.Emit(OpCodes.Box, constructorInfo.DeclaringType);
        ilGenerator.Emit(OpCodes.Ret);
        this.methodDelegate = (MethodAccessor.MethodDelegate) dynamicMethod.CreateDelegate(typeof (MethodAccessor.MethodDelegate));
      }
    }

    private sealed class TypeMethodAccessor : MethodAccessor
    {
      public TypeMethodAccessor(MethodInfo methodInfo)
      {
        Type[] parametersType = this.GetParametersType(methodInfo.GetParameters());
        DynamicMethod dynamicMethod = new DynamicMethod("method_" + methodInfo.Name, typeof (object), MethodAccessor.delegateParamsType, true);
        ILGenerator ilGenerator = dynamicMethod.GetILGenerator();
        if (!this.isStatic)
        {
          ilGenerator.Emit(OpCodes.Ldarg_0);
          if (methodInfo.DeclaringType.IsValueType)
            ilGenerator.Emit(OpCodes.Unbox_Any, methodInfo.DeclaringType);
          else
            ilGenerator.Emit(OpCodes.Castclass, methodInfo.DeclaringType);
        }
        this.LoadArguments(ilGenerator, parametersType);
        if (methodInfo.IsFinal)
          ilGenerator.Emit(OpCodes.Call, methodInfo);
        else
          ilGenerator.Emit(OpCodes.Callvirt, methodInfo);
        if (methodInfo.ReturnType == typeof (void))
          ilGenerator.Emit(OpCodes.Ldnull);
        else if (methodInfo.ReturnType.IsValueType)
          ilGenerator.Emit(OpCodes.Box, methodInfo.ReturnType);
        ilGenerator.Emit(OpCodes.Ret);
        this.methodDelegate = (MethodAccessor.MethodDelegate) dynamicMethod.CreateDelegate(typeof (MethodAccessor.MethodDelegate));
      }
    }
  }
}
