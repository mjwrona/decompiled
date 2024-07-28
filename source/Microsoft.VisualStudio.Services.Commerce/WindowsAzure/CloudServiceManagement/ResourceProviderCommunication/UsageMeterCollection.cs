// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsAzure.CloudServiceManagement.ResourceProviderCommunication.UsageMeterCollection
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft.WindowsAzure.CloudServiceManagement.ResourceProviderCommunication
{
  [CollectionDataContract(Name = "UsageMeters", Namespace = "http://schemas.microsoft.com/windowsazure")]
  [ExcludeFromCodeCoverage]
  public class UsageMeterCollection : List<UsageMeter>
  {
    public UsageMeterCollection()
    {
    }

    public UsageMeterCollection(IEnumerable<UsageMeter> meters)
      : base(meters)
    {
    }
  }
}
