using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMPie.Assembly.Utils
{
   public static class Factory
    {
       
       private static Dictionary<string, System.Reflection.Assembly> Modules = new Dictionary<string, System.Reflection.Assembly>();

        public static List<T> GetInstances<T>(string FilePath)
        {
             System.Reflection.Assembly ProcessorAssembly;
        var res = new List<T>();
            if (!Modules.ContainsKey(FilePath))
            {
                Modules[FilePath] = System.Reflection.Assembly.LoadFrom(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FilePath));
            }
            ProcessorAssembly = Modules[FilePath];
            var shouldLoadAssembly =
                ProcessorAssembly.GetTypes()
                    .Any(
                        type =>
                            type.GetInterfaces()
                                .Any(t => t.FullName == typeof(T).FullName));

            if (shouldLoadAssembly)
            {
                foreach (var newInstance in (from type in ProcessorAssembly.GetTypes()
                                             where type.GetInterfaces().Any(t => t.FullName == typeof(T).FullName)
                                             select ProcessorAssembly.CreateInstance(type.FullName)).OfType<T>())
                {
                    res.Add(newInstance);
                }
            }
            return res;
        }
    }
}
