// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.HostUnavailableDuringDataImportHostUpgradeException
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Net;

namespace Microsoft.VisualStudio.Services.Cloud
{
  [Serializable]
  public class HostUnavailableDuringDataImportHostUpgradeException : RequestFilterException
  {
    public HostUnavailableDuringDataImportHostUpgradeException()
      : base(FrameworkResources.HostOfflineForDataImportWithId((object) Guid.Empty), HttpStatusCode.ServiceUnavailable)
    {
    }

    public HostUnavailableDuringDataImportHostUpgradeException(Guid dataImportId)
      : base(FrameworkResources.HostOfflineForDataImportWithId((object) dataImportId), HttpStatusCode.ServiceUnavailable)
    {
    }

    public HostUnavailableDuringDataImportHostUpgradeException(WrappedException ex)
      : base(ex.Message, ex.UnwrappedInnerException)
    {
    }
  }
}
