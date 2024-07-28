// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.BufferHandlerAsync`1
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class BufferHandlerAsync<T> : IDisposable
  {
    private int _bufferSize;
    private List<T> _buffer;
    private BufferHandlerAsync<T>.BufferActionDelegate _bufferAction;

    public BufferHandlerAsync(
      int bufferSize,
      BufferHandlerAsync<T>.BufferActionDelegate bufferAction)
    {
      this._bufferSize = bufferSize;
      this._buffer = new List<T>(this._bufferSize);
      this._bufferAction = bufferAction;
    }

    public async Task ProcessAsync(IEnumerable<T> inputs)
    {
      if (inputs.Count<T>() <= this._bufferSize)
      {
        if (this.AvailableSpace > inputs.Count<T>())
        {
          this._buffer.InsertRange(this._buffer.Count, inputs);
        }
        else
        {
          int subsetSize = this.AvailableSpace;
          this._buffer.AddRange(inputs.Take<T>(subsetSize));
          await this._bufferAction((IEnumerable<T>) this._buffer);
          this._buffer.Clear();
          this._buffer.AddRange(inputs.Skip<T>(subsetSize));
        }
      }
      else
      {
        foreach (IEnumerable<T> inputs1 in inputs.Batch<T>(this._bufferSize))
          await this.ProcessAsync(inputs1);
      }
    }

    public void Dispose()
    {
      if (this._buffer.Count <= 0)
        return;
      Task task = this._bufferAction((IEnumerable<T>) this._buffer);
    }

    private int AvailableSpace => this._bufferSize - this._buffer.Count;

    public delegate Task BufferActionDelegate(IEnumerable<T> inputs);
  }
}
