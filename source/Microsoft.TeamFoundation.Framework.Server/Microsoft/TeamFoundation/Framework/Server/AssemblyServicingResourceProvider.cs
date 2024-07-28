// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AssemblyServicingResourceProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class AssemblyServicingResourceProvider : MarshalByRefObject, IServicingResourceProvider
  {
    private const string c_gzipSuffix = "(gz)";
    private const string c_sqlAzureSuffix = "(SqlAzure)";
    private const string c_servicingDataDefaultNamespace = "ServicingData";
    private string m_servicingFilesPath;
    private Assembly[] m_servicingDataAssemblies;
    private string[] m_resourceNames;
    private object m_lock = new object();

    public AssemblyServicingResourceProvider(string servicingFilesPath, bool hostedDeployment)
    {
      this.m_servicingFilesPath = servicingFilesPath;
      this.HostedDeployment = hostedDeployment;
    }

    public bool HostedDeployment { get; private set; }

    public Stream GetServicingResource(string resourceName) => this.GetServiceResourceInternal(resourceName);

    public Stream GetServiceResourceInternal(string resourceRootName)
    {
      string[] strArray;
      if (this.HostedDeployment)
        strArray = new string[2]
        {
          resourceRootName + "(SqlAzure)(gz)",
          resourceRootName + "(gz)"
        };
      else
        strArray = new string[1]
        {
          resourceRootName + "(gz)"
        };
      Stream stream = (Stream) null;
      foreach (Assembly servicingDataAssembly in this.ServicingDataAssemblies)
      {
        foreach (string name in strArray)
        {
          stream = servicingDataAssembly.GetManifestResourceStream(name);
          if (stream != null)
            return (Stream) new GZipStream(stream, CompressionMode.Decompress);
        }
      }
      return stream;
    }

    public string GetSqlScript(string scriptName)
    {
      string sqlScript = (string) null;
      using (Stream servicingResource = this.GetServicingResource(scriptName))
      {
        if (servicingResource != null)
        {
          using (StreamReader streamReader = new StreamReader(servicingResource))
            sqlScript = streamReader.ReadToEnd();
        }
      }
      return sqlScript;
    }

    public string[] ResourceNames
    {
      get
      {
        string[] resourceNames = this.m_resourceNames;
        if (resourceNames == null)
        {
          lock (this.m_lock)
          {
            List<string> stringList = new List<string>();
            foreach (Assembly servicingDataAssembly in this.ServicingDataAssemblies)
            {
              string[] manifestResourceNames = servicingDataAssembly.GetManifestResourceNames();
              stringList.AddRange(((IEnumerable<string>) manifestResourceNames).Where<string>((Func<string, bool>) (resName => resName.IndexOf("(SqlAzure)", StringComparison.Ordinal) < 0)).Where<string>((Func<string, bool>) (resName => resName.IndexOf("(gz)", StringComparison.Ordinal) >= 0)).Select<string, string>((Func<string, string>) (resName => resName.Replace("(gz)", ""))));
            }
            this.m_resourceNames = resourceNames = stringList.ToArray();
          }
        }
        return resourceNames;
      }
    }

    private Assembly[] ServicingDataAssemblies
    {
      get
      {
        Assembly[] servicingDataAssemblies = this.m_servicingDataAssemblies;
        if (servicingDataAssemblies == null)
        {
          lock (this.m_lock)
          {
            if (this.m_servicingDataAssemblies == null)
            {
              string[] files = Directory.GetFiles(this.m_servicingFilesPath, "*.sql.*dll");
              Assembly[] assemblyArray = new Assembly[files.Length];
              for (int index = 0; index < files.Length; ++index)
                assemblyArray[index] = Assembly.ReflectionOnlyLoadFrom(files[index]);
              servicingDataAssemblies = this.m_servicingDataAssemblies = assemblyArray;
            }
          }
        }
        return servicingDataAssemblies;
      }
    }
  }
}
