// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingConsoleProgressWriter
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class ServicingConsoleProgressWriter
  {
    private int m_fileLogCounter;
    private bool m_error;
    private ServicingContext m_servicingContext;

    public ServicingConsoleProgressWriter(ServicingContext servicingContext) => this.m_servicingContext = servicingContext;

    public void WriteLine(string message)
    {
      this.m_servicingContext.LogInfo(message);
      Console.WriteLine(message);
    }

    public void WriteProgress(int current, int total, bool initialize = false)
    {
      if (initialize)
        this.m_fileLogCounter = 0;
      if (total == 0)
        return;
      double num = (double) current * 100.0 / (double) total;
      string message = string.Format("{0} of {1}  ({2:0.00}%) ", (object) current, (object) total, (object) num);
      if (!this.m_error)
      {
        try
        {
          Console.CursorLeft = 0;
          Console.Write(message);
          if (num == 100.0)
            Console.WriteLine();
        }
        catch (Exception ex)
        {
          this.m_error = true;
        }
      }
      if (num < (double) (10 * this.m_fileLogCounter))
        return;
      ++this.m_fileLogCounter;
      this.m_servicingContext.LogInfo(message);
    }
  }
}
