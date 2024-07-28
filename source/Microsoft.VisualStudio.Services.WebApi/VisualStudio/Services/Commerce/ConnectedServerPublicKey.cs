// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.ConnectedServerPublicKey
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [DataContract]
  public sealed class ConnectedServerPublicKey
  {
    public ConnectedServerPublicKey()
    {
    }

    public ConnectedServerPublicKey(byte[] exponent, byte[] modulus)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) exponent, nameof (exponent));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) modulus, nameof (modulus));
      this.Exponent = exponent;
      this.Modulus = modulus;
    }

    private ConnectedServerPublicKey(ConnectedServerPublicKey objectToBeCloned)
    {
      if (objectToBeCloned.Exponent != null)
      {
        this.Exponent = new byte[objectToBeCloned.Exponent.Length];
        Buffer.BlockCopy((Array) objectToBeCloned.Exponent, 0, (Array) this.Exponent, 0, objectToBeCloned.Exponent.Length);
      }
      if (objectToBeCloned.Modulus == null)
        return;
      this.Modulus = new byte[objectToBeCloned.Modulus.Length];
      Buffer.BlockCopy((Array) objectToBeCloned.Modulus, 0, (Array) this.Modulus, 0, objectToBeCloned.Modulus.Length);
    }

    [DataMember(EmitDefaultValue = false)]
    public byte[] Exponent { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public byte[] Modulus { get; set; }

    public ConnectedServerPublicKey Clone() => new ConnectedServerPublicKey(this);
  }
}
