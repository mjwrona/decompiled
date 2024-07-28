// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataVersionCache`1
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;

namespace Microsoft.OData
{
  internal sealed class ODataVersionCache<T>
  {
    private readonly SimpleLazy<T> v4;
    private readonly SimpleLazy<T> v401;

    internal ODataVersionCache(Func<ODataVersion, T> factory)
    {
      this.v4 = new SimpleLazy<T>((Func<T>) (() => factory(ODataVersion.V4)), true);
      this.v401 = new SimpleLazy<T>((Func<T>) (() => factory(ODataVersion.V401)), true);
    }

    internal T this[ODataVersion version]
    {
      get
      {
        if (version == ODataVersion.V4)
          return this.v4.Value;
        if (version == ODataVersion.V401)
          return this.v401.Value;
        throw new ODataException(Strings.General_InternalError((object) InternalErrorCodes.ODataVersionCache_UnknownVersion));
      }
    }
  }
}
