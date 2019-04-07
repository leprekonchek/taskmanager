using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Management;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic.Devices;

namespace Lab_05_Lopukhina.ViewModels
{
    class ProcessModel : INotifyPropertyChanged
    {
        #region  Fields
        private readonly Process _process;
        private readonly string _path;
        private readonly DateTime _startTime;
        private float _cpu;
        private float _ram;
        PerformanceCounter _memoryCounter;
        PerformanceCounter _cpuCounter;
        private readonly double _total;
        #endregion

        #region Constructor

        // add reference to your project Microsoft.VisualBasic.Devices
        public ProcessModel(Process process)
        {
            _process = process;
            _total = (new ComputerInfo()).TotalPhysicalMemory;
            _memoryCounter = new PerformanceCounter("Process", "Working Set", Name);
            _cpuCounter = new PerformanceCounter("Process", "% Processor Time", Name);
            try
            {
                _startTime = _process.StartTime;
                _path = _process.MainModule.FileName;
            }
            catch (Exception) { }
        }
        #endregion

        #region Properties
        //- Ім'я процесу
        public string Name => _process.ProcessName;

        //- ID процесу
        public int ID => _process.Id;

        //- Ідентифікатор того чи процес активний і відповідає на запити
        public bool IsActive => _process.Responding;

        //- % CPU, який використовує даний процес
        public string CPU => ((float)Math.Round(_cpu * 100f) / 100f).ToString("0.00") + "%";

        //- % і об'єм оперативної пам'яті, який використовує даний процес
        public string RAM => ((_ram / _total) * 100).ToString("0.00") + "% , " + (_ram / (1024 * 1024)).ToString("0.00") + "MB";

        //- Кількість потоків запущених даним процесом
        public int ThreadsQuantity => _process.Threads.Count;

        //- Ім'я користувача, під яким запущений даний процес
        public string UserName => GetProcessOwner(ID);

        //- Ім'я і шлях до файлу, звідки процес запущено
        public string FileInfo => _path;

        //- Дата та час запуску процесу
        public DateTime StartTime => _startTime;

        public string FilePath => _path;

        // threads
        public ProcessThreadCollection Threads
        {
            get
            {
                try
                {
                    ProcessThreadCollection ptc = _process.Threads;
                    return _process.Threads;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        // modules
        public ProcessModuleCollection Modules
        {
            get
            {
                try
                {
                    ProcessModuleCollection pmc = _process.Modules;
                    return _process.Modules;
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }

        #endregion

        #region Methods
        public void StopProcess()
        {
            _process.Kill();
        }

        public void UpdateProcess()
        {
            try
            {
                _cpu = _cpuCounter.NextValue() / Environment.ProcessorCount;
                _ram = _memoryCounter.NextValue();
            }
            catch (Exception) { }
            OnPropertyChanged("RAM");
            OnPropertyChanged("CPU");
            OnPropertyChanged("ThreadsQuantity");
        }

        // add reference to your project System.Management.dll
        public string GetProcessOwner(int processId)
        {
            string query = "Select * From Win32_Process Where ProcessID = " + processId;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection processList = searcher.Get();

            foreach (ManagementObject obj in processList)
            {
                string[] argList = new string[] { string.Empty, string.Empty };
                int returnVal = Convert.ToInt32(obj.InvokeMethod("GetOwner", argList));
                if (returnVal == 0)
                {
                    // return DOMAIN\user
                    return argList[1] + "\\" + argList[0];
                }
            }

            return "NO OWNER";
        }
        #endregion

        #region OnPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
