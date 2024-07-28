// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.AuthorizeAttribute
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace Microsoft.AspNet.SignalR
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
  public class AuthorizeAttribute : Attribute, IAuthorizeHubConnection, IAuthorizeHubMethodInvocation
  {
    private string _roles;
    private string[] _rolesSplit = new string[0];
    private string _users;
    private string[] _usersSplit = new string[0];
    protected bool? _requireOutgoing;

    public bool RequireOutgoing
    {
      get => throw new NotImplementedException(Resources.Error_DoNotReadRequireOutgoing);
      set => this._requireOutgoing = new bool?(value);
    }

    public string Roles
    {
      get => this._roles ?? string.Empty;
      set
      {
        this._roles = value;
        this._rolesSplit = AuthorizeAttribute.SplitString(value);
      }
    }

    public string Users
    {
      get => this._users ?? string.Empty;
      set
      {
        this._users = value;
        this._usersSplit = AuthorizeAttribute.SplitString(value);
      }
    }

    public virtual bool AuthorizeHubConnection(HubDescriptor hubDescriptor, IRequest request)
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      return this._requireOutgoing.HasValue && !this._requireOutgoing.Value || this.UserAuthorized(request.User);
    }

    public virtual bool AuthorizeHubMethodInvocation(
      IHubIncomingInvokerContext hubIncomingInvokerContext,
      bool appliesToMethod)
    {
      if (hubIncomingInvokerContext == null)
        throw new ArgumentNullException(nameof (hubIncomingInvokerContext));
      if (appliesToMethod)
      {
        bool? requireOutgoing = this._requireOutgoing;
        bool flag = true;
        if (requireOutgoing.GetValueOrDefault() == flag & requireOutgoing.HasValue)
          throw new ArgumentException(Resources.Error_MethodLevelOutgoingAuthorization);
      }
      return this.UserAuthorized(hubIncomingInvokerContext.Hub.Context.User);
    }

    protected virtual bool UserAuthorized(IPrincipal user) => user != null && user.Identity.IsAuthenticated && (this._usersSplit.Length == 0 || ((IEnumerable<string>) this._usersSplit).Contains<string>(user.Identity.Name, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)) && (this._rolesSplit.Length == 0 || ((IEnumerable<string>) this._rolesSplit).Any<string>(new Func<string, bool>(user.IsInRole)));

    private static string[] SplitString(string original)
    {
      if (string.IsNullOrEmpty(original))
        return new string[0];
      return ((IEnumerable<string>) original.Split(',')).Select(piece => new
      {
        piece = piece,
        trimmed = piece.Trim()
      }).Where(_param1 => !string.IsNullOrEmpty(_param1.trimmed)).Select(_param1 => _param1.trimmed).ToArray<string>();
    }
  }
}
