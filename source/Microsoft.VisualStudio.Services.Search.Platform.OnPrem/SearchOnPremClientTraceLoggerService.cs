// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Search.Platform.OnPrem.SearchOnPremClientTraceLoggerService
// Assembly: Microsoft.VisualStudio.Services.Search.Platform.OnPrem, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 34F866B2-C282-4F28-9C5F-A4E5E97C2DB9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platform.OnPrem.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.Search.Platform.Common;
using Microsoft.VisualStudio.Services.WebPlatform;
using System;
using System.ComponentModel.Composition;

namespace Microsoft.TeamFoundation.Search.Platform.OnPrem
{
  [Export(typeof (ISearchClientTraceLoggerService))]
  public class SearchOnPremClientTraceLoggerService : 
    ISearchClientTraceLoggerService,
    IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void Publish(
      IVssRequestContext requestContext,
      string area,
      string feature,
      ClientTraceData properties)
    {
    }

    public void Publish(
      IVssRequestContext requestContext,
      string area,
      string layer,
      Level level,
      string message)
    {
    }

    public void Publish(
      IVssRequestContext requestContext,
      string area,
      string layer,
      Exception exception)
    {
    }
  }
}
