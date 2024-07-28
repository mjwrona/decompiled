// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SqlCommandTimeouts
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public struct SqlCommandTimeouts
  {
    public readonly int Timeout;
    public readonly int AnonymousTimeout;

    public SqlCommandTimeouts(int timeout, int anonymousTimeout = 10)
    {
      ArgumentUtility.CheckForOutOfRange(timeout, nameof (timeout), 0);
      ArgumentUtility.CheckForOutOfRange(anonymousTimeout, nameof (anonymousTimeout), 1, 10);
      this.Timeout = timeout;
      this.AnonymousTimeout = anonymousTimeout;
    }
  }
}
