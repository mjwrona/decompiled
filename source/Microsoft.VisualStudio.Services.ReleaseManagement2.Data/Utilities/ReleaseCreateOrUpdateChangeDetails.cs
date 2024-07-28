// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ReleaseCreateOrUpdateChangeDetails
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
  public class ReleaseCreateOrUpdateChangeDetails : ReleaseRevisionChangeDetails
  {
    public ReleaseCreateOrUpdateChangeDetails() => this.Id = ReleaseHistoryMessageId.ReleaseCreateOrUpdateChange;

    public ReleaseHistoryChangeTypes ChangeType { get; set; }

    public string ReleaseName { get; set; }

    public override string GetServerFormat() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "%messageId=\"{0}\";%:%releaseName=\"{1}\";% %changeType=\"{2}\";%", (object) this.Id, (object) this.ReleaseName, (object) this.ChangeType);

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, new Dictionary<ReleaseHistoryChangeTypes, string>()
    {
      {
        ReleaseHistoryChangeTypes.Create,
        Resources.ReleaseHistoryCreatedRelease
      },
      {
        ReleaseHistoryChangeTypes.Update,
        Resources.ReleaseHistoryUpdatedRelease
      },
      {
        ReleaseHistoryChangeTypes.Delete,
        Resources.ReleaseHistoryDeletedRelease
      },
      {
        ReleaseHistoryChangeTypes.Undelete,
        Resources.ReleaseHistoryUndeleteRelease
      }
    }[this.ChangeType], (object) this.ReleaseName);
  }
}
