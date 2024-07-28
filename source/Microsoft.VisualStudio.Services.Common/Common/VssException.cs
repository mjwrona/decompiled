// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.VssException
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security;

namespace Microsoft.VisualStudio.Services.Common
{
  [ExceptionMapping("0.0", "3.0", "VssException", "Microsoft.VisualStudio.Services.Common.VssException, Microsoft.VisualStudio.Services.Common, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public abstract class VssException : ApplicationException
  {
    private const string c_currentAssemblyMajorVersionString = "Version=19";
    private const string c_backCompatVersionString = "Version=14";
    private const int c_backCompatVersion = 14;
    private static Version s_backCompatExclusiveMaxVersion = new Version(3, 0);
    private bool m_logException;
    private bool m_reportException;
    private int m_errorCode;
    private EventLogEntryType m_logLevel = EventLogEntryType.Warning;
    private int m_eventId = 3000;
    public const int DefaultExceptionEventId = 3000;

    public VssException()
    {
    }

    public VssException(int errorCode)
      : this(errorCode, false)
    {
    }

    public VssException(int errorCode, bool logException)
    {
      this.ErrorCode = errorCode;
      this.LogException = logException;
    }

    public VssException(string message)
      : base(SecretUtility.ScrubSecrets(message))
    {
    }

    public VssException(string message, Exception innerException)
      : base(SecretUtility.ScrubSecrets(message), innerException)
    {
    }

    public VssException(string message, int errorCode, Exception innerException)
      : this(message, innerException)
    {
      this.ErrorCode = errorCode;
      this.LogException = false;
    }

    public VssException(string message, int errorCode)
      : this(message, errorCode, false)
    {
    }

    public VssException(string message, int errorCode, bool logException)
      : this(message)
    {
      this.ErrorCode = errorCode;
      this.LogException = logException;
    }

    public VssException(
      string message,
      int errorCode,
      bool logException,
      Exception innerException)
      : this(message, innerException)
    {
      this.ErrorCode = errorCode;
      this.LogException = logException;
    }

    protected VssException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      this.LogException = (bool) info.GetValue(nameof (m_logException), typeof (bool));
      this.ReportException = (bool) info.GetValue(nameof (m_reportException), typeof (bool));
      this.ErrorCode = (int) info.GetValue(nameof (m_errorCode), typeof (int));
      this.LogLevel = (EventLogEntryType) info.GetValue(nameof (m_logLevel), typeof (EventLogEntryType));
      this.EventId = (int) info.GetValue(nameof (m_eventId), typeof (int));
    }

    [SecurityCritical]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);
      info.AddValue("m_logException", this.LogException);
      info.AddValue("m_reportException", this.ReportException);
      info.AddValue("m_errorCode", this.ErrorCode);
      info.AddValue("m_logLevel", (object) this.LogLevel);
      info.AddValue("m_eventId", this.EventId);
    }

    public bool LogException
    {
      get => this.m_logException;
      set => this.m_logException = value;
    }

    public EventLogEntryType LogLevel
    {
      get => this.m_logLevel;
      set => this.m_logLevel = value;
    }

    public int ErrorCode
    {
      get => this.m_errorCode;
      set => this.m_errorCode = value;
    }

    public int EventId
    {
      get => this.m_eventId;
      set => this.m_eventId = value;
    }

    public bool ReportException
    {
      get => this.m_reportException;
      set => this.m_reportException = value;
    }

    internal static void GetTypeNameAndKeyForExceptionType(
      Type exceptionType,
      Version restApiVersion,
      out string typeName,
      out string typeKey)
    {
      typeName = (string) null;
      typeKey = exceptionType.Name;
      if (restApiVersion != (Version) null)
      {
        IEnumerable<ExceptionMappingAttribute> source = exceptionType.GetTypeInfo().GetCustomAttributes<ExceptionMappingAttribute>().Where<ExceptionMappingAttribute>((Func<ExceptionMappingAttribute, bool>) (ea => ea.MinApiVersion <= restApiVersion && ea.ExclusiveMaxApiVersion > restApiVersion));
        if (source.Any<ExceptionMappingAttribute>())
        {
          ExceptionMappingAttribute mappingAttribute = source.First<ExceptionMappingAttribute>();
          typeName = mappingAttribute.TypeName;
          typeKey = mappingAttribute.TypeKey;
        }
        else if (restApiVersion < VssException.s_backCompatExclusiveMaxVersion)
          typeName = VssException.GetBackCompatAssemblyQualifiedName(exceptionType);
      }
      if (typeName != null)
        return;
      AssemblyName name = exceptionType.GetTypeInfo().Assembly.GetName();
      if (name != null)
      {
        typeName = exceptionType.FullName + ", " + name.Name;
      }
      else
      {
        string fullName = exceptionType.GetTypeInfo().Assembly.FullName;
        string str = fullName.Substring(0, fullName.IndexOf(','));
        typeName = exceptionType.FullName + ", " + str;
      }
    }

    internal static string GetBackCompatAssemblyQualifiedName(Type type)
    {
      AssemblyName name = type.GetTypeInfo().Assembly.GetName();
      if (name == null)
        return type.AssemblyQualifiedName.Replace("Version=19", "Version=14");
      AssemblyName assemblyName = name;
      assemblyName.Version = new Version(14, 0, 0, 0);
      return Assembly.CreateQualifiedName(assemblyName.ToString(), type.FullName);
    }
  }
}
