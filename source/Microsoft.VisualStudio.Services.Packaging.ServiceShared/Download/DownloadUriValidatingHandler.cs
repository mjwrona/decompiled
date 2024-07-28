// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download.DownloadUriValidatingHandler
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Web;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download
{
  public class DownloadUriValidatingHandler : IHandler<Uri, bool>
  {
    private readonly ITracerService tracerService;
    private readonly ITimeProvider timeProvider;

    public DownloadUriValidatingHandler(ITracerService tracerService, ITimeProvider timeProvider)
    {
      this.tracerService = tracerService;
      this.timeProvider = timeProvider;
    }

    public bool Handle(Uri url)
    {
      using (ITracerBlock tracerBlock = this.tracerService.Enter((object) this, nameof (Handle)))
      {
        DateTime dateTime = DateTime.MinValue;
        try
        {
          string s = HttpUtility.ParseQueryString(url.Query).Get("se");
          if (!string.IsNullOrEmpty(s))
            dateTime = DateTime.Parse(s).ToUniversalTime();
        }
        catch (Exception ex)
        {
        }
        DateTime universalTime = this.timeProvider.Now.ToUniversalTime();
        if (!(dateTime < universalTime.AddMinutes(5.0)))
          return true;
        tracerBlock.TraceError(Resources.Error_ExpiredUrl((object) url, (object) dateTime, (object) universalTime));
        return false;
      }
    }
  }
}
