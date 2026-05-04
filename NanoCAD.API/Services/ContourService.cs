using Teigha.DatabaseServices;
using NanoCAD.API.Models;
using NanoCAD.API.Data;

namespace NanoCAD.API.Services
{
    /// <summary>
    /// Сервис для управления контурами автоматизации
    /// </summary>
    public class ContourService
    {
        private Database _db;
        private ContourState _state;

        public ContourService(Database db)
        {
            _db = db;
            _state = ContourStateStore.LoadState(db);
        }

        // Получить все контуры и их счётчики
        public Dictionary<int, int> GetAllContours()
        {
            return new Dictionary<int, int>(_state.ElementCounters);
        }

        // Получить текущий номер контура
        public int GetCurrentContour()
        {
            return _state.CurrentContour;
        }

        // Получить последнее использованное обозначение типа
        public string GetLastTypeDesignation()
        {
            return _state.LastTypeDesignation;
        }

        // Получить следующее позиционное обозначение
        public string GetNextPosition()
        {
            string position = _state.GetNextPosition();
            SaveState();
            return position;
        }

        // Получить следующее позиционное обозначение для указанного контура
        public string GetNextPosition(int contourNumber)
        {
            string position = _state.GetPosition(contourNumber, _state.GetNextElementNumber(contourNumber));
            SaveState();
            return position;
        }

        // Получить следующую позицию без увеличения счётчика (для предпросмотра)
        public string GetNextPositionPreview()
        {
            int currentContour = _state.CurrentContour;
            int currentCounter = _state.ElementCounters.ContainsKey(currentContour)
                ? _state.ElementCounters[currentContour]
                : 0;

            return $"{currentContour}-{currentCounter + 1}";
        }

        // Получить информацию о текущем состоянии (для отображения)
        public string GetStatusInfo()
        {
            int currentContour = _state.CurrentContour;
            int currentCounter = _state.ElementCounters.ContainsKey(currentContour)
                ? _state.ElementCounters[currentContour]
                : 0;

            return $"Контур: {currentContour}, Следующий элемент: {currentContour}-{currentCounter + 1}";
        }

        // Установить текущий контур
        public void SetCurrentContour(int contourNumber)
        {
            if (contourNumber < 1)
                throw new System.ArgumentException("Номер контура должен быть больше 0");

            _state.CurrentContour = contourNumber;

            // Инициализируем счётчик для нового контура, если его нет
            if (!_state.ElementCounters.ContainsKey(contourNumber))
            {
                _state.ElementCounters[contourNumber] = 0;
            }

            SaveState();
        }

        // Сохранить последнее использованное обозначение типа
        public void SetLastTypeDesignation(string typeDesignation)
        {
            if (!string.IsNullOrWhiteSpace(typeDesignation))
            {
                _state.LastTypeDesignation = typeDesignation.ToUpperInvariant();
                SaveState();
            }
        }

        // Сбросить счётчик текущего контура
        public void ResetCurrentContour()
        {
            _state.ResetContour(_state.CurrentContour);
            SaveState();
        }

        // Сбросить счётчик указанного контура
        public void ResetContour(int contourNumber)
        {
            _state.ResetContour(contourNumber);
            SaveState();
        }

        // Сохранить состояние в чертёж
        private void SaveState()
        {
            ContourStateStore.SaveState(_db, _state);
        }
    }
}