// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.ArmResourceIdentifier
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class ArmResourceIdentifier
  {
    private readonly string resourceId;

    internal ArmResourceIdentifier(string resourceId)
    {
      this.resourceId = resourceId;
      this.Parse();
    }

    internal Guid SubscriptionId { get; private set; }

    internal string ResourceGroupName { get; private set; }

    internal string ProviderNamespace { get; private set; }

    internal string ResourceType { get; private set; }

    internal string ResourceName { get; private set; }

    private void Parse()
    {
      string[] strArray = this.resourceId.Trim('/').Split(new char[1]
      {
        '/'
      }, StringSplitOptions.RemoveEmptyEntries);
      int length = strArray.Length;
      Guid result;
      if (length < 8 || !string.Equals(strArray[0], "subscriptions", StringComparison.OrdinalIgnoreCase) || !string.Equals(strArray[2], "resourceGroups", StringComparison.OrdinalIgnoreCase) || !string.Equals(strArray[4], "providers", StringComparison.OrdinalIgnoreCase) || !Guid.TryParse(strArray[1], out result))
        throw new ArgumentException(HostingResources.InvalidResourceIdentifier0((object) this.resourceId));
      this.SubscriptionId = result;
      this.ResourceGroupName = strArray[3];
      this.ProviderNamespace = strArray[5];
      this.ResourceType = strArray[length - 2];
      this.ResourceName = strArray[length - 1];
    }
  }
}
