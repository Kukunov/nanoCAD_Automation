using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NanoCAD.API.Models
{
    // Единая модель состояния для всего UI
    public class UIModel
    {
        #region Приватные поля

        private string _currentDrawing = "Нет открытого чертежа";
        private int _currentContour = 1;
        private int _nextElementNumber = 1;
        private int _elementNumber = 1;
        private string _selectedBlockCode = "PNSH";
        private string _selectedBlockName = "ПНЩ (Прибор на щите)";
        private string _typeDesignation = "TE";
        private string _position = "1-1";

        #endregion

        #region Свойства состояния

        // Текущий чертёж nanoCAD
        public string CurrentDrawing
        {
            get => _currentDrawing;
            set { _currentDrawing = value; NotifyStateChanged(); }
        }

        // Текущий контур автоматизации
        public int CurrentContour
        {
            get => _currentContour;
            set { _currentContour = value; NotifyStateChanged(); }
        }

        // Следующий номер элемента (из nanoCAD)
        public int NextElementNumber
        {
            get => _nextElementNumber;
            set { _nextElementNumber = value; NotifyStateChanged(); }
        }

        // Текущий номер элемента для вставки
        public int ElementNumber
        {
            get => _elementNumber;
            set { _elementNumber = value; NotifyStateChanged(); }
        }

        // Код выбранного блока (PNSH, PVSH, ...)
        public string SelectedBlockCode
        {
            get => _selectedBlockCode;
            set { _selectedBlockCode = value; NotifyStateChanged(); }
        }

        // Название выбранного блока (для отображения)
        public string SelectedBlockName
        {
            get => _selectedBlockName;
            set { _selectedBlockName = value; NotifyStateChanged(); }
        }

        // Обозначение типа элемента (ТИП)
        public string TypeDesignation
        {
            get => _typeDesignation;
            set { _typeDesignation = value?.ToUpperInvariant() ?? "TE"; NotifyStateChanged(); }
        }

        // Позиционное обозначение (ПОЗ)
        public string Position
        {
            get => _position;
            set { _position = value; NotifyStateChanged(); }
        }

        // Свойство, показывающее, запущен ли nanoCAD
        public bool IsNanoCADRunning
        {
            get => _currentDrawing != "Нет открытого чертежа" &&
                   _currentDrawing != "nanoCAD не запущен";
        }

        #endregion

        #region Данные для подбора компонентов (будущее расширение)

        // Выбранный производитель
        public string SelectedManufacturer { get; set; } = "ОВЕН";

        // Выбранная модель устройства
        public string SelectedDeviceModel { get; set; } = string.Empty;

        // Дополнительные параметры устройства
        public Dictionary<string, string> DeviceParameters { get; set; } = new();

        #endregion

        #region Данные для сметы (будущее расширение)

        // Список элементов сметы
        public List<EstimateItem> EstimateItems { get; set; } = new();

        #endregion

        #region События

        // Событие изменения состояния (вызывается при любом изменении)
        public event Action? StateChanged;

        // Событие изменения конкретного свойства
        public event Action<string>? PropertyChanged;

        #endregion

        #region Методы уведомления

        // Уведомить подписчиков об изменении состояния
        public void NotifyStateChanged([CallerMemberName] string? propertyName = null)
        {
            StateChanged?.Invoke();

            if (propertyName != null)
            {
                PropertyChanged?.Invoke(propertyName);
            }
        }

        #endregion
    }

    // Элемент сметы
    public class EstimateItem
    {
        // Позиционное обозначение на схеме
        public string Position { get; set; } = string.Empty;

        // Наименование устройства
        public string DeviceName { get; set; } = string.Empty;

        // Модель устройства
        public string Model { get; set; } = string.Empty;

        // Цена за единицу
        public decimal Price { get; set; }

        // Количество
        public int Quantity { get; set; } = 1;

        // Сумма (вычисляемое свойство)
        public decimal Total => Price * Quantity;
    }
}