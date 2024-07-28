// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VssComponentInitInfo
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Text;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class VssComponentInitInfo
  {
    private int m_maxHistory;
    private VssComponentInitInfo.RequestRetryInfo m_currentRequest;
    private VssComponentInitInfo.RequestRetryInfo[] m_retryInfos;
    private ServicingContext m_servicingContext;
    private int m_distinctRequests;
    private int m_attempts;
    private TimeSpan m_sleepDuration;

    public VssComponentInitInfo(ServicingContext servicingContext, int maxHistory)
    {
      this.m_servicingContext = servicingContext;
      this.m_maxHistory = maxHistory;
      this.Init();
    }

    private void Init()
    {
      this.m_distinctRequests = 0;
      this.m_retryInfos = new VssComponentInitInfo.RequestRetryInfo[this.m_maxHistory];
      this.m_attempts = 0;
      this.m_sleepDuration = TimeSpan.Zero;
      this.m_currentRequest = (VssComponentInitInfo.RequestRetryInfo) null;
    }

    public void InitialAttempt(string componentName)
    {
      ++this.m_distinctRequests;
      ++this.m_attempts;
      if (this.m_distinctRequests > this.m_retryInfos.Length)
      {
        this.m_currentRequest = (VssComponentInitInfo.RequestRetryInfo) null;
      }
      else
      {
        this.m_currentRequest = new VssComponentInitInfo.RequestRetryInfo(componentName);
        ++this.m_currentRequest.m_attempts;
        this.m_retryInfos[this.m_distinctRequests - 1] = this.m_currentRequest;
      }
    }

    public void Reset() => this.Init();

    public void Retry(TimeSpan sleep)
    {
      try
      {
        if (this.m_servicingContext != null)
        {
          if (!this.m_servicingContext.IsDisposed)
            this.m_servicingContext.Log(ServicingStepLogEntryKind.SleepDuration, sleep.TotalSeconds.ToString());
        }
      }
      catch (Exception ex)
      {
      }
      ++this.m_attempts;
      this.m_sleepDuration += sleep;
      if (this.m_currentRequest == null)
        return;
      ++this.m_currentRequest.m_attempts;
      this.m_currentRequest.m_sleepDuration += sleep;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("Distinct Sql Resource Component initializations: " + this.m_distinctRequests.ToString());
      stringBuilder.AppendLine("Attempts: " + this.m_attempts.ToString());
      stringBuilder.AppendLine("Retries: " + (this.m_attempts - this.m_distinctRequests).ToString());
      stringBuilder.AppendLine("Total sleep (seconds): " + this.m_sleepDuration.TotalSeconds.ToString());
      if (this.m_maxHistory > 0 && this.m_distinctRequests > 0)
      {
        stringBuilder.AppendLine("Request Details:");
        foreach (VssComponentInitInfo.RequestRetryInfo retryInfo in this.m_retryInfos)
        {
          if (retryInfo != null)
          {
            stringBuilder.AppendLine("ComponentName: " + retryInfo.m_componentName);
            stringBuilder.AppendLine("   Attempts: " + retryInfo.m_attempts.ToString());
            stringBuilder.AppendLine("   Sleep (seconds): " + retryInfo.m_sleepDuration.TotalSeconds.ToString());
          }
        }
      }
      return stringBuilder.ToString();
    }

    private class RequestRetryInfo
    {
      public string m_componentName;
      public int m_attempts;
      public TimeSpan m_sleepDuration;

      public RequestRetryInfo(string componentName)
      {
        this.m_componentName = componentName;
        this.m_attempts = 0;
        this.m_sleepDuration = TimeSpan.Zero;
      }
    }
  }
}
