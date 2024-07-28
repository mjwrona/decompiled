// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.ContentIdentification
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.VisualStudio.Services.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class ContentIdentification : IDisposable
  {
    private const int m_pagesPerBlock = 32;
    private const int m_bytesPerPage = 65536;
    private byte[] m_blockBuffer;
    private byte[] m_pageBuffer;
    private byte[] m_startingConstant = Encoding.ASCII.GetBytes("VSO Content Identifier Seed");
    private SHA256CryptoServiceProvider m_hashProvider = new SHA256CryptoServiceProvider();

    public ContentIdentification(int pageSize = 65536, int pagesPerBlock = 32)
      : this(new byte[pageSize * pagesPerBlock], new byte[pageSize], pageSize, pagesPerBlock)
    {
      this.PageSize = pageSize;
      this.PagesPerBlock = pagesPerBlock;
    }

    public ContentIdentification(
      byte[] blockBuffer,
      byte[] pageBuffer,
      int pageSize = 65536,
      int pagesPerBlock = 32)
    {
      this.m_blockBuffer = blockBuffer;
      this.m_pageBuffer = pageBuffer;
      this.PageSize = pageSize;
      this.PagesPerBlock = pagesPerBlock;
    }

    public int PageSize { get; private set; }

    public int PagesPerBlock { get; private set; }

    public int BlockSize => this.PageSize * this.PagesPerBlock;

    public byte[] CalculateContentIdentifier(
      Stream blocks,
      bool includesFinalBlock,
      byte[] startingContentIdentifier = null)
    {
      byte[] previousRollingIdentifier1 = startingContentIdentifier != null ? startingContentIdentifier : this.m_startingConstant;
      int blockLength = blocks.Read(this.m_blockBuffer, 0, this.BlockSize);
      int num;
      byte[] previousRollingIdentifier2;
      byte[] singleBlockIdentifier;
      do
      {
        num = blockLength;
        previousRollingIdentifier2 = previousRollingIdentifier1;
        singleBlockIdentifier = this.CalculateSingleBlockIdentifier(this.m_blockBuffer, blockLength);
        previousRollingIdentifier1 = this.CalculateRollingBlockIdentifier(singleBlockIdentifier, previousRollingIdentifier1, false);
        blockLength = blocks.Read(this.m_blockBuffer, 0, this.BlockSize);
      }
      while (blockLength > 0);
      if (includesFinalBlock)
        previousRollingIdentifier1 = this.CalculateRollingBlockIdentifier(singleBlockIdentifier, previousRollingIdentifier2, true);
      else if (num != this.BlockSize)
        throw new ArgumentException(CommonResources.ContentIdCalculationBlockSizeError((object) this.BlockSize), nameof (blocks));
      return previousRollingIdentifier1;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public byte[] CalculateContentIdentifier(
      byte[] block,
      bool isFinalBlock,
      byte[] startingContentIdentifier = null)
    {
      if (block.Length != this.BlockSize && !isFinalBlock || block.Length > this.BlockSize & isFinalBlock)
        throw new ArgumentException(CommonResources.ContentIdCalculationBlockSizeError((object) this.BlockSize), nameof (block));
      byte[] previousRollingIdentifier = startingContentIdentifier != null ? startingContentIdentifier : this.m_startingConstant;
      return this.CalculateRollingBlockIdentifier(this.CalculateSingleBlockIdentifier(block, block.Length), previousRollingIdentifier, isFinalBlock);
    }

    private byte[] CalculateRollingBlockIdentifier(
      byte[] currentBlockIdentifier,
      byte[] previousRollingIdentifier,
      bool isFinalBlock)
    {
      List<byte> buffer = new List<byte>((IEnumerable<byte>) previousRollingIdentifier);
      buffer.AddRange((IEnumerable<byte>) currentBlockIdentifier);
      buffer.Add(Convert.ToByte(isFinalBlock));
      return this.CalculateHash(buffer);
    }

    private byte[] CalculateSingleBlockIdentifier(byte[] block, int blockLength)
    {
      int num1 = 0;
      List<byte> buffer = new List<byte>();
      while (blockLength > num1 * this.PageSize)
      {
        int num2 = Math.Min(blockLength - num1 * this.PageSize, this.PageSize);
        Array.Copy((Array) block, num1 * this.PageSize, (Array) this.m_pageBuffer, 0, num2);
        byte[] hash = this.CalculateHash(this.m_pageBuffer, num2);
        ++num1;
        buffer.AddRange((IEnumerable<byte>) hash);
        if (num1 > this.PagesPerBlock)
          throw new ArgumentException(CommonResources.ContentIdCalculationBlockSizeError((object) this.BlockSize), nameof (block));
      }
      return this.CalculateHash(buffer);
    }

    private byte[] CalculateHash(byte[] buffer, int count) => this.m_hashProvider.ComputeHash(buffer, 0, count);

    private byte[] CalculateHash(List<byte> buffer) => this.m_hashProvider.ComputeHash(buffer.ToArray(), 0, buffer.Count);

    public void Dispose() => this.Dispose(true);

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing || this.m_hashProvider == null)
        return;
      this.m_hashProvider.Dispose();
    }
  }
}
