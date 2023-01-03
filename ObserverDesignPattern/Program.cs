// See https://aka.ms/new-console-template for more information

namespace MyApp // Note: actual namespace depends on the project name.
{
    internal class Status
    {
        private bool Loading { get; set; }
        public bool GetStatus => Loading;

        public Status()
        {
            Loading = false;
        }

        public void ChangeStatus()
        {
            Loading = !Loading;
        }
    }
    
    internal class StatusProvider: IObservable<Status>
    {
        private List<IObserver<Status>> _observers = new List<IObserver<Status>>();
        private Status Status { get; }

        public StatusProvider()
        {
            Status = new Status();
        }

        private class Unsubscribe : IDisposable
        {
            private List<IObserver<Status>> _observers;
            private IObserver<Status> _observer;

            public Unsubscribe(List<IObserver<Status>> observers, IObserver<Status> observer)
            {
                _observers = observers;
                _observer = observer;
            }
            
            public void Dispose()
            {
                if (_observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }
        
        public IDisposable Subscribe(IObserver<Status> observer)
        {
            _observers.Add(observer);
            return new Unsubscribe(_observers, observer);
        }

        public void UpdateStatus()
        {
            Status.ChangeStatus();
            foreach (var obs in _observers)
            {
                obs.OnNext(Status);
            }
        }
    }

    internal class StatusObserver : IObserver<Status>
    {
        private string Name { get; init; }

        public StatusObserver(string name)
        {
            Name = name;
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(Status value)
        {
            Console.WriteLine($"Provider - {Name} Current Status: {value.GetStatus}");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            var provider = new StatusProvider();
            
            var observer1 = provider.Subscribe(new StatusObserver("1"));
            var observer2 = provider.Subscribe(new StatusObserver("2"));

            provider.UpdateStatus();
            provider.UpdateStatus();
            provider.UpdateStatus();
            observer2.Dispose();
            provider.UpdateStatus();
            observer1.Dispose();
        }
    }
}