// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.UnsupportedODataModelVersionException
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.VisualStudio.Services.Analytics
{
  public class UnsupportedODataModelVersionException : VssServiceException
  {
    public UnsupportedODataModelVersionException(string message, IVssRequestContext requestContext)
      : base(message + " " + AnalyticsResources.ODATA_BAD_VERSION_SUFFIX_MESSAGE())
    {
    }
  }
}
