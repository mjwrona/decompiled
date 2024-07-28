// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.CommandLine.AttributeBasedOperationModeHandlerFactory
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.Common.CommandLine
{
  public class AttributeBasedOperationModeHandlerFactory : OperationHandlerFactory
  {
    private static ICollection<KeyValuePair<OperationModeAttribute, Type>> handlerTypes = (ICollection<KeyValuePair<OperationModeAttribute, Type>>) null;
    private static readonly object staticSyncRoot = new object();
    private IEnumerable<Assembly> operationHandlerAssemblies;

    public AttributeBasedOperationModeHandlerFactory()
      : this(Assembly.GetCallingAssembly())
    {
    }

    public AttributeBasedOperationModeHandlerFactory(Assembly assembly) => this.operationHandlerAssemblies = !(assembly == (Assembly) null) ? (IEnumerable<Assembly>) new List<Assembly>()
    {
      assembly
    } : throw new ArgumentNullException(nameof (assembly));

    public AttributeBasedOperationModeHandlerFactory(IEnumerable<Assembly> assemblies) => this.operationHandlerAssemblies = assemblies != null ? (IEnumerable<Assembly>) new List<Assembly>(assemblies) : throw new ArgumentNullException(nameof (assemblies));

    public override OperationHandler CreateHandler(IEnumerable<string> args) => this.CreateHandler(args, (object[]) null);

    public override OperationHandler CreateHandler(
      IEnumerable<string> args,
      params object[] constructorArgs)
    {
      Type handlerType = (Type) null;
      ICollection<KeyValuePair<OperationModeAttribute, Type>> attributedHandlerTypes = this.GetAttributedHandlerTypes();
      if (attributedHandlerTypes.Any<KeyValuePair<OperationModeAttribute, Type>>())
      {
        if (args != null)
          handlerType = this.GetHandler(args, attributedHandlerTypes);
        if (handlerType == (Type) null)
          handlerType = this.GetDefaultHandler(attributedHandlerTypes);
      }
      return !(handlerType == (Type) null) ? AttributeBasedOperationModeHandlerFactory.CreateOperationHandler(handlerType, constructorArgs) : throw new OperationHandlerNotFoundException(CommonResources.ErrorOperationHandlerNotFound());
    }

    protected Type GetHandler(
      IEnumerable<string> args,
      ICollection<KeyValuePair<OperationModeAttribute, Type>> attributedHandlerTypes)
    {
      Collection<KeyValuePair<OperationModeAttribute, Type>> source = new Collection<KeyValuePair<OperationModeAttribute, Type>>();
      if (attributedHandlerTypes != null && attributedHandlerTypes.Any<KeyValuePair<OperationModeAttribute, Type>>())
      {
        foreach (KeyValuePair<OperationModeAttribute, Type> attributedHandlerType in (IEnumerable<KeyValuePair<OperationModeAttribute, Type>>) attributedHandlerTypes)
        {
          if (AttributeBasedOperationModeHandlerFactory.IsMatch(attributedHandlerType.Key, args))
            source.Add(attributedHandlerType);
        }
      }
      Type handler = (Type) null;
      if (source.Count > 0)
      {
        if (source.Count == 1)
        {
          handler = source.First<KeyValuePair<OperationModeAttribute, Type>>().Value;
        }
        else
        {
          Type type = (Type) null;
          int num = 0;
          foreach (KeyValuePair<OperationModeAttribute, Type> keyValuePair in source)
          {
            int length = keyValuePair.Key.Name.Replace(" ", string.Empty).Length;
            if (length > 0)
            {
              if (length == num)
                throw new DuplicateOperationHandlerException(CommonResources.ErrorDuplicateOperationModeHandlerFound());
              if (length > num)
              {
                type = keyValuePair.Value;
                num = length;
              }
            }
          }
          handler = type;
        }
      }
      return handler;
    }

    protected virtual Type GetDefaultHandler(
      ICollection<KeyValuePair<OperationModeAttribute, Type>> attributedHandlerTypes)
    {
      Type defaultHandler = (Type) null;
      if (attributedHandlerTypes != null && attributedHandlerTypes.Any<KeyValuePair<OperationModeAttribute, Type>>())
      {
        foreach (KeyValuePair<OperationModeAttribute, Type> attributedHandlerType in (IEnumerable<KeyValuePair<OperationModeAttribute, Type>>) attributedHandlerTypes)
        {
          if (attributedHandlerType.Key.IsDefault)
            defaultHandler = !(defaultHandler != (Type) null) ? attributedHandlerType.Value : throw new DuplicateOperationHandlerException(CommonResources.ErrorDuplicateDefaultOperationModeHandlerFound());
        }
      }
      return defaultHandler;
    }

    private ICollection<KeyValuePair<OperationModeAttribute, Type>> GetAttributedHandlerTypes()
    {
      if (AttributeBasedOperationModeHandlerFactory.handlerTypes == null)
      {
        lock (AttributeBasedOperationModeHandlerFactory.staticSyncRoot)
        {
          if (AttributeBasedOperationModeHandlerFactory.handlerTypes == null)
          {
            AttributeBasedOperationModeHandlerFactory.handlerTypes = (ICollection<KeyValuePair<OperationModeAttribute, Type>>) new Collection<KeyValuePair<OperationModeAttribute, Type>>();
            foreach (Assembly operationHandlerAssembly in this.operationHandlerAssemblies)
            {
              foreach (Type type in operationHandlerAssembly.GetTypes())
              {
                OperationModeAttribute operationModeAttribute = AttributeBasedOperationModeHandlerFactory.GetOperationModeAttribute(type);
                if (operationModeAttribute != null)
                  AttributeBasedOperationModeHandlerFactory.handlerTypes.Add(new KeyValuePair<OperationModeAttribute, Type>(operationModeAttribute, type));
              }
            }
          }
        }
      }
      return AttributeBasedOperationModeHandlerFactory.handlerTypes;
    }

    private static OperationHandler CreateOperationHandler(
      Type handlerType,
      params object[] constructorArgs)
    {
      OperationHandler operationHandler = (OperationHandler) null;
      ConstructorInfo[] constructors = handlerType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      int length = constructorArgs != null ? constructorArgs.Length : 0;
      foreach (ConstructorInfo constructorInfo in constructors)
      {
        ParameterInfo[] parameters = constructorInfo.GetParameters();
        if (parameters.Length == length)
        {
          if (length == 0)
          {
            operationHandler = (OperationHandler) constructorInfo.Invoke((object[]) null);
            break;
          }
          bool flag = true;
          for (int index = 0; index < length; ++index)
          {
            Type parameterType = ((IEnumerable<ParameterInfo>) parameters).ElementAt<ParameterInfo>(index).ParameterType;
            Type type = constructorArgs[index].GetType();
            if (type != parameterType && !parameterType.IsAssignableFrom(type))
            {
              flag = false;
              break;
            }
          }
          if (flag)
          {
            operationHandler = (OperationHandler) constructorInfo.Invoke(constructorArgs);
            break;
          }
        }
      }
      return operationHandler != null ? operationHandler : throw new OperationHandlerNotFoundException(CommonResources.ErrorOperationHandlerConstructorNotFound((object) handlerType.FullName));
    }

    private static OperationModeAttribute GetOperationModeAttribute(Type assemblyType)
    {
      OperationModeAttribute operationModeAttribute1 = (OperationModeAttribute) null;
      if (assemblyType != (Type) null)
      {
        object[] customAttributes = assemblyType.GetCustomAttributes(false);
        if (customAttributes != null)
        {
          foreach (object obj in customAttributes)
          {
            if (obj is OperationModeAttribute operationModeAttribute2)
            {
              operationModeAttribute1 = operationModeAttribute2;
              break;
            }
          }
        }
      }
      return operationModeAttribute1;
    }

    private static bool IsMatch(OperationModeAttribute modeAttribute, IEnumerable<string> args)
    {
      bool flag = false;
      if (modeAttribute != null && args != null && args.Any<string>())
      {
        IEnumerable<string> source = args.Where<string>((Func<string, bool>) (arg => !Option.HasSwitch(arg)));
        if (!string.IsNullOrWhiteSpace(modeAttribute.Name) && !modeAttribute.IsDefault)
        {
          string[] strArray = modeAttribute.Name.Split(new char[1]
          {
            ' '
          }, StringSplitOptions.RemoveEmptyEntries);
          int num = source.Count<string>();
          if (num >= strArray.Length)
          {
            for (int index = 0; index < num; ++index)
            {
              string str = args.ElementAt<string>(index);
              if (str != null)
              {
                if (index < strArray.Length)
                  flag = string.Equals(strArray[index].Trim(), str.Trim(), modeAttribute.CaseSensitivity);
                if (!flag)
                  break;
              }
            }
          }
        }
      }
      return flag;
    }
  }
}
