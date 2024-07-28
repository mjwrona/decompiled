// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsAzure.CloudServiceManagement.ResourceProviderCommunication.ResourceOutputCollection
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft.WindowsAzure.CloudServiceManagement.ResourceProviderCommunication
{
  [CollectionDataContract(Name = "Resources", Namespace = "http://schemas.microsoft.com/windowsazure")]
  [ExcludeFromCodeCoverage]
  public class ResourceOutputCollection : List<ResourceOutput>
  {
    public ResourceOutputCollection()
    {
    }

    public ResourceOutputCollection(IEnumerable<ResourceOutput> resources)
      : base(resources)
    {
    }

    public ResourceOutput Find(string resourceType, string resourceName) => this.Find((Predicate<ResourceOutput>) (res => string.Equals(res.Type, resourceType, StringComparison.InvariantCultureIgnoreCase) && string.Equals(res.Name, resourceName, StringComparison.InvariantCultureIgnoreCase)));
  }
}
