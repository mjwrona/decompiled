// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.IKustoQueryProviderService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Cloud
{
  [DefaultServiceImplementation(typeof (KustoQueryProviderService))]
  internal interface IKustoQueryProviderService : IVssFrameworkService
  {
    IDataReader ExecuteQuery(IVssRequestContext requestContext, string query);

    IEnumerable<T> ExecuteQuery<T>(IVssRequestContext requestContext, string query) where T : class;

    bool ReadDataReader(IDataReader dataReader);
  }
}
