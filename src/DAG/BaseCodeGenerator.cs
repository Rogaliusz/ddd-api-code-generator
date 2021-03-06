﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using DAG.Helpers;

namespace DAG
{
    public abstract class BaseCodeGenerator
    {
        protected readonly string _assemblyPath;
        protected readonly string _classDirectoryPath;

        protected bool _gExt = true;

        protected BaseCodeGenerator(string assemblyPath, string classDirectoryPath)
        {
            _assemblyPath = assemblyPath;
            _classDirectoryPath = classDirectoryPath;

           if(!Directory.Exists(classDirectoryPath))
                Directory.CreateDirectory(_classDirectoryPath);
        }


        public abstract void Generate();

        public string ReadTemplate(string templatePath)
            => File.ReadAllText(templatePath);

        protected void CreateClass(string path, string body, bool update)
        {
            path = _gExt ?
                $"{path}.g.cs" : 
                $"{path}.cs";

            if (File.Exists(path) && !update)
                return;

            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            File.WriteAllText(path, body);
            Console.WriteLine($"{path} created");
        }

        /// <summary>
        /// This method fill template with values from dictionary
        /// </summary>
        /// <param name="template">string template (not path!)</param>
        /// <param name="values">TemplateKey : value</param>
        protected string CreateBody(string template, IDictionary<string, string> values)
            => values.Aggregate(template, (current, val) => current.Replace(val.Key, val.Value));
        

        protected IEnumerable<Type> GetModelsFromAssembly(string modelsNamespace)
        {
            var assembly = Assembly.LoadFrom(_assemblyPath);
            return assembly.GetClassFromAssemblyNamespace(modelsNamespace);
        }

        protected IEnumerable<Type> GetModelsFromAssembly(string assemblyPath, string modelsNamespace)
        {
            var assembly = Assembly.LoadFrom(assemblyPath);
            return assembly.GetClassFromAssemblyNamespace(modelsNamespace);
        }

    }
}
