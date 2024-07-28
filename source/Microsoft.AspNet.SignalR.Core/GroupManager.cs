// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.GroupManager
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNet.SignalR.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR
{
  public class GroupManager : IConnectionGroupManager, IGroupManager
  {
    private readonly IConnection _connection;
    private readonly string _groupPrefix;

    public GroupManager(IConnection connection, string groupPrefix)
    {
      this._connection = connection != null ? connection : throw new ArgumentNullException(nameof (connection));
      this._groupPrefix = groupPrefix;
    }

    public Task Send(string groupName, object value, params string[] excludeConnectionIds)
    {
      if (string.IsNullOrEmpty(groupName))
        throw new ArgumentException(Resources.Error_ArgumentNullOrEmpty, nameof (groupName));
      return this._connection.Send(new ConnectionMessage(this.CreateQualifiedName(groupName), value, PrefixHelper.GetPrefixedConnectionIds((IList<string>) excludeConnectionIds)));
    }

    public Task Send(IList<string> groupNames, object value, params string[] excludeConnectionIds)
    {
      if (groupNames == null)
        throw new ArgumentNullException(nameof (groupNames));
      return this._connection.Send(new ConnectionMessage((IList<string>) groupNames.Select<string, string>((Func<string, string>) (groupName => this.CreateQualifiedName(groupName))).ToList<string>(), value, PrefixHelper.GetPrefixedConnectionIds((IList<string>) excludeConnectionIds)));
    }

    public Task Add(string connectionId, string groupName)
    {
      if (connectionId == null)
        throw new ArgumentNullException(nameof (connectionId));
      Command command = groupName != null ? new Command()
      {
        CommandType = CommandType.AddToGroup,
        Value = this.CreateQualifiedName(groupName),
        WaitForAck = true
      } : throw new ArgumentNullException(nameof (groupName));
      return this._connection.Send(connectionId, (object) command);
    }

    public Task Remove(string connectionId, string groupName)
    {
      if (connectionId == null)
        throw new ArgumentNullException(nameof (connectionId));
      Command command = groupName != null ? new Command()
      {
        CommandType = CommandType.RemoveFromGroup,
        Value = this.CreateQualifiedName(groupName),
        WaitForAck = true
      } : throw new ArgumentNullException(nameof (groupName));
      return this._connection.Send(connectionId, (object) command);
    }

    private string CreateQualifiedName(string groupName) => this._groupPrefix + "." + groupName;
  }
}
