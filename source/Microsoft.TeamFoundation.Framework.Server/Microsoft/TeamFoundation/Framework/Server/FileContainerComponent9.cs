// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileContainerComponent9
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.FileContainer;
using System;
using System.Data;
using System.Globalization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FileContainerComponent9 : FileContainerComponent8
  {
    public override long AddContainer(
      Uri artifactUri,
      string securityToken,
      string name,
      string description,
      ContainerOptions options,
      Guid dataspaceIdentifier,
      string locatorPath)
    {
      this.TraceEnter(0, nameof (AddContainer));
      this.PrepareStoredProcedure("prc_CreateContainer");
      this.BindString("@artifactUri", artifactUri.ToString(), 128, false, SqlDbType.NVarChar);
      this.BindString("@securityToken", securityToken, -1, false, SqlDbType.NVarChar);
      this.BindString("@name", name, 260, false, SqlDbType.NVarChar);
      this.BindString("@description", description, 2048, true, SqlDbType.NVarChar);
      this.BindByte("@options", (byte) options);
      this.BindNullableGuid("@signingKeyId", Guid.Empty);
      this.BindGuid("@createdBy", this.Author);
      this.BindDataspace(dataspaceIdentifier);
      this.BindString("@locatorPath", locatorPath, 260, true, SqlDbType.NVarChar);
      long int64 = Convert.ToInt64(this.ExecuteScalar(), (IFormatProvider) CultureInfo.InvariantCulture);
      this.TraceLeave(0, nameof (AddContainer));
      return int64;
    }

    internal override ContainerBinder GetFileContainerBinder() => (ContainerBinder) new ContainerBinder3((TeamFoundationSqlResourceComponent) this);
  }
}
