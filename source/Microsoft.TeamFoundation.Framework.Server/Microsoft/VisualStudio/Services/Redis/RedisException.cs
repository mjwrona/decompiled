// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Redis.RedisException
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.CircuitBreaker;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Redis
{
  [Serializable]
  public class RedisException : Exception
  {
    private string m_message;
    internal const string RedisNotAvailable = "Redis cache is not available, try again later";

    public RedisException(string message)
      : this(message, (Exception) null)
    {
    }

    public RedisException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    public RedisException(CommandAsync command)
      : this(RedisException.FormatMessage("Redis cache is not available, try again later", command))
    {
    }

    protected RedisException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public override string Message => this.m_message ?? base.Message;

    public void SetMessage(string message) => this.m_message = message;

    private static string FormatMessage(string message, CommandAsync command)
    {
      if (command != null)
        message += string.Format(" (circuit {0})", command.IsCircuitBreakerOpen ? (object) "opened" : (object) "closed");
      return message;
    }
  }
}
