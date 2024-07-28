// Decompiled with JetBrains decompiler
// Type: NCrontab.CrontabException
// Assembly: NCrontab.Signed, Version=3.3.2.0, Culture=neutral, PublicKeyToken=5247b4370afff365
// MVID: 57590294-74AB-400C-8AE3-EEC26A6094FB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\NCrontab.Signed.dll

using System;
using System.Runtime.Serialization;

namespace NCrontab
{
  [Serializable]
  public class CrontabException : Exception
  {
    public CrontabException()
      : base("Crontab error.")
    {
    }

    public CrontabException(string message)
      : base(message)
    {
    }

    public CrontabException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected CrontabException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
