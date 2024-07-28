// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.Services.IFileDataService
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.CodeSense.Server.Services
{
  public interface IFileDataService
  {
    T GetCachedData<T>(IVssRequestContext requestContext, int fileId) where T : class;

    string GetData(IVssRequestContext requestContext, int fileId);

    T GetData<T>(IVssRequestContext requestContext, int fileId) where T : class;

    int PersistData<T>(IVssRequestContext requestContext, T data, out long length) where T : class;

    void DeleteData(IVssRequestContext requestContext, IEnumerable<int> fileIds);
  }
}
