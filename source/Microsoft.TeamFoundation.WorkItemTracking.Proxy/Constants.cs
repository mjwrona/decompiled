// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Proxy.Constants
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Proxy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF15D8B4-8AC0-4915-8153-9054E8546EA2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Proxy.dll

using System.Runtime.InteropServices;

namespace Microsoft.TeamFoundation.WorkItemTracking.Proxy
{
  [StructLayout(LayoutKind.Sequential, Size = 1)]
  internal struct Constants
  {
    internal const string requestIdParamter = "requestId";
    internal const string rowSetParameter = "rowSet";
    internal const string indexParameter = "index";
    internal const string readerParameter = "reader";
    internal const string columnParameter = "column";
    internal const string rowParameter = "row";
    internal const string row1Parameter = "row1";
    internal const string row2Parameter = "row2";
    internal const string nameParameter = "name";
    internal const string uuidScheme = "uuid:";
    public const int RetryAttemptsDeadlock = 2;
    public const int RetryAttemptsStaleView = 1;
  }
}
