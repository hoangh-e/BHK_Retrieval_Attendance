# Settings & Export - Views Implementation Checklist

## 📋 OVERVIEW
Backend implementation đã hoàn thành (Services, ViewModels, DI). 
Còn lại: Tạo XAML Views và wiring navigation.

---

## ✅ COMPLETED (Backend)

- [x] OneDriveOptions.cs
- [x] SharePointOptions.cs (updated)
- [x] IPathSettingsService + Implementation
- [x] IExcelService + Implementation
- [x] SettingsViewModel
- [x] ExportAttendanceViewModel
- [x] ExportEmployeeViewModel
- [x] Properties/Settings.settings
- [x] appsettings.json (updated)
- [x] DI Registration
- [x] ClosedXML package
- [x] Build successful

---

## 🎨 TODO: VIEWS (XAML)

### 1. SettingsView.xaml ⏳
**File**: `BHK.Retrieval.Attendance.WPF/Views/Pages/SettingsView.xaml`

**DataContext**: SettingsViewModel (inject via DI)

**Layout Structure**:
```xml
<UserControl>
    <ScrollViewer>
        <StackPanel Margin="20">
            <!-- Header -->
            <TextBlock Text="CÀI ĐẶT HỆ THỐNG" Style="MaterialDesignHeadline4"/>
            
            <!-- Card 1: Attendance Export Folder -->
            <materialDesign:Card>
                <Grid>
                    <TextBox Text="{Binding AttendanceExportFolder}"/>
                    <Button Command="{Binding BrowseAttendanceFolderCommand}"/>
                </Grid>
            </materialDesign:Card>
            
            <!-- Card 2: Employee Data File -->
            <materialDesign:Card>
                <Grid>
                    <TextBox Text="{Binding EmployeeDataFilePath}"/>
                    <Button Command="{Binding BrowseEmployeeFileCommand}"/>
                </Grid>
            </materialDesign:Card>
            
            <!-- Card 3: Table Names -->
            <materialDesign:Card>
                <StackPanel>
                    <TextBox Text="{Binding AttendanceTableName}"/>
                    <TextBox Text="{Binding EmployeeTableName}"/>
                </StackPanel>
            </materialDesign:Card>
            
            <!-- Card 4: Test Functions -->
            <materialDesign:Card>
                <StackPanel>
                    <Button Command="{Binding TestExportAttendanceCommand}"/>
                    <Button Command="{Binding TestExportEmployeeCommand}"/>
                </StackPanel>
            </materialDesign:Card>
            
            <!-- Action Buttons -->
            <StackPanel Orientation="Horizontal">
                <Button Command="{Binding ResetSettingsCommand}"/>
                <Button Command="{Binding SaveSettingsCommand}"/>
            </StackPanel>
            
            <!-- Loading Indicator -->
            <ProgressBar IsIndeterminate="True" 
                         Visibility="{Binding IsLoading, Converter=BoolToVis}"/>
        </StackPanel>
    </ScrollViewer>
</UserControl>
```

**Code-behind** (SettingsView.xaml.cs):
```csharp
public partial class SettingsView : UserControl
{
    public SettingsView()
    {
        InitializeComponent();
        // DataContext set via DI hoặc Navigation
    }
}
```

**Checklist**:
- [ ] Tạo file SettingsView.xaml
- [ ] Tạo file SettingsView.xaml.cs
- [ ] Design layout với Material Design cards
- [ ] Bind tất cả properties
- [ ] Bind tất cả commands
- [ ] Add loading indicator
- [ ] Test UI preview

---

### 2. ExportEmployeeDialog.xaml ⏳
**File**: `BHK.Retrieval.Attendance.WPF/Views/Dialogs/ExportEmployeeDialog.xaml`

**DataContext**: ExportEmployeeViewModel

