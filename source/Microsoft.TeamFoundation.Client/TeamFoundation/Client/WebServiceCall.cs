// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.WebServiceCall
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.Client
{
  public class WebServiceCall
  {
    private string m_webServiceCalled;
    private int m_runTime;
    private DateTime m_startTime;
    private DateTime m_endTime;
    private int m_threadId;
    private string m_threadPriority;
    private bool m_isRunning;

    public WebServiceCall(string webServiceName, int runTime, bool isRunning)
    {
      this.m_webServiceCalled = webServiceName;
      this.m_runTime = runTime;
      this.m_isRunning = isRunning;
      if (isRunning)
        this.m_startTime = DateTime.Now;
      else
        this.m_endTime = DateTime.Now;
    }

    public string WebServiceCalled => this.m_webServiceCalled;

    public int RunTime => this.m_runTime;

    public DateTime StartTime
    {
      get => this.m_startTime;
      set => this.m_startTime = value;
    }

    public string StartTimeString => this.m_isRunning ? "Running" : this.m_startTime.ToString("G", (IFormatProvider) CultureInfo.CurrentCulture);

    public DateTime EndTime
    {
      get => this.m_endTime;
      set => this.m_endTime = value;
    }

    public string EndTimeString => this.m_endTime.ToString("G", (IFormatProvider) CultureInfo.CurrentCulture);

    public int ThreadId
    {
      get => this.m_threadId;
      set => this.m_threadId = value;
    }

    public string ThreadPriority
    {
      get => this.m_threadPriority;
      set => this.m_threadPriority = value;
    }

    public bool IsRunning => this.m_isRunning;
  }
}
