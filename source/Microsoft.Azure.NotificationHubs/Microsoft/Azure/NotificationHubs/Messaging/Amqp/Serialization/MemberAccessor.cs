// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Serialization.MemberAccessor
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Serialization
{
  internal abstract class MemberAccessor
  {
    private readonly Type type;
    private Func<object, object> getter;
    private Action<object, object> setter;

    protected MemberAccessor(Type type) => this.type = type;

    public Type Type => this.type;

    public static MemberAccessor Create(MemberInfo memberInfo, bool requiresSetter)
    {
      if (memberInfo.MemberType == MemberTypes.Field)
        return (MemberAccessor) new MemberAccessor.FieldMemberAccessor((FieldInfo) memberInfo);
      return memberInfo.MemberType == MemberTypes.Property ? (MemberAccessor) new MemberAccessor.PropertyMemberAccessor((PropertyInfo) memberInfo, requiresSetter) : throw new NotSupportedException(memberInfo.MemberType.ToString());
    }

    public object Get(object container) => this.getter(container);

    public void Set(object container, object value) => this.setter(container, value);

    private static void EmitTypeConversion(ILGenerator generator, Type castType, bool isContainer)
    {
      if (castType == typeof (object))
        return;
      if (castType.IsValueType)
        generator.Emit(isContainer ? OpCodes.Unbox : OpCodes.Unbox_Any, castType);
      else
        generator.Emit(OpCodes.Castclass, castType);
    }

    private static void EmitCall(ILGenerator generator, MethodInfo method)
    {
      OpCode opcode = method.IsStatic || method.DeclaringType.IsValueType ? OpCodes.Call : OpCodes.Callvirt;
      generator.EmitCall(opcode, method, (Type[]) null);
    }

    private static string GetAccessorName(bool isGetter, string name) => (isGetter ? "get_" : "set_") + name;

    private sealed class FieldMemberAccessor : MemberAccessor
    {
      public FieldMemberAccessor(FieldInfo fieldInfo)
        : base(fieldInfo.FieldType)
      {
        this.InitializeGetter(fieldInfo);
        this.InitializeSetter(fieldInfo);
      }

      private void InitializeGetter(FieldInfo fieldInfo)
      {
        DynamicMethod dynamicMethod = new DynamicMethod(MemberAccessor.GetAccessorName(true, fieldInfo.Name), typeof (object), new Type[1]
        {
          typeof (object)
        }, true);
        ILGenerator ilGenerator = dynamicMethod.GetILGenerator();
        ilGenerator.Emit(OpCodes.Ldarg_0);
        MemberAccessor.EmitTypeConversion(ilGenerator, fieldInfo.DeclaringType, true);
        ilGenerator.Emit(OpCodes.Ldfld, fieldInfo);
        if (fieldInfo.FieldType.IsValueType)
          ilGenerator.Emit(OpCodes.Box, fieldInfo.FieldType);
        ilGenerator.Emit(OpCodes.Ret);
        this.getter = (Func<object, object>) dynamicMethod.CreateDelegate(typeof (Func<object, object>));
      }

      private void InitializeSetter(FieldInfo fieldInfo)
      {
        DynamicMethod dynamicMethod = new DynamicMethod(MemberAccessor.GetAccessorName(false, fieldInfo.Name), typeof (void), new Type[2]
        {
          typeof (object),
          typeof (object)
        }, true);
        ILGenerator ilGenerator = dynamicMethod.GetILGenerator();
        ilGenerator.Emit(OpCodes.Ldarg_0);
        MemberAccessor.EmitTypeConversion(ilGenerator, fieldInfo.DeclaringType, true);
        ilGenerator.Emit(OpCodes.Ldarg_1);
        MemberAccessor.EmitTypeConversion(ilGenerator, fieldInfo.FieldType, false);
        ilGenerator.Emit(OpCodes.Stfld, fieldInfo);
        ilGenerator.Emit(OpCodes.Ret);
        this.setter = (Action<object, object>) dynamicMethod.CreateDelegate(typeof (Action<object, object>));
      }
    }

    private sealed class PropertyMemberAccessor : MemberAccessor
    {
      public PropertyMemberAccessor(PropertyInfo propertyInfo, bool requiresSetter)
        : base(propertyInfo.PropertyType)
      {
        this.InitializeGetter(propertyInfo);
        this.InitializeSetter(propertyInfo, requiresSetter);
      }

      private void InitializeGetter(PropertyInfo propertyInfo)
      {
        DynamicMethod dynamicMethod = new DynamicMethod(MemberAccessor.GetAccessorName(true, propertyInfo.Name), typeof (object), new Type[1]
        {
          typeof (object)
        }, true);
        ILGenerator ilGenerator = dynamicMethod.GetILGenerator();
        ilGenerator.DeclareLocal(typeof (object));
        ilGenerator.Emit(OpCodes.Ldarg_0);
        MemberAccessor.EmitTypeConversion(ilGenerator, propertyInfo.DeclaringType, true);
        MemberAccessor.EmitCall(ilGenerator, propertyInfo.GetGetMethod(true));
        if (propertyInfo.PropertyType.IsValueType)
          ilGenerator.Emit(OpCodes.Box, propertyInfo.PropertyType);
        ilGenerator.Emit(OpCodes.Ret);
        this.getter = (Func<object, object>) dynamicMethod.CreateDelegate(typeof (Func<object, object>));
      }

      private void InitializeSetter(PropertyInfo propertyInfo, bool requiresSetter)
      {
        MethodInfo setMethod = propertyInfo.GetSetMethod(true);
        if (setMethod == (MethodInfo) null)
        {
          if (requiresSetter)
            throw new SerializationException("Property annotated with AmqpMemberAttribute must have a setter.");
        }
        else
        {
          DynamicMethod dynamicMethod = new DynamicMethod(MemberAccessor.GetAccessorName(false, propertyInfo.Name), typeof (void), new Type[2]
          {
            typeof (object),
            typeof (object)
          }, true);
          ILGenerator ilGenerator = dynamicMethod.GetILGenerator();
          ilGenerator.Emit(OpCodes.Ldarg_0);
          MemberAccessor.EmitTypeConversion(ilGenerator, propertyInfo.DeclaringType, true);
          ilGenerator.Emit(OpCodes.Ldarg_1);
          MemberAccessor.EmitTypeConversion(ilGenerator, propertyInfo.PropertyType, false);
          MemberAccessor.EmitCall(ilGenerator, setMethod);
          ilGenerator.Emit(OpCodes.Ret);
          this.setter = (Action<object, object>) dynamicMethod.CreateDelegate(typeof (Action<object, object>));
        }
      }
    }
  }
}
