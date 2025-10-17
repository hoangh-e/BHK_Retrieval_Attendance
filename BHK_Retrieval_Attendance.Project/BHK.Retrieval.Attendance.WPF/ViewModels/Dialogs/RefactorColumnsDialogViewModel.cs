using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using BHK.Retrieval.Attendance.WPF.Services.Interfaces;

namespace BHK.Retrieval.Attendance.WPF.ViewModels.Dialogs
{
    public partial class RefactorColumnsDialogViewModel : ObservableObject
    {
        #region Private Fields

        private readonly IExcelTableService _excelService;
        private readonly ILogger<RefactorColumnsDialogViewModel> _logger;
        private Window? _dialog;

        #endregion

        #region Properties

        [ObservableProperty]
        private string _tableName = string.Empty;

        [ObservableProperty]
        private string _tableType = string.Empty;

        [ObservableProperty]
        private string _filePath = string.Empty;

        [ObservableProperty]
        private List<string> _currentColumns = new();

        [ObservableProperty]
        private List<string> _expectedColumns = new();

        [ObservableProperty]
        private bool _isLoading;

        public bool? DialogResult { get; private set; }

        #endregion

        #region Constructor

        public RefactorColumnsDialogViewModel(
            IExcelTableService excelService,
            ILogger<RefactorColumnsDialogViewModel> logger)
        {
            _excelService = excelService ?? throw new ArgumentNullException(nameof(excelService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion

        #region Public Methods

        public void SetDialog(Window dialog)
        {
            _dialog = dialog;
        }

        public void SetData(string filePath, string tableName, string tableType, 
                           List<string> currentColumns, List<string> expectedColumns)
        {
            FilePath = filePath;
            TableName = tableName;
            TableType = tableType;
            CurrentColumns = currentColumns ?? new List<string>();
            ExpectedColumns = expectedColumns ?? new List<string>();
        }

        #endregion

        #region Commands

        [RelayCommand]
        private void Close()
        {
            DialogResult = false;
            _dialog?.Close();
        }

        [RelayCommand]
        private async Task Refactor()
        {
            try
            {
                IsLoading = true;
                
                await _excelService.RefactorTableColumnsAsync(FilePath, TableName, TableType);
                
                DialogResult = true;
                _logger.LogInformation($"Successfully refactored columns for table '{TableName}'");
                
                MessageBox.Show(
                    "Đã refactor cột thành công!\nTable đã được cập nhật theo đúng định dạng.", 
                    "Thành công", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Information);
                
                _dialog?.Close();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to refactor table columns");
                MessageBox.Show(
                    $"Lỗi refactor cột: {ex.Message}", 
                    "Lỗi", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        #endregion
    }
}