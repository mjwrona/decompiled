// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.WebApi.AadCapResult
// Assembly: Microsoft.VisualStudio.Services.Cloud.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52A8E326-8E84-4175-AE92-8ED7AF376B63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Cloud.WebApi
{
  [DataContract]
  public sealed class AadCapResult
  {
    [DataMember]
    public AadCapResult.AadCapAction Action { get; private set; }

    [DataMember]
    public DateTimeOffset Expires { get; private set; }

    public bool Allowed => this.Action == AadCapResult.AadCapAction.Allow;

    public AadCapResult(AadCapResult.AadCapAction aadCapResponse, DateTimeOffset expires)
    {
      this.Action = aadCapResponse;
      this.Expires = expires;
    }

    public static AadCapResult CreateAllowed(DateTimeOffset expires) => new AadCapResult(AadCapResult.AadCapAction.Allow, expires);

    public static AadCapResult CreateReauth() => new AadCapResult(AadCapResult.AadCapAction.ReAuth, DateTimeOffset.MinValue);

    public override string ToString() => string.Format("{{Action: {0}, Expires: {1}}}", (object) this.Action, (object) this.Expires);

    public enum AadCapAction
    {
      Allow,
      ReAuth,
    }
  }
}
