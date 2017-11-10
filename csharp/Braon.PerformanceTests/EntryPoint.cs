using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using Microsoft.Extensions.CommandLineUtils;

namespace Braon.PerformanceTests
{
    public static class EntryPoint
    {
        public class LambdaConfiguration : ManualConfig
        {
            public LambdaConfiguration(Action<ManualConfig> configAction)
            {
               this.Add(DefaultConfig.Instance);
               configAction.Invoke(this);
            }
        }
        
        private static readonly Dictionary<string,Action<ManualConfig>> configurations = new Dictionary<string, Action<ManualConfig>>()
        {
            {"default",c => {}},
            {"allocations",c => c.Add(new MemoryDiagnoser())}
        };
        
        public static int Main(string[] arguments)
        {
            var application = new CommandLineApplication()
            {
                Name = "Braon.PerformanceTests"
            };

            application.HelpOption("-?|-h|--help");

            application.Command("benchmark", command =>
            {
                command.HelpOption("-?|-h|--help");
                command.Description = "Execute selected benchmarks in benchmarking mode";

                var selectorOption = command.Option("-s|--select <selections>", "Benchmarks to run",CommandOptionType.SingleValue);
                var configurationOption = command.Option("-c|--configuration <configuration>",
                    "Benchmark configuration to use", CommandOptionType.SingleValue);
                              
                command.OnExecute(() =>
                {
                    var configurationName = configurationOption.HasValue() ? configurationOption.Value() : "default";
                    var selector = selectorOption.HasValue() ? selectorOption.Value() : null;

                    if (configurations.TryGetValue(configurationName, out var configBuilder))
                    {
                        var configuration = new LambdaConfiguration(configBuilder);

                        var suites = EnumerateBenchmarkSuites(configuration, GetSuiteSelector(selector));

                        foreach (var suite in suites)
                        {
                            suite.Run(GetMethodSelector(selector));
                        }

                        return 0;
                    }
                    
                    command.Error.WriteLine("Invalid configuration selected");
                    return 1;
                });
            });

            application.Command("profile", command =>
            {
                command.HelpOption("-?|-h|--help");
                command.Description = "Execute a single benchmark in an endless loop, for profiler analysis";
                
                var selectorOption = command.Option("-s|--select <selections>", "Benchmarks to run",CommandOptionType.SingleValue);
                
                command.OnExecute(() =>
                { 
                    var selector = selectorOption.HasValue() ? selectorOption.Value() : null;

                    var benchmarkAction =
                        EnumerateBenchmarkSuites(new LambdaConfiguration(c => { }), GetSuiteSelector(selector))
                            .SelectMany(suite =>
                            {
                                return suite.GetBenchmarksAsActions(GetMethodSelector(selector))
                                    .Select(action => (suite, action.Item2, action.Item1));
                            })
                            .FirstOrDefault();

                    if (benchmarkAction.Item1 != null)
                    {
                        command.Out.WriteLine("Running suite {0}, benchmark {1} in profile mode",benchmarkAction.Item1.Name,benchmarkAction.Item3);

                        using (var cancellationTokenSource = new CancellationTokenSource())
                        {
                            var cancellationToken = cancellationTokenSource.Token;
                            
                            Console.CancelKeyPress += (sender, args) =>
                            {
                                cancellationTokenSource.Cancel();
                            };

                            while (!cancellationToken.IsCancellationRequested)
                            {
                                benchmarkAction.Item2.Invoke();
                            }
                        }

                        return 0;
                    }
                    
                    command.Error.WriteLine("No benchmark selected");
                    return 1;
                });
            });

            return application.Execute(arguments);
        }

        private static IEnumerable<BenchmarkSuiteDescription> EnumerateBenchmarkSuites(IConfig config,Func<BenchmarkSuiteDescription,bool> predicate)
        {
            return typeof(EntryPoint).Assembly.GetTypes()
                .Where(t => t.GetCustomAttribute<BenchmarkSuiteAttribute>(false) != null).Select(t =>
                    new BenchmarkSuiteDescription(t, t.GetCustomAttribute<BenchmarkSuiteAttribute>(),config));
        }

        private static Func<(string, MethodInfo), bool> GetMethodSelector(string selector)
        {
            if (string.IsNullOrWhiteSpace(selector))
            {
                return tuple => true;
            }

            if (selector.Contains("."))
            {
                var methodPart = selector.Split('.')[1];

                return tuple => tuple.Item1.Equals(methodPart, StringComparison.InvariantCultureIgnoreCase);
            }

            return tuple => true;
        }
        
        private static Func<BenchmarkSuiteDescription,bool> GetSuiteSelector(string selector)
        {
            if (string.IsNullOrWhiteSpace(selector))
            {
                return suite => true;
            }

            if (selector.Contains("."))
            {
                var suitePart = selector.Split(".")[0];
                return suite => suite.Name.Equals(suitePart, StringComparison.InvariantCultureIgnoreCase);
            }
            else
            {
                return suite => suite.Name.Equals(selector, StringComparison.InvariantCultureIgnoreCase);
            }
        }
    }
}