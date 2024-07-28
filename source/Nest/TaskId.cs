// Decompiled with JetBrains decompiler
// Type: Nest.TaskId
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Diagnostics;
using System.Globalization;

namespace Nest
{
  [DebuggerDisplay("{DebugDisplay,nq}")]
  [JsonFormatter(typeof (TaskIdFormatter))]
  public class TaskId : IUrlParameter, IEquatable<TaskId>
  {
    public TaskId(string taskId)
    {
      string[] strArray = !string.IsNullOrWhiteSpace(taskId) ? taskId.Split(new char[1]
      {
        ':'
      }, StringSplitOptions.RemoveEmptyEntries) : throw new ArgumentException("TaskId can not be an empty string", nameof (taskId));
      this.NodeId = strArray.Length == 2 ? strArray[0] : throw new ArgumentException("TaskId:" + taskId + " not in expected format of <node_id>:<task_id>", nameof (taskId));
      this.FullyQualifiedId = taskId;
      long result;
      if (!long.TryParse(strArray[1].Trim(), NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result) || result < -1L || result == 0L)
        throw new ArgumentException("TaskId task component:" + strArray[1] + " could not be parsed to long or is out of range", nameof (taskId));
      this.TaskNumber = result;
    }

    public string FullyQualifiedId { get; }

    public string NodeId { get; }

    public long TaskNumber { get; }

    private string DebugDisplay => this.FullyQualifiedId;

    public bool Equals(TaskId other) => this.EqualsString(other?.FullyQualifiedId);

    public string GetString(IConnectionConfigurationValues settings) => this.FullyQualifiedId;

    public override string ToString() => this.FullyQualifiedId;

    public static implicit operator TaskId(string taskId) => !taskId.IsNullOrEmpty() ? new TaskId(taskId) : (TaskId) null;

    public static bool operator ==(TaskId left, TaskId right) => object.Equals((object) left, (object) right);

    public static bool operator !=(TaskId left, TaskId right) => !object.Equals((object) left, (object) right);

    public override bool Equals(object obj)
    {
      if (obj != null && obj is string other)
        return this.EqualsString(other);
      TaskId taskId = obj as TaskId;
      return (object) taskId != null && this.EqualsString(taskId.FullyQualifiedId);
    }

    private bool EqualsString(string other) => !other.IsNullOrEmpty() && other == this.FullyQualifiedId;

    public override int GetHashCode() => this.NodeId.GetHashCode() * 397 ^ this.TaskNumber.GetHashCode();
  }
}
