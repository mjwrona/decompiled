// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskAgentPublicKey
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public sealed class TaskAgentPublicKey
  {
    public TaskAgentPublicKey()
    {
    }

    public TaskAgentPublicKey(byte[] exponent, byte[] modulus)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) exponent, nameof (exponent));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) modulus, nameof (modulus));
      this.Exponent = exponent;
      this.Modulus = modulus;
    }

    private TaskAgentPublicKey(TaskAgentPublicKey objectToBeCloned)
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

    public TaskAgentPublicKey Clone() => new TaskAgentPublicKey(this);
  }
}
