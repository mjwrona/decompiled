// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Memory.ByteArray
// Assembly: Microsoft.VisualStudio.Services.Search.ReSearch.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 657A74AE-F2A6-4615-BB2F-7FA1F961B173
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.ReSearch.Core.dll

using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Index;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Memory
{
  public class ByteArray : ByteView
  {
    private HashSet<ByteArray> m_children = new HashSet<ByteArray>();
    private ByteArray m_parent;

    protected byte[] BackingArray { get; set; }

    protected GCHandle ArrayPinHandle { get; set; }

    public unsafe long Size => this.ArrayEnd - this.ArrayStart;

    public ByteArray(uint size)
    {
      this.BackingArray = new byte[(int) size];
      this.InitializePointers();
    }

    public unsafe ByteArray(ByteArray other)
    {
      this.BackingArray = other != null ? other.BackingArray : throw new ArgumentNullException(nameof (other));
      this.ArrayStart = other.ArrayStart;
      this.ArrayEnd = other.ArrayEnd;
      this.ArrayPosition = other.ArrayPosition;
      this.SetParent(other);
    }

    private void SetParent(ByteArray parent)
    {
      this.m_parent = parent;
      parent.m_children.Add(this);
    }

    public ByteArray(byte[] array)
    {
      this.BackingArray = array;
      this.InitializePointers();
    }

    ~ByteArray() => this.Dispose(false);

    public byte[] GetArray() => this.BackingArray;

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (this.m_parent != null)
          this.m_parent.m_children.Remove(this);
        else
          IndexException.AssertEqual<int>(0, this.m_children.Count, "Disposing ByteArray with children views still open.");
      }
      if (this.ArrayPinHandle.IsAllocated)
        this.ArrayPinHandle.Free();
      base.Dispose(disposing);
    }

    protected unsafe void InitializePointers()
    {
      this.ArrayPinHandle = GCHandle.Alloc((object) this.BackingArray, GCHandleType.Pinned);
      this.ArrayStart = (byte*) this.ArrayPinHandle.AddrOfPinnedObject().ToPointer();
      this.ArrayEnd = this.ArrayStart + this.BackingArray.Length;
      this.ArrayPosition = this.ArrayStart;
      foreach (ByteArray child in this.m_children)
        child.UpdateView();
    }

    private unsafe void UpdateView()
    {
      uint position = this.Position;
      this.BackingArray = this.m_parent.BackingArray;
      this.ArrayStart = this.m_parent.ArrayStart;
      this.ArrayEnd = this.m_parent.ArrayEnd;
      this.Position = position;
    }
  }
}
