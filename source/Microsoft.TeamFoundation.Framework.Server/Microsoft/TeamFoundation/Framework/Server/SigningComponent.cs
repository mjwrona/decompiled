// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SigningComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class SigningComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[8]
    {
      (IComponentCreator) new ComponentCreator<SigningComponent>(1, true),
      (IComponentCreator) new ComponentCreator<SigningComponent2>(2),
      (IComponentCreator) new ComponentCreator<SigningComponent3>(3),
      (IComponentCreator) new ComponentCreator<SigningComponent4>(4),
      (IComponentCreator) new ComponentCreator<SigningComponent5>(5),
      (IComponentCreator) new ComponentCreator<SigningComponent6>(6),
      (IComponentCreator) new ComponentCreator<SigningComponent7>(7),
      (IComponentCreator) new ComponentCreator<SigningComponent8>(8)
    }, "Signing");

    public virtual SigningComponent.SigningServiceKey GetPrivateKey(Guid identifier)
    {
      this.PrepareStoredProcedure("prc_GetSigningKey");
      this.BindGuid("@id", identifier);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<SigningComponent.SigningServiceKey>((ObjectBinder<SigningComponent.SigningServiceKey>) new SigningKeyColumns());
        List<SigningComponent.SigningServiceKey> items = resultCollection.GetCurrent<SigningComponent.SigningServiceKey>().Items;
        return items != null && items.Count > 0 ? items[0] : (SigningComponent.SigningServiceKey) null;
      }
    }

    public virtual void RemoveKeys(IList<Guid> identifiers) => throw new ServiceVersionNotSupportedException(nameof (SigningComponent), this.Version, 5);

    public virtual SigningComponent.SigningServiceKey SetPrivateKey(
      Guid identifier,
      SigningKeyType keyType,
      byte[] storedKeyData,
      bool overwriteExisting)
    {
      this.PrepareStoredProcedure("prc_SetSigningKey");
      this.BindGuid("@id", identifier);
      this.BindBinary("@privateKey", storedKeyData, SqlDbType.VarBinary);
      this.ExecuteNonQuery();
      return new SigningComponent.SigningServiceKey()
      {
        KeyType = SigningKeyType.RSAStored,
        Identifier = identifier,
        KeyData = storedKeyData
      };
    }

    public virtual List<SigningComponent.SigningServiceKey> GetPrivateKeysByIds(
      IEnumerable<Guid> identifiers)
    {
      return identifiers.Select<Guid, SigningComponent.SigningServiceKey>((System.Func<Guid, SigningComponent.SigningServiceKey>) (id => this.GetPrivateKey(id))).ToList<SigningComponent.SigningServiceKey>();
    }

    public virtual int GetSigningKeyTypeCount(SigningKeyType keyType) => -1;

    internal class SigningServiceKey
    {
      internal SigningKeyType KeyType;
      internal Guid Identifier;
      internal byte[] KeyData;
    }
  }
}
