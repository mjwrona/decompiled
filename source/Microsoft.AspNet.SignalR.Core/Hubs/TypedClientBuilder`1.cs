// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Hubs.TypedClientBuilder`1
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.Hubs
{
  internal static class TypedClientBuilder<T>
  {
    private const string ClientModuleName = "Microsoft.AspNet.SignalR.Hubs.TypedClientBuilder";
    private static Lazy<Func<IClientProxy, T>> _builder = new Lazy<Func<IClientProxy, T>>((Func<Func<IClientProxy, T>>) (() => TypedClientBuilder<T>.GenerateClientBuilder()));

    public static T Build(IClientProxy proxy) => TypedClientBuilder<T>._builder.Value(proxy);

    public static void Validate()
    {
      Func<IClientProxy, T> func = TypedClientBuilder<T>._builder.Value;
    }

    private static Func<IClientProxy, T> GenerateClientBuilder()
    {
      TypedClientBuilder<T>.VerifyInterface(typeof (T));
      Type clientType = TypedClientBuilder<T>.GenerateInterfaceImplementation(AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("Microsoft.AspNet.SignalR.Hubs.TypedClientBuilder"), AssemblyBuilderAccess.Run).DefineDynamicModule("Microsoft.AspNet.SignalR.Hubs.TypedClientBuilder"));
      return (Func<IClientProxy, T>) (proxy => (T) Activator.CreateInstance(clientType, (object) proxy));
    }

    private static Type GenerateInterfaceImplementation(ModuleBuilder moduleBuilder)
    {
      TypeBuilder type = moduleBuilder.DefineType("Microsoft.AspNet.SignalR.Hubs.TypedClientBuilder." + typeof (T).Name + "Impl", TypeAttributes.Public, typeof (object), new Type[1]
      {
        typeof (T)
      });
      FieldBuilder proxyField = type.DefineField("_proxy", typeof (IClientProxy), FieldAttributes.Private);
      TypedClientBuilder<T>.BuildConstructor(type, (FieldInfo) proxyField);
      foreach (MethodInfo allInterfaceMethod in TypedClientBuilder<T>.GetAllInterfaceMethods(typeof (T)))
        TypedClientBuilder<T>.BuildMethod(type, allInterfaceMethod, (FieldInfo) proxyField);
      return type.CreateType();
    }

    private static IEnumerable<MethodInfo> GetAllInterfaceMethods(Type interfaceType)
    {
      Type[] typeArray = interfaceType.GetInterfaces();
      int index;
      for (index = 0; index < typeArray.Length; ++index)
      {
        foreach (MethodInfo allInterfaceMethod in TypedClientBuilder<T>.GetAllInterfaceMethods(typeArray[index]))
          yield return allInterfaceMethod;
      }
      typeArray = (Type[]) null;
      MethodInfo[] methodInfoArray = interfaceType.GetMethods();
      for (index = 0; index < methodInfoArray.Length; ++index)
        yield return methodInfoArray[index];
      methodInfoArray = (MethodInfo[]) null;
    }

    private static void BuildConstructor(TypeBuilder type, FieldInfo proxyField)
    {
      MethodBuilder methodBuilder = type.DefineMethod(".ctor", MethodAttributes.Public | MethodAttributes.HideBySig);
      ConstructorInfo constructor = typeof (object).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, (Binder) null, new Type[0], (ParameterModifier[]) null);
      methodBuilder.SetReturnType(typeof (void));
      methodBuilder.SetParameters(typeof (IClientProxy));
      ILGenerator ilGenerator = methodBuilder.GetILGenerator();
      ilGenerator.Emit(OpCodes.Ldarg_0);
      ilGenerator.Emit(OpCodes.Call, constructor);
      ilGenerator.Emit(OpCodes.Ldarg_0);
      ilGenerator.Emit(OpCodes.Ldarg_1);
      ilGenerator.Emit(OpCodes.Stfld, proxyField);
      ilGenerator.Emit(OpCodes.Ret);
    }

    private static void BuildMethod(
      TypeBuilder type,
      MethodInfo interfaceMethodInfo,
      FieldInfo proxyField)
    {
      MethodAttributes attributes = MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.VtableLayoutMask;
      ParameterInfo[] parameters = interfaceMethodInfo.GetParameters();
      Type[] array1 = ((IEnumerable<ParameterInfo>) parameters).Select<ParameterInfo, Type>((Func<ParameterInfo, Type>) (param => param.ParameterType)).ToArray<Type>();
      MethodBuilder methodBuilder = type.DefineMethod(interfaceMethodInfo.Name, attributes);
      MethodInfo method = typeof (IClientProxy).GetMethod("Invoke", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, (Binder) null, new Type[2]
      {
        typeof (string),
        typeof (object[])
      }, (ParameterModifier[]) null);
      methodBuilder.SetReturnType(interfaceMethodInfo.ReturnType);
      methodBuilder.SetParameters(array1);
      string[] array2 = ((IEnumerable<Type>) array1).Where<Type>((Func<Type, bool>) (p => p.IsGenericParameter)).Select<Type, string>((Func<Type, string>) (p => p.Name)).Distinct<string>().ToArray<string>();
      if (((IEnumerable<string>) array2).Any<string>())
        methodBuilder.DefineGenericParameters(array2);
      ILGenerator ilGenerator = methodBuilder.GetILGenerator();
      ilGenerator.DeclareLocal(typeof (object[]));
      ilGenerator.Emit(OpCodes.Ldarg_0);
      ilGenerator.Emit(OpCodes.Ldfld, proxyField);
      ilGenerator.Emit(OpCodes.Ldstr, interfaceMethodInfo.Name);
      ilGenerator.Emit(OpCodes.Ldc_I4, parameters.Length);
      ilGenerator.Emit(OpCodes.Newarr, typeof (object));
      ilGenerator.Emit(OpCodes.Stloc_0);
      for (int index = 0; index < array1.Length; ++index)
      {
        ilGenerator.Emit(OpCodes.Ldloc_0);
        ilGenerator.Emit(OpCodes.Ldc_I4, index);
        ilGenerator.Emit(OpCodes.Ldarg, index + 1);
        ilGenerator.Emit(OpCodes.Box, array1[index]);
        ilGenerator.Emit(OpCodes.Stelem_Ref);
      }
      ilGenerator.Emit(OpCodes.Ldloc_0);
      ilGenerator.Emit(OpCodes.Callvirt, method);
      if (interfaceMethodInfo.ReturnType == typeof (void))
        ilGenerator.Emit(OpCodes.Pop);
      ilGenerator.Emit(OpCodes.Ret);
    }

    private static void VerifyInterface(Type interfaceType)
    {
      if (!interfaceType.IsInterface)
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_TypeMustBeInterface, new object[1]
        {
          (object) interfaceType.Name
        }));
      if (interfaceType.GetProperties().Length != 0)
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_TypeMustNotContainProperties, new object[1]
        {
          (object) interfaceType.Name
        }));
      if (interfaceType.GetEvents().Length != 0)
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_TypeMustNotContainEvents, new object[1]
        {
          (object) interfaceType.Name
        }));
      foreach (MethodInfo method in interfaceType.GetMethods())
        TypedClientBuilder<T>.VerifyMethod(interfaceType, method);
      foreach (Type interfaceType1 in interfaceType.GetInterfaces())
        TypedClientBuilder<T>.VerifyInterface(interfaceType1);
    }

    private static void VerifyMethod(Type interfaceType, MethodInfo interfaceMethod)
    {
      if (interfaceMethod.ReturnType != typeof (void) && interfaceMethod.ReturnType != typeof (Task))
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_MethodMustReturnVoidOrTask, new object[2]
        {
          (object) interfaceType.Name,
          (object) interfaceMethod.Name
        }));
      foreach (ParameterInfo parameter in interfaceMethod.GetParameters())
        TypedClientBuilder<T>.VerifyParameter(interfaceType, interfaceMethod, parameter);
    }

    private static void VerifyParameter(
      Type interfaceType,
      MethodInfo interfaceMethod,
      ParameterInfo parameter)
    {
      if (parameter.IsOut)
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_MethodMustNotTakeOutParameter, new object[3]
        {
          (object) parameter.Name,
          (object) interfaceType.Name,
          (object) interfaceMethod.Name
        }));
      if (parameter.ParameterType.IsByRef)
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_MethodMustNotTakeRefParameter, new object[3]
        {
          (object) parameter.Name,
          (object) interfaceType.Name,
          (object) interfaceMethod.Name
        }));
    }
  }
}
