// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Admin.StdoutLogWriter
// Assembly: Microsoft.TeamFoundation.Admin.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4DC7473-FE52-49C1-BB5D-1E769BB5001D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Admin.Common.dll

using System;

namespace Microsoft.TeamFoundation.Admin
{
  internal sealed class StdoutLogWriter : ILogWriter, IDisposable
  {
    public static readonly StdoutLogWriter Instance = new StdoutLogWriter();

    public void Write(string text)
    {
      Console.Out.Write(text);
      Console.Out.Flush();
    }

    void IDisposable.Dispose()
    {
    }

    private StdoutLogWriter()
    {
    }
  }
}
