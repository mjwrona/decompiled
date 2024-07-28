// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.SubscriptionEvaluationStats
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using System.Text;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class SubscriptionEvaluationStats
  {
    private long m_processingTimeTaken;
    private long m_evaluationTimeTaken;
    private long m_xpathEvaluationtimeTaken;
    private long m_parseSubscriptionTimeTaken;
    private long m_parseProjectTimeTaken;
    private long m_updateProjectTimeTaken;
    private long m_totalTimeTaken;

    public bool GroupSubscription { get; set; }

    public long ProcessingTimeTaken
    {
      get => this.m_processingTimeTaken;
      set => this.SetAndTotal(ref this.m_processingTimeTaken, value);
    }

    public long EvaluationTimeTaken
    {
      get => this.m_evaluationTimeTaken;
      set => this.SetAndTotal(ref this.m_evaluationTimeTaken, value);
    }

    public long XpathEvaluationtimeTaken
    {
      get => this.m_xpathEvaluationtimeTaken;
      set => this.SetAndTotal(ref this.m_xpathEvaluationtimeTaken, value);
    }

    public long ParseSubscriptionTimeTaken
    {
      get => this.m_parseSubscriptionTimeTaken;
      set => this.SetAndTotal(ref this.m_parseSubscriptionTimeTaken, value);
    }

    public long ParseProjectTimeTaken
    {
      get => this.m_parseProjectTimeTaken;
      set => this.SetAndTotal(ref this.m_parseProjectTimeTaken, value);
    }

    public long UpdateProjectTimeTaken
    {
      get => this.m_updateProjectTimeTaken;
      set => this.SetAndTotal(ref this.m_updateProjectTimeTaken, value);
    }

    public long TotalTimeTaken => this.m_totalTimeTaken;

    public int Notifications { get; set; }

    public int Evaluations { get; set; }

    public void UpdateFrom(SubscriptionEvaluationStats that)
    {
      this.Evaluations += that.Evaluations;
      this.Notifications += that.Notifications;
      this.m_evaluationTimeTaken += that.m_evaluationTimeTaken;
      this.m_parseProjectTimeTaken += that.m_parseProjectTimeTaken;
      this.m_parseSubscriptionTimeTaken += that.m_parseSubscriptionTimeTaken;
      this.m_processingTimeTaken += that.m_processingTimeTaken;
      this.m_updateProjectTimeTaken += that.m_updateProjectTimeTaken;
      this.m_xpathEvaluationtimeTaken += that.m_xpathEvaluationtimeTaken;
      this.m_totalTimeTaken += that.m_totalTimeTaken;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendFormat("Sub team subs: {0} evaluations: {1} notifications: {2} processing: {3}", (object) this.GroupSubscription, (object) this.Evaluations, (object) this.Notifications, (object) this.GetProcessingTimeTaken());
      stringBuilder.AppendLine();
      stringBuilder.AppendFormat("Eval: {0} XPath: {1} parse subs: {2} parse project: {3} update project: {4}", (object) this.GetEvaluationTimeTaken(), (object) this.GetXpathEvaluationtimeTaken(), (object) this.GetParseSubscriptionTimeTaken(), (object) this.GetParseProjectTimeTaken(), (object) this.GetUpdateProjectTimeTaken());
      return stringBuilder.ToString();
    }

    public int GetProcessingTimeTaken() => this.ConvertToMS(this.ProcessingTimeTaken);

    public int GetEvaluationTimeTaken() => this.ConvertToMS(this.EvaluationTimeTaken);

    public int GetXpathEvaluationtimeTaken() => this.ConvertToMS(this.XpathEvaluationtimeTaken);

    public int GetParseSubscriptionTimeTaken() => this.ConvertToMS(this.ParseSubscriptionTimeTaken);

    public int GetParseProjectTimeTaken() => this.ConvertToMS(this.ParseProjectTimeTaken);

    public int GetUpdateProjectTimeTaken() => this.ConvertToMS(this.UpdateProjectTimeTaken);

    public int GetTotalTimeTaken() => this.ConvertToMS(this.TotalTimeTaken);

    private int ConvertToMS(long value) => (int) (value / 10000L);

    private void SetAndTotal(ref long field, long newValue)
    {
      this.m_totalTimeTaken += newValue - field;
      field = newValue;
    }
  }
}
