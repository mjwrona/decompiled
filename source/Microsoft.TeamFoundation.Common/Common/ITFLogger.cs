// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.ITFLogger
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System;

namespace Microsoft.TeamFoundation.Common
{
  public interface ITFLogger
  {
    void Info(string message);

    void Info(string message, params object[] args);

    void Warning(string message);

    void Warning(string message, params object[] args);

    void Warning(Exception exception);

    void Error(string message);

    void Error(string message, params object[] args);

    void Error(Exception exception);
  }
}
