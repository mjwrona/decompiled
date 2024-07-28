// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Infrastructure.Connection
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Json;
using Microsoft.AspNet.SignalR.Messaging;
using Microsoft.AspNet.SignalR.Tracing;
using Microsoft.AspNet.SignalR.Transports;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.Infrastructure
{
  public class Connection : IConnection, ITransportConnection, ISubscriber
  {
    private readonly IMessageBus _bus;
    private readonly JsonSerializer _serializer;
    private readonly string _baseSignal;
    private readonly string _connectionId;
    private readonly IList<string> _signals;
    private readonly DiffSet<string> _groups;
    private readonly IPerformanceCounterManager _counters;
    private bool _aborted;
    private bool _initializing;
    private readonly TraceSource _traceSource;
    private readonly IAckHandler _ackHandler;
    private readonly IProtectedData _protectedData;
    private readonly Func<Message, bool> _excludeMessage;
    private readonly IMemoryPool _pool;

    public Connection(
      IMessageBus newMessageBus,
      JsonSerializer jsonSerializer,
      string baseSignal,
      string connectionId,
      IList<string> signals,
      IList<string> groups,
      ITraceManager traceManager,
      IAckHandler ackHandler,
      IPerformanceCounterManager performanceCounterManager,
      IProtectedData protectedData,
      IMemoryPool pool)
    {
      if (traceManager == null)
        throw new ArgumentNullException(nameof (traceManager));
      this._bus = newMessageBus;
      this._serializer = jsonSerializer;
      this._baseSignal = baseSignal;
      this._connectionId = connectionId;
      this._signals = (IList<string>) new List<string>(signals.Concat<string>((IEnumerable<string>) groups));
      this._groups = new DiffSet<string>((IEnumerable<string>) groups);
      this._traceSource = traceManager["SignalR.Connection"];
      this._ackHandler = ackHandler;
      this._counters = performanceCounterManager;
      this._protectedData = protectedData;
      this._excludeMessage = (Func<Message, bool>) (m => this.ExcludeMessage(m));
      this._pool = pool;
    }

    public string DefaultSignal => this._baseSignal;

    IList<string> ISubscriber.EventKeys => this._signals;

    public event Action<ISubscriber, string> EventKeyAdded;

    public event Action<ISubscriber, string> EventKeyRemoved;

    public Action<TextWriter> WriteCursor { get; set; }

    public string Identity => this._connectionId;

    private TraceSource Trace => this._traceSource;

    public Subscription Subscription { get; set; }

    public Task Send(ConnectionMessage message)
    {
      if (!string.IsNullOrEmpty(message.Signal) && message.Signals != null)
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_AmbiguousMessage, new object[2]
        {
          (object) message.Signal,
          (object) string.Join(", ", (IEnumerable<string>) message.Signals)
        }));
      if (message.Signals != null)
        return this.MultiSend(message.Signals, message.Value, message.ExcludedSignals);
      Message message1 = this.CreateMessage(message.Signal, message.Value);
      message1.Filter = Connection.GetFilter(message.ExcludedSignals);
      if (!message1.WaitForAck)
        return this._bus.Publish(message1);
      Task ack = this._ackHandler.CreateAck(message1.CommandId);
      return this._bus.Publish(message1).Then<Task>((Func<Task, Task>) (task => task), ack);
    }

    private Task MultiSend(IList<string> signals, object value, IList<string> excludedSignals)
    {
      if (signals.Count == 0)
        return TaskAsyncHelper.Empty;
      ArraySegment<byte> messageBuffer = this.GetMessageBuffer(value);
      string filter = Connection.GetFilter(excludedSignals);
      Task[] taskArray = new Task[signals.Count];
      for (int index = 0; index < signals.Count; ++index)
      {
        Message message = new Message(this._connectionId, signals[index], messageBuffer);
        if (!string.IsNullOrEmpty(filter))
          message.Filter = filter;
        taskArray[index] = this._bus.Publish(message);
      }
      return Task.WhenAll(taskArray);
    }

    private static string GetFilter(IList<string> excludedSignals) => excludedSignals != null ? string.Join("|", (IEnumerable<string>) excludedSignals) : (string) null;

    private Message CreateMessage(string key, object value)
    {
      ArraySegment<byte> messageBuffer = this.GetMessageBuffer(value);
      Message message = new Message(this._connectionId, key, messageBuffer);
      if (value is Command command)
      {
        message.CommandId = command.Id;
        message.WaitForAck = command.WaitForAck;
      }
      return message;
    }

    private ArraySegment<byte> GetMessageBuffer(object value)
    {
      if (!(value is ArraySegment<byte> messageBuffer))
        messageBuffer = this.SerializeMessageValue(value);
      return messageBuffer;
    }

    private ArraySegment<byte> SerializeMessageValue(object value)
    {
      using (MemoryPoolTextWriter writer = new MemoryPoolTextWriter(this._pool))
      {
        this._serializer.Serialize(value, (TextWriter) writer);
        writer.Flush();
        ArraySegment<byte> buffer = writer.Buffer;
        byte[] numArray = new byte[buffer.Count];
        Buffer.BlockCopy((Array) buffer.Array, buffer.Offset, (Array) numArray, 0, buffer.Count);
        return new ArraySegment<byte>(numArray);
      }
    }

    public IDisposable Receive(
      string messageId,
      Func<PersistentResponse, object, Task<bool>> callback,
      int maxMessages,
      object state)
    {
      Connection.ReceiveContext state1 = new Connection.ReceiveContext(this, callback, state);
      return this._bus.Subscribe((ISubscriber) this, messageId, (Func<MessageResult, object, Task<bool>>) ((result, s) => Connection.MessageBusCallback(result, s)), maxMessages, (object) state1);
    }

    private static Task<bool> MessageBusCallback(MessageResult result, object state) => ((Connection.ReceiveContext) state).InvokeCallback(result);

    private PersistentResponse GetResponse(MessageResult result)
    {
      this.ProcessResults(result);
      PersistentResponse response = new PersistentResponse(this._excludeMessage, this.WriteCursor);
      response.Terminal = result.Terminal;
      if (!result.Terminal)
      {
        response.Messages = result.Messages;
        response.Aborted = this._aborted;
        response.TotalCount = result.TotalCount;
        response.Initializing = this._initializing;
        this._initializing = false;
      }
      this.PopulateResponseState(response);
      this._counters.ConnectionMessagesReceivedTotal.IncrementBy((long) result.TotalCount);
      this._counters.ConnectionMessagesReceivedPerSec.IncrementBy((long) result.TotalCount);
      return response;
    }

    private bool ExcludeMessage(Message message)
    {
      if (string.IsNullOrEmpty(message.Filter))
        return false;
      return ((IEnumerable<string>) message.Filter.Split('|')).Any<string>((Func<string, bool>) (signal => this.Identity.Equals(signal, StringComparison.OrdinalIgnoreCase) || this._signals.Contains(signal) || this._groups.Contains(signal)));
    }

    private void ProcessResults(MessageResult result) => result.Messages.Enumerate<Connection>((Func<Message, bool>) (message => message.IsCommand), (Action<Connection, Message>) ((connection, message) => Connection.ProcessResultsCore(connection, message)), this);

    private static void ProcessResultsCore(Connection connection, Message message)
    {
      if (message.IsAck)
      {
        connection.Trace.TraceError("Connection {0} received an unexpected ACK message.", (object) connection.Identity);
      }
      else
      {
        Command command = connection._serializer.Parse<Command>(message.Value, message.Encoding);
        connection.ProcessCommand(command);
        if (!message.WaitForAck)
          return;
        connection._bus.Ack(connection._connectionId, message.CommandId).Catch<Task>(connection._traceSource);
      }
    }

    private void ProcessCommand(Command command)
    {
      switch (command.CommandType)
      {
        case CommandType.Initializing:
          this._initializing = true;
          break;
        case CommandType.AddToGroup:
          string str1 = command.Value;
          if (this.EventKeyAdded == null)
            break;
          this._groups.Add(str1);
          this.EventKeyAdded((ISubscriber) this, str1);
          break;
        case CommandType.RemoveFromGroup:
          string str2 = command.Value;
          if (this.EventKeyRemoved == null)
            break;
          this._groups.Remove(str2);
          this.EventKeyRemoved((ISubscriber) this, str2);
          break;
        case CommandType.Abort:
          this._aborted = true;
          break;
      }
    }

    private void PopulateResponseState(PersistentResponse response) => Connection.PopulateResponseState(response, this._groups, this._serializer, this._protectedData, this._connectionId);

    internal static void PopulateResponseState(
      PersistentResponse response,
      DiffSet<string> groupSet,
      JsonSerializer serializer,
      IProtectedData protectedData,
      string connectionId)
    {
      if (!groupSet.DetectChanges())
        return;
      IEnumerable<string> snapshot = (IEnumerable<string>) groupSet.GetSnapshot();
      string data = connectionId + ":" + serializer.Stringify((object) PrefixHelper.RemoveGroupPrefixes(snapshot));
      response.GroupsToken = protectedData.Protect(data, "SignalR.Groups.v1.1");
    }

    private class ReceiveContext
    {
      private readonly Connection _connection;
      private readonly Func<PersistentResponse, object, Task<bool>> _callback;
      private readonly object _callbackState;

      public ReceiveContext(
        Connection connection,
        Func<PersistentResponse, object, Task<bool>> callback,
        object callbackState)
      {
        this._connection = connection;
        this._callback = callback;
        this._callbackState = callbackState;
      }

      public Task<bool> InvokeCallback(MessageResult result) => this._callback(this._connection.GetResponse(result), this._callbackState);
    }
  }
}
