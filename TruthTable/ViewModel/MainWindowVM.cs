using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TruthTable.Model;
using TruthTable.Model.NormalForms;

namespace TruthTable.ViewModel
{
    class MainWindowVM : INotifyPropertyChanged
    {
        #region Реализация интерфейса
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        #endregion

        private string inputExpression = "";
        public string InputExpression { get { return inputExpression; }
            set {
                inputExpression = value;
                OnPropertyChanged();
            } }

        private ValueTable valueTable;

        //команда построения функции
        private RelayCommand buildFunction;
        public RelayCommand BuildFunction { get
            {
                return buildFunction ?? (buildFunction = new RelayCommand(obj =>
                {
                    (string expression, bool isVector) = CheckAndRewriteExpression();

                    //задаем таблицу значений функции
                    if (isVector)
                        valueTable = new ValueTable(expression);
                    else
                        valueTable = new ValueTable(
                            new LogicalFunction(expression));
                    //Вызываем метод заполнения DataTable и отображаем datagrid
                    if (IsBuildValueTable)
                    {
                        FillDataGridValueTable();
                        DataGridVisibility = Visibility.Visible;
                    }

                    //построение сднф
                    if (IsBuildSdnf)
                    {
                        valueTable.LogicNormalForm = new SDNF();
                        //нужно вынести слово СДНФ в отдельный блок
                        SdnfExpression = "СДНФ:\t" + valueTable.GetNormalForm();
                    }

                    //построение скнф
                    if (IsBuildSknf)
                    {
                        //...
                        valueTable.LogicNormalForm = new SKNF();
                        SknfExpression = "СКНФ:\t" + valueTable.GetNormalForm();
                    }
                }));
            } }

        ///<summary>
        /// проверка корректности введенной функции и замена операторов на те, что обрабатывает LogicalExpression
        /// </summary>
        /// <returns>string: функция/вектор-функция <br/> bool: true - вектор-функция; false - обычная функция</returns>
        /// <exception cref="ArgumentException"></exception>
        private (string, bool) CheckAndRewriteExpression()
        {
            //копируем введенное выражение
            StringBuilder expression = new StringBuilder(inputExpression);

            //замена операторов
            (char, char)[] replacingOperators =
            {
                ('\u2194', '~'),
                ('\u2192', '>'),
                ('\u2190', '<'),
                ('\u2228', '+'),
                ('\u2227', '*'),
                ('\u00AC', '!'),
            };
            
            foreach (var replace in replacingOperators)
                expression = expression.Replace(replace.Item1.ToString(), replace.Item2.ToString());

            //проверка корректного ввода
            //проверка на соответствие вектор-функции
            bool isVector = true;
            for (int i = 0; i < expression.Length; i++)
                if (!char.IsDigit(expression[i])) isVector = false;

            //работаем с вектор функцией
            if (isVector)
            {
                if ((int)Math.Sqrt(expression.Length) % 2 == 0)
                    return (expression.ToString(), true);
                else
                    throw new ArgumentException("Функция задана неверно!");
            }
            else
            {
                //реализовать проверку для обычной функции
                return (expression.ToString(), false);
            }

        }

        #region Информация, выводимая в ResultPanel

        //сднф
        private string sdnfExpression;
        public string SdnfExpression { get => sdnfExpression; set
            {
                sdnfExpression = value;
                OnPropertyChanged();
            } }

        //скнф
        private string sknfExpression;
        public string SknfExpression { get => sknfExpression; set
            {
                sknfExpression = value;
                OnPropertyChanged(); } }

        //содержит таблицу значений функции для вывода на экран
        private DataView dataViewValueTable;
        public DataView DataViewValueTable { get => dataViewValueTable; set {
                dataViewValueTable = value;
                OnPropertyChanged(nameof(DataViewValueTable));
            } }
        #endregion

        //заполняет таблицу значений
        private void FillDataGridValueTable()
        {
            //DataView содержит в себе DataTable, поэтому можно работать с DataTable
            DataTable table = new DataTable();

            //добавление столбцов
            for (int i = 0; i < valueTable.AmountOfVariables;  i++)
                table.Columns.Add(new DataColumn(valueTable.Variables[i]));
            //столбец с результатом
            table.Columns.Add(new DataColumn("Result"));

            //добавление строк
            for (int i = 0; i < valueTable.MaxDecimalValue; i++)
            {
                //формирование массива, из которого будет заполнена строка
                List<object> row = new List<object>(valueTable.AmountOfVariables + 1);
                for (int j = 0; j < valueTable.AmountOfVariables; j++)
                    row.Add(valueTable[i].args[j]);
                row.Add((object)valueTable[i].value);

                table.Rows.Add(row.ToArray());
            }
            //формируем DataView на основании DataTable
            DataViewValueTable = table.AsDataView();
        }

        //видимость datagrid
        Visibility dataGridVisibility = Visibility.Hidden;
        public Visibility DataGridVisibility { get => dataGridVisibility; set {
                dataGridVisibility = value;
                OnPropertyChanged();
            } }

        //команда вывода содержимого кнопки в вводимое выражение
        private RelayCommand printSymbol;
        public RelayCommand PrintSymbol { get
            {
                return printSymbol ?? (printSymbol = new RelayCommand(obj =>
                {
                    InsertSymbToInputField((string)obj);
                }));
            } }

        private void InsertSymbToInputField(string symb)
        {
            int cursorPos = cursorPosition; //позиция курсора до вставки символа
            InputExpression = InputExpression.Insert(CursorPosition, symb);
            //указываем позицию с учетом вставленного символа
            CursorPosition = cursorPos + symb.Length;
            //возвращаем фокус в поле ввода
            IsFocused = true;
        }

        //хранит позицию курсора в строке ввода
        private int cursorPosition;
        public int CursorPosition { get => cursorPosition; set
            {
                cursorPosition = value;
                OnPropertyChanged();
            } }

        #region CheckBox'ы, свойства, к которым они привязаны
        //построение таблицы истинности (значений)
        private bool isBuildValueTable;
        public bool IsBuildValueTable { get { return isBuildValueTable; } set
            {
                isBuildValueTable = value;
                OnPropertyChanged();
            } }

        //построение СДНФ
        private bool isBuildSdnf;
        public bool IsBuildSdnf { get => isBuildSdnf; set
            {
                isBuildSdnf = value;
                OnPropertyChanged();
            } }

        //построение СКНФ
        private bool isBuildSknf;
        public bool IsBuildSknf { get => isBuildSknf; set
            {
                isBuildSknf = value;
                OnPropertyChanged();
            } }
        #endregion

        //свойство, необходимое для отслеживания фокуса у поля ввода
        bool _isFocused = false;
        public bool IsFocused
        {
            get { return _isFocused; }
            set
            {
                _isFocused = value;
                OnPropertyChanged(nameof(IsFocused));
            }
        }

    }
}

