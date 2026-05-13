; GOST Automation Setup Script for Inno Setup

#define MyAppName "GOST 21.208 nanoCAD Automation"
#define MyAppVersion "0.0.5-alpha"
#define MyAppPublisher "Кукунов Константин"
#define MyAppURL "https://github.com/Kukunov/nanoCAD_Automation"

[Setup]
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppSupportURL={#MyAppURL}
DefaultDirName={code:GetNanoCADPath}\GOST_Automation
DisableDirPage=no
DisableWelcomePage=True
DefaultGroupName={#MyAppName}
OutputDir=.\Output
OutputBaseFilename=GOST_Automation_Setup
Compression=lzma
SolidCompression=yes
PrivilegesRequired=admin
UsedUserAreasWarning=no
Uninstallable=yes
CreateUninstallRegKey=yes

SetupIconFile=icon.ico
UninstallDisplayIcon={app}\icon.ico

ShowLanguageDialog=no 
[Languages]
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "NanoCAD.API.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "blocks.dwg"; DestDir: "{app}"; Flags: ignoreversion
Source: "GOST_Automation.cfg"; DestDir: "{userappdata}\Nanosoft\nanoCAD x64 25.0\Config"; Flags: ignoreversion

; Превью блоков
Source: "Resources\Previews\*.png"; DestDir: "{app}\Resources\Previews"; Flags: ignoreversion
; Иконки меню
Source: "Resources\Icons\*.ico"; DestDir: "{app}\Resources\Icons"; Flags: ignoreversion

[Code]

var
  DescriptionLabel: TLabel;
  LanguageCombo: TComboBox;
  LanguageLabel: TLabel;
  LangArray: Array of String;
  IsConfirm: Boolean;
  
const
  DescRussian = 'Надстройка GOST Automation добавляет в nanoCAD 25.0 инструменты ' +
    'для проектирования схем автоматизации по ГОСТ 21.208-2013: вставку приборов, ' +
    'управление контурами и цветовое кодирование';
  DescEnglish = 'GOST Automation add-on extends nanoCAD 25.0 with tools ' +
    'for designing automation diagrams according to GOST 21.208-2013: ' +
    'device insertion, contour management and color coding';

// Вызов ShellExecute из Windows API
function ShellExecute(hwnd: HWND; lpOperation, lpFile, lpParameters, lpDirectory: String; nShowCmd: Integer): THandle;
external 'ShellExecuteW@shell32.dll stdcall';

// Получить индекс текущего активного языка в массиве
function ActiveLangIndex: Integer;
var
  I: Integer;
begin
  for I := 0 to GetArrayLength(LangArray) - 1 do
  begin
    if LangArray[I] = ActiveLanguage then
    begin
      Result := I;
      Exit;
    end;
  end;
  Result := 0;
end;

// Мгновенная смена языка при выборе в выпадающем списке
procedure LanguageComboChange(Sender: TObject);
var
  NewLang: String;
  ResultCode: THandle;
begin
  NewLang := LangArray[LanguageCombo.ItemIndex];

  if NewLang <> ActiveLanguage then
  begin
    IsConfirm := False;
    ResultCode := ShellExecute(0, '', ExpandConstant('{srcexe}'),
      '/LANG=' + NewLang, '', SW_SHOW);
    WizardForm.Close;
  end;
end;

// Проверка, запущен ли процесс nanoCAD (nCAD.exe)
function IsNanoCADRunning: Boolean;
var
  ResultCode: Integer;
  TempFile: String;
  OutputList: TStringList;
begin
  Result := False;
  TempFile := ExpandConstant('{tmp}') + '\nanoCAD_check.txt';
  // Ищем процесс с именем nCad.exe
  Exec('cmd.exe', '/C tasklist /FI "IMAGENAME eq nCad.exe" /NH > "' + TempFile + '"', 
       '', SW_HIDE, true, ResultCode);
  if FileExists(TempFile) then
  begin
    OutputList := TStringList.Create;
    try
      OutputList.LoadFromFile(TempFile);
      if OutputList.Count > 0 then
        // Проверяем наличие nCad.exe в выводе
        if Pos('nCad.exe', OutputList.Text) > 0 then
          Result := True;
    finally
      OutputList.Free;
    end;
  end;
  DeleteFile(TempFile);
end;

// Запуск установщика: проверяем, что nanoCAD закрыт
function InitializeSetup: Boolean;
begin
  Result := True;
  if IsNanoCADRunning then
  begin
    MsgBox('Обнаружен запущенный nanoCAD.' + #13#10 +
           'Пожалуйста, закройте nanoCAD перед установкой.', mbError, MB_OK);
    Result := False;
  end;
end;

// Запуск деинсталлятора: проверяем, что nanoCAD закрыт
function InitializeUninstall: Boolean;
begin
  Result := True;
  
  // Пока nanoCAD запущен — показываем предупреждение с жёлтым треугольником
  while IsNanoCADRunning do
  begin
    if MsgBox('Невозможно продолжить удаление.' + #13#10#13#10 +
              'Обнаружен запущенный nanoCAD. Закройте его и нажмите "Повторить".',
              mbError, MB_RETRYCANCEL) = IDCANCEL then
    begin
      Result := False;
      Exit;
    end;
  end;
end;

// Поиск папки nanoCAD через реестр, иначе — путь по умолчанию
function GetNanoCADPath(Param: String): String;
var
  RegPath: String;
begin
  RegPath := 'Software\Nanosoft\nanoCAD x64\25.0';
  if RegQueryStringValue(HKEY_LOCAL_MACHINE, RegPath, 'InstallDir', Result) then
  begin
    if Copy(Result, Length(Result), 1) = '\' then
      Delete(Result, Length(Result), 1);
  end
  else
    Result := 'C:\Program Files\Nanosoft\nanoCAD x64 25.0';
end;

// Заменяет все вхождения Placeholder на Replacement в файле
procedure ReplacePlaceholderInFile(FilePath, Placeholder, Replacement: String);
var
  Content: TStringList;
  i: Integer;
  Line: String;
begin
  if not FileExists(FilePath) then Exit;
  
  Content := TStringList.Create;
  try
    Content.LoadFromFile(FilePath);
    for i := 0 to Content.Count - 1 do
    begin
      Line := Content[i];
      StringChange(Line, Placeholder, Replacement);
      Content[i] := Line;
    end;
    Content.SaveToFile(FilePath);
  finally
    Content.Free;
  end;
end;

// Действия после копирования файлов (ssPostInstall)
procedure CurStepChanged(CurStep: TSetupStep);
var
  ConfigDir: String;
  ConfigFile, CfgIniLines: TStringList;
  WorkDir: String;
begin
  if CurStep = ssPostInstall then
  begin
    WorkDir := ExpandConstant('{app}');
    ConfigDir := ExpandConstant('{userappdata}\Nanosoft\nanoCAD x64 25.0\Config');
    
    // Заменяем плейсхолдер в .cfg на путь к иконкам
    ReplacePlaceholderInFile(
      ConfigDir + '\GOST_Automation.cfg',
      '{{ICONSPATH}}',
      WorkDir + '\Resources\Icons\'
    );
    
    ConfigFile := TStringList.Create;
    try
      if FileExists(ConfigDir + '\nanoCAD.cfg') then
        ConfigFile.LoadFromFile(ConfigDir + '\nanoCAD.cfg');
      if Pos('GOST_Automation.cfg', ConfigFile.Text) = 0 then
      begin
        ConfigFile.Add('#include "GOST_Automation.cfg"');
        ConfigFile.SaveToFile(ConfigDir + '\nanoCAD.cfg');
      end;
    finally
      ConfigFile.Free;
    end;

    CfgIniLines := TStringList.Create;
    try
      CfgIniLines.Add('');
      CfgIniLines.Add('[\Configuration\<<Default>>\Appload\Startup\app0]');
      CfgIniLines.Add('Loader=s' + WorkDir + '\NanoCAD.API.dll');
      CfgIniLines.Add('Type=sMGD');
      CfgIniLines.Add('Enabled=i1');

      ConfigFile := TStringList.Create;
      try
        if FileExists(ConfigDir + '\cfg.ini') then
          ConfigFile.LoadFromFile(ConfigDir + '\cfg.ini');
        ConfigFile.AddStrings(CfgIniLines);
        ConfigFile.SaveToFile(ConfigDir + '\cfg.ini');
      finally
        ConfigFile.Free;
      end;
    finally
      CfgIniLines.Free;
    end;
  end;
end;

// Действия при удалении (до стирания файлов)
procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
var
  ConfigDir: String;
  ConfigFile: TStringList;
  i: Integer;
begin
  if CurUninstallStep = usUninstall then
  begin
    ConfigDir := ExpandConstant('{userappdata}\Nanosoft\nanoCAD x64 25.0\Config');
    
    ConfigFile := TStringList.Create;
    try
      if FileExists(ConfigDir + '\nanoCAD.cfg') then
      begin
        ConfigFile.LoadFromFile(ConfigDir + '\nanoCAD.cfg');
        for i := ConfigFile.Count - 1 downto 0 do
          if Pos('GOST_Automation.cfg', ConfigFile[i]) > 0 then
            ConfigFile.Delete(i);
        ConfigFile.SaveToFile(ConfigDir + '\nanoCAD.cfg');
      end;
    finally
      ConfigFile.Free;
    end;

    ConfigFile := TStringList.Create;
    try
      if FileExists(ConfigDir + '\cfg.ini') then
      begin
        ConfigFile.LoadFromFile(ConfigDir + '\cfg.ini');
        i := 0;
        while i < ConfigFile.Count do
        begin
          if Pos('[\Configuration\<<Default>>\Appload\Startup\app0]', ConfigFile[i]) > 0 then
          begin          
            if (i > 0) and (Trim(ConfigFile[i - 1]) = '') then
            begin
              ConfigFile.Delete(i - 1);
              i := i - 1;
            end;
            ConfigFile.Delete(i);
            while (i < ConfigFile.Count) and (Pos('[', ConfigFile[i]) <> 1) do
              ConfigFile.Delete(i);
            Break;
          end;
          i := i + 1;
        end;
        ConfigFile.SaveToFile(ConfigDir + '\cfg.ini');
      end;
    finally
      ConfigFile.Free;
    end;

    if FileExists(ConfigDir + '\GOST_Automation.cfg') then
      DeleteFile(ConfigDir + '\GOST_Automation.cfg');
  end;
end;

// Обновление описания в зависимости от языка
procedure UpdateDescriptionLanguage;
begin
  if DescriptionLabel = nil then Exit;
  if Lowercase(ActiveLanguage) = 'russian' then
    DescriptionLabel.Caption := DescRussian
  else
    DescriptionLabel.Caption := DescEnglish;
end;

// Корректная отмена: при перезапуске — без подтверждения
procedure CancelButtonClick(CurPageID: Integer; var Cancel, Confirm: Boolean);
begin
  Cancel := True;
  Confirm := IsConfirm;
end;

// Инициализация элементов на странице выбора пути
procedure InitializeWizard;
begin 
  IsConfirm := True;

  // Массив языков (порядок совпадает с ComboBox)
  SetArrayLength(LangArray, 2);
  LangArray[0] := 'russian';
  LangArray[1] := 'english';
  
  // Метка "Язык установки"
  LanguageLabel := TLabel.Create(WizardForm);
  LanguageLabel.Parent := WizardForm.SelectDirPage;
  LanguageLabel.Caption := 'Installation language / Язык установки :';
  LanguageLabel.AutoSize := True;
  LanguageLabel.Left := 0;
  LanguageLabel.Top := 120;
  LanguageLabel.Font.Name := 'Segoe UI';
  LanguageLabel.Font.Size := 9;
  
  // Выпадающий список языков
  LanguageCombo := TComboBox.Create(WizardForm);
  LanguageCombo.Parent := WizardForm.SelectDirPage;
  LanguageCombo.Left := 0;
  LanguageCombo.Top := LanguageLabel.Top + LanguageLabel.Height + 5;
  LanguageCombo.Width := 200;
  LanguageCombo.Style := csDropDownList;
  LanguageCombo.Items.Add('Русский');
  LanguageCombo.Items.Add('English');
  LanguageCombo.ItemIndex := ActiveLangIndex;
  LanguageCombo.OnChange := @LanguageComboChange;
  
  // Описание надстройки
  DescriptionLabel := TLabel.Create(WizardForm);
  DescriptionLabel.Parent := WizardForm.SelectDirPage;
  DescriptionLabel.AutoSize := False;
  DescriptionLabel.WordWrap := True;
  DescriptionLabel.Width := WizardForm.SelectDirPage.Width;
  DescriptionLabel.Height := 80;
  DescriptionLabel.Left := 0;
  DescriptionLabel.Top := LanguageCombo.Top + LanguageCombo.Height + 20;
  DescriptionLabel.Font.Name := 'Segoe UI';
  DescriptionLabel.Font.Size := 9;
  DescriptionLabel.Alignment := taLeftJustify;
  
  UpdateDescriptionLanguage;
end;

// Настройка доступа к редактированию пути
procedure CurPageChanged(CurPageID: Integer);
begin
  if CurPageID = wpSelectDir then
  begin
    WizardForm.DirEdit.Enabled := True;
    WizardForm.DirBrowseButton.Enabled := True;
    UpdateDescriptionLanguage;
  end;
end;