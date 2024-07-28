// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.MitigationProviderJobStatus
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.AzureAd.Icm.Types
{
  public class MitigationProviderJobStatus
  {
    public MitigationProviderJobStatus()
    {
      this.ResultSummary = MitigationJobResultSummary.Unknown;
      this.State = MitigationJobState.Unknown;
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This parameter needs to be a collection of dictionaries")]
    public IList<IDictionary<string, object>> ResultDetails { get; set; }

    public MitigationJobResultSummary ResultSummary { get; set; }

    public MitigationJobState State { get; set; }
  }
}
