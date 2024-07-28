// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.QueryMD5ContextResult
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class QueryMD5ContextResult
  {
    public byte[] MD5Context;
    public long MD5ByteCount;

    public QueryMD5ContextResult(byte[] md5Context)
    {
      this.MD5Context = md5Context;
      this.MD5ByteCount = -1L;
    }

    public QueryMD5ContextResult(byte[] md5Context, long md5ByteCount)
    {
      this.MD5Context = md5Context;
      this.MD5ByteCount = md5ByteCount;
    }

    public bool HasMD5ByteCount => this.MD5ByteCount >= 0L;
  }
}
