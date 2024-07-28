// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.ProcessTemplates.LegacyProcessPackage
// Assembly: Microsoft.Azure.Boards.ProcessTemplates, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A94E8BA8-9851-4F5D-B619-9CF2FFF5B128
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.ProcessTemplates.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace Microsoft.Azure.Boards.ProcessTemplates
{
  public sealed class LegacyProcessPackage : IProcessTemplate
  {
    private IDictionary<string, byte[]> m_resources;

    private LegacyProcessPackage(
      ProcessDescriptor descriptor,
      IDictionary<string, byte[]> resources)
    {
      ArgumentUtility.CheckForNull<ProcessDescriptor>(descriptor, nameof (descriptor));
      ArgumentUtility.CheckForNull<IDictionary<string, byte[]>>(resources, nameof (resources));
      this.Descriptor = descriptor;
      this.m_resources = resources;
    }

    public ProcessDescriptor Descriptor { get; private set; }

    public LegacyProcessPackage CreateClone(ProcessDescriptor derivedProcess)
    {
      LegacyProcessPackage clone = this.MemberwiseClone() as LegacyProcessPackage;
      clone.Descriptor = derivedProcess;
      return clone;
    }

    public static LegacyProcessPackage Load(ProcessDescriptor descriptor, Stream zipContentStream)
    {
      ArgumentUtility.CheckForNull<ProcessDescriptor>(descriptor, nameof (descriptor));
      ArgumentUtility.CheckForNull<Stream>(zipContentStream, nameof (zipContentStream));
      Dictionary<string, byte[]> resources = new Dictionary<string, byte[]>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      using (ZipArchive zipArchive = new ZipArchive(zipContentStream, ZipArchiveMode.Read, true))
      {
        foreach (ZipArchiveEntry entry in zipArchive.Entries)
        {
          using (Stream stream = entry.Open())
          {
            byte[] buffer = new byte[(int) entry.Length];
            stream.Read(buffer, 0, buffer.Length);
            resources.Add(entry.FullName.Replace('/', '\\'), buffer);
          }
        }
      }
      return new LegacyProcessPackage(descriptor, (IDictionary<string, byte[]>) resources);
    }

    public Stream GetResource(string resourceName) => this.GetResource(resourceName, out byte[] _, out long _);

    public Stream GetResource(string resourceName, out byte[] hashValue, out long contentLength)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(resourceName, nameof (resourceName));
      byte[] numArray;
      if (this.m_resources.TryGetValue(resourceName, out numArray))
      {
        contentLength = (long) numArray.Length;
        hashValue = MD5Util.CalculateMD5(numArray);
        return (Stream) new MemoryStream(numArray, false);
      }
      contentLength = 0L;
      hashValue = (byte[]) null;
      return (Stream) null;
    }
  }
}
