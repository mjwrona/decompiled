// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Index.CustomProjectForwarder
// Assembly: Microsoft.VisualStudio.Services.Search.Index, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C10BA9-319D-46E2-AA64-F18680226A42
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Index.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Index
{
  public class CustomProjectForwarder : ICustomProjectForwarder
  {
    public CustomProjectForwarder() => this.DataAccessFactoryInstance = DataAccessFactory.GetInstance();

    internal IDataAccessFactory DataAccessFactoryInstance { get; set; }

    public IEnumerable<string> ForwardGetProjectsRequest(IVssRequestContext requestContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1081370, "Query Pipeline", "Query", nameof (ForwardGetProjectsRequest));
      try
      {
        return this.DataAccessFactoryInstance.GetCustomRepositoryDataAccess().GetProjects(requestContext, requestContext.GetCollectionID());
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1081371, "Query Pipeline", "Query", nameof (ForwardGetProjectsRequest));
      }
    }
  }
}
