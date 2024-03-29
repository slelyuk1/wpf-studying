﻿using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using Shared.Tool.ViewModel;
using TaskManager.Model.UI;

namespace TaskManager.ViewModel
{
    public class ProcessesInfoViewModel : ObservableItem
    {
        private const int UpdateInterval = 1000;
        private const int RebuildInterval = 10000;

        private readonly ProcessesInfoModel _model;
        private readonly ILogger<ProcessesInfoViewModel> _logger;

        private readonly Timer _updateTimer;
        private readonly Timer _rebuildTimer;

        public ProcessesInfoViewModel(ProcessesInfoModel model, ILogger<ProcessesInfoViewModel> logger)
        {
            _model = model;
            _logger = logger;

            StopProcessCommand = new DelegateBasedCommand(StopProcess, _ => CanProcessBeStopped());
            OpenFolderCommand = new DelegateBasedCommand(OpenFolder, _ => CanProcessBeNavigated());

            BindingOperations.EnableCollectionSynchronization(Processes, _model);

            _rebuildTimer = new Timer(RebuildProcesses, null, 0, RebuildInterval);
            _updateTimer = new Timer(UpdateProcesses, null, UpdateInterval, UpdateInterval);
        }

        public ObservableCollection<ReadableProcess> Processes => _model.ViewedProcesses;

        public ICommand StopProcessCommand { get; }

        public ICommand OpenFolderCommand { get; }

        public ReadableProcess? SelectedProcess
        {
            private get => _model.SelectedProcess;
            set
            {
                _model.SelectedProcess = value;
                OnPropertyChanged(nameof(SelectedProcessThreads));
                OnPropertyChanged(nameof(SelectedProcessModules));
            }
        }

        public ProcessThreadCollection SelectedProcessThreads => _model.SelectedThreads ?? new ProcessThreadCollection(new ProcessThread[] { });

        public ProcessModuleCollection SelectedProcessModules => _model.SelectedModules ?? new ProcessModuleCollection(new ProcessModule[] { });

        private bool CanProcessBeStopped() => SelectedProcess is {IsActive: true};

        private bool CanProcessBeNavigated() => SelectedProcess is {FileLocation: { }};


        private void OpenFolder(object obj)
        {
            if (SelectedProcess == null)
            {
                _logger.LogWarning("Tried to open folder without selected process");
                return;
            }

            if (SelectedProcess.FileLocation == null)
            {
                _logger.LogWarning("Tried to open folder for process without FileLocation");
                return;
            }

            try
            {
                Process.Start(SelectedProcess.FileLocation);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "An error occurred during \"Open Folder\" action");
                MessageBox.Show("Unfortunately, couldn't open the folder!", "Error!");
            }
        }

        private void StopProcess(object obj)
        {
            if (SelectedProcess == null)
            {
                _logger.LogWarning("Tried to stop process without selected process");
                return;
            }

            try
            {
                SelectedProcess.ObservedProcess.Kill();
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "An error occurred during \"Stop Process\" action");
                MessageBox.Show("Unfortunately, couldn't stop the selected process!", "Error!");
            }
        }

        private void UpdateProcesses(object obj)
        {
            _updateTimer.Change(Timeout.Infinite, Timeout.Infinite);
            _model.UpdateProcesses();

            OnPropertyChanged(nameof(SelectedProcessThreads));
            OnPropertyChanged(nameof(SelectedProcessModules));

            _updateTimer.Change(0, UpdateInterval);
        }

        private void RebuildProcesses(object obj)
        {
            _rebuildTimer.Change(Timeout.Infinite, Timeout.Infinite);
            _model.RebuildProcesses(Process.GetProcesses());
            _rebuildTimer.Change(0, RebuildInterval);
        }
    }
}