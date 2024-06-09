using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruthTable.Model
{
    /// <summary>
    /// Таблица значений заданной функции
    /// </summary>
    public class ValueTable
    {
        LogicalFunction function; //логическая функция

        public ArgValuePair this[int i]
        {
            get { return table[i]; }
        }
        ArgValuePair[] table; //таблица значений

        /// <summary>
        /// Количество переменных у заданной функции
        /// </summary>
        public int AmountOfVariables { get => function.variables.Length; }

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

        //генерация таблицы значений
        private void CreateTable()
        {
            int maxValue = (int)Math.Pow(2, AmountOfVariables); //макс значение в десятичной системе

            for (int i = 0; i < maxValue; i++)
            {
                byte[] args = ConvertToBinaryArray(i, AmountOfVariables);
                byte result = function.FunctionValue(args);
                table[i] = new ArgValuePair(args, result);
            }
        }

        //public ValueTable(string vectorFunction)
        //{
        //    CreateTable(vectorFunction);
        //}

        //private void CreateTable(string vectorFunction)
        //{
        //    int maxValue = vectorFunction.Length;
        //}

        //преобразование десятичного числа в двоичное в виде массива битов
        private byte[] ConvertToBinaryArray(int number, int digits)
        {
            byte[] binaryArray = new byte[digits];

            //число, переведенное в двоичную систему с количеством разрядов <= digits
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