**Layout Structure**:
```xml
<Window Title="Xuất Danh Sách Nhân Viên"
        Height="500" Width="700"
        WindowStartupLocation="CenterOwner">
    <Grid Margin="20">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/> <!-- Header -->
            <RowDefinition Height="*"/>    <!-- Content -->
            <RowDefinition Height="Auto"/> <!-- Footer -->
        </Grid.RowDefinitions>
        
        <!-- Header -->
        <TextBlock Grid.Row="0" Text="XUẤT DANH SÁCH NHÂN VIÊN"/>
        
        <!-- Content -->
        <ScrollViewer Grid.Row="1">
            <StackPanel>
                <!-- File Path Browser -->
                <Grid>
                    <TextBox Text="{Binding FilePath}"/>
                    <Button Command="{Binding BrowseFileCommand}"/>
                </Grid>
                
                <!-- File Name (ReadOnly) -->
                <TextBox Text="{Binding FileName}" IsReadOnly="True"/>
                
                <!-- Table Selector -->
                <ComboBox ItemsSource="{Binding AvailableTables}"
                          SelectedItem="{Binding SelectedTable}"/>
                
                <!-- Record Count -->
                <TextBlock>
                    <Run Text="Số lượng record: "/>
                    <Run Text="{Binding RecordCount}"/>
                </TextBlock>
                
                <!-- Create Table Button (Conditional) -->
                <Button Content="TẠO TABLE MỚI"
                        Command="{Binding CreateTableCommand}"
                        Visibility="{Binding ShowCreateTableButton, Converter=BoolToVis}"/>
                
                <!-- Loading -->
                <ProgressBar IsIndeterminate="True"
                             Visibility="{Binding IsLoading, Converter=BoolToVis}"/>
            </StackPanel>
        </ScrollViewer>
        
        <!-- Footer -->
        <StackPanel Grid.Row="2" Orientation="Horizontal">
            <Button Content="HỦY" Command="{Binding CancelCommand}"/>
            <Button Content="XUẤT" Command="{Binding ExportCommand}" 
                    IsEnabled="{Binding CanExport}"/>
        </StackPanel>
    </Grid>
</Window>
```

**Code-behind** (ExportEmployeeDialog.xaml.cs):
```csharp
public partial class ExportEmployeeDialog : Window
{
    public ExportEmployeeDialog()
    {
        InitializeComponent();
    }
    
    // Hoặc inject ViewModel qua constructor
    public ExportEmployeeDialog(ExportEmployeeViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
```

**Checklist**:
- [ ] Tạo file ExportEmployeeDialog.xaml
- [ ] Tạo file ExportEmployeeDialog.xaml.cs
- [ ] Design layout
- [ ] Bind file path và browser
- [ ] Bind table selector (ComboBox)
- [ ] Bind record count display
- [ ] Conditional create table button
- [ ] Export và Cancel buttons
- [ ] Loading indicator
- [ ] Test dialog

---

### 3. ExportAttendanceDialog.xaml ⏳
**File**: `BHK.Retrieval.Attendance.WPF/Views/Dialogs/ExportAttendanceDialog.xaml`

**Options**:
1. ✅ **Tái sử dụng ExportConfigurationDialog** nếu UI tương thích
2. 🆕 **Tạo mới** tương tự ExportEmployeeDialog

**Layout Structure** (Nếu tạo mới):
```xml
<Window Title="Xuất Điểm Danh"
        Height="400" Width="600"
        WindowStartupLocation="CenterOwner">
    <Grid Margin="20">
        <!-- Folder Browser -->
        <Grid>
            <TextBox Text="{Binding ExportFolder}"/>
            <Button Command="{Binding BrowseFolderCommand}"/>
        </Grid>
        
        <!-- File Name -->
        <TextBox Text="{Binding FileName}"/>
        
        <!-- Table Name -->
        <TextBox Text="{Binding TableName}"/>
        
        <!-- Export Button -->
        <Button Content="XUẤT DỮ LIỆU" Command="{Binding ExportCommand}"/>
        
        <!-- Loading -->
        <ProgressBar IsIndeterminate="True"
                     Visibility="{Binding IsLoading, Converter=BoolToVis}"/>
    </Grid>
</Window>
```

**Checklist**:
- [ ] Decide: Tái sử dụng hoặc tạo mới?
- [ ] Nếu tạo mới: Tạo XAML files
- [ ] Design layout
- [ ] Bind folder, filename, tablename
- [ ] Export button
- [ ] Test dialog

---

## 🔗 TODO: NAVIGATION INTEGRATION

### 4. Add Settings to Navigation ⏳

**File**: `MainWindowViewModel.cs`

**Changes**:
```csharp
public class MainWindowViewModel
{
    // Add command
    public ICommand NavigateToSettingsCommand { get; }
    
    public MainWindowViewModel(INavigationService navigationService)
    {
        // Register command
        NavigateToSettingsCommand = new RelayCommand(() => 
            navigationService.NavigateTo<SettingsViewModel>());
    }
}
```

**Checklist**:
- [ ] Add NavigateToSettingsCommand to MainWindowViewModel
- [ ] Wire command in MainWindow menu/sidebar
- [ ] Test navigation

---

### 5. DataTemplate Registration ⏳

**File**: `App.xaml` hoặc `MainWindow.xaml`

**Add**:
```xml
<Application.Resources>
    <ResourceDictionary>
        <DataTemplate DataType="{x:Type vm:SettingsViewModel}">
            <views:SettingsView />
        </DataTemplate>
    </ResourceDictionary>
</Application.Resources>
```

