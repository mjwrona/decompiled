// Decompiled with JetBrains decompiler
// Type: Nest.Specification.SecurityApi.SecurityNamespace
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nest.Specification.SecurityApi
{
  public class SecurityNamespace : Nest.NamespacedClientProxy
  {
    internal SecurityNamespace(ElasticClient client)
      : base(client)
    {
    }

    public AuthenticateResponse Authenticate(
      Func<AuthenticateDescriptor, IAuthenticateRequest> selector = null)
    {
      return this.Authenticate(selector.InvokeOrDefault<AuthenticateDescriptor, IAuthenticateRequest>(new AuthenticateDescriptor()));
    }

    public Task<AuthenticateResponse> AuthenticateAsync(
      Func<AuthenticateDescriptor, IAuthenticateRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.AuthenticateAsync(selector.InvokeOrDefault<AuthenticateDescriptor, IAuthenticateRequest>(new AuthenticateDescriptor()), ct);
    }

    public AuthenticateResponse Authenticate(IAuthenticateRequest request) => this.DoRequest<IAuthenticateRequest, AuthenticateResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<AuthenticateResponse> AuthenticateAsync(
      IAuthenticateRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IAuthenticateRequest, AuthenticateResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public ChangePasswordResponse ChangePassword(
      Func<ChangePasswordDescriptor, IChangePasswordRequest> selector)
    {
      return this.ChangePassword(selector.InvokeOrDefault<ChangePasswordDescriptor, IChangePasswordRequest>(new ChangePasswordDescriptor()));
    }

    public Task<ChangePasswordResponse> ChangePasswordAsync(
      Func<ChangePasswordDescriptor, IChangePasswordRequest> selector,
      CancellationToken ct = default (CancellationToken))
    {
      return this.ChangePasswordAsync(selector.InvokeOrDefault<ChangePasswordDescriptor, IChangePasswordRequest>(new ChangePasswordDescriptor()), ct);
    }

    public ChangePasswordResponse ChangePassword(IChangePasswordRequest request) => this.DoRequest<IChangePasswordRequest, ChangePasswordResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<ChangePasswordResponse> ChangePasswordAsync(
      IChangePasswordRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IChangePasswordRequest, ChangePasswordResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public ClearApiKeyCacheResponse ClearApiKeyCache(
      Func<ClearApiKeyCacheDescriptor, IClearApiKeyCacheRequest> selector = null)
    {
      return this.ClearApiKeyCache(selector.InvokeOrDefault<ClearApiKeyCacheDescriptor, IClearApiKeyCacheRequest>(new ClearApiKeyCacheDescriptor()));
    }

    public Task<ClearApiKeyCacheResponse> ClearApiKeyCacheAsync(
      Func<ClearApiKeyCacheDescriptor, IClearApiKeyCacheRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.ClearApiKeyCacheAsync(selector.InvokeOrDefault<ClearApiKeyCacheDescriptor, IClearApiKeyCacheRequest>(new ClearApiKeyCacheDescriptor()), ct);
    }

    public ClearApiKeyCacheResponse ClearApiKeyCache(IClearApiKeyCacheRequest request) => this.DoRequest<IClearApiKeyCacheRequest, ClearApiKeyCacheResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<ClearApiKeyCacheResponse> ClearApiKeyCacheAsync(
      IClearApiKeyCacheRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IClearApiKeyCacheRequest, ClearApiKeyCacheResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public ClearCachedPrivilegesResponse ClearCachedPrivileges(
      Names application,
      Func<ClearCachedPrivilegesDescriptor, IClearCachedPrivilegesRequest> selector = null)
    {
      return this.ClearCachedPrivileges(selector.InvokeOrDefault<ClearCachedPrivilegesDescriptor, IClearCachedPrivilegesRequest>(new ClearCachedPrivilegesDescriptor(application)));
    }

    public Task<ClearCachedPrivilegesResponse> ClearCachedPrivilegesAsync(
      Names application,
      Func<ClearCachedPrivilegesDescriptor, IClearCachedPrivilegesRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.ClearCachedPrivilegesAsync(selector.InvokeOrDefault<ClearCachedPrivilegesDescriptor, IClearCachedPrivilegesRequest>(new ClearCachedPrivilegesDescriptor(application)), ct);
    }

    public ClearCachedPrivilegesResponse ClearCachedPrivileges(IClearCachedPrivilegesRequest request) => this.DoRequest<IClearCachedPrivilegesRequest, ClearCachedPrivilegesResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<ClearCachedPrivilegesResponse> ClearCachedPrivilegesAsync(
      IClearCachedPrivilegesRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IClearCachedPrivilegesRequest, ClearCachedPrivilegesResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public ClearCachedRealmsResponse ClearCachedRealms(
      Names realms,
      Func<ClearCachedRealmsDescriptor, IClearCachedRealmsRequest> selector = null)
    {
      return this.ClearCachedRealms(selector.InvokeOrDefault<ClearCachedRealmsDescriptor, IClearCachedRealmsRequest>(new ClearCachedRealmsDescriptor(realms)));
    }

    public Task<ClearCachedRealmsResponse> ClearCachedRealmsAsync(
      Names realms,
      Func<ClearCachedRealmsDescriptor, IClearCachedRealmsRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.ClearCachedRealmsAsync(selector.InvokeOrDefault<ClearCachedRealmsDescriptor, IClearCachedRealmsRequest>(new ClearCachedRealmsDescriptor(realms)), ct);
    }

    public ClearCachedRealmsResponse ClearCachedRealms(IClearCachedRealmsRequest request) => this.DoRequest<IClearCachedRealmsRequest, ClearCachedRealmsResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<ClearCachedRealmsResponse> ClearCachedRealmsAsync(
      IClearCachedRealmsRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IClearCachedRealmsRequest, ClearCachedRealmsResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public ClearCachedRolesResponse ClearCachedRoles(
      Names name,
      Func<ClearCachedRolesDescriptor, IClearCachedRolesRequest> selector = null)
    {
      return this.ClearCachedRoles(selector.InvokeOrDefault<ClearCachedRolesDescriptor, IClearCachedRolesRequest>(new ClearCachedRolesDescriptor(name)));
    }

    public Task<ClearCachedRolesResponse> ClearCachedRolesAsync(
      Names name,
      Func<ClearCachedRolesDescriptor, IClearCachedRolesRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.ClearCachedRolesAsync(selector.InvokeOrDefault<ClearCachedRolesDescriptor, IClearCachedRolesRequest>(new ClearCachedRolesDescriptor(name)), ct);
    }

    public ClearCachedRolesResponse ClearCachedRoles(IClearCachedRolesRequest request) => this.DoRequest<IClearCachedRolesRequest, ClearCachedRolesResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<ClearCachedRolesResponse> ClearCachedRolesAsync(
      IClearCachedRolesRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IClearCachedRolesRequest, ClearCachedRolesResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public CreateApiKeyResponse CreateApiKey(
      Func<CreateApiKeyDescriptor, ICreateApiKeyRequest> selector)
    {
      return this.CreateApiKey(selector.InvokeOrDefault<CreateApiKeyDescriptor, ICreateApiKeyRequest>(new CreateApiKeyDescriptor()));
    }

    public Task<CreateApiKeyResponse> CreateApiKeyAsync(
      Func<CreateApiKeyDescriptor, ICreateApiKeyRequest> selector,
      CancellationToken ct = default (CancellationToken))
    {
      return this.CreateApiKeyAsync(selector.InvokeOrDefault<CreateApiKeyDescriptor, ICreateApiKeyRequest>(new CreateApiKeyDescriptor()), ct);
    }

    public CreateApiKeyResponse CreateApiKey(ICreateApiKeyRequest request) => this.DoRequest<ICreateApiKeyRequest, CreateApiKeyResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<CreateApiKeyResponse> CreateApiKeyAsync(
      ICreateApiKeyRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<ICreateApiKeyRequest, CreateApiKeyResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public DeletePrivilegesResponse DeletePrivileges(
      Name application,
      Name name,
      Func<DeletePrivilegesDescriptor, IDeletePrivilegesRequest> selector = null)
    {
      return this.DeletePrivileges(selector.InvokeOrDefault<DeletePrivilegesDescriptor, IDeletePrivilegesRequest>(new DeletePrivilegesDescriptor(application, name)));
    }

    public Task<DeletePrivilegesResponse> DeletePrivilegesAsync(
      Name application,
      Name name,
      Func<DeletePrivilegesDescriptor, IDeletePrivilegesRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DeletePrivilegesAsync(selector.InvokeOrDefault<DeletePrivilegesDescriptor, IDeletePrivilegesRequest>(new DeletePrivilegesDescriptor(application, name)), ct);
    }

    public DeletePrivilegesResponse DeletePrivileges(IDeletePrivilegesRequest request) => this.DoRequest<IDeletePrivilegesRequest, DeletePrivilegesResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<DeletePrivilegesResponse> DeletePrivilegesAsync(
      IDeletePrivilegesRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IDeletePrivilegesRequest, DeletePrivilegesResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public DeleteRoleResponse DeleteRole(
      Name name,
      Func<DeleteRoleDescriptor, IDeleteRoleRequest> selector = null)
    {
      return this.DeleteRole(selector.InvokeOrDefault<DeleteRoleDescriptor, IDeleteRoleRequest>(new DeleteRoleDescriptor(name)));
    }

    public Task<DeleteRoleResponse> DeleteRoleAsync(
      Name name,
      Func<DeleteRoleDescriptor, IDeleteRoleRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DeleteRoleAsync(selector.InvokeOrDefault<DeleteRoleDescriptor, IDeleteRoleRequest>(new DeleteRoleDescriptor(name)), ct);
    }

    public DeleteRoleResponse DeleteRole(IDeleteRoleRequest request) => this.DoRequest<IDeleteRoleRequest, DeleteRoleResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<DeleteRoleResponse> DeleteRoleAsync(
      IDeleteRoleRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IDeleteRoleRequest, DeleteRoleResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public DeleteRoleMappingResponse DeleteRoleMapping(
      Name name,
      Func<DeleteRoleMappingDescriptor, IDeleteRoleMappingRequest> selector = null)
    {
      return this.DeleteRoleMapping(selector.InvokeOrDefault<DeleteRoleMappingDescriptor, IDeleteRoleMappingRequest>(new DeleteRoleMappingDescriptor(name)));
    }

    public Task<DeleteRoleMappingResponse> DeleteRoleMappingAsync(
      Name name,
      Func<DeleteRoleMappingDescriptor, IDeleteRoleMappingRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DeleteRoleMappingAsync(selector.InvokeOrDefault<DeleteRoleMappingDescriptor, IDeleteRoleMappingRequest>(new DeleteRoleMappingDescriptor(name)), ct);
    }

    public DeleteRoleMappingResponse DeleteRoleMapping(IDeleteRoleMappingRequest request) => this.DoRequest<IDeleteRoleMappingRequest, DeleteRoleMappingResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<DeleteRoleMappingResponse> DeleteRoleMappingAsync(
      IDeleteRoleMappingRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IDeleteRoleMappingRequest, DeleteRoleMappingResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public DeleteUserResponse DeleteUser(
      Name username,
      Func<DeleteUserDescriptor, IDeleteUserRequest> selector = null)
    {
      return this.DeleteUser(selector.InvokeOrDefault<DeleteUserDescriptor, IDeleteUserRequest>(new DeleteUserDescriptor(username)));
    }

    public Task<DeleteUserResponse> DeleteUserAsync(
      Name username,
      Func<DeleteUserDescriptor, IDeleteUserRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DeleteUserAsync(selector.InvokeOrDefault<DeleteUserDescriptor, IDeleteUserRequest>(new DeleteUserDescriptor(username)), ct);
    }

    public DeleteUserResponse DeleteUser(IDeleteUserRequest request) => this.DoRequest<IDeleteUserRequest, DeleteUserResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<DeleteUserResponse> DeleteUserAsync(
      IDeleteUserRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IDeleteUserRequest, DeleteUserResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public DisableUserResponse DisableUser(
      Name username,
      Func<DisableUserDescriptor, IDisableUserRequest> selector = null)
    {
      return this.DisableUser(selector.InvokeOrDefault<DisableUserDescriptor, IDisableUserRequest>(new DisableUserDescriptor(username)));
    }

    public Task<DisableUserResponse> DisableUserAsync(
      Name username,
      Func<DisableUserDescriptor, IDisableUserRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DisableUserAsync(selector.InvokeOrDefault<DisableUserDescriptor, IDisableUserRequest>(new DisableUserDescriptor(username)), ct);
    }

    public DisableUserResponse DisableUser(IDisableUserRequest request) => this.DoRequest<IDisableUserRequest, DisableUserResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<DisableUserResponse> DisableUserAsync(
      IDisableUserRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IDisableUserRequest, DisableUserResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public EnableUserResponse EnableUser(
      Name username,
      Func<EnableUserDescriptor, IEnableUserRequest> selector = null)
    {
      return this.EnableUser(selector.InvokeOrDefault<EnableUserDescriptor, IEnableUserRequest>(new EnableUserDescriptor(username)));
    }

    public Task<EnableUserResponse> EnableUserAsync(
      Name username,
      Func<EnableUserDescriptor, IEnableUserRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.EnableUserAsync(selector.InvokeOrDefault<EnableUserDescriptor, IEnableUserRequest>(new EnableUserDescriptor(username)), ct);
    }

    public EnableUserResponse EnableUser(IEnableUserRequest request) => this.DoRequest<IEnableUserRequest, EnableUserResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<EnableUserResponse> EnableUserAsync(
      IEnableUserRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IEnableUserRequest, EnableUserResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public GetApiKeyResponse GetApiKey(
      Func<GetApiKeyDescriptor, IGetApiKeyRequest> selector = null)
    {
      return this.GetApiKey(selector.InvokeOrDefault<GetApiKeyDescriptor, IGetApiKeyRequest>(new GetApiKeyDescriptor()));
    }

    public Task<GetApiKeyResponse> GetApiKeyAsync(
      Func<GetApiKeyDescriptor, IGetApiKeyRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.GetApiKeyAsync(selector.InvokeOrDefault<GetApiKeyDescriptor, IGetApiKeyRequest>(new GetApiKeyDescriptor()), ct);
    }

    public GetApiKeyResponse GetApiKey(IGetApiKeyRequest request) => this.DoRequest<IGetApiKeyRequest, GetApiKeyResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<GetApiKeyResponse> GetApiKeyAsync(IGetApiKeyRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<IGetApiKeyRequest, GetApiKeyResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public GetBuiltinPrivilegesResponse GetBuiltinPrivileges(
      Func<GetBuiltinPrivilegesDescriptor, IGetBuiltinPrivilegesRequest> selector = null)
    {
      return this.GetBuiltinPrivileges(selector.InvokeOrDefault<GetBuiltinPrivilegesDescriptor, IGetBuiltinPrivilegesRequest>(new GetBuiltinPrivilegesDescriptor()));
    }

    public Task<GetBuiltinPrivilegesResponse> GetBuiltinPrivilegesAsync(
      Func<GetBuiltinPrivilegesDescriptor, IGetBuiltinPrivilegesRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.GetBuiltinPrivilegesAsync(selector.InvokeOrDefault<GetBuiltinPrivilegesDescriptor, IGetBuiltinPrivilegesRequest>(new GetBuiltinPrivilegesDescriptor()), ct);
    }

    public GetBuiltinPrivilegesResponse GetBuiltinPrivileges(IGetBuiltinPrivilegesRequest request) => this.DoRequest<IGetBuiltinPrivilegesRequest, GetBuiltinPrivilegesResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<GetBuiltinPrivilegesResponse> GetBuiltinPrivilegesAsync(
      IGetBuiltinPrivilegesRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IGetBuiltinPrivilegesRequest, GetBuiltinPrivilegesResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public GetPrivilegesResponse GetPrivileges(
      Name name = null,
      Func<GetPrivilegesDescriptor, IGetPrivilegesRequest> selector = null)
    {
      return this.GetPrivileges(selector.InvokeOrDefault<GetPrivilegesDescriptor, IGetPrivilegesRequest>(new GetPrivilegesDescriptor().Name(name)));
    }

    public Task<GetPrivilegesResponse> GetPrivilegesAsync(
      Name name = null,
      Func<GetPrivilegesDescriptor, IGetPrivilegesRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.GetPrivilegesAsync(selector.InvokeOrDefault<GetPrivilegesDescriptor, IGetPrivilegesRequest>(new GetPrivilegesDescriptor().Name(name)), ct);
    }

    public GetPrivilegesResponse GetPrivileges(IGetPrivilegesRequest request) => this.DoRequest<IGetPrivilegesRequest, GetPrivilegesResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<GetPrivilegesResponse> GetPrivilegesAsync(
      IGetPrivilegesRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IGetPrivilegesRequest, GetPrivilegesResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public GetRoleResponse GetRole(Names name = null, Func<GetRoleDescriptor, IGetRoleRequest> selector = null) => this.GetRole(selector.InvokeOrDefault<GetRoleDescriptor, IGetRoleRequest>(new GetRoleDescriptor().Name(name)));

    public Task<GetRoleResponse> GetRoleAsync(
      Names name = null,
      Func<GetRoleDescriptor, IGetRoleRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.GetRoleAsync(selector.InvokeOrDefault<GetRoleDescriptor, IGetRoleRequest>(new GetRoleDescriptor().Name(name)), ct);
    }

    public GetRoleResponse GetRole(IGetRoleRequest request) => this.DoRequest<IGetRoleRequest, GetRoleResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<GetRoleResponse> GetRoleAsync(IGetRoleRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<IGetRoleRequest, GetRoleResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public GetRoleMappingResponse GetRoleMapping(
      Names name = null,
      Func<GetRoleMappingDescriptor, IGetRoleMappingRequest> selector = null)
    {
      return this.GetRoleMapping(selector.InvokeOrDefault<GetRoleMappingDescriptor, IGetRoleMappingRequest>(new GetRoleMappingDescriptor().Name(name)));
    }

    public Task<GetRoleMappingResponse> GetRoleMappingAsync(
      Names name = null,
      Func<GetRoleMappingDescriptor, IGetRoleMappingRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.GetRoleMappingAsync(selector.InvokeOrDefault<GetRoleMappingDescriptor, IGetRoleMappingRequest>(new GetRoleMappingDescriptor().Name(name)), ct);
    }

    public GetRoleMappingResponse GetRoleMapping(IGetRoleMappingRequest request) => this.DoRequest<IGetRoleMappingRequest, GetRoleMappingResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<GetRoleMappingResponse> GetRoleMappingAsync(
      IGetRoleMappingRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IGetRoleMappingRequest, GetRoleMappingResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public GetUserAccessTokenResponse GetUserAccessToken(
      string username,
      string password,
      Func<GetUserAccessTokenDescriptor, IGetUserAccessTokenRequest> selector = null)
    {
      return this.GetUserAccessToken(selector.InvokeOrDefault<GetUserAccessTokenDescriptor, IGetUserAccessTokenRequest>(new GetUserAccessTokenDescriptor(username, password)));
    }

    public Task<GetUserAccessTokenResponse> GetUserAccessTokenAsync(
      string username,
      string password,
      Func<GetUserAccessTokenDescriptor, IGetUserAccessTokenRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.GetUserAccessTokenAsync(selector.InvokeOrDefault<GetUserAccessTokenDescriptor, IGetUserAccessTokenRequest>(new GetUserAccessTokenDescriptor(username, password)), ct);
    }

    public GetUserAccessTokenResponse GetUserAccessToken(IGetUserAccessTokenRequest request) => this.DoRequest<IGetUserAccessTokenRequest, GetUserAccessTokenResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<GetUserAccessTokenResponse> GetUserAccessTokenAsync(
      IGetUserAccessTokenRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IGetUserAccessTokenRequest, GetUserAccessTokenResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public GetUserResponse GetUser(Func<GetUserDescriptor, IGetUserRequest> selector = null) => this.GetUser(selector.InvokeOrDefault<GetUserDescriptor, IGetUserRequest>(new GetUserDescriptor()));

    public Task<GetUserResponse> GetUserAsync(
      Func<GetUserDescriptor, IGetUserRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.GetUserAsync(selector.InvokeOrDefault<GetUserDescriptor, IGetUserRequest>(new GetUserDescriptor()), ct);
    }

    public GetUserResponse GetUser(IGetUserRequest request) => this.DoRequest<IGetUserRequest, GetUserResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<GetUserResponse> GetUserAsync(IGetUserRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<IGetUserRequest, GetUserResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public GetUserPrivilegesResponse GetUserPrivileges(
      Func<GetUserPrivilegesDescriptor, IGetUserPrivilegesRequest> selector = null)
    {
      return this.GetUserPrivileges(selector.InvokeOrDefault<GetUserPrivilegesDescriptor, IGetUserPrivilegesRequest>(new GetUserPrivilegesDescriptor()));
    }

    public Task<GetUserPrivilegesResponse> GetUserPrivilegesAsync(
      Func<GetUserPrivilegesDescriptor, IGetUserPrivilegesRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.GetUserPrivilegesAsync(selector.InvokeOrDefault<GetUserPrivilegesDescriptor, IGetUserPrivilegesRequest>(new GetUserPrivilegesDescriptor()), ct);
    }

    public GetUserPrivilegesResponse GetUserPrivileges(IGetUserPrivilegesRequest request) => this.DoRequest<IGetUserPrivilegesRequest, GetUserPrivilegesResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<GetUserPrivilegesResponse> GetUserPrivilegesAsync(
      IGetUserPrivilegesRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IGetUserPrivilegesRequest, GetUserPrivilegesResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public GrantApiKeyResponse GrantApiKey(
      Func<GrantApiKeyDescriptor, IGrantApiKeyRequest> selector)
    {
      return this.GrantApiKey(selector.InvokeOrDefault<GrantApiKeyDescriptor, IGrantApiKeyRequest>(new GrantApiKeyDescriptor()));
    }

    public Task<GrantApiKeyResponse> GrantApiKeyAsync(
      Func<GrantApiKeyDescriptor, IGrantApiKeyRequest> selector,
      CancellationToken ct = default (CancellationToken))
    {
      return this.GrantApiKeyAsync(selector.InvokeOrDefault<GrantApiKeyDescriptor, IGrantApiKeyRequest>(new GrantApiKeyDescriptor()), ct);
    }

    public GrantApiKeyResponse GrantApiKey(IGrantApiKeyRequest request) => this.DoRequest<IGrantApiKeyRequest, GrantApiKeyResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<GrantApiKeyResponse> GrantApiKeyAsync(
      IGrantApiKeyRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IGrantApiKeyRequest, GrantApiKeyResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public HasPrivilegesResponse HasPrivileges(
      Func<HasPrivilegesDescriptor, IHasPrivilegesRequest> selector = null)
    {
      return this.HasPrivileges(selector.InvokeOrDefault<HasPrivilegesDescriptor, IHasPrivilegesRequest>(new HasPrivilegesDescriptor()));
    }

    public Task<HasPrivilegesResponse> HasPrivilegesAsync(
      Func<HasPrivilegesDescriptor, IHasPrivilegesRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.HasPrivilegesAsync(selector.InvokeOrDefault<HasPrivilegesDescriptor, IHasPrivilegesRequest>(new HasPrivilegesDescriptor()), ct);
    }

    public HasPrivilegesResponse HasPrivileges(IHasPrivilegesRequest request) => this.DoRequest<IHasPrivilegesRequest, HasPrivilegesResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<HasPrivilegesResponse> HasPrivilegesAsync(
      IHasPrivilegesRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IHasPrivilegesRequest, HasPrivilegesResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public InvalidateApiKeyResponse InvalidateApiKey(
      Func<InvalidateApiKeyDescriptor, IInvalidateApiKeyRequest> selector)
    {
      return this.InvalidateApiKey(selector.InvokeOrDefault<InvalidateApiKeyDescriptor, IInvalidateApiKeyRequest>(new InvalidateApiKeyDescriptor()));
    }

    public Task<InvalidateApiKeyResponse> InvalidateApiKeyAsync(
      Func<InvalidateApiKeyDescriptor, IInvalidateApiKeyRequest> selector,
      CancellationToken ct = default (CancellationToken))
    {
      return this.InvalidateApiKeyAsync(selector.InvokeOrDefault<InvalidateApiKeyDescriptor, IInvalidateApiKeyRequest>(new InvalidateApiKeyDescriptor()), ct);
    }

    public InvalidateApiKeyResponse InvalidateApiKey(IInvalidateApiKeyRequest request) => this.DoRequest<IInvalidateApiKeyRequest, InvalidateApiKeyResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<InvalidateApiKeyResponse> InvalidateApiKeyAsync(
      IInvalidateApiKeyRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IInvalidateApiKeyRequest, InvalidateApiKeyResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public InvalidateUserAccessTokenResponse InvalidateUserAccessToken(
      string token,
      Func<InvalidateUserAccessTokenDescriptor, IInvalidateUserAccessTokenRequest> selector = null)
    {
      return this.InvalidateUserAccessToken(selector.InvokeOrDefault<InvalidateUserAccessTokenDescriptor, IInvalidateUserAccessTokenRequest>(new InvalidateUserAccessTokenDescriptor(token)));
    }

    public Task<InvalidateUserAccessTokenResponse> InvalidateUserAccessTokenAsync(
      string token,
      Func<InvalidateUserAccessTokenDescriptor, IInvalidateUserAccessTokenRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.InvalidateUserAccessTokenAsync(selector.InvokeOrDefault<InvalidateUserAccessTokenDescriptor, IInvalidateUserAccessTokenRequest>(new InvalidateUserAccessTokenDescriptor(token)), ct);
    }

    public InvalidateUserAccessTokenResponse InvalidateUserAccessToken(
      IInvalidateUserAccessTokenRequest request)
    {
      return this.DoRequest<IInvalidateUserAccessTokenRequest, InvalidateUserAccessTokenResponse>(request, (IRequestParameters) request.RequestParameters);
    }

    public Task<InvalidateUserAccessTokenResponse> InvalidateUserAccessTokenAsync(
      IInvalidateUserAccessTokenRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IInvalidateUserAccessTokenRequest, InvalidateUserAccessTokenResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public PutPrivilegesResponse PutPrivileges(
      Func<PutPrivilegesDescriptor, IPutPrivilegesRequest> selector)
    {
      return this.PutPrivileges(selector.InvokeOrDefault<PutPrivilegesDescriptor, IPutPrivilegesRequest>(new PutPrivilegesDescriptor()));
    }

    public Task<PutPrivilegesResponse> PutPrivilegesAsync(
      Func<PutPrivilegesDescriptor, IPutPrivilegesRequest> selector,
      CancellationToken ct = default (CancellationToken))
    {
      return this.PutPrivilegesAsync(selector.InvokeOrDefault<PutPrivilegesDescriptor, IPutPrivilegesRequest>(new PutPrivilegesDescriptor()), ct);
    }

    public PutPrivilegesResponse PutPrivileges(IPutPrivilegesRequest request) => this.DoRequest<IPutPrivilegesRequest, PutPrivilegesResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<PutPrivilegesResponse> PutPrivilegesAsync(
      IPutPrivilegesRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IPutPrivilegesRequest, PutPrivilegesResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public PutRoleResponse PutRole(Name name, Func<PutRoleDescriptor, IPutRoleRequest> selector) => this.PutRole(selector.InvokeOrDefault<PutRoleDescriptor, IPutRoleRequest>(new PutRoleDescriptor(name)));

    public Task<PutRoleResponse> PutRoleAsync(
      Name name,
      Func<PutRoleDescriptor, IPutRoleRequest> selector,
      CancellationToken ct = default (CancellationToken))
    {
      return this.PutRoleAsync(selector.InvokeOrDefault<PutRoleDescriptor, IPutRoleRequest>(new PutRoleDescriptor(name)), ct);
    }

    public PutRoleResponse PutRole(IPutRoleRequest request) => this.DoRequest<IPutRoleRequest, PutRoleResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<PutRoleResponse> PutRoleAsync(IPutRoleRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<IPutRoleRequest, PutRoleResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public PutRoleMappingResponse PutRoleMapping(
      Name name,
      Func<PutRoleMappingDescriptor, IPutRoleMappingRequest> selector)
    {
      return this.PutRoleMapping(selector.InvokeOrDefault<PutRoleMappingDescriptor, IPutRoleMappingRequest>(new PutRoleMappingDescriptor(name)));
    }

    public Task<PutRoleMappingResponse> PutRoleMappingAsync(
      Name name,
      Func<PutRoleMappingDescriptor, IPutRoleMappingRequest> selector,
      CancellationToken ct = default (CancellationToken))
    {
      return this.PutRoleMappingAsync(selector.InvokeOrDefault<PutRoleMappingDescriptor, IPutRoleMappingRequest>(new PutRoleMappingDescriptor(name)), ct);
    }

    public PutRoleMappingResponse PutRoleMapping(IPutRoleMappingRequest request) => this.DoRequest<IPutRoleMappingRequest, PutRoleMappingResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<PutRoleMappingResponse> PutRoleMappingAsync(
      IPutRoleMappingRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IPutRoleMappingRequest, PutRoleMappingResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public PutUserResponse PutUser(Name username, Func<PutUserDescriptor, IPutUserRequest> selector) => this.PutUser(selector.InvokeOrDefault<PutUserDescriptor, IPutUserRequest>(new PutUserDescriptor(username)));

    public Task<PutUserResponse> PutUserAsync(
      Name username,
      Func<PutUserDescriptor, IPutUserRequest> selector,
      CancellationToken ct = default (CancellationToken))
    {
      return this.PutUserAsync(selector.InvokeOrDefault<PutUserDescriptor, IPutUserRequest>(new PutUserDescriptor(username)), ct);
    }

    public PutUserResponse PutUser(IPutUserRequest request) => this.DoRequest<IPutUserRequest, PutUserResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<PutUserResponse> PutUserAsync(IPutUserRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<IPutUserRequest, PutUserResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public GetCertificatesResponse GetCertificates(
      Func<GetCertificatesDescriptor, IGetCertificatesRequest> selector = null)
    {
      return this.GetCertificates(selector.InvokeOrDefault<GetCertificatesDescriptor, IGetCertificatesRequest>(new GetCertificatesDescriptor()));
    }

    public Task<GetCertificatesResponse> GetCertificatesAsync(
      Func<GetCertificatesDescriptor, IGetCertificatesRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.GetCertificatesAsync(selector.InvokeOrDefault<GetCertificatesDescriptor, IGetCertificatesRequest>(new GetCertificatesDescriptor()), ct);
    }

    public GetCertificatesResponse GetCertificates(IGetCertificatesRequest request) => this.DoRequest<IGetCertificatesRequest, GetCertificatesResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<GetCertificatesResponse> GetCertificatesAsync(
      IGetCertificatesRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IGetCertificatesRequest, GetCertificatesResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }
  }
}
