// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VssServerByteArrayContent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Net.Http;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class VssServerByteArrayContent : ByteArrayContent, IVssServerHttpContent
  {
    public VssServerByteArrayContent(byte[] content, object securedObject)
      : base(content)
    {
      this.Validate(securedObject);
    }

    public VssServerByteArrayContent(byte[] content, int offset, int count, object securedObject)
      : base(content, offset, count)
    {
      this.Validate(securedObject);
    }
  }
}
