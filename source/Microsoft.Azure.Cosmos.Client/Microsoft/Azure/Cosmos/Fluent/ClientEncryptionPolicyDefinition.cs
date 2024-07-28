// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Fluent.ClientEncryptionPolicyDefinition
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Azure.Cosmos.Fluent
{
  public sealed class ClientEncryptionPolicyDefinition
  {
    private readonly Collection<ClientEncryptionIncludedPath> clientEncryptionIncludedPaths = new Collection<ClientEncryptionIncludedPath>();
    private readonly ContainerBuilder parent;
    private readonly Action<ClientEncryptionPolicy> attachCallback;
    private readonly int policyFormatVersion;

    internal ClientEncryptionPolicyDefinition(
      ContainerBuilder parent,
      Action<ClientEncryptionPolicy> attachCallback,
      int policyFormatVersion = 1)
    {
      this.parent = parent;
      this.attachCallback = attachCallback;
      this.policyFormatVersion = policyFormatVersion <= 2 && policyFormatVersion >= 1 ? policyFormatVersion : throw new ArgumentException("Supported versions of client encryption policy are 1 and 2. ");
    }

    public ClientEncryptionPolicyDefinition WithIncludedPath(ClientEncryptionIncludedPath path)
    {
      this.clientEncryptionIncludedPaths.Add(path);
      return this;
    }

    public ContainerBuilder Attach()
    {
      this.attachCallback(new ClientEncryptionPolicy((IEnumerable<ClientEncryptionIncludedPath>) this.clientEncryptionIncludedPaths, this.policyFormatVersion));
      return this.parent;
    }
  }
}
