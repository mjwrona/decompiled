// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Bundling.BundledFileGenerator
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Bundling
{
  internal class BundledFileGenerator : 
    WaitHandleLock<Tuple<IVssRequestContext, UrlHelper>, BundledFile>
  {
    private static readonly TimeSpan s_singleWaitTime = TimeSpan.FromSeconds(30.0);
    private static readonly TimeSpan s_overallWaitTime = TimeSpan.FromSeconds(70.0);

    public BundledFileGenerator(
      string bundleKey,
      Func<IVssRequestContext, UrlHelper, BundledFile> getFileAction)
      : base((Func<Tuple<IVssRequestContext, UrlHelper>, BundledFile>) (args => getFileAction(args.Item1, args.Item2)), BundledFileGenerator.s_singleWaitTime, BundledFileGenerator.s_overallWaitTime)
    {
      this.TimeOutExceptionMessage = string.Format("Timed out waiting for bundle {0} to be generated", (object) bundleKey);
    }

    public BundledFile GetFile(IVssRequestContext requestContext, UrlHelper urlHelper) => this.PerformAction(new Tuple<IVssRequestContext, UrlHelper>(requestContext, urlHelper));

    public class GetFileActionArgs
    {
      public IVssRequestContext RequestContext { get; set; }

      public UrlHelper UrlHelper { get; set; }
    }
  }
}
