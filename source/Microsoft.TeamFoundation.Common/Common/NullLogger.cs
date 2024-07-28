// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.NullLogger
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System;

namespace Microsoft.TeamFoundation.Common
{
  public class NullLogger : ITFLogger
  {
    public virtual void Info(string message)
    {
    }

    public virtual void Info(string message, params object[] args)
    {
    }

    public virtual void Warning(string message)
    {
    }

    public virtual void Warning(string message, params object[] args)
    {
    }

    public virtual void Warning(Exception exception)
    {
    }

    public virtual void Error(string message)
    {
    }

    public virtual void Error(string message, params object[] args)
    {
    }

    public virtual void Error(Exception exception)
    {
    }
  }
}
