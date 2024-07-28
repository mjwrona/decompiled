// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.WrappedException
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.WebApi
{
  [DataContract(IsReference = true)]
  public class WrappedException : ISecuredObject
  {
    private Type m_type;
    private const int c_backCompatVer = 14;
    private static object syncObject = new object();
    private static byte[] s_currentAssemblyPublicKeyToken = (byte[]) null;
    private static Version s_currentAssemblyVersion = (Version) null;
    private static HashSet<Assembly> s_assembliesCheckedForExceptionMappings = new HashSet<Assembly>();
    private static readonly byte[] s_testCodePublicKeyToken = new byte[8]
    {
      (byte) 104,
      (byte) 157,
      (byte) 92,
      (byte) 59,
      (byte) 25,
      (byte) 170,
      (byte) 230,
      (byte) 35
    };
    private static Dictionary<string, Tuple<Version, Type>> s_exceptionsWithAttributeMapping = new Dictionary<string, Tuple<Version, Type>>();
    private static readonly IDictionary<string, Type> baseTranslatedExceptions = (IDictionary<string, Type>) new Dictionary<string, Type>()
    {
      {
        "VssAccessCheckException",
        typeof (AccessCheckException)
      }
    };

    public WrappedException()
    {
    }

    public WrappedException(Exception exception, bool includeErrorDetail, Version restApiVersion)
    {
      if (exception is AggregateException && ((AggregateException) exception).InnerExceptions != null && ((AggregateException) exception).Flatten().InnerExceptions.Count == 1)
        exception = ((AggregateException) exception).Flatten().InnerException;
      Type exceptionType = exception.GetType();
      string typeName;
      string typeKey;
      if (exception is VssServiceResponseException)
      {
        exceptionType = typeof (VssServiceException);
        VssException.GetTypeNameAndKeyForExceptionType(exceptionType, restApiVersion, out typeName, out typeKey);
      }
      else if (exception is VssServiceException)
        ((VssServiceException) exception).GetTypeNameAndKey(restApiVersion, out typeName, out typeKey);
      else
        VssException.GetTypeNameAndKeyForExceptionType(exceptionType, restApiVersion, out typeName, out typeKey);
      this.Type = exceptionType;
      this.TypeName = typeName;
      this.TypeKey = typeKey;
      if (includeErrorDetail && exception.InnerException != null)
        this.InnerException = new WrappedException(exception.InnerException, includeErrorDetail, restApiVersion);
      this.Message = exception.Message;
      if (includeErrorDetail)
        this.StackTrace = exception.StackTrace ?? new System.Diagnostics.StackTrace(2, true).ToString();
      if (!string.IsNullOrWhiteSpace(exception.HelpLink))
        this.HelpLink = exception.HelpLink;
      if (exception is VssException)
      {
        this.EventId = ((VssException) exception).EventId;
        this.ErrorCode = ((VssException) exception).ErrorCode;
      }
      this.TryWrapCustomProperties(exception);
    }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public Dictionary<string, object> CustomProperties { get; set; }

    [DataMember]
    public WrappedException InnerException { get; set; }

    public Exception UnwrappedInnerException { get; set; }

    [DataMember]
    public string Message { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public string HelpLink { get; set; }

    public Type Type
    {
      get
      {
        if (this.m_type == (Type) null && !string.IsNullOrEmpty(this.TypeName))
          this.m_type = WrappedException.LoadType(this.TypeName);
        return this.m_type;
      }
      set => this.m_type = value;
    }

    [DataMember]
    public string TypeName { get; set; }

    [DataMember]
    public string TypeKey { get; set; }

    [DataMember]
    public int ErrorCode { get; set; }

    [DataMember]
    public int EventId { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public string StackTrace { get; set; }

    public Exception Unwrap(IDictionary<string, Type> typeMapping)
    {
      Exception innerException = (Exception) null;
      if (this.InnerException != null)
      {
        innerException = this.InnerException.Unwrap(typeMapping);
        this.UnwrappedInnerException = innerException;
      }
      Exception exception = (Exception) null;
      Type type;
      if (!string.IsNullOrEmpty(this.TypeKey) && (typeMapping != null && typeMapping.TryGetValue(this.TypeKey, out type) || WrappedException.baseTranslatedExceptions.TryGetValue(this.TypeKey, out type)))
      {
        try
        {
          this.Type = type;
          exception = Activator.CreateInstance(this.Type, (object) this.Message, (object) innerException) as Exception;
        }
        catch (Exception ex)
        {
        }
      }
      if (exception == null)
        exception = this.UnWrap(innerException);
      if (exception is VssException)
      {
        ((VssException) exception).EventId = this.EventId;
        ((VssException) exception).ErrorCode = this.ErrorCode;
      }
      if (exception == null && !string.IsNullOrEmpty(this.Message))
        exception = (Exception) new VssServiceException(this.Message, innerException);
      if (exception == null)
        string.IsNullOrEmpty(this.TypeName);
      if (exception != null && !string.IsNullOrEmpty(this.HelpLink))
        exception.HelpLink = this.HelpLink;
      if (exception != null && !string.IsNullOrEmpty(this.StackTrace))
      {
        FieldInfo declaredField = typeof (Exception).GetTypeInfo().GetDeclaredField("_stackTraceString");
        if (declaredField != (FieldInfo) null && !declaredField.Attributes.HasFlag((Enum) FieldAttributes.Public) && !declaredField.Attributes.HasFlag((Enum) FieldAttributes.Static))
          declaredField.SetValue((object) exception, (object) this.StackTrace);
      }
      if (exception != null && exception.GetType() == this.Type)
        this.TryUnWrapCustomProperties(exception);
      return exception;
    }

    private Exception UnWrap(Exception innerException)
    {
      Exception exception = (Exception) null;
      if (this.Type != (Type) null)
      {
        try
        {
          object[] parameters = (object[]) null;
          ConstructorInfo matchingConstructor = this.GetMatchingConstructor(typeof (WrappedException));
          if (matchingConstructor != (ConstructorInfo) null)
          {
            parameters = new object[1]{ (object) this };
          }
          else
          {
            matchingConstructor = this.GetMatchingConstructor(typeof (string), typeof (Exception));
            if (matchingConstructor != (ConstructorInfo) null)
            {
              parameters = new object[2]
              {
                (object) this.Message,
                (object) innerException
              };
            }
            else
            {
              matchingConstructor = this.GetMatchingConstructor(typeof (string));
              if (matchingConstructor != (ConstructorInfo) null)
                parameters = new object[1]
                {
                  (object) this.Message
                };
              else
                matchingConstructor = this.GetMatchingConstructor();
            }
          }
          if (matchingConstructor != (ConstructorInfo) null)
            exception = matchingConstructor.Invoke(parameters) as Exception;
        }
        catch (Exception ex)
        {
        }
      }
      return exception;
    }

    private ConstructorInfo GetMatchingConstructor(params Type[] parameterTypes) => this.Type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, (Binder) null, parameterTypes, (ParameterModifier[]) null);

    private static Type LoadType(string typeName)
    {
      Type type = (Type) null;
      try
      {
        type = Type.GetType(typeName, false, true);
      }
      catch (Exception ex)
      {
      }
      if (type == (Type) null)
      {
        type = WrappedException.LookupExceptionAttributeMapping(typeName);
        if (type == (Type) null)
        {
          try
          {
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            type = Type.GetType(typeName, WrappedException.\u003C\u003EO.\u003C0\u003E__ResolveAssembly ?? (WrappedException.\u003C\u003EO.\u003C0\u003E__ResolveAssembly = new Func<AssemblyName, Assembly>(WrappedException.ResolveAssembly)), (Func<Assembly, string, bool, Type>) null, false, true);
          }
          catch (Exception ex)
          {
          }
        }
      }
      return type;
    }

    private static Assembly ResolveAssembly(AssemblyName asmName)
    {
      if (asmName.Version == (Version) null || asmName.Version.Major <= 14)
      {
        AssemblyName assemblyRef = new AssemblyName()
        {
          Name = asmName.Name,
          CultureInfo = asmName.CultureInfo
        };
        assemblyRef.SetPublicKeyToken(asmName.GetPublicKeyToken());
        try
        {
          Assembly assembly = Assembly.Load(assemblyRef);
          if (assembly != (Assembly) null)
            return assembly;
        }
        catch (Exception ex)
        {
        }
      }
      string location = Assembly.GetExecutingAssembly().Location;
      if (!string.IsNullOrEmpty(location))
      {
        string str = Path.Combine(Path.GetDirectoryName(location), asmName.Name + ".dll");
        if (File.Exists(str))
          return Assembly.LoadFrom(str);
      }
      return (Assembly) null;
    }

    private static Type LookupExceptionAttributeMapping(string typeName)
    {
      Type type = (Type) null;
      Tuple<Version, Type> tuple = (Tuple<Version, Type>) null;
      lock (WrappedException.syncObject)
      {
        if (!WrappedException.s_exceptionsWithAttributeMapping.TryGetValue(typeName, out tuple))
        {
          WrappedException.UpdateExceptionAttributeMappingCache();
          WrappedException.s_exceptionsWithAttributeMapping.TryGetValue(typeName, out tuple);
        }
      }
      if (tuple != null)
        type = tuple.Item2;
      return type;
    }

    private static void UpdateExceptionAttributeMappingCache()
    {
      foreach (Assembly assembly in ((IEnumerable<Assembly>) AppDomain.CurrentDomain.GetAssemblies()).Where<Assembly>((Func<Assembly, bool>) (a => !WrappedException.s_assembliesCheckedForExceptionMappings.Contains(a))))
      {
        if (WrappedException.DoesAssemblyQualify(assembly))
        {
          try
          {
            IEnumerable<Type> types;
            try
            {
              types = (IEnumerable<Type>) assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
              types = ((IEnumerable<Type>) ex.Types).Where<Type>((Func<Type, bool>) (t => t != (Type) null));
            }
            foreach (TypeInfo element in types)
            {
              foreach (ExceptionMappingAttribute customAttribute in element.GetCustomAttributes<ExceptionMappingAttribute>())
              {
                Tuple<Version, Type> tuple;
                if (!WrappedException.s_exceptionsWithAttributeMapping.TryGetValue(customAttribute.TypeName, out tuple) || customAttribute.ExclusiveMaxApiVersion > tuple.Item1)
                  WrappedException.s_exceptionsWithAttributeMapping[customAttribute.TypeName] = new Tuple<Version, Type>(customAttribute.ExclusiveMaxApiVersion, element.AsType());
              }
            }
          }
          catch (Exception ex)
          {
          }
        }
        WrappedException.s_assembliesCheckedForExceptionMappings.Add(assembly);
      }
    }

    private static bool DoesAssemblyQualify(Assembly assembly)
    {
      if (WrappedException.s_currentAssemblyPublicKeyToken == null || WrappedException.s_currentAssemblyVersion == (Version) null)
      {
        AssemblyName name = typeof (WrappedException).GetTypeInfo().Assembly.GetName();
        WrappedException.s_currentAssemblyPublicKeyToken = name.GetPublicKeyToken();
        WrappedException.s_currentAssemblyVersion = name.Version;
      }
      AssemblyName name1 = assembly.GetName();
      if (name1.Version.Major != WrappedException.s_currentAssemblyVersion.Major)
        return false;
      byte[] publicKeyToken = name1.GetPublicKeyToken();
      return ArrayUtility.Equals(WrappedException.s_currentAssemblyPublicKeyToken, publicKeyToken) || ArrayUtility.Equals(WrappedException.s_testCodePublicKeyToken, publicKeyToken);
    }

    private void TryWrapCustomProperties(Exception exception)
    {
      IEnumerable<PropertyInfo> customPropertiesInfo = this.GetCustomPropertiesInfo();
      if (customPropertiesInfo.Any<PropertyInfo>())
        this.CustomProperties = new Dictionary<string, object>();
      foreach (PropertyInfo propertyInfo in customPropertiesInfo)
      {
        try
        {
          this.CustomProperties.Add(propertyInfo.Name, propertyInfo.GetValue((object) exception));
        }
        catch
        {
        }
      }
    }

    private void TryUnWrapCustomProperties(Exception exception)
    {
      if (this.CustomProperties == null)
        return;
      foreach (PropertyInfo propertyInfo in this.GetCustomPropertiesInfo())
      {
        if (this.CustomProperties.ContainsKey(propertyInfo.Name))
        {
          try
          {
            object obj = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(this.CustomProperties[propertyInfo.Name]), propertyInfo.PropertyType);
            propertyInfo.SetValue((object) exception, obj);
          }
          catch
          {
          }
        }
      }
    }

    private IEnumerable<PropertyInfo> GetCustomPropertiesInfo() => this.Type.GetTypeInfo().DeclaredProperties.Where<PropertyInfo>((Func<PropertyInfo, bool>) (p => p.GetMethod.Attributes.HasFlag((Enum) MethodAttributes.Public) && !p.GetMethod.Attributes.HasFlag((Enum) MethodAttributes.Static) && p.CustomAttributes.Any<CustomAttributeData>((Func<CustomAttributeData, bool>) (a => a.AttributeType.GetTypeInfo().IsAssignableFrom(typeof (DataMemberAttribute).GetTypeInfo())))));

    Guid ISecuredObject.NamespaceId => throw new NotImplementedException();

    int ISecuredObject.RequiredPermissions => throw new NotImplementedException();

    string ISecuredObject.GetToken() => throw new NotImplementedException();
  }
}
