using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

static class EmbeddedAssemblyResolver
{
	internal static void HookResolve(params string[] namespaceNames)
	{
#if !DEBUG
		var assembly = Assembly.GetEntryAssembly();
		string namespaceName = namespaceNames.FirstOrDefault() ?? assembly.EntryPoint.DeclaringType.Namespace;
		AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
		{
			string[] parts = args.Name.Split(',');
			string name = parts[0];
			string culture = parts[2];
			if(name.EndsWith(".resources") && !culture.EndsWith("neutral"))
				return null;
			var assemblyName = new AssemblyName(args.Name);
			string resourceName = namespaceName + '.' + assemblyName.Name + ".dll";
			using(var stream = assembly.GetManifestResourceStream(resourceName))
			{
				var assemblyData = new byte[stream.Length];
				stream.Read(assemblyData, 0, assemblyData.Length);
				return Assembly.Load(assemblyData);
			}
		};
#endif
	}
}
