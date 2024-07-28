// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.PirResponseTimeline
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.AzureAd.Icm.Types
{
  [DataContract]
  public class PirResponseTimeline
  {
    [DataMember(Name = "ImpactStart")]
    public DateTime? ImpactStart { get; set; }

    [DataMember(Name = "Detection")]
    public DateTime? Detection { get; set; }

    [DataMember(Name = "Triage")]
    public DateTime? Triage { get; set; }

    [DataMember(Name = "EngEngaged")]
    public DateTime? EngEngaged { get; set; }

    [DataMember(Name = "CommsEngaged")]
    public DateTime? CommsEngaged { get; set; }

    [DataMember(Name = "FirstCustomerAdvisory")]
    public DateTime? FirstCustomerAdvisory { get; set; }

    [DataMember(Name = "DetailedCustomerAdvisory")]
    public DateTime? DetailedCustomerAdvisory { get; set; }

    [DataMember(Name = "Dashboard")]
    public DateTime? Dashboard { get; set; }

    [DataMember(Name = "Mitigation")]
    public DateTime? Mitigation { get; set; }

    [DataMember(Name = "Diagnosis")]
    public DateTime? Diagnosis { get; set; }

    [DataMember(Name = "Recovery")]
    public DateTime? Recovery { get; set; }

    [DataMember(Name = "Other1")]
    public DateTime? Other1 { get; set; }

    [DataMember(Name = "Other2")]
    public DateTime? Other2 { get; set; }

    [DataMember(Name = "Other3")]
    public DateTime? Other3 { get; set; }

    [DataMember(Name = "OutageDeclaredDate")]
    public DateTime? OutageDeclaredDate { get; set; }

    [DataMember(Name = "ImpactStartDescription")]
    public string ImpactStartDescription { get; set; }

    [DataMember(Name = "DetectionDescription")]
    public string DetectionDescription { get; set; }

    [DataMember(Name = "TriageDescription")]
    public string TriageDescription { get; set; }

    [DataMember(Name = "EngEngagedDescription")]
    public string EngEngagedDescription { get; set; }

    [DataMember(Name = "CommsEngagedDescription")]
    public string CommsEngagedDescription { get; set; }

    [DataMember(Name = "FirstCustomerAdvisoryDescription")]
    public string FirstCustomerAdvisoryDescription { get; set; }

    [DataMember(Name = "DetailedCustomerAdvisoryDescription")]
    public string DetailedCustomerAdvisoryDescription { get; set; }

    [DataMember(Name = "DashboardDescription")]
    public string DashboardDescription { get; set; }

    [DataMember(Name = "MitigationDescription")]
    public string MitigationDescription { get; set; }

    [DataMember(Name = "DiagnosisDescription")]
    public string DiagnosisDescription { get; set; }

    [DataMember(Name = "RecoveryDescription")]
    public string RecoveryDescription { get; set; }

    [DataMember(Name = "Other1Description")]
    public string Other1Description { get; set; }

    [DataMember(Name = "Other2Description")]
    public string Other2Description { get; set; }

    [DataMember(Name = "Other3Description")]
    public string Other3Description { get; set; }

    [DataMember(Name = "OutageDeclaredDescription")]
    public string OutageDeclaredDescription { get; set; }

    public override bool Equals(object obj)
    {
      if (obj == null || !(obj is PirResponseTimeline responseTimeline))
        return false;
      DateTime? nullable1 = this.ImpactStart;
      DateTime? nullable2 = responseTimeline.ImpactStart;
      if ((nullable1.HasValue == nullable2.HasValue ? (nullable1.HasValue ? (nullable1.GetValueOrDefault() == nullable2.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0 && string.Equals(this.ImpactStartDescription, responseTimeline.ImpactStartDescription, StringComparison.Ordinal))
      {
        nullable2 = this.Detection;
        nullable1 = responseTimeline.Detection;
        if ((nullable2.HasValue == nullable1.HasValue ? (nullable2.HasValue ? (nullable2.GetValueOrDefault() == nullable1.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0 && string.Equals(this.DetectionDescription, responseTimeline.DetectionDescription, StringComparison.Ordinal))
        {
          nullable1 = this.Triage;
          nullable2 = responseTimeline.Triage;
          if ((nullable1.HasValue == nullable2.HasValue ? (nullable1.HasValue ? (nullable1.GetValueOrDefault() == nullable2.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0 && string.Equals(this.TriageDescription, responseTimeline.TriageDescription, StringComparison.Ordinal))
          {
            nullable2 = this.EngEngaged;
            nullable1 = responseTimeline.EngEngaged;
            if ((nullable2.HasValue == nullable1.HasValue ? (nullable2.HasValue ? (nullable2.GetValueOrDefault() == nullable1.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0 && string.Equals(this.EngEngagedDescription, responseTimeline.EngEngagedDescription, StringComparison.Ordinal))
            {
              nullable1 = this.Mitigation;
              nullable2 = responseTimeline.Mitigation;
              if ((nullable1.HasValue == nullable2.HasValue ? (nullable1.HasValue ? (nullable1.GetValueOrDefault() == nullable2.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0 && string.Equals(this.MitigationDescription, responseTimeline.MitigationDescription, StringComparison.Ordinal))
              {
                nullable2 = this.Diagnosis;
                nullable1 = responseTimeline.Diagnosis;
                if ((nullable2.HasValue == nullable1.HasValue ? (nullable2.HasValue ? (nullable2.GetValueOrDefault() == nullable1.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0 && string.Equals(this.DiagnosisDescription, responseTimeline.DiagnosisDescription, StringComparison.Ordinal))
                {
                  nullable1 = this.Recovery;
                  nullable2 = responseTimeline.Recovery;
                  if ((nullable1.HasValue == nullable2.HasValue ? (nullable1.HasValue ? (nullable1.GetValueOrDefault() == nullable2.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0 && string.Equals(this.RecoveryDescription, responseTimeline.RecoveryDescription, StringComparison.Ordinal))
                  {
                    nullable2 = this.CommsEngaged;
                    nullable1 = responseTimeline.CommsEngaged;
                    if ((nullable2.HasValue == nullable1.HasValue ? (nullable2.HasValue ? (nullable2.GetValueOrDefault() == nullable1.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0 && string.Equals(this.CommsEngagedDescription, responseTimeline.CommsEngagedDescription, StringComparison.Ordinal))
                    {
                      nullable1 = this.FirstCustomerAdvisory;
                      nullable2 = responseTimeline.FirstCustomerAdvisory;
                      if ((nullable1.HasValue == nullable2.HasValue ? (nullable1.HasValue ? (nullable1.GetValueOrDefault() == nullable2.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0 && string.Equals(this.FirstCustomerAdvisoryDescription, responseTimeline.FirstCustomerAdvisoryDescription, StringComparison.Ordinal))
                      {
                        nullable2 = this.DetailedCustomerAdvisory;
                        nullable1 = responseTimeline.DetailedCustomerAdvisory;
                        if ((nullable2.HasValue == nullable1.HasValue ? (nullable2.HasValue ? (nullable2.GetValueOrDefault() == nullable1.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0 && string.Equals(this.DetailedCustomerAdvisoryDescription, responseTimeline.DetailedCustomerAdvisoryDescription, StringComparison.Ordinal))
                        {
                          nullable1 = this.Other1;
                          nullable2 = responseTimeline.Other1;
                          if ((nullable1.HasValue == nullable2.HasValue ? (nullable1.HasValue ? (nullable1.GetValueOrDefault() == nullable2.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0 && string.Equals(this.Other1Description, responseTimeline.Other1Description, StringComparison.Ordinal))
                          {
                            nullable2 = this.Dashboard;
                            nullable1 = responseTimeline.Dashboard;
                            if ((nullable2.HasValue == nullable1.HasValue ? (nullable2.HasValue ? (nullable2.GetValueOrDefault() == nullable1.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0 && string.Equals(this.DashboardDescription, responseTimeline.DashboardDescription, StringComparison.Ordinal))
                            {
                              nullable1 = this.Other2;
                              nullable2 = responseTimeline.Other2;
                              if ((nullable1.HasValue == nullable2.HasValue ? (nullable1.HasValue ? (nullable1.GetValueOrDefault() == nullable2.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0 && string.Equals(this.Other2Description, responseTimeline.Other2Description, StringComparison.Ordinal))
                              {
                                nullable2 = this.Other3;
                                nullable1 = responseTimeline.Other3;
                                if ((nullable2.HasValue == nullable1.HasValue ? (nullable2.HasValue ? (nullable2.GetValueOrDefault() == nullable1.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0 && string.Equals(this.Other3Description, responseTimeline.Other3Description, StringComparison.Ordinal))
                                {
                                  nullable1 = this.OutageDeclaredDate;
                                  nullable2 = responseTimeline.OutageDeclaredDate;
                                  return (nullable1.HasValue == nullable2.HasValue ? (nullable1.HasValue ? (nullable1.GetValueOrDefault() == nullable2.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0 && string.Equals(this.OutageDeclaredDescription, responseTimeline.OutageDeclaredDescription, StringComparison.Ordinal);
                                }
                              }
                            }
                          }
                        }
                      }
                    }
                  }
                }
              }
            }
          }
        }
      }
      return false;
    }

    public override int GetHashCode() => (this.ImpactStart.HasValue ? this.ImpactStart.GetHashCode() : 0) ^ (this.Detection.HasValue ? this.Detection.GetHashCode() : 0) ^ (this.Triage.HasValue ? this.Triage.GetHashCode() : 0) ^ (this.EngEngaged.HasValue ? this.EngEngaged.GetHashCode() : 0) ^ (this.Mitigation.HasValue ? this.Mitigation.GetHashCode() : 0) ^ (this.Diagnosis.HasValue ? this.Diagnosis.GetHashCode() : 0) ^ (this.Recovery.HasValue ? this.Recovery.GetHashCode() : 0) ^ (string.IsNullOrEmpty(this.ImpactStartDescription) ? 0 : this.ImpactStartDescription.GetHashCode()) ^ (string.IsNullOrEmpty(this.DetectionDescription) ? 0 : this.DetectionDescription.GetHashCode()) ^ (string.IsNullOrEmpty(this.TriageDescription) ? 0 : this.TriageDescription.GetHashCode()) ^ (string.IsNullOrEmpty(this.EngEngagedDescription) ? 0 : this.EngEngagedDescription.GetHashCode()) ^ (string.IsNullOrEmpty(this.MitigationDescription) ? 0 : this.MitigationDescription.GetHashCode()) ^ (string.IsNullOrEmpty(this.DiagnosisDescription) ? 0 : this.DiagnosisDescription.GetHashCode()) ^ (string.IsNullOrEmpty(this.RecoveryDescription) ? 0 : this.RecoveryDescription.GetHashCode());
  }
}
