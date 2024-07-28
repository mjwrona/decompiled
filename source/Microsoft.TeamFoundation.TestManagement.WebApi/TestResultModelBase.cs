// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.TestResultModelBase
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public class TestResultModelBase : TestManagementBaseSecuredObject
  {
    private DateTime m_startedDate;
    private DateTime m_completedDate;
    private double m_duration;

    public TestResultModelBase()
    {
    }

    public TestResultModelBase(
      string outcome,
      string startedDate,
      string completedDate,
      string duration,
      string errorMessage = null,
      string comment = null)
    {
      this.Outcome = outcome;
      if (string.IsNullOrEmpty(startedDate) || !DateTime.TryParse(startedDate, (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out this.m_startedDate))
        throw new ArgumentException(nameof (startedDate));
      if (string.IsNullOrEmpty(completedDate) || !DateTime.TryParse(completedDate, (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out this.m_completedDate))
        throw new ArgumentException(nameof (completedDate));
      if (string.IsNullOrEmpty(duration) || !double.TryParse(duration, out this.m_duration))
        throw new ArgumentException(nameof (duration));
      if (!string.IsNullOrEmpty(errorMessage))
        this.ErrorMessage = errorMessage;
      if (string.IsNullOrEmpty(comment))
        return;
      this.Comment = comment;
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Outcome { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ErrorMessage { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Comment { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime StartedDate
    {
      get => this.m_startedDate;
      set => this.m_startedDate = value;
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime CompletedDate
    {
      get => this.m_completedDate;
      set => this.m_completedDate = value;
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public double DurationInMs
    {
      get => this.m_duration;
      set => this.m_duration = value;
    }
  }
}
