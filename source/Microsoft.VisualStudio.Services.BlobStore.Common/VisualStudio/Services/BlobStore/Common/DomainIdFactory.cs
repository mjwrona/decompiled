// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.DomainIdFactory
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using Microsoft.VisualStudio.Services.Content.Common.MultiDomainExceptions;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  public class DomainIdFactory
  {
    public static IDomainId Create(string value)
    {
      if (string.IsNullOrWhiteSpace(value))
        return WellKnownDomainIds.DefaultDomainId;
      ProjectDomainId result1;
      string error1;
      if (value.Contains("_") && ProjectDomainId.TryParse(value, out result1, out error1))
        return (IDomainId) result1;
      ShardSetDomainId result2;
      if (ShardSetDomainId.TryParse(value, out result2, out error1))
        return (IDomainId) result2;
      byte result3;
      string error2;
      bool isUnSupportedDomainId;
      if (ByteDomainId.TryParse(value, out result3, out error2, out isUnSupportedDomainId))
        return (IDomainId) new ByteDomainId(result3);
      if (isUnSupportedDomainId)
        throw new InvalidDomainIdException(error2);
      throw new DomainIdDeserializationException(Resources.UnknownDomainIdError((object) value));
    }
  }
}
