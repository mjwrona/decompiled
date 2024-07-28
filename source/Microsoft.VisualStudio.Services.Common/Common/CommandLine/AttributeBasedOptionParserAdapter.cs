// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.CommandLine.AttributeBasedOptionParserAdapter
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using Microsoft.VisualStudio.Services.Common.CommandLine.Validation;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.Common.CommandLine
{
  public class AttributeBasedOptionParserAdapter
  {
    public const BindingFlags SupportedMemberBindings = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
    private static readonly Type defaultConversionType = typeof (string);
    private OptionParser optionParser;

    public AttributeBasedOptionParserAdapter(OptionParser parser) => this.optionParser = parser != null ? parser : throw new ArgumentNullException(nameof (parser));

    public T Parse<T>(IEnumerable<string> commandLine) where T : class, new() => this.Parse(Activator.CreateInstance(typeof (T)), commandLine, (IEnumerable<IOptionValidation>) null) as T;

    public T Parse<T>(
      IEnumerable<string> commandLine,
      IEnumerable<IOptionValidation> optionValidators)
      where T : class, new()
    {
      return this.Parse(Activator.CreateInstance(typeof (T)), commandLine, optionValidators) as T;
    }

    public object Parse(object attributedTypeInstance, IEnumerable<string> commandLine) => this.Parse(attributedTypeInstance, commandLine, (IEnumerable<IOptionValidation>) new Collection<IOptionValidation>()
    {
      (IOptionValidation) DefaultValidation.Instance
    });

    public object Parse(
      object attributedTypeInstance,
      IEnumerable<string> commandLine,
      IEnumerable<IOptionValidation> optionValidators)
    {
      if (commandLine == null)
        throw new ArgumentNullException(nameof (commandLine));
      ICollection<KeyValuePair<MemberInfo, OptionAttributeBase>> keyValuePairs = attributedTypeInstance != null ? AttributeBasedOptionParserAdapter.GetMembersWithAttributes(attributedTypeInstance.GetType()) : throw new ArgumentNullException(nameof (attributedTypeInstance));
      if (keyValuePairs.Any<KeyValuePair<MemberInfo, OptionAttributeBase>>())
      {
        AttributeBasedOptionParserAdapter.ValidateNoDuplicatePositionalAttributes(keyValuePairs);
        AttributeBasedOptionParserAdapter.ValidateCollectionMemberRequirements(keyValuePairs, attributedTypeInstance);
        ICollection<KeyValuePair<MemberInfo, Option>> membersWithAttributes = AttributeBasedOptionParserAdapter.CreateOptionsFromMembersWithAttributes(keyValuePairs);
        if (membersWithAttributes.Any<KeyValuePair<MemberInfo, Option>>())
        {
          IEnumerable<Argument> source = this.optionParser.Parse(commandLine, membersWithAttributes.Select<KeyValuePair<MemberInfo, Option>, Option>((Func<KeyValuePair<MemberInfo, Option>, Option>) (o => o.Value)), optionValidators);
          if (source != null && source.Any<Argument>())
          {
            foreach (Argument obj in source)
            {
              Argument argument = obj;
              MemberInfo key = membersWithAttributes.First<KeyValuePair<MemberInfo, Option>>((Func<KeyValuePair<MemberInfo, Option>, bool>) (entry => entry.Value.Name.Equals(argument.Option.Name))).Key;
              if (argument.Option is PositionalOption || argument.Option.AllowMultiple)
                AttributeBasedOptionParserAdapter.AddCollectionMemberValue(key, argument, attributedTypeInstance);
              else
                AttributeBasedOptionParserAdapter.SetMemberValue(key, argument, attributedTypeInstance);
            }
          }
        }
      }
      return attributedTypeInstance;
    }

    protected static ICollection<KeyValuePair<MemberInfo, Option>> CreateOptionsFromMembersWithAttributes(
      ICollection<KeyValuePair<MemberInfo, OptionAttributeBase>> attributedMembers)
    {
      if (attributedMembers == null)
        throw new ArgumentNullException(nameof (attributedMembers));
      Collection<KeyValuePair<MemberInfo, Option>> membersWithAttributes = new Collection<KeyValuePair<MemberInfo, Option>>();
      foreach (KeyValuePair<MemberInfo, OptionAttributeBase> attributedMember in (IEnumerable<KeyValuePair<MemberInfo, OptionAttributeBase>>) attributedMembers)
      {
        MemberInfo key = attributedMember.Key;
        OptionAttributeBase attribute = attributedMember.Value;
        if (string.IsNullOrWhiteSpace(attribute.Name))
          attribute.Name = key.Name;
        if (attribute.ConverterType == ValueConverter.None.GetType())
        {
          Type conversionType = AttributeBasedOptionParserAdapter.GetConversionType(key, attribute);
          IValueConvertible valueConverter = ValueConverter.None;
          if (conversionType != AttributeBasedOptionParserAdapter.defaultConversionType)
          {
            if (conversionType.IsEnum)
            {
              valueConverter = (IValueConvertible) new EnumConverter(conversionType);
            }
            else
            {
              valueConverter = ValueConverter.GetDefaultConverter(conversionType);
              if (valueConverter == null)
                throw new DefaultValueConverterNotFoundException(CommonResources.ErrorInvalidValueConverterOrNoDefaultFound((object) conversionType.Name));
            }
          }
          attribute.ConverterType = valueConverter.GetType();
          membersWithAttributes.Add(new KeyValuePair<MemberInfo, Option>(key, attribute.ToOption(valueConverter)));
        }
        else
          membersWithAttributes.Add(new KeyValuePair<MemberInfo, Option>(key, attribute.ToOption()));
      }
      return (ICollection<KeyValuePair<MemberInfo, Option>>) membersWithAttributes;
    }

    protected static ICollection<KeyValuePair<MemberInfo, OptionAttributeBase>> GetMembersWithAttributes(
      Type type)
    {
      if (type == (Type) null)
        throw new ArgumentNullException(nameof (type));
      Collection<KeyValuePair<MemberInfo, OptionAttributeBase>> membersWithAttributes = new Collection<KeyValuePair<MemberInfo, OptionAttributeBase>>();
      foreach (MemberInfo member in type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy))
      {
        OptionAttributeBase optionAttribute = AttributeBasedOptionParserAdapter.GetOptionAttribute(member);
        if (optionAttribute != null)
        {
          if (string.IsNullOrWhiteSpace(optionAttribute.Name))
            optionAttribute.Name = member.Name;
          membersWithAttributes.Add(new KeyValuePair<MemberInfo, OptionAttributeBase>(member, optionAttribute));
        }
      }
      return (ICollection<KeyValuePair<MemberInfo, OptionAttributeBase>>) membersWithAttributes;
    }

    private static void AddCollectionMemberValue(
      MemberInfo member,
      Argument argument,
      object classInstance)
    {
      object instance;
      MethodInfo method;
      if (member.MemberType == MemberTypes.Property)
      {
        PropertyInfo propertyInfo = member as PropertyInfo;
        instance = propertyInfo.GetValue(classInstance);
        if (instance == null)
        {
          instance = Activator.CreateInstance(propertyInfo.PropertyType);
          propertyInfo.SetValue(classInstance, instance);
        }
        method = propertyInfo.PropertyType.GetMethod("Add");
      }
      else
      {
        FieldInfo fieldInfo = member as FieldInfo;
        instance = fieldInfo.GetValue(classInstance);
        if (instance == null)
        {
          instance = Activator.CreateInstance(fieldInfo.FieldType);
          fieldInfo.SetValue(classInstance, instance);
        }
        method = fieldInfo.FieldType.GetMethod("Add");
      }
      method.Invoke(instance, new object[1]
      {
        argument.Value
      });
    }

    private static OptionAttributeBase GetOptionAttribute(MemberInfo member)
    {
      OptionAttributeBase optionAttribute = (OptionAttributeBase) null;
      if (member.CustomAttributes != null && member.CustomAttributes.Any<CustomAttributeData>())
        optionAttribute = member.GetCustomAttribute<OptionAttributeBase>(true);
      return optionAttribute;
    }

    private static Type GetConversionType(MemberInfo member, OptionAttributeBase attribute)
    {
      Type conversionType = (Type) null;
      if (OptionAttributeBase.RequiresCollectionMember(attribute))
      {
        Type type = (Type) null;
        if (member.MemberType == MemberTypes.Field)
          type = (member as FieldInfo).FieldType;
        else if (member.MemberType == MemberTypes.Property)
          type = (member as PropertyInfo).PropertyType;
        conversionType = !type.IsGenericType ? AttributeBasedOptionParserAdapter.defaultConversionType : ((IEnumerable<Type>) type.GetGenericArguments()).First<Type>();
      }
      else if (member.MemberType == MemberTypes.Field)
        conversionType = (member as FieldInfo).FieldType;
      else if (member.MemberType == MemberTypes.Property)
        conversionType = (member as PropertyInfo).PropertyType;
      return conversionType;
    }

    private static void SetMemberValue(MemberInfo member, Argument argument, object classInstance)
    {
      if (member.MemberType == MemberTypes.Property)
      {
        PropertyInfo propertyInfo = member as PropertyInfo;
        if (argument.Option.ArgumentType == OptionArgumentType.None)
        {
          if (!propertyInfo.PropertyType.Equals(typeof (bool)))
            throw new OptionValueConversionException(CommonResources.ErrorOptionFlagRequiresBooleanMember((object) argument.Option.Name));
          propertyInfo.SetValue(classInstance, (object) true);
        }
        else
          propertyInfo.SetValue(classInstance, argument.Value);
      }
      else
      {
        FieldInfo fieldInfo = member as FieldInfo;
        if (argument.Option.ArgumentType == OptionArgumentType.None)
        {
          if (!fieldInfo.FieldType.Equals(typeof (bool)))
            throw new OptionValueConversionException(CommonResources.ErrorOptionFlagRequiresBooleanMember((object) argument.Option.Name));
        }
        else
          fieldInfo.SetValue(classInstance, argument.Value);
      }
    }

    private static void ValidateCollectionMemberRequirements(
      ICollection<KeyValuePair<MemberInfo, OptionAttributeBase>> attributedMembers,
      object classInstance)
    {
      foreach (KeyValuePair<MemberInfo, OptionAttributeBase> attributedMember in (IEnumerable<KeyValuePair<MemberInfo, OptionAttributeBase>>) attributedMembers)
      {
        MemberInfo key = attributedMember.Key;
        OptionAttributeBase attribute = attributedMember.Value;
        if (OptionAttributeBase.RequiresCollectionMember(attribute))
        {
          bool flag = false;
          try
          {
            if (key.MemberType == MemberTypes.Property)
            {
              PropertyInfo propertyInfo = key as PropertyInfo;
              IEnumerable enumerable = (IEnumerable) propertyInfo.GetValue(classInstance);
              if ((MemberInfo) propertyInfo.PropertyType.GetMethod("Add") != (MemberInfo) null)
                flag = true;
            }
            else if (key.MemberType == MemberTypes.Field)
            {
              FieldInfo fieldInfo = key as FieldInfo;
              IEnumerable enumerable = (IEnumerable) fieldInfo.GetValue(classInstance);
              if ((MemberInfo) fieldInfo.FieldType.GetMethod("Add") != (MemberInfo) null)
                flag = true;
            }
          }
          catch (InvalidCastException ex)
          {
            flag = false;
          }
          if (!flag)
          {
            if (attribute is PositionalOptionAttribute)
              throw new OptionValueConversionException(CommonResources.ErrorMembersContainingPositionalsRequireCollection());
            throw new OptionValueConversionException(CommonResources.ErrorOptionsAllowingMultiplesRequireCollection((object) attribute.Name));
          }
        }
      }
    }

    private static void ValidateNoDuplicatePositionalAttributes(
      ICollection<KeyValuePair<MemberInfo, OptionAttributeBase>> attributedMembers)
    {
      if (attributedMembers == null)
        return;
      int num = 0;
      foreach (KeyValuePair<MemberInfo, OptionAttributeBase> attributedMember in (IEnumerable<KeyValuePair<MemberInfo, OptionAttributeBase>>) attributedMembers)
      {
        if (attributedMember.Value is PositionalOptionAttribute)
          ++num;
      }
      if (num > 1)
        throw new DuplicatePositionalOptionAttributeException(CommonResources.ErrorDuplicatePositionalOptionAttributes((object) typeof (PositionalOptionAttribute).Name));
    }
  }
}
