using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace PathTracerSharp.Rendering
{
    public class ThreadManager : IDisposable
    {
        private readonly ConcurrentQueue<Action> _queue = new ConcurrentQueue<Action>();
        private Thread[] _pool;
        private Action _onDone;

        public void Start(int count, Action onDone) 
        {
            _onDone = onDone;
            _pool = Enumerable.Range(0, count)
                .Select(x => new Thread(ThreadProcess) { IsBackground = true })
                .ToArray();

            for (var i = 0; i < _pool.Length; i++)
            {
                if (_pool[i] != null) 
                {
                    _pool[i].Start();
                }
            }
        }

        public void Push(Action action)
        {
            _queue.Enqueue(action);
        }

        private void Kill() 
        {
            if (_pool is null) return;

            for (var i = 0; i < _pool.Length; i++) 
            {
                if (_pool[i] != null) 
                {
                    _pool[i].Interrupt();
                    _pool[i] = null;
                }
            }

            _queue.Clear();
            _onDone = null;
        }

        private void ThreadProcess() 
        {
            try
            {
                while (true)
                {
                    if (_queue.Count > 0)
                    {
                        if (_queue.TryDequeue(out var action)) 
                        {
                            action?.Invoke();

                            if (_queue.Count == 0) 
                            {
                                _onDone?.Invoke();
                            }
                        }
                    }
                    else
                    {
                        Thread.Sleep(1);
                    }
                }
            }
            catch (ThreadInterruptedException)
            {
            }
        }

        public void Dispose()
        {
            Kill();
            GC.SuppressFinalize(this);
            GC.ReRegisterForFinalize(this);
        }
    }
}
