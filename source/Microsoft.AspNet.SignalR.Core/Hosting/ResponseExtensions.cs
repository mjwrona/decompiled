// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Hosting.ResponseExtensions
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.Hosting
{
  public static class ResponseExtensions
  {
    public static Task End(this IResponse response, string data)
    {
      if (response == null)
        throw new ArgumentNullException(nameof (response));
      byte[] bytes = Encoding.UTF8.GetBytes(data);
      response.Write(new ArraySegment<byte>(bytes, 0, bytes.Length));
      return TaskAsyncHelper.Empty;
    }
  }
}
