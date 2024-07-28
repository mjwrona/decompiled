// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ClientResponseTypeAttribute
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
  public sealed class ClientResponseTypeAttribute : Attribute
  {
    public ClientResponseTypeAttribute(Type responseType, string methodName = null, string mediaType = null)
    {
      this.ResponseType = responseType;
      this.MethodName = methodName;
      this.MediaType = mediaType;
    }

    public Type ResponseType { get; set; }

    public string MethodName { get; set; }

    public string MediaType { get; set; }
  }
}
