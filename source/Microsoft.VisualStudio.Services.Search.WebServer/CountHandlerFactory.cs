// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.CountHandlerFactory
// Assembly: Microsoft.VisualStudio.Services.Search.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1112A012-BB03-4D21-B53E-3AFB00CCC7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using System;

namespace Microsoft.VisualStudio.Services.Search.WebServer
{
  public sealed class CountHandlerFactory : ICountHandlerFactory
  {
    public bool TryGetCountRequestHandler(
      IEntityType entityType,
      out ICountHandler countRequestHandler)
    {
      switch (entityType.Name)
      {
        case "WorkItem":
          countRequestHandler = (ICountHandler) new WorkItemCountHandler();
          return true;
        case "Code":
          countRequestHandler = (ICountHandler) new CodeCountHandler();
          return true;
        case "Wiki":
          countRequestHandler = (ICountHandler) new WikiCountHandler();
          return true;
        case "Package":
          countRequestHandler = (ICountHandler) new CollectionPackageCountHandler();
          return true;
        default:
          countRequestHandler = (ICountHandler) null;
          return false;
      }
    }

    public bool TryGetCountRequestHandler(
      string entityType,
      IVssRequestContext requestContext,
      out ICountHandler countRequestHandler)
    {
      throw new NotImplementedException();
    }
  }
}
