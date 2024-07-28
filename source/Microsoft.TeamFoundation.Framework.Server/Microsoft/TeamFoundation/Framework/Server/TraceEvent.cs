// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TraceEvent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public struct TraceEvent
  {
    public TraceEvent()
    {
      // ISSUE: reference to a compiler-generated field
      this.\u003CFormat\u003Ek__BackingField = (string) null;
      // ISSUE: reference to a compiler-generated field
      this.\u003CArgs\u003Ek__BackingField = new TraceEvent.ArgumentHolder();
      // ISSUE: reference to a compiler-generated field
      this.\u003Cm_message\u003Ek__BackingField = (string) null;
      // ISSUE: reference to a compiler-generated field
      this.\u003CTraceId\u003Ek__BackingField = new Guid();
      // ISSUE: reference to a compiler-generated field
      this.\u003CTracepoint\u003Ek__BackingField = 0;
      // ISSUE: reference to a compiler-generated field
      this.\u003CUserLogin\u003Ek__BackingField = (string) null;
      // ISSUE: reference to a compiler-generated field
      this.\u003CVSID\u003Ek__BackingField = new Guid();
      // ISSUE: reference to a compiler-generated field
      this.\u003CCUID\u003Ek__BackingField = new Guid();
      // ISSUE: reference to a compiler-generated field
      this.\u003CTenantId\u003Ek__BackingField = new Guid();
      // ISSUE: reference to a compiler-generated field
      this.\u003CProviderId\u003Ek__BackingField = new Guid();
      // ISSUE: reference to a compiler-generated field
      this.\u003CService\u003Ek__BackingField = (string) null;
      // ISSUE: reference to a compiler-generated field
      this.\u003CMethod\u003Ek__BackingField = (string) null;
      // ISSUE: reference to a compiler-generated field
      this.\u003CLevel\u003Ek__BackingField = TraceLevel.Off;
      // ISSUE: reference to a compiler-generated field
      this.\u003CArea\u003Ek__BackingField = (string) null;
      // ISSUE: reference to a compiler-generated field
      this.\u003CLayer\u003Ek__BackingField = (string) null;
      // ISSUE: reference to a compiler-generated field
      this.\u003CUserAgent\u003Ek__BackingField = (string) null;
      // ISSUE: reference to a compiler-generated field
      this.\u003CUri\u003Ek__BackingField = (string) null;
      // ISSUE: reference to a compiler-generated field
      this.\u003CPath\u003Ek__BackingField = (string) null;
      // ISSUE: reference to a compiler-generated field
      this.\u003CServiceHost\u003Ek__BackingField = new Guid();
      // ISSUE: reference to a compiler-generated field
      this.\u003CTags\u003Ek__BackingField = (string[]) null;
      // ISSUE: reference to a compiler-generated field
      this.\u003CTimeCreated\u003Ek__BackingField = new DateTime();
      // ISSUE: reference to a compiler-generated field
      this.\u003CContextId\u003Ek__BackingField = 0L;
      // ISSUE: reference to a compiler-generated field
      this.\u003CActivityId\u003Ek__BackingField = new Guid();
      // ISSUE: reference to a compiler-generated field
      this.\u003CExceptionType\u003Ek__BackingField = (string) null;
      // ISSUE: reference to a compiler-generated field
      this.\u003CUniqueIdentifier\u003Ek__BackingField = new Guid();
      // ISSUE: reference to a compiler-generated field
      this.\u003CE2EId\u003Ek__BackingField = new Guid();
      // ISSUE: reference to a compiler-generated field
      this.\u003COrchestrationId\u003Ek__BackingField = (string) null;
      this.ProcessName = TeamFoundationTracingService.ProcessName;
    }

    public TraceEvent(string format)
    {
      this = new TraceEvent();
      this.Format = format;
      this.Args = new TraceEvent.ArgumentHolder((object[]) null);
      this.m_message = (string) null;
    }

    public TraceEvent(string format, object arg0)
      : this(format)
    {
      this.Args = new TraceEvent.ArgumentHolder(arg0);
    }

    public TraceEvent(string format, object arg0, object arg1)
      : this(format)
    {
      this.Args = new TraceEvent.ArgumentHolder(arg0, arg1);
    }

    public TraceEvent(string format, object arg0, object arg1, object arg2)
      : this(format)
    {
      this.Args = new TraceEvent.ArgumentHolder(arg0, arg1, arg2);
    }

    public TraceEvent(string format, params object[] args)
      : this(format)
    {
      this.Args = new TraceEvent.ArgumentHolder(args);
    }

    public TraceEvent(
      IVssRequestContext requestContext,
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string[] tags,
      string exceptionType,
      string format)
      : this(format)
    {
      this.Tracepoint = tracepoint;
      this.Level = level;
      this.Area = area;
      this.Layer = layer;
      this.Tags = tags;
      this.ExceptionType = exceptionType;
      IVssServiceHost serviceHost = requestContext.ServiceHost;
      this.ServiceHost = serviceHost != null ? serviceHost.InstanceId : Guid.Empty;
      this.Method = requestContext.RootContext.Method?.Name;
      this.UserAgent = requestContext.RootContext.UserAgent;
      this.UserLogin = requestContext.RootContext.AuthenticatedUserName;
      this.Service = requestContext.RootContext.ServiceName;
      this.Uri = requestContext.RawUrl();
      this.UniqueIdentifier = requestContext.UniqueIdentifier;
      this.E2EId = requestContext.E2EId;
      this.OrchestrationId = requestContext.OrchestrationId;
    }

    public TraceEvent(
      IVssRequestContext requestContext,
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string[] tags,
      string exceptionType,
      string format,
      object arg0)
      : this(requestContext, tracepoint, level, area, layer, tags, exceptionType, format)
    {
      this.Args = new TraceEvent.ArgumentHolder(arg0);
    }

    public TraceEvent(
      IVssRequestContext requestContext,
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string[] tags,
      string exceptionType,
      string format,
      object arg0,
      object arg1)
      : this(requestContext, tracepoint, level, area, layer, tags, exceptionType, format)
    {
      this.Args = new TraceEvent.ArgumentHolder(arg0, arg1);
    }

    public TraceEvent(
      IVssRequestContext requestContext,
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string[] tags,
      string exceptionType,
      string format,
      object arg0,
      object arg1,
      object arg2)
      : this(requestContext, tracepoint, level, area, layer, tags, exceptionType, format)
    {
      this.Args = new TraceEvent.ArgumentHolder(arg0, arg1, arg2);
    }

    public TraceEvent(
      IVssRequestContext requestContext,
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string[] tags,
      string exceptionType,
      string format,
      params object[] args)
      : this(requestContext, tracepoint, level, area, layer, tags, exceptionType, format)
    {
      this.Args = new TraceEvent.ArgumentHolder(args);
    }

    private string Format { get; set; }

    private TraceEvent.ArgumentHolder Args { get; set; }

    private string m_message { get; set; }

    public Guid TraceId { get; set; }

    public int Tracepoint { get; set; }

    public string ProcessName { get; set; }

    public string UserLogin { get; set; }

    public Guid VSID { get; set; }

    public Guid CUID { get; set; }

    public Guid TenantId { get; set; }

    public Guid ProviderId { get; set; }

    public string Service { get; set; }

    public string Method { get; set; }

    public TraceLevel Level { get; set; }

    public string Area { get; set; }

    public string Layer { get; set; }

    public string UserAgent { get; set; }

    public string Uri { get; set; }

    public string Path { get; set; }

    public Guid ServiceHost { get; set; }

    public string[] Tags { get; set; }

    public DateTime TimeCreated { get; set; }

    public long ContextId { get; set; }

    public Guid ActivityId { get; set; }

    public string ExceptionType { get; set; }

    public Guid UniqueIdentifier { get; set; }

    public Guid E2EId { get; set; }

    public string OrchestrationId { get; set; }

    public string GetMessage()
    {
      if (this.m_message == null)
      {
        string str;
        switch (this.Args.ArgCount)
        {
          case 0:
            str = this.Format;
            break;
          case 1:
            str = string.Format(this.Format, this.FormatArg(this.Args.Arg0));
            break;
          case 2:
            str = string.Format(this.Format, this.FormatArg(this.Args.Arg0), this.FormatArg(this.Args.Arg1));
            break;
          case 3:
            str = string.Format(this.Format, this.FormatArg(this.Args.Arg0), this.FormatArg(this.Args.Arg1), this.FormatArg(this.Args.Arg2));
            break;
          default:
            str = string.Format(this.Format, this.FormatArgs(this.Args.Args));
            break;
        }
        this.m_message = str;
        this.WarnOnSecretTraced();
      }
      return this.m_message;
    }

    private void WarnOnSecretTraced()
    {
    }

    private object[] FormatArgs(object[] objs)
    {
      for (int index = 0; index < objs.Length; ++index)
        objs[index] = this.FormatArg(objs[index]);
      return objs;
    }

    private object FormatArg(object obj)
    {
      switch (obj)
      {
        case byte[] numArray:
          int num1 = Math.Min(numArray.Length, 1024);
          char[] chArray = new char[num1 * 2];
          int num2 = 0;
          for (int index = 0; index < num1 * 2; index += 2)
          {
            int num3 = (int) numArray[num2++];
            char ch1 = (char) ((num3 >> 4 & 15) + 48);
            char ch2 = (char) ((num3 & 15) + 48);
            chArray[index] = ch1 >= ':' ? (char) ((uint) ch1 + 39U) : ch1;
            chArray[index + 1] = ch2 >= ':' ? (char) ((uint) ch2 + 39U) : ch2;
          }
          return (object) new string(chArray);
        case Lazy<string> lazy:
          try
          {
            return (object) lazy.Value;
          }
          catch (Exception ex)
          {
            return (object) ("Exception thrown while generating trace message: " + ex.ToReadableStackTrace());
          }
        default:
          return obj;
      }
    }

    public struct ArgumentHolder
    {
      public object Arg0;
      public object Arg1;
      public object Arg2;
      public object[] Args;
      public byte ArgCount;

      public ArgumentHolder(object arg0)
        : this()
      {
        this.ArgCount = (byte) 1;
        this.Arg0 = arg0;
      }

      public ArgumentHolder(object arg0, object arg1)
        : this(arg0)
      {
        this.ArgCount = (byte) 2;
        this.Arg1 = arg1;
      }

      public ArgumentHolder(object arg0, object arg1, object arg2)
        : this(arg0, arg1)
      {
        this.ArgCount = (byte) 3;
        this.Arg2 = arg2;
      }

      public ArgumentHolder(object[] args)
        : this()
      {
        this.ArgCount = args == null || args.Length == 0 ? (byte) 0 : (byte) 4;
        this.Args = args;
      }
    }
  }
}
