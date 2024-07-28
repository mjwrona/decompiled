// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.MasterWrappingKeySigningServiceKey
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Text;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class MasterWrappingKeySigningServiceKey : SigningServiceKey
  {
    internal MasterWrappingKeySigningServiceKey(Guid identifier)
      : base(SigningKeyType.MasterWrappingKey, identifier)
    {
    }

    public string GetKeyUrl() => Encoding.ASCII.GetString(this.KeyData);

    protected override byte[] GetKeyData(IVssRequestContext requestContext)
    {
      this.CheckKeyDataIsNotNull();
      return this.KeyData;
    }

    protected override void StoreKeyData(
      IVssRequestContext requestContext,
      byte[] rawKeyData,
      bool overwriteExisting)
    {
      this.CheckKeyDataIsNull();
      this.KeyData = this.StoreKeyDataToDatabase(requestContext, rawKeyData, overwriteExisting).KeyData;
    }

    protected override void DeleteKeyData(IVssRequestContext requestContext)
    {
    }
  }
}
