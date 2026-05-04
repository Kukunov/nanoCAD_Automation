using System.Text.Json;
using Teigha.DatabaseServices;
using NanoCAD.API.Models;

namespace NanoCAD.API.Data
{
    /// <summary>
    /// Класс для сохранения и загрузки состояния контуров в XData чертежа
    /// </summary>
    public static class ContourStateStore
    {
        // Уникальное имя приложения для XData
        private const string AppName = "NanoCAD_GOST_ContourState";

        // Сохранить состояние контуров в чертёж
        public static void SaveState(Database db, ContourState state)
        {
            using (var tr = db.TransactionManager.StartTransaction())
            {
                // Получаем словарь расширений чертежа
                var nod = (DBDictionary)tr.GetObject(db.NamedObjectsDictionaryId, OpenMode.ForWrite);

                // Создаём или получаем наш словарь
                DBDictionary contourDict;
                if (nod.Contains(AppName))
                {
                    contourDict = (DBDictionary)tr.GetObject(nod.GetAt(AppName), OpenMode.ForWrite);
                }
                else
                {
                    contourDict = new DBDictionary();
                    nod.SetAt(AppName, contourDict);
                    tr.AddNewlyCreatedDBObject(contourDict, true);
                }

                // Сериализуем состояние в JSON
                string json = JsonSerializer.Serialize(state);

                // Сохраняем JSON в XRecord
                Xrecord xrec;
                if (contourDict.Contains("State"))
                {
                    xrec = (Xrecord)tr.GetObject(contourDict.GetAt("State"), OpenMode.ForWrite);
                }
                else
                {
                    xrec = new Xrecord();
                    contourDict.SetAt("State", xrec);
                    tr.AddNewlyCreatedDBObject(xrec, true);
                }

                // Записываем данные
                xrec.Data = new ResultBuffer(new TypedValue((int)DxfCode.Text, json));

                tr.Commit();
            }
        }

        // Загрузить состояние контуров из чертежа
        public static ContourState LoadState(Database db)
        {
            var state = new ContourState();

            using (var tr = db.TransactionManager.StartTransaction())
            {
                var nod = (DBDictionary)tr.GetObject(db.NamedObjectsDictionaryId, OpenMode.ForRead);

                if (nod.Contains(AppName))
                {
                    var contourDict = (DBDictionary)tr.GetObject(nod.GetAt(AppName), OpenMode.ForRead);

                    if (contourDict.Contains("State"))
                    {
                        var xrec = (Xrecord)tr.GetObject(contourDict.GetAt("State"), OpenMode.ForRead);

                        if (xrec.Data != null)
                        {
                            foreach (TypedValue tv in xrec.Data)
                            {
                                if (tv.TypeCode == (int)DxfCode.Text)
                                {
                                    string json = tv.Value.ToString();
                                    var loadedState = JsonSerializer.Deserialize<ContourState>(json);
                                    if (loadedState != null)
                                    {
                                        state = loadedState;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }

                tr.Commit();
            }

            return state;
        }

        // Проверить, есть ли сохранённое состояние
        public static bool HasSavedState(Database db)
        {
            using (var tr = db.TransactionManager.StartTransaction())
            {
                var nod = (DBDictionary)tr.GetObject(db.NamedObjectsDictionaryId, OpenMode.ForRead);
                bool hasState = nod.Contains(AppName);
                tr.Commit();
                return hasState;
            }
        }
    }
}