using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using spnode.communication.bolt;
using spnode.world.procedural.Generators.Noise;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace spnode
{
    public interface ISpNode
    {
        void Start();

        void Stop();
    }

    public class NodeMonitor<TNode> where TNode : ISpNode
    {
        private static readonly Lazy<NodeMonitor<TNode>> LazyInstance =
            new Lazy<NodeMonitor<TNode>>(() => new NodeMonitor<TNode>());

        private long nodeUsageMonitoringRate = TimeSpan.FromMilliseconds(100d).Ticks;
        private NodeInfo cachedNodeInfo;
        private NodeUsageInfo lastNodeUsageInfo;


        protected NodeMonitor()
        {
        }


        public NodeInfo GetNodeInfo()
        {
            // Cache node info (we need to do this only once in process lifetime)
            return this.cachedNodeInfo ?? (this.cachedNodeInfo = new NodeInfo
            {
                ProcessId = Process.GetCurrentProcess().Id.ToString(),
                Version = typeof(TNode).Assembly.GetName().Version.ToString(),
                FrameworkDescription = RuntimeInformation.FrameworkDescription,
                Os = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                    ? "windows"
                    : (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                        ? "linux"
                        : (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
                            ? "mac"
                            : "unknown")),
                OsVersion = Environment.OSVersion.VersionString,
                OsDescription = RuntimeInformation.OSDescription,
                OsArchitecture = RuntimeInformation.OSArchitecture.ToString(),
                ProcessArchitecture = RuntimeInformation.ProcessArchitecture.ToString(),
                ProcessorCount = Environment.ProcessorCount
            });
        }

        public NodeUsageInfo GetNodeUsageInfo()
        {
            // Use cached if not expired
            var nowUtcTicks = DateTime.UtcNow.Ticks;
            if (this.lastNodeUsageInfo != null &&
                this.lastNodeUsageInfo.TimeStamp + this.nodeUsageMonitoringRate > nowUtcTicks)
                return this.lastNodeUsageInfo;

            // Get the process info
            var runningProcess = Process.GetCurrentProcess();

            // Processor percentage
            var totalProcessorTime = runningProcess.TotalProcessorTime.Ticks;
            var processorUsage = 0d;
            if (this.lastNodeUsageInfo != null)
            {
                processorUsage = (double) (totalProcessorTime - this.lastNodeUsageInfo.TotalProcessorTime) /
                                 ((nowUtcTicks - this.lastNodeUsageInfo.TimeStamp) * this.GetNodeInfo().ProcessorCount);
            }

            return this.lastNodeUsageInfo = new NodeUsageInfo
            {
                TimeStamp = nowUtcTicks,
                ProcessorUsage = processorUsage,
                TotalProcessorTime = totalProcessorTime,
                MemoryWorkingSet = runningProcess.WorkingSet64
            };
        }

        public long NodeUsageMonitoringRate
        {
            get => this.nodeUsageMonitoringRate;
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value),
                        "Monitoring rate can't be less or equal to zero.");

                this.nodeUsageMonitoringRate = value;
            }
        }

        public static NodeMonitor<TNode> Instance => LazyInstance.Value;
    }

    public class NodeInfo
    {
        public string ProcessId { get; set; }

        public string Version { get; set; }

        public string FrameworkDescription { get; set; }

        public string Os { get; set; }

        public string OsVersion { get; set; }

        public string OsArchitecture { get; set; }

        public string OsDescription { get; set; }

        public string ProcessArchitecture { get; set; }

        public int ProcessorCount { get; set; }
    }

    public class NodeUsageInfo
    {
        public long TimeStamp { get; set; }

        public double ProcessorUsage { get; set; }

        public long TotalProcessorTime { get; set; }

        public long MemoryWorkingSet { get; set; }
    }


    public class SpNode : ISpNode
    {
        private int isRunning;
        private readonly DependencyInjectionContainer container = new DependencyInjectionContainer();
        protected readonly CancellationTokenSource NodeCancellationTokenSource = new CancellationTokenSource();
        private BoltServer boltServer;


        public void Start()
        {
            if (Interlocked.CompareExchange(ref this.isRunning, 1, 0) == 1)
                return;

            RegisterLogging(this.container);
            RegisterServices(this.container);
            DumpNodeInfo();
            DumpNodeUsagePeriodically(this.NodeCancellationTokenSource.Token);
            StartBoltServer(this.NodeCancellationTokenSource.Token);
        }

        private void StartBoltServer(CancellationToken cancellationToken)
        {
            this.boltServer = new BoltServer();
            this.boltServer.Start(47554, cancellationToken);
        }

        public void Stop()
        {
            if (Interlocked.CompareExchange(ref this.isRunning, 0, 1) == 0)
                return;

            Log.Logger?.Information("Shutting down...");
            this.NodeCancellationTokenSource.Cancel();
        }

        private static void DumpNodeUsagePeriodically(CancellationToken cancellationToken)
        {
            Task.Run(() => DumpNodeUsage(cancellationToken), cancellationToken);
        }

        private static void DumpNodeInfo()
        {
            var nodeInfo = NodeMonitor<SpNode>.Instance.GetNodeInfo();
            Log.Logger?.Information("CPU cores: {ProcessorCount}", nodeInfo.ProcessorCount);
            Log.Logger?.Information("OS: {Os}, {OsVersion}, {OsArchitecture}, {OsDescription}", 
                nodeInfo.Os, nodeInfo.OsVersion, nodeInfo.OsArchitecture, nodeInfo.OsDescription);
            Log.Logger?.Information("Process ID: {ProcessId}", nodeInfo.ProcessId);
            Log.Logger?.Information("Framework: {FrameworkDescription}", nodeInfo.FrameworkDescription);
            Log.Logger?.Information("Node Version: {NodeVersion}", nodeInfo.Version);
        }

        private static void RegisterLogging(DependencyInjectionContainer container)
        {
            var logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Lelve:u3}] <{SourceContext:l}> {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(
                    new CompactJsonFormatter(),
                    "Logs/spnode.log",
                    LogEventLevel.Information,
                    rollingInterval: RollingInterval.Hour,
                    rollOnFileSizeLimit: true,
                    shared: true,
                    retainedFileCountLimit: null)
                .CreateLogger();
            Log.Logger = logger;

            container.Add(c => c
                .ExportInstance(logger)
                .As<ILogger>());
        }

        private static void RegisterServices(DependencyInjectionContainer container)
        {

        }

        private static async Task DumpNodeUsage(CancellationToken cancellationToken = default)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var usage = NodeMonitor<SpNode>.Instance.GetNodeUsageInfo();
                Log.Logger.Debug("Usage CPU: {NodeCpuUsage:0.00}% MEM: {NodeMemoryWorkingSet:0.00}MB",
                    usage.ProcessorUsage * 100d, usage.MemoryWorkingSet / (1000 * 1024d));
                await Task.Delay(1000, cancellationToken);
            }
        }

        public bool IsRunning => this.isRunning != 0;
    }
}