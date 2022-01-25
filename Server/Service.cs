using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    public class Service
    {
        private volatile bool _working;
        public bool Working
        {
            get => _working;
            private set => _working = value;
        }
        private Thread? _runningService;
        private readonly Network _network = new(1337);
        public void Start()
        {
            Debug.WriteLine("Starting Server");
            _runningService = new Thread(new ThreadStart(Run));
            _runningService.Start();
        }
        public void Stop()
        {
            Working = false;
        }
        private void Run()
        {
            _network.Open();
        }
    }
}
