using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

namespace Braon.PerformanceTests
{
    public sealed class BenchmarkSuiteDescription
    {
        private readonly Type classType;
        private readonly BenchmarkSuiteAttribute attribute;
        private readonly IConfig config;
        private readonly object instance;

        public BenchmarkSuiteDescription(Type classType, BenchmarkSuiteAttribute attribute,IConfig config)
        {
            this.classType = classType;
            this.attribute = attribute;
            this.config = config;
            this.instance = Activator.CreateInstance(classType);
        }

        public string Name => string.IsNullOrWhiteSpace(attribute.Name)
            ? classType.Name.Replace("Benchmarks", string.Empty)
            : attribute.Name;

        public IEnumerable<(string, Action)> GetBenchmarksAsActions(Func<(string,MethodInfo),bool> predicate)
        {
            return GetBenchmarksAsMethods(predicate).Select(m =>
            {
                var action = (Action) Delegate.CreateDelegate(typeof(Action), instance, m.Item2);

                return (m.Item1, action);
            });
        }

        public IEnumerable<(string,MethodInfo)> GetBenchmarksAsMethods(Func<(string,MethodInfo),bool> predicate)
        {
            return classType.GetMethods().Where(m => m.GetCustomAttribute<BenchmarkAttribute>() != null)
                .Select(m =>
                {
                    var benchmarkAttribute = m.GetCustomAttribute<BenchmarkAttribute>();

                    var name = string.IsNullOrWhiteSpace(benchmarkAttribute.Description)
                        ? m.Name
                        : benchmarkAttribute.Description;

                    return (name, m);
                }).Where(predicate);
        }

        public void Run(Func<(string,MethodInfo),bool> predicate)
        {
            var methods = GetBenchmarksAsMethods(predicate);

            BenchmarkRunner.Run(classType, methods.Select(m => m.Item2).ToArray(), config);
        }
    }
}