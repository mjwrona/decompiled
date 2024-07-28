// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.GalleryProcessHandlerHelper
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Configuration;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  internal class GalleryProcessHandlerHelper
  {
    public virtual ProcessOutput RunExe(string command, CommandLineBuilder args, ITFLogger logger) => ProcessHandler.RunExe(command, args, logger);

    public virtual ProcessOutput RunExe(
      ProcessStartInfo startInfo,
      string displayArguments,
      ITFLogger logger)
    {
      return ProcessHandler.RunExe(startInfo, displayArguments, logger);
    }
  }
}
