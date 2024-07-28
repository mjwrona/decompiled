// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Permission
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  public abstract class Permission
  {
    public abstract string Id { get; }

    public abstract Task<PermissionResponse> ReadAsync(
      int? tokenExpiryInSeconds = null,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<PermissionResponse> ReplaceAsync(
      PermissionProperties permissionProperties,
      int? tokenExpiryInSeconds = null,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<PermissionResponse> DeleteAsync(
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));
  }
}
