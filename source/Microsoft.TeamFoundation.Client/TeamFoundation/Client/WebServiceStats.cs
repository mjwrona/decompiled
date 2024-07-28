// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.WebServiceStats
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;

namespace Microsoft.TeamFoundation.Client
{
  public class WebServiceStats
  {
    private string m_webService;
    private int m_minTime = -1;
    private int m_maxTime = -1;
    private int m_last = -1;
    private int m_average;
    private int m_count;
    private int m_totalTime;

    public WebServiceStats(string webServiceName) => this.m_webService = webServiceName;

    public void AddTime(int runTime)
    {
      if (runTime < this.MinTime || this.MinTime == -1)
        this.MinTime = runTime;
      if (runTime > this.MaxTime)
        this.MaxTime = runTime;
      ++this.Count;
      this.TotalTime += runTime;
      this.Average = this.TotalTime / this.Count;
      this.Last = runTime;
    }

    public event EventHandler WebServiceChanged;

    public string WebService
    {
      get => this.m_webService;
      private set
      {
        this.m_webService = value;
        EventHandler webServiceChanged = this.WebServiceChanged;
        if (webServiceChanged == null)
          return;
        webServiceChanged((object) this, EventArgs.Empty);
      }
    }

    public event EventHandler MinTimeChanged;

    public int MinTime
    {
      get => this.m_minTime;
      private set
      {
        this.m_minTime = value;
        EventHandler minTimeChanged = this.MinTimeChanged;
        if (minTimeChanged == null)
          return;
        minTimeChanged((object) this, EventArgs.Empty);
      }
    }

    public event EventHandler MaxTimeChanged;

    public int MaxTime
    {
      get => this.m_maxTime;
      private set
      {
        this.m_maxTime = value;
        EventHandler maxTimeChanged = this.MaxTimeChanged;
        if (maxTimeChanged == null)
          return;
        maxTimeChanged((object) this, EventArgs.Empty);
      }
    }

    public event EventHandler CountChanged;

    public int Count
    {
      get => this.m_count;
      private set
      {
        this.m_count = value;
        EventHandler countChanged = this.CountChanged;
        if (countChanged == null)
          return;
        countChanged((object) this, EventArgs.Empty);
      }
    }

    public event EventHandler TotalTimeChanged;

    public int TotalTime
    {
      get => this.m_totalTime;
      private set
      {
        this.m_totalTime = value;
        EventHandler totalTimeChanged = this.TotalTimeChanged;
        if (totalTimeChanged == null)
          return;
        totalTimeChanged((object) this, EventArgs.Empty);
      }
    }

    public event EventHandler AverageChanged;

    public int Average
    {
      get => this.m_average;
      private set
      {
        this.m_average = value;
        EventHandler averageChanged = this.AverageChanged;
        if (averageChanged == null)
          return;
        averageChanged((object) this, EventArgs.Empty);
      }
    }

    public event EventHandler LastChanged;

    public int Last
    {
      get => this.m_last;
      private set
      {
        this.m_last = value;
        EventHandler lastChanged = this.LastChanged;
        if (lastChanged == null)
          return;
        lastChanged((object) this, EventArgs.Empty);
      }
    }

    public static int DescendingSort(WebServiceStats first, WebServiceStats second)
    {
      if (second.TotalTime < first.TotalTime)
        return -1;
      if (second.TotalTime > first.TotalTime)
        return 1;
      if (second.Average < first.Average)
        return -1;
      if (second.Average > first.Average)
        return 1;
      if (second.Count < first.Count)
        return -1;
      return second.Count <= first.Count ? string.Compare(second.WebService, first.WebService, StringComparison.Ordinal) : 1;
    }
  }
}
