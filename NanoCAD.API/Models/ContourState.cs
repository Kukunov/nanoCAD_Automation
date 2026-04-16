namespace NanoCAD.API.Models
{
    // Состояние контура автоматизации
    public class ContourState
    {
        // Номер текущего контура (1, 2, 3...)
        public int CurrentContour { get; set; } = 1;

        // Счётчики элементов для каждого контура
        // Ключ - номер контура, Значение - последний использованный номер элемента
        public Dictionary<int, int> ElementCounters { get; set; } = new();

        // Получить следующий номер элемента для указанного контура
        public int GetNextElementNumber(int contourNumber)
        {
            if (!ElementCounters.ContainsKey(contourNumber))
            {
                ElementCounters[contourNumber] = 0;
            }

            ElementCounters[contourNumber]++;
            return ElementCounters[contourNumber];
        }

        // Получить следующее позиционное обозначение для текущего контура
        public string GetNextPosition()
        {
            int nextElement = GetNextElementNumber(CurrentContour);
            return $"{CurrentContour}-{nextElement}";
        }

        // Получить позиционное обозначение для указанного контура и элемента
        public string GetPosition(int contourNumber, int elementNumber)
        {
            return $"{contourNumber}-{elementNumber}";
        }

        // Сбросить счётчик для контура
        public void ResetContour(int contourNumber)
        {
            ElementCounters[contourNumber] = 0;
        }
    }
}