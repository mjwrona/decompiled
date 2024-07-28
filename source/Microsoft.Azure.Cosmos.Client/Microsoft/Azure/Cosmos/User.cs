// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.User
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  public abstract class User
  {
    public abstract string Id { get; }

    public abstract Task<UserResponse> ReadAsync(
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<UserResponse> ReplaceAsync(
      UserProperties userProperties,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<UserResponse> DeleteAsync(
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Permission GetPermission(string id);

    public abstract Task<PermissionResponse> CreatePermissionAsync(
      PermissionProperties permissionProperties,
      int? tokenExpiryInSeconds = null,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<PermissionResponse> UpsertPermissionAsync(
      PermissionProperties permissionProperties,
      int? tokenExpiryInSeconds = null,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract FeedIterator<T> GetPermissionQueryIterator<T>(
      string queryText = null,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null);

    public abstract FeedIterator<T> GetPermissionQueryIterator<T>(
      QueryDefinition queryDefinition,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null);
  }
}
