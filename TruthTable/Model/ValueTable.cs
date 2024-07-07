using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TruthTable.Model.NormalForms;

namespace TruthTable.Model
{
    /// <summary>
    /// Таблица значений заданной функции
    /// </summary>
    public class ValueTable
    {
        LogicalFunction function; //логическая функция
        
        /// <summary>
        /// Логическая функция
        /// </summary>
        public string Expression { get => function.InFix; }

        public ArgValuePair this[int i]
        {
            get { return table[i]; }
        }
        ArgValuePair[] table; //таблица значений

        /// <summary>
        /// Количество переменных у заданной функции
        /// </summary>
        public int AmountOfVariables { get => function.variables.Length; }

        //максимальное числовое значение в десятичной системе, построенное из набора переменных = 1
        public int MaxDecimalValue { get => (int)Math.Pow(2, AmountOfVariables); }

        /// <summary>
        /// Переменные
        /// </summary>
        public string[] Variables {  get => function.variables; }

        public ValueTable(LogicalFunction function)
        {
            this.function = function;
            table = new ArgValuePair[(int)Math.Pow(2, AmountOfVariables)];
            CreateTable();
        }

        ///генерация таблицы значений <summary>
        /// </summary>
        private void CreateTable()
        { 
            int maxValue = MaxDecimalValue; //макс значение в десятичной системе

            for (int i = 0; i < maxValue; i++)
            {
                byte[] args = ConvertToBinaryArray(i, AmountOfVariables);
                byte result = function.FunctionValue(args);
                table[i] = new ArgValuePair(args, result);
            }
        }

        ///<summary>создание таблицы значений на основе вектор-функции 
        /// </summary>
        /// <param name="vectorFunction"></param>
        public ValueTable(string vectorFunction)
        {
            CreateTable(vectorFunction);
            LogicNormalForm = new SDNF();
            this.function = new LogicalFunction(GetNormalForm());
        }

        private void CreateTable(string vectorFunction)
        {
            int maxValue = vectorFunction.Length;
            //получаем количество переменных
            int amountVariables = (int)Math.Log(maxValue, 2);

            //преобразуем строку 0 и 1 в массив 0 и 1
            byte[] bytes = vectorFunction.ToCharArray().Select(c => (byte)(c - '0')).ToArray();
            table = new ArgValuePair[maxValue];
            //заполняем таблицу значений
            for (int i = 0; i < maxValue; i++)
            {
                byte[] args = ConvertToBinaryArray(i, amountVariables);
                table[i] = new ArgValuePair(args, bytes[i]);
            }
        }

        ///<summary>
        /// алгоритм, применяющийся к логической функции, чтобы получить конкретную форму логической функции,
        /// </summary>
        //например, СДНФ, МДНФ и т.п.
        public INormalizationForm LogicNormalForm { get; set; }

        //метод для генерации нормальной формы
        public string GetNormalForm()
        {
            return LogicNormalForm.GetNormalForm(this.table);
        }

        /// <summary>
        /// преобразование десятичного числа в двоичное в виде массива битов
        /// </summary>
        /// <param name="number">число, которое необходимо получить в двоичном виде</param>
        /// <param name="amountVariables">количество переменных, т.е. количество разрядов у числа на выходе</param>
        /// <returns></returns>
        private byte[] ConvertToBinaryArray(int number, int amountVariables)
        {
            byte[] binaryArray = new byte[amountVariables];

            //число, переведенное в двоичную систему с количеством разрядов <= amountVariables
            char[] binary = Convert.ToString(number, 2).ToCharArray();

            //количество разрядов, которых не хватает в массиве символов
            int delta = binaryArray.Length - binary.Length;

            for (int i = 0; i < binary.Length; i++)
            {
                //в конечном массиве делаем сдвиг вправо на разницу размеров двух массивов
                binaryArray[delta + i] = (byte)(binary[i] - '0');
            }

            return binaryArray;
        }
    }

    /// <summary>
    /// Представляет собой набор аргументов и значение функции
    /// </summary>
    public class ArgValuePair
    {
        public readonly byte[] args;
        public readonly byte value;

        public ArgValuePair(byte[] args, byte value)
        {
            this.args = args;
            this.value = value;
        }
    }
}
