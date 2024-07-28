// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.CertificateNotFoundException
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Runtime.Serialization;
using System.Security;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Serializable]
  public class CertificateNotFoundException : TeamFoundationServiceException, ISerializable
  {
    public CertificateNotFoundException(string message, string thumbprint)
      : base(message)
    {
      this.Thumbprint = thumbprint;
    }

    public string Thumbprint { get; protected set; }

    protected CertificateNotFoundException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      this.Thumbprint = (string) info.GetValue(nameof (Thumbprint), typeof (string));
    }

    [SecurityCritical]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);
      info.AddValue("Thumbprint", (object) this.Thumbprint);
    }
  }
}
