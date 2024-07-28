// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ReleaseStatusChangeDetails
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities
{
  public class ReleaseStatusChangeDetails : ReleaseRevisionChangeDetails
  {
    public ReleaseStatusChangeDetails() => this.Id = ReleaseHistoryMessageId.ReleaseStatusChange;

    public string ReleaseName { get; set; }

    public ReleaseStatus ReleaseStatus { get; set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, new Dictionary<ReleaseStatus, string>()
    {
      {
        ReleaseStatus.Active,
        Resources.ReleaseHistoryStartedRelease
      },
      {
        ReleaseStatus.Abandoned,
        Resources.ReleaseHistoryAbandonedRelease
      }
    }[this.ReleaseStatus], (object) this.ReleaseName);
  }
}
