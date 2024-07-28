// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Fluent.ChangeFeedPolicyDefinition
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;

namespace Microsoft.Azure.Cosmos.Fluent
{
  internal class ChangeFeedPolicyDefinition
  {
    private readonly ContainerBuilder parent;
    private readonly Action<ChangeFeedPolicy> attachCallback;
    private TimeSpan changeFeedPolicyRetention;

    internal ChangeFeedPolicyDefinition(
      ContainerBuilder parent,
      TimeSpan retention,
      Action<ChangeFeedPolicy> attachCallback)
    {
      this.parent = parent ?? throw new ArgumentNullException(nameof (parent));
      this.attachCallback = attachCallback ?? throw new ArgumentNullException(nameof (attachCallback));
      this.changeFeedPolicyRetention = retention;
    }

    public ContainerBuilder Attach()
    {
      this.attachCallback(new ChangeFeedPolicy()
      {
        FullFidelityRetention = this.changeFeedPolicyRetention
      });
      return this.parent;
    }
  }
}
