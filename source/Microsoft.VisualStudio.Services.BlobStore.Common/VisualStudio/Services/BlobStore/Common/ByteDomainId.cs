// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.ByteDomainId
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using Microsoft.VisualStudio.Services.Content.Common.MultiDomainExceptions;
using Newtonsoft.Json;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  [CLSCompliant(false)]
  [Serializable]
  public class ByteDomainId : IDomainId
  {
    internal const byte Original = 0;
    private const byte MaxDomainId = 99;

    private ByteDomainId()
    {
    }

    public static ByteDomainId Deserialize(string value)
    {
      byte result;
      string error;
      bool isUnSupportedDomainId;
      if (ByteDomainId.TryParse(value, out result, out error, out isUnSupportedDomainId))
        return new ByteDomainId(result);
      if (isUnSupportedDomainId)
        throw new InvalidDomainIdException(error);
      throw new DomainIdDeserializationException(error);
    }

    public static bool TryParse(
      string input,
      out byte result,
      out string error,
      out bool isUnSupportedDomainId)
    {
      result = (byte) 0;
      isUnSupportedDomainId = false;
      if (input == null)
      {
        error = Resources.DomainIdNullError();
        return false;
      }
      if (byte.TryParse(input, out result))
        return ByteDomainId.TryValidate(result, out error, out isUnSupportedDomainId);
      error = Resources.DomainIdParseError();
      return false;
    }

    public static bool TryValidate(byte input, out string error, out bool isUnSupportedDomainId)
    {
      isUnSupportedDomainId = false;
      if (input > (byte) 99)
      {
        error = Resources.DomainIdLengthError();
        isUnSupportedDomainId = true;
        return false;
      }
      error = (string) null;
      return true;
    }

    public byte Id { get; }

    [JsonConstructor]
    public ByteDomainId(byte id)
    {
      string error;
      this.Id = ByteDomainId.TryValidate(id, out error, out bool _) ? id : throw new InvalidDomainIdException(error);
    }

    public override bool Equals(IDomainId other) => other is ByteDomainId byteDomainId && (int) this.Id == (int) byteDomainId.Id;

    public override int GetHashCode() => this.Id.GetHashCode();

    public override string Serialize() => this.Id.ToString();
  }
}
