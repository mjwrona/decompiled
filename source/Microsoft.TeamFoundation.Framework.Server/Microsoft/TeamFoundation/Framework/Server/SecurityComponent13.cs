// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SecurityComponent13
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class SecurityComponent13 : SecurityComponent12
  {
    public override int RemovePermissions(
      Guid namespaceId,
      string token,
      IEnumerable<Guid> teamFoundationIds,
      char separator)
    {
      this.PrepareStoredProcedure("prc_RemoveAccessControlEntry");
      this.BindServiceHostId();
      this.BindGuid("@namespaceGuid", namespaceId);
      this.BindInt("@dataspaceId", this.GetDataspaceIdForToken(token));
      this.BindGuidTable("@identityList", teamFoundationIds);
      this.BindString("@token", token, 4000, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindString("@separator", separator.ToString(), 1, true, SqlDbType.NVarChar);
      this.BindGuid("@writerIdentifier", this.Author);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_RemoveAccessControlEntry", this.RequestContext);
      resultCollection.AddBinder<int>((ObjectBinder<int>) new SecurityComponent.ChangeIdColumns());
      return resultCollection.GetCurrent<int>().First<int>();
    }

    public override int RemoveAccessControlLists(
      Guid namespaceId,
      IEnumerable<string> tokens,
      bool recurse,
      char separator)
    {
      this.PrepareStoredProcedure("prc_RemoveAccessControlList");
      this.BindServiceHostId();
      this.BindGuid("@namespaceGuid", namespaceId);
      this.BindTokenTable("@deleteToken", tokens, separator, recurse);
      this.BindString("@separator", separator.ToString((IFormatProvider) CultureInfo.InvariantCulture), 1, true, SqlDbType.NVarChar);
      this.BindGuid("@writerIdentifier", this.Author);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_RemoveAccessControlList", this.RequestContext);
      resultCollection.AddBinder<int>((ObjectBinder<int>) new SecurityComponent.ChangeIdColumns());
      return resultCollection.GetCurrent<int>().First<int>();
    }
  }
}