**Checklist**:
- [ ] Add DataTemplate for SettingsViewModel
- [ ] Add namespace imports
- [ ] Test view rendering

---

## 🪟 TODO: DIALOG SERVICE ENHANCEMENT

### 6. Show Dialog Methods ⏳

**File**: `DialogService.cs` hoặc create helper

**Option 1: Generic Method**
```csharp
public async Task<bool> ShowDialogAsync<TViewModel>(TViewModel viewModel) 
    where TViewModel : class
{
    Window dialog = viewModel switch
    {
        ExportEmployeeViewModel => new ExportEmployeeDialog { DataContext = viewModel },
        ExportAttendanceViewModel => new ExportAttendanceDialog { DataContext = viewModel },
        _ => throw new NotSupportedException()
    };
    
    return dialog.ShowDialog() ?? false;
}
```

**Option 2: Specific Methods**
```csharp
public async Task ShowExportEmployeeDialogAsync(List<EmployeeDto> data)
{
    var vm = _serviceProvider.GetRequiredService<ExportEmployeeViewModel>();
    vm.Initialize(data);
    
    var dialog = new ExportEmployeeDialog { DataContext = vm };
    dialog.ShowDialog();
}
```

**Checklist**:
- [ ] Decide approach: Generic vs Specific
- [ ] Implement dialog showing logic
- [ ] Update SettingsViewModel to use dialog service
- [ ] Test dialog display

---

## 🧪 TODO: TESTING

### 7. Manual Testing ⏳

**Test Scenarios**:
- [ ] Navigate to Settings page
- [ ] Browse and select attendance export folder
- [ ] Browse and select employee data file
- [ ] Change table names
- [ ] Click Save -> Verify settings saved
- [ ] Restart app -> Verify settings persisted
- [ ] Click Reset -> Verify defaults restored
- [ ] Click "Test Export Attendance" -> Dialog appears
- [ ] Click "Test Export Employee" -> Dialog appears
- [ ] In Export Employee Dialog:
  - [ ] Browse file (existing)
  - [ ] See available tables
  - [ ] Select table -> See record count
  - [ ] Click Export -> File updated
- [ ] In Export Employee Dialog (new file):
  - [ ] Browse file (non-existing)
  - [ ] Click Create Table -> Table created
  - [ ] Click Export -> Data exported

---

## 📊 PROGRESS TRACKER

### Backend: 100% ✅
- Services: ✅
- ViewModels: ✅
- DI: ✅
- Configuration: ✅

### Frontend: 0% ⏳
- [ ] SettingsView (0%)
- [ ] ExportEmployeeDialog (0%)
- [ ] ExportAttendanceDialog (0%)
- [ ] Navigation (0%)
- [ ] DataTemplate (0%)
- [ ] DialogService enhancement (0%)
- [ ] Testing (0%)

---

## 🎯 PRIORITY ORDER

1. **HIGH**: SettingsView.xaml + Navigation
2. **HIGH**: DataTemplate registration
3. **MEDIUM**: ExportEmployeeDialog.xaml
4. **MEDIUM**: ExportAttendanceDialog.xaml (or reuse)
5. **MEDIUM**: DialogService enhancement
6. **LOW**: Manual testing
7. **LOW**: Unit tests

---

## 💡 TIPS

### Material Design Components
Sử dụng:
- `materialDesign:Card` cho sections
- `materialDesign:HintAssist.Hint` cho placeholders
- `Style="{StaticResource MaterialDesignRaisedButton}"` cho buttons
- `Style="{StaticResource MaterialDesignOutlinedTextBox}"` cho inputs

### Converters
Cần có converter cho `Visibility`:
```xml
<BooleanToVisibilityConverter x:Key="BoolToVisibilityConverter"/>
```

### Ookii Dialogs
Đã có package, sử dụng trong ViewModels:
- `VistaFolderBrowserDialog` - Folder picker
- `VistaOpenFileDialog` - File picker

---

## 📝 NOTES

- Backend đã sẵn sàng, ViewModels đã có test data
- Dialog service hiện tại chỉ có basic methods
- Navigation service đã hoạt động (dùng cho các pages khác)
- Material Design theme đã cài đặt

---

## ✅ FINAL CHECKLIST

Khi hoàn thành tất cả:
- [ ] All views created
- [ ] Navigation working
- [ ] Dialogs showing
- [ ] Settings persisting
- [ ] Export functions working
- [ ] No errors in console
- [ ] UI responsive
- [ ] Material Design consistent

---

Tạo ngày: 2025-10-15
Backend Status: ✅ COMPLETE
Frontend Status: ⏳ PENDING
